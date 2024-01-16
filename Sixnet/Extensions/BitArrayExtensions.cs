namespace System.Collections
{
    /// <summary>
    /// Bit array extensions
    /// </summary>
    public static class BitArrayExtensions
    {
        #region Generate a hash value from an array of bits

        /// <summary>
        /// Generate a hash value from an array of bits
        /// Return 0 if data is null
        /// </summary>
        /// <param name="data">Array of bits</param>
        /// <returns>Return the hash value</returns>
        public static int HashValue(this BitArray data)
        {
            if (data == null)
            {
                return 0;
            }
            int[] intArray = new int[(data.Length + 31) / 32];
            data.CopyTo(intArray, 0);
            unchecked
            {
                int hash = 23;
                foreach (int n in intArray)
                {
                    hash = hash * 37 + n;
                }
                return hash;
            }
        }

        #endregion

        #region Check if two arrays of bits are equals

        /// <summary>
        /// Check if two arrays of bits are equals
        /// </summary>
        /// <param name="sourceData">Source data</param>
        /// <param name="targetData">Target data</param>
        /// <returns>Return whether </returns>
        public static bool Compare(this BitArray sourceData, BitArray targetData)
        {
            if (sourceData == null || targetData == null || sourceData.Length != targetData.Length)
            {
                return false;
            }
            var sourceDataEnumerator = sourceData.GetEnumerator();
            var targetDataEnumerator = targetData.GetEnumerator();
            while (sourceDataEnumerator.MoveNext() && targetDataEnumerator.MoveNext())
            {
                if ((bool)sourceDataEnumerator.Current != (bool)targetDataEnumerator.Current)
                {
                    return false;
                }
            }
            return true;
        } 

        #endregion
    }
}
