using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EZNEW.Data.ParameterHandler
{
    public class BooleanToIntegerParameterHandler : IParameterHandler
    {
        public ParameterItem Parse(ParameterItem originalParameter)
        {
            if (originalParameter != null
                && (originalParameter.Value is bool || originalParameter.DbType == DbType.Boolean))
            {
                originalParameter.DbType = DbType.Int32;
                bool.TryParse(originalParameter.Value?.ToString(), out var boolVal);
                originalParameter.Value = boolVal ? 1 : 0;
            }
            return originalParameter;
        }
    }
}
