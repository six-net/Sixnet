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
    [Entity(TableName = "SIXNET_APPLICATION_UPDATE_RECORD", Module = "", Description = "App update record", IsSystem = true)]
    public class SixnetAppUpdateRecordEntity : CreateUpdateDateEntity<SixnetAppUpdateRecordEntity>
    {
        /// <summary>
        /// Id
        /// </summary>
        [EntityField(Role = FieldRole.PrimaryKey)]
        public long Id { get; set; }

        /// <summary>
        /// Record app version
        /// </summary>
        [EntityField(Length = 30)]
        public string RecordAppVersion { get; set; }

        /// <summary>
        /// Record app version id
        /// </summary>
        [EntityField()]
        public long RecordAppVersionId { get; set; }

        /// <summary>
        /// Current app version
        /// </summary>
        [EntityField(Length = 30)]
        public string CurrentAppVersion { get; set; }

        /// <summary>
        /// Current app version id
        /// </summary>
        [EntityField()]
        public long CurrentAppVersionId { get; set; }

        /// <summary>
        /// Execute count
        /// </summary>
        [EntityField()]
        public int ExecuteCount {  get; set; }

        /// <summary>
        /// Note
        /// </summary>
        [EntityField(Length = 2000)]
        public string Note { get; set; }
    }
}
