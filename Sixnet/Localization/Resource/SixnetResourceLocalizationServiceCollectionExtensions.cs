using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Sixnet.Exceptions;
using Sixnet.Localization.Json;

namespace Sixnet.Localization.Resource
{
    /// <summary>
    /// Resource localization service collection extensions
    /// </summary>
    internal static class SixnetResourceLocalizationServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services required for application localization.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddSixnetResourceLocalization(this IServiceCollection services)
        {
            ThrowHelper.ThrowArgNullIf(services == null, nameof(services));
            services.AddOptions();
            AddSixnetLocalizationServices(services);
            return services;
        }

        /// <summary>
        /// Adds services required for application localization.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="setupAction">
        /// An <see cref="Action{LocalizationOptions}"/> to configure the <see cref="LocalizationOptions"/>.
        /// </param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddSixnetResourceLocalization(
            this IServiceCollection services,
            Action<LocalizationOptions> setupAction)
        {
            ThrowHelper.ThrowArgNullIf(services == null, nameof(services));
            ThrowHelper.ThrowArgNullIf(setupAction == null, nameof(setupAction));
            AddSixnetLocalizationServices(services, setupAction);
            return services;
        }

        // To enable unit testing
        internal static void AddSixnetLocalizationServices(IServiceCollection services)
        {
            services.TryAddSingleton<ISixnetResourceManagerFactory, SixnetResourceManagerFactory>();
        }

        internal static void AddSixnetLocalizationServices(
            IServiceCollection services,
            Action<LocalizationOptions> setupAction)
        {
            AddSixnetLocalizationServices(services);
            services.Configure(setupAction);
        }
    }
}
