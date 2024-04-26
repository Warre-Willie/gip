using MQTTnet;
using MQTTnet.Client;
using System.Threading;
using System;
using System.Threading.Tasks;

namespace crowd_management.classes
{
    public class MqttRepository
    {
        private readonly IMqttClient _mqttClient;
				private const string Broker = "192.168.0.101";
				private readonly string username;
        private readonly string password;

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
                        //.WithCredentials(username, password)
                        //.WithTls(tls =>
                        //{
                        //    tls.UseTls = true;
                        //    tls.AllowUntrustedCertificates = true;
                        //})
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
    }
}