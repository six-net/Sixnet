using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EZNEW.Data.TypeHandler
{
    internal class GuidHandler : Dapper.SqlMapper.TypeHandler<Guid>
    {
        public override Guid Parse(object value)
        {
            if (value is byte[] byteValues)
            {
                return new Guid(byteValues);
            }
            if (value is Guid guidValue)
            {
                return guidValue;
            }
            return Guid.Parse(value?.ToString());
        }

        public override void SetValue(IDbDataParameter parameter, Guid value)
        {
            parameter.Value = value;
        }
    }
}
