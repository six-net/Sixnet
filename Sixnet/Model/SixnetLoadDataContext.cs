using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Model
{
    public interface ISixnetLoadDataContext<out T>
    {
        /// <summary>
        /// Gets or sets the loadable
        /// </summary>
        ISixnetLoadable Loadable { get; set; }

        /// <summary>
        /// Gets or sets the source datas
        /// </summary>
        IEnumerable<T> GetSourceDatas();
    }

    /// <summary>
    /// Load data context
    /// </summary>
    public class SixnetLoadDataContext<T> : ISixnetLoadDataContext<T>
    {
        private readonly IEnumerable<T> _sourceDatas;

        public SixnetLoadDataContext(IEnumerable<T> sourceDatas, ISixnetLoadable loadable = null)
        {
            _sourceDatas = sourceDatas;
            Loadable = loadable;
        }

        /// <summary>
        /// Gets or sets the loadable
        /// </summary>
        public ISixnetLoadable Loadable { get; set; }

        /// <summary>
        /// Get source datas
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetSourceDatas()
        {
            return _sourceDatas;
        }
    }
}
