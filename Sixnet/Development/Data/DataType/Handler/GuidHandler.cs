using System;
using System.Data;
using Sixnet.Development.Data.Dapper;

namespace Sixnet.Development.Data.DataType.Handler
{
    internal class GuidHandler : SqlMapper.TypeHandler<Guid>
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
