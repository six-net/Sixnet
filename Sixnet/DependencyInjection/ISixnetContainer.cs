using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Sixnet.DependencyInjection
{
    /// <summary>
    /// Dependency injection container contract
    /// </summary>
    public interface ISixnetContainer
    {
        #region Add service

        /// <summary>
        /// Add service
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <typeparam name="TImplement">Service implementation type</typeparam>
        /// <param name="lifetime">Life time(default:singleton)</param>
        /// <param name="behaviors">Behaviors</param>
        void AddService<TService, TImplement>(ServiceLifetime lifetime = ServiceLifetime.Singleton, IEnumerable<Type> behaviors = null);

        /// <summary>
        /// Add service
        /// </summary>
        /// <param name="serviceType">Service type</param>
        /// <param name="implementationType">Service implementation type</param>
        /// <param name="lifetime">Life time(default:singleton)</param>
        /// <param name="behaviors">Behaviors</param>
        void AddService(Type serviceType, Type implementationType, ServiceLifetime lifetime = ServiceLifetime.Singleton, IEnumerable<Type> behaviors = null);

        /// <summary>
        /// Add service
        /// </summary>
        /// <param name="serviceDescriptors">Service descriptor</param>
        void AddService(params ServiceDescriptor[] serviceDescriptors);

        #endregion

        #region Has service

        /// <summary>
        ///Determine whether has added the service
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <returns></returns>
        bool HasService<TService>();

        #endregion

        #region Gets service

        /// <summary>
        /// Gets a service instance
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <returns>Return a service instance</returns>
        TService GetService<TService>();

        /// <summary>
        /// Gets a service instance
        /// </summary>
        /// <param name="serviceType">Service type</param>
        /// <returns>Return a service instance</returns>
        object GetService(Type serviceType);

        #endregion

        #region Build service provider

        /// <summary>
        /// Build service provider
        /// </summary>
        /// <returns>Return a service provider</returns>
        IServiceProvider BuildServiceProvider();

        #endregion
    }
}
