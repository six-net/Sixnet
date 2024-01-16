using System;
using System.Runtime.Serialization;

namespace Sixnet.Exceptions
{
    /// <summary>
    /// Sixnet framework exception
    /// </summary>
    [Serializable]
    public class SixnetException : Exception
    {
        public SixnetException() { }

        public SixnetException(string message) : base(message) { }

        public SixnetException(string message, Exception innerException) : base(message, innerException) { }

        protected SixnetException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public static void ThrowIf(bool predicate, string message = "")
        {
            if (predicate)
            {
                throw new SixnetException(message);
            }
        }
    }
}
