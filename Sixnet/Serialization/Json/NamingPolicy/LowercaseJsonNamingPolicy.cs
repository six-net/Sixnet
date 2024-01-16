using System.Text.Json;

namespace Sixnet.Serialization.Json.NamingPolicy
{
    /// <summary>
    /// Defines lowercase nameing policy
    /// </summary>
    public class LowercaseJsonNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return name?.ToLower() ?? string.Empty;
        }

        public static LowercaseJsonNamingPolicy Instance = new LowercaseJsonNamingPolicy();
    }
}
