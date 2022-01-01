using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EZNEW.Data.ParameterHandler
{
    public class CharToStringParameterHandler : IParameterHandler
    {
        public ParameterItem Parse(ParameterItem originalParameter)
        {
            if (originalParameter != null)
            {
                if (originalParameter.Value is IEnumerable values)
                {
                    List<string> stringValues = new();
                    foreach (var val in values)
                    {
                        if (val is char charValue)
                        {
                            stringValues.Add(charValue.ToString());
                        }
                        else
                        {
                            return originalParameter;
                        }
                    }
                    originalParameter.DbType = null;
                    originalParameter.Value = stringValues;
                }
                else if (originalParameter.Value is char || originalParameter.DbType == DbType.StringFixedLength)
                {
                    originalParameter.DbType = DbType.String;
                    originalParameter.Value = originalParameter.Value.ToString();
                }
            }
            return originalParameter;
        }
    }
}
