﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Sixnet.Algorithm.Filter
{
    /// <summary>
    /// Implementation of a Bloom-filter
    /// </summary>
    public class SixnetBloomFilter
    {
        private readonly BitArray bitSet;
        private readonly int bitSetSize;
        private readonly double bitsPerElement;
        private readonly int expectedNumberOfFilterElements; // expected (maximum) number of elements to be added
        private int numberOfAddedElements; // number of elements actually added to the Bloom filter
        private readonly int hashFuncNumber; // number of hash functions

        /// <summary>
        /// Encoding charset
        /// Default value is UTF8
        /// </summary>
        public static Encoding Charset = Encoding.UTF8; // encoding used for storing hash values as strings

        /// <summary>
        /// Hash algorithm
        /// Default vaule is HMACMD5
        /// </summary>
        public static KeyedHashAlgorithm HashAlgorithm = new HMACMD5();

        /// <summary>
        /// Constructs an empty Bloom filter. The total length of the Bloom filter will be
        /// bitNumPerElement*expectedNumberOElements.
        /// </summary>
        /// <param name="bitNumPerElement">Is the number of bits used per element.</param>
        /// <param name="expectedNumberOElements">Is the expected number of elements the filter will contain.</param>
        /// <param name="hashFuncNum">Is the number of hash functions used.</param>
        public SixnetBloomFilter(double bitNumPerElement, int expectedNumberOElements, int hashFuncNum)
        {
            expectedNumberOfFilterElements = expectedNumberOElements;
            hashFuncNumber = hashFuncNum;
            bitsPerElement = bitNumPerElement;
            bitSetSize = (int)Math.Ceiling(bitNumPerElement * expectedNumberOElements);
            numberOfAddedElements = 0;
            bitSet = new BitArray(bitSetSize);
        }

        /// <summary>
        /// Constructs an empty Bloom filter. The optimal number of hash functions (k) is estimated from the total size of the Bloom
        /// and the number of expected elements.
        /// </summary>
        /// <param name="bitSetSize">Defines how many bits should be used in total for the filter.</param>
        /// <param name="expectedNumberOElements">Defines the maximum number of elements the filter is expected to contain.</param>
        public SixnetBloomFilter(int bitSetSize, int expectedNumberOElements)
            : this(
                bitSetSize / (double)expectedNumberOElements,
                expectedNumberOElements,
                (int)Math.Round((bitSetSize / (double)expectedNumberOElements) * Math.Log(2.0))
              )
        { }

        /// <summary>
        /// Constructs an empty Bloom filter with a given false positive probability. The number of bits per
        /// element and the number of hash functions is estimated
        /// to match the false positive probability.
        /// </summary>
        /// <param name="falsePositiveProbability">Is the desired false positive probability.</param>
        /// <param name="expectedNumberOfElements">Is the expected number of elements in the Bloom filter.</param>
        public SixnetBloomFilter(double falsePositiveProbability, int expectedNumberOfElements)
            : this(
                Math.Ceiling(-(Math.Log(falsePositiveProbability) / Math.Log(2))) / Math.Log(2), // bitNumPerElement = hashFuncNum / ln(2)
                expectedNumberOfElements,
                (int)Math.Ceiling(-(Math.Log(falsePositiveProbability) / Math.Log(2))) // hashFuncNum = ceil(-log_2(falsePositiveProbability))
              )
        { }

        /// <summary>
        /// Construct a new Bloom filter based on existing Bloom filter data.
        /// </summary>
        /// <param name="bitSetSize">defines how many bits should be used for the filter.</param>
        /// <param name="expectedNumberOfFilterElements">defines the maximum number of elements the filter is expected to contain.</param>
        /// <param name="actualNumberOfFilterElements">specifies how many elements have been inserted into the <code>filterData</code> BitArray.</param>
        /// <param name="filterData">a BitArray representing an existing Bloom filter.</param>
        public SixnetBloomFilter(int bitSetSize, int expectedNumberOfFilterElements, int actualNumberOfFilterElements, BitArray filterData)
            : this(bitSetSize, expectedNumberOfFilterElements)
        {
            bitSet = filterData;
            numberOfAddedElements = actualNumberOfFilterElements;
        }

        /// <summary>
        /// Generates a digest based on the contents of a string.
        /// </summary>
        /// <param name="value">Specifies the input data.</param>
        /// <param name="charset">Specifies the encoding of the input data.</param>
        /// <returns>Return digest as long.</returns>
        public static int CreateHash(string value, Encoding charset)
        {
            return CreateHash(charset.GetBytes(value));
        }

        /// <summary>
        /// Generates a digest based on the contents of a string.
        /// </summary>
        /// <param name="value">Specifies the input data. The encoding is expected to be UTF-8.</param>
        /// <returns>Return digest as long.</returns>
        public static int CreateHash(string value)
        {
            return CreateHash(value, Charset);
        }

        /// <summary>
        /// Generates a digest based on the contents of an array of bytes.
        /// </summary>
        /// <param name="data">Specifies input data.</param>
        /// <returns>Return digest as long.</returns>
        public static int CreateHash(byte[] data)
        {
            return CreateHash(data, 1)[0];
        }

        /// <summary>
        /// Generates digests based on the contents of an array of bytes and splits the result into 4-byte int's and store them in an array. The
        /// digest function is called until the required number of int's are produced. For each call to digest a salt
        /// is prepended to the data. The salt is increased by 1 for each call.
        /// </summary>
        /// <param name="data">Specifies input data</param>
        /// <param name="hashes">Number of hashes/int's to produce</param>
        /// <returns>Return array of int-sized hashes</returns>
        public static int[] CreateHash(byte[] data, int hashes)
        {
            int[] result = new int[hashes];

            int k = 0;
            byte[] salt = new byte[1];
            while (k < hashes)
            {
                byte[] digest;
                lock (HashAlgorithm)
                {
                    HashAlgorithm.Key = salt;
                    salt[0]++;
                    digest = HashAlgorithm.ComputeHash(data);
                }

                for (int i = 0; i < digest.Length / 4 && k < hashes; i++)
                {
                    int h = 0;
                    for (int j = (i * 4); j < (i * 4) + 4; j++)
                    {
                        h <<= 8;
                        h |= digest[j] & 0xFF;
                    }
                    result[k] = h;
                    k++;
                }
            }
            return result;
        }

        /// <summary>
        /// Compares the contents of two instances to see if they are equal.
        /// </summary>
        /// <param name="obj">is the object to compare to.</param>
        /// <returns>True if the contents of the objects are equal.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!GetType().Equals(obj.GetType()))
            {
                return false;
            }
            SixnetBloomFilter other = (SixnetBloomFilter)obj;
            if (expectedNumberOfFilterElements != other.expectedNumberOfFilterElements)
            {
                return false;
            }
            if (hashFuncNumber != other.hashFuncNumber)
            {
                return false;
            }
            if (bitSetSize != other.bitSetSize)
            {
                return false;
            }
            if (bitSet != other.bitSet && (bitSet == null || !bitSet.Compare(other.bitSet)))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Calculates a hash code for this class.
        /// <remarks>performance concerns : note that we read all the bits of bitset to compute the hash</remarks>
        /// <returns>hash code representing the contents of an instance of this class.</returns>
        /// </summary>
        public override int GetHashCode()
        {
            int hash = 7;
            hash = 61 * hash + (bitSet != null ? bitSet.HashValue() : 0);
            hash = 61 * hash + expectedNumberOfFilterElements;
            hash = 61 * hash + bitSetSize;
            hash = 61 * hash + hashFuncNumber;
            return hash;
        }

        /// <summary>
        /// Calculates the expected probability of false positives based on
        /// the number of expected filter elements and the size of the Bloom filter.
        /// <br /><br />
        /// The value returned by this method is the <i>expected</i> rate of false
        /// positives, assuming the number of inserted elements equals the number of
        /// expected elements. If the number of elements in the Bloom filter is less
        /// than the expected value, the true probability of false positives will be lower.
        /// </summary>
        /// <returns>Return expected probability of false positives.</returns>
        public double ExpectedFalsePositiveProbability()
        {
            return GetFalsePositiveProbability(expectedNumberOfFilterElements);
        }

        /// <summary>
        /// Calculate the probability of a false positive given the specified
        /// number of inserted elements.
        /// </summary>
        /// <param name="numberOfElements">number of inserted elements.</param>
        /// <returns>Return probability of a false positive.</returns>
        public double GetFalsePositiveProbability(double numberOfElements)
        {
            // (1 - e^(-hashFuncNumber * numberOfElements / bitSetSize)) ^ hashFuncNumber
            return Math.Pow((1 - Math.Exp(-hashFuncNumber * (double)numberOfElements
                        / bitSetSize)), hashFuncNumber);

        }

        /// <summary>
        /// Get the current probability of a false positive. The probability is calculated from
        /// the size of the Bloom filter and the current number of elements added to it.
        /// </summary>
        /// <returns>Return probability of false positives.</returns>
        public double GetFalsePositiveProbability()
        {
            return GetFalsePositiveProbability(numberOfAddedElements);
        }

        /// <summary>
        /// Returns the value chosen for hashFuncNumber.<br />
        /// <br />
        /// hashFuncNumber is the optimal number of hash functions based on the size
        /// of the Bloom filter and the expected number of inserted elements.
        /// </summary>
        /// <returns>optimal k.</returns>
        public int GetHashFuncNumber()
        {
            return hashFuncNumber;
        }

        /// <summary>
        /// Sets all bits to false in the Bloom filter.
        /// </summary>
        public void Clear()
        {
            bitSet.SetAll(false);
            numberOfAddedElements = 0;
        }

        /// <summary>
        /// Adds an object to the Bloom filter. The output from the object's
        /// ToString() method is used as input to the hash functions.
        /// </summary>
        /// <param name="element">is an element to register in the Bloom filter.</param>
        public void Add(object element)
        {
            Add(Charset.GetBytes(element.ToString()));
        }

        /// <summary>
        /// Adds an array of bytes to the Bloom filter.
        /// </summary>
        /// <param name="bytes">array of bytes to add to the Bloom filter.</param>
        public void Add(byte[] bytes)
        {
            int[] hashes = CreateHash(bytes, hashFuncNumber);
            foreach (int hash in hashes)
            {
                bitSet.Set(Math.Abs(hash % bitSetSize), true);
            }
            numberOfAddedElements++;
        }

        /// <summary>
        /// Adds all elements from a Collection to the Bloom filter.
        /// </summary>
        /// <param name="elements">Collection of elements.</param>
        public void Add(IEnumerable<object> elements)
        {
            foreach (object element in elements)
            {
                Add(element);
            }
        }

        /// <summary>
        /// Adds all elements from a Collection to the Bloom filter.
        /// </summary>
        /// <param name="elements">Collection of elements.</param>
        public void Add(IEnumerable<byte[]> elements)
        {
            foreach (byte[] byteArray in elements)
            {
                Add(byteArray);
            }
        }

        /// <summary>
        /// Returns true if the element could have been inserted into the Bloom filter.
        /// Use getFalsePositiveProbability() to calculate the probability of this
        /// being correct.
        /// </summary>
        /// <param name="element">element to check.</param>
        /// <returns>Return true if the element could have been inserted into the Bloom filter.</returns>
        public bool Contains(object element)
        {
            return Contains(Charset.GetBytes(element.ToString()));
        }

        /// <summary>
        /// Returns true if the array of bytes could have been inserted into the Bloom filter.
        /// Use getFalsePositiveProbability() to calculate the probability of this
        /// being correct.
        /// </summary>
        /// <param name="bytes">array of bytes to check.</param>
        /// <returns>Return true if the array could have been inserted into the Bloom filter.</returns>
        public bool Contains(byte[] bytes)
        {
            int[] hashes = CreateHash(bytes, hashFuncNumber);
            foreach (int hash in hashes)
            {
                if (!bitSet.Get(Math.Abs(hash % bitSetSize)))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns true if all the elements of a Collection could have been inserted
        /// into the Bloom filter. Use getFalsePositiveProbability() to calculate the
        /// probability of this being correct.
        /// </summary>
        /// <param name="elements">Elements to check.</param>
        /// <returns>Return true if all the elements in c could have been inserted into the Bloom filter.</returns>
        public bool ContainsAll(IEnumerable<object> elements)
        {
            foreach (object element in elements)
            {
                if (!Contains(element)) return false;
            }
            return true;
        }

        /// <summary>
        /// Read a single bit from the Bloom filter.
        /// </summary>
        /// <param name="bit">the bit to read.</param>
        /// <returns>Return true if the bit is set, false if it is not.</returns>
        public bool GetBit(int bit)
        {
            return bitSet.Get(bit);
        }

        /// <summary>
        /// Set a single bit in the Bloom filter.
        /// </summary>
        /// <param name="bit">is the bit to set.</param>
        /// <param name="value">If true, the bit is set. If false, the bit is cleared.</param>
        public void SetBit(int bit, bool value)
        {
            bitSet.Set(bit, value);
        }

        /// <summary>
        /// Return the bit set used to store the Bloom filter.
        /// </summary>
        /// <returns>bit set representing the Bloom filter.</returns>
        public BitArray GetBitSet()
        {
            return bitSet;
        }

        /// <summary>
        /// Returns the number of bits in the Bloom filter. Use count() to retrieve
        /// the number of inserted elements.
        /// </summary>
        /// <returns>the size of the bitset used by the Bloom filter.</returns>
        public int Size()
        {
            return bitSetSize;
        }

        /// <summary>
        /// Returns the number of elements added to the Bloom filter after it
        /// was constructed or after clear() was called.
        /// </summary>
        /// <returns>Return number of elements added to the Bloom filter.</returns>
        public int Count()
        {
            return numberOfAddedElements;
        }

        /// <summary>
        /// Returns the expected number of elements to be inserted into the filter.
        /// This value is the same value as the one passed to the constructor.
        /// </summary>
        /// <returns>Return expected number of elements.</returns>
        public int GetExpectedNumberOfElements()
        {
            return expectedNumberOfFilterElements;
        }

        /// <summary>
        /// Get expected number of bits per element when the Bloom filter is full. This value is set by the constructor
        /// when the Bloom filter is created. See also getBitsPerElement().
        /// </summary>
        /// <returns>Return expected number of bits per element.</returns>
        public double GetExpectedBitsPerElement()
        {
            return bitsPerElement;
        }

        /// <summary>
        /// Get actual number of bits per element based on the number of elements that have currently been inserted and the length
        /// of the Bloom filter. See also getExpectedBitsPerElement().
        /// </summary>
        /// <returns>Return number of bits per element.</returns>
        public double GetBitsPerElement()
        {
            return bitSetSize / (double)numberOfAddedElements;
        }
    }
}