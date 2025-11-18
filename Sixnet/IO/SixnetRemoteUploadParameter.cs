// "Company © 2025. All rights reserved."

namespace Sixnet.IO
{
    /// <summary>
    /// Remote upload parameter
    /// </summary>
    [Serializable]
    public class SixnetRemoteUploadParameter
    {
        /// <summary>
        /// Gets or sets upload items
        /// </summary>
        public List<SixnetUploadFile> Items { get; set; }
    }
}
