/*
* File: Program.cs
* Author: Warre Willeme & Jesse UijtdeHaag
* Date: 20-05-2024
* Description: This file contains the and logic to update the heatmap and send disconnect notifications live.
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;

namespace crowd_management_live;

static class Program
{
	private static IHubProxy _hubProxy;
	private static HubConnection _hubConnection;
	private static IMqttClient _mqttClient;
	private static DbRepository _dbRepository;

	// MQTT configuration
	private static readonly string _mqttBrokerHost = "192.169.0.101";
	private static readonly string _mqttUsername = "";
	private static readonly string _mqttPassword = "";
	private static readonly string _mqttTopic = "gip/disconnected";

	static async Task Main()
	{
		// SignalR configuration
		var signalrUrl = "https://localhost:44384/"; // Replace XXXX with the port of your ASP.NET WebForms application
		var hubName = "LiveUpdateHub";

		// Create and open the DbRepository connection
		_dbRepository = new DbRepository();

		// Create MQTT client
		var mqttFactory = new MqttFactory();
		_mqttClient = mqttFactory.CreateMqttClient();
		await ConnectToMqttBroker(_mqttUsername, _mqttPassword);
		await SubscribeToTopic(_mqttTopic);

		_mqttClient.ApplicationMessageReceivedAsync += HandleMqttMessageReceived;

		// Connect to SignalR Hub
		_hubConnection = new HubConnection(signalrUrl);
		_hubProxy = _hubConnection.CreateHubProxy(hubName);
		await _hubConnection.Start();

		Console.WriteLine("Connected to MQTT broker and SignalR Hub. Listening for MQTT messages...");

		while (true)
		{
			await Task.Delay(3000);
			try
			{
				await _hubProxy.Invoke("Send", "HeatMap", FetchHeatMapData());
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
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

	private static async Task ConnectToMqttBroker(string username, string password)
	{
		if (!_mqttClient.IsConnected)
		{
			try
			{
				var mqttClientOptions = new MqttClientOptionsBuilder()
					.WithTcpServer(_mqttBrokerHost)
					.WithCredentials(username, password)
					.WithTls(tls =>
					{
						tls.UseTls = true;
						tls.AllowUntrustedCertificates = true;
					})
					.WithCleanSession()
					.Build();

				await _mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
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

	private static async Task SubscribeToTopic(string topic)
	{
		var mqttSubscribeOptions = new MqttClientSubscribeOptionsBuilder()
			.WithTopicFilter(f => f.WithTopic(topic))
			.Build();

		await _mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
		Console.WriteLine($"Subscribed to topic: {topic}");
	}

	private static async Task HandleMqttMessageReceived(MqttApplicationMessageReceivedEventArgs args)
	{
		string message = args.ApplicationMessage.ConvertPayloadToString();
		dynamic jsonMessage = JsonConvert.DeserializeObject(message);

		Console.WriteLine(jsonMessage?["name"]);
		// Send MQTT message to SignalR Hub
		string notificaton = $"Verbinding met <b>{jsonMessage?["name"]}</b> is verbroken";
		await _hubProxy.Invoke("Send", "Notification", notificaton);
	}

	private class ZoneData
	{
		public string Name { get; set; }
		public double Percentage { get; set; }
		public bool Lockdown { get; set; }
		public string Color { get; set; }
	}
}