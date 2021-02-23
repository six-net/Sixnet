using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Diagnostics
{
    /// <summary>
    /// Additional option contract
    /// </summary>
    public interface IAdditionalOption
    {
        /// <summary>
        /// Gets or sets additionals
        /// </summary>
        Dictionary<string, string> Additionals { get; set; }
    }
}
