using System;
using System.Collections.Generic;

namespace Sixnet.IO.FileAccess
{
    /// <summary>
    /// File access options
    /// </summary>
    [Serializable]
    public class FileAccessOptions
    {
        /// <summary>
        /// Gets or sets the default file access setting
        /// </summary>
        public FileAccessSetting Default { get; set; } = new();

        /// <summary>
        /// Gets or sets the file access setting
        /// Key: file object name
        /// </summary>
        public Dictionary<string, FileAccessSetting> Files { get; set; }
    }
}
