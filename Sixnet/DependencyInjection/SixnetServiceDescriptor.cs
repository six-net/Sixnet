using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Sixnet.DependencyInjection
{
    /// <summary>
    /// Service descriptor
    /// </summary>
    [Serializable]
    public class SixnetServiceDescriptor : ServiceDescriptor
    {
        /// <summary>
        /// Service behaviors
        /// </summary>
        public IEnumerable<Type> Behaviors { get; set; }

        public SixnetServiceDescriptor(Type serviceType, object instance) : base(serviceType, instance)
        {
        }

        public SixnetServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime) : base(serviceType, implementationType, lifetime)
        {
        }

        public SixnetServiceDescriptor(Type serviceType, Func<IServiceProvider, object> factory, ServiceLifetime lifetime) : base(serviceType, factory, lifetime)
        {
        }
    }
}
