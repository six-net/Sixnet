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
        /// <returns>Return a IMapper object</returns>
        IMapper CreateMapper();
    }
}
