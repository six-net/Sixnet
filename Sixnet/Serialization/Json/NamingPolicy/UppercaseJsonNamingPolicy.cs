using System.Text.Json;

namespace Sixnet.Serialization.Json.NamingPolicy
{
    /// <summary>
    /// Defines uppercase json naming policy
    /// </summary>
    public class UppercaseJsonNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return name?.ToUpper() ?? string.Empty;
        }

        public static UppercaseJsonNamingPolicy Instance = new UppercaseJsonNamingPolicy();
    }
}
