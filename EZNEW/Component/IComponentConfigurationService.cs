using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Component
{
    /// <summary>
    /// Component configuration service contract
    /// </summary>
    public interface IComponentConfigurationService
    {
        /// <summary>
        /// Configure component
        /// </summary>
        /// <param name="componentConfigureOptions">Component configure </param>
        void Configure(ComponentConfigureOptions componentConfigureOptions);
    }
}
