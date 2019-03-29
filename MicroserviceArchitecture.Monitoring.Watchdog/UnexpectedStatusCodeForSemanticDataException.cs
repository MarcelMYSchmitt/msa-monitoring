using System;
using System.Runtime.Serialization;

namespace MicroserviceArchitecture.Monitoring.Watchdog
{
    public class UnexpectedStatusCodeForSemanticDataException : Exception
    {
        public UnexpectedStatusCodeForSemanticDataException()
        {
        }

        public UnexpectedStatusCodeForSemanticDataException(string message) : base(message)
        {
        }

        public UnexpectedStatusCodeForSemanticDataException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnexpectedStatusCodeForSemanticDataException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
