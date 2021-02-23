using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EZNEW.Sms
{
    /// <summary>
    /// Check keyword options
    /// </summary>
    public class CheckKeywordOptions : SmsOptions
    {
        public override SmsOptions Clone()
        {
            return new CheckKeywordOptions()
            {
                Tag = Tag,
                Additionals = Additionals?.ToDictionary(c => c.Key, c => c.Value) ?? new Dictionary<string, string>(0),
            };
        }
    }
}
