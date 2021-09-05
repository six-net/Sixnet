using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using EZNEW.Upload;
using EZNEW.FileAccess;
using EZNEW.Application;
using EZNEW.Module;
using EZNEW.Development.Domain.Repository.Event;
using EZNEW.Mapper;
using EZNEW.Data.Cache;

namespace EZNEW.DependencyInjection
{
    /// <summary>
    /// Dependency injection container manager
    /// </summary>
    public static class ContainerManager
    {
        /// <summary>
        /// Internal services
        /// </summary>
        internal static Dictionary<Type, Type> InternalServices = null;

        /// <summary>
        /// Default project services
        /// </summary>
        internal static Dictionary<Type, Type> DefaultProjectServices = null;

        /// <summary>
        /// Default services
        /// </summary>
        static IServiceCollection defaultServices = null;

        /// <summary>
        /// Gets the current DI container
        /// </summary>
        public static IDIContainer Container { get; private set; } = null;

        /// <summary>
        /// Gets or sets the default services
        /// </summary>
        public static IServiceCollection ServiceCollection
        {
            get
            {
                return defaultServices;
            }
            set
            {
                SetDefaultServiceCollection(value);
            }
        }

        /// <summary>
        /// Gets the current service provider
        /// </summary>
        public static IServiceProvider ServiceProvider { get; private set; } = null;

        /// <summary>
        /// Sets the default services   
        /// </summary>
        /// <param name="services">Services</param>
        static void SetDefaultServiceCollection(IServiceCollection services)
        {
            if (services == null)
            {
                defaultServices?.Clear();
                ServiceProvider = null;
            }
            else
            {
                defaultServices = services;
                ServiceProvider = defaultServices.BuildServiceProvider();
            }
        }

        /// <summary>
        /// Register service to default service collection
        /// </summary>
        /// <param name="serviceDescriptors">Service descriptors</param>
        internal static void RegisterToServiceCollection(params ServiceDescriptor[] serviceDescriptors)
        {
            if (serviceDescriptors == null || serviceDescriptors.Length <= 0)
            {
                return;
            }
            foreach (var sd in serviceDescriptors)
            {
                defaultServices.Add(sd);
            }
            ServiceProvider = defaultServices.BuildServiceProvider();
        }

        /// <summary>
        /// Determines whether the service is registered
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <returns>Return whether the service is registered</returns>
        internal static bool ServiceCollectionIsRegister<TService>()
        {
            return defaultServices?.Any(c => c.ServiceType == typeof(TService)) ?? false;
        }

        /// <summary>
        /// Resolve service
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <returns>Return the service instance</returns>
        internal static TService ResolveFromServiceCollection<TService>()
        {
            var service = ResolveFromServiceCollection(typeof(TService));
            return service == null ? default : (TService)service;
        }

        /// <summary>
        /// Resolve service
        /// </summary>
        /// <param name="serviceType">Service type</param>
        /// <returns>Return the service instance</returns>
        internal static object ResolveFromServiceCollection(Type serviceType)
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
        internal static IServiceProvider BuildServiceProviderFromServiceCollection()
        {
            ServiceProvider = defaultServices?.BuildServiceProvider();
            return ServiceProvider;
        }

        /// <summary>
        /// Init dependency injection container
        /// </summary>
        /// <param name="defaultServices">Default services</param>
        /// <param name="container">Dependency injection container</param>
        /// <param name="configureApplicationDelegate">Configure service delegate</param>
        /// <param name="registerDefaultProjectService">Whether register default project service</param>
        public static void Init(IServiceCollection services,Action<ApplicationOptions> configureApplicationDelegate = null)
        {
            //Configure application
            ApplicationManager.Configure(configureApplicationDelegate);
            var applicationOptions = ApplicationManager.Options;

            //Init default container
            services ??= new ServiceCollection();
            var container = ApplicationManager.Options.DIContainer ?? new ServiceProviderContainer();
            SetDefaultServiceCollection(services);

            if (defaultServices != null && !(container is ServiceProviderContainer))
            {
                container.Register(defaultServices.ToArray());
            }

            //Register component
            RegisterComponentConfiguration();

            //Register internal service
            RegisterInternalService(defaultServices);

            //Register default project service
            if (applicationOptions.RegisterProjectDefaultService)
            {
                RegisterDefaultProjectService(defaultServices);
            }
            SetDefaultServiceCollection(defaultServices);
            Container = container;

            //Configure module
            ModuleManager.ConfigureModule();

            //Default repository event
            RepositoryEventBus.InitDefaultEvent();

            //Object mapper
            ObjectMapper.BuildMapper();

            GC.Collect();
        }

        /// <summary>
        /// Determine whether register the specified type
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <returns>Return whether service has registered</returns>
        public static bool IsRegister<TService>()
        {
            return Container?.IsRegister<TService>() ?? ServiceCollectionIsRegister<TService>();
        }

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
                data = ResolveFromServiceCollection<TService>();
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
                data = ResolveFromServiceCollection(serviceType);
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

        /// <summary>
        /// Register service,use singleton lifetime by default
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <typeparam name="TImplementation">Service implementation</typeparam>
        /// <param name="behaviors">Behaviors</param>
        public static void Register<TService, TImplementation>(ServiceLifetime lifetime = ServiceLifetime.Singleton, IEnumerable<Type> behaviors = null)
        {
            Register(typeof(TService), typeof(TImplementation), lifetime, behaviors);
        }

        /// <summary>
        /// Register service,use singleton lifetime by default
        /// </summary>
        /// <param name="serviceType">Service type</param>
        /// <param name="implementationType">Service implementation type</param>
        /// <param name="lifetime">Life time(default:singleton)</param>
        /// <param name="behaviors">Behaviors</param>
        public static void Register(Type serviceType, Type implementationType, ServiceLifetime lifetime = ServiceLifetime.Singleton, IEnumerable<Type> behaviors = null)
        {
            Register(new MServiceDescriptor(serviceType, implementationType, lifetime)
            {
                Behaviors = behaviors
            });
        }

        /// <summary>
        /// Register service
        /// </summary>
        /// <param name="serviceDescriptor">Service descriptor</param>
        public static void Register(ServiceDescriptor serviceDescriptor)
        {
            if (serviceDescriptor == null)
            {
                return;
            }
            if (defaultServices != null)
            {
                defaultServices.Add(serviceDescriptor);
                ServiceProvider = defaultServices.BuildServiceProvider();
            }
            if (Container != null && !(Container is ServiceProviderContainer))
            {
                Container.Register(serviceDescriptor);
            }
        }

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
                provider = BuildServiceProviderFromServiceCollection();
            }
            return provider;
        }

        /// <summary>
        /// Register component configuration
        /// </summary>
        static void RegisterComponentConfiguration()
        {
            var configuration = Resolve<IConfiguration>();
            if (configuration == null || ServiceCollection == null)
            {
                return;
            }
            //Configure upload
            ServiceCollection.Configure<UploadConfiguration>(configuration.GetSection(UploadConfiguration.UploadConfigurationName));
            //Configure file access
            ServiceCollection?.Configure<FileAccessConfiguration>(configuration.GetSection(FileAccessConfiguration.FileAccessConfigurationName));
        }

        /// <summary>
        /// Add internal service
        /// </summary>
        /// <param name="serviceType">Service type</param>
        /// <param name="serviceInstance">Service instance</param>
        internal static void AddInternalService(Type serviceType, Type implementationType)
        {
            if (serviceType == null || implementationType == null)
            {
                return;
            }
            InternalServices ??= new Dictionary<Type, Type>();
            InternalServices[serviceType] = implementationType;
        }

        /// <summary>
        /// Register internal service
        /// </summary>
        internal static void RegisterInternalService(IServiceCollection services)
        {
            if (!InternalServices.IsNullOrEmpty())
            {
                foreach (var serviceItem in InternalServices)
                {
                    services.AddSingleton(serviceItem.Key, serviceItem.Value);
                }
                InternalServices = null;
            }
            services.AddSingleton(typeof(IDataCacheProvider), typeof(DefaultDataCacheProvider));
        }

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
            DefaultProjectServices ??= new Dictionary<Type, Type>();
            DefaultProjectServices[serviceType] = implementationType;
        }

        /// <summary>
        /// Register ddefault project service
        /// </summary>
        /// <param name="services">Service collection</param>
        internal static void RegisterDefaultProjectService(IServiceCollection services)
        {
            if (!DefaultProjectServices.IsNullOrEmpty())
            {
                foreach (var serverItem in DefaultProjectServices)
                {
                    services.AddSingleton(serverItem.Key, serverItem.Value);
                }
                DefaultProjectServices = null;
            }
        }
    }
}
