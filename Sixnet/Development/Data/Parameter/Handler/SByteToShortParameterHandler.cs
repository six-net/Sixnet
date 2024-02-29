using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Sixnet.Development.Data.Parameter.Handler;

namespace Sixnet.Development.Data.ParameterHandler.Handler
{
    public class SByteToShortParameterHandler : ISixnetDataCommandParameterHandler
    {
        public DataCommandParameterItem Parse(DataCommandParameterItem originalParameter)
        {
            if (originalParameter != null)
            {
                if (originalParameter.Value is IEnumerable values)
                {
                    List<short> shortValues = new();
                    foreach (var val in values)
                    {
                        if (val is sbyte sbyteValue)
                        {
                            shortValues.Add(sbyteValue);
                        }
                        else
                        {
                            return originalParameter;
                        }
                    }
                    originalParameter.DbType = null;
                    originalParameter.Value = shortValues;
                }
                else if (originalParameter.Value is sbyte || originalParameter.DbType == DbType.SByte)
                {
                    originalParameter.DbType = DbType.Int16;
                    originalParameter.Value = Convert.ToInt16(originalParameter.Value);
                }
            }
            return originalParameter;
        }
    }
}
