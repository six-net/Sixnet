using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;

namespace Sixnet.Mapper
{
    public interface ISixnetMapperBuilder
    {
        /// <summary>
        /// Create mapper
        /// </summary>
        /// <returns>Return a IMapper object</returns>
        ISixnetMapper CreateMapper();

        /// <summary>
        /// Configure map
        /// </summary>
        /// <param name="configurationAction">Configuration action</param>
        void ConfigureMap(Action<IMapperConfigurationExpression> configurationAction);
    }
}
