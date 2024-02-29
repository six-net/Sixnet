using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

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
