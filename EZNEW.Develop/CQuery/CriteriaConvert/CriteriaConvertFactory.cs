using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.CQuery.CriteriaConvert
{
    public static class CriteriaConvertFactory
    {
        /// <summary>
        /// create criterial convert
        /// </summary>
        /// <param name="convertConfigName">convert config name</param>
        /// <returns></returns>
        public static ICriteriaConvert Create(string convertConfigName)
        {
            return new DefaultCriteriaConvert()
            {
                Name = convertConfigName
            };
        }
    }
}
