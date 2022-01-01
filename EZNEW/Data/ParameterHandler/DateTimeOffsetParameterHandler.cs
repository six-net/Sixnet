using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EZNEW.Data.ParameterHandler
{
    public class DateTimeOffsetParameterHandler : IParameterHandler
    {
        public ParameterItem Parse(ParameterItem originalParameter)
        {
            if (originalParameter != null)
            {
                if (originalParameter.Value is IEnumerable values)
                {
                    List<DateTime> dateTimeValues = new();
                    foreach (var val in values)
                    {
                        if (val is DateTimeOffset)
                        {
                            dateTimeValues.Add(((DateTimeOffset)val).DateTime);
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
