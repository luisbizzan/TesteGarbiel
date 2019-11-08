using System;
using System.Runtime.Serialization;

namespace System
{
    /// <summary>
    /// Represents a Unauthorized exception thrown in user invalid operations.
    /// </summary>
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message)
            : base(message)
        {

        }

        public UnauthorizedException(string format, params object[] args)
            : base(String.Format(format, args))
        {

        }

        public UnauthorizedException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public UnauthorizedException(Exception innerException, string format, params object[] args)
            : base(String.Format(format, args), innerException)
        {

        }

        protected UnauthorizedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
