// "Company © 2025. All rights reserved."

using System.Data;

using Sixnet.Development.Data.Dapper;

namespace Sixnet.Development.Data.DataType.Handler
{
    internal class SixnetDateTimeOffsetHandler : SqlMapper.TypeHandler<DateTimeOffset>
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
