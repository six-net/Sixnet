// "Company © 2025. All rights reserved."

using System.Collections;
using System.Data;

using Sixnet.Development.Data.Parameter.Handler;

namespace Sixnet.Development.Data.ParameterHandler.Handler
{
    public class SixnetDateTimeOffsetParameterHandler : ISixnetDataCommandParameterHandler
    {
        public DataCommandParameterItem Parse(DataCommandParameterItem originalParameter)
        {
            if (originalParameter != null)
            {
                if (originalParameter.Value is IEnumerable values)
                {
                    List<DateTime> dateTimeValues = new();
                    foreach (var val in values)
                    {
                        if (val is DateTimeOffset offsetVal)
                        {
                            dateTimeValues.Add(offsetVal.DateTime);
                        }
                        else
                        {
                            return originalParameter;
                        }
                    }
                    originalParameter.DbType = null;
                    originalParameter.Value = dateTimeValues;
                }
                else if (originalParameter.Value is DateTimeOffset || originalParameter.DbType == DbType.DateTimeOffset)
                {
                    originalParameter.DbType = DbType.DateTime;
                    originalParameter.Value = ((DateTimeOffset)originalParameter.Value).DateTime;
                }
            }
            return originalParameter;
        }
    }
}
