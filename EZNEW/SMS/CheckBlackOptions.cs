using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EZNEW.Sms
{
    /// <summary>
    /// Check black options
    /// </summary>
    public class CheckBlackOptions : SmsOptions
    {
        public override SmsOptions Clone()
        {
            return new CheckBlackOptions()
            {
                Tag = Tag,
                Additionals = Additionals?.ToDictionary(c => c.Key, c => c.Value) ?? new Dictionary<string, string>(0),
            };
        }
    }
}
