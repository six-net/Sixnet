using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EZNEW.Data.TypeHandler
{
#pragma warning disable CS3009
    public class UIntHandler : Dapper.SqlMapper.TypeHandler<uint>
#pragma warning restore CS3009
    {
#pragma warning disable CS3002
        public override uint Parse(object value)
#pragma warning restore CS3002
        {
            return Convert.ToUInt32(value);
        }

#pragma warning disable CS3001
        public override void SetValue(IDbDataParameter parameter, uint value)
#pragma warning restore CS3001
        {
            parameter.Value = value;
        }
    }
}
