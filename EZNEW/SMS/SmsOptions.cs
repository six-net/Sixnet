using System;
using System.Collections.Generic;
using System.Text;
using EZNEW.Diagnostics;

namespace EZNEW.Sms
{
    /// <summary>
    /// Sms options
    /// </summary>
    public abstract class SmsOptions : IAdditionalOption
    {
        /// <summary>
        /// Gets or sets the tag
        /// </summary>
        public string Tag { get; set; } = SmsManager.DefaultTag;

        /// <summary>
        /// Gets or sets the additional info
        /// </summary>
        public Dictionary<string, string> Additionals { get; set; }

        public abstract SmsOptions Clone();
    }
}
