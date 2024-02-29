using AutoMapper;
using Sixnet.DependencyInjection;
using Sixnet.Mapper;

namespace Sixnet.App
{
    /// <summary>
    /// Application options
    /// </summary>
    public class ApplicationOptions
    {
        /// <summary>
        /// Gets or sets object mapper builder
        /// </summary>
        public ISixnetMapperBuilder MapperBuilder { get; set; } = new ConventionMapperBuilder();

        /// <summary>
        /// Gets or sets the dependency injection container
        /// </summary>
        public ISixnetContainer DIContainer { get; set; }

        /// <summary>
        /// Gets or sets the file match options
        /// </summary>
        public FileMatchOptions FileMatchOptions { get; private set; } = new();

        /// <summary>
        /// Gets the current application info
        /// </summary>
        public ApplicationInfo Current { get; internal set; } = SixnetApplication.Current;

        /// <summary>
        /// Whether register default service
        /// </summary>
        public bool RegisterDefaultService { get; set; } = true;

        /// <summary>
        /// Gets or sets the virtual path
        /// </summary>
        public string VirtualPath { get; set; }
    }
}
