using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using EZNEW.Application;

namespace EZNEW.DependencyInjection
{
    /// <summary>
    /// Defines default service provider factory
    /// </summary>
    public class DefaultServiceProviderFactory : IServiceProviderFactory<IDIContainer>
    {
        readonly Action<IServiceCollection> _configureServicesDelegate = null;
        readonly Action<ApplicationOptions> _configureApplicationDelegate = null;

        public DefaultServiceProviderFactory(Action<IServiceCollection> configureServicesDelegate = null, Action<ApplicationOptions> configureApplicationDelegate = null)
        {
            _configureServicesDelegate = configureServicesDelegate;
            _configureApplicationDelegate = configureApplicationDelegate;
        }

        public IDIContainer CreateBuilder(IServiceCollection services)
        {
            _configureServicesDelegate?.Invoke(services);
            services?.AddEZNEW(_configureApplicationDelegate);
            return ContainerManager.Container;
        }

        public IServiceProvider CreateServiceProvider(IDIContainer containerBuilder)
        {
            return ContainerManager.BuildServiceProvider();
        }
    }
}
