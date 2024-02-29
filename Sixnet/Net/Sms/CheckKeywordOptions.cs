using System.Collections.Generic;
using System.Linq;

namespace Sixnet.Net.Sms
{
    /// <summary>
    /// Check keyword options
    /// </summary>
    public class CheckKeywordOptions : SmsExecutionOptions
    {
        public override SmsExecutionOptions Clone()
        {
            return new CheckKeywordOptions()
            {
                Tag = Tag,
                Parameters = Parameters?.ToDictionary(c => c.Key, c => c.Value) ?? new Dictionary<string, string>(0),
            };
        }
    }
}
