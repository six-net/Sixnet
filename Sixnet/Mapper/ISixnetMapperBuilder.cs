// "Company © 2025. All rights reserved."

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
