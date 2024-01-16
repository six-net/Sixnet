using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Sixnet.Development.Data.Parameter.Handler;

namespace Sixnet.Development.Data.ParameterHandler.Handler
{
    public class ULongToDecimalParameterHandler : IDataCommandParameterHandler
    {
        public DataCommandParameterItem Parse(DataCommandParameterItem originalParameter)
        {
            if (originalParameter != null)
            {
                if (originalParameter.Value is IEnumerable values)
                {
                    List<decimal> decimalValues = new();
                    foreach (ulong val in values)
                    {
                        decimalValues.Add(val);
                    }
                    originalParameter.DbType = null;
                    originalParameter.Value = decimalValues;
                }
                else if (originalParameter.Value is ulong || originalParameter.DbType == DbType.UInt64)
                {
                    originalParameter.DbType = DbType.Decimal;
                    originalParameter.Value = Convert.ToDecimal(originalParameter.Value);
                }
            }
            return originalParameter;
        }
    }
}
