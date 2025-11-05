// "Company © 2025. All rights reserved."

namespace Sixnet.IO
{
    public class SixnetFileOptions
    {
        /// <summary>
        /// Gets or sets the default file access setting
        /// </summary>
        public SixnetFileSetting Default { get; set; } = new();

        /// <summary>
        /// Gets or sets the file access setting
        /// Key: file object name
        /// </summary>
        public Dictionary<string, SixnetFileSetting> Files { get; set; }

        /// <summary>
        /// Gets or sets the temp folder
        /// </summary>
        public string UploadTempFolder { get; set; } = SixnetFileManager.DefaultTempFolder;

        /// <summary>
        /// Gets or sets the upload root folder
        /// </summary>
        public string UploadRootFolder { get; set; } = SixnetFileManager.DefaultContentFolder;

        /// <summary>
        /// Gets upload provider
        /// </summary>
        public Func<SixnetUploadParameter, ISixnetUploadProvider> GetUploadProvider { get; set; }
    }
}
