// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Data.Parameter.Handler
{
    /// <summary>
    /// Defines default parameter handler
    /// </summary>
    public class SixnetDefaultDataCommandParameterHandler : ISixnetDataCommandParameterHandler
    {
        readonly Func<DataCommandParameterItem, DataCommandParameterItem> _handler = null;

        public SixnetDefaultDataCommandParameterHandler(Func<DataCommandParameterItem, DataCommandParameterItem> handleParameterDelegate)
        {
            _handler = handleParameterDelegate;
        }

        public DataCommandParameterItem Parse(DataCommandParameterItem originalParameter)
        {
            if (originalParameter == null || _handler == null)
            {
                return originalParameter;
            }
            return _handler(originalParameter);
        }
    }
}
