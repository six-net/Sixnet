// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Data.Parameter.Handler
{
    public class SixnetNullCharacterParameterHandler : ISixnetDataCommandParameterHandler
    {
        const char nullChar = '\u0000';
        public SixnetDataCommandParameterItem Parse(SixnetDataCommandParameterItem originalParameter)
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
