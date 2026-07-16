// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.Development.Data.Database
{
    public class SixnetEntityIndexInfo
    {
        /// <summary>
        /// Gets or ses the table
        /// </summary>
        public SixnetDatabaseObjectName Table { get; set; }

        /// <summary>
        /// Gets or sets the fields
        /// </summary>
        public List<SixnetEntityIndexField> Fields { get; set; }

        /// <summary>
        /// Whether is a unique index
        /// </summary>
        public bool Unique { get; set; }
    }

    public struct SixnetEntityIndexField
    {
        public SixnetDatabaseObjectName Name { get; set; }

        public int Sequence { get; set; }

        public bool Desc { get; set; }
    }
}
