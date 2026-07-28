// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Data.Parameter.Handler
{
    /// <summary>
    /// Defines parameter handler
    /// </summary>
    public interface ISixnetDataCommandParameterHandler
    {
        /// <summary>
        /// Parse parameter
        /// </summary>
        /// <param name="originalParameter">Original parameter</param>
        /// <returns></returns>
        SixnetDataCommandParameterItem Parse(SixnetDataCommandParameterItem originalParameter);
    }
}
