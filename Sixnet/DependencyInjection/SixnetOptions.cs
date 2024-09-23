using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sixnet.App;
using Sixnet.Cache;
using Sixnet.Development.Data;
using Sixnet.Development.Message;
using Sixnet.Development.Work;
using Sixnet.Exceptions;
using Sixnet.IO.FileAccess;
using Sixnet.Localization;
using Sixnet.Logging;
using Sixnet.MQ;
using Sixnet.Net.Email;
using Sixnet.Net.Sms;
using Sixnet.Net.Upload;
using Sixnet.Security.Authorization;
using Sixnet.Security.Cryptography;
using Sixnet.Serialization.Json;
using Sixnet.Token.Jwt;
using Sixnet.Validation;
using System;
using System.Collections.Generic;

namespace Sixnet.DependencyInjection
{
    /// <summary>
    /// Sixnet options
    /// </summary>
    public class SixnetOptions
    {
        /// <summary>
        /// Options styyles
        /// </summary>
        readonly Dictionary<Guid, OptionsStyle> _optionsStyles = new();

        /// <summary>
        /// Services
        /// </summary>
        public IServiceCollection Services { get; set; }

        /// <summary>
        /// Configure app
        /// </summary>
        public Action<ApplicationOptions> ConfigureApp { get; set; }

        /// <summary>
        /// Configure data
        /// </summary>
        public Action<DataOptions> ConfigureData { get; set; }

        /// <summary>
        /// Configure email
        /// </summary>
        public Action<EmailOptions> ConfigureEmail { get; set; }

        /// <summary>
        /// Configure sms
        /// </summary>
        public Action<SmsOptions> ConfigureSms { get; set; }

        /// <summary>
        /// Configure messag
        /// </summary>
        public Action<MessageOptions> ConfigureMessage { get; set; }

        /// <summary>
        /// Configure service
        /// </summary>
        public Action<IServiceCollection> ConfigureService { get; set; }

        /// <summary>
        /// Configure logging
        /// </summary>
        public Action<SixnetLoggingOptions> ConfigureLogging { get; set; }

        /// <summary>
        /// Configure logging builder
        /// </summary>
        public Action<ILoggingBuilder> ConfigureLoggingBuilder { get; set; }

        /// <summary>
        /// Configure upload
        /// </summary>
        public Action<UploadOptions> ConfigureUpload { get; set; }

        /// <summary>
        /// Configure file access
        /// </summary>
        public Action<FileAccessOptions> ConfigureFileAccess { get; set; }

        /// <summary>
        /// Configure rsa
        /// </summary>
        public Action<RSAOptions> ConfigureRSA { get; set; }

        /// <summary>
        /// Configure jwt
        /// </summary>
        public Action<JwtOptions> ConfigureJwt { get; set; }

        /// <summary>
        /// Configure cache
        /// </summary>
        public Action<CacheOptions> ConfigureCache { get; set; }

        /// <summary>
        /// Configure localization
        /// </summary>
        public Action<SixnetLocalizationOptions> ConfigureLocalization { get; set; }

        /// <summary>
        /// Gets or sets configure json
        /// </summary>
        public Action<SixnetJsonSerializationOptions> ConfigureJson { get; set; }

        /// <summary>
        /// Configure message queue
        /// </summary>
        public Action<MessageQueueOptions> ConfigureMessageQueue { get; set; }

        /// <summary>
        /// Configure validation
        /// </summary>
        public Action<SixnetValidationOptions> ConfigureValidation { get; set; }

        /// <summary>
        /// Configure authorization
        /// </summary>
        public Action<SixnetAuthorizationOptions> ConfigureAuthorization { get; set; }
        
        /// <summary>
        /// Configure unitofwork
        /// </summary>
        public Action<UnitOfWorkOptions> ConfigureUnitOfWork { get; set; }

        /// <summary>
        /// Set options style
        /// </summary>
        /// <typeparam name="TOptions"></typeparam>
        /// <param name="style"></param>
        public void SetOptionsStyle<TOptions>(OptionsStyle style)
        {
            SetOptionsStyle(typeof(TOptions), style);
        }

        /// <summary>
        /// Set options style
        /// </summary>
        /// <param name="optionsType"></param>
        /// <param name="style"></param>
        public void SetOptionsStyle(Type optionsType, OptionsStyle style)
        {
            if (optionsType != null)
            {
                _optionsStyles[optionsType.GUID] = style;
            }
        }

        /// <summary>
        /// Get options style
        /// </summary>
        /// <param name="optionsType"></param>
        /// <returns></returns>
        internal OptionsStyle GetOptionsStyle(Type optionsType)
        {
            SixnetDirectThrower.ThrowArgNullIf(optionsType == null, nameof(optionsType));
            var typeGuid = optionsType.GUID;
            if (_optionsStyles.ContainsKey(typeGuid))
            {
                return _optionsStyles[typeGuid];
            }
            return OptionsStyle.Constant;
        }
    }
}
