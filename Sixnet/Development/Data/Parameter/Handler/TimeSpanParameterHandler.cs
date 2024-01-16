using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Sixnet.Development.Data.Parameter.Handler;

namespace Sixnet.Development.Data.ParameterHandler.Handler
{
    public class TimeSpanParameterHandler : IDataCommandParameterHandler
    {
        public DataCommandParameterItem Parse(DataCommandParameterItem originalParameter)
        {
            if (originalParameter != null)
            {
                if (originalParameter.Value is IEnumerable values)
                {
                    List<object> objectValues = new();
                    foreach (TimeSpan val in values)
                    {
                        objectValues.Add(val);
                    }
                    originalParameter.DbType = null;
                    originalParameter.Value = objectValues;
                }
                else if (originalParameter.Value is TimeSpan || originalParameter.DbType == DbType.Time)
                {
                    originalParameter.DbType = DbType.Object;
                }
            }
            return originalParameter;
        }
    }
}
