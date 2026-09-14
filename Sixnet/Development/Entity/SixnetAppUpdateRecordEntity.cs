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
        /// App version
        /// </summary>
        [SixnetEntityField(Length = 30)]
        public string AppVersion { get; set; }

        /// <summary>
        /// App version id
        /// </summary>
        [SixnetEntityField()]
        public long AppVersionId { get; set; }

        /// <summary>
        /// Execute count
        /// </summary>
        [SixnetEntityField()]
        public int ExecutionCount {  get; set; }

        /// <summary>
        /// Note
        /// </summary>
        [SixnetEntityField(Length = 2000)]
        public string Note { get; set; }
    }
}
