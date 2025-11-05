// "Company © 2025. All rights reserved."

using System.Reflection;

namespace Sixnet.Localization
{
    public interface ISixnetResourceManagerFactory
    {
        ISixnetResourceManager Create(ResourcePrefix resourcePrefix, Assembly assembly);
    }
}
