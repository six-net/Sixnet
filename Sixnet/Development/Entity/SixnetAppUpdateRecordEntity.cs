// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper.Configuration.Conventions;

namespace Sixnet.Development.Entity
{
    /// <summary>
    /// Sixnet app update record entity
    /// </summary>
    [SixnetEntity(TableName = "SIXNET_APPLICATION_UPDATE_RECORD", Module = "", Description = "App update record", IsSystem = true)]
    public class SixnetAppUpdateRecordEntity : SixnetCreateUpdateDateEntity<SixnetAppUpdateRecordEntity>
    {
        /// <summary>
        /// Id
        /// </summary>
        [SixnetEntityField(Role = SixnetFieldRole.PrimaryKey)]
        public long Id { get; set; }

        /// <summary>
        /// Record app version
        /// </summary>
        [SixnetEntityField(Length = 30)]
        public string RecordAppVersion { get; set; }

        /// <summary>
        /// Record app version id
        /// </summary>
        [SixnetEntityField()]
        public long RecordAppVersionId { get; set; }

        /// <summary>
        /// Current app version
        /// </summary>
        [SixnetEntityField(Length = 30)]
        public string CurrentAppVersion { get; set; }

        /// <summary>
        /// Current app version id
        /// </summary>
        [SixnetEntityField()]
        public long CurrentAppVersionId { get; set; }

        /// <summary>
        /// Execute count
        /// </summary>
        [SixnetEntityField()]
        public int ExecuteCount {  get; set; }

        /// <summary>
        /// Note
        /// </summary>
        [SixnetEntityField(Length = 2000)]
        public string Note { get; set; }
    }
}
