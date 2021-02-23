using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Sms
{
    /// <summary>
    /// Check black result
    /// </summary>
    public class CheckBlackResult : SmsResult
    {
        /// <summary>
        /// Gets or sets the blacklist
        /// </summary>
        public List<string> Blacklist { get; set; }
    }
}
