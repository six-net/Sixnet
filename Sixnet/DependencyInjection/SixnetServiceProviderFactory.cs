using System;
using Sixnet.App;
using Microsoft.Extensions.DependencyInjection;

namespace Sixnet.DependencyInjection
{
    /// <summary>
    /// Defines default service provider factory
    /// </summary>
    public class SixnetServiceProviderFactory : IServiceProviderFactory<IDIContainer>
    {
        readonly Action<IServiceCollection> _configureServices = null;
        readonly Action<ApplicationOptions> _configureApplication = null;

        public SixnetServiceProviderFactory(Action<IServiceCollection> configureServices = null, Action<ApplicationOptions> configureApplication = null)
        {
            _configureServices = configureServices;
            _configureApplication = configureApplication;
        }

        public IDIContainer CreateBuilder(IServiceCollection services)
        {
            ContainerManager.Configure(services, _configureServices, _configureApplication);
            return ContainerManager.Container;
        }

        public IServiceProvider CreateServiceProvider(IDIContainer containerBuilder)
        {
            return ContainerManager.BuildServiceProvider();
        }
    }
}
