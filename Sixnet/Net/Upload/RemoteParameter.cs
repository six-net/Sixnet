using System;
using System.Collections.Generic;

namespace Sixnet.Net.Upload
{
    /// <summary>
    /// Remote upload parameter
    /// </summary>
    [Serializable]
    public class RemoteParameter
    {
        /// <summary>
        /// Gets or sets upload files
        /// </summary>
        public List<UploadFile> Files { get; set; }

        /// <summary>
        /// Gets or sets upload file parameter name
        /// Default value is 'sixnet_upload_file_options'
        /// </summary>
        public const string RequestParameterName = "sixnet_upload_file_options";
    }
}
