using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Sixnet.Development.Data.Parameter.Handler
{
    public class NullCharacterParameterHandler : IDataCommandParameterHandler
    {
        const char nullChar = '\u0000';
        public DataCommandParameterItem Parse(DataCommandParameterItem originalParameter)
        {
            if (originalParameter != null)
            {
                if (originalParameter.Value is char charVal && charVal == nullChar)
                {
                    originalParameter.Value = string.Empty;
                }
            }
            return originalParameter;
        }
    }
}
