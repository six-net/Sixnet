using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace EZNEW.Data.ParameterHandler
{
    public class DateTimeOffsetParameterHandler : IParameterHandler
    {
        public ParameterItem Parse(ParameterItem originalParameter)
        {
            if (originalParameter != null
                && (originalParameter.Value is DateTimeOffset || originalParameter.DbType == DbType.DateTimeOffset))
            {
                originalParameter.DbType = DbType.DateTime;
                originalParameter.Value = ((DateTimeOffset)originalParameter.Value).DateTime;
            }
            return originalParameter;
        }
    }
}
