/*
* File: MqttRepository.cs
* Author: Warre Willeme & Jesse UijtdeHaag
* Date: 12-05-2024
* Description: This file contains the MqttRepository class. This class is used to connect to the MQTT broker and publish messages.
*/

using MQTTnet;
using MQTTnet.Client;
using System.Threading;
using System;
using System.Threading.Tasks;

namespace crowd_management.classes;

public class MqttRepository
{
	#region Variables and constands

	private readonly IMqttClient _mqttClient;
	private const string Broker = "192.168.80.81";
	private const string Username = "gip";
	private const string Password = "gip-WJ";

	#endregion

	#region Methods

	public MqttRepository()
	{
		var factory = new MqttFactory();
		_mqttClient = factory.CreateMqttClient();
		if (!_mqttClient.IsConnected)
		{
			try
			{
				var options = new MqttClientOptionsBuilder()
					.WithTcpServer(Broker)
					.WithCredentials(Username, Password)
					.WithCleanSession()
					.Build();

				_mqttClient.ConnectAsync(options, CancellationToken.None).Wait();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
	}

	public void Dispose()
	{
		if (_mqttClient.IsConnected)
		{
			_mqttClient.DisconnectAsync().Wait();
		}
	}

	public async Task PublishAsync(string topic, string payload)
	{
		var message = new MqttApplicationMessageBuilder()
			.WithTopic(topic)
			.WithPayload(payload)
			.Build();

		await _mqttClient.PublishAsync(message, CancellationToken.None);
	}

	#endregion
}