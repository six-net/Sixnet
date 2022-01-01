using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EZNEW.Data.ParameterHandler
{
    public class UShortToIntParameterHandler : IParameterHandler
    {
        public ParameterItem Parse(ParameterItem originalParameter)
        {
            if (originalParameter != null)
            {
                if (originalParameter.Value is IEnumerable values)
                {
                    List<int> intValues = new();
                    foreach (ushort val in values)
                    {
                        intValues.Add(val);
                    }
                    originalParameter.DbType = null;
                    originalParameter.Value = intValues;
                }
                else if (originalParameter.Value is ushort || originalParameter.DbType == DbType.UInt16)
                {
                    originalParameter.DbType = DbType.Int32;
                    originalParameter.Value = Convert.ToInt32(originalParameter.Value);
                }
            }
            return originalParameter;
        }
    }
}
