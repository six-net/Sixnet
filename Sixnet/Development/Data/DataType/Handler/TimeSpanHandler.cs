using System;
using System.Data;
using Sixnet.Development.Data.Dapper;

namespace Sixnet.Development.Data.DataType.Handler
{
    public class TimeSpanHandler : SqlMapper.TypeHandler<TimeSpan>
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
