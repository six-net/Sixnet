using System;
using Sixnet.Localization.Json;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Sixnet.Exceptions;
using Sixnet.Localization;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Json localization service collection extensions
    /// </summary>
    public static class JsonLocalizationServiceCollectionExtensions
    {
        /// <summary>
        /// Add json localization
        /// </summary>
        /// <param name="services">Services</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddSixnetJsonLocalization(this IServiceCollection services)
        {
            ThrowHelper.ThrowArgNullIf(services == null, nameof(services));
            services.AddOptions();
            AddSixnetJsonLocalizationServices(services);
            return services;
        }

        /// <summary>
        /// Add json localization
        /// </summary>
        /// <param name="services">Services</param>
        /// <param name="setupAction">Json localization options setup action</param>
        /// <returns></returns>
        public static IServiceCollection AddSixnetJsonLocalization(
           this IServiceCollection services,
           Action<LocalizationOptions> setupAction)
        {
            ThrowHelper.ThrowArgNullIf(services == null, nameof(services));
            ThrowHelper.ThrowArgNullIf(setupAction == null, nameof(setupAction));
            AddSixnetJsonLocalizationServices(services, setupAction);
            return services;
        }

        /// <summary>
        /// Add json localization services
        /// </summary>
        /// <param name="services">Services</param>
        internal static void AddSixnetJsonLocalizationServices(IServiceCollection services)
        {
            services.TryAddSingleton<ISixnetResourceManagerFactory, SixnetJsonManagerFactory>();
        }

        /// <summary>
        /// Add json localization services
        /// </summary>
        /// <param name="services">Services</param>
        /// <param name="setupAction">Json localization options setup action</param>
        internal static void AddSixnetJsonLocalizationServices(
            IServiceCollection services,
            Action<LocalizationOptions> setupAction)
        {
            AddSixnetJsonLocalizationServices(services);
            services.Configure(setupAction);
        }
    }
}