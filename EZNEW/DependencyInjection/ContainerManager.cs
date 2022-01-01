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
using EZNEW.Development.Command;
using EZNEW.Data;
using EZNEW.Development.Domain.Repository.Warehouse;
using EZNEW.Development.Message;

namespace EZNEW.DependencyInjection
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

        ///// <summary>
        ///// Gets or sets the default services
        ///// </summary>
        //public static IServiceCollection ServiceCollection
        //{
        //    get
        //    {
        //        return defaultServices;
        //    }
        //    set
        //    {
        //        SetDefaultServiceCollection(value);
        //    }
        //}

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
        /// <param name="configureServiceDelegae">Configure service delegate</param>
        /// <param name="configureApplicationDelegate">Configure service delegate</param>
        public static IServiceCollection Configure(IServiceCollection services, Action<IServiceCollection> configureServiceDelegae = null, Action<ApplicationOptions> configureApplicationDelegate = null)
        {
            //Init default service collection
            serviceCollection = services ?? new ServiceCollection();
            BuildServiceProviderFromDefaultServices();

            //Configure application
            ApplicationManager.Configure(configureApplicationDelegate);
            var applicationOptions = ApplicationManager.Options;

            //Add component
            AddComponentService();

            //Add default project service
            if (applicationOptions.RegisterProjectDefaultService)
            {
                RegisterDefaultProjectService(serviceCollection);
            }

            //Custom configure service
            configureServiceDelegae?.Invoke(serviceCollection);

            //Container
            var container = ApplicationManager.Options.DIContainer ?? new ServiceProviderContainer();
            if (serviceCollection != null && !(container is ServiceProviderContainer))
            {
                container.Register(serviceCollection.ToArray());
            }
            Container = container;

            //Build service provider
            BuildServiceProviderFromDefaultServices();

            //Configure module
            ModuleManager.ConfigureModule();

            //Default repository event
            RepositoryEventBus.InitDefaultEvent();

            //Object mapper
            ObjectMapper.BuildMapper();

            //Build service provider
            BuildServiceProviderFromDefaultServices();

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

        #region Component

        /// <summary>
        /// Add component Service
        /// </summary>
        static void AddComponentService()
        {
            var configuration = Resolve<IConfiguration>();
            if (configuration != null)
            {
                //Configure upload
                serviceCollection.Configure<UploadConfiguration>(configuration.GetSection(UploadConfiguration.UploadConfigurationName));
                //Configure file access
                serviceCollection.Configure<FileAccessConfiguration>(configuration.GetSection(FileAccessConfiguration.FileAccessConfigurationName));
            }

            //Data cache provider
            serviceCollection.AddSingleton<IDataCacheProvider, DefaultDataCacheProvider>();
            //Command executor
            serviceCollection.AddSingleton<ICommandExecutor, DatabaseCommandExecutor>();
            //Message provider
            serviceCollection.AddSingleton<IMessageProvider, DefaultMessageProvider>();
        }

        /// <summary>
        /// Add warehouse
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="dataAccessService">Data access service</param>
        internal static void AddWarehouseService(Type entityType, Type dataAccessService)
        {
            if (entityType == null || dataAccessService == null)
            {
                return;
            }

            var repositoryWarehouseInterface = typeof(IEntityWarehouse<,>).MakeGenericType(entityType, dataAccessService);
            if (ApplicationManager.Options.UseDebugWarehouse)
            {
                var debugWarehouseType = typeof(DebugEntityWarehouse<,>).MakeGenericType(entityType, dataAccessService);
                AddDefaultProjectService(repositoryWarehouseInterface, debugWarehouseType);
            }
            else
            {
                var defaultWarehouseType = typeof(DefaultEntityWarehouse<,>).MakeGenericType(entityType, dataAccessService);
                AddDefaultProjectService(repositoryWarehouseInterface, defaultWarehouseType);
            }
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
