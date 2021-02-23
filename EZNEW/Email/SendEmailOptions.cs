using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using EZNEW.Diagnostics;

namespace EZNEW.Email
{
    /// <summary>
    /// Send email options
    /// </summary>
    [Serializable]
    public class SendEmailOptions : IAdditionalOption
    {
        /// <summary>
        /// Gets or sets the email id
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the additional info
        /// </summary>
        public Dictionary<string, string> Additionals { get; set; }

        /// <summary>
        /// Gets or sets the email category
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the email subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the email content
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets receive addresses
        /// </summary>
        public IEnumerable<string> Emails { get; set; }

        /// <summary>
        /// Gets or sets the subject encoding.
        /// Default use the Encoding.UTF8.
        /// </summary>
        public Encoding SubjectEncoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// Gets or set the email body encoding.
        /// Default use the Encoding.UTF8.
        /// </summary>
        public Encoding BodyEncoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// Gets or sets the body is html format.
        /// Default value is true
        /// </summary>
        public bool BodyIsHtml { get; set; } = true;

        /// <summary>
        /// Gets or sets whether send asynchronously.
        /// Default value is true.
        /// </summary>
        public bool Asynchronously { get; set; } = true;

        internal SendEmailOptions Clone()
        {
            return new SendEmailOptions()
            {
                Id = Id,
                Asynchronously = Asynchronously,
                BodyEncoding = BodyEncoding,
                BodyIsHtml = BodyIsHtml,
                Category = Category,
                Content = Content,
                Emails = Emails.Select(c => c).ToList(),
                Subject = Subject,
                SubjectEncoding = SubjectEncoding,
                Additionals = Additionals?.ToDictionary(c => c.Key, c => c.Value) ?? new Dictionary<string, string>(0)
            };
        }
    }
}
