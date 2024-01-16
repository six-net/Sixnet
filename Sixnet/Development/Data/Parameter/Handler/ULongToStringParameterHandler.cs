using System.Collections;
using System.Collections.Generic;
using System.Data;
using Sixnet.Development.Data.Parameter.Handler;

namespace Sixnet.Development.Data.ParameterHandler.Handler
{
    public class ULongToStringParameterHandler : IDataCommandParameterHandler
    {
        public DataCommandParameterItem Parse(DataCommandParameterItem originalParameter)
        {
            if (originalParameter != null)
            {
                if (originalParameter.Value is IEnumerable values)
                {
                    List<string> stringValues = new();
                    foreach (ulong val in values)
                    {
                        stringValues.Add(val.ToString());
                    }
                    originalParameter.DbType = null;
                    originalParameter.Value = stringValues;
                }
                else if (originalParameter.Value is ulong || originalParameter.DbType == DbType.UInt64)
                {
                    originalParameter.DbType = DbType.String;
                    originalParameter.Value = originalParameter.Value.ToString();
                }
            }
            return originalParameter;
        }
    }
}
