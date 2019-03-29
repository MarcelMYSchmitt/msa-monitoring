using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Http;

namespace MicroserviceArchitecture.Monitoring.Watchdog
{
    public class ServiceProxy : IServiceProxy
    {
        private static IConfigurationRoot _configuration;

        public ServiceProxy(IConfigurationRoot configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public DataResponse GetSemanticData()
        {
            var checkQuery = GetCheckQueryUrl();

            // Creating a HTTP client for every check is mandatory because sharing a single client introduces caching issues
            // which watchdog does not need to handle due to low performance needs
            var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };

            try
            {
                var httpResponse = httpClient.GetAsync(checkQuery).Result;

                if (!httpResponse.IsSuccessStatusCode && httpResponse.StatusCode != HttpStatusCode.NotFound)
                {
                    throw new UnexpectedStatusCodeForSemanticDataException(
                        "Expected success status or not found when " +
                        "querying the endpoint, but actually got " + httpResponse.StatusCode);
                }

                var httpResponseString = httpResponse.Content.ReadAsStringAsync().Result;

                return Newtonsoft.Json.JsonConvert.DeserializeObject<DataResponse>(httpResponseString);
            }
            catch (Exception exception)
            {
                throw new ServiceUnavailableException(
                    "Service was not available or an unknown exception occured", exception);
            }
        }

         private static string GetCheckQueryUrl()
         {
            var baseUrlForBackendForFrontend = "http://localhost:8080";
            var query = $"{baseUrlForBackendForFrontend}/api/bff";
            var semanticCheckTestId = "100";
            return $"{query}/{semanticCheckTestId}";
        }
    }
}
