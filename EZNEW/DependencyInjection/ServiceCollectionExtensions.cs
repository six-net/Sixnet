using System;
using System.Collections.Generic;
using System.Text;
using EZNEW.Application;
using EZNEW.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEZNEW(this IServiceCollection services, Action<ApplicationOptions> configureApplicationDelegate = null)
        {
            ContainerManager.Init(services, configureApplicationDelegate);
            return services;
        }
    }
}
