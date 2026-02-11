// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sixnet.Development.Data.Database;

namespace Sixnet.Development.Entity
{
    /// <summary>
    /// Sixnet localization entity
    /// </summary>
    [Entity(TableName = "SIXNET_LOCALIZATION", SplitTableType = SplitTableType.Custom, SplitTableProviderName = DefaultLocalizationSplitTableProvider.Name, Module = "SIXNET", Description = "Localization", IsSystem = true)]
    public class SixnetLocalizationEntity : SixnetBaseEntity<SixnetLocalizationEntity>
    {
        /// <summary>
        /// Code
        /// </summary>
        [EntityField(Role = FieldRole.PrimaryKey, Length = 1000)]
        public string Code { get; set; }

        /// <summary>
        /// Text
        /// </summary>
        [EntityField(Length = 2000)]
        public string Text { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        [EntityField(Length = 2000)]
        public string Description { get; set; }
    }
}
