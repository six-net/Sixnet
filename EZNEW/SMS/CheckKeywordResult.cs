using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Sms
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
