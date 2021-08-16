using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Data
{
    /// <summary>
    /// Defines default parameter handler
    /// </summary>
    public class DefaultParameterHandler : IParameterHandler
    {
        readonly Func<ParameterItem, ParameterItem> _handler = null;

        public DefaultParameterHandler(Func<ParameterItem, ParameterItem> handleParameterDelegate)
        {
            _handler = handleParameterDelegate;
        }

        public ParameterItem Parse(ParameterItem originalParameter)
        {
            if (originalParameter == null || _handler == null)
            {
                return originalParameter;
            }
            return _handler(originalParameter);
        }
    }
}
