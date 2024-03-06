using System.Collections.Generic;

namespace Sixnet.Model
{
    /// <summary>
    /// Defines properties contract
    /// </summary>
    public interface ISixnetProperties
    {
        /// <summary>
        /// Gets or sets the properties
        /// </summary>
        Dictionary<string, string> Properties { get; set; }
    }
}
