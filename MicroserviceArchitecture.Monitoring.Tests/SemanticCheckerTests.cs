using MicroserviceArchitecture.Monitoring.Watchdog;
using MicroserviceArchitecture.Monitoring.Watchdog.MetricServer;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using Serilog;
using System;
using Xunit;

namespace MicroserviceArchitecture.Monitoring.Tests
{
    public class SemanticCheckerTests
    {
        private readonly IMonitoringMetricServer _metricServer;
        private readonly IServiceProxy _serviceProxy;
        private readonly SemanticChecker _semanticChecker;
        private static ILogger _logger;

        public SemanticCheckerTests()
        {
            _logger = Substitute.For<ILogger>();
            _metricServer = Substitute.For<IMonitoringMetricServer>();
            _serviceProxy = Substitute.For<IServiceProxy>();
            _semanticChecker = new SemanticChecker(_metricServer, _serviceProxy, _logger);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void GivenServiceUnavailabe_ShouldBeBroken()
        {
            // Arrange
            _serviceProxy.GetSemanticData().Throws<ServiceUnavailableException>();

            // Act
            _semanticChecker.DoSemanticCheck(false); // it should be irrelevant whether first run

            // Assert
            _metricServer.Received().ReportIsBroken();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void GivenServiceAvailable_WhenFirstRun_ShouldBeUnconclusive()
        {
            // Act
            _semanticChecker.DoSemanticCheck(true);

            // Assert
            _metricServer.Received().ReportInconclusive();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void GivenServiceAvailable_WhenCurrentMessageIsStillNotFoundAfterFirstRun_ShouldBeBroken()
        {
            // Arrange
            _serviceProxy.GetSemanticData().ReturnsNull();

            // Act
            _semanticChecker.DoSemanticCheck(false);

            // Assert
            _metricServer.Received().ReportIsBroken();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void GivenServiceAvailable_WhenCurrentMessageIsNotOlderThanAllowed_ShouldBeHealthy()
        {
            // Arrange
            var data = new DataResponse
            {
                MessageCreatedAt = DateTime.UtcNow - TimeSpan.FromSeconds(10)
            };

            _serviceProxy.GetSemanticData().Returns(data);

            // Act
            _semanticChecker.DoSemanticCheck(false);

            // Assert
            _metricServer.Received().ReportIsHealthy();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void GivenServiceAvailable_WhenCurrentMessageIsOlderThanAllowed_ShouldBeBroken()
        {
            // Arrange
            var data = new DataResponse
            {
                MessageCreatedAt = DateTime.UtcNow - TimeSpan.FromSeconds(60)
            };

            _serviceProxy.GetSemanticData().Returns(data);

            // Act
            _semanticChecker.DoSemanticCheck(false);

            // Assert
            _metricServer.Received().ReportIsBroken();
        }
    }
}
