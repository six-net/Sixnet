using System;
using System.Runtime.Serialization;

namespace EZNEW.Exceptions
{
    /// <summary>
    /// EZNEW framework exception
    /// </summary>
    [Serializable]
    public class EZNEWException : Exception
    {
        public EZNEWException() { }

        public EZNEWException(string message) : base(message) { }

        public EZNEWException(string message, Exception innerException) : base(message, innerException) { }

        protected EZNEWException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
