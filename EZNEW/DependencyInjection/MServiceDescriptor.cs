using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace EZNEW.DependencyInjection
{
    /// <summary>
    /// Service descriptor
    /// </summary>
    [Serializable]
    public class MServiceDescriptor : ServiceDescriptor
    {
        /// <summary>
        /// Service behaviors
        /// </summary>
        public IEnumerable<Type> Behaviors { get; set; }

        public MServiceDescriptor(Type serviceType, object instance) : base(serviceType, instance)
        {
        }

        public MServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime) : base(serviceType, implementationType, lifetime)
        {
        }

        public MServiceDescriptor(Type serviceType, Func<IServiceProvider, object> factory, ServiceLifetime lifetime) : base(serviceType, factory, lifetime)
        {
        }
    }
}
