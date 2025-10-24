using Microsoft.Extensions.DependencyInjection;
using Sixnet.App;
using System;

namespace Sixnet.DependencyInjection
{
    /// <summary>
    /// Defines default service provider factory
    /// </summary>
    public class SixnetServiceProviderFactory : IServiceProviderFactory<ISixnetContainer>
    {
        readonly Action<SixnetOptions> _configure = null;
        SixnetOptions _options = null;

        public SixnetServiceProviderFactory(Action<SixnetOptions> configure = null)
        {
            _configure = configure;
        }

        public SixnetServiceProviderFactory(SixnetOptions options)
        {
            _options = options;
        }

        public ISixnetContainer CreateBuilder(IServiceCollection services)
        {
            _options ??= new SixnetOptions();
            _options.Services = services;
            _configure?.Invoke(_options);
            Sixneter.Init(_options);
            return SixnetContainer.Container;
        }

        public IServiceProvider CreateServiceProvider(ISixnetContainer containerBuilder)
        {
            return SixnetContainer.ServiceProvider;
        }
    }
}
