// "Company © 2025. All rights reserved."

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

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
using Sixnet.Threading.Locking;
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

        Action<IHostBuilder> _configureHostBuilderAction;

        /// <summary>
        /// Configure host builder
        /// </summary>
        public SixnetOptions ConfigureHostBuilder(Action<IHostBuilder> configure, bool toFirst = false)
        {
            _configureHostBuilderAction = toFirst
                ? configure + _configureHostBuilderAction
                : _configureHostBuilderAction + configure;
            return this;
        }

        /// <summary>
        /// Configure host builder
        /// </summary>
        /// <param name="hostBuilder"></param>
        internal void InvokeConfigureHostBuilder(IHostBuilder hostBuilder)
        {
            _configureHostBuilderAction?.Invoke(hostBuilder);
        }

        /// <summary>
        /// Set host builder
        /// </summary>
        /// <param name="hostBuilder"></param>
        public void SetHostBuilder(IHostBuilder hostBuilder)
        {
            InvokeConfigureHostBuilder(hostBuilder);
            HostBuilder = hostBuilder;
        }

        #endregion

        #region Configure app

        Action<SixnetApplicationOptions> _configureAppAction;

        /// <summary>
        /// Register configure app action
        /// </summary>
        public SixnetOptions ConfigureApp(Action<SixnetApplicationOptions> configure, bool toFirst = false)
        {
            _configureAppAction = toFirst
                ? configure + _configureAppAction
                : _configureAppAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure app action
        /// </summary>
        /// <param name="options"></param>
        internal void InvokeConfigureApp(SixnetApplicationOptions options)
        {
            _configureAppAction?.Invoke(options);
        }

        #endregion

        #region Configure data

        Action<SixnetDataOptions> _configureDataAction;

        /// <summary>
        /// Register configure data action
        /// </summary>
        public SixnetOptions ConfigureData(Action<SixnetDataOptions> configure, bool toFirst = false)
        {
            _configureDataAction = toFirst
                ? configure + _configureDataAction
                : _configureDataAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure data action
        /// </summary>
        /// <param name="options"></param>
        internal void InvokeConfigureData(SixnetDataOptions options)
        {
            _configureDataAction?.Invoke(options);
        }

        #endregion

        #region Configure email

        Action<SixnetEmailOptions> _configureEmailAction;

        /// <summary>
        /// Register configure email action
        /// </summary>
        public SixnetOptions ConfigureEmail(Action<SixnetEmailOptions> configure, bool toFirst = false)
        {
            _configureEmailAction = toFirst
                ? configure + _configureEmailAction
                : _configureEmailAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure email action
        /// </summary>
        /// <param name="options"></param>
        internal void InvokeConfigureEmail(SixnetEmailOptions options)
        {
            _configureEmailAction?.Invoke(options);
        }

        #endregion

        #region Configure sms

        Action<SixnetSmsOptions> _configureSmsAction;

        /// <summary>
        /// Register configure sms action
        /// </summary>
        public SixnetOptions ConfigureSms(Action<SixnetSmsOptions> configure, bool toFirst = false)
        {
            _configureSmsAction = toFirst
                ? configure + _configureSmsAction
                : _configureSmsAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure sms action
        /// </summary>
        /// <param name="options"></param>
        internal void InvokeConfigureSms(SixnetSmsOptions options)
        {
            _configureSmsAction?.Invoke(options);
        }

        #endregion

        #region Configure message

        Action<SixnetMessageOptions> _configureMessageAction;

        /// <summary>
        /// Register configure message action
        /// </summary>
        public SixnetOptions ConfigureMessage(Action<SixnetMessageOptions> configure, bool toFirst = false)
        {
            _configureMessageAction = toFirst
                ? configure + _configureMessageAction
                : _configureMessageAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure message action
        /// </summary>
        internal void InvokeConfigureMessage(SixnetMessageOptions options)
        {
            _configureMessageAction?.Invoke(options);
        }

        #endregion

        #region Configure service

        Action<IServiceCollection> _configureServiceAction;

        /// <summary>
        /// Register configure service action
        /// </summary>
        public SixnetOptions ConfigureService(Action<IServiceCollection> configure, bool toFirst = false)
        {
            _configureServiceAction = toFirst
                ? configure + _configureServiceAction
                : _configureServiceAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure service action
        /// </summary>
        internal void InvokeConfigureService(IServiceCollection services)
        {
            _configureServiceAction?.Invoke(services);
        }

        #endregion

        #region Configure logging

        Action<SixnetLoggingOptions> _configureLoggingAction;

        /// <summary>
        /// Register configure logging action
        /// </summary>
        public SixnetOptions ConfigureLogging(Action<SixnetLoggingOptions> configure, bool toFirst = false)
        {
            _configureLoggingAction = toFirst
                ? configure + _configureLoggingAction
                : _configureLoggingAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure logging action
        /// </summary>
        internal void InvokeConfigureLogging(SixnetLoggingOptions options)
        {
            _configureLoggingAction?.Invoke(options);
        }

        #endregion

        #region Configure file

        Action<SixnetFileOptions> _configureFileAction;

        /// <summary>
        /// Register configure file action
        /// </summary>
        public SixnetOptions ConfigureFile(Action<SixnetFileOptions> configure, bool toFirst = false)
        {
            _configureFileAction = toFirst
                ? configure + _configureFileAction
                : _configureFileAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure file action
        /// </summary>
        internal void InvokeConfigureFile(SixnetFileOptions options)
        {
            _configureFileAction?.Invoke(options);
        }

        #endregion

        #region Configure RSA

        Action<SixnetRsaOptions> _configureRsaAction;

        /// <summary>
        /// Register configure RSA action
        /// </summary>
        public SixnetOptions ConfigureRSA(Action<SixnetRsaOptions> configure, bool toFirst = false)
        {
            _configureRsaAction = toFirst
                ? configure + _configureRsaAction
                : _configureRsaAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure RSA action
        /// </summary>
        internal void InvokeConfigureRSA(SixnetRsaOptions options)
        {
            _configureRsaAction?.Invoke(options);
        }

        #endregion

        #region Configure cache

        Action<SixnetCacheOptions> _configureCacheAction;

        /// <summary>
        /// Register configure cache action
        /// </summary>
        public SixnetOptions ConfigureCache(Action<SixnetCacheOptions> configure, bool toFirst = false)
        {
            _configureCacheAction = toFirst
                ? configure + _configureCacheAction
                : _configureCacheAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure cache action
        /// </summary>
        internal void InvokeConfigureCache(SixnetCacheOptions options)
        {
            _configureCacheAction?.Invoke(options);
        }

        #endregion

        #region Configure localization

        internal Action<SixnetLocalizationOptions> _configureLocalizationAction;

        /// <summary>
        /// Register configure localization action
        /// </summary>
        public SixnetOptions ConfigureLocalization(Action<SixnetLocalizationOptions> configure, bool toFirst = false)
        {
            _configureLocalizationAction = toFirst
                ? configure + _configureLocalizationAction
                : _configureLocalizationAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure localization action
        /// </summary>
        internal void InvokeConfigureLocalization(SixnetLocalizationOptions options)
        {
            _configureLocalizationAction?.Invoke(options);
        }

        #endregion

        #region Configure JSON

        Action<SixnetJsonSerializationOptions> _configureJsonAction;

        /// <summary>
        /// Register configure JSON action
        /// </summary>
        public SixnetOptions ConfigureJson(Action<SixnetJsonSerializationOptions> configure, bool toFirst = false)
        {
            _configureJsonAction = toFirst
                ? configure + _configureJsonAction
                : _configureJsonAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure JSON action
        /// </summary>
        internal void InvokeConfigureJson(SixnetJsonSerializationOptions options)
        {
            _configureJsonAction?.Invoke(options);
        }

        #endregion

        #region Configure message queue

        Action<SixnetMessageQueueOptions> _configureMessageQueueAction;

        /// <summary>
        /// Register configure message queue action
        /// </summary>
        public SixnetOptions ConfigureMessageQueue(Action<SixnetMessageQueueOptions> configure, bool toFirst = false)
        {
            _configureMessageQueueAction = toFirst
                ? configure + _configureMessageQueueAction
                : _configureMessageQueueAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure message queue action
        /// </summary>
        internal void InvokeConfigureMessageQueue(SixnetMessageQueueOptions options)
        {
            _configureMessageQueueAction?.Invoke(options);
        }

        #endregion

        #region Configure validation

        Action<SixnetValidationOptions> _configureValidationAction;

        /// <summary>
        /// Register configure validation action
        /// </summary>
        public SixnetOptions ConfigureValidation(Action<SixnetValidationOptions> configure, bool toFirst = false)
        {
            _configureValidationAction = toFirst
                ? configure + _configureValidationAction
                : _configureValidationAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure validation action
        /// </summary>
        internal void InvokeConfigureValidation(SixnetValidationOptions options)
        {
            _configureValidationAction?.Invoke(options);
        }

        #endregion

        #region Configure authorization

        Action<SixnetAuthorizationOptions> _configureAuthorizationAction;

        /// <summary>
        /// Register configure authorization action
        /// </summary>
        public SixnetOptions ConfigureAuthorization(Action<SixnetAuthorizationOptions> configure, bool toFirst = false)
        {
            _configureAuthorizationAction = toFirst
                ? configure + _configureAuthorizationAction
                : _configureAuthorizationAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure authorization action
        /// </summary>
        internal void InvokeConfigureAuthorization(SixnetAuthorizationOptions options)
        {
            _configureAuthorizationAction?.Invoke(options);
        }

        #endregion

        #region Configure authentication

        Action<SixnetAuthenticationOptions> _configureAuthenticationAction;

        /// <summary>
        /// Register configure authentication action
        /// </summary>
        public SixnetOptions ConfigureAuthentication(Action<SixnetAuthenticationOptions> configure, bool toFirst = false)
        {
            _configureAuthenticationAction = toFirst
                ? configure + _configureAuthenticationAction
                : _configureAuthenticationAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure authentication action
        /// </summary>
        internal void InvokeConfigureAuthentication(SixnetAuthenticationOptions options)
        {
            _configureAuthenticationAction?.Invoke(options);
        }

        #endregion

        #region Configure unit of work

        Action<SixnetUnitOfWorkOptions> _configureUnitOfWorkAction;

        /// <summary>
        /// Register configure unit of work action
        /// </summary>
        public SixnetOptions ConfigureUnitOfWork(Action<SixnetUnitOfWorkOptions> configure, bool toFirst = false)
        {
            _configureUnitOfWorkAction = toFirst
                ? configure + _configureUnitOfWorkAction
                : _configureUnitOfWorkAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure unit of work action
        /// </summary>
        internal void InvokeConfigureUnitOfWork(SixnetUnitOfWorkOptions options)
        {
            _configureUnitOfWorkAction?.Invoke(options);
        }

        #endregion

        #region Configure entity

        Action<SixnetEntityOptions> _configureEntityAction;

        /// <summary>
        /// Register configure entity action
        /// </summary>
        public SixnetOptions ConfigureEntity(Action<SixnetEntityOptions> configure, bool toFirst = false)
        {
            _configureEntityAction = toFirst
                ? configure + _configureEntityAction
                : _configureEntityAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure entity action
        /// </summary>
        internal void InvokeConfigureEntity(SixnetEntityOptions options)
        {
            _configureEntityAction?.Invoke(options);
        }

        #endregion

        #region Configure enum

        Action<SixnetEnumOptions> _configureEnumAction;

        /// <summary>
        /// Register configure enum action
        /// </summary>
        public SixnetOptions ConfigureEnum(Action<SixnetEnumOptions> configure, bool toFirst = false)
        {
            _configureEnumAction = toFirst
                ? configure + _configureEnumAction
                : _configureEnumAction + configure;
            return this;
        }

        /// <summary>
        /// Invoke the registered configure enum action
        /// </summary>
        internal void InvokeConfigureEnum(SixnetEnumOptions options)
        {
            _configureEnumAction?.Invoke(options);
        }

        #endregion

        #region Configure lock

        internal Action<SixnetLockOptions> _configureSixnetLockAction;

        /// <summary>
        /// Configure lock
        /// </summary>
        /// <param name="configure"></param>
        /// <param name="toFirst"></param>
        /// <returns></returns>
        public SixnetOptions ConfigureLock(Action<SixnetLockOptions> configure, bool toFirst = false)
        {
            _configureSixnetLockAction = toFirst
                ? configure + _configureSixnetLockAction
                : _configureSixnetLockAction + configure;
            return this;
        }

        internal void InvokeConfigureLock(SixnetLockOptions options)
        {
            _configureSixnetLockAction?.Invoke(options);
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
