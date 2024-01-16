using System;

namespace Sixnet.Net.Upload
{
    /// <summary>
    /// Defines the target to upload to
    /// </summary>
    [Serializable]
    public enum UploadTarget
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
