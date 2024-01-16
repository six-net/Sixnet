using System.Collections.Generic;

namespace Sixnet.App.Module
{
    /// <summary>
    /// Module manager
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
                    configration?.Configure();
                }
                ModuleConfigurations = null;
            }
        }
    }
}
