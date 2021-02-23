using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Sms
{
    /// <summary>
    /// Send sms result
    /// </summary>
    public class SendSmsResult : SmsResult
    {
        /// <summary>
        /// Gets or sets the message id
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// Gets or sets the business id
        /// </summary>
        public string BizId { get; set; }

        /// <summary>
        /// Gets or sets the mobiles which send failed
        /// </summary>
        public List<string> FailMobiles { get; set; }

        internal SendSmsResult Clone()
        {
            return new SendSmsResult()
            {
                RequestId = RequestId,
                Result = Result,
                Code = Code,
                Description = Description,
                Success = Success,
                MessageId = MessageId,
                BizId = BizId,
                FailMobiles = FailMobiles?.Select(c => c).ToList() ?? new List<string>(0),
                SmsAccount = SmsAccount?.Clone(),
                SmsOptions = SmsOptions?.Clone()
            };
        }
    }
}
