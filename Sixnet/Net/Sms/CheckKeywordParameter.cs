using System.Collections.Generic;
using System.Linq;

namespace Sixnet.Net.Sms
{
    /// <summary>
    /// Check keyword parameter
    /// </summary>
    public class CheckKeywordParameter : SmsParameter
    {
        public override SmsParameter Clone()
        {
            return new CheckKeywordParameter()
            {
                Tag = Tag,
                Properties = Properties?.ToDictionary(c => c.Key, c => c.Value) ?? new Dictionary<string, string>(0),
            };
        }
    }
}
