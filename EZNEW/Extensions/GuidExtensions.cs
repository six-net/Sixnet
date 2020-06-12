namespace System
{
    /// <summary>
    /// Guid extensions
    /// </summary>
    public static class GuidExtensions
    {
        #region Verify guid vlaue whether is empty

        /// <summary>
        /// Verify Guid is empty or not
        /// </summary>
        /// <param name="value">Guid value</param>
        /// <returns>Return whether value is empty</returns>
        public static bool IsEmpty(this Guid value)
        {
            return value.Equals(Guid.Empty);
        }

        #endregion

        #region Convert Guid to Int64

        /// <summary>
        /// Convert Guid to Int64
        /// </summary>
        /// <param name="value">Guid value</param>
        /// <returns>Return the int64 value</returns>
        public static long ToInt64(this Guid value)
        {
            byte[] bytes = value.ToByteArray();
            return BitConverter.ToInt64(bytes, 0);
        }

        #endregion

        #region Convert Guid to Int32

        /// <summary>
        /// Convert Guid to Int32
        /// </summary>
        /// <param name="value">Guid value</param>
        /// <returns>Return the int32 value</returns>
        public static int ToInt32(this Guid value)
        {
            byte[] bytes = value.ToByteArray();
            return BitConverter.ToInt32(bytes, 0);
        }

        #endregion

        #region Convert guid to uniquecode(formatting to upper)

        /// <summary>
        /// Convert guid to uniquecode(formatting to upper)
        /// </summary>
        /// <param name="value">Guid value</param>
        /// <returns>Return the uniquecode value</returns>
        public static string ToUniqueCode(this Guid value)
        {
            long i = 1;
            byte[] bytes = value.ToByteArray();
            foreach (byte b in bytes)
            {
                i *= (b + 1);
            }
            string code = string.Format("{0:x}", i - DateTimeOffset.Now.Ticks);
            return code;
        }

        #endregion
    }
}
