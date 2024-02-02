using MQTTnet.Client;
using MQTTnet;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet.Protocol;

namespace NET_Framwork_MQTT_test
{
    internal class Program
    {
        private static IMqttClient mqttClient;

        public static string host_url = "url";
        public static string username = "username";
        public static string password = "password";

        public static async Task Main(string[] args)
        {
            // Initialize the MQTT client
            mqttClient = new MqttFactory().CreateMqttClient();

            // Connect to the MQTT broker with TLS encryption and accepting all certificates
            await ConnectToMqttBroker(username, password);

            // Subscribe to a topic
            await SubscribeToTopic("gitok/test");

            // Add the event handler for received messages
            mqttClient.ApplicationMessageReceivedAsync += HandleReceivedMessage;

            // Publish and listen for messages
            await HandlePublishAndSubscribe();

            // Disconnect from the MQTT broker
            await mqttClient.DisconnectAsync();
        }

        [Obsolete]
        public static async Task ConnectToMqttBroker(string username, string password)
        {
            if (!mqttClient.IsConnected)
            {
                var mqttClientOptions = new MqttClientOptionsBuilder()
                    .WithTcpServer(host_url)
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

        public static async Task HandlePublishAndSubscribe()
        {
            while (true)
            {
                Console.WriteLine("Enter a message to publish (or 'exit' to quit): ");
                var input = Console.ReadLine();

                if (input.ToLower() == "exit")
                {
                    break;
                }

                // Publish the message
                await PublishApplicationMessage("gitok/test", input);
            }
        }

        public static async Task PublishApplicationMessage(string topic, string content)
        {
            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(content)
                .Build();

            await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);
            Console.WriteLine($"Published message to topic {topic}: {content}");
        }

        public static Task HandleReceivedMessage(MqttApplicationMessageReceivedEventArgs e)
        {
            Console.WriteLine($"Received message: {e.ApplicationMessage.ConvertPayloadToString()}");
            return Task.CompletedTask;
        }
    }
}
