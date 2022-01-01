using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EZNEW.Data.ParameterHandler
{
    /// <summary>
    /// Defines guid parameter handler
    /// </summary>
    public class GuidFormattingParameterHandler : IParameterHandler
    {
        public ParameterItem Parse(ParameterItem originalParameter)
        {
            if (originalParameter != null)
            {
                if (originalParameter.Value is IEnumerable values)
                {
                    List<byte[]> byteArrayValues = new();
                    foreach (var val in values)
                    {
                        if (val is Guid guidValue)
                        {
                            byteArrayValues.Add(guidValue.ToByteArray());
                        }
                        else
                        {
                            return originalParameter;
                        }
                    }
                    originalParameter.DbType = null;
                    originalParameter.Value = byteArrayValues;
                }
                else if (originalParameter.Value is Guid guidValue || originalParameter.DbType == DbType.Guid)
                {
                    
                    originalParameter.DbType = DbType.Binary;
                    originalParameter.Value = guidValue.ToByteArray();
                }
            }
            return originalParameter;
        }
    }
}
