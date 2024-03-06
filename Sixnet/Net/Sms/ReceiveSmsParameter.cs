using System.Collections.Generic;
using System.Linq;

namespace Sixnet.Net.Sms
{
    /// <summary>
    /// Sms receive parameter
    /// </summary>
    public class ReceiveSmsParameter : SmsParameter
    {
        public override SmsParameter Clone()
        {
            return new ReceiveSmsParameter()
            {
                Tag = Tag,
                Properties = Properties?.ToDictionary(c => c.Key, c => c.Value) ?? new Dictionary<string, string>(0)
            };
        }
    }
}
