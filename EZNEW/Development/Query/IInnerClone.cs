using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Development.Query
{
    /// <summary>
    /// Defines inner clone contract
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IInnerClone<T>
    {
        /// <summary>
        /// Clone an object
        /// </summary>
        /// <returns></returns>
        T Clone();
    }
}
