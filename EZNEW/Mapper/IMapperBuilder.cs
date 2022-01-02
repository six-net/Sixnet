using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;

namespace EZNEW.Mapper
{
    public interface IMapperBuilder
    {
        /// <summary>
        /// Create mapper
        /// </summary>
        /// <returns>Return a IMapper object</returns>
        IMapper CreateMapper();

        /// <summary>
        /// Configure map
        /// </summary>
        /// <param name="configurationAction">Configuration action</param>
        void ConfigureMap(Action<IMapperConfigurationExpression> configurationAction);
    }
}
