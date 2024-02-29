using System.Collections.Generic;

namespace Sixnet.Model
{
    /// <summary>
    /// Defines parameter option contract
    /// </summary>
    public interface ISixnetExtraParameterModel
    {
        /// <summary>
        /// Gets or sets parameters
        /// </summary>
        Dictionary<string, string> Parameters { get; set; }
    }
}
