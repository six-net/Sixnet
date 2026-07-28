// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Data.Parameter.Handler
{
    /// <summary>
    /// Defines default parameter handler
    /// </summary>
    public class SixnetDefaultDataCommandParameterHandler : ISixnetDataCommandParameterHandler
    {
        readonly Func<SixnetDataCommandParameterItem, SixnetDataCommandParameterItem> _handler = null;

        public SixnetDefaultDataCommandParameterHandler(Func<SixnetDataCommandParameterItem, SixnetDataCommandParameterItem> handleParameterDelegate)
        {
            _handler = handleParameterDelegate;
        }

        public SixnetDataCommandParameterItem Parse(SixnetDataCommandParameterItem originalParameter)
        {
            if (originalParameter == null || _handler == null)
            {
                return originalParameter;
            }
            return _handler(originalParameter);
        }
    }
}
