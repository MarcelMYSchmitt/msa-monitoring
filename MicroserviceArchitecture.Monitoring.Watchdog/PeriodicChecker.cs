using Serilog;
using System;
using System.Threading;

namespace MicroserviceArchitecture.Monitoring.Watchdog
{
    public class PeriodicChecker
    {
        private readonly TimeSpan _timeToSleepBeforeNextRequest = TimeSpan.FromSeconds(10);
        private readonly ISemanticChecker _semanticChecker;
        private static ILogger _logger;

        public PeriodicChecker(ISemanticChecker semanticChecker, ILogger logger)
        {
            _semanticChecker = semanticChecker;
            _logger = logger;
        }

        public PeriodicChecker(ISemanticChecker semanticChecker, TimeSpan timeToSleepBetweenChecks, ILogger logger)
        {
            _semanticChecker = semanticChecker;
            _timeToSleepBeforeNextRequest = timeToSleepBetweenChecks;
            _logger = logger;
        }

        public void PeriodicallyCheckForSemanticMessage(CancellationToken cancellationToken)
        {
            bool firstAttemptToCheck = true;

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                try
                {
                    _semanticChecker.DoSemanticCheck(firstAttemptToCheck);
                }
                catch (Exception exception)
                {
                    _logger.Information($"Unhandled exception occured: {exception}");
                }

                firstAttemptToCheck = false;

                Thread.Sleep(_timeToSleepBeforeNextRequest);
            }
        }
    }
}
