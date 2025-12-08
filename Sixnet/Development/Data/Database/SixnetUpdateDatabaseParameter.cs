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

        ///// <summary>
        ///// Records
        ///// </summary>
        //public List<ISixnetDatabaseUpdateRecord> Records { get; set; }

        /// <summary>
        /// Report process
        /// </summary>
        public Action<UpdateDatabaseProcess> ReportProcess { get; set; }
    }
}
