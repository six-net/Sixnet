using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sixnet.DependencyInjection;

namespace Sixnet
{
    public static class Sixneter
    {
        #region Fields

        /// <summary>
        /// Sixnet options
        /// </summary>
        internal static SixnetOptions Options = new();

        #endregion

        #region Init

        /// <summary>
        /// Init sixnet
        /// </summary>
        /// <param name="options"></param>
        public static void Init(SixnetOptions options = null)
        {
            if (options != null)
            {
                Options = options;
            }
            InitCore();
        }

        /// <summary>
        /// Init sixnet
        /// </summary>
        /// <param name="configure"></param>
        public static void Init(Action<SixnetOptions> configure)
        {
            configure?.Invoke(Options);
            InitCore();
        }

        /// <summary>
        /// Init core
        /// </summary>
        static void InitCore()
        {
            if(Options.Services == null)
            {
                Host.CreateDefaultBuilder(Options.Args)
                    .UseServiceProviderFactory(new SixnetServiceProviderFactory())
                    .Build();
            }
            else
            {
                _ = SixnetContainer.Configure(Options);
            }
        } 

        #endregion
    }
}
