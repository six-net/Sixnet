using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Sixnet.Localization
{
    public interface ISixnetResourceManagerFactory
    {
        ISixnetResourceManager Create(ResourcePrefix resourcePrefix, Assembly assembly);
    }
}
