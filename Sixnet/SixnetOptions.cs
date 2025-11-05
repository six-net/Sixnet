// "Company © 2025. All rights reserved."

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Sixnet.App;
using Sixnet.Cache;
using Sixnet.Development.Data;
using Sixnet.Development.Entity;
using Sixnet.Development.Message;
using Sixnet.Development.Work;
using Sixnet.Exceptions;
using Sixnet.Extensions;
using Sixnet.IO;
using Sixnet.Localization;
using Sixnet.Logging;
using Sixnet.MQ;
using Sixnet.Net.Email;
using Sixnet.Net.Sms;
using Sixnet.Security.Authentication;
using Sixnet.Security.Authorization;
using Sixnet.Security.Cryptography;
using Sixnet.Serialization.Json;
using Sixnet.Validation;

namespace Sixnet
{
    /// <summary>
    /// Sixnet options
    /// </summary>
    public class SixnetOptions
    {
        /// <summary>
        /// Host builder
        /// </summary>
        internal protected IHostBuilder HostBuilder { get; set; }

        /// <summary>
        /// Options styyles
        /// </summary>
        readonly Dictionary<Guid, SixnetOptionsStyle> _optionsStyles = new();

        /// <summary>
        /// Gets or sets the args
        /// </summary>
        public string[] Args { get; set; }

        /// <summary>
        /// Whether not auto init entity id
        /// </summary>
        public bool NotAutoInitEntityId { get; set; }

        /// <summary>
        /// Whether not auto execute initializable
        /// </summary>
        public bool NotAutoExecuteInitializable { get; set; }

        /// <summary>
        /// Services
        /// </summary>
        public IServiceCollection Services { get; set; }

        /// <summary>
        /// Configure host builder
        /// </summary>
        public Action<IHostBuilder> ConfigureHostBuilder { get; set; }

        /// <summary>
        /// Configure app
        /// </summary>
        public Action<SixnetApplicationOptions> ConfigureApp { get; set; }

        /// <summary>
        /// Configure data
        /// </summary>
        public Action<SixnetDataOptions> ConfigureData { get; set; }

        /// <summary>
        /// Configure email
        /// </summary>
        public Action<SixnetEmailOptions> ConfigureEmail { get; set; }

        /// <summary>
        /// Configure sms
        /// </summary>
        public Action<SixnetSmsOptions> ConfigureSms { get; set; }

        /// <summary>
        /// Configure messag
        /// </summary>
        public Action<SixnetMessageOptions> ConfigureMessage { get; set; }

        /// <summary>
        /// Configure service
        /// </summary>
        public Action<IServiceCollection> ConfigureService { get; set; }

        /// <summary>
        /// Configure logging
        /// </summary>
        public Action<SixnetLoggingOptions> ConfigureLogging { get; set; }

        /// <summary>
        /// Configure file
        /// </summary>
        public Action<SixnetFileOptions> ConfigureFile { get; set; }

        /// <summary>
        /// Configure rsa
        /// </summary>
        public Action<SixnetRsaOptions> ConfigureRSA { get; set; }

        /// <summary>
        /// Configure cache
        /// </summary>
        public Action<SixnetCacheOptions> ConfigureCache { get; set; }

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
        public Action<SixnetMessageQueueOptions> ConfigureMessageQueue { get; set; }

        /// <summary>
        /// Configure validation
        /// </summary>
        public Action<SixnetValidationOptions> ConfigureValidation { get; set; }

        /// <summary>
        /// Configure authorization
        /// </summary>
        public Action<SixnetAuthorizationOptions> ConfigureAuthorization { get; set; }

        /// <summary>
        /// Configure authentication
        /// </summary>
        public Action<SixnetAuthenticationOptions> ConfigureAuthentication { get; set; }

        /// <summary>
        /// Configure unitofwork
        /// </summary>
        public Action<SixnetUnitOfWorkOptions> ConfigureUnitOfWork { get; set; }

        /// <summary>
        /// Configure entity
        /// </summary>
        public Action<SixnetEntityOptions> ConfigureEntity { get; set; }

        /// <summary>
        /// Configure enum
        /// </summary>
        public Action<SixnetEnumOptions> ConfigureEnum { get; set; }

        /// <summary>
        /// Set options style
        /// </summary>
        /// <typeparam name="TOptions"></typeparam>
        /// <param name="style"></param>
        public void SetOptionsStyle<TOptions>(SixnetOptionsStyle style)
        {
            SetOptionsStyle(typeof(TOptions), style);
        }

        /// <summary>
        /// Set options style
        /// </summary>
        /// <param name="optionsType"></param>
        /// <param name="style"></param>
        public void SetOptionsStyle(Type optionsType, SixnetOptionsStyle style)
        {
            if (optionsType != null)
            {
                _optionsStyles[optionsType.GUID] = style;
            }
        }

        /// <summary>
        /// Application started
        /// </summary>
        public Action<SixnetOptions> ApplicationStarted { get; set; }

        /// <summary>
        /// Application stopping
        /// </summary>
        public Action<SixnetOptions> ApplicationStopping { get; set; }

        /// <summary>
        /// Application stopped
        /// </summary>
        public Action<SixnetOptions> ApplicationStopped { get; set; }

        /// <summary>
        /// Get options style
        /// </summary>
        /// <param name="optionsType"></param>
        /// <returns></returns>
        internal SixnetOptionsStyle GetOptionsStyle(Type optionsType)
        {
            SixnetDirectThrower.ThrowArgNullIf(optionsType == null, nameof(optionsType));
            var typeGuid = optionsType.GUID;
            if (_optionsStyles.ContainsKey(typeGuid))
            {
                return _optionsStyles[typeGuid];
            }
            return SixnetOptionsStyle.Constant;
        }
    }

    /// <summary>
    /// Sixnet options style
    /// </summary>
    public enum SixnetOptionsStyle
    {
        Constant = 1,
        Snapshot = 2,
        Monitor = 3
    }
}
