using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Sixnet.Exceptions;
using Sixnet.Localization;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Sixnet localization service collection extensions
    /// </summary>
    public static class SixnetLocalizationServiceCollectionExtensions
    {
        /// <summary>
        /// Add sixnet localization
        /// </summary>
        /// <param name="services">Services</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddSixnetLocalization(this IServiceCollection services, Action<SixnetLocalizationOptions> configure = null)
        {
            SixnetDirectThrower.ThrowArgNullIf(services == null, nameof(services));

            // local options
            var options = new SixnetLocalizationOptions();
            configure?.Invoke(options);

            // system
            services.TryAddSingleton<IStringLocalizerFactory, SixnetResourceStringLocalizerFactory>();
            services.TryAddSingleton<IStringLocalizer, SixnetStringLocalizer>();
            services.TryAddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));
            services.Configure<LocalizationOptions>(op =>
            {
                op.ResourcesPath = options.ResourcesPath;
            });

            // sixnet
            services.TryAddSingleton<ISixnetStringLocalizerFactory, SixnetStringLocalizerFactory>();
            services.TryAddSingleton<ISixnetResourceManagerFactory, SixnetResourceManagerFactory>();
            services.TryAddSingleton<ISixnetStringLocalizer, SixnetStringLocalizer>();
            services.TryAddSingleton(typeof(ISixnetStringLocalizer<>), typeof(SixnetStringLocalizer<>));
            services.Configure(configure);

            return services;
        }
    }
}
