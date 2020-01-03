using System;
using System.Runtime.Serialization;

namespace System
{
    /// <summary>
    /// Represents a business exception thrown in user invalid operations.
    /// </summary>
    public class BusinessException : Exception
    {
        public BusinessException(string message)
            : base(message)
        {

        }

        public BusinessException(string format, params object[] args)
            : base(String.Format(format, args))
        {

        }

        public BusinessException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public BusinessException(Exception innerException, string format, params object[] args)
            : base(String.Format(format, args), innerException)
        {

        }

        protected BusinessException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
