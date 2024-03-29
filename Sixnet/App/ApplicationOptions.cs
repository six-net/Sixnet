﻿using AutoMapper;
using Sixnet.DependencyInjection;
using Sixnet.Mapper;

namespace Sixnet.App
{
    /// <summary>
    /// Defines application options
    /// </summary>
    public class ApplicationOptions
    {
        /// <summary>
        /// Gets or sets object mapper builder
        /// </summary>
        public IMapperBuilder MapperBuilder { get; set; } = new ConventionMapperBuilder();

        /// <summary>
        /// Gets or sets the dependency injection container
        /// </summary>
        public IDIContainer DIContainer { get; set; }

        /// <summary>
        /// Gets or sets the file match options
        /// </summary>
        public FileMatchOptions FileMatchOptions { get; private set; } = new();

        /// <summary>
        /// Gets the current application info
        /// </summary>
        public ApplicationInfo Current { get; internal set; } = ApplicationManager.Current;

        /// <summary>
        /// Whether register project default service
        /// </summary>
        public bool RegisterProjectDefaultService { get; set; } = true;
    }
}
