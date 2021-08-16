using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Serialization;

namespace EZNEW.Serialization.Json.Newtonsoft.NamingStrategies
{
    public class LowercaseNamingStrategy : NamingStrategy
    {
        protected override string ResolvePropertyName(string name)
        {
            return name?.ToLower() ?? string.Empty;
        }
    }
}
