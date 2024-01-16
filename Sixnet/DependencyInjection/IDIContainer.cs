using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Sixnet.DependencyInjection
{
    /// <summary>
    /// Dependency injection container contract
    /// </summary>
    public interface IDIContainer
    {
        #region Register

        /// <summary>
        /// Register service
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <typeparam name="TImplement">Service implementation type</typeparam>
        /// <param name="lifetime">Life time(default:singleton)</param>
        /// <param name="behaviors">Behaviors</param>
        void Register<TService, TImplement>(ServiceLifetime lifetime = ServiceLifetime.Singleton, IEnumerable<Type> behaviors = null);

        /// <summary>
        /// Register service
        /// </summary>
        /// <param name="serviceType">Service type</param>
        /// <param name="implementationType">Service implementation type</param>
        /// <param name="lifetime">Life time(default:singleton)</param>
        /// <param name="behaviors">Behaviors</param>
        void Register(Type serviceType, Type implementationType, ServiceLifetime lifetime = ServiceLifetime.Singleton, IEnumerable<Type> behaviors = null);

        /// <summary>
        /// Register service
        /// </summary>
        /// <param name="serviceDescriptors">Service descriptor</param>
        void Register(params ServiceDescriptor[] serviceDescriptors);

        #endregion

        #region Determine whether service is register

        /// <summary>
        ///Determine whether has registered the specified type
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <returns>Return whether register service</returns>
        bool IsRegister<TService>();

        #endregion

        #region Gets service

        /// <summary>
        /// Gets a service instance
        /// </summary>
        /// <typeparam name="TService">Service type</typeparam>
        /// <returns>Return a service instance</returns>
        TService Resolve<TService>();

        /// <summary>
        /// Gets a service instance
        /// </summary>
        /// <param name="serviceType">Service type</param>
        /// <returns>Return a service instance</returns>
        object Resolve(Type serviceType);

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
