// "Company © 2025. All rights reserved."

using System.Text.Json;

namespace Sixnet.Serialization.Json.NamingPolicy
{
    /// <summary>
    /// Defines original json naming policy
    /// </summary>
    public class OriginalJsonNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return name;
        }

        public static OriginalJsonNamingPolicy Instance = new OriginalJsonNamingPolicy();
    }
}
