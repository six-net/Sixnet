using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EZNEW.Sms
{
    /// <summary>
    /// Sms receive options
    /// </summary>
    public class ReceiveSmsOptions : SmsOptions
    {
        public override SmsOptions Clone()
        {
            return new ReceiveSmsOptions()
            {
                Tag = Tag,
                Additionals = Additionals?.ToDictionary(c => c.Key, c => c.Value) ?? new Dictionary<string, string>(0)
            };
        }
    }
}
