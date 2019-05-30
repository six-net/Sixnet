using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.CQuery.CriteriaConvert
{
    public static class CriteriaConvertFactory
    {
        /// <summary>
        /// create criterial convert type
        /// </summary>
        /// <param name="type">convert type</param>
        /// <returns></returns>
        public static ICriteriaConvert Create(CriteriaConvertType type)
        {
            ICriteriaConvert convert = null;
            switch (type)
            {
                case CriteriaConvertType.StringLength:
                    convert = new StringLengthCriteriaConvert();
                    break;
            }
            return convert;
        }
    }
}
