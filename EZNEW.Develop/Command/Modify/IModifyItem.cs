using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.Command.Modify
{
    public interface IModifyItem
    {
        /// <summary>
        /// parse modify value
        /// </summary>
        /// <returns></returns>
        KeyValuePair<string, IModifyValue> ParseModifyValue();
    }
}
