using System;
using System.Collections.Generic;

namespace EZNEW.Upload
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
        /// Default value is 'eznew_upload_file_option'
        /// </summary>
        public const string RequestParameterName = "eznew_upload_file_option";
    }
}
