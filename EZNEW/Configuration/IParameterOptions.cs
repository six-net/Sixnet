using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Configuration
{
    /// <summary>
    /// Defines parameter option contract
    /// </summary>
    public interface IParameterOptions
    {
        /// <summary>
        /// Gets or sets parameters
        /// </summary>
        Dictionary<string, string> Parameters { get; set; }
    }
}
