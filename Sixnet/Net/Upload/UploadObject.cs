using System;

namespace Sixnet.Net.Upload
{
    /// <summary>
    /// Upload object
    /// </summary>
    [Serializable]
    public class UploadObject
    {
        /// <summary>
        /// Gets or sets upload object name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets upload options
        /// </summary>
        public UploadSetting Options { get; set; }
    }
}
