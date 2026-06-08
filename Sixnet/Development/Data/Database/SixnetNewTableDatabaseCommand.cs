// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// DatabaseNewTableCommand
    /// </summary>
    /// <remarks>作者: dingbin.li, 时间: 2023/11/7 22:49:06, 版本: 1.0, 描述: 创建</remarks>
    public class SixnetNewTableDatabaseCommand : SixnetDatabaseCommand
    {
        private SixnetNewTableDatabaseCommand() { }

        /// <summary>
        /// Gets or sets the new table info
        /// </summary>
        public SixnetNewTableInfo NewTableInfo { get; set; }
    }
}
