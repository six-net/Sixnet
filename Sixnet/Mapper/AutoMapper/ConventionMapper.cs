using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMapper
{
    internal class ConventionMapper : Sixnet.Mapper.IMapper
    {
        readonly IMapper Mapper = null;

        /// <summary>
        /// Object map transformation
        /// </summary>
        /// <typeparam name="T">Target data type</typeparam>
        /// <param name="sourceObj">Source object</param>
        /// <returns>Return a target object</returns>
        public T MapTo<T>(object sourceObj)
        {
            return Mapper.Map<T>(sourceObj);
        }

        /// <summary>
        /// Convert an object
        /// </summary>
        /// <param name="destinationType">Destination data type</param>
        /// <param name="source">Source object</param>
        /// <returns>Return ta destionation object</returns>
        public object MapTo(Type destinationType, object source)
        {
            return Mapper.Map(source, source?.GetType(), destinationType);
        }

        public ConventionMapper(Action<IMapperConfigurationExpression> configuration)
        {
            var mapperConfiguration = new MapperConfiguration(configuration);
            Mapper = mapperConfiguration.CreateMapper();
        }
    }
}
