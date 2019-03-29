using MicroserviceArchitecture.Monitoring.Watchdog;
using NSubstitute;
using Serilog;
using System;
using System.Threading;
using Xunit;

namespace MicroserviceArchitecture.Monitoring.Tests
{
    public class PeriodicCheckerTests
    {
        private readonly ISemanticChecker _semanticChecker;
        private readonly PeriodicChecker _periodicChecker;
        private static ILogger _logger;

        public PeriodicCheckerTests()
        {
            _logger = Substitute.For<ILogger>();
            _semanticChecker = Substitute.For<ISemanticChecker>();
            _periodicChecker = new PeriodicChecker(_semanticChecker, TimeSpan.FromMilliseconds(50), _logger);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ShouldNotBeFirstAttemptAnymore_WhenExecutedTheSecondTime()
        {
            // Arrange
            var cancellationTokenSource = CreateCancellationTokenWhichExpiresAutomatically();

            // Act
            _periodicChecker.PeriodicallyCheckForSemanticMessage(cancellationTokenSource.Token);

            // Assert
            AssertThatSemanticCheckIsCalledWithCorrectOrderForFirstRun();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ShouldNotBeFirstAttemptAnymore_WhenExceptionsOccur()
        {
            // Arrange
            SemanticCheckerAlwaysThrowsExceptions();
            var cancellationTokenSource = CreateCancellationTokenWhichExpiresAutomatically();

            // Act
            _periodicChecker.PeriodicallyCheckForSemanticMessage(cancellationTokenSource.Token);

            // Assert
            AssertThatSemanticCheckIsCalledWithCorrectOrderForFirstRun();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ShouldContinueChecking_EvenWhenExceptionsOccur()
        {
            // Arrange
            SemanticCheckerAlwaysThrowsExceptions();
            var cancellationTokenSource = CreateCancellationTokenWhichExpiresAutomatically();

            // Act
            _periodicChecker.PeriodicallyCheckForSemanticMessage(cancellationTokenSource.Token);

            // Assert
            // simply check if multiple calls occured, the order of first run is irrelevant here 
            AssertThatSemanticCheckIsCalledWithCorrectOrderForFirstRun();
        }

        private static CancellationTokenSource CreateCancellationTokenWhichExpiresAutomatically()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(250));
            return cancellationTokenSource;
        }

        private void SemanticCheckerAlwaysThrowsExceptions()
        {
            _semanticChecker.When(x => x.DoSemanticCheck(Arg.Any<bool>()))
                .Do(x => throw new Exception());
        }

        private void AssertThatSemanticCheckIsCalledWithCorrectOrderForFirstRun()
        {
            // this simply check below is not perfect, because it does not guarantee order,
            // but we decided it is enough for now
            _semanticChecker.Received().DoSemanticCheck(Arg.Is(true));
            _semanticChecker.Received().DoSemanticCheck(Arg.Is(false));
        }
    }
}
