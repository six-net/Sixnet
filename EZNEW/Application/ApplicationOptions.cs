using System;
using System.Collections.Generic;
using System.Text;
using EZNEW.Mapper;
using AutoMapper;

namespace EZNEW.Application
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
        /// Gets or sets the file match options
        /// </summary>
        public FileMatchOptions FileMatchOptions { get; private set; } = new();

        public ApplicationInfo Current { get; internal set; } = ApplicationManager.Current;
    }
}
