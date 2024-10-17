using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Net.Upload
{
    /// <summary>
    /// Move upload file parameter
    /// </summary>
    public class MoveUploadFileParameter
    {
        /// <summary>
        /// Gets or sets the object
        /// </summary>
        public string ObjectName {  get; set; }

        /// <summary>
        /// Gets or sets the relative file paths
        /// </summary>
        public List<string> RelativeFilePaths {  get; set; }
    }
}
