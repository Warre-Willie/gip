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
		private static MqttRepository _mqttRepository;
		private static DbRepository _dbRepository;

		// MQTT configuration
		private static readonly string _mqttBrokerHost = "567c45d531f2488ebee03bbbf8b02f1a.s1.eu.hivemq.cloud";
		private static readonly string _mqttUsername = "Willeme";
		private static readonly string _mqttPassword = "VickW2607-hi";
		private static readonly string _mqttTopic = "gip/disconnected";

		static async Task Main(string[] args)
		{
			// SignalR configuration
			var signalrUrl = "https://localhost:44384/"; // Replace XXXX with the port of your ASP.NET WebForms application
			var hubName = "LiveUpdateHub";

			// Create and open the DbRepository connection
			_dbRepository = new DbRepository();

			// Create MQTT client
			_mqttRepository = new MqttRepository(_mqttBrokerHost, _mqttUsername, _mqttPassword);
			await _mqttRepository.ConnectAsync();
			await _mqttRepository.SubscribeAsync(_mqttTopic);

			// Connect to SignalR Hub
			_hubConnection = new HubConnection(signalrUrl);
			_hubProxy = _hubConnection.CreateHubProxy(hubName);
			await _hubConnection.Start();

			Console.WriteLine("Connected to MQTT broker and SignalR Hub. Listening for MQTT messages...");

			_mqttRepository.MessageReceived += async (message) =>
			{
				Console.WriteLine($"Received MQTT message: {message}");
				await _hubProxy.Invoke("Send", "Console App", message);
			};

			while (true)
			{
				await Task.Delay(3000);
				await _hubProxy.Invoke("Send", "HeatMap", FetchHeatMapData());
			}
		}

		static string FetchHeatMapData()
		{
			string query = "SELECT * FROM zones";
			DataTable zones = _dbRepository.SqlExecuteReader(query);

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

				string color;
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

		private class ZoneData
		{
			public string Name { get; set; }
			public double Percentage { get; set; }
			public bool Lockdown { get; set; }
			public string Color { get; set; }
		}
	}

	/*
	* File: MqttRepository.cs
	* Author: Warre Willeme & Jesse UijtdeHaag
	* Date: 12-05-2024
	* Description: This file contains the MqttRepository class. This class is used to connect to the MQTT broker and publish messages.
	*/
	public class MqttRepository
	{
		#region Variables and constants

		private readonly IMqttClient _mqttClient;
		private readonly string _broker;
		private readonly string _username;
		private readonly string _password;

		#endregion

		#region Events

		public event Func<string, Task> MessageReceived;

		#endregion

		#region Methods

		public MqttRepository(string broker, string username, string password)
		{
			_broker = broker;
			_username = username;
			_password = password;
			var factory = new MqttFactory();
			_mqttClient = factory.CreateMqttClient();
			_mqttClient.ApplicationMessageReceivedAsync += HandleMessageReceived;
		}

		public async Task ConnectAsync()
		{
			if (!_mqttClient.IsConnected)
			{
				try
				{
					var options = new MqttClientOptionsBuilder()
							.WithTcpServer(_broker)
							.WithCredentials(_username, _password)
							.WithTls(tls =>
							{
								tls.UseTls = true;
								tls.AllowUntrustedCertificates = true;
							})
							.WithCleanSession()
							.Build();

					await _mqttClient.ConnectAsync(options, CancellationToken.None);
					Console.WriteLine("Connected to the MQTT broker with TLS encryption and accepting all certificates.");
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
			else
			{
				Console.WriteLine("Already connected to the MQTT broker.");
			}
		}

		public async Task SubscribeAsync(string topic)
		{
			var mqttSubscribeOptions = new MqttClientSubscribeOptionsBuilder()
					.WithTopicFilter(f => f.WithTopic(topic))
					.Build();

			await _mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
			Console.WriteLine($"Subscribed to topic: {topic}");
		}

		public async Task PublishAsync(string topic, string payload)
		{
			var message = new MqttApplicationMessageBuilder()
					.WithTopic(topic)
					.WithPayload(payload)
					.Build();

			await _mqttClient.PublishAsync(message, CancellationToken.None);
		}

		private async Task HandleMessageReceived(MqttApplicationMessageReceivedEventArgs args)
		{
			var message = args.ApplicationMessage.ConvertPayloadToString();
			if (MessageReceived != null)
			{
				await MessageReceived.Invoke(message);
			}
		}

		public void Dispose()
		{
			if (_mqttClient.IsConnected)
			{
				_mqttClient.DisconnectAsync().Wait();
			}
		}

		#endregion
	}
}
