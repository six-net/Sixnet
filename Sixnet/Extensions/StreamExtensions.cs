namespace System.IO
{
    /// <summary>
    /// Stream extensions
    /// </summary>
    public static class StreamExtensions
    {
        #region Serialization a stream object to byte array

        /// <summary>
        /// Serialization a stream object to byte array
        /// </summary>
        /// <param name="value">Stream value</param>
        /// <returns>Return the byte array</returns>
        public static byte[] ToBytes(this Stream value)
        {
            if (value == null)
            {
                return Array.Empty<byte>();
            }
            using (var memoryStream = new MemoryStream())
            {
                value.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        #endregion
    }
}
