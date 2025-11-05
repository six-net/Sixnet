// "Company © 2025. All rights reserved."

using System.Reflection;

namespace Sixnet.Localization
{
    /// <summary>
    /// Sixnet resource manager factory
    /// </summary>
    public class SixnetResourceManagerFactory : ISixnetResourceManagerFactory
    {
        public ISixnetResourceManager Create(ResourcePrefix resourcePrefix, Assembly assembly)
        {
            return new SixnetResourceManager(resourcePrefix, assembly);
        }
    }
}
