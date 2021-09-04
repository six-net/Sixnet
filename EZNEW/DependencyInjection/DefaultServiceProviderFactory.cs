using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace EZNEW.DependencyInjection
{
    /// <summary>
    /// Defines default service provider factory
    /// </summary>
    public class DefaultServiceProviderFactory : IServiceProviderFactory<IDIContainer>
    {
        readonly Action<IServiceCollection> _configureServicesDelegate = null;

        public DefaultServiceProviderFactory(Action<IServiceCollection> configureServicesDelegate = null)
        {
            _configureServicesDelegate = configureServicesDelegate;
        }

        public IDIContainer CreateBuilder(IServiceCollection services)
        {
            _configureServicesDelegate?.Invoke(services);
            ContainerManager.Init(services);
            return ContainerManager.Container;
        }

        public IServiceProvider CreateServiceProvider(IDIContainer containerBuilder)
        {
            return ContainerManager.BuildServiceProvider();
        }
    }
}
