// "Company © 2025. All rights reserved."

namespace Sixnet.IO
{
    /// <summary>
    /// Upload parameter
    /// </summary>
    public class SixnetUploadParameter
    {
        /// <summary>
        /// Files
        /// </summary>
        public List<SixnetUploadFile> Files { get; set; }

        /// <summary>
        /// Sixnet file setting
        /// </summary>
        public SixnetFileSetting Setting { get; set; }

        /// <summary>
        /// Upload properties
        /// </summary>
        public Dictionary<string, string> Properties { get; set; }
    }
}
