using System;

namespace Sixnet.IO
{
    /// <summary>
    /// Defines the location to upload to
    /// </summary>
    [Serializable]
    public enum UploadLocation
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
