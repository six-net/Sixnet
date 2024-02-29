using System.Collections.Generic;
using System.Linq;

namespace Sixnet.Net.Sms
{
    /// <summary>
    /// Check black options
    /// </summary>
    public class CheckBlackOptions : SmsExecutionOptions
    {
        public override SmsExecutionOptions Clone()
        {
            return new CheckBlackOptions()
            {
                Tag = Tag,
                Parameters = Parameters?.ToDictionary(c => c.Key, c => c.Value) ?? new Dictionary<string, string>(0),
            };
        }
    }
}
