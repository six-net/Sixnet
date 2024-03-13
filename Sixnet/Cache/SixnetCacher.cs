using Sixnet.Cache.Hash.Parameters;
using Sixnet.Cache.Hash.Results;
using Sixnet.Cache.Keys.Parameters;
using Sixnet.Cache.Keys.Results;
using Sixnet.Cache.List.Parameters;
using Sixnet.Cache.List.Results;
using Sixnet.Cache.Server.Parameters;
using Sixnet.Cache.Server.Response;
using Sixnet.Cache.Set.Parameters;
using Sixnet.Cache.Set.Results;
using Sixnet.Cache.SortedSet.Parameters;
using Sixnet.Cache.SortedSet.Results;
using Sixnet.Cache.String.Parameters;
using Sixnet.Cache.String.Results;
using Sixnet.DependencyInjection;
using Sixnet.Exceptions;
using Sixnet.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sixnet.Cache
{
    /// <summary>
    /// Cache manager
    /// </summary>
    public static partial class SixnetCacher
    {
        #region Fields

        /// <summary>
        /// Split table cache object name
        /// </summary>
        public const string SplitTableCacheObjectName = "SIXNET_SPLIT_TABLE_CACHE_OBJECT_NAME";
        /// <summary>
        /// Default options
        /// </summary>
        static readonly CacheOptions _defaultOptions = new();

        #endregion

        #region Properties

        /// <summary>
        /// Throw on missing database
        /// </summary>
        public static bool ThrowOnMissingDatabase => Options?.ThrowOnMissingDatabase ?? false;

        /// <summary>
        /// Encoding
        /// </summary>
        public static Encoding Encoding => Options?.Encoding ?? Encoding.UTF8;

        /// <summary>
        /// Get cache options
        /// </summary>
        public static CacheOptions Options => GetCacheOptions();

        #endregion

        #region Data

        #region Set data

        /// <summary>
        /// Store data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="data">Data object</param>
        /// <param name="absoluteExpiration">Absolute expiration time</param>
        /// <param name="when">Cache setting conditions</param>
        /// <param name="cacheObject">The data belongs to the cache object</param>
        /// <returns>Return cache set result</returns>
        public static StringSetResult Store<T>(CacheKey key, T data, DateTimeOffset? absoluteExpiration = null, CacheSetWhen when = CacheSetWhen.Always, CacheObject cacheObject = null)
        {
            var value = SixnetJsonSerializer.Serialize(data);
            if (string.IsNullOrWhiteSpace(value))
            {
                return CacheResult.FailResponse<StringSetResult>(SixnetCacheCodes.ValuesIsNullOrEmpty);
            }
            return String.Set(key, value, absoluteExpiration, when, cacheObject);
        }

        /// <summary>
        /// Store data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="data">Data object</param>
        /// <param name="absoluteExpirationRelativeToNow">The expiration time relative to the current time</param>
        /// <param name="slidingExpiration">Whether to enable sliding expiration</param>
        /// <param name="when">Cache setting conditions</param>
        /// <param name="cacheObject">The data belongs to the cache object</param>
        /// <returns>Return cache set result</returns>
        public static StringSetResult Store<T>(CacheKey key, T data, TimeSpan? absoluteExpirationRelativeToNow = null, bool slidingExpiration = true, CacheSetWhen when = CacheSetWhen.Always, CacheObject cacheObject = null)
        {
            var value = SixnetJsonSerializer.Serialize(data);
            if (string.IsNullOrWhiteSpace(value))
            {
                return CacheResult.FailResponse<StringSetResult>(SixnetCacheCodes.ValuesIsNullOrEmpty);
            }
            return String.Set(key, value, absoluteExpirationRelativeToNow, slidingExpiration, when, cacheObject);
        }

        #endregion

        #region Get data

        /// <summary>
        /// Get stored data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="cacheObject">Cache object</param>
        /// <returns>Return data object</returns>
        public static T Get<T>(CacheKey key, CacheObject cacheObject = null)
        {
            return String.Get<T>(key, cacheObject);
        }

        #endregion

        #region Get data list

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="cacheKeys">Cache keys</param>
        /// <param name="cacheObject">Cache object</param>
        /// <returns>Return data list</returns>
        public static List<T> GetList<T>(IEnumerable<CacheKey> cacheKeys, CacheObject cacheObject = null)
        {
            return String.Get<T>(cacheKeys, cacheObject);
        }

        #endregion

        #endregion

        #region String

        /// <summary>
        /// String data type operation
        /// </summary>
        public static partial class String
        {
            #region Set range

            /// <summary>
            /// Starting with offset, overwrite the string value stored by the overwrite key with the value parameter,
            /// Make sure the string is long enough to set the value to the specified offset, 
            /// If the original string length stored by the key is smaller than the offset (say, the string is only 5 characters long, but you set the offset to 10),  
            /// the space between the original character and the offset will be filled with zerobytes (zerobytes, "\x00")
            /// </summary>
            /// <param name="stringSetRangeParameter">Set range parameter</param>
            /// <returns>Return cache set result</returns>
            public static StringSetRangeResult SetRange(StringSetRangeParameter stringSetRangeParameter)
            {
                return ExecuteCacheOperation(stringSetRangeParameter);
            }

            #endregion

            #region Set bit

            /// <summary>
            /// To set or clear the bit (bit) at the specified offset for the string value stored by the key,
            /// When the key does not exist, a new string value is automatically generated.
            /// The string is stretched (grown) to ensure that it can hold the value at the specified offset. When the string value is stretched, the empty space is filled with 0
            /// The offset parameter must be greater than or equal to 0 and less than 2^32
            /// </summary>
            /// <param name="stringSetBitParameter">Set bit parameter</param>
            /// <returns>Return cache set result</returns>
            public static StringSetBitResult SetBit(StringSetBitParameter stringSetBitParameter)
            {
                return ExecuteCacheOperation(stringSetBitParameter);
            }

            #endregion

            #region Set

            /// <summary>
            /// Associate the string value value to the key
            /// If the key already holds other values, SET will overwrite the old values, regardless of the type
            /// When the SET command sets a key with a time to live (TTL), the original TTL of the key will be cleared
            /// </summary>
            /// <param name="stringSetParameter">String set parameter</param>
            /// <returns>Return cache result</returns>
            public static StringSetResult Set(StringSetParameter stringSetParameter)
            {
                return ExecuteCacheOperation(stringSetParameter);
            }

            /// <summary>
            /// Associate the string value value to the key
            /// If the key already holds other values, overwrite the old values
            /// When the SET command sets a key with a time to live (TTL), the original TTL of the key will be cleared
            /// </summary>
            /// <param name="key">Cache key</param>
            /// <param name="value">String value</param>
            /// <param name="absoluteExpiration">Absolute expiration time</param>
            /// <param name="when">Set value conditions</param>
            /// <param name="cacheObject">Cache object</param>
            /// <returns>Return cache result</returns>
            public static StringSetResult Set(CacheKey key, string value, DateTimeOffset? absoluteExpiration = null, CacheSetWhen when = CacheSetWhen.Always, CacheObject cacheObject = null)
            {
                return Set(new StringSetParameter()
                {
                    CacheObject = cacheObject,
                    Items = new List<CacheEntry>()
                    {
                        new CacheEntry ()
                        {
                            Key=key,
                            Value=value,
                            When=when,
                            Expiration = new CacheExpiration ()
                            {
                                AbsoluteExpiration=absoluteExpiration,
                                SlidingExpiration=false
                            }
                        }
                    }
                });
            }

            /// <summary>
            /// Associate the string value value to the key
            /// If the key already holds other values, overwrite the old values
            /// When the SET command sets a key with a time to live (TTL), the original TTL of the key will be cleared
            /// </summary>
            /// <param name="key">Cache key</param>
            /// <param name="value">String value</param>
            /// <param name="absoluteExpirationRelativeToNow">The expiration time relative to the current time</param>
            /// <param name="slidingExpiration">Whether to enable sliding expiration (expiration time will be recalculated for each pair of cache access)</param>
            /// <param name="when">Set value conditions</param>
            /// <param name="cacheObject">Cache object</param>
            /// <returns>Return cache result</returns>
            public static StringSetResult Set(CacheKey key, string value, TimeSpan? absoluteExpirationRelativeToNow = null, bool slidingExpiration = true, CacheSetWhen when = CacheSetWhen.Always, CacheObject cacheObject = null)
            {
                return Set(new StringSetParameter()
                {
                    CacheObject = cacheObject,
                    Items = new List<CacheEntry>()
                    {
                        new CacheEntry ()
                        {
                            Key=key,
                            Value=value,
                            When=when,
                            Expiration = new CacheExpiration ()
                            {
                                AbsoluteExpirationRelativeToNow=absoluteExpirationRelativeToNow,
                                SlidingExpiration=slidingExpiration
                            }
                        }
                    }
                });
            }

            #endregion

            #region Length

            /// <summary>
            /// Returns the length of the string value stored by key
            /// </summary>
            /// <param name="stringLengthParameter">String length parameter</param>
            /// <returns>Return cache result</returns>
            public static StringLengthResult Length(StringLengthParameter stringLengthParameter)
            {
                return ExecuteCacheOperation(stringLengthParameter);
            }

            #endregion

            #region Increment

            /// <summary>
            /// Add one to the numeric value stored by the key
            /// If the key key does not exist, then its value will be initialized to 0 first, and then execute the command
            /// If the value stored by the key cannot be interpreted as a number, the command will return an error
            /// </summary>
            /// <param name="stringIncrementParameter">String increment parameter</param>
            /// <returns>Return cache result</returns>
            public static StringIncrementResult Increment(StringIncrementParameter stringIncrementParameter)
            {
                return ExecuteCacheOperation(stringIncrementParameter);
            }

            #endregion

            #region Get with expiry

            /// <summary>
            /// Returns the string value associated with the key
            /// If the key key does not exist, then return an empty string, otherwise, return the value of the key key
            /// If the value of key is not of type string, then return an error.
            /// </summary>
            /// <param name="stringGetWithExpiryParameter">String get with expiry parameter</param>
            /// <returns>Return cache result</returns>
            public static StringGetWithExpiryResult GetWithExpiry(StringGetWithExpiryParameter stringGetWithExpiryParameter)
            {
                return ExecuteCacheOperation(stringGetWithExpiryParameter);
            }

            #endregion

            #region Get set

            /// <summary>
            /// Set the value of the key key to value and return the old value of the key key before it is set
            /// When the key key exists but is not a string type, the command returns an error
            /// </summary>
            /// <param name="stringGetSetParameter">String get set parameter</param>
            /// <returns>Return cache result</returns>
            public static StringGetSetResult GetSet(StringGetSetParameter stringGetSetParameter)
            {
                return ExecuteCacheOperation(stringGetSetParameter);
            }

            #endregion

            #region Get range

            /// <summary>
            /// Returns the specified part of the string value stored by the key key, the range of the string interception is determined by the two offsets of start and end (including start and end)
            /// Negative offset means counting from the end of the string, -1 means the last character, -2 means the penultimate character, and so on
            /// </summary>
            /// <param name="stringGetRangeParameter">String get range parameter</param>
            /// <returns>Return cache result</returns>
            public static StringGetRangeResult GetRange(StringGetRangeParameter stringGetRangeParameter)
            {
                return ExecuteCacheOperation(stringGetRangeParameter);
            }

            #endregion

            #region Get bit

            /// <summary>
            /// For the string value stored in key, get the bit at the specified offset
            /// When offset is greater than the length of the string value, or the key does not exist, return 0
            /// </summary>
            /// <param name="stringGetBitParameter">String get bit parameter</param>
            /// <returns>Return cache result</returns>
            public static StringGetBitResult GetBit(StringGetBitParameter stringGetBitParameter)
            {
                return ExecuteCacheOperation(stringGetBitParameter);
            }

            #endregion

            #region Get

            /// <summary>
            /// Returns the value of the given string key or keys
            /// </summary>
            /// <param name="stringGetParameter">String get parameter</param>
            /// <returns>Return cache result</returns>
            public static StringGetResult Get(StringGetParameter stringGetParameter)
            {
                return ExecuteCacheOperation(stringGetParameter);
            }

            /// <summary>
            /// Returns the string value associated with the key
            /// If the key key does not exist, then return an empty string, otherwise, return the value of the key key
            /// If the value of key is not of type string, then return an error.
            /// </summary>
            /// <param name="key">Cache key</param>
            /// <param name="cacheObject">Cache object</param>
            /// <returns>Return cache result</returns>
            public static string Get(CacheKey key, CacheObject cacheObject = null)
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    return string.Empty;
                }
                var values = Get(new List<CacheKey>() { key }, cacheObject);
                return values?.FirstOrDefault() ?? string.Empty;
            }

            /// <summary>
            /// Return the value associated with the key and convert it to the specified data object
            /// </summary>
            /// <param name="key">Cache key</param>
            /// <param name="cacheObject">Cache object information</param>
            /// <returns>Return cache result</returns>
            public static T Get<T>(CacheKey key, CacheObject cacheObject = null)
            {
                var cacheValue = Get(key, cacheObject);
                if (string.IsNullOrWhiteSpace(cacheValue))
                {
                    return default;
                }
                return SixnetJsonSerializer.Deserialize<T>(cacheValue);
            }

            /// <summary>
            /// Returns the value of the given string key or keys
            /// </summary>
            /// <param name="keys">Cache keys​​</param>
            /// <param name="cacheObject">Cache object</param>
            /// <returns>Return values</returns>
            public static List<string> Get(IEnumerable<CacheKey> keys, CacheObject cacheObject = null)
            {
                if (keys.IsNullOrEmpty())
                {
                    return new List<string>(0);
                }
                var result = Get(new StringGetParameter()
                {
                    CacheObject = cacheObject,
                    Keys = keys.ToList()
                });
                return result?.Values?.Select(c => c.Value?.ToString()).ToList();
            }

            /// <summary>
            /// Return the value of the given one or more string keys and convert to the specified data type
            /// </summary>
            /// <typeparam name="T">Data type</typeparam>
            /// <param name="keys">Cache key</param>
            /// <param name="cacheObject">Cache object</param>
            /// <returns>Return datas</returns>
            public static List<T> Get<T>(IEnumerable<CacheKey> keys, CacheObject cacheObject = null)
            {
                var values = Get(keys, cacheObject);
                if (values.IsNullOrEmpty())
                {
                    return new List<T>(0);
                }
                var datas = new List<T>(values.Count);
                foreach (var val in values)
                {
                    var data = SixnetJsonSerializer.Deserialize<T>(val);
                    datas.Add(data);
                }
                return datas;
            }

            #endregion

            #region Decrement

            /// <summary>
            /// Minus one for the numeric value stored for the key
            /// If the key key does not exist, the value of the key key will be initialized to 0 first, and then perform the operation
            /// If the value stored by the key cannot be interpreted as a number, an error will be returned
            /// </summary>
            /// <param name="stringDecrementParameter">String decrement parameter</param>
            /// <returns>Return cache result</returns>
            public static StringDecrementResult Decrement(StringDecrementParameter stringDecrementParameter)
            {
                return ExecuteCacheOperation(stringDecrementParameter);
            }

            #endregion

            #region Bit position

            /// <summary>
            /// Returns the position of the first binary bit in the bitmap
            /// By default, the command will detect the entire bitmap, but the user can also specify the range to be detected through the optional start parameter and end parameter
            /// </summary>
            /// <param name="stringBitPositionParameter">String bit position parameter</param>
            /// <returns>Return cache result</returns>
            public static StringBitPositionResult BitPosition(StringBitPositionParameter stringBitPositionParameter)
            {
                return ExecuteCacheOperation(stringBitPositionParameter);
            }

            #endregion

            #region Bit operation

            /// <summary>
            /// Perform bit operations on one or more string keys that hold binary bits, and save the result to destkey
            /// Except NOT operation, other operations can accept one or more keys as input
            /// </summary>
            /// <param name="stringBitOperationParameter">String bit operation parameter</param>
            /// <returns>Return cache result</returns>
            public static StringBitOperationResult BitOperation(StringBitOperationParameter stringBitOperationParameter)
            {
                return ExecuteCacheOperation(stringBitOperationParameter);
            }

            #endregion

            #region Bit count

            /// <summary>
            /// Calculate the number of bits set to 1 in a given string.
            /// Under normal circumstances, the given entire string will be counted, by specifying additional start or end parameters, you can make the count only on a specific bit
            /// </summary>
            /// <param name="stringBitCountParameter">String bit count parameter</param>
            /// <returns>Return cache result</returns>
            public static StringBitCountResult BitCount(StringBitCountParameter stringBitCountParameter)
            {
                return ExecuteCacheOperation(stringBitCountParameter);
            }

            #endregion

            #region Append

            /// <summary>
            /// If the key key already exists and its value is a string, the value will be appended to the end of the key key's existing value
            /// If the key does not exist, simply set the value of the key key to value
            /// </summary>
            /// <param name="stringAppendParameter">String append parameter</param>
            /// <returns>Return cache result</returns>
            public static StringAppendResult Append(StringAppendParameter stringAppendParameter)
            {
                return ExecuteCacheOperation(stringAppendParameter);
            }

            #endregion
        }

        #endregion

        #region List

        /// <summary>
        /// List data type operation
        /// </summary>
        public static partial class List
        {
            #region Trim

            /// <summary>
            /// Trim a list, that is, let the list keep only the elements in the specified interval, and elements that are not in the specified interval will be deleted
            /// The subscript (index) parameters start and stop are both based on 0, that is, 0 represents the first element of the list, 1 represents the second element of the list, and so on
            /// You can also use negative subscripts, with -1 for the last element of the list, -2 for the penultimate element of the list, and so on
            /// When key is not a list type, an error is returned
            /// </summary>
            /// <param name="listTrimParameter">List trim parameter</param>
            /// <returns>Return cache result</returns>
            public static ListTrimResult Trim(ListTrimParameter listTrimParameter)
            {
                return ExecuteCacheOperation(listTrimParameter);
            }

            #endregion

            #region Set by index

            /// <summary>
            /// Set the value of the element whose index of the list key is index to value
            /// When the index parameter is out of range, or an operation is performed on an empty list (the key does not exist), an error is returned
            /// </summary>
            /// <param name="listSetByIndexParameter">List set by index parameter</param>
            /// <returns>Return cache result</returns>
            public static ListSetByIndexResult SetByIndex(ListSetByIndexParameter listSetByIndexParameter)
            {
                return ExecuteCacheOperation(listSetByIndexParameter);
            }

            #endregion

            #region Right push

            /// <summary>
            /// Insert one or more values ​​into the end of the list key (far right).
            /// If there are multiple value values, then each value value is inserted into the end of the table in order from left to right
            /// </summary>
            /// <param name="listRightPushParameter">List right push parameter</param>
            /// <returns>Return cache result</returns>
            public static ListRightPushResult RightPush(ListRightPushParameter listRightPushParameter)
            {
                return ExecuteCacheOperation(listRightPushParameter);
            }

            #endregion

            #region Right pop left push

            /// <summary>
            /// Pop the last element (tail element) in the list source and return it to the client
            /// Insert the element popped by source into the destination list as the head element of the destination list
            /// </summary>
            /// <param name="listRightPopLeftPushParameter">List right pop left push parameter</param>
            /// <returns>Return cache result</returns>
            public static ListRightPopLeftPushResult RightPopLeftPush(ListRightPopLeftPushParameter listRightPopLeftPushParameter)
            {
                return ExecuteCacheOperation(listRightPopLeftPushParameter);
            }

            #endregion

            #region Right pop

            /// <summary>
            /// Remove and return the tail element of the list key.
            /// </summary>
            /// <param name="listRightPopParameter">List right pop parameter</param>
            /// <returns>Return cache result</returns>
            public static ListRightPopResult RightPop(ListRightPopParameter listRightPopParameter)
            {
                return ExecuteCacheOperation(listRightPopParameter);
            }

            #endregion

            #region Remove

            /// <summary>
            /// According to the value of the parameter count, remove the elements equal to the parameter value in the list
            /// count is greater than 0: search from the beginning of the table to the end of the table, remove the elements equal to value, the number is count
            /// count is less than 0: search from the end of the table to the head of the table, remove the elements equal to value, the number is the absolute value of count
            /// count equals 0: remove all values ​​in the table that are equal to value
            /// </summary>
            /// <param name="listRemoveParameter">List remove parameter</param>
            /// <returns>Return cache result</returns>
            public static ListRemoveResult Remove(ListRemoveParameter listRemoveParameter)
            {
                return ExecuteCacheOperation(listRemoveParameter);
            }

            #endregion

            #region Range

            /// <summary>
            /// Return the elements in the specified interval in the list key, the interval is specified by the offset start and stop
            /// The subscript (index) parameters start and stop are both based on 0, that is, 0 represents the first element of the list, 1 represents the second element of the list, and so on
            /// You can also use negative subscripts, with -1 for the last element of the list, -2 for the penultimate element of the list, and so on
            /// </summary>
            /// <param name="listRangeParameter">List range parameter</param>
            /// <returns>Return cache result</returns>
            public static ListRangeResult Range(ListRangeParameter listRangeParameter)
            {
                return ExecuteCacheOperation(listRangeParameter);
            }

            #endregion

            #region Length

            /// <summary>
            /// Return the length of the list key
            /// If the key does not exist, the key is interpreted as an empty list and returns 0 
            /// If key is not a list type, return an error.
            /// </summary>
            /// <param name="listLengthParameter">List length parameter</param>
            /// <returns>Return cache result</returns>
            public static ListLengthResult Length(ListLengthParameter listLengthParameter)
            {
                return ExecuteCacheOperation(listLengthParameter);
            }

            #endregion

            #region Left push

            /// <summary>
            /// Insert one or more values ​​into the header of the list key
            /// If the key does not exist, an empty list will be created and the operation will be performed
            /// </summary>
            /// <param name="listLeftPushParameter">List left push parameter</param>
            /// <returns>Return cache result</returns>
            public static ListLeftPushResult LeftPush(ListLeftPushParameter listLeftPushParameter)
            {
                return ExecuteCacheOperation(listLeftPushParameter);
            }

            #endregion

            #region Left pop

            /// <summary>
            /// Remove and return the head element of the list key
            /// </summary>
            /// <param name="listLeftPopParameter">List left pop parameter</param>
            /// <returns>Return cache result</returns>
            public static ListLeftPopResult LeftPop(ListLeftPopParameter listLeftPopParameter)
            {
                return ExecuteCacheOperation(listLeftPopParameter);
            }

            #endregion

            #region Insert before

            /// <summary>
            /// Insert the value value into the list key, before the value pivot
            /// </summary>
            /// <param name="listInsertBeforeParameter">List insert before parameter</param>
            /// <returns>Return cache result</returns>
            public static ListInsertBeforeResult InsertBefore(ListInsertBeforeParameter listInsertBeforeParameter)
            {
                return ExecuteCacheOperation(listInsertBeforeParameter);
            }

            #endregion

            #region Insert after

            /// <summary>
            /// Insert the value value into the list key, after the value pivot
            /// </summary>
            /// <param name="listInsertAfterParameter">List insert after parameter</param>
            /// <returns>Return cache result</returns>
            public static ListInsertAfterResult InsertAfter(ListInsertAfterParameter listInsertAfterParameter)
            {
                return ExecuteCacheOperation(listInsertAfterParameter);
            }

            #endregion

            #region Get by index

            /// <summary>
            /// Return the element with index index in the list key
            /// The subscript (index) parameters start and stop are both based on 0, that is, 0 represents the first element of the list, 1 represents the second element of the list, and so on
            /// You can also use negative subscripts, with -1 for the last element of the list, -2 for the penultimate element of the list, and so on
            /// </summary>
            /// <param name="listGetByIndexParameter">List get by index parameter</param>
            /// <returns>Return cache result</returns>
            public static ListGetByIndexResult GetByIndex(ListGetByIndexParameter listGetByIndexParameter)
            {
                return ExecuteCacheOperation(listGetByIndexParameter);
            }

            #endregion
        }

        #endregion

        #region Hash

        /// <summary>
        /// Hash data type operation
        /// </summary>
        public static partial class Hash
        {
            #region Values

            /// <summary>
            /// Return the values ​​of all fields in the hash table key
            /// </summary>
            /// <param name="hashValuesParameter">Hash values parameter</param>
            /// <returns>Return cache result</returns>
            public static HashValuesResult Values(HashValuesParameter hashValuesParameter)
            {
                return ExecuteCacheOperation(hashValuesParameter);
            }

            #endregion

            #region Set

            /// <summary>
            /// Set the value of the field field in the hash table hash to value
            /// If the given hash table does not exist, then a new hash table will be created and perform the operation
            /// If the field field already exists in the hash table, its old value will be overwritten by the new value
            /// </summary>
            /// <param name="hashSetParameter">Hash set parameter</param>
            /// <returns>Return cache result</returns>
            public static HashSetResult Set(HashSetParameter hashSetParameter)
            {
                return ExecuteCacheOperation(hashSetParameter);
            }

            #endregion

            #region Length

            /// <summary>
            /// returns the number of fields in the hash table key
            /// </summary>
            /// <param name="hashLengthParameter">Hash length parameter</param>
            /// <returns>Return cache result</returns>
            public static HashLengthResult Length(HashLengthParameter hashLengthParameter)
            {
                return ExecuteCacheOperation(hashLengthParameter);
            }

            #endregion

            #region Keys

            /// <summary>
            /// Return all keys in the hash table key
            /// </summary>
            /// <param name="hashKeysParameter">Hash keys parameter</param>
            /// <returns>Return cache result</returns>
            public static HashKeysResult Keys(HashKeysParameter hashKeysParameter)
            {
                return ExecuteCacheOperation(hashKeysParameter);
            }

            #endregion

            #region Increment

            /// <summary>
            /// Add incremental increment to the value of the field field in the hash table key
            /// </summary>
            /// <param name="hashIncrementParameter">Hash increment parameter</param>
            /// <returns>Return cache result</returns>
            public static HashIncrementResult Increment(HashIncrementParameter hashIncrementParameter)
            {
                return ExecuteCacheOperation(hashIncrementParameter);
            }

            #endregion

            #region Get

            /// <summary>
            /// Returns the value of the given field in the hash table
            /// </summary>
            /// <param name="hashGetParameter">Hash get parameter</param>
            /// <returns>Return cache result</returns>
            public static HashGetResult Get(HashGetParameter hashGetParameter)
            {
                return ExecuteCacheOperation(hashGetParameter);
            }

            #endregion

            #region Get all

            /// <summary>
            /// Return the values ​​of all fields in the hash table key
            /// </summary>
            /// <param name="hashGetAllParameter">Hash get all parameter</param>
            /// <returns>Return cache result</returns>
            public static HashGetAllResult GetAll(HashGetAllParameter hashGetAllParameter)
            {
                return ExecuteCacheOperation(hashGetAllParameter);
            }

            #endregion

            #region Exist

            /// <summary>
            /// Check if the given field exists in the hash of the hash table
            /// </summary>
            /// <param name="hashExistsParameter">Hash exists parameter</param>
            /// <returns>Return cache result</returns>
            public static HashExistsResult Exist(HashExistsParameter hashExistsParameter)
            {
                return ExecuteCacheOperation(hashExistsParameter);
            }

            #endregion

            #region Delete

            /// <summary>
            /// Delete one or more specified fields in the hash table key, the non-existing fields will be ignored
            /// </summary>
            /// <param name="hashDeleteParameter">Hash delete parameter</param>
            /// <returns>Return cache result</returns>
            public static HashDeleteResult Delete(HashDeleteParameter hashDeleteParameter)
            {
                return ExecuteCacheOperation(hashDeleteParameter);
            }

            #endregion

            #region Decrement

            /// <summary>
            /// Is the value of the field in the hash table key minus the increment increment
            /// </summary>
            /// <param name="hashDecrementParameter">Hash decrement parameter</param>
            /// <returns>Return cache result</returns>
            public static HashDecrementResult Decrement(HashDecrementParameter hashDecrementParameter)
            {
                return ExecuteCacheOperation(hashDecrementParameter);
            }

            #endregion

            #region Scan

            /// <summary>
            /// Each element returned is a key-value pair, a key-value pair consists of a key and a value
            /// </summary>
            /// <param name="hashScanParameter">Hash scan parameter</param>
            /// <returns>Return cache result</returns>
            public static HashScanResult Scan(HashScanParameter hashScanParameter)
            {
                return ExecuteCacheOperation(hashScanParameter);
            }

            #endregion
        }

        #endregion

        #region Set

        /// <summary>
        /// Set data type operation
        /// </summary>
        public static partial class Set
        {
            #region Remove

            /// <summary>
            /// Remove one or more member elements in the collection key, non-existent member elements will be ignored
            /// </summary>
            /// <param name="setRemoveParameter">Set remove parameter</param>
            /// <returns>Return cache result</returns>
            public static SetRemoveResult Remove(SetRemoveParameter setRemoveParameter)
            {
                return ExecuteCacheOperation(setRemoveParameter);
            }

            #endregion

            #region Random members

            /// <summary>
            /// Then return a set of random elements in the collection
            /// </summary>
            /// <param name="setRandomMembersParameter">Set random members parameter</param>
            /// <returns>Return cache result</returns>
            public static SetRandomMembersResult RandomMembers(SetRandomMembersParameter setRandomMembersParameter)
            {
                return ExecuteCacheOperation(setRandomMembersParameter);
            }

            #endregion

            #region Random member

            /// <summary>
            /// Returns a random element in the collection
            /// </summary>
            /// <param name="setRandomMemberParameter">Set random member parameter</param>
            /// <returns>Return cache result</returns>
            public static SetRandomMemberResult RandomMember(SetRandomMemberParameter setRandomMemberParameter)
            {
                return ExecuteCacheOperation(setRandomMemberParameter);
            }

            #endregion

            #region Pop

            /// <summary>
            /// Remove and return a random element in the collection
            /// </summary>
            /// <param name="setPopParameter">Set pop parameter</param>
            /// <returns>Return cache result</returns>
            public static SetPopResult Pop(SetPopParameter setPopParameter)
            {
                return ExecuteCacheOperation(setPopParameter);
            }

            #endregion

            #region Move

            /// <summary>
            /// Move the member element from the source collection to the destination collection
            /// </summary>
            /// <param name="setMoveParameter">Set move parameter</param>
            /// <returns>Return cache result</returns>
            public static SetMoveResult Move(SetMoveParameter setMoveParameter)
            {
                return ExecuteCacheOperation(setMoveParameter);
            }

            #endregion

            #region Members

            /// <summary>
            /// Return all members in the collection key
            /// </summary>
            /// <param name="setMembersParameter">Set members parameter</param>
            /// <returns>Return cache result</returns>
            public static SetMembersResult Members(SetMembersParameter setMembersParameter)
            {
                return ExecuteCacheOperation(setMembersParameter);
            }

            #endregion

            #region Length

            /// <summary>
            /// Returns the number of elements in the collection
            /// </summary>
            /// <param name="setLengthParameter">Set length parameter</param>
            /// <returns>Return cache result</returns>
            public static SetLengthResult Length(SetLengthParameter setLengthParameter)
            {
                return ExecuteCacheOperation(setLengthParameter);
            }

            #endregion

            #region Contains

            /// <summary>
            /// Determine whether the member element is a member of the set key
            /// </summary>
            /// <param name="setContainsParameter">Set contaims parameter</param>
            /// <returns>Return cache result</returns>
            public static SetContainsResult Contains(SetContainsParameter setContainsParameter)
            {
                return ExecuteCacheOperation(setContainsParameter);
            }

            #endregion

            #region Combine

            /// <summary>
            /// According to the operation mode, return the processing results of multiple collections
            /// </summary>
            /// <param name="setCombineParameter">Set combine parameter</param>
            /// <returns>Return cache result</returns>
            public static SetCombineResult Combine(SetCombineParameter setCombineParameter)
            {
                return ExecuteCacheOperation(setCombineParameter);
            }

            #endregion

            #region Combine and store

            /// <summary>
            /// Return the processing results of multiple collections according to the operation mode, and store the results to the specified key value at the same time
            /// </summary>
            /// <param name="setCombineAndStoreParameter">Set combine and store parameter</param>
            /// <returns>Return cache result</returns>
            public static SetCombineAndStoreResult CombineAndStore(SetCombineAndStoreParameter setCombineAndStoreParameter)
            {
                return ExecuteCacheOperation(setCombineAndStoreParameter);
            }

            #endregion

            #region Add

            /// <summary>
            /// Add one or more member elements to the collection key, the member elements already in the collection will be ignored
            /// If the key does not exist, create a collection that contains only the member element as a member
            /// </summary>
            /// <param name="setAddParameter">Set add parameter</param>
            /// <returns>Return cache result</returns>
            public static SetAddResult Add(SetAddParameter setAddParameter)
            {
                return ExecuteCacheOperation(setAddParameter);
            }

            #endregion
        }

        #endregion

        #region Sorted set

        /// <summary>
        /// Sorted set data type operation
        /// </summary>
        public static partial class SortedSet
        {
            #region Score

            /// <summary>
            /// Returns the score value of the member member in the ordered set key
            /// </summary>
            /// <param name="sortedSetScoreParameter">Sorted set score parameter</param>
            /// <returns>Return cache result</returns>
            public static SortedSetScoreResult Score(SortedSetScoreParameter sortedSetScoreParameter)
            {
                return ExecuteCacheOperation(sortedSetScoreParameter);
            }

            #endregion

            #region Remove range by value

            /// <summary>
            /// Remove the elements in the specified range after sorting the elements
            /// </summary>
            /// <param name="sortedSetRemoveRangeByValueParameter">Sorted set remove range by value parameter</param>
            /// <returns>Return cache result</returns>
            public static SortedSetRemoveRangeByValueResult RemoveRangeByValue(SortedSetRemoveRangeByValueParameter sortedSetRemoveRangeByValueParameter)
            {
                return ExecuteCacheOperation(sortedSetRemoveRangeByValueParameter);
            }

            #endregion

            #region Remove range by score

            /// <summary>
            /// Remove all members in the ordered set key whose score value is between min and max
            /// </summary>
            /// <param name="sortedSetRemoveRangeByScoreParameter">Sorted set remove range by score parameter</param>
            /// <returns>Return cache result</returns>
            public static SortedSetRemoveRangeByScoreResult RemoveRangeByScore(SortedSetRemoveRangeByScoreParameter sortedSetRemoveRangeByScoreParameter)
            {
                return ExecuteCacheOperation(sortedSetRemoveRangeByScoreParameter);
            }

            #endregion

            #region Remove range by rank

            /// <summary>
            /// Remove all members in the specified rank range in the ordered set key
            /// The subscript parameters start and stop are both based on 0, that is, 0 means the first member of the ordered set, 1 means the second member of the ordered set, and so on. 
            /// You can also use negative subscripts, with -1 for the last member, -2 for the penultimate member, and so on.
            /// </summary>
            /// <param name="sortedSetRemoveRangeByRankParameter">Sorted set range by rank parameter</param>
            /// <returns>Return cache result</returns>
            public static SortedSetRemoveRangeByRankResult RemoveRangeByRank(SortedSetRemoveRangeByRankParameter sortedSetRemoveRangeByRankParameter)
            {
                return ExecuteCacheOperation(sortedSetRemoveRangeByRankParameter);
            }

            #endregion

            #region Remove

            /// <summary>
            /// Remove the specified element in the ordered collection
            /// </summary>
            /// <param name="sortedSetRemoveParameter">Sorted set remove parameter</param>
            /// <returns>Return cache result</returns>
            public static SortedSetRemoveResult Remove(SortedSetRemoveParameter sortedSetRemoveParameter)
            {
                return ExecuteCacheOperation(sortedSetRemoveParameter);
            }

            #endregion

            #region Rank

            /// <summary>
            /// Returns the ranking of the member member in the ordered set key. The members of the ordered set are arranged in order of increasing score value (from small to large) by default
            /// The ranking is based on 0, that is, the member with the smallest score is ranked 0
            /// </summary>
            /// <param name="sortedSetRankParameter">Sorted set rank parameter</param>
            /// <returns>Return cache result</returns>
            public static SortedSetRankResult Rank(SortedSetRankParameter sortedSetRankParameter)
            {
                return ExecuteCacheOperation(sortedSetRankParameter);
            }

            #endregion

            #region Range by value

            /// <summary>
            /// Returns the elements in the ordered set between min and max
            /// </summary>
            /// <param name="sortedSetRangeByValueParameter">Sorted set range by value parameter</param>
            /// <returns>Return cache result</returns>
            public static SortedSetRangeByValueResult RangeByValue(SortedSetRangeByValueParameter sortedSetRangeByValueParameter)
            {
                return ExecuteCacheOperation(sortedSetRangeByValueParameter);
            }

            #endregion

            #region Range by score with scores

            /// <summary>
            /// Returns the value and score of the members in the specified interval in the ordered set key, the positions are arranged according to score
            /// </summary>
            /// <param name="sortedSetRangeByScoreWithScoresParameter">Sorted set range by score with scores parameter</param>
            /// <returns>Return cache result</returns>
            public static SortedSetRangeByScoreWithScoresResult RangeByScoreWithScores(SortedSetRangeByScoreWithScoresParameter sortedSetRangeByScoreWithScoresParameter)
            {
                return ExecuteCacheOperation(sortedSetRangeByScoreWithScoresParameter);
            }

            #endregion

            #region Range by score

            /// <summary>
            /// Return the value of the members in the specified interval in the ordered set key, the position is arranged by score
            /// </summary>
            /// <param name="sortedSetRangeByScoreParameter">Sorted set range by score parameter</param>
            /// <returns>Return cache result</returns>
            public static SortedSetRangeByScoreResult RangeByScore(SortedSetRangeByScoreParameter sortedSetRangeByScoreParameter)
            {
                return ExecuteCacheOperation(sortedSetRangeByScoreParameter);
            }

            #endregion

            #region Range by rank with scores

            /// <summary>
            /// Returns the value and score of the members in the specified interval in the ordered set key, the positions are arranged according to score
            /// </summary>
            /// <param name="sortedSetRangeByRankWithScoresParameter">Sorted set range by rank with scores parameter</param>
            /// <returns>Return cache result</returns>
            public static SortedSetRangeByRankWithScoresResult RangeByRankWithScores(SortedSetRangeByRankWithScoresParameter sortedSetRangeByRankWithScoresParameter)
            {
                return ExecuteCacheOperation(sortedSetRangeByRankWithScoresParameter);
            }

            #endregion

            #region Range by rank

            /// <summary>
            /// Return the value of the members in the specified interval in the ordered set key, the positions are arranged by score
            /// </summary>
            /// <param name="sortedSetRangeByRankParameter">Sorted set range by rank parameter</param>
            /// <returns>sorted set range by rank response</returns>
            public static SortedSetRangeByRankResult RangeByRank(SortedSetRangeByRankParameter sortedSetRangeByRankParameter)
            {
                return ExecuteCacheOperation(sortedSetRangeByRankParameter);
            }

            #endregion

            #region Length by value

            /// <summary>
            /// Returns the number of members whose value is between min and max in the ordered set key
            /// </summary>
            /// <param name="sortedSetLengthByValueParameter">Sorted set length by value parameter</param>
            /// <returns>Return cache result</returns>
            public static SortedSetLengthByValueResult LengthByValue(SortedSetLengthByValueParameter sortedSetLengthByValueParameter)
            {
                return ExecuteCacheOperation(sortedSetLengthByValueParameter);
            }

            #endregion

            #region Length

            /// <summary>
            /// Returns the number of members in the ordered set key whose score value is between min and max (including the score value equal to min or max by default)
            /// </summary>
            /// <param name="sortedSetLengthParameter">Sorted set length parameter</param>
            /// <returns>Return cache result</returns>
            public static SortedSetLengthResult Length(SortedSetLengthParameter sortedSetLengthParameter)
            {
                return ExecuteCacheOperation(sortedSetLengthParameter);
            }

            #endregion

            #region Increment

            /// <summary>
            /// Add the incremental increment to the score value of the member of the ordered set key
            /// </summary>
            /// <param name="sortedSetIncrementParameter">Sorted set increment parameter</param>
            /// <returns>Return cache result</returns>
            public static SortedSetIncrementResult Increment(SortedSetIncrementParameter sortedSetIncrementParameter)
            {
                return ExecuteCacheOperation(sortedSetIncrementParameter);
            }

            #endregion

            #region Decrement

            /// <summary>
            /// is the score value of the member of the ordered set key minus the increment increment
            /// </summary>
            /// <param name="sortedSetDecrementParameter">Sorted set decrement parameter</param>
            /// <returns>Return cache result</returns>
            public static SortedSetDecrementResult Decrement(SortedSetDecrementParameter sortedSetDecrementParameter)
            {
                return ExecuteCacheOperation(sortedSetDecrementParameter);
            }

            #endregion

            #region Combine and store

            /// <summary>
            /// Aggregate multiple ordered collections and save to destination
            /// </summary>
            /// <param name="sortedSetCombineAndStoreParameter">Sorted set combine and store parameter</param>
            /// <returns>Return cache result</returns>
            public static SortedSetCombineAndStoreResult CombineAndStore(SortedSetCombineAndStoreParameter sortedSetCombineAndStoreParameter)
            {
                return ExecuteCacheOperation(sortedSetCombineAndStoreParameter);
            }

            #endregion

            #region Add

            /// <summary>
            /// Add one or more member elements and their score values ​​to the ordered set key
            /// </summary>
            /// <param name="sortedSetAddParameter">Sorted set add parameter</param>
            /// <returns>Return cache result</returns>
            public static SortedSetAddResult Add(SortedSetAddParameter sortedSetAddParameter)
            {
                return ExecuteCacheOperation(sortedSetAddParameter);
            }

            #endregion
        }

        #endregion

        #region Key

        /// <summary>
        /// Key operation
        /// </summary>
        public static partial class Keys
        {
            #region Sort

            /// <summary>
            /// Return or save the sorted elements in the given list, collection, ordered set key
            /// </summary>
            /// <param name="sortParameter">Sort parameter</param>
            /// <returns>Return cache result</returns>
            public static SortResult Sort(SortParameter sortParameter)
            {
                return ExecuteCacheOperation(sortParameter);
            }

            #endregion

            #region Sort and store

            /// <summary>
            /// Return or save the sorted elements in the given list, collection, ordered set key, and save to the specified key value
            /// </summary>
            /// <param name="sortAndStoreParameter">Sort and store parameter</param>
            /// <returns>Return cache result</returns>
            public static SortAndStoreResult SortAndStore(SortAndStoreParameter sortAndStoreParameter)
            {
                return ExecuteCacheOperation(sortAndStoreParameter);
            }

            #endregion

            #region Type

            /// <summary>
            /// Returns the type of value stored by key
            /// </summary>
            /// <param name="typeParameter">type parameter</param>
            /// <returns>Return cache result</returns>
            public static TypeResult Type(TypeParameter typeParameter)
            {
                return ExecuteCacheOperation(typeParameter);
            }

            #endregion

            #region Time to live

            /// <summary>
            /// Return the remaining time to live for a given key (TTL, time to live)
            /// </summary>
            /// <param name="timeToLiveParameter">Time to live parameter</param>
            /// <returns>Return cache result</returns>
            public static TimeToLiveResult TimeToLive(TimeToLiveParameter timeToLiveParameter)
            {
                return ExecuteCacheOperation(timeToLiveParameter);
            }

            #endregion

            #region Restore

            /// <summary>
            /// Deserialize the given serialized value and associate it with the given key
            /// </summary>
            /// <param name="restoreParameter">Restore parameter</param>
            /// <returns> Return cache result </returns>
            public static RestoreResult Restore(RestoreParameter restoreParameter)
            {
                return ExecuteCacheOperation(restoreParameter);
            }

            #endregion

            #region Rename

            /// <summary>
            /// Rename the key to newkey
            /// When the key and newkey are the same, or the key does not exist, an error is returned
            /// When newkey already exists, it will overwrite the old value
            /// </summary>
            /// <param name="renameParameter">Rename parameter</param>
            /// <returns>Return cache result</returns>
            public static RenameResult Rename(RenameParameter renameParameter)
            {
                return ExecuteCacheOperation(renameParameter);
            }

            #endregion

            #region Random

            /// <summary>
            /// randomly return (do not delete) a key
            /// </summary>
            /// <param name="randomParameter">Random parameter</param>
            /// <returns>Return cache result</returns>
            public static RandomResult KeyRandom(RandomParameter randomParameter)
            {
                return ExecuteCacheOperation(randomParameter);
            }

            #endregion

            #region Persist

            /// <summary>
            /// Remove the survival time of a given key, and convert this key from "volatile" (with survival time key) to "persistent" (a key with no survival time and never expires)
            /// </summary>
            /// <param name="persistParameter">Persist parameter</param>
            /// <returns>Return cache result</returns>
            public static PersistResult Persist(PersistParameter persistParameter)
            {
                return ExecuteCacheOperation(persistParameter);
            }

            #endregion

            #region Move

            /// <summary>
            /// Move the key of the current database to the given database
            /// </summary>
            /// <param name="moveParameter">Move parameter</param>
            /// <returns>Return cache result</returns>
            public static MoveResult Move(MoveParameter moveParameter)
            {
                return ExecuteCacheOperation(moveParameter);
            }

            #endregion

            #region Migrate

            /// <summary>
            /// Transfer the key atomically from the current instance to the specified database of the target instance. Once the transfer is successful, the key is guaranteed to appear on the target instance, and the key on the current instance will be deleted.
            /// </summary>
            /// <param name="migrateParameter">Migrate parameter</param>
            /// <returns>Return cache result</returns>
            public static MigrateKeyResult Migrate(MigrateKeyParameter migrateParameter)
            {
                return ExecuteCacheOperation(migrateParameter);
            }

            #endregion

            #region Expire

            /// <summary>
            /// Set the survival time for the given key. When the key expires (the survival time is 0), it will be automatically deleted
            /// </summary>
            /// <param name="expireParameter">Expire parameter</param>
            /// <returns>Return cache result</returns>
            public static ExpireResult Expire(ExpireParameter expireParameter)
            {
                return ExecuteCacheOperation(expireParameter);
            }

            #endregion;

            #region Dump

            /// <summary>
            /// Serialize the given key and return the serialized value. Use the RESTORE command to deserialize this value
            /// </summary>
            /// <param name="dumpParameter">Dump parameter</param>
            /// <returns>Return cache result</returns>
            public static DumpResult Dump(DumpParameter dumpParameter)
            {
                return ExecuteCacheOperation(dumpParameter);
            }

            #endregion

            #region Delete

            /// <summary>
            /// Delete the specified key
            /// </summary>
            /// <param name="deleteParameter">Delete parameter</param>
            /// <returns>Return cache result</returns>
            public static DeleteResult Delete(DeleteParameter deleteParameter)
            {
                return ExecuteCacheOperation(deleteParameter);
            }

            #endregion

            #region Get keys

            /// <summary>
            /// Find all keys that match a given pattern
            /// </summary>
            /// <param name="getKeysParameter">Get keys parameter</param>
            /// <returns>Return cache result</returns>
            public static GetKeysResult GetKeys(GetKeysParameter getKeysParameter)
            {
                return ExecuteCacheOperation(getKeysParameter);
            }

            #endregion

            #region Exist

            /// <summary>
            /// Check whether key exist
            /// </summary>
            /// <param name="existParameter">Exist parameter</param>
            /// <returns>Return cache result</returns>
            public static ExistResult Exist(ExistParameter existParameter)
            {
                return ExecuteCacheOperation(existParameter);
            }

            #endregion
        }

        #endregion

        #region Server

        /// <summary>
        /// Cache server operation
        /// </summary>
        public static partial class Server
        {
            #region Get all data base

            /// <summary>
            /// Returns all cached databases in the server
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="getAllDataBaseParameter">Get all database parameter</param>
            /// <returns>Return cache result</returns>
            public static GetAllDataBaseResult GetAllDataBase(CacheServer server, GetAllDataBaseParameter getAllDataBaseParameter)
            {
                return getAllDataBaseParameter.Execute(server);
            }

            #endregion

            #region Query keys

            /// <summary>
            /// Query key value
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="getKeysParameter"> Get keys options </param>
            /// <returns>Return cache result</returns>
            public static GetKeysResult GetKeys(CacheServer server, GetKeysParameter getKeysParameter)
            {
                return getKeysParameter.Execute(server);
            }

            #endregion

            #region Clear data

            /// <summary>
            /// Clear all data in the cache database
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="clearDataParameter"> Clear data options </param>
            /// <returns>Return cache result</returns>
            public static ClearDataResult ClearData(CacheServer server, ClearDataParameter clearDataParameter)
            {
                return clearDataParameter.Execute(server);
            }

            #endregion

            #region Get cache item detail

            /// <summary>
            /// Get data item details
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="getDetailParameter"> Get detail options </param>
            /// <returns>Return cache result</returns>
            public static GetDetailResult GetKeyDetail(CacheServer server, GetDetailParameter getDetailParameter)
            {
                return getDetailParameter.Execute(server);
            }

            #endregion

            #region Get server configuration

            /// <summary>
            /// Get server configuration
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="getServerConfigurationParameter">Get server configuration parameter</param>
            /// <returns>Return cache result</returns>
            public static GetServerConfigurationResult GetServerConfiguration(CacheServer server, GetServerConfigurationParameter getServerConfigurationParameter)
            {
                return getServerConfigurationParameter.Execute(server);
            }

            #endregion

            #region Save server configuration

            /// <summary>
            /// Save the server configuration
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="saveServerConfigurationParameter"> Save server configuration options </param>
            /// <returns>Return cache result</returns>
            public static SaveServerConfigurationResult SaveServerConfiguration(CacheServer server, SaveServerConfigurationParameter saveServerConfigurationParameter)
            {
                return saveServerConfigurationParameter.Execute(server);
            }

            #endregion
        }

        #endregion

        #region Provider

        /// <summary>
        /// Get cache provider
        /// </summary>
        /// <param name="databaseType">Server type</param>
        /// <returns>Return cache provider</returns>
        internal static ISixnetCacheProvider GetCacheProvider(CacheServerType databaseType)
        {
            return Options?.GetCacheProvider(databaseType);
        }

        #endregion

        #region Cache server

        /// <summary>
        /// Get cache server
        /// </summary>
        /// <param name="operationParameter">Cache operation parameter</param>
        /// <returns>Return cache server</returns>
        internal static CacheServer GetCacheServer<T>(CacheParameter<T> operationParameter) where T : CacheResult, new()
        {
            return Options?.GetCacheServer(operationParameter);
        }

        /// <summary>
        /// Get default in-memory server
        /// </summary>
        /// <returns></returns>
        internal static CacheServer GetDefaultInMemoryServer()
        {
            return Options?.DefaultInMemoryServer;
        }

        #endregion

        #region Key prefixs

        /// <summary>
        /// Get global cache key prefixs
        /// </summary>
        /// <returns>Return global cache key prefixs</returns>
        internal static List<string> GetGlobalPrefixs()
        {
            return Options?.GetGlobalPrefixs();
        }

        /// <summary>
        /// Get cache object prefixs
        /// </summary>
        /// <param name="cacheObject">Cache object</param>
        /// <returns>Return cache object prefixs</returns>
        internal static List<string> GetObjectPrefixs(CacheObject cacheObject)
        {
            return Options?.GetObjectPrefixs(cacheObject);
        }

        #endregion

        #region Split char

        /// <summary>
        /// Get name&value split char
        /// </summary>
        /// <returns></returns>
        internal static string GetNameValueSplitChar()
        {
            return Options?.NameValueSplitChar;
        }

        /// <summary>
        /// Get key&name split char
        /// </summary>
        /// <returns></returns>
        internal static string GetKeyNameSplitChar()
        {
            return Options?.KeyNameSplitChar;
        }

        #endregion

        #region Execute cache operation

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="options">Request parameter</param>
        /// <returns>Reurn cache result</returns>
        static TResponse ExecuteCacheOperation<TResponse>(CacheParameter<TResponse> options) where TResponse : CacheResult, new()
        {
            SixnetDirectThrower.ThrowArgNullIf(options == null, nameof(options));
            return options.Execute();
        }

        #endregion

        #region Get cache options

        /// <summary>
        /// Get cache options
        /// </summary>
        /// <returns></returns>
        static CacheOptions GetCacheOptions()
        {
            var options = SixnetContainer.GetOptions<CacheOptions>();
            return options ?? _defaultOptions;
        }

        #endregion
    }
}
