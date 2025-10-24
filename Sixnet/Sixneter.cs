using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sixnet.DependencyInjection;
using Sixnet.Logging;

namespace Sixnet
{
    /// <summary>
    /// Sixnet manager
    /// </summary>
    public static class Sixneter
    {
        #region Run

        /// <summary>
        /// Run application
        /// </summary>
        /// <param name="main"></param>
        /// <param name="options"></param>
        public static void Run(Action<string[]> main, SixnetOptions options = null)
        {
            options ??= new SixnetOptions();
            var host = Init(options);
            host?.Start();
            main?.Invoke(options?.Args);
            host?.StopAsync();
        }

        /// <summary>
        /// Run application
        /// </summary>
        /// <param name="main"></param>
        /// <param name="options"></param>
        public static async Task RunAsync(Action<string[]> main, SixnetOptions options = null)
        {
            options ??= new SixnetOptions();
            var host = Init(options);
            if (host != null)
            {
                await host.StartAsync();
            }

            main?.Invoke(options?.Args);

            if (host != null)
            {
                await host.StopAsync();
            }
        }

        /// <summary>
        /// Run application
        /// </summary>
        /// <param name="main"></param>
        /// <param name="configure"></param>
        public static void Run(Action<string[]> main, Action<SixnetOptions> configure)
        {
            var options = new SixnetOptions();
            configure?.Invoke(options);
            var host = Init(options);
            if (host != null)
            {
                host.Start();
            }

            main?.Invoke(options.Args);

            if (host != null)
            {
                host.StopAsync();
            }
        }

        /// <summary>
        /// Run application
        /// </summary>
        /// <param name="main"></param>
        /// <param name="configure"></param>
        public static async Task RunAsync(Action<string[]> main, Action<SixnetOptions> configure)
        {
            var options = new SixnetOptions();
            configure?.Invoke(options);
            var host = Init(options);
            if (host != null)
            {
                await host.StartAsync();
            }

            main?.Invoke(options.Args);

            if (host != null)
            {
                await host.StopAsync();
            }
        }

        #endregion

        #region Init

        /// <summary>
        /// Init core
        /// </summary>
        internal static IHost Init(SixnetOptions options)
        {
            if (options.HostBuilder == null)
            {
                SixnetLogger.LogDebug($"Init sixnet through new self host");
                var hostBuilder = Host.CreateDefaultBuilder(options.Args)
                    .UseServiceProviderFactory(new SixnetServiceProviderFactory(options));
                options.ConfigureHostBuilder?.Invoke(hostBuilder);
                options.HostBuilder = hostBuilder;
                return hostBuilder.Build();
            }
            else
            {
                _ = SixnetContainer.Configure(options);
                return null;
            }
        }

        #endregion
    }
}
