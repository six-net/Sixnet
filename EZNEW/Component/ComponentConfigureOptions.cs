using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Component
{
    /// <summary>
    /// Component configure options
    /// </summary>
    public class ComponentConfigureOptions
    {
        /// <summary>
        /// Gets or sets the service collection
        /// </summary>
        public IServiceCollection Services { get; set; }
    }
}
