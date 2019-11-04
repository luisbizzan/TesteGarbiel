using System;
using System.Runtime.Serialization;

namespace System
{
    /// <summary>
    /// Represents a business exception thrown in user invalid operations.
    /// </summary>
    public class ConpanyException : Exception
    {
        public ConpanyException(string message)
            : base(message)
        {

        }

        public ConpanyException(string format, params object[] args)
            : base(String.Format(format, args))
        {

        }

        public ConpanyException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public ConpanyException(Exception innerException, string format, params object[] args)
            : base(String.Format(format, args), innerException)
        {

        }

        protected ConpanyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
