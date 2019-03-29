namespace MicroserviceArchitecture.Monitoring.Watchdog.MetricServer
{
    public interface IMonitoringMetricServer
    {
        void Start();
        void ReportIsHealthy();
        void ReportInconclusive();
        void ReportIsBroken();
    }
}