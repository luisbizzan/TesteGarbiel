using System.Runtime.Serialization;

namespace System
{
    /// <summary>
    /// Represents a business exception thrown in user invalid operations.
    /// </summary>
    public class CompanyException : Exception
    {
        public CompanyException(string message)
            : base(message)
        {

        }

        public CompanyException(string format, params object[] args)
            : base(String.Format(format, args))
        {

        }

        public CompanyException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public CompanyException(Exception innerException, string format, params object[] args)
            : base(String.Format(format, args), innerException)
        {

        }

        protected CompanyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}