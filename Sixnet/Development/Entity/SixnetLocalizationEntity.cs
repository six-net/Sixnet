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
    [SixnetEntity(TableName = "SIXNET_LOCALIZATION", SplitTableType = SixnetSplitTableType.Custom, SplitTableProviderName = SixnetDefaultLocalizationSplitTableProvider.Name, Module = "SIXNET", Description = "Localization", IsSystem = true)]
    public class SixnetLocalizationEntity : SixnetBaseEntity<SixnetLocalizationEntity>
    {
        /// <summary>
        /// Code
        /// </summary>
        [SixnetEntityField(Role = SixnetFieldRole.PrimaryKey, Length = 1000)]
        public string Code { get; set; }

        /// <summary>
        /// Text
        /// </summary>
        [SixnetEntityField(Length = 2000)]
        public string Text { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        [SixnetEntityField(Length = 2000)]
        public string Description { get; set; }
    }
}
