using Newtonsoft.Json.Serialization;

namespace Sixnet.Serialization.Json.Newtonsoft.NamingStrategies
{
    public class LowercaseNamingStrategy : NamingStrategy
    {
        protected override string ResolvePropertyName(string name)
        {
            return name?.ToLower() ?? string.Empty;
        }
    }
}
