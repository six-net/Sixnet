using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EZNEW.Data.ParameterHandler
{
    public class ULongToDecimalParameterHandler : IParameterHandler
    {
        public ParameterItem Parse(ParameterItem originalParameter)
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
