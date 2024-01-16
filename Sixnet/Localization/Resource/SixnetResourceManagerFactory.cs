using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Sixnet.Localization.Resource
{
    public class SixnetResourceManagerFactory : ISixnetResourceManagerFactory
    {
        public ISixnetResourceManager Create(string baseName, Assembly assembly)
        {
            return new SixnetResourceManager(baseName, assembly);
        }
    }
}
