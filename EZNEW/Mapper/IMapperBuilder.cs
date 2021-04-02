using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Mapper
{
    public interface IMapperBuilder
    {
        /// <summary>
        /// Create mapper
        /// </summary>
        /// <param name="configurationTypes">Configuration types</param>
        /// <returns>Return a IMapper object</returns>
        IMapper CreateMapper(IEnumerable<Type> configurationTypes);
    }
}
