// "Company © 2025. All rights reserved."

namespace Sixnet.IO
{
    /// <summary>
    /// Defines the location to upload to
    /// </summary>
    [Serializable]
    public enum SixnetUploadLocation
    {
        /// <summary>
        /// Upload file to local
        /// </summary>
        Local = 2,
        /// <summary>
        /// Upload file to remote server
        /// </summary>
        Remote = 4
    }
}
