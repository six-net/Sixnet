using System;
using System.Collections.Generic;
using System.Linq;
using Sixnet.Code;

namespace Sixnet.Algorithm.Selection
{
    /// <summary>
    /// Shufflenet algorithm implement by dotnet
    /// </summary>
    [Serializable]
    public class ShuffleNet<T>
    {
        IEnumerable<T> originalValues;
        T[] valueArray;
        int lastIndex = 0;

        /// <summary>
        /// Get or set a value whether enable to take value by loop
        /// </summary>
        public bool Loop { get; set; }

        /// <summary>
        /// Initializes a new instance of the EZNEW.Code.ShuffleNet class
        /// </summary>
        /// <param name="originalValues">Original values</param>
        /// <param name="derange">Whether to derange value when Initialize</param>
        /// <param name="loop">Enable to take value by loop</param>
        public ShuffleNet(IEnumerable<T> originalValues, bool derange = true, bool loop = false)
        {
            this.originalValues = originalValues;
            valueArray = this.originalValues?.ToArray() ?? new T[0];
            Loop = loop;
            if (derange)
            {
                Derange();
            }
        }

        /// <summary>
        /// Derange values
        /// </summary>
        /// <param name="useOriginalValue">whether to use original value to derange</param>
        public void Derange(bool useOriginalValue = false)
        {
            if (useOriginalValue)
            {
                valueArray = originalValues?.ToArray() ?? new T[0];
            }
            if (valueArray.IsNullOrEmpty())
            {
                return;
            }
            lastIndex = 0;
            var size = valueArray.Length;
            for (var i = size - 1; i >= 0; --i)
            {
                var nextIndex = RandomNumberHelper.GetRandomNumber(i);
                var nextValue = valueArray[nextIndex];
                valueArray[nextIndex] = valueArray[i];
                valueArray[i] = nextValue;
            }
        }

        /// <summary>
        /// Reset
        /// </summary>
        /// <param name="derange">Whether derange value</param>
        public void Reset(bool derange = true)
        {
            if (derange)
            {
                Derange();
            }
            lastIndex = 0;
        }

        /// <summary>
        ///  Take next values
        /// </summary>
        /// <param name="size">Return data size</param>
        /// <param name="derange">Whether derange data before take value</param>
        /// <param name="derangeByOriginalValue">Use original value to derange data</param>
        /// <returns>Return values</returns>
        public IEnumerable<T> TakeNextValues(int size, bool derange = false, bool derangeByOriginalValue = false)
        {
            if (size < 1 || (!Loop && lastIndex >= valueArray.Length))
            {
                return Array.Empty<T>();
            }
            if (size >= valueArray.Length)
            {
                return valueArray;
            }
            if (derange)
            {
                Derange(derangeByOriginalValue);
            }
            var outNum = lastIndex + size - valueArray.Length;
            var nextValues = valueArray.Skip(lastIndex).Take(size);
            if (Loop && outNum > 0)
            {
                lastIndex = outNum;
                nextValues = nextValues.Union(valueArray.Take(outNum));
            }
            else
            {
                lastIndex += size;
            }
            return nextValues;
        }
    }
}
