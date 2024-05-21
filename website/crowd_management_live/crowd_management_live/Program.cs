using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using crowd_management_live;
using Microsoft.AspNet.SignalR.Client;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;

namespace crowd_management_live
{
	class Program
	{
		private static IHubProxy _hubProxy;
		private static HubConnection _hubConnection;
		private static IMqttClient mqttClient;
		private static DbRepository dbRepository;

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

			// Create and open the DbRepository connection
			dbRepository = new DbRepository();

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
				await Task.Delay(3000);
				await _hubProxy.Invoke("Send", "HeatMap", FetchHeatMapData());
			}
		}

		static string FetchHeatMapData()
		{
			string query = "SELECT * FROM zones";
			DataTable zones = dbRepository.SqlExecuteReader(query);

			var heatMapData = new Dictionary<string, ZoneData>
						{
								{ "1", new ZoneData() },
								{ "2", new ZoneData() },
								{ "3", new ZoneData() },
								{ "4", new ZoneData() }
						};

			foreach (DataRow row in zones.Rows)
			{
				int zoneId = Convert.ToInt32(row["id"]);
				double percentage = -1;
				bool lockdown = Convert.ToBoolean(row["lockdown"]);

				// Formatting zone data
				if (!string.IsNullOrEmpty(row["people_count"].ToString()) && !string.IsNullOrEmpty(row["max_people"].ToString()))
				{
					percentage = Convert.ToDouble(row["people_count"]) / Convert.ToDouble(row["max_people"]) * 100;
					percentage = Math.Round(percentage, 2);
				}

				string color = "link";
				switch (row["barometer_color"].ToString())
				{
					case "green":
						color = "success";
						break;
					case "orange":
						color = "warning";
						break;
					case "red":
						color = "danger";
						break;
					default:
						color = "link";
						break;
				}

				heatMapData[zoneId.ToString()] = new ZoneData
				{
					Name = row["name"].ToString(),
					Color = color,
					Percentage = percentage,
					Lockdown = lockdown
				};
			}

			return JsonConvert.SerializeObject(heatMapData);
		}

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

		private class ZoneData
		{
			public string Name { get; set; }
			public double Percentage { get; set; }
			public bool Lockdown { get; set; }
			public string Color { get; set; }
		}
	}
}
