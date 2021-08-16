using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Data
{
    /// <summary>
    /// Defines parameter handler
    /// </summary>
    public interface IParameterHandler
    {
        /// <summary>
        /// Parse parameter
        /// </summary>
        /// <param name="originalParameter">Original parameter</param>
        /// <returns></returns>
        ParameterItem Parse(ParameterItem originalParameter);
    }
}
