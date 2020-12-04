using System;

namespace EZNEW.Upload
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
        /// Gets or sets upload option
        /// </summary>
        public UploadOptions UploadOption { get; set; }
    }
}
