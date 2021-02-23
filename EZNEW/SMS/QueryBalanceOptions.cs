using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EZNEW.Sms
{
    /// <summary>
    /// Query balance options
    /// </summary>
    public class QueryBalanceOptions : SmsOptions
    {
        public override SmsOptions Clone()
        {
            return new QueryBalanceOptions()
            {
                Tag = Tag,
                Additionals = Additionals?.ToDictionary(c => c.Key, c => c.Value) ?? new Dictionary<string, string>(0)
            };
        }
    }
}
