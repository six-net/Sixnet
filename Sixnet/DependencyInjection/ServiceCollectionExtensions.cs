using System;
using Sixnet.App;
using Sixnet.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSixnet(this IServiceCollection services, Action<ApplicationOptions> configureApp = null)
        {
            return ContainerManager.Configure(services, null, configureApp);
        }
    }
}
