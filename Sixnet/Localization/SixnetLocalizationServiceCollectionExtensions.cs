using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Sixnet.Exceptions;
using Sixnet.Localization;
using Sixnet.Localization.Json;
using Sixnet.Localization.Resource;

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
            ThrowHelper.ThrowArgNullIf(services == null, nameof(services));

            var options = new SixnetLocalizationOptions();
            configure?.Invoke(options);

            switch (options.StringSource)
            {
                case LocalizationStringSource.Resource:
                    AddResourceLocalization(services, op =>
                    {
                        op.ResourcesPath = options.ResourcesPath;
                    });
                    break;
                case LocalizationStringSource.Json:
                    AddJsonLocalization(services, op =>
                    {
                        op.ResourcesPath = options.ResourcesPath;
                    });
                    break;
            }

            // system
            services.TryAddSingleton<IStringLocalizer, SixnetStringLocalizer>();
            services.TryAddSingleton<IStringLocalizerFactory, SixnetResourceStringLocalizerFactory>();
            services.TryAddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));

            // sixnet
            services.TryAddSingleton<ISixnetStringLocalizer, SixnetStringLocalizer>();
            services.TryAddTransient(typeof(ISixnetStringLocalizer<>), typeof(SixnetStringLocalizer<>));
            services.TryAddSingleton<ISixnetStringLocalizerFactory, SixnetStringLocalizerFactory>();

            return services;
        }

        /// <summary>
        /// Add resource localization
        /// </summary>
        /// <param name="services">Services</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        static IServiceCollection AddResourceLocalization(IServiceCollection services, Action<LocalizationOptions> configure = null)
        {
            services.AddSixnetResourceLocalization(configure);
            return services;
        }

        /// <summary>
        /// Add sixnet json localization
        /// </summary>
        /// <param name="services">Services</param>
        /// <param name="configure">Configure</param>
        /// <returns></returns>
        static IServiceCollection AddJsonLocalization(IServiceCollection services, Action<LocalizationOptions> configure = null)
        {
            services.AddSixnetJsonLocalization(configure);
            return services;
        }
    }
}
