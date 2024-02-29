﻿using System.Collections.Generic;
using System.Linq;

namespace Sixnet.Net.Sms
{
    /// <summary>
    /// Sms receive options
    /// </summary>
    public class ReceiveSmsOptions : SmsExecutionOptions
    {
        public override SmsExecutionOptions Clone()
        {
            return new ReceiveSmsOptions()
            {
                Tag = Tag,
                Parameters = Parameters?.ToDictionary(c => c.Key, c => c.Value) ?? new Dictionary<string, string>(0)
            };
        }
    }
}
