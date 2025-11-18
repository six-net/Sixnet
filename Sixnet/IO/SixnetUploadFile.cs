namespace Sixnet.IO
{
    /// <summary>
    /// Sixnet upload file
    /// </summary>
    [Serializable]
    public class SixnetUploadFile
    {
        /// <summary>
        /// Gets or sets the file object name
        /// </summary>
        public string ObjectName { get; set; }

        /// <summary>
        /// Gets or sets file name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets file suffix
        /// </summary>
        public string Suffix { get; set; }

        /// <summary>
        /// Gets or sets the file content
        /// </summary>
        public byte[] Content { get; set; }
    }
}
