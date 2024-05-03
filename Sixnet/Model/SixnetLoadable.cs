using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Model
{
    public class SixnetLoadable : ISixnetLoadable
    {
        /// <summary>
        /// Allowed load data names
        /// </summary>
        protected HashSet<string> AllowedLoadDataNames { get; set; }

        /// <summary>
        /// Whether allow load data
        /// </summary>
        /// <param name="dataName">Data name</param>
        /// <returns></returns>
        public bool Allow(string dataName)
        {
            return !string.IsNullOrWhiteSpace(dataName)
            && (AllowedLoadDataNames?.Contains(dataName) ?? false);
        }

        /// <summary>
        /// Set need to load datas
        /// </summary>
        /// <param name="dataNames">Data names</param>
        /// <returns></returns>
        public void Need(params string[] dataNames)
        {
            if (!dataNames.IsNullOrEmpty())
            {
                AllowedLoadDataNames ??= new HashSet<string>();
                foreach (var name in dataNames)
                {
                    AllowedLoadDataNames.Add(name);
                }
            }
        }
    }
}
