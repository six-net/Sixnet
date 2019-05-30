using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.Command.Modify
{
    public class FixedModifyValue : IModifyValue
    {
        /// <summary>
        /// value
        /// </summary>
        public dynamic Value
        {
            get; set;
        }

        public FixedModifyValue(dynamic value)
        {
            Value = value;
        }

        public dynamic GetModifyValue(dynamic originValue)
        {
            return Value;
        }
    }
}
