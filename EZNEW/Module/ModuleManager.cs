using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Module
{
    /// <summary>
    /// Defines module manager
    /// </summary>
    internal static class ModuleManager
    {
        static List<IModuleConfiguration> ModuleConfigurations = null;

        /// <summary>
        /// Register module configuration
        /// </summary>
        /// <param name="configuration"></param>
        internal static void RegisterModuleConfiguration(IModuleConfiguration configuration)
        {
            if (configuration != null)
            {
                ModuleConfigurations ??= new List<IModuleConfiguration>();
                ModuleConfigurations.Add(configuration);
            }
        }

        /// <summary>
        /// Configure module
        /// </summary>
        internal static void ConfigureModule()
        {
            if (!ModuleConfigurations.IsNullOrEmpty())
            {
                foreach (var configration in ModuleConfigurations)
                {
                    configration?.Init();
                }
                ModuleConfigurations = null;
            }
        }
    }
}
