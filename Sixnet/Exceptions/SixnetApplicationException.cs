using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Sixnet.Exceptions
{
    /// <summary>
    /// Sixnet application exception
    /// </summary>
    public class SixnetApplicationException : Exception
    {
        public SixnetApplicationException() { }

        public SixnetApplicationException(string message) : base(message) { }

        public SixnetApplicationException(string message, Exception innerException) : base(message, innerException) { }

        protected SixnetApplicationException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public static void ThrowIf(bool predicate, string message = "")
        {
            if (predicate)
            {
                throw new SixnetException(message);
            }
        }
    }
}
