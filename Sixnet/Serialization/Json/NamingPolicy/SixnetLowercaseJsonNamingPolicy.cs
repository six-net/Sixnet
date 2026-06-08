// "Company © 2025. All rights reserved."

using System.Text.Json;

namespace Sixnet.Serialization.Json.NamingPolicy
{
    /// <summary>
    /// Defines lowercase nameing policy
    /// </summary>
    public class SixnetLowercaseJsonNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return name?.ToLower() ?? string.Empty;
        }

        public static SixnetLowercaseJsonNamingPolicy Instance = new SixnetLowercaseJsonNamingPolicy();
    }
}
