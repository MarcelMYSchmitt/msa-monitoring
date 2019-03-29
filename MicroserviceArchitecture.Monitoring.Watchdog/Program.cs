using MicroserviceArchitecture.Monitoring.Core;
using MicroserviceArchitecture.Monitoring.Watchdog.MetricServer;
using System.Threading;

namespace MicroserviceArchitecture.Monitoring.Watchdog
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var tokenSource = new CancellationTokenSource();
            var configuration = new ConfigurationProvider().CreateConfiguration();

            var logEventLevelSwitch =
                new LoggingProvider().ConvertStringToLoggingLevelSwitch(
                    configuration[ApplicationConfigurationKeys.LogEventLevel]);

            var logger = new LoggingProvider().CreateLogger(configuration, logEventLevelSwitch,
                System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().ToString());

            MonitoringMetricServer metricServer = new MonitoringMetricServer(logger);
            metricServer.Start();


            logger.Information("Initializing Periodic Checker.");
            new PeriodicChecker(new SemanticChecker(metricServer, new ServiceProxy(configuration), logger), logger)
                .PeriodicallyCheckForSemanticMessage(tokenSource.Token);
        }
    }
}
