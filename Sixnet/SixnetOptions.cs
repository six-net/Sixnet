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
        #region Fields

        /// <summary>
        /// Host builder
        /// </summary>
        internal protected IHostBuilder HostBuilder { get; set; }

        /// <summary>
        /// Options styyles
        /// </summary>
        readonly Dictionary<Guid, SixnetOptionsStyle> _optionsStyles = new();

        #endregion

        #region Properties

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
        /// Whether trace framework log
        /// </summary>
        public bool TraceFrameworkLog { get; set; }

        #endregion

        #region Methods

        #region Configure host builder

        internal Action<IHostBuilder> ConfigureHostBuilderAction;

        /// <summary>
        /// Configure host builder
        /// </summary>
        public SixnetOptions ConfigureHostBuilder(Action<IHostBuilder> configure, bool toFirst = false)
        {
            ConfigureHostBuilderAction = toFirst
                ? configure + ConfigureHostBuilderAction
                : ConfigureHostBuilderAction + configure;
            return this;
        }

        /// <summary>
        /// Configure host builder
        /// </summary>
        /// <param name="hostBuilder"></param>
        internal protected void ConfigureHostBuilder(IHostBuilder hostBuilder)
        {
            ConfigureHostBuilderAction?.Invoke(hostBuilder);
        }

        #endregion

        #region Configure app

        internal Action<SixnetApplicationOptions> ConfigureAppAction;

        /// <summary>
        /// Register configure app action
        /// </summary>
        public SixnetOptions ConfigureApp(Action<SixnetApplicationOptions> configure, bool toFirst = false)
        {
            ConfigureAppAction = toFirst
                ? configure + ConfigureAppAction
                : ConfigureAppAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure app action
        /// </summary>
        /// <param name="options"></param>
        internal void ConfigureApp(SixnetApplicationOptions options)
        {
            ConfigureAppAction?.Invoke(options);
        }

        #endregion

        #region Configure data

        internal Action<SixnetDataOptions> ConfigureDataAction;

        /// <summary>
        /// Register configure data action
        /// </summary>
        public SixnetOptions ConfigureData(Action<SixnetDataOptions> configure, bool toFirst = false)
        {
            ConfigureDataAction = toFirst
                ? configure + ConfigureDataAction
                : ConfigureDataAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure data action
        /// </summary>
        /// <param name="options"></param>
        internal void ConfigureData(SixnetDataOptions options)
        {
            ConfigureDataAction?.Invoke(options);
        }

        #endregion

        #region Configure email

        internal Action<SixnetEmailOptions> ConfigureEmailAction;

        /// <summary>
        /// Register configure email action
        /// </summary>
        public SixnetOptions ConfigureEmail(Action<SixnetEmailOptions> configure, bool toFirst = false)
        {
            ConfigureEmailAction = toFirst
                ? configure + ConfigureEmailAction
                : ConfigureEmailAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure email action
        /// </summary>
        /// <param name="options"></param>
        internal void ConfigureEmail(SixnetEmailOptions options)
        {
            ConfigureEmailAction?.Invoke(options);
        }

        #endregion

        #region Configure sms

        internal Action<SixnetSmsOptions> ConfigureSmsAction;

        /// <summary>
        /// Register configure sms action
        /// </summary>
        public SixnetOptions ConfigureSms(Action<SixnetSmsOptions> configure, bool toFirst = false)
        {
            ConfigureSmsAction = toFirst
                ? configure + ConfigureSmsAction
                : ConfigureSmsAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure sms action
        /// </summary>
        /// <param name="options"></param>
        internal void ConfigureSms(SixnetSmsOptions options)
        {
            ConfigureSmsAction?.Invoke(options);
        }

        #endregion

        #region Configure message

        internal Action<SixnetMessageOptions> ConfigureMessageAction;

        /// <summary>
        /// Register configure message action
        /// </summary>
        public SixnetOptions ConfigureMessage(Action<SixnetMessageOptions> configure, bool toFirst = false)
        {
            ConfigureMessageAction = toFirst
                ? configure + ConfigureMessageAction
                : ConfigureMessageAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure message action
        /// </summary>
        internal void ConfigureMessage(SixnetMessageOptions options)
        {
            ConfigureMessageAction?.Invoke(options);
        }

        #endregion

        #region Configure service

        internal Action<IServiceCollection> ConfigureServiceAction;

        /// <summary>
        /// Register configure service action
        /// </summary>
        public SixnetOptions ConfigureService(Action<IServiceCollection> configure, bool toFirst = false)
        {
            ConfigureServiceAction = toFirst
                ? configure + ConfigureServiceAction
                : ConfigureServiceAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure service action
        /// </summary>
        internal void ConfigureService(IServiceCollection services)
        {
            ConfigureServiceAction?.Invoke(services);
        }

        #endregion

        #region Configure logging

        internal Action<SixnetLoggingOptions> ConfigureLoggingAction;

        /// <summary>
        /// Register configure logging action
        /// </summary>
        public SixnetOptions ConfigureLogging(Action<SixnetLoggingOptions> configure, bool toFirst = false)
        {
            ConfigureLoggingAction = toFirst
                ? configure + ConfigureLoggingAction
                : ConfigureLoggingAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure logging action
        /// </summary>
        internal void ConfigureLogging(SixnetLoggingOptions options)
        {
            ConfigureLoggingAction?.Invoke(options);
        }

        #endregion

        #region Configure file

        internal Action<SixnetFileOptions> ConfigureFileAction;

        /// <summary>
        /// Register configure file action
        /// </summary>
        public SixnetOptions ConfigureFile(Action<SixnetFileOptions> configure, bool toFirst = false)
        {
            ConfigureFileAction = toFirst
                ? configure + ConfigureFileAction
                : ConfigureFileAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure file action
        /// </summary>
        internal void ConfigureFile(SixnetFileOptions options)
        {
            ConfigureFileAction?.Invoke(options);
        }

        #endregion

        #region Configure RSA

        internal Action<SixnetRsaOptions> ConfigureRsaAction;

        /// <summary>
        /// Register configure RSA action
        /// </summary>
        public SixnetOptions ConfigureRSA(Action<SixnetRsaOptions> configure, bool toFirst = false)
        {
            ConfigureRsaAction = toFirst
                ? configure + ConfigureRsaAction
                : ConfigureRsaAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure RSA action
        /// </summary>
        internal void ConfigureRSA(SixnetRsaOptions options)
        {
            ConfigureRsaAction?.Invoke(options);
        }

        #endregion

        #region Configure cache

        internal Action<SixnetCacheOptions> ConfigureCacheAction;

        /// <summary>
        /// Register configure cache action
        /// </summary>
        public SixnetOptions ConfigureCache(Action<SixnetCacheOptions> configure, bool toFirst = false)
        {
            ConfigureCacheAction = toFirst
                ? configure + ConfigureCacheAction
                : ConfigureCacheAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure cache action
        /// </summary>
        internal void ConfigureCache(SixnetCacheOptions options)
        {
            ConfigureCacheAction?.Invoke(options);
        }

        #endregion

        #region Configure localization

        internal Action<SixnetLocalizationOptions> ConfigureLocalizationAction;

        /// <summary>
        /// Register configure localization action
        /// </summary>
        public SixnetOptions ConfigureLocalization(Action<SixnetLocalizationOptions> configure, bool toFirst = false)
        {
            ConfigureLocalizationAction = toFirst
                ? configure + ConfigureLocalizationAction
                : ConfigureLocalizationAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure localization action
        /// </summary>
        internal void ConfigureLocalization(SixnetLocalizationOptions options)
        {
            ConfigureLocalizationAction?.Invoke(options);
        }

        #endregion

        #region Configure JSON

        internal Action<SixnetJsonSerializationOptions> ConfigureJsonAction;

        /// <summary>
        /// Register configure JSON action
        /// </summary>
        public SixnetOptions ConfigureJson(Action<SixnetJsonSerializationOptions> configure, bool toFirst = false)
        {
            ConfigureJsonAction = toFirst
                ? configure + ConfigureJsonAction
                : ConfigureJsonAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure JSON action
        /// </summary>
        internal void ConfigureJson(SixnetJsonSerializationOptions options)
        {
            ConfigureJsonAction?.Invoke(options);
        }

        #endregion

        #region Configure message queue

        internal Action<SixnetMessageQueueOptions> ConfigureMessageQueueAction;

        /// <summary>
        /// Register configure message queue action
        /// </summary>
        public SixnetOptions ConfigureMessageQueue(Action<SixnetMessageQueueOptions> configure, bool toFirst = false)
        {
            ConfigureMessageQueueAction = toFirst
                ? configure + ConfigureMessageQueueAction
                : ConfigureMessageQueueAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure message queue action
        /// </summary>
        internal void ConfigureMessageQueue(SixnetMessageQueueOptions options)
        {
            ConfigureMessageQueueAction?.Invoke(options);
        }

        #endregion

        #region Configure validation

        internal Action<SixnetValidationOptions> ConfigureValidationAction;

        /// <summary>
        /// Register configure validation action
        /// </summary>
        public SixnetOptions ConfigureValidation(Action<SixnetValidationOptions> configure, bool toFirst = false)
        {
            ConfigureValidationAction = toFirst
                ? configure + ConfigureValidationAction
                : ConfigureValidationAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure validation action
        /// </summary>
        internal void ConfigureValidation(SixnetValidationOptions options)
        {
            ConfigureValidationAction?.Invoke(options);
        }

        #endregion

        #region Configure authorization

        internal Action<SixnetAuthorizationOptions> ConfigureAuthorizationAction;

        /// <summary>
        /// Register configure authorization action
        /// </summary>
        public SixnetOptions ConfigureAuthorization(Action<SixnetAuthorizationOptions> configure, bool toFirst = false)
        {
            ConfigureAuthorizationAction = toFirst
                ? configure + ConfigureAuthorizationAction
                : ConfigureAuthorizationAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure authorization action
        /// </summary>
        internal void ConfigureAuthorization(SixnetAuthorizationOptions options)
        {
            ConfigureAuthorizationAction?.Invoke(options);
        }

        #endregion

        #region Configure authentication

        internal Action<SixnetAuthenticationOptions> ConfigureAuthenticationAction;

        /// <summary>
        /// Register configure authentication action
        /// </summary>
        public SixnetOptions ConfigureAuthentication(Action<SixnetAuthenticationOptions> configure, bool toFirst = false)
        {
            ConfigureAuthenticationAction = toFirst
                ? configure + ConfigureAuthenticationAction
                : ConfigureAuthenticationAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure authentication action
        /// </summary>
        internal void ConfigureAuthentication(SixnetAuthenticationOptions options)
        {
            ConfigureAuthenticationAction?.Invoke(options);
        }

        #endregion

        #region Configure unit of work

        internal Action<SixnetUnitOfWorkOptions> ConfigureUnitOfWorkAction;

        /// <summary>
        /// Register configure unit of work action
        /// </summary>
        public SixnetOptions ConfigureUnitOfWork(Action<SixnetUnitOfWorkOptions> configure, bool toFirst = false)
        {
            ConfigureUnitOfWorkAction = toFirst
                ? configure + ConfigureUnitOfWorkAction
                : ConfigureUnitOfWorkAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure unit of work action
        /// </summary>
        internal void ConfigureUnitOfWork(SixnetUnitOfWorkOptions options)
        {
            ConfigureUnitOfWorkAction?.Invoke(options);
        }

        #endregion

        #region Configure entity

        internal Action<SixnetEntityOptions> ConfigureEntityAction;

        /// <summary>
        /// Register configure entity action
        /// </summary>
        public SixnetOptions ConfigureEntity(Action<SixnetEntityOptions> configure, bool toFirst = false)
        {
            ConfigureEntityAction = toFirst
                ? configure + ConfigureEntityAction
                : ConfigureEntityAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure entity action
        /// </summary>
        internal void ConfigureEntity(SixnetEntityOptions options)
        {
            ConfigureEntityAction?.Invoke(options);
        }

        #endregion

        #region Configure enum

        internal Action<SixnetEnumOptions> ConfigureEnumAction;

        /// <summary>
        /// Register configure enum action
        /// </summary>
        public SixnetOptions ConfigureEnum(Action<SixnetEnumOptions> configure, bool toFirst = false)
        {
            ConfigureEnumAction = toFirst
                ? configure + ConfigureEnumAction
                : ConfigureEnumAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure enum action
        /// </summary>
        internal void ConfigureEnum(SixnetEnumOptions options)
        {
            ConfigureEnumAction?.Invoke(options);
        }

        #endregion

        #region Options

        /// <summary>
        /// Set options style
        /// </summary>
        /// <typeparam name="TOptions"></typeparam>
        /// <param name="style"></param>
        public SixnetOptions SetOptionsStyle<TOptions>(SixnetOptionsStyle style)
        {
            return SetOptionsStyle(typeof(TOptions), style);
        }

        /// <summary>
        /// Set options style
        /// </summary>
        /// <param name="optionsType"></param>
        /// <param name="style"></param>
        public SixnetOptions SetOptionsStyle(Type optionsType, SixnetOptionsStyle style)
        {
            if (optionsType != null)
            {
                _optionsStyles[optionsType.GUID] = style;
            }
            return this;
        }

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

        #endregion

        #region Application started

        internal Action<SixnetOptions> ConfigureApplicationStartedAction;

        /// <summary>
        /// Register application started action
        /// </summary>
        public SixnetOptions ConfigureApplicationStarted(Action<SixnetOptions> configure, bool toFirst = false)
        {
            ConfigureApplicationStartedAction = toFirst
                ? configure + ConfigureApplicationStartedAction
                : ConfigureApplicationStartedAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered application started action
        /// </summary>
        internal void ApplicationStarted(SixnetOptions options)
        {
            ConfigureApplicationStartedAction?.Invoke(options);
        }

        #endregion

        #region Application stopping

        internal Action<SixnetOptions> ConfigureApplicationStoppingAction;

        /// <summary>
        /// Register application stopping action
        /// </summary>
        public SixnetOptions ConfigureApplicationStopping(Action<SixnetOptions> configure, bool toFirst = false)
        {
            ConfigureApplicationStoppingAction = toFirst
                ? configure + ConfigureApplicationStoppingAction
                : ConfigureApplicationStoppingAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered application stopping action
        /// </summary>
        internal void ApplicationStopping(SixnetOptions options)
        {
            ConfigureApplicationStoppingAction?.Invoke(options);
        }

        #endregion

        #region Application stopped

        internal Action<SixnetOptions> ConfigureApplicationStoppedAction;

        /// <summary>
        /// Register application stopped action
        /// </summary>
        public SixnetOptions ConfigureApplicationStopped(Action<SixnetOptions> configure, bool toFirst = false)
        {
            ConfigureApplicationStoppedAction = toFirst
                ? configure + ConfigureApplicationStoppedAction
                : ConfigureApplicationStoppedAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered application stopped action
        /// </summary>
        internal void ApplicationStopped(SixnetOptions options)
        {
            ConfigureApplicationStoppedAction?.Invoke(options);
        }

        #endregion

        #endregion
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
