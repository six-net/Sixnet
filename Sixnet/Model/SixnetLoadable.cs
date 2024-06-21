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
            IEnumerable<string> dataNameCollection = dataNames;
            Need(dataNameCollection);
        }

        /// <summary>
        /// Set need to load datas
        /// </summary>
        /// <param name="dataNames">Data names</param>
        public void Need(IEnumerable<string> dataNames)
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

        /// <summary>
        /// Set need to load datas
        /// </summary>
        /// <param name="originalLoadable">Original loadable</param>
        public void Need(ISixnetLoadable originalLoadable)
        {
            var originalDataNames = originalLoadable?.GetDataNames();
            if (originalDataNames?.IsNullOrEmpty() ?? true)
            {
                return;
            }
            AllowedLoadDataNames ??= new HashSet<string>();
            foreach (var name in originalDataNames)
            {
                AllowedLoadDataNames.Add(name);
            }
        }

        /// <summary>
        /// Get data names
        /// </summary>
        /// <returns></returns>
        public List<string> GetDataNames()
        {
            if (AllowedLoadDataNames.IsNullOrEmpty())
            {
                return new List<string>(0);
            }
            return new List<string>(AllowedLoadDataNames);
        }
    }
}
