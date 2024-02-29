using System.Collections.Generic;
using Sixnet.Model;

namespace Sixnet.Net.Sms
{
    /// <summary>
    /// Sms execution options
    /// </summary>
    public abstract class SmsExecutionOptions : ISixnetExtraParameterModel
    {
        /// <summary>
        /// Gets or sets the tag
        /// </summary>
        public string Tag { get; set; } = SixnetSms.DefaultTag;

        /// <summary>
        /// Gets or sets the parameters
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; }

        public abstract SmsExecutionOptions Clone();
    }
}
