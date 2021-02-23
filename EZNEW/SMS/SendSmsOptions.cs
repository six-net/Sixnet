using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Sms
{
    /// <summary>
    /// Sms send options
    /// </summary>
    public class SendSmsOptions : SmsOptions
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
        /// Gets or sets content
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets parameters
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; }

        /// <summary>
        /// Gets or sets the callback data
        /// </summary>
        public string CallbackData { get; set; }

        /// <summary>
        /// Gets or sets the subcode
        /// </summary>
        public string Subcode { get; set; }

        /// <summary>
        /// Gets or sets the send time
        /// </summary>
        public DateTimeOffset SendTime { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// Gets or sets the content format
        /// </summary>
        public SmsContentFormat ContentFormat { get; set; }

        /// <summary>
        /// Gets or sets whether send asynchronously.
        /// Default value is true.
        /// </summary>
        public bool Asynchronously { get; set; } = true;

        public override SmsOptions Clone()
        {
            return new SendSmsOptions()
            {
                Tag = Tag,
                SendTime = SendTime,
                Additionals = Additionals?.ToDictionary(c => c.Key, c => c.Value) ?? new Dictionary<string, string>(0),
                Asynchronously = Asynchronously,
                CallbackData = CallbackData,
                Content = Content,
                ContentFormat = ContentFormat,
                Id = Id,
                Mobiles = Mobiles?.Select(c => c).ToList() ?? new List<string>(0),
                Parameters = Parameters?.ToDictionary(c => c.Key, c => c.Value) ?? new Dictionary<string, string>(0),
                Subcode = Subcode
            };
        }
    }
}
