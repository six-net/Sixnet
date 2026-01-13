// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Update database parameter
    /// </summary>
    public class SixnetUpdateDatabaseParameter
    {
        /// <summary>
        /// Database server
        /// </summary>
        public DatabaseServer DatabaseServer { get; set; }

        /// <summary>
        /// Target version
        /// </summary>
        public Version TargetVersion { get; set; }

        /// <summary>
        /// Database update records
        /// </summary>
        public List<ISixnetDatabaseUpdateRecord> Records { get; set; }

        /// <summary>
        /// Whether execute record directly
        /// </summary>
        public bool ExecuteRecordDirectly {  get; set; }

        /// <summary>
        /// Whether execute record for rollback
        /// </summary>
        public bool ExecuteRecordForRollback { get; set; }

        /// <summary>
        /// Report process
        /// </summary>
        public Action<UpdateDatabaseProcess> ReportProcess { get; set; }
    }
}
