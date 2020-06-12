using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace EZNEW.Email
{
    /// <summary>
    /// Email send info
    /// </summary>
    [Serializable]
    public class EmailSendInfo
    {
        /// <summary>
        /// Gets the send info id
        /// </summary>
        public string Id { get; } = Guid.NewGuid().ToString();

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
        public IEnumerable<string> EmailAddress { get; set; }

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
    }
}
