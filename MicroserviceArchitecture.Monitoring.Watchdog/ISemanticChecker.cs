namespace MicroserviceArchitecture.Monitoring.Watchdog
{
    public interface ISemanticChecker
    {
        void DoSemanticCheck(bool firstAttemptToCheck);
    }
}
