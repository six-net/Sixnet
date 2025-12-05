// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sixnet.Development.Data.Client;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Sixnet database update context
    /// </summary>
    public class SixnetUpdateDatabaseContext
    {
        /// <summary>
        /// Update parameter
        /// </summary>
        public SixnetUpdateDatabaseParameter UpdateParameter { get; set; }
        
        /// <summary>
        /// Current version
        /// </summary>
        public Version CurrentVersion { get; set; }

        /// <summary>
        /// Current record id
        /// </summary>
        public long CurrentRecordId {  get; set; }
    }
}
