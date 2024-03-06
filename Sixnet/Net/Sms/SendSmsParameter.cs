using System;
using System.Collections.Generic;
using System.Linq;

namespace Sixnet.Net.Sms
{
    /// <summary>
    /// Send sms parameter
    /// </summary>
    public class SendSmsParameter : SmsParameter
    {
        /// <summary>
        /// Gets or sets the message id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the mobiles
        /// </summary>
        public IEnumerable<string> Mobiles { get; set; }

        /// <summary>
        /// Gets or sets the subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the content
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the content format
        /// </summary>
        public SmsContentFormat ContentFormat { get; set; }

        /// <summary>
        /// Gets or sets the callback info
        /// </summary>
        public string CallbackInfo { get; set; }

        /// <summary>
        /// Gets or sets the subcode
        /// </summary>
        public string Subcode { get; set; }

        /// <summary>
        /// Gets or sets the send time
        /// </summary>
        public DateTimeOffset SendTime { get; set; } = DateTimeOffset.Now;

        public override SmsParameter Clone()
        {
            return new SendSmsParameter()
            {
                Tag = Tag,
                SendTime = SendTime,
                Properties = Properties?.ToDictionary(c => c.Key, c => c.Value) ?? new Dictionary<string, string>(0),
                CallbackInfo = CallbackInfo,
                Content = Content,
                ContentFormat = ContentFormat,
                Id = Id,
                Mobiles = Mobiles?.Select(c => c).ToList() ?? new List<string>(0),
                Subcode = Subcode
            };
        }
    }
}
