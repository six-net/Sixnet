// "Company © 2025. All rights reserved."

using Microsoft.Extensions.Configuration;

using Sixnet;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class SixnetServiceCollectionExtensions
    {
        public static IServiceCollection AddSixnet(this IServiceCollection services, Action<SixnetOptions> configure = null)
        {
            var options = new SixnetOptions
            {
                Services = services
            };
            configure?.Invoke(options);
            Sixneter.Init(options);
            return services;
        }

        public static IServiceCollection ConfigureIfNotNull<TOptions>(this IServiceCollection services, IConfiguration configuration) where TOptions : class
        {
            if (configuration != null)
            {
                services.Configure<TOptions>(configuration);
            }
            return services;
        }

        public static IServiceCollection PostConfigureIfNotNull<TOptions>(this IServiceCollection services, Action<TOptions> configure) where TOptions : class
        {
            if (configure != null)
            {
                services.PostConfigure(configure);
            }
            return services;
        }
    }
}
