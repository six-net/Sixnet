﻿using System.Collections.Generic;
using System.Linq;

namespace Sixnet.Net.Sms
{
    /// <summary>
    /// Query balance options
    /// </summary>
    public class QueryBalanceOptions : SmsExecutionOptions
    {
        public override SmsExecutionOptions Clone()
        {
            return new QueryBalanceOptions()
            {
                Tag = Tag,
                Parameters = Parameters?.ToDictionary(c => c.Key, c => c.Value) ?? new Dictionary<string, string>(0)
            };
        }
    }
}
