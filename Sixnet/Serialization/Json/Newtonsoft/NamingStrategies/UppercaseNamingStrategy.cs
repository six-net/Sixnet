using Newtonsoft.Json.Serialization;

namespace Sixnet.Serialization.Json.Newtonsoft.NamingStrategies
{
    public class UppercaseNamingStrategy : NamingStrategy
    {
        protected override string ResolvePropertyName(string name)
        {
            return name?.ToUpper() ?? string.Empty;
        }
    }
}
