using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Model
{
    /// <summary>
    /// Load data context
    /// </summary>
    public class SixnetLoadDataContext<T>
    {
        /// <summary>
        /// Gets or sets the loadable
        /// </summary>
        public SixnetLoadable Loadable { get; set; }

        /// <summary>
        /// Gets or sets the source datas
        /// </summary>
        public IEnumerable<T> SourceDatas { get; set; }
    }
}
