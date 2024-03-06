using Sixnet.Cache;
using Sixnet.Development.Data;
using Sixnet.Development.Message;
using Sixnet.IO.FileAccess;
using Sixnet.Net.Email;
using Sixnet.Net.Sms;
using Sixnet.Net.Upload;
using Sixnet.Security.Cryptography;
using Sixnet.Token.Jwt;

namespace Sixnet.DependencyInjection
{
    /// <summary>
    /// Sixnet configuration
    /// </summary>
    internal class SixnetConfiguration
    {
        /// <summary>
        /// Gets or sets upload options
        /// </summary>
        public UploadOptions Upload { get; set; }

        /// <summary>
        /// Gets or sets file access options
        /// </summary>
        public FileAccessOptions FileAccess { get; set; }

        /// <summary>
        /// Gets or sets rsa key options
        /// </summary>
        public RSAKeyOptions RSAKey { get; set; }

        /// <summary>
        /// Gets or sets jwt options
        /// </summary>
        public JwtOptions Jwt { get; set; }

        /// <summary>
        /// Gets or sets data options
        /// </summary>
        public DataOptions Data { get; set; }

        /// <summary>
        /// Gets or sets cache options
        /// </summary>
        public CacheOptions Cache { get; set; }

        /// <summary>
        /// Gets or sets the email options
        /// </summary>
        public EmailOptions Email { get; set; }

        /// <summary>
        /// Gets or sets the sms options
        /// </summary>
        public SmsOptions Sms { get; set; }

        /// <summary>
        /// Gets or sets the message options
        /// </summary>
        public MessageOptions Message { get; set; }
    }
}
