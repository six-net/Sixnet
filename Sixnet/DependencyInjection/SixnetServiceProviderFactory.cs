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
        readonly SixnetOptions _options = SixnetContainer.Options;

        public SixnetServiceProviderFactory(Action<SixnetOptions> configure = null)
        {
            _configure = configure;
        }

        public SixnetServiceProviderFactory(SixnetOptions options)
        {
            if (options != null)
            {
                _options = options;
            }
        }

        public ISixnetContainer CreateBuilder(IServiceCollection services)
        {
            if (_options != null)
            {
                _options.Services = services;
                _configure?.Invoke(_options);
            }
            SixnetContainer.Configure(_options);
            return SixnetContainer.Container;
        }

        public IServiceProvider CreateServiceProvider(ISixnetContainer containerBuilder)
        {
            return SixnetContainer.ServiceProvider;
        }
    }
}
