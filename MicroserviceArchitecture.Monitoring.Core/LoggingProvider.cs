using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace MicroserviceArchitecture.Monitoring.Core
{
    public class LoggingProvider
    {
        public Logger CreateLogger(Microsoft.Extensions.Configuration.IConfigurationRoot config,
            LoggingLevelSwitch levelSwitch,
            string version, string providerName)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .MinimumLevel.ControlledBy(levelSwitch)
                .Enrich.WithProperty("version", version)
                .Enrich.WithProperty("environment", $"{config[InfrastructureConfigurationKeys.EnvironmentTag]}")
                .Enrich.WithProperty("providerName", providerName)
                .WriteTo.Console(new JsonFormatter());

            var logger = loggerConfiguration.CreateLogger();
            return logger;
        }


        public LoggingLevelSwitch ConvertStringToLoggingLevelSwitch(string loggingLevelString)
        {
            var loggingLevelSwitch = new LoggingLevelSwitch();

            switch (loggingLevelString)
            {
                case "Debug":
                    loggingLevelSwitch.MinimumLevel = LogEventLevel.Debug;
                    break;
                case "Verbose":
                    loggingLevelSwitch.MinimumLevel = LogEventLevel.Verbose;
                    break;
                case "Information":
                    loggingLevelSwitch.MinimumLevel = LogEventLevel.Information;
                    break;
                case "Warning":
                    loggingLevelSwitch.MinimumLevel = LogEventLevel.Warning;
                    break;
                case "Error":
                    loggingLevelSwitch.MinimumLevel = LogEventLevel.Error;
                    break;
                default:
                    loggingLevelSwitch.MinimumLevel = LogEventLevel.Debug;
                    break;
            }

            return loggingLevelSwitch;
        }
    }
}