using MicroserviceArchitecture.Monitoring.Watchdog.MetricServer;
using Serilog;
using System;

namespace MicroserviceArchitecture.Monitoring.Watchdog
{
    public class SemanticChecker : ISemanticChecker
    {
        private readonly IMonitoringMetricServer _metricServer;
        private readonly IServiceProxy _serviceProxy;
        private static ILogger _logger;

        public SemanticChecker(IMonitoringMetricServer metricServer, IServiceProxy serviceProxy, ILogger logger)
        {
            _metricServer = metricServer ?? throw new ArgumentNullException(nameof(metricServer));
            _serviceProxy = serviceProxy ?? throw new ArgumentNullException(nameof(serviceProxy));
            _logger = logger;
        }

        public void DoSemanticCheck(bool firstAttemptToCheck)
        {
            DataResponse data;

            try
            {
                data = _serviceProxy.GetSemanticData();
            }
            catch (ServiceUnavailableException e)
            {
                var message = "Data service unavailable";

                if (e.InnerException != null)
                    message += ": " + e.InnerException.Message;

                _logger.Information(message);
                _metricServer.ReportIsBroken();
                return;
            }

            if (firstAttemptToCheck)
            {
                _metricServer.ReportInconclusive();
                return;
            }

            var expectedTimeToGetTheSemanticMessage = TimeSpan.FromSeconds(60);

            if (data == null)
            {
                _logger.Information("No message received, semantic message did not pass " +
                                  $"through in the expected time ({expectedTimeToGetTheSemanticMessage})");
                _metricServer.ReportIsBroken();
                return;
            }


            if (data.MessageCreatedAt == DateTime.MinValue)
            {
                _metricServer.ReportIsBroken();
                throw new Exception($"Received data.MessageCreatedAt as {DateTime.MinValue} which should NEVER be possible " +
                                    "(we always send a semantic message with message created at which is datetime.now!");
            }

            if ((DateTime.UtcNow - data.MessageCreatedAt) > expectedTimeToGetTheSemanticMessage)
            {
                _metricServer.ReportIsBroken();
                _logger.Information("Broken because not arrived in expected time -> MessageCreatedAt = " + data.MessageCreatedAt);
            }

            else
                _metricServer.ReportIsHealthy();
        }
    }

    public class DataResponse
    {
        public string Id { get; set; }
        public string Value { get; set; }
        public DateTime MessageCreatedAt { get; set; }
    }
}
