using System;
using System.Runtime.Serialization;

namespace EZNEW.Exceptions
{
    /// <summary>
    /// EZNEW application exception
    /// </summary>
    [Serializable]
    public class EZNEWApplicationException : EZNEWException
    {
        public EZNEWApplicationException() { }

        public EZNEWApplicationException(string message) : base(message) { }

        public EZNEWApplicationException(string message, Exception innerException) : base(message, innerException) { }

        protected EZNEWApplicationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
