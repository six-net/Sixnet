using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Runtime.Serialization;
using System.Text;

namespace Sixnet.Exceptions
{
    /// <summary>
    /// Sixnet sql alread exists exception
    /// </summary>
    public class SixnetSqlAlreadExistsException : SqlTypeException
    {
        public SixnetSqlAlreadExistsException() { }

        public SixnetSqlAlreadExistsException(string message) : base(message) { }

        public SixnetSqlAlreadExistsException(string message, Exception innerException) : base(message, innerException) { }

        protected SixnetSqlAlreadExistsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
