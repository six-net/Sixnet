using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using EZNEW.Logging;
using EZNEW.Upload.Configuration;
using EZNEW.FileAccess.Configuration;
using EZNEW.Application;
using System.Runtime.InteropServices.ComTypes;
using EZNEW.Data.Cache;

namespace EZNEW.DependencyInjection
{
    /// <summary>
    /// Dependency injection container manager
    /// </summary>
    public static class ContainerManager
    {
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
        /// <param name="configureServiceAction">Configure service action</param>
        /// <param name="registerDefaultProjectService">Whether register default project service</param>
        public static void Init(IServiceCollection defaultServices = null, IDIContainer container = null, Action<IDIContainer> configureServiceAction = null, bool registerDefaultProjectService = true)
        {
            defaultServices ??= new ServiceCollection();
            container ??= new ServiceProviderContainer();
            SetDefaultServiceCollection(defaultServices);
            if (defaultServices != null && !(container is ServiceProviderContainer))
            {
                container.Register(defaultServices.ToArray());
            }
            RegisterComponentConfiguration();
            if (registerDefaultProjectService)
            {
                RegisterDefaultProjectService();
            }
            configureServiceAction?.Invoke(container);
            SetDefaultServiceCollection(defaultServices);
            Container = container;
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
        /// Register default project service
        /// </summary>
        static void RegisterDefaultProjectService()
        {
            var appPath = ApplicationManager.GetApplicationExecutableDirectory();
            var conventionFiles = new string[] { "DataAccess", "Business", "Repository", "Service", "Domain" };
            IEnumerable<FileInfo> files = new DirectoryInfo(appPath).GetFiles("*.dll", SearchOption.AllDirectories)?.
                Where(c => conventionFiles.Any(kw => c.Name.Contains(kw))) ?? Array.Empty<FileInfo>();
            List<Type> allTypes = new List<Type>();
            foreach (var file in files)
            {
                try
                {
                    allTypes.AddRange(Assembly.LoadFrom(file.FullName).GetTypes());
                }
                catch (Exception ex)
                {
                    LogManager.LogError(ex, ex.Message);
                }
            }
            foreach (Type serviceType in allTypes)
            {
                if (!serviceType.IsInterface)
                {
                    continue;
                }
                string typeName = serviceType.Name;
                if (typeName.EndsWith("Service") || typeName.EndsWith("Business") || typeName.EndsWith("DbAccess") || typeName.EndsWith("Repository"))
                {
                    Type implementType = allTypes.FirstOrDefault(t => t.Name != serviceType.Name && !t.IsInterface && serviceType.IsAssignableFrom(t));
                    if (implementType != null)
                    {
                        Register(serviceType, implementType);
                    }
                }
                if (typeName.EndsWith("DataAccess"))
                {
                    List<Type> relateTypes = allTypes.Where(t => t.Name != serviceType.Name && !t.IsInterface && serviceType.IsAssignableFrom(t)).ToList();
                    if (relateTypes != null && relateTypes.Count > 0)
                    {
                        Type providerType = relateTypes.FirstOrDefault(c => c.Name.EndsWith("CacheDataAccess"));
                        providerType ??= relateTypes.First();
                        Register(serviceType, providerType);
                    }
                }
            }

            //Data cache provider
            Register<IDataCacheProvider, DefaultDataCacheProvider>();
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
    }
}
