using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Net.Upload
{
    /// <summary>
    /// Upload parameter
    /// </summary>
    public class UploadParameter
    {
        /// <summary>
        /// Files
        /// </summary>
        public List<UploadFile> Files { get; set; }

        /// <summary>
        /// Upload setting
        /// </summary>
        public UploadSetting Setting { get; set; }

        /// <summary>
        /// Upload properties
        /// </summary>
        public Dictionary<string, string> Properties { get; set; }
    }
}
