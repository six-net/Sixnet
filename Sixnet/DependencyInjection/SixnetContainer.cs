using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sixnet.App;
using Sixnet.Cache;
using Sixnet.Cache.Provider.Memory;
using Sixnet.Development.Data;
using Sixnet.Development.Message;
using Sixnet.Development.Repository;
using Sixnet.Exceptions;
using Sixnet.IO.FileAccess;
using Sixnet.Mapper;
using Sixnet.MQ;
using Sixnet.Net.Email;
using Sixnet.Net.Sms;
using Sixnet.Net.Upload;
using Sixnet.Security.Cryptography;
using Sixnet.Token.Jwt;

namespace Sixnet.DependencyInjection
{
    /// <summary>
    /// Sixnet container
    /// </summary>
    public static class SixnetContainer
    {
        #region Fields

        /// <summary>
        /// Default service collection
        /// </summary>
        static IServiceCollection _serviceCollection = null;

        /// <summary>
        /// Sixnet options
        /// </summary>
        internal static SixnetOptions Options = new();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current DI container
        /// </summary>
        public static ISixnetContainer Container { get; private set; } = null;

        /// <summary>
        /// Gets the current service provider
        /// </summary>
        public static IServiceProvider ServiceProvider { get; private set; } = null;

        #endregion

        #region Methods

        #region Configure

        /// <summary>
        /// Configure sixnet
        /// </summary>
        /// <param name="configure">Configure</param>
        public static IServiceCollection Configure(Action<SixnetOptions> configure = null)
        {
            configure?.Invoke(Options);
            return ConfigureCore();
        }

        /// <summary>
        /// Configure sixnet
        /// </summary>
        /// <param name="options">Sixnet options</param>
        /// <returns></returns>
        public static IServiceCollection Configure(SixnetOptions options)
        {
            // Set options
            if (options != null)
            {
                Options = options;
            }
            return ConfigureCore();
        }

        /// <summary>
        /// Configure core
        /// </summary>
        /// <returns></returns>
        static IServiceCollection ConfigureCore()
        {
            // Configure application
            SixnetApplication.Configure(Options.ConfigureApp);
            var appOptions = SixnetApplication.Options;

            // Init services
            _serviceCollection = Options.Services ?? new ServiceCollection();

            // Build service provider
            BuildServiceProvider(true);

            // Init application
            SixnetApplication.Init();

            // Register default project service
            ConfigureProjectDefaultOptions(_serviceCollection, Options);
            AddProjectDefaultService(_serviceCollection, Options);

            // Configure service
            Options.ConfigureService?.Invoke(_serviceCollection);

            // Container
            var container = SixnetApplication.Options.DIContainer ?? new DefaultServiceProviderContainer();
            if (_serviceCollection != null && container is not DefaultServiceProviderContainer)
            {
                container.AddService(_serviceCollection.ToArray());
            }
            Container = container;

            //Build service provider
            BuildServiceProvider(true);

            //Init module
            SixnetApplication.InitModules();

            //Object mapper
            SixnetMapper.BuildMapper();

            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect();

            return _serviceCollection;
        }

        #endregion

        #region Add service

        /// <summary>
        /// Add service.
        /// Use singleton lifetime by default
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <typeparam name="TImplementation">Service implementation</typeparam>
        /// <param name="behaviors">Behaviors</param>
        public static void AddService<TService, TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Singleton, IEnumerable<Type> behaviors = null)
        {
            AddService(typeof(TService), typeof(TImplementation), lifetime, behaviors);
        }

        /// <summary>
        /// Add service.
        /// Use singleton lifetime by default
        /// </summary>
        /// <param name="serviceType">Service type</param>
        /// <param name="implementationType">Service implementation type</param>
        /// <param name="lifetime">Life time(default:singleton)</param>
        /// <param name="behaviors">Behaviors</param>
        public static void AddService(Type serviceType, Type implementationType, ServiceLifetime lifetime = ServiceLifetime.Singleton, IEnumerable<Type> behaviors = null)
        {
            AddService(new SixnetServiceDescriptor(serviceType, implementationType, lifetime)
            {
                Behaviors = behaviors
            });
        }

        /// <summary>
        /// Add service
        /// </summary>
        /// <param name="serviceDescriptors">Service descriptors</param>
        public static void AddService(params ServiceDescriptor[] serviceDescriptors)
        {
            if (!serviceDescriptors.IsNullOrEmpty())
            {
                foreach (var sd in serviceDescriptors)
                {
                    _serviceCollection.Add(sd);
                }
            }
        }

        #endregion

        #region Get service

        /// <summary>
        /// Get service
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <returns></returns>
        public static TService GetService<TService>()
        {
            return (TService)GetService(typeof(TService));
        }

        /// <summary>
        /// Get service
        /// </summary>
        /// <param name="serviceType">Service type</param>
        /// <returns></returns>
        public static object GetService(Type serviceType)
        {
            return Container != null
                ? Container.GetService(serviceType)
                : GetDefaultService(serviceType);
        }

        /// <summary>
        /// Get added or default service
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <typeparam name="TDefault">Default service implementation</typeparam>
        /// <returns></returns>
        public static TService GetService<TService, TDefault>() where TDefault : TService, new()
        {
            var service = GetService<TService>();
            service ??= new TDefault();
            return service;
        }

        /// <summary>
        /// Get default service
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <returns></returns>
        internal static TService GetDefaultService<TService>()
        {
            var service = GetDefaultService(typeof(TService));
            return service == null ? default : (TService)service;
        }

        /// <summary>
        /// Get default service
        /// </summary>
        /// <param name="serviceType">Service type</param>
        /// <returns></returns>
        internal static object GetDefaultService(Type serviceType)
        {
            if (ServiceProvider != null && serviceType != null)
            {
                return ServiceProvider.GetService(serviceType);
            }
            return null;
        }

        #endregion

        #region Has service

        /// <summary>
        /// Determine whether has added the service
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <returns>Return whether service has registered</returns>
        public static bool HasService<TService>()
        {
            return Container?.HasService<TService>() ?? HasDefaultService<TService>();
        }

        /// <summary>
        /// Whether added the service
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <returns>Return whether the service is registered</returns>
        internal static bool HasDefaultService<TService>()
        {
            return _serviceCollection?.Any(c => c.ServiceType == typeof(TService)) ?? false;
        }

        #endregion

        #region Service provider

        /// <summary>
        /// Build service provider
        /// </summary>
        /// <returns>Service service provider</returns>
        internal static IServiceProvider BuildServiceProvider(bool refresh = false)
        {
            if (ServiceProvider == null || refresh)
            {
                ServiceProvider = null;
                if (Container != null && Container is not DefaultServiceProviderContainer)
                {
                    ServiceProvider = Container.BuildServiceProvider();
                }
                ServiceProvider ??= _serviceCollection?.BuildServiceProvider();
            }
            return ServiceProvider;
        }

        #endregion

        #region Options

        /// <summary>
        /// Registers a configuration instance which TOptions will bind against.
        /// </summary>
        /// <typeparam name="TOptions">The type of options being configured.</typeparam>
        /// <param name="configureBinder">Configure binder</param>
        /// <param name="configurationSectionKey">Configuration section key</param>
        public static void ConfigureOptions<TOptions>(string configurationSectionKey = "", Action<BinderOptions> configureBinder = null) where TOptions : class
        {
            var configuration = GetService<IConfiguration>() ?? throw new SixnetException("Not initialize configuration");
            configurationSectionKey ??= typeof(TOptions).Name;
            if (configureBinder == null)
            {
                _serviceCollection.Configure<TOptions>(configuration.GetSection(configurationSectionKey));
            }
            else
            {
                _serviceCollection.Configure<TOptions>(configuration.GetSection(configurationSectionKey), configureBinder);
            }
        }

        /// <summary>
        /// Get sixnet configuration section
        /// </summary>
        /// <param name="configurationSectionKey"></param>
        /// <returns></returns>
        public static IConfigurationSection GetSixnetConfigurationSection(string configurationSectionKey)
        {
            return GetSixnetConfigurationSection(GetService<IConfiguration>(), configurationSectionKey);
        }

        /// <summary>
        /// Get sixnet configuration section
        /// </summary>
        /// <param name="root"></param>
        /// <param name="configurationSectionKey"></param>
        /// <returns></returns>
        public static IConfigurationSection GetSixnetConfigurationSection(IConfiguration root, string configurationSectionKey)
        {
            if (string.IsNullOrWhiteSpace(configurationSectionKey))
            {
                return null;
            }
            return root?.GetSection(nameof(Sixnet))?.GetSection(configurationSectionKey);
        }

        /// <summary>
        /// Get options
        /// </summary>
        /// <typeparam name="TOptions"></typeparam>
        /// <param name="name">Options name</param>
        /// <param name="style">Options style</param>
        /// <returns></returns>
        public static TOptions GetOptions<TOptions>(string name, OptionsStyle? style = null) where TOptions : class
        {
            var optionsStyle = style ?? Options.GetOptionsStyle(typeof(TOptions));

            TOptions currentOptions;
            switch (optionsStyle)
            {
                case OptionsStyle.Snapshot:
                    var optionsSnapshot = GetService<IOptionsSnapshot<TOptions>>();
                    currentOptions = string.IsNullOrWhiteSpace(name) ? optionsSnapshot?.Value : optionsSnapshot?.Get(name);
                    break;
                case OptionsStyle.Monitor:
                    var optionsModitor = GetService<IOptionsMonitor<TOptions>>();
                    currentOptions = string.IsNullOrWhiteSpace(name) ? optionsModitor?.CurrentValue : optionsModitor?.Get(name);
                    break;
                default:
                    var optionsConstant = GetService<IOptions<TOptions>>();
                    currentOptions = optionsConstant?.Value;
                    break;
            }
            return currentOptions;
        }

        /// <summary>
        /// Get options
        /// </summary>
        /// <typeparam name="TOptions"></typeparam>
        /// <param name="style">Options style</param>
        /// <returns></returns>
        public static TOptions GetOptions<TOptions>(OptionsStyle? style = null) where TOptions : class
        {
            return GetOptions<TOptions>(string.Empty, style);
        }

        /// <summary>
        /// Configure project default options
        /// </summary>
        /// <param name="services"></param>
        /// <param name="sixnetOptions"></param>
        static void ConfigureProjectDefaultOptions(IServiceCollection services, SixnetOptions sixnetOptions)
        {
            // Upload
            services.ConfigureIfNotNull<UploadOptions>(GetSixnetConfigurationSection(nameof(SixnetConfiguration.Upload)));
            // File access
            services.ConfigureIfNotNull<FileAccessOptions>(GetSixnetConfigurationSection(nameof(SixnetConfiguration.FileAccess)));
            // Rsa key
            services.ConfigureIfNotNull<RSAKeyOptions>(GetSixnetConfigurationSection(nameof(SixnetConfiguration.RSAKey)));
            // Jwt
            services.ConfigureIfNotNull<JwtOptions>(GetSixnetConfigurationSection(nameof(SixnetConfiguration.Jwt)));
            // Database
            services.ConfigureIfNotNull<DataOptions>(GetSixnetConfigurationSection(nameof(SixnetConfiguration.Data)));
            // Cache
            services.ConfigureIfNotNull<CacheOptions>(GetSixnetConfigurationSection(nameof(SixnetConfiguration.Cache)));
            // Email
            services.ConfigureIfNotNull<EmailOptions>(GetSixnetConfigurationSection(nameof(SixnetConfiguration.Email)));
            // Sms
            services.ConfigureIfNotNull<SmsOptions>(GetSixnetConfigurationSection(nameof(SixnetConfiguration.Sms)));
            // Message
            services.ConfigureIfNotNull<MessageOptions>(GetSixnetConfigurationSection(nameof(SixnetConfiguration.Message)));
            // Message queue
            services.ConfigureIfNotNull<MessageQueueOptions>(GetSixnetConfigurationSection(nameof(SixnetConfiguration.MessageQueue)));

            // Post config options
            services.PostConfigureIfNotNull(sixnetOptions.ConfigureUpload);
            services.PostConfigureIfNotNull(sixnetOptions.ConfigureFileAccess);
            services.PostConfigureIfNotNull(sixnetOptions.ConfigureRSAKey);
            services.PostConfigureIfNotNull(sixnetOptions.ConfigureJwt);
            services.PostConfigureIfNotNull(sixnetOptions.ConfigureEmail);
            services.PostConfigureIfNotNull(sixnetOptions.ConfigureSms);
            services.PostConfigureIfNotNull(sixnetOptions.ConfigureMessage);
            services.PostConfigureIfNotNull(sixnetOptions.ConfigureData);
            services.PostConfigureIfNotNull(sixnetOptions.ConfigureMessageQueue);
            services.PostConfigureIfNotNull<CacheOptions>((options) =>
            {
                options.AddCacheProvider(CacheServerType.InMemory, new MemoryProvider());
                sixnetOptions.ConfigureCache?.Invoke(options);
            });
        }

        #endregion

        #region Project service

        /// <summary>
        /// Add project service
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        internal static void AddProjectService(Type serviceType, Type implementationType)
        {
            if (serviceType != null
                && implementationType != null
                && (SixnetApplication.Options?.RegisterDefaultService ?? false))
            {
                _serviceCollection.Add(new SixnetServiceDescriptor(serviceType, implementationType, ServiceLifetime.Singleton));
            }
        }

        /// <summary>
        /// Register ddefault project service
        /// </summary>
        /// <param name="services">Service collection</param>
        static void AddProjectDefaultService(IServiceCollection services, SixnetOptions sixnetOptions)
        {
            services.AddSingleton<ISixnetMessageProvider, DefaultMessageProvider>();
            services.AddSingleton(typeof(ISixnetDataAccess<>), typeof(DefaultDataAccess<>));
            services.AddSingleton(typeof(ISixnetRepository<>), typeof(DefaultRepository<>));
            //services.AddLogging(sixnetOptions?.ConfigureLogging == null
            //? builder => { }
            //: sixnetOptions.ConfigureLogging);
            services.AddSixnetLocalization(sixnetOptions?.ConfigureLocalization);
        }

        #endregion

        #endregion
    }
}
