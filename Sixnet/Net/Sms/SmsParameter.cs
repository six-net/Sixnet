using System.Collections.Generic;
using Sixnet.Model;

namespace Sixnet.Net.Sms
{
    /// <summary>
    /// Sms parameter
    /// </summary>
    public abstract class SmsParameter : ISixnetProperties
    {
        /// <summary>
        /// Gets or sets the tag
        /// </summary>
        public string Tag { get; set; } = SixnetSms.DefaultTag;

        /// <summary>
        /// Gets or sets the parameters
        /// </summary>
        public Dictionary<string, string> Properties { get; set; }

        public abstract SmsParameter Clone();
    }
}
