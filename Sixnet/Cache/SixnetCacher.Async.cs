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
using Sixnet.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sixnet.Cache
{
    /// <summary>
    /// Cache manager
    /// </summary>
    public static partial class SixnetCacher
    {
        #region Data

        #region Set data

        /// <summary>
        /// Serializes the specified object and saves it to the cache
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="data">Data object</param>
        /// <param name="absoluteExpiration">Absolute expiration time</param>
        /// <param name="when">Cache setting conditions</param>
        /// <param name="cacheObject">The data belongs to the cache object</param>
        /// <returns>Return cache set result</returns>
        public static async Task<StringSetResult> SetDataAsync<T>(CacheKey key, T data, DateTimeOffset? absoluteExpiration = null, CacheSetWhen when = CacheSetWhen.Always, CacheObject cacheObject = null)
        {
            var value = SixnetJsonSerializer.Serialize(data);
            if (string.IsNullOrWhiteSpace(value))
            {
                return CacheResult.FailResponse<StringSetResult>(SixnetCacheCodes.ValuesIsNullOrEmpty);
            }
            return await String.SetAsync(key, value, absoluteExpiration, when, cacheObject).ConfigureAwait(false);
        }

        /// <summary>
        /// Serializes the specified object and saves it to the cache
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="data">Data object</param>
        /// <param name="absoluteExpirationRelativeToNow">The expiration time relative to the current time</param>
        /// <param name="slidingExpiration">Whether to enable sliding expiration</param>
        /// <param name="when">Cache setting conditions</param>
        /// <param name="cacheObject">The data belongs to the cache object</param>
        /// <returns>Return cache set result</returns>
        public static async Task<StringSetResult> SetDataByRelativeExpirationAsync<T>(CacheKey key, T data, TimeSpan? absoluteExpirationRelativeToNow = null, bool slidingExpiration = true, CacheSetWhen when = CacheSetWhen.Always, CacheObject cacheObject = null)
        {
            var value = SixnetJsonSerializer.Serialize(data);
            if (string.IsNullOrWhiteSpace(value))
            {
                return CacheResult.FailResponse<StringSetResult>(SixnetCacheCodes.ValuesIsNullOrEmpty);
            }
            return await String.SetByRelativeExpirationAsync(key, value, absoluteExpirationRelativeToNow, slidingExpiration, when, cacheObject).ConfigureAwait(false);
        }

        #endregion

        #region Get data

        /// <summary>
        /// Get the data object according to the specified key value (use Json deserialization)
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="cacheObject">Cache object</param>
        /// <returns>Return data object</returns>
        public static async Task<T> GetDataAsync<T>(CacheKey key, CacheObject cacheObject = null)
        {
            return await String.GetAsync<T>(key, cacheObject).ConfigureAwait(false);
        }

        #endregion

        #region Get data list

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="cacheKeys">Cache keys</param>
        /// <param name="cacheObject">Cache object</param>
        /// <returns>Return data list</returns>
        public static async Task<List<T>> GetDataListAsync<T>(IEnumerable<CacheKey> cacheKeys, CacheObject cacheObject = null)
        {
            return await String.GetAsync<T>(cacheKeys, cacheObject).ConfigureAwait(false);
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
            public static async Task<StringSetRangeResult> SetRangeAsync(StringSetRangeParameter stringSetRangeParameter)
            {
                return await ExecuteCommandAsync(stringSetRangeParameter).ConfigureAwait(false);
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
            public static async Task<StringSetBitResult> SetBitAsync(StringSetBitParameter stringSetBitParameter)
            {
                return await ExecuteCommandAsync(stringSetBitParameter).ConfigureAwait(false);
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
            public static async Task<StringSetResult> SetAsync(StringSetParameter stringSetParameter)
            {
                return await ExecuteCommandAsync(stringSetParameter).ConfigureAwait(false);
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
            public static async Task<StringSetResult> SetAsync(CacheKey key, string value, DateTimeOffset? absoluteExpiration = null, CacheSetWhen when = CacheSetWhen.Always, CacheObject cacheObject = null)
            {
                return await SetAsync(new StringSetParameter()
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
                }).ConfigureAwait(false);
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
            public static async Task<StringSetResult> SetByRelativeExpirationAsync(CacheKey key, string value, TimeSpan? absoluteExpirationRelativeToNow = null, bool slidingExpiration = true, CacheSetWhen when = CacheSetWhen.Always, CacheObject cacheObject = null)
            {
                return await SetAsync(new StringSetParameter()
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
                }).ConfigureAwait(false);
            }

            #endregion

            #region Length

            /// <summary>
            /// Returns the length of the string value stored by key
            /// </summary>
            /// <param name="stringLengthParameter">String length parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<StringLengthResult> LengthAsync(StringLengthParameter stringLengthParameter)
            {
                return await ExecuteCommandAsync(stringLengthParameter).ConfigureAwait(false);
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
            public static async Task<StringIncrementResult> IncrementAsync(StringIncrementParameter stringIncrementParameter)
            {
                return await ExecuteCommandAsync(stringIncrementParameter).ConfigureAwait(false);
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
            public static async Task<StringGetWithExpiryResult> GetWithExpiryAsync(StringGetWithExpiryParameter stringGetWithExpiryParameter)
            {
                return await ExecuteCommandAsync(stringGetWithExpiryParameter).ConfigureAwait(false);
            }

            #endregion

            #region Get set

            /// <summary>
            /// Set the value of the key key to value and return the old value of the key key before it is set
            /// When the key key exists but is not a string type, the command returns an error
            /// </summary>
            /// <param name="stringGetSetParameter">String get set parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<StringGetSetResult> GetSetAsync(StringGetSetParameter stringGetSetParameter)
            {
                return await ExecuteCommandAsync(stringGetSetParameter).ConfigureAwait(false);
            }

            #endregion

            #region Get range

            /// <summary>
            /// Returns the specified part of the string value stored by the key key, the range of the string interception is determined by the two offsets of start and end (including start and end)
            /// Negative offset means counting from the end of the string, -1 means the last character, -2 means the penultimate character, and so on
            /// </summary>
            /// <param name="stringGetRangeParameter">String get range parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<StringGetRangeResult> GetRangeAsync(StringGetRangeParameter stringGetRangeParameter)
            {
                return await ExecuteCommandAsync(stringGetRangeParameter).ConfigureAwait(false);
            }

            #endregion

            #region Get bit

            /// <summary>
            /// For the string value stored in key, get the bit at the specified offset
            /// When offset is greater than the length of the string value, or the key does not exist, return 0
            /// </summary>
            /// <param name="stringGetBitParameter">String get bit parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<StringGetBitResult> GetBitAsync(StringGetBitParameter stringGetBitParameter)
            {
                return await ExecuteCommandAsync(stringGetBitParameter).ConfigureAwait(false);
            }

            #endregion

            #region Get

            /// <summary>
            /// Returns the value of the given string key or keys
            /// </summary>
            /// <param name="stringGetParameter">String get parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<StringGetResult> GetAsync(StringGetParameter stringGetParameter)
            {
                return await ExecuteCommandAsync(stringGetParameter).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns the string value associated with the key
            /// If the key key does not exist, then return an empty string, otherwise, return the value of the key key
            /// If the value of key is not of type string, then return an error.
            /// </summary>
            /// <param name="key">Cache key</param>
            /// <param name="cacheObject">Cache object</param>
            /// <returns>Return cache result</returns>
            public static async Task<string> GetAsync(CacheKey key, CacheObject cacheObject = null)
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    return string.Empty;
                }
                var values = await GetAsync(new List<CacheKey>() { key }, cacheObject).ConfigureAwait(false);
                return values?.FirstOrDefault() ?? string.Empty;
            }

            /// <summary>
            /// Return the value associated with the key and convert it to the specified data object
            /// </summary>
            /// <param name="key">Cache key</param>
            /// <param name="cacheObject">Cache object information</param>
            /// <returns>Return cache result</returns>
            public static async Task<T> GetAsync<T>(CacheKey key, CacheObject cacheObject = null)
            {
                var cacheValue = await GetAsync(key, cacheObject).ConfigureAwait(false);
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
            public static async Task<List<string>> GetAsync(IEnumerable<CacheKey> keys, CacheObject cacheObject = null)
            {
                if (keys.IsNullOrEmpty())
                {
                    return new List<string>(0);
                }
                var result = await GetAsync(new StringGetParameter()
                {
                    CacheObject = cacheObject,
                    Keys = keys.ToList()
                }).ConfigureAwait(false);
                return result?.Values?.Select(c => c.Value?.ToString()).ToList();
            }

            /// <summary>
            /// Return the value of the given one or more string keys and convert to the specified data type
            /// </summary>
            /// <typeparam name="T">Data type</typeparam>
            /// <param name="keys">Cache key</param>
            /// <param name="cacheObject">Cache object</param>
            /// <returns>Return datas</returns>
            public static async Task<List<T>> GetAsync<T>(IEnumerable<CacheKey> keys, CacheObject cacheObject = null)
            {
                var values = await GetAsync(keys, cacheObject).ConfigureAwait(false);
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
            public static async Task<StringDecrementResult> DecrementAsync(StringDecrementParameter stringDecrementParameter)
            {
                return await ExecuteCommandAsync(stringDecrementParameter).ConfigureAwait(false);
            }

            #endregion

            #region Bit position

            /// <summary>
            /// Returns the position of the first binary bit in the bitmap
            /// By default, the command will detect the entire bitmap, but the user can also specify the range to be detected through the optional start parameter and end parameter
            /// </summary>
            /// <param name="stringBitPositionParameter">String bit position parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<StringBitPositionResult> BitPositionAsync(StringBitPositionParameter stringBitPositionParameter)
            {
                return await ExecuteCommandAsync(stringBitPositionParameter).ConfigureAwait(false);
            }

            #endregion

            #region Bit operation

            /// <summary>
            /// Perform bit operations on one or more string keys that hold binary bits, and save the result to destkey
            /// Except NOT operation, other operations can accept one or more keys as input
            /// </summary>
            /// <param name="stringBitOperationParameter">String bit operation parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<StringBitOperationResult> BitOperationAsync(StringBitOperationParameter stringBitOperationParameter)
            {
                return await ExecuteCommandAsync(stringBitOperationParameter).ConfigureAwait(false);
            }

            #endregion

            #region Bit count

            /// <summary>
            /// Calculate the number of bits set to 1 in a given string.
            /// Under normal circumstances, the given entire string will be counted, by specifying additional start or end parameters, you can make the count only on a specific bit
            /// </summary>
            /// <param name="stringBitCountParameter">String bit count parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<StringBitCountResult> BitCountAsync(StringBitCountParameter stringBitCountParameter)
            {
                return await ExecuteCommandAsync(stringBitCountParameter).ConfigureAwait(false);
            }

            #endregion

            #region Append

            /// <summary>
            /// If the key key already exists and its value is a string, the value will be appended to the end of the key key's existing value
            /// If the key does not exist, simply set the value of the key key to value
            /// </summary>
            /// <param name="stringAppendParameter">String append parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<StringAppendResult> AppendAsync(StringAppendParameter stringAppendParameter)
            {
                return await ExecuteCommandAsync(stringAppendParameter).ConfigureAwait(false);
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
            public static async Task<ListTrimResult> TrimAsync(ListTrimParameter listTrimParameter)
            {
                return await ExecuteCommandAsync(listTrimParameter).ConfigureAwait(false);
            }

            #endregion

            #region Set by index

            /// <summary>
            /// Set the value of the element whose index of the list key is index to value
            /// When the index parameter is out of range, or an operation is performed on an empty list (the key does not exist), an error is returned
            /// </summary>
            /// <param name="listSetByIndexParameter">List set by index parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<ListSetByIndexResult> SetByIndexAsync(ListSetByIndexParameter listSetByIndexParameter)
            {
                return await ExecuteCommandAsync(listSetByIndexParameter).ConfigureAwait(false);
            }

            #endregion

            #region Right push

            /// <summary>
            /// Insert one or more values ​​into the end of the list key (far right).
            /// If there are multiple value values, then each value value is inserted into the end of the table in order from left to right
            /// </summary>
            /// <param name="listRightPushParameter">List right push parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<ListRightPushResult> RightPushAsync(ListRightPushParameter listRightPushParameter)
            {
                return await ExecuteCommandAsync(listRightPushParameter).ConfigureAwait(false);
            }

            #endregion

            #region Right pop left push

            /// <summary>
            /// Pop the last element (tail element) in the list source and return it to the client
            /// Insert the element popped by source into the destination list as the head element of the destination list
            /// </summary>
            /// <param name="listRightPopLeftPushParameter">List right pop left push parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<ListRightPopLeftPushResult> RightPopLeftPushAsync(ListRightPopLeftPushParameter listRightPopLeftPushParameter)
            {
                return await ExecuteCommandAsync(listRightPopLeftPushParameter).ConfigureAwait(false);
            }

            #endregion

            #region Right pop

            /// <summary>
            /// Remove and return the tail element of the list key.
            /// </summary>
            /// <param name="listRightPopParameter">List right pop parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<ListRightPopResult> RightPopAsync(ListRightPopParameter listRightPopParameter)
            {
                return await ExecuteCommandAsync(listRightPopParameter).ConfigureAwait(false);
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
            public static async Task<ListRemoveResult> RemoveAsync(ListRemoveParameter listRemoveParameter)
            {
                return await ExecuteCommandAsync(listRemoveParameter).ConfigureAwait(false);
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
            public static async Task<ListRangeResult> RangeAsync(ListRangeParameter listRangeParameter)
            {
                return await ExecuteCommandAsync(listRangeParameter).ConfigureAwait(false);
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
            public static async Task<ListLengthResult> LengthAsync(ListLengthParameter listLengthParameter)
            {
                return await ExecuteCommandAsync(listLengthParameter).ConfigureAwait(false);
            }

            #endregion

            #region Left push

            /// <summary>
            /// Insert one or more values ​​into the header of the list key
            /// If the key does not exist, an empty list will be created and the operation will be performed
            /// </summary>
            /// <param name="listLeftPushParameter">List left push parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<ListLeftPushResult> LeftPushAsync(ListLeftPushParameter listLeftPushParameter)
            {
                return await ExecuteCommandAsync(listLeftPushParameter).ConfigureAwait(false);
            }

            #endregion

            #region Left pop

            /// <summary>
            /// Remove and return the head element of the list key
            /// </summary>
            /// <param name="listLeftPopParameter">List left pop parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<ListLeftPopResult> LeftPopAsync(ListLeftPopParameter listLeftPopParameter)
            {
                return await ExecuteCommandAsync(listLeftPopParameter).ConfigureAwait(false);
            }

            #endregion

            #region Insert before

            /// <summary>
            /// Insert the value value into the list key, before the value pivot
            /// </summary>
            /// <param name="listInsertBeforeParameter">List insert before parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<ListInsertBeforeResult> InsertBeforeAsync(ListInsertBeforeParameter listInsertBeforeParameter)
            {
                return await ExecuteCommandAsync(listInsertBeforeParameter).ConfigureAwait(false);
            }

            #endregion

            #region Insert after

            /// <summary>
            /// Insert the value value into the list key, after the value pivot
            /// </summary>
            /// <param name="listInsertAfterParameter">List insert after parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<ListInsertAfterResult> InsertAfterAsync(ListInsertAfterParameter listInsertAfterParameter)
            {
                return await ExecuteCommandAsync(listInsertAfterParameter).ConfigureAwait(false);
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
            public static async Task<ListGetByIndexResult> GetByIndexAsync(ListGetByIndexParameter listGetByIndexParameter)
            {
                return await ExecuteCommandAsync(listGetByIndexParameter).ConfigureAwait(false);
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
            public static async Task<HashValuesResult> ValuesAsync(HashValuesParameter hashValuesParameter)
            {
                return await ExecuteCommandAsync(hashValuesParameter).ConfigureAwait(false);
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
            public static async Task<HashSetResult> SetAsync(HashSetParameter hashSetParameter)
            {
                return await ExecuteCommandAsync(hashSetParameter).ConfigureAwait(false);
            }

            #endregion

            #region Length

            /// <summary>
            /// returns the number of fields in the hash table key
            /// </summary>
            /// <param name="hashLengthParameter">Hash length parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<HashLengthResult> LengthAsync(HashLengthParameter hashLengthParameter)
            {
                return await ExecuteCommandAsync(hashLengthParameter).ConfigureAwait(false);
            }

            #endregion

            #region Keys

            /// <summary>
            /// Return all keys in the hash table key
            /// </summary>
            /// <param name="hashKeysParameter">Hash keys parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<HashKeysResult> KeysAsync(HashKeysParameter hashKeysParameter)
            {
                return await ExecuteCommandAsync(hashKeysParameter).ConfigureAwait(false);
            }

            #endregion

            #region Increment

            /// <summary>
            /// Add incremental increment to the value of the field field in the hash table key
            /// </summary>
            /// <param name="hashIncrementParameter">Hash increment parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<HashIncrementResult> IncrementAsync(HashIncrementParameter hashIncrementParameter)
            {
                return await ExecuteCommandAsync(hashIncrementParameter).ConfigureAwait(false);
            }

            #endregion

            #region Get

            /// <summary>
            /// Returns the value of the given field in the hash table
            /// </summary>
            /// <param name="hashGetParameter">Hash get parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<HashGetResult> GetAsync(HashGetParameter hashGetParameter)
            {
                return await ExecuteCommandAsync(hashGetParameter).ConfigureAwait(false);
            }

            #endregion

            #region Get all

            /// <summary>
            /// Return the values ​​of all fields in the hash table key
            /// </summary>
            /// <param name="hashGetAllParameter">Hash get all parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<HashGetAllResult> GetAllAsync(HashGetAllParameter hashGetAllParameter)
            {
                return await ExecuteCommandAsync(hashGetAllParameter).ConfigureAwait(false);
            }

            #endregion

            #region Exist

            /// <summary>
            /// Check if the given field exists in the hash of the hash table
            /// </summary>
            /// <param name="hashExistsParameter">Hash exists parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<HashExistsResult> ExistAsync(HashExistsParameter hashExistsParameter)
            {
                return await ExecuteCommandAsync(hashExistsParameter).ConfigureAwait(false);
            }

            #endregion

            #region Delete

            /// <summary>
            /// Delete one or more specified fields in the hash table key, the non-existing fields will be ignored
            /// </summary>
            /// <param name="hashDeleteParameter">Hash delete parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<HashDeleteResult> DeleteAsync(HashDeleteParameter hashDeleteParameter)
            {
                return await ExecuteCommandAsync(hashDeleteParameter).ConfigureAwait(false);
            }

            #endregion

            #region Decrement

            /// <summary>
            /// Is the value of the field in the hash table key minus the increment increment
            /// </summary>
            /// <param name="hashDecrementParameter">Hash decrement parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<HashDecrementResult> DecrementAsync(HashDecrementParameter hashDecrementParameter)
            {
                return await ExecuteCommandAsync(hashDecrementParameter).ConfigureAwait(false);
            }

            #endregion

            #region Scan

            /// <summary>
            /// Each element returned is a key-value pair, a key-value pair consists of a key and a value
            /// </summary>
            /// <param name="hashScanParameter">Hash scan parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<HashScanResult> ScanAsync(HashScanParameter hashScanParameter)
            {
                return await ExecuteCommandAsync(hashScanParameter).ConfigureAwait(false);
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
            public static async Task<SetRemoveResult> RemoveAsync(SetRemoveParameter setRemoveParameter)
            {
                return await ExecuteCommandAsync(setRemoveParameter).ConfigureAwait(false);
            }

            #endregion

            #region Random members

            /// <summary>
            /// Then return a set of random elements in the collection
            /// </summary>
            /// <param name="setRandomMembersParameter">Set random members parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<SetRandomMembersResult> RandomMembersAsync(SetRandomMembersParameter setRandomMembersParameter)
            {
                return await ExecuteCommandAsync(setRandomMembersParameter).ConfigureAwait(false);
            }

            #endregion

            #region Random member

            /// <summary>
            /// Returns a random element in the collection
            /// </summary>
            /// <param name="setRandomMemberParameter">Set random member parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<SetRandomMemberResult> RandomMemberAsync(SetRandomMemberParameter setRandomMemberParameter)
            {
                return await ExecuteCommandAsync(setRandomMemberParameter).ConfigureAwait(false);
            }

            #endregion

            #region Pop

            /// <summary>
            /// Remove and return a random element in the collection
            /// </summary>
            /// <param name="setPopParameter">Set pop parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<SetPopResult> PopAsync(SetPopParameter setPopParameter)
            {
                return await ExecuteCommandAsync(setPopParameter).ConfigureAwait(false);
            }

            #endregion

            #region Move

            /// <summary>
            /// Move the member element from the source collection to the destination collection
            /// </summary>
            /// <param name="setMoveParameter">Set move parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<SetMoveResult> MoveAsync(SetMoveParameter setMoveParameter)
            {
                return await ExecuteCommandAsync(setMoveParameter).ConfigureAwait(false);
            }

            #endregion

            #region Members

            /// <summary>
            /// Return all members in the collection key
            /// </summary>
            /// <param name="setMembersParameter">Set members parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<SetMembersResult> MembersAsync(SetMembersParameter setMembersParameter)
            {
                return await ExecuteCommandAsync(setMembersParameter).ConfigureAwait(false);
            }

            #endregion

            #region Length

            /// <summary>
            /// Returns the number of elements in the collection
            /// </summary>
            /// <param name="setLengthParameter">Set length parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<SetLengthResult> LengthAsync(SetLengthParameter setLengthParameter)
            {
                return await ExecuteCommandAsync(setLengthParameter).ConfigureAwait(false);
            }

            #endregion

            #region Contains

            /// <summary>
            /// Determine whether the member element is a member of the set key
            /// </summary>
            /// <param name="setContainsParameter">Set contaims parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<SetContainsResult> ContainsAsync(SetContainsParameter setContainsParameter)
            {
                return await ExecuteCommandAsync(setContainsParameter).ConfigureAwait(false);
            }

            #endregion

            #region Combine

            /// <summary>
            /// According to the operation mode, return the processing results of multiple collections
            /// </summary>
            /// <param name="setCombineParameter">Set combine parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<SetCombineResult> CombineAsync(SetCombineParameter setCombineParameter)
            {
                return await ExecuteCommandAsync(setCombineParameter).ConfigureAwait(false);
            }

            #endregion

            #region Combine and store

            /// <summary>
            /// Return the processing results of multiple collections according to the operation mode, and store the results to the specified key value at the same time
            /// </summary>
            /// <param name="setCombineAndStoreParameter">Set combine and store parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<SetCombineAndStoreResult> CombineAndStoreAsync(SetCombineAndStoreParameter setCombineAndStoreParameter)
            {
                return await ExecuteCommandAsync(setCombineAndStoreParameter).ConfigureAwait(false);
            }

            #endregion

            #region Add

            /// <summary>
            /// Add one or more member elements to the collection key, the member elements already in the collection will be ignored
            /// If the key does not exist, create a collection that contains only the member element as a member
            /// </summary>
            /// <param name="setAddParameter">Set add parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<SetAddResult> AddAsync(SetAddParameter setAddParameter)
            {
                return await ExecuteCommandAsync(setAddParameter).ConfigureAwait(false);
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
            public static async Task<SortedSetScoreResult> ScoreAsync(SortedSetScoreParameter sortedSetScoreParameter)
            {
                return await ExecuteCommandAsync(sortedSetScoreParameter).ConfigureAwait(false);
            }

            #endregion

            #region Remove range by value

            /// <summary>
            /// Remove the elements in the specified range after sorting the elements
            /// </summary>
            /// <param name="sortedSetRemoveRangeByValueParameter">Sorted set remove range by value parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<SortedSetRemoveRangeByValueResult> RemoveRangeByValueAsync(SortedSetRemoveRangeByValueParameter sortedSetRemoveRangeByValueParameter)
            {
                return await ExecuteCommandAsync(sortedSetRemoveRangeByValueParameter).ConfigureAwait(false);
            }

            #endregion

            #region Remove range by score

            /// <summary>
            /// Remove all members in the ordered set key whose score value is between min and max
            /// </summary>
            /// <param name="sortedSetRemoveRangeByScoreParameter">Sorted set remove range by score parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<SortedSetRemoveRangeByScoreResult> RemoveRangeByScoreAsync(SortedSetRemoveRangeByScoreParameter sortedSetRemoveRangeByScoreParameter)
            {
                return await ExecuteCommandAsync(sortedSetRemoveRangeByScoreParameter).ConfigureAwait(false);
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
            public static async Task<SortedSetRemoveRangeByRankResult> RemoveRangeByRankAsync(SortedSetRemoveRangeByRankParameter sortedSetRemoveRangeByRankParameter)
            {
                return await ExecuteCommandAsync(sortedSetRemoveRangeByRankParameter).ConfigureAwait(false);
            }

            #endregion

            #region Remove

            /// <summary>
            /// Remove the specified element in the ordered collection
            /// </summary>
            /// <param name="sortedSetRemoveParameter">Sorted set remove parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<SortedSetRemoveResult> RemoveAsync(SortedSetRemoveParameter sortedSetRemoveParameter)
            {
                return await ExecuteCommandAsync(sortedSetRemoveParameter).ConfigureAwait(false);
            }

            #endregion

            #region Rank

            /// <summary>
            /// Returns the ranking of the member member in the ordered set key. The members of the ordered set are arranged in order of increasing score value (from small to large) by default
            /// The ranking is based on 0, that is, the member with the smallest score is ranked 0
            /// </summary>
            /// <param name="sortedSetRankParameter">Sorted set rank parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<SortedSetRankResult> RankAsync(SortedSetRankParameter sortedSetRankParameter)
            {
                return await ExecuteCommandAsync(sortedSetRankParameter).ConfigureAwait(false);
            }

            #endregion

            #region Range by value

            /// <summary>
            /// Returns the elements in the ordered set between min and max
            /// </summary>
            /// <param name="sortedSetRangeByValueParameter">Sorted set range by value parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<SortedSetRangeByValueResult> RangeByValueAsync(SortedSetRangeByValueParameter sortedSetRangeByValueParameter)
            {
                return await ExecuteCommandAsync(sortedSetRangeByValueParameter).ConfigureAwait(false);
            }

            #endregion

            #region Range by score with scores

            /// <summary>
            /// Returns the value and score of the members in the specified interval in the ordered set key, the positions are arranged according to score
            /// </summary>
            /// <param name="sortedSetRangeByScoreWithScoresParameter">Sorted set range by score with scores parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<SortedSetRangeByScoreWithScoresResult> RangeByScoreWithScoresAsync(SortedSetRangeByScoreWithScoresParameter sortedSetRangeByScoreWithScoresParameter)
            {
                return await ExecuteCommandAsync(sortedSetRangeByScoreWithScoresParameter).ConfigureAwait(false);
            }

            #endregion

            #region Range by score

            /// <summary>
            /// Return the value of the members in the specified interval in the ordered set key, the position is arranged by score
            /// </summary>
            /// <param name="sortedSetRangeByScoreParameter">Sorted set range by score parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<SortedSetRangeByScoreResult> RangeByScoreAsync(SortedSetRangeByScoreParameter sortedSetRangeByScoreParameter)
            {
                return await ExecuteCommandAsync(sortedSetRangeByScoreParameter).ConfigureAwait(false);
            }

            #endregion

            #region Range by rank with scores

            /// <summary>
            /// Returns the value and score of the members in the specified interval in the ordered set key, the positions are arranged according to score
            /// </summary>
            /// <param name="sortedSetRangeByRankWithScoresParameter">Sorted set range by rank with scores parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<SortedSetRangeByRankWithScoresResult> RangeByRankWithScoresAsync(SortedSetRangeByRankWithScoresParameter sortedSetRangeByRankWithScoresParameter)
            {
                return await ExecuteCommandAsync(sortedSetRangeByRankWithScoresParameter).ConfigureAwait(false);
            }

            #endregion

            #region Range by rank

            /// <summary>
            /// Return the value of the members in the specified interval in the ordered set key, the positions are arranged by score
            /// </summary>
            /// <param name="sortedSetRangeByRankParameter">Sorted set range by rank parameter</param>
            /// <returns>sorted set range by rank response</returns>
            public static async Task<SortedSetRangeByRankResult> RangeByRankAsync(SortedSetRangeByRankParameter sortedSetRangeByRankParameter)
            {
                return await ExecuteCommandAsync(sortedSetRangeByRankParameter).ConfigureAwait(false);
            }

            #endregion

            #region Length by value

            /// <summary>
            /// Returns the number of members whose value is between min and max in the ordered set key
            /// </summary>
            /// <param name="sortedSetLengthByValueParameter">Sorted set length by value parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<SortedSetLengthByValueResult> LengthByValueAsync(SortedSetLengthByValueParameter sortedSetLengthByValueParameter)
            {
                return await ExecuteCommandAsync(sortedSetLengthByValueParameter).ConfigureAwait(false);
            }

            #endregion

            #region Length

            /// <summary>
            /// Returns the number of members in the ordered set key whose score value is between min and max (including the score value equal to min or max by default)
            /// </summary>
            /// <param name="sortedSetLengthParameter">Sorted set length parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<SortedSetLengthResult> LengthAsync(SortedSetLengthParameter sortedSetLengthParameter)
            {
                return await ExecuteCommandAsync(sortedSetLengthParameter).ConfigureAwait(false);
            }

            #endregion

            #region Increment

            /// <summary>
            /// Add the incremental increment to the score value of the member of the ordered set key
            /// </summary>
            /// <param name="sortedSetIncrementParameter">Sorted set increment parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<SortedSetIncrementResult> IncrementAsync(SortedSetIncrementParameter sortedSetIncrementParameter)
            {
                return await ExecuteCommandAsync(sortedSetIncrementParameter).ConfigureAwait(false);
            }

            #endregion

            #region Decrement

            /// <summary>
            /// is the score value of the member of the ordered set key minus the increment increment
            /// </summary>
            /// <param name="sortedSetDecrementParameter">Sorted set decrement parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<SortedSetDecrementResult> DecrementAsync(SortedSetDecrementParameter sortedSetDecrementParameter)
            {
                return await ExecuteCommandAsync(sortedSetDecrementParameter).ConfigureAwait(false);
            }

            #endregion

            #region Combine and store

            /// <summary>
            /// Aggregate multiple ordered collections and save to destination
            /// </summary>
            /// <param name="sortedSetCombineAndStoreParameter">Sorted set combine and store parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<SortedSetCombineAndStoreResult> CombineAndStoreAsync(SortedSetCombineAndStoreParameter sortedSetCombineAndStoreParameter)
            {
                return await ExecuteCommandAsync(sortedSetCombineAndStoreParameter).ConfigureAwait(false);
            }

            #endregion

            #region Add

            /// <summary>
            /// Add one or more member elements and their score values ​​to the ordered set key
            /// </summary>
            /// <param name="sortedSetAddParameter">Sorted set add parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<SortedSetAddResult> AddAsync(SortedSetAddParameter sortedSetAddParameter)
            {
                return await ExecuteCommandAsync(sortedSetAddParameter).ConfigureAwait(false);
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
            public static async Task<SortResult> SortAsync(SortParameter sortParameter)
            {
                return await ExecuteCommandAsync(sortParameter).ConfigureAwait(false);
            }

            #endregion

            #region Sort and store

            /// <summary>
            /// Return or save the sorted elements in the given list, collection, ordered set key, and save to the specified key value
            /// </summary>
            /// <param name="sortAndStoreParameter">Sort and store parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<SortAndStoreResult> SortAndStoreAsync(SortAndStoreParameter sortAndStoreParameter)
            {
                return await ExecuteCommandAsync(sortAndStoreParameter).ConfigureAwait(false);
            }

            #endregion

            #region Type

            /// <summary>
            /// Returns the type of value stored by key
            /// </summary>
            /// <param name="typeParameter">type parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<TypeResult> TypeAsync(TypeParameter typeParameter)
            {
                return await ExecuteCommandAsync(typeParameter).ConfigureAwait(false);
            }

            #endregion

            #region Time to live

            /// <summary>
            /// Return the remaining time to live for a given key (TTL, time to live)
            /// </summary>
            /// <param name="timeToLiveParameter">Time to live parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<TimeToLiveResult> TimeToLiveAsync(TimeToLiveParameter timeToLiveParameter)
            {
                return await ExecuteCommandAsync(timeToLiveParameter).ConfigureAwait(false);
            }

            #endregion

            #region Restore

            /// <summary>
            /// Deserialize the given serialized value and associate it with the given key
            /// </summary>
            /// <param name="restoreParameter">Restore parameter</param>
            /// <returns> Return cache result </returns>
            public static async Task<RestoreResult> RestoreAsync(RestoreParameter restoreParameter)
            {
                return await ExecuteCommandAsync(restoreParameter).ConfigureAwait(false);
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
            public static async Task<RenameResult> RenameAsync(RenameParameter renameParameter)
            {
                return await ExecuteCommandAsync(renameParameter).ConfigureAwait(false);
            }

            #endregion

            #region Random

            /// <summary>
            /// randomly return (do not delete) a key
            /// </summary>
            /// <param name="randomParameter">Random parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<RandomResult> KeyRandomAsync(RandomParameter randomParameter)
            {
                return await ExecuteCommandAsync(randomParameter).ConfigureAwait(false);
            }

            #endregion

            #region Persist

            /// <summary>
            /// Remove the survival time of a given key, and convert this key from "volatile" (with survival time key) to "persistent" (a key with no survival time and never expires)
            /// </summary>
            /// <param name="persistParameter">Persist parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<PersistResult> PersistAsync(PersistParameter persistParameter)
            {
                return await ExecuteCommandAsync(persistParameter).ConfigureAwait(false);
            }

            #endregion

            #region Move

            /// <summary>
            /// Move the key of the current database to the given database
            /// </summary>
            /// <param name="moveParameter">Move parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<MoveResult> MoveAsync(MoveParameter moveParameter)
            {
                return await ExecuteCommandAsync(moveParameter).ConfigureAwait(false);
            }

            #endregion

            #region Migrate

            /// <summary>
            /// Transfer the key atomically from the current instance to the specified database of the target instance. Once the transfer is successful, the key is guaranteed to appear on the target instance, and the key on the current instance will be deleted.
            /// </summary>
            /// <param name="migrateParameter">Migrate parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<MigrateKeyResult> MigrateAsync(MigrateKeyParameter migrateParameter)
            {
                return await ExecuteCommandAsync(migrateParameter).ConfigureAwait(false);
            }

            #endregion

            #region Expire

            /// <summary>
            /// Set the survival time for the given key. When the key expires (the survival time is 0), it will be automatically deleted
            /// </summary>
            /// <param name="expireParameter">Expire parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<ExpireResult> ExpireAsync(ExpireParameter expireParameter)
            {
                return await ExecuteCommandAsync(expireParameter).ConfigureAwait(false);
            }

            #endregion;

            #region Dump

            /// <summary>
            /// Serialize the given key and return the serialized value. Use the RESTORE command to deserialize this value
            /// </summary>
            /// <param name="dumpParameter">Dump parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<DumpResult> DumpAsync(DumpParameter dumpParameter)
            {
                return await ExecuteCommandAsync(dumpParameter).ConfigureAwait(false);
            }

            #endregion

            #region Delete

            /// <summary>
            /// Delete the specified key
            /// </summary>
            /// <param name="deleteParameter">Delete parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<DeleteResult> DeleteAsync(DeleteParameter deleteParameter)
            {
                return await ExecuteCommandAsync(deleteParameter).ConfigureAwait(false);
            }

            #endregion

            #region Get keys

            /// <summary>
            /// Find all keys that match a given pattern
            /// </summary>
            /// <param name="getKeysParameter">Get keys parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<GetKeysResult> GetKeysAsync(GetKeysParameter getKeysParameter)
            {
                return await ExecuteCommandAsync(getKeysParameter).ConfigureAwait(false);
            }

            #endregion

            #region Exist

            /// <summary>
            /// Check whether key exist
            /// </summary>
            /// <param name="existParameter">Exist parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<ExistResult> ExistAsync(ExistParameter existParameter)
            {
                return await ExecuteCommandAsync(existParameter).ConfigureAwait(false);
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
            public static async Task<GetAllDataBaseResult> GetAllDataBaseAsync(CacheServer server, GetAllDataBaseParameter getAllDataBaseParameter)
            {
                return await getAllDataBaseParameter.ExecuteAsync(server).ConfigureAwait(false);
            }

            #endregion

            #region Query keys

            /// <summary>
            /// Query key value
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="getKeysParameter"> Get keys options </param>
            /// <returns>Return cache result</returns>
            public static async Task<GetKeysResult> GetKeysAsync(CacheServer server, GetKeysParameter getKeysParameter)
            {
                return await getKeysParameter.ExecuteAsync(server).ConfigureAwait(false);
            }

            #endregion

            #region Clear data

            /// <summary>
            /// Clear all data in the cache database
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="clearDataParameter"> Clear data options </param>
            /// <returns>Return cache result</returns>
            public static async Task<ClearDataResult> ClearDataAsync(CacheServer server, ClearDataParameter clearDataParameter)
            {
                return await clearDataParameter.ExecuteAsync(server).ConfigureAwait(false);
            }

            #endregion

            #region Get cache item detail

            /// <summary>
            /// Get data item details
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="getDetailParameter"> Get detail options </param>
            /// <returns>Return cache result</returns>
            public static async Task<GetDetailResult> GetKeyDetailAsync(CacheServer server, GetDetailParameter getDetailParameter)
            {
                return await getDetailParameter.ExecuteAsync(server).ConfigureAwait(false);
            }

            #endregion

            #region Get server configuration

            /// <summary>
            /// Get server configuration
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="getServerConfigurationParameter">Get server configuration parameter</param>
            /// <returns>Return cache result</returns>
            public static async Task<GetServerConfigurationResult> GetServerConfigurationAsync(CacheServer server, GetServerConfigurationParameter getServerConfigurationParameter)
            {
                return await getServerConfigurationParameter.ExecuteAsync(server).ConfigureAwait(false);
            }

            #endregion

            #region Save server configuration

            /// <summary>
            /// Save the server configuration
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="saveServerConfigurationParameter"> Save server configuration options </param>
            /// <returns>Return cache result</returns>
            public static async Task<SaveServerConfigurationResult> SaveServerConfigurationAsync(CacheServer server, SaveServerConfigurationParameter saveServerConfigurationParameter)
            {
                return await saveServerConfigurationParameter.ExecuteAsync(server).ConfigureAwait(false);
            }

            #endregion
        }

        #endregion

        #region Execute command

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="options">Request parameter</param>
        /// <returns>Reurn cache result</returns>
        static async Task<TResponse> ExecuteCommandAsync<TResponse>(CacheParameter<TResponse> options) where TResponse : CacheResult, new()
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            return await options.ExecuteAsync().ConfigureAwait(false);
        }

        #endregion
    }
}