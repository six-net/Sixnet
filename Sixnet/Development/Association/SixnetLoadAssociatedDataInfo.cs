// "Company © 2025. All rights reserved."

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.Development.Association
{
    internal class SixnetLoadAssociatedDataInfo
    {
        /// <summary>
        /// Gets or sets the loadable
        /// </summary>
        public ISixnetAssociationLoadable Loadable { get; set; }

        /// <summary>
        /// Gets or sets the source datas
        /// </summary>
        public IEnumerable SourceDatas { get; set; }

        /// <summary>
        /// Gets or sets the data type
        /// </summary>
        public Type SourceDataType { get; set; }
    }
}
