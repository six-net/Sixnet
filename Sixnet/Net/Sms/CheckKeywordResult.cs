using System.Collections.Generic;

namespace Sixnet.Net.Sms
{
    /// <summary>
    /// Check keyword result
    /// </summary>
    public class CheckKeywordResult : SmsResult
    {
        /// <summary>
        /// Gets or sets the keywords
        /// </summary>
        public List<string> Keywords { get; set; }
    }
}
