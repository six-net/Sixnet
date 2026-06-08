// "Company © 2025. All rights reserved."

using System.Text;

using Sixnet.Model;

namespace Sixnet.Net.Email
{
    /// <summary>
    /// Send email options
    /// </summary>
    [Serializable]
    public class SixnetEmailInfo : ISixnetProperties
    {
        /// <summary>
        /// Gets or sets the email id
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the parameters
        /// </summary>
        public Dictionary<string, string> Properties { get; set; }

        /// <summary>
        /// Gets or sets the email subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the email title
        /// </summary>
        public string Title { get; set; }

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

        internal SixnetEmailInfo Clone()
        {
            return new SixnetEmailInfo()
            {
                Id = Id,
                BodyEncoding = BodyEncoding,
                BodyIsHtml = BodyIsHtml,
                Subject = Subject,
                Content = Content,
                Emails = Emails.Select(c => c).ToList(),
                Title = Title,
                SubjectEncoding = SubjectEncoding,
                Properties = Properties?.ToDictionary(c => c.Key, c => c.Value) ?? new Dictionary<string, string>(0)
            };
        }
    }
}
