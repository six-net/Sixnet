using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Sixnet.DependencyInjection
{
    /// <summary>
    /// Service provider container
    /// </summary>
    internal class DefaultServiceProviderContainer : ISixnetContainer
    {
        /// <summary>
        /// Determine whether the service is registered
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <returns>Return whether is register service</returns>
        public bool HasService<TService>()
        {
            return SixnetContainer.HasDefaultService<TService>();
        }

        /// <summary>
        /// Register service
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <typeparam name="TImplement">Implementation type</typeparam>
        /// <param name="lifetime">Life time(default:singleton)</param>
        /// <param name="behaviors">Behaviors</param>
        public void AddService<TService, TImplement>(ServiceLifetime lifetime = ServiceLifetime.Singleton, IEnumerable<Type> behaviors = null)
        {
            AddService(typeof(TService), typeof(TImplement), lifetime, behaviors);
        }

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="serviceType">Service type</param>
        /// <param name="implementationType">Implementation type</param>
        /// <param name="lifetime">lifetime(default:singleton)</param>
        /// <param name="behaviors">Behaviors</param>
        public void AddService(Type serviceType, Type implementationType, ServiceLifetime lifetime = ServiceLifetime.Singleton, IEnumerable<Type> behaviors = null)
        {
            AddService(new SixnetServiceDescriptor(serviceType, implementationType, lifetime)
            {
                Behaviors = behaviors
            });
        }

        /// <summary>
        /// Add service
        /// </summary>
        /// <param name="serviceDescriptors">Service descriptor</param>
        public void AddService(params ServiceDescriptor[] serviceDescriptors)
        {
            SixnetContainer.AddService(serviceDescriptors);
        }

        /// <summary>
        /// Resolve registered services
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <returns>Return a service instance</returns>
        public TService GetService<TService>()
        {
            return SixnetContainer.GetDefaultService<TService>();
        }

        /// <summary>
        /// Get register service
        /// </summary>
        /// <param name="serviceType">Service type</param>
        /// <returns>Return a service instance</returns>
        public object GetService(Type serviceType)
        {
            return SixnetContainer.GetDefaultService(serviceType);
        }

        /// <summary>
        /// Build service provider
        /// </summary>
        /// <returns>Return a service provider</returns>
        public IServiceProvider BuildServiceProvider()
        {
            return SixnetContainer.BuildServiceProvider();
        }
    }
}
