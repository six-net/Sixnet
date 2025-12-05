// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.Development.Entity
{
    /// <summary>
    /// Sixnet app update record entity
    /// </summary>
    [Entity(TableName = "SIXNET_APPLICATION_UPDATE_RECORD", Module = "", Description = "App update record", IsSystem = true)]
    public class SixnetAppUpdateRecordEntity : CreateDateEntity<SixnetAppUpdateRecordEntity>
    {
        /// <summary>
        /// Id
        /// </summary>
        [EntityField(Role = FieldRole.PrimaryKey)]
        public long Id { get; set; }

        /// <summary>
        /// App version
        /// </summary>
        [EntityField(Length = 30)]
        public string AppVersion { get; set; }

        /// <summary>
        /// App version id
        /// </summary>
        [EntityField(Length = 30)]
        public long AppVersionId {  get; set; } 

        /// <summary>
        /// Note
        /// </summary>
        [EntityField(Length = 2000)]
        public string Note { get; set; }
    }
}
