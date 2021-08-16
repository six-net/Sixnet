using System.Collections.Generic;

namespace EZNEW.Development.Command.Modification
{
    /// <summary>
    /// Defines modification item
    /// </summary>
    public interface IModificationItem
    {
        /// <summary>
        /// Resolve value
        /// </summary>
        /// <returns>Return modification values</returns>
        KeyValuePair<string, IModificationValue> ResolveValue();
    }
}
