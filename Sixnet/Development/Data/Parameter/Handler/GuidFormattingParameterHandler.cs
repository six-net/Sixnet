using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Sixnet.Development.Data.Parameter.Handler;

namespace Sixnet.Development.Data.ParameterHandler.Handler
{
    /// <summary>
    /// Defines guid parameter handler
    /// </summary>
    public class GuidFormattingParameterHandler : ISixnetDataCommandParameterHandler
    {
        public DataCommandParameterItem Parse(DataCommandParameterItem originalParameter)
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
                else if (originalParameter.Value is Guid || originalParameter.DbType == DbType.Guid)
                {
                    var guidValue = (Guid)originalParameter.Value;
                    originalParameter.DbType = DbType.Binary;
                    originalParameter.Value = guidValue.ToByteArray();
                }
            }
            return originalParameter;
        }
    }
}
