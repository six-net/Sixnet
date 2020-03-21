using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.Command.Modify
{
    public interface IModifyValue
    {
        /// <summary>
        /// calculate value
        /// </summary>
        dynamic Value { get; }

        /// <summary>
        /// get modifyed value
        /// </summary>
        /// <param name="originValue"></param>
        /// <returns></returns>
        dynamic GetModifyValue(dynamic originValue);
    }
}
