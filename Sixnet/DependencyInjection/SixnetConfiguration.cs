// "Company © 2025. All rights reserved."

using Sixnet.Cache;
using Sixnet.Development.Data;
using Sixnet.Development.Entity;
using Sixnet.Development.Message;
using Sixnet.Development.Work;
using Sixnet.Extensions;
using Sixnet.IO;
using Sixnet.Logging;
using Sixnet.MQ;
using Sixnet.Net.Email;
using Sixnet.Net.Sms;
using Sixnet.Security.Authentication;
using Sixnet.Security.Authorization;
using Sixnet.Security.Cryptography;
using Sixnet.Serialization.Json;
using Sixnet.Validation;

namespace Sixnet.DependencyInjection
{
    /// <summary>
    /// Sixnet configuration
    /// </summary>
    internal class SixnetConfiguration
    {
        /// <summary>
        /// Gets or sets file access options
        /// </summary>
        public SixnetFileOptions File { get; set; }

        /// <summary>
        /// Gets or sets rsa options
        /// </summary>
        public SixnetRsations Rsa { get; set; }

        /// <summary>
        /// Gets or sets data options
        /// </summary>
        public SixnetDataOptions Data { get; set; }

        /// <summary>
        /// Gets or sets cache options
        /// </summary>
        public SixnetCacheOptions Cache { get; set; }

        /// <summary>
        /// Gets or sets the email options
        /// </summary>
        public SixnetEmailOptions Email { get; set; }

        /// <summary>
        /// Gets or sets the sms options
        /// </summary>
        public SixnetSmsOptions Sms { get; set; }

        /// <summary>
        /// Gets or sets the message options
        /// </summary>
        public SixnetMessageOptions Message { get; set; }

        /// <summary>
        /// Gets or sets the message queue options
        /// </summary>
        public SixnetMessageQueueOptions MessageQueue { get; set; }

        /// <summary>
        /// Gets or sets the json options
        /// </summary>
        public SixnetJsonSerializationOptions Json { get; set; }

        /// <summary>
        /// Gets or sets the validation options
        /// </summary>
        public SixnetValidationOptions Validation { get; set; }

        /// <summary>
        /// Gets or sets the authorization options 
        /// </summary>
        public SixnetAuthorizationOptions Authorization { get; set; }

        /// <summary>
        /// Gets or sets the authentication options 
        /// </summary>
        public SixnetAuthenticationOptions Authentication { get; set; }

        /// <summary>
        /// Gets or sets the unitofwork options
        /// </summary>
        public SixnetUnitOfWorkOptions UnitOfWork { get; set; }

        /// <summary>
        /// Gets or sets the logging options
        /// </summary>
        public SixnetLoggingOptions Logging { get; set; }

        /// <summary>
        /// Gets or sets the entity options
        /// </summary>
        public SixnetEntityOptions Entity { get; set; }

        /// <summary>
        /// Gets or sets the enum options
        /// </summary>
        public SixnetEnumOptions Enum { get; set; }
    }
}
