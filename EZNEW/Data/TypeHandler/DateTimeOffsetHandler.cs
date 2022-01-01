using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EZNEW.Data.TypeHandler
{
    internal class DateTimeOffsetHandler : Dapper.SqlMapper.TypeHandler<DateTimeOffset>
    {
        public override DateTimeOffset Parse(object value)
        {
            if (value is DateTimeOffset dateTimeOffsetValue)
            {
                return dateTimeOffsetValue;
            }
            DateTimeOffset.TryParse(value?.ToString(), out var dateTimeOffset);
            return dateTimeOffset;
        }

        public override void SetValue(IDbDataParameter parameter, DateTimeOffset value)
        {
            parameter.Value = value;
        }
    }
}
