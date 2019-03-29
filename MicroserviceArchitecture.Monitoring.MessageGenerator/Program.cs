using MicroserviceArchitecture.Monitoring.Core;
using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MicroserviceArchitecture.MessageGenerator
{
    class Program
    {
        private static EventHubClient _eventHubClient;
        private static ILogger _logger;

        static void Main(string[] args)
        {
            var continueSendingTestMessage = true;
            var timeToWaitForNextSending = 15000;

            var configuration = new ConfigurationProvider().CreateConfiguration();
            var connectionString = configuration[InfrastructureConfigurationKeys.EventHubSendConnectionString];

            var logEventLevelSwitch =
                new LoggingProvider().ConvertStringToLoggingLevelSwitch(
                    configuration[ApplicationConfigurationKeys.LogEventLevel]);

            var logger = new LoggingProvider().CreateLogger(configuration, logEventLevelSwitch,
                System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().ToString());

            _logger = logger;

            var connectionStringBuilder = new EventHubsConnectionStringBuilder(connectionString);
            _eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            while (continueSendingTestMessage)
            {
                var testMessage = CreateTestMessage();
                var encodedTestMessage = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(testMessage));

                SendMessagesToEventHub(encodedTestMessage).Wait();
                System.Threading.Thread.Sleep(timeToWaitForNextSending);
            }
        }

        private static async Task SendMessagesToEventHub(byte[] byteMessage)
        {
            try
            {
                await _eventHubClient.SendAsync(new EventData(byteMessage));
                _logger.Information("Sent json message to eventhub");
            }
            catch (Exception e)
            {
                _logger.Information($"There was an error sending messages to the eventhub: {e}");
                Log.CloseAndFlush();
                throw;
            }
        }

        private static DataMessage CreateTestMessage()
        {
            var sampleMessage = new DataMessage
            {
                Id = "100",
                Value = "test"
            };

            return sampleMessage;
        }
    }

    public class DataMessage
    {
        public string Id { get; set; }
        public string Value { get; set; }
    }

    public class Foo
    {
        public byte[] Data { get; set; }
    }
}
