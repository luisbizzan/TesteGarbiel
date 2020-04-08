using System.Runtime.Serialization;

namespace System
{
    public class UserActionHistoricalException : Exception
    {
        public UserActionHistoricalException(string message)
            : base(message)
        {
        }

        public UserActionHistoricalException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        public UserActionHistoricalException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public UserActionHistoricalException(Exception innerException, string format, params object[] args)
            : base(string.Format(format, args), innerException)
        {
        }

        protected UserActionHistoricalException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}