using System;
using System.Collections.Generic;

namespace EZNEW.Upload.Configuration
{
    /// <summary>
    /// Upload file configuration
    /// </summary>
    [Serializable]
    public class UploadConfiguration
    {
        /// <summary>
        /// Gets or sets the default upload option
        /// </summary>
        public UploadOptions Default { get; set; }

        /// <summary>
        /// Gets or sets the upload configuration for the specified upload object
        /// </summary>
        public List<UploadObject> UploadObjects { get; set; } = new List<UploadObject>();

        /// <summary>
        /// Gets or sets the upload configuration name
        /// </summary>
        public static string UploadConfigurationName { get; set; } = nameof(UploadConfiguration);
    }
}
