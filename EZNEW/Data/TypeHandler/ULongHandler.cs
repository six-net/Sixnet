using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EZNEW.Data.TypeHandler
{
#pragma warning disable CS3009
    public class ULongHandler : Dapper.SqlMapper.TypeHandler<ulong>
#pragma warning restore CS3009
    {
#pragma warning disable CS3002
        public override ulong Parse(object value)
#pragma warning restore CS3002
        {
            return Convert.ToUInt64(value);
        }

#pragma warning disable CS3001
        public override void SetValue(IDbDataParameter parameter, ulong value)
#pragma warning restore CS3001
        {
            parameter.Value = value; ;
        }
    }
}
