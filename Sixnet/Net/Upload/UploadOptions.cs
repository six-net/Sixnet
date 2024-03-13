using System;
using System.Collections.Generic;

namespace Sixnet.Net.Upload
{
    /// <summary>
    /// Upload options
    /// </summary>
    [Serializable]
    public class UploadOptions
    {
        /// <summary>
        /// Gets or sets the default upload setting
        /// </summary>
        public UploadSetting Default { get; set; } = new();

        /// <summary>
        /// Gets or sets the upload object settings
        /// Key: upload object name
        /// </summary>
        public Dictionary<string, UploadSetting> UploadObjects { get; set; } = new Dictionary<string, UploadSetting>();

        /// <summary>
        /// Gets upload provider
        /// </summary>
        public Func<UploadParameter, ISixnetUploadProvider> GetUploadProvider { get; set; }
    }
}
