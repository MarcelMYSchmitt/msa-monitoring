using Microsoft.Extensions.Configuration;
using System;

namespace MicroserviceArchitecture.Monitoring.Core
{
    public class ConfigurationProvider
    {
        public IConfigurationRoot CreateConfiguration()
        {
            return CreateConfiguration(builder => { });
        }

        public IConfigurationRoot CreateConfiguration(Action<ConfigurationBuilder> configureAdditionalSources)
        {
            var builder = new ConfigurationBuilder();
            builder.AddEnvironmentVariables();
            configureAdditionalSources(builder);

            return builder.Build();
        }
    }
}
