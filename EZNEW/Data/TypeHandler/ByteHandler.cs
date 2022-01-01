using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EZNEW.Data.TypeHandler
{
    public class ByteHandler : Dapper.SqlMapper.TypeHandler<byte>
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
