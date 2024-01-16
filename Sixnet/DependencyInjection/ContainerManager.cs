using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sixnet.App;
using Sixnet.App.Module;
using Sixnet.Development.Data;
using Sixnet.Development.Data.Database;
using Sixnet.Development.Message;
using Sixnet.Development.Repository;
using Sixnet.Exceptions;
using Sixnet.IO.FileAccess;
using Sixnet.Mapper;
using Sixnet.Net.Upload;
using Sixnet.Security.Cryptography;
using Sixnet.Token.Jwt;

namespace Sixnet.DependencyInjection
{
    /// <summary>
    /// Dependency injection container manager
    /// </summary>
    public static class ContainerManager
    {
        #region Fields

        ///// <summary>
        ///// Internal services
        ///// </summary>
        //static List<ServiceDescriptor> internalServices = null;

        /// <summary>
        /// Default project services
        /// </summary>
        static List<ServiceDescriptor> defaultProjectServices = null;

        /// <summary>
        /// Default service collection
        /// </summary>
        static IServiceCollection serviceCollection = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current DI container
        /// </summary>
        public static IDIContainer Container { get; private set; } = null;

        /// <summary>
        /// Gets the current service provider
        /// </summary>
        public static IServiceProvider ServiceProvider { get; private set; } = null;

        /// <summary>
        /// Gets the service collection
        /// </summary>
        public static IServiceCollection ServiceCollection => serviceCollection;

        #endregion

        #region Methods

        #region Default services

        /// <summary>
        /// Register service to default service collection
        /// </summary>
        /// <param name="serviceDescriptors">Service descriptors</param>
        internal static void RegisterToDefaultServices(params ServiceDescriptor[] serviceDescriptors)
        {
            if (serviceDescriptors == null || serviceDescriptors.Length <= 0)
            {
                return;
            }
            foreach (var sd in serviceDescriptors)
            {
                serviceCollection.Add(sd);
            }
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        /// <summary>
        /// Determines whether the service is registered
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <returns>Return whether the service is registered</returns>
        internal static bool CheckDefaultServicesIsRegister<TService>()
        {
            return serviceCollection?.Any(c => c.ServiceType == typeof(TService)) ?? false;
        }

        /// <summary>
        /// Resolve service
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <returns>Return the service instance</returns>
        internal static TService ResolveFromDefaultServices<TService>()
        {
            var service = ResolveFromDefaultServices(typeof(TService));
            return service == null ? default : (TService)service;
        }

        /// <summary>
        /// Resolve service
        /// </summary>
        /// <param name="serviceType">Service type</param>
        /// <returns>Return the service instance</returns>
        internal static object ResolveFromDefaultServices(Type serviceType)
        {
            if (ServiceProvider != null && serviceType != null)
            {
                return ServiceProvider.GetService(serviceType);
            }
            return null;
        }

        /// <summary>
        /// Build service provider from serrvice collection
        /// </summary>
        /// <returns>Return the service provider</returns>
        internal static IServiceProvider BuildServiceProviderFromDefaultServices()
        {
            ServiceProvider = serviceCollection?.BuildServiceProvider();
            return ServiceProvider;
        }

        #endregion

        #region Configure

        /// <summary>
        /// Configure dependency injection container
        /// </summary>
        /// <param name="services">Default services</param>
        /// <param name="configureService">Configure service</param>
        /// <param name="configureApp">Configure app</param>
        public static IServiceCollection Configure(IServiceCollection services, Action<IServiceCollection> configureService = null, Action<ApplicationOptions> configureApp = null)
        {
            // Init default service collection
            serviceCollection = services ?? new ServiceCollection();
            BuildServiceProviderFromDefaultServices();

            // Configure application
            ApplicationManager.Configure(configureApp);
            var applicationOptions = ApplicationManager.Options;

            // Register default project service
            if (applicationOptions.RegisterProjectDefaultService)
            {
                RegisterDefaultProjectService(serviceCollection);
            }

            // Configure service
            configureService?.Invoke(serviceCollection);

            // Container
            var container = ApplicationManager.Options.DIContainer ?? new ServiceProviderContainer();
            if (serviceCollection != null && container is not ServiceProviderContainer)
            {
                container.Register(serviceCollection.ToArray());
            }
            Container = container;

            //Build service provider
            BuildServiceProviderFromDefaultServices();

            //Configure module
            ModuleManager.ConfigureModule();

            //Object mapper
            ObjectMapper.BuildMapper();

            //Build service provider
            BuildServiceProviderFromDefaultServices();

            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect();

            return serviceCollection;
        }

        #endregion

        #region Resolve

        /// <summary>
        /// Resolve the specified type
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <returns>Return the service instance</returns>
        public static TService Resolve<TService>()
        {
            TService data;
            if (Container != null)
            {
                data = Container.Resolve<TService>();
            }
            else
            {
                data = ResolveFromDefaultServices<TService>();
            }
            return data;
        }

        /// <summary>
        /// Resolve the specified type
        /// </summary>
        /// <param name="serviceType">Service type</param>
        /// <returns>Return the service instance</returns>
        public static object Resolve(Type serviceType)
        {
            object data;
            if (Container != null)
            {
                data = Container.Resolve(serviceType);
            }
            else
            {
                data = ResolveFromDefaultServices(serviceType);
            }
            return data;
        }

        /// <summary>
        /// Returns the registered service or default service
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <typeparam name="TDefault">Default service implementation</typeparam>
        /// <returns>Return the service instance</returns>
        public static TService Resolve<TService, TDefault>() where TDefault : TService, new()
        {
            var service = Resolve<TService>();
            if (service == null)
            {
                service = new TDefault();
            }
            return service;
        }

        #endregion

        #region Register

        /// <summary>
        /// Determine whether register the specified type
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <returns>Return whether service has registered</returns>
        public static bool IsRegister<TService>()
        {
            return Container?.IsRegister<TService>() ?? CheckDefaultServicesIsRegister<TService>();
        }

        #endregion

        #region Service provider

        /// <summary>
        /// Build service provider
        /// </summary>
        /// <returns>Service service provider</returns>
        public static IServiceProvider BuildServiceProvider()
        {
            IServiceProvider provider = null;
            if (Container != null && !(Container is ServiceProviderContainer))
            {
                provider = Container.BuildServiceProvider();
            }
            if (provider == null)
            {
                provider = BuildServiceProviderFromDefaultServices();
            }
            return provider;
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
            AddService(new MServiceDescriptor(serviceType, implementationType, lifetime)
            {
                Behaviors = behaviors
            });
        }

        /// <summary>
        /// Add service
        /// </summary>
        /// <param name="serviceDescriptor">Service descriptor</param>
        public static void AddService(ServiceDescriptor serviceDescriptor)
        {
            if (serviceDescriptor == null)
            {
                return;
            }
            serviceCollection.Add(serviceDescriptor);
        }

        #endregion

        #region Configure options

        /// <summary>
        /// Registers a configuration instance which TOptions will bind against.
        /// </summary>
        /// <typeparam name="TOptions">The type of options being configured.</typeparam>
        /// <param name="configureBinder">Configure binder</param>
        /// <param name="configurationSectionKey">Configuration section key</param>
        public static void Configure<TOptions>(string configurationSectionKey = "", Action<BinderOptions> configureBinder = null) where TOptions : class
        {
            var configuration = Resolve<IConfiguration>();
            if (configuration == null)
            {
                throw new SixnetException("Not initialize configuration");
            }
            configurationSectionKey ??= typeof(TOptions).Name;
            if (configureBinder == null)
            {
                serviceCollection.Configure<TOptions>(configuration.GetSection(configurationSectionKey));
            }
            else
            {
                serviceCollection.Configure<TOptions>(configuration.GetSection(configurationSectionKey), configureBinder);
            }
        }

        #endregion

        #region Default project service

        /// <summary>
        /// Add default project service
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="implementationType"></param>
        internal static void AddDefaultProjectService(Type serviceType, Type implementationType)
        {
            if (serviceType == null || implementationType == null)
            {
                return;
            }
            defaultProjectServices ??= new List<ServiceDescriptor>();
            defaultProjectServices.Add(new MServiceDescriptor(serviceType, implementationType, ServiceLifetime.Singleton));
        }

        /// <summary>
        /// Register ddefault project service
        /// </summary>
        /// <param name="services">Service collection</param>
        internal static void RegisterDefaultProjectService(IServiceCollection services)
        {
            var configuration = Resolve<IConfiguration>();
            if (configuration != null)
            {
                // Configure upload
                services.Configure<UploadConfiguration>(configuration.GetSection(nameof(UploadConfiguration)));
                // Configure file access
                services.Configure<FileAccessConfiguration>(configuration.GetSection(nameof(FileAccessConfiguration)));
                // Configure Rsa key
                services.Configure<RSAKeyConfiguration>(configuration.GetSection(nameof(RSAKeyConfiguration)));
                // Configure jwt
                services.Configure<JwtConfiguration>(configuration.GetSection(nameof(JwtConfiguration)));
                // Database server
                var databaseServers = configuration.GetSection("DatabaseServers")?.Get<List<DatabaseServer>>();
                if(!databaseServers.IsNullOrEmpty())
                {
                    DataManager.AddDatabaseServers(databaseServers);
                }
            }

            //Message provider
            services.AddSingleton<IMessageProvider, DefaultMessageProvider>();

            // Data access
            services.AddSingleton(typeof(IDataAccess<>), typeof(DefaultDataAccess<>));

            // Repository
            services.AddSingleton(typeof(IRepository<>), typeof(DefaultRepository<>));

            if (!defaultProjectServices.IsNullOrEmpty())
            {
                foreach (var serverItem in defaultProjectServices)
                {
                    services.Add(serverItem);
                }
                defaultProjectServices = null;
            }
        }

        #endregion

        #endregion
    }
}
