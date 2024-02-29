using System.Collections;
using System.Collections.Generic;
using System.Data;
using Sixnet.Development.Data.Parameter.Handler;

namespace Sixnet.Development.Data.ParameterHandler.Handler
{
    public class BooleanToIntegerParameterHandler : ISixnetDataCommandParameterHandler
    {
        public DataCommandParameterItem Parse(DataCommandParameterItem originalParameter)
        {
            if (originalParameter != null)
            {
                if (originalParameter.Value is IEnumerable values)
                {
                    List<byte> byteValues = new();
                    foreach (var val in values)
                    {
                        if (val is bool boolValue)
                        {
                            byteValues.Add((byte)(boolValue ? 1 : 0));
                        }
                        else
                        {
                            return originalParameter;
                        }
                    }
                    originalParameter.DbType = null;
                    originalParameter.Value = byteValues;
                }
                else if (originalParameter.Value is bool || originalParameter.DbType == DbType.Boolean)
                {
                    originalParameter.DbType = DbType.Byte;
                    bool.TryParse(originalParameter.Value?.ToString(), out var boolValue);
                    originalParameter.Value = boolValue ? 1 : 0;
                }
            }
            return originalParameter;
        }
    }
}
