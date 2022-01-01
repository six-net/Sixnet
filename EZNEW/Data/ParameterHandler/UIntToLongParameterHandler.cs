using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EZNEW.Data.ParameterHandler
{
    public class UIntToLongParameterHandler : IParameterHandler
    {
        public ParameterItem Parse(ParameterItem originalParameter)
        {
            if (originalParameter != null)
            {
                if (originalParameter.Value is IEnumerable values)
                {
                    List<long> longValues = new();
                    foreach (uint val in values)
                    {
                        longValues.Add(val);
                    }
                    originalParameter.DbType = null;
                    originalParameter.Value = longValues;
                }
                else if (originalParameter.Value is uint || originalParameter.DbType == DbType.UInt32)
                {
                    originalParameter.DbType = DbType.Int64;
                    originalParameter.Value = Convert.ToInt64(originalParameter.Value);
                }
            }
            return originalParameter;
        }
    }
}
