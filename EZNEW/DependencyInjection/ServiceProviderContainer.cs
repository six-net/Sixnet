using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace EZNEW.DependencyInjection
{
    /// <summary>
    /// Service provider container
    /// </summary>
    internal class ServiceProviderContainer : IDIContainer
    {
        /// <summary>
        /// Determine whether the service is registered
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <returns>Return whether is register service</returns>
        public bool IsRegister<TService>()
        {
            return ContainerManager.ServiceCollectionIsRegister<TService>();
        }

        /// <summary>
        /// Register service
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <typeparam name="TImplement">Implementation type</typeparam>
        /// <param name="lifetime">Life time(default:singleton)</param>
        /// <param name="behaviors">Behaviors</param>
        public void Register<TService, TImplement>(ServiceLifetime lifetime = ServiceLifetime.Singleton, IEnumerable<Type> behaviors = null)
        {
            Register(typeof(TService), typeof(TImplement), lifetime, behaviors);
        }

        /// <summary>
        /// Register
        /// </summary>
        /// <param name="serviceType">Service type</param>
        /// <param name="implementationType">Implementation type</param>
        /// <param name="lifetime">lifetime(default:singleton)</param>
        /// <param name="behaviors">Behaviors</param>
        public void Register(Type serviceType, Type implementationType, ServiceLifetime lifetime = ServiceLifetime.Singleton, IEnumerable<Type> behaviors = null)
        {
            Register(new MServiceDescriptor(serviceType, implementationType, lifetime)
            {
                Behaviors = behaviors
            });
        }

        /// <summary>
        /// Register service
        /// </summary>
        /// <param name="serviceDescriptors">Service descriptor</param>
        public void Register(params ServiceDescriptor[] serviceDescriptors)
        {
            ContainerManager.RegisterToServiceCollection(serviceDescriptors);
        }

        /// <summary>
        /// Resolve registered services
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <returns>Return a service instance</returns>
        public TService Resolve<TService>()
        {
            return ContainerManager.ResolveFromServiceCollection<TService>();
        }

        /// <summary>
        /// Get register service
        /// </summary>
        /// <param name="serviceType">Service type</param>
        /// <returns>Return a service instance</returns>
        public object Resolve(Type serviceType)
        {
            return ContainerManager.ResolveFromServiceCollection(serviceType);
        }

        /// <summary>
        /// Build service provider
        /// </summary>
        /// <returns>Return a service provider</returns>
        public IServiceProvider BuildServiceProvider()
        {
            return ContainerManager.BuildServiceProviderFromServiceCollection();
        }
    }
}
