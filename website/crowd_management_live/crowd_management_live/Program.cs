using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using MQTTnet;
using MQTTnet.Client;

namespace crowd_management_live
{
	class Program
	{
		private static IHubProxy _hubProxy;
		private static HubConnection _hubConnection;
		private static IMqttClient mqttClient;

		// MQTT configuration
		public static string mqttBrokerHost = "567c45d531f2488ebee03bbbf8b02f1a.s1.eu.hivemq.cloud";
		public static string mqttUsername = "Willeme";
		public static string mqttPassword = "VickW2607-hi";
		public static string mqttTopic = "SignalR";

		static async Task Main(string[] args)
		{
			// SignalR configuration
			var signalrUrl = "https://localhost:44384/"; // Replace XXXX with the port of your ASP.NET WebForms application
			var hubName = "LiveUpdateHub";

			// Create MQTT client
			var mqttFactory = new MqttFactory();
			mqttClient = mqttFactory.CreateMqttClient();
			await ConnectToMqttBroker(mqttUsername, mqttPassword);

			await SubscribeToTopic(mqttTopic);
			// Connect to MQTT broker
			mqttClient.ApplicationMessageReceivedAsync += HandleMqttMessageReceived;


			// Connect to SignalR Hub
			_hubConnection = new HubConnection(signalrUrl);
			_hubProxy = _hubConnection.CreateHubProxy(hubName);
			await _hubConnection.Start();

			Console.WriteLine("Connected to MQTT broker and SignalR Hub. Listening for MQTT messages...");

			while (true)
			{
				//await Task.Delay(1000); // Adjust delay as needed
			}
		}

		[Obsolete]
		public static async Task ConnectToMqttBroker(string username, string password)
		{
			if (!mqttClient.IsConnected)
			{
				var mqttClientOptions = new MqttClientOptionsBuilder()
					.WithTcpServer(mqttBrokerHost)
					.WithCredentials(username, password)
					.WithTls(tls =>
					{
						tls.UseTls = true;
						tls.AllowUntrustedCertificates = true;

					})
					.WithTlsOptions(o => o.WithCertificateValidationHandler(_ => true)) // Accept all certificates
					.Build();

				await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
				Console.WriteLine("Connected to the MQTT broker with TLS encryption and accepting all certificates.");
			}
			else
			{
				Console.WriteLine("Already connected to the MQTT broker.");
			}
		}

		public static async Task SubscribeToTopic(string topic)
		{
			var mqttSubscribeOptions = new MqttClientSubscribeOptionsBuilder()
				.WithTopicFilter(f => f.WithTopic(topic))
				.Build();

			await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
			Console.WriteLine($"Subscribed to topic: {topic}");
		}

		private static async Task HandleMqttMessageReceived(MqttApplicationMessageReceivedEventArgs args)
		{
			var message = args.ApplicationMessage.ConvertPayloadToString();
			Console.WriteLine($"Received MQTT message: {message}");

			// Send MQTT message to SignalR Hub
			await _hubProxy.Invoke("Send", "Console App", message);
		}
	}
}