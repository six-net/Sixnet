using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EZNEW.Serialization.Json.NamingPolicy
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
