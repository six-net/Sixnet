using System;
using System.Data;
using Sixnet.Development.Data.Dapper;

namespace Sixnet.Development.Data.DataType.Handler
{
    public class ByteHandler : SqlMapper.TypeHandler<byte>
    {
        public override byte Parse(object value)
        {
            return Convert.ToByte(value);
        }

        public override void SetValue(IDbDataParameter parameter, byte value)
        {
            parameter.Value = value;
        }
    }
}
