using System;
using Prometheus;
using Serilog;

namespace MicroserviceArchitecture.Monitoring.Watchdog.MetricServer
{
    public class MonitoringMetricServer : IDisposable, IMonitoringMetricServer
    {
        IMetricServer metricServer;
        private Gauge _healthGauge;
        private static ILogger _logger;

        public MonitoringMetricServer(ILogger logger)
        {
            metricServer = new Prometheus.MetricServer(port: 9200);
            _logger = logger;
        }

        public void Start()
        {
            metricServer.Start();

            _logger.Information("Metric server started");
        }

        public void ReportIsHealthy()
        {
            _healthGauge = Metrics.CreateGauge("microservice_health", "MicroServiceArchitecture overall system health check. Zero (0) for unhealthy, One (1) for healthy.");
            _healthGauge.Set(1.0);
        }

        public void ReportInconclusive()
        {
            _healthGauge = Metrics.CreateGauge("microservice_health", "MicroServiceArchitecture overall system health check. Zero (0) for unhealthy, One (1) for healthy.");
            _healthGauge.Set(0.5);
        }

        public void ReportIsBroken()
        {
            _healthGauge = Metrics.CreateGauge("microservice_health", "MicroServiceArchitecture overall system health check. Zero (0) for unhealthy, One (1) for healthy.");
            _healthGauge.Set(0);
        }

        public void Dispose()
        {
            metricServer.Stop();
        }
    }
}
