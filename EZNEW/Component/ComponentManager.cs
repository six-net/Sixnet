using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EZNEW.Component
{
    public static class ComponentManager
    {
        #region Configuration service

        /// <summary>
        /// Query configuration service types
        /// </summary>
        /// <param name="assemblies">Assemblies</param>
        /// <returns>Return component configuration service</returns>
        public static IEnumerable<Type> QueryConfigurationService(params Assembly[] assemblies)
        {
            if (assemblies.IsNullOrEmpty())
            {
                return Array.Empty<Type>();
            }
            List<Type> serviceTypes = new List<Type>();
            foreach (var ass in assemblies)
            {
                if (ass != null)
                {
                    serviceTypes.AddRange(ass.GetTypes().Where(t => !t.IsInterface && typeof(IComponentConfigurationService).IsAssignableFrom(t)));
                }
            }
            return serviceTypes;
        }

        #endregion
    }
}
