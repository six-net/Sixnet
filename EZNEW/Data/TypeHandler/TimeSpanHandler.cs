using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EZNEW.Data.TypeHandler
{
    public class TimeSpanHandler : Dapper.SqlMapper.TypeHandler<TimeSpan>
    {
        public override TimeSpan Parse(object value)
        {
            if (value is TimeSpan timeSpan)
            {
                return timeSpan;
            }
            TimeSpan.TryParse(value?.ToString(), out var timeSpanValue);
            return timeSpanValue;
        }

        public override void SetValue(IDbDataParameter parameter, TimeSpan value)
        {
            parameter.Value = value;
        }
    }
}
