using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.IO
{
    /// <summary>
    /// Store uploaded file parameter
    /// </summary>
    public class SixnetStoreUploadedFileParameter
    {
        /// <summary>
        /// Gets or sets the object
        /// </summary>
        public string ObjectName { get; set; }

        /// <summary>
        /// Gets or sets the relative file paths
        /// </summary>
        public List<string> RelativeFilePaths { get; set; }
    }
}
