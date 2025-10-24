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
        #region Fields

        /// <summary>
        /// Sixnet options
        /// </summary>
        internal static SixnetOptions Options = new();

        #endregion

        #region Run

        /// <summary>
        /// Run application
        /// </summary>
        /// <param name="main"></param>
        /// <param name="args"></param>
        /// <param name="options"></param>
        public static void Run(Action<string[]> main, string[] args, SixnetOptions options = null)
        {
            var host = Init(options);
            if (host != null)
            {
                host.Start();
            }

            main?.Invoke(args);

            if (host != null)
            {
                host.StopAsync();
            }
        }

        /// <summary>
        /// Run application
        /// </summary>
        /// <param name="main"></param>
        /// <param name="args"></param>
        /// <param name="options"></param>
        public static async Task RunAsync(Action<string[]> main, string[] args, SixnetOptions options = null)
        {
            var host = Init(options);
            if (host != null)
            {
                await host.StartAsync();
            }

            main?.Invoke(args);

            if (host != null)
            {
                await host.StopAsync();
            }
        }

        /// <summary>
        /// Run application
        /// </summary>
        /// <param name="main"></param>
        /// <param name="args"></param>
        /// <param name="configure"></param>
        public static void Run(Action<string[]> main, string[] args, Action<SixnetOptions> configure)
        {
            var host = Init(configure);
            if (host != null)
            {
                host.Start();
            }

            main?.Invoke(args);

            if (host != null)
            {
                host.StopAsync();
            }
        }

        /// <summary>
        /// Run application
        /// </summary>
        /// <param name="main"></param>
        /// <param name="args"></param>
        /// <param name="configure"></param>
        public static async Task RunAsync(Action<string[]> main, string[] args, Action<SixnetOptions> configure)
        {
            var host = Init(configure);
            if (host != null)
            {
                await host.StartAsync();
            }

            main?.Invoke(args);

            if (host != null)
            {
                await host.StopAsync();
            }
        }

        #endregion

        #region Init

        /// <summary>
        /// Init sixnet
        /// </summary>
        /// <param name="options"></param>
        internal static IHost Init(SixnetOptions options = null)
        {
            if (options != null)
            {
                Options = options;
            }
            return InitCore();
        }

        /// <summary>
        /// Init sixnet
        /// </summary>
        /// <param name="configure"></param>
        internal static IHost Init(Action<SixnetOptions> configure)
        {
            configure?.Invoke(Options);
            return InitCore();
        }

        /// <summary>
        /// Init core
        /// </summary>
        static IHost InitCore()
        {
            if (Options.HostBuilder == null)
            {
                SixnetLogger.LogDebug($"Init sixnet through new self host");

                var hostBuilder = Host.CreateDefaultBuilder(Options.Args)
                    .UseServiceProviderFactory(new SixnetServiceProviderFactory());
                Options.HostBuilder = hostBuilder;
                return hostBuilder.Build();
            }
            else
            {
                _ = SixnetContainer.Configure(Options);
                return null;
            }
        }

        #endregion
    }
}
