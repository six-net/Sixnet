using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sixnet.Cache.Hash;
using Sixnet.Cache.Hash.Options;
using Sixnet.Cache.Hash.Response;
using Sixnet.Cache.Keys;
using Sixnet.Cache.Keys.Options;
using Sixnet.Cache.Keys.Response;
using Sixnet.Cache.List;
using Sixnet.Cache.List.Options;
using Sixnet.Cache.List.Response;
using Sixnet.Cache.Provider.Memory;
using Sixnet.Cache.Server;
using Sixnet.Cache.Server.Options;
using Sixnet.Cache.Server.Response;
using Sixnet.Cache.Set;
using Sixnet.Cache.Set.Options;
using Sixnet.Cache.Set.Response;
using Sixnet.Cache.SortedSet;
using Sixnet.Cache.SortedSet.Options;
using Sixnet.Cache.SortedSet.Response;
using Sixnet.Cache.String;
using Sixnet.Cache.String.Response;
using Sixnet.Serialization;

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
        public static async Task<StringSetResponse> SetDataAsync<T>(CacheKey key, T data, DateTimeOffset? absoluteExpiration = null, CacheSetWhen when = CacheSetWhen.Always, CacheObject cacheObject = null)
        {
            var value = SixnetJsonSerializer.Serialize(data);
            if (string.IsNullOrWhiteSpace(value))
            {
                return CacheResponse.FailResponse<StringSetResponse>(SixnetCacheCodes.ValuesIsNullOrEmpty);
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
        public static async Task<StringSetResponse> SetDataByRelativeExpirationAsync<T>(CacheKey key, T data, TimeSpan? absoluteExpirationRelativeToNow = null, bool slidingExpiration = true, CacheSetWhen when = CacheSetWhen.Always, CacheObject cacheObject = null)
        {
            var value = SixnetJsonSerializer.Serialize(data);
            if (string.IsNullOrWhiteSpace(value))
            {
                return CacheResponse.FailResponse<StringSetResponse>(SixnetCacheCodes.ValuesIsNullOrEmpty);
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
            /// <param name="stringSetRangeOptions">Set range options</param>
            /// <returns>Return cache set result</returns>
            public static async Task<StringSetRangeResponse> SetRangeAsync(StringSetRangeOptions stringSetRangeOptions)
            {
                return await ExecuteCommandAsync(stringSetRangeOptions).ConfigureAwait(false);
            }

            #endregion

            #region Set bit

            /// <summary>
            /// To set or clear the bit (bit) at the specified offset for the string value stored by the key,
            /// When the key does not exist, a new string value is automatically generated.
            /// The string is stretched (grown) to ensure that it can hold the value at the specified offset. When the string value is stretched, the empty space is filled with 0
            /// The offset parameter must be greater than or equal to 0 and less than 2^32
            /// </summary>
            /// <param name="stringSetBitOptions">Set bit options</param>
            /// <returns>Return cache set result</returns>
            public static async Task<StringSetBitResponse> SetBitAsync(StringSetBitOptions stringSetBitOptions)
            {
                return await ExecuteCommandAsync(stringSetBitOptions).ConfigureAwait(false);
            }

            #endregion

            #region Set

            /// <summary>
            /// Associate the string value value to the key
            /// If the key already holds other values, SET will overwrite the old values, regardless of the type
            /// When the SET command sets a key with a time to live (TTL), the original TTL of the key will be cleared
            /// </summary>
            /// <param name="stringSetOptions">String set options</param>
            /// <returns>Return cache result</returns>
            public static async Task<StringSetResponse> SetAsync(StringSetOptions stringSetOptions)
            {
                return await ExecuteCommandAsync(stringSetOptions).ConfigureAwait(false);
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
            public static async Task<StringSetResponse> SetAsync(CacheKey key, string value, DateTimeOffset? absoluteExpiration = null, CacheSetWhen when = CacheSetWhen.Always, CacheObject cacheObject = null)
            {
                return await SetAsync(new StringSetOptions()
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
            public static async Task<StringSetResponse> SetByRelativeExpirationAsync(CacheKey key, string value, TimeSpan? absoluteExpirationRelativeToNow = null, bool slidingExpiration = true, CacheSetWhen when = CacheSetWhen.Always, CacheObject cacheObject = null)
            {
                return await SetAsync(new StringSetOptions()
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
            /// <param name="stringLengthOptions">String length options</param>
            /// <returns>Return cache result</returns>
            public static async Task<StringLengthResponse> LengthAsync(StringLengthOptions stringLengthOptions)
            {
                return await ExecuteCommandAsync(stringLengthOptions).ConfigureAwait(false);
            }

            #endregion

            #region Increment

            /// <summary>
            /// Add one to the numeric value stored by the key
            /// If the key key does not exist, then its value will be initialized to 0 first, and then execute the command
            /// If the value stored by the key cannot be interpreted as a number, the command will return an error
            /// </summary>
            /// <param name="stringIncrementOptions">String increment options</param>
            /// <returns>Return cache result</returns>
            public static async Task<StringIncrementResponse> IncrementAsync(StringIncrementOptions stringIncrementOptions)
            {
                return await ExecuteCommandAsync(stringIncrementOptions).ConfigureAwait(false);
            }

            #endregion

            #region Get with expiry

            /// <summary>
            /// Returns the string value associated with the key
            /// If the key key does not exist, then return an empty string, otherwise, return the value of the key key
            /// If the value of key is not of type string, then return an error.
            /// </summary>
            /// <param name="stringGetWithExpiryOptions">String get with expiry options</param>
            /// <returns>Return cache result</returns>
            public static async Task<StringGetWithExpiryResponse> GetWithExpiryAsync(StringGetWithExpiryOptions stringGetWithExpiryOptions)
            {
                return await ExecuteCommandAsync(stringGetWithExpiryOptions).ConfigureAwait(false);
            }

            #endregion

            #region Get set

            /// <summary>
            /// Set the value of the key key to value and return the old value of the key key before it is set
            /// When the key key exists but is not a string type, the command returns an error
            /// </summary>
            /// <param name="stringGetSetOptions">String get set options</param>
            /// <returns>Return cache result</returns>
            public static async Task<StringGetSetResponse> GetSetAsync(StringGetSetOptions stringGetSetOptions)
            {
                return await ExecuteCommandAsync(stringGetSetOptions).ConfigureAwait(false);
            }

            #endregion

            #region Get range

            /// <summary>
            /// Returns the specified part of the string value stored by the key key, the range of the string interception is determined by the two offsets of start and end (including start and end)
            /// Negative offset means counting from the end of the string, -1 means the last character, -2 means the penultimate character, and so on
            /// </summary>
            /// <param name="stringGetRangeOptions">String get range options</param>
            /// <returns>Return cache result</returns>
            public static async Task<StringGetRangeResponse> GetRangeAsync(StringGetRangeOptions stringGetRangeOptions)
            {
                return await ExecuteCommandAsync(stringGetRangeOptions).ConfigureAwait(false);
            }

            #endregion

            #region Get bit

            /// <summary>
            /// For the string value stored in key, get the bit at the specified offset
            /// When offset is greater than the length of the string value, or the key does not exist, return 0
            /// </summary>
            /// <param name="stringGetBitOptions">String get bit options</param>
            /// <returns>Return cache result</returns>
            public static async Task<StringGetBitResponse> GetBitAsync(StringGetBitOptions stringGetBitOptions)
            {
                return await ExecuteCommandAsync(stringGetBitOptions).ConfigureAwait(false);
            }

            #endregion

            #region Get

            /// <summary>
            /// Returns the value of the given string key or keys
            /// </summary>
            /// <param name="stringGetOptions">String get options</param>
            /// <returns>Return cache result</returns>
            public static async Task<StringGetResponse> GetAsync(StringGetOptions stringGetOptions)
            {
                return await ExecuteCommandAsync(stringGetOptions).ConfigureAwait(false);
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
                var result = await GetAsync(new StringGetOptions()
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
            /// <param name="stringDecrementOptions">String decrement options</param>
            /// <returns>Return cache result</returns>
            public static async Task<StringDecrementResponse> DecrementAsync(StringDecrementOptions stringDecrementOptions)
            {
                return await ExecuteCommandAsync(stringDecrementOptions).ConfigureAwait(false);
            }

            #endregion

            #region Bit position

            /// <summary>
            /// Returns the position of the first binary bit in the bitmap
            /// By default, the command will detect the entire bitmap, but the user can also specify the range to be detected through the optional start parameter and end parameter
            /// </summary>
            /// <param name="stringBitPositionOptions">String bit position options</param>
            /// <returns>Return cache result</returns>
            public static async Task<StringBitPositionResponse> BitPositionAsync(StringBitPositionOptions stringBitPositionOptions)
            {
                return await ExecuteCommandAsync(stringBitPositionOptions).ConfigureAwait(false);
            }

            #endregion

            #region Bit operation

            /// <summary>
            /// Perform bit operations on one or more string keys that hold binary bits, and save the result to destkey
            /// Except NOT operation, other operations can accept one or more keys as input
            /// </summary>
            /// <param name="stringBitOperationOptions">String bit operation options</param>
            /// <returns>Return cache result</returns>
            public static async Task<StringBitOperationResponse> BitOperationAsync(StringBitOperationOptions stringBitOperationOptions)
            {
                return await ExecuteCommandAsync(stringBitOperationOptions).ConfigureAwait(false);
            }

            #endregion

            #region Bit count

            /// <summary>
            /// Calculate the number of bits set to 1 in a given string.
            /// Under normal circumstances, the given entire string will be counted, by specifying additional start or end parameters, you can make the count only on a specific bit
            /// </summary>
            /// <param name="stringBitCountOptions">String bit count options</param>
            /// <returns>Return cache result</returns>
            public static async Task<StringBitCountResponse> BitCountAsync(StringBitCountOptions stringBitCountOptions)
            {
                return await ExecuteCommandAsync(stringBitCountOptions).ConfigureAwait(false);
            }

            #endregion

            #region Append

            /// <summary>
            /// If the key key already exists and its value is a string, the value will be appended to the end of the key key's existing value
            /// If the key does not exist, simply set the value of the key key to value
            /// </summary>
            /// <param name="stringAppendOptions">String append options</param>
            /// <returns>Return cache result</returns>
            public static async Task<StringAppendResponse> AppendAsync(StringAppendOptions stringAppendOptions)
            {
                return await ExecuteCommandAsync(stringAppendOptions).ConfigureAwait(false);
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
            /// <param name="listTrimOptions">List trim options</param>
            /// <returns>Return cache result</returns>
            public static async Task<ListTrimResponse> TrimAsync(ListTrimOptions listTrimOptions)
            {
                return await ExecuteCommandAsync(listTrimOptions).ConfigureAwait(false);
            }

            #endregion

            #region Set by index

            /// <summary>
            /// Set the value of the element whose index of the list key is index to value
            /// When the index parameter is out of range, or an operation is performed on an empty list (the key does not exist), an error is returned
            /// </summary>
            /// <param name="listSetByIndexOptions">List set by index options</param>
            /// <returns>Return cache result</returns>
            public static async Task<ListSetByIndexResponse> SetByIndexAsync(ListSetByIndexOptions listSetByIndexOptions)
            {
                return await ExecuteCommandAsync(listSetByIndexOptions).ConfigureAwait(false);
            }

            #endregion

            #region Right push

            /// <summary>
            /// Insert one or more values ​​into the end of the list key (far right).
            /// If there are multiple value values, then each value value is inserted into the end of the table in order from left to right
            /// </summary>
            /// <param name="listRightPushOptions">List right push options</param>
            /// <returns>Return cache result</returns>
            public static async Task<ListRightPushResponse> RightPushAsync(ListRightPushOptions listRightPushOptions)
            {
                return await ExecuteCommandAsync(listRightPushOptions).ConfigureAwait(false);
            }

            #endregion

            #region Right pop left push

            /// <summary>
            /// Pop the last element (tail element) in the list source and return it to the client
            /// Insert the element popped by source into the destination list as the head element of the destination list
            /// </summary>
            /// <param name="listRightPopLeftPushOptions">List right pop left push options</param>
            /// <returns>Return cache result</returns>
            public static async Task<ListRightPopLeftPushResponse> RightPopLeftPushAsync(ListRightPopLeftPushOptions listRightPopLeftPushOptions)
            {
                return await ExecuteCommandAsync(listRightPopLeftPushOptions).ConfigureAwait(false);
            }

            #endregion

            #region Right pop

            /// <summary>
            /// Remove and return the tail element of the list key.
            /// </summary>
            /// <param name="listRightPopOptions">List right pop options</param>
            /// <returns>Return cache result</returns>
            public static async Task<ListRightPopResponse> RightPopAsync(ListRightPopOptions listRightPopOptions)
            {
                return await ExecuteCommandAsync(listRightPopOptions).ConfigureAwait(false);
            }

            #endregion

            #region Remove

            /// <summary>
            /// According to the value of the parameter count, remove the elements equal to the parameter value in the list
            /// count is greater than 0: search from the beginning of the table to the end of the table, remove the elements equal to value, the number is count
            /// count is less than 0: search from the end of the table to the head of the table, remove the elements equal to value, the number is the absolute value of count
            /// count equals 0: remove all values ​​in the table that are equal to value
            /// </summary>
            /// <param name="listRemoveOptions">List remove options</param>
            /// <returns>Return cache result</returns>
            public static async Task<ListRemoveResponse> RemoveAsync(ListRemoveOptions listRemoveOptions)
            {
                return await ExecuteCommandAsync(listRemoveOptions).ConfigureAwait(false);
            }

            #endregion

            #region Range

            /// <summary>
            /// Return the elements in the specified interval in the list key, the interval is specified by the offset start and stop
            /// The subscript (index) parameters start and stop are both based on 0, that is, 0 represents the first element of the list, 1 represents the second element of the list, and so on
            /// You can also use negative subscripts, with -1 for the last element of the list, -2 for the penultimate element of the list, and so on
            /// </summary>
            /// <param name="listRangeOptions">List range options</param>
            /// <returns>Return cache result</returns>
            public static async Task<ListRangeResponse> RangeAsync(ListRangeOptions listRangeOptions)
            {
                return await ExecuteCommandAsync(listRangeOptions).ConfigureAwait(false);
            }

            #endregion

            #region Length

            /// <summary>
            /// Return the length of the list key
            /// If the key does not exist, the key is interpreted as an empty list and returns 0 
            /// If key is not a list type, return an error.
            /// </summary>
            /// <param name="listLengthOptions">List length options</param>
            /// <returns>Return cache result</returns>
            public static async Task<ListLengthResponse> LengthAsync(ListLengthOptions listLengthOptions)
            {
                return await ExecuteCommandAsync(listLengthOptions).ConfigureAwait(false);
            }

            #endregion

            #region Left push

            /// <summary>
            /// Insert one or more values ​​into the header of the list key
            /// If the key does not exist, an empty list will be created and the operation will be performed
            /// </summary>
            /// <param name="listLeftPushOptions">List left push options</param>
            /// <returns>Return cache result</returns>
            public static async Task<ListLeftPushResponse> LeftPushAsync(ListLeftPushOptions listLeftPushOptions)
            {
                return await ExecuteCommandAsync(listLeftPushOptions).ConfigureAwait(false);
            }

            #endregion

            #region Left pop

            /// <summary>
            /// Remove and return the head element of the list key
            /// </summary>
            /// <param name="listLeftPopOptions">List left pop options</param>
            /// <returns>Return cache result</returns>
            public static async Task<ListLeftPopResponse> LeftPopAsync(ListLeftPopOptions listLeftPopOptions)
            {
                return await ExecuteCommandAsync(listLeftPopOptions).ConfigureAwait(false);
            }

            #endregion

            #region Insert before

            /// <summary>
            /// Insert the value value into the list key, before the value pivot
            /// </summary>
            /// <param name="listInsertBeforeOptions">List insert before options</param>
            /// <returns>Return cache result</returns>
            public static async Task<ListInsertBeforeResponse> InsertBeforeAsync(ListInsertBeforeOptions listInsertBeforeOptions)
            {
                return await ExecuteCommandAsync(listInsertBeforeOptions).ConfigureAwait(false);
            }

            #endregion

            #region Insert after

            /// <summary>
            /// Insert the value value into the list key, after the value pivot
            /// </summary>
            /// <param name="listInsertAfterOptions">List insert after options</param>
            /// <returns>Return cache result</returns>
            public static async Task<ListInsertAfterResponse> InsertAfterAsync(ListInsertAfterOptions listInsertAfterOptions)
            {
                return await ExecuteCommandAsync(listInsertAfterOptions).ConfigureAwait(false);
            }

            #endregion

            #region Get by index

            /// <summary>
            /// Return the element with index index in the list key
            /// The subscript (index) parameters start and stop are both based on 0, that is, 0 represents the first element of the list, 1 represents the second element of the list, and so on
            /// You can also use negative subscripts, with -1 for the last element of the list, -2 for the penultimate element of the list, and so on
            /// </summary>
            /// <param name="listGetByIndexOptions">List get by index options</param>
            /// <returns>Return cache result</returns>
            public static async Task<ListGetByIndexResponse> GetByIndexAsync(ListGetByIndexOptions listGetByIndexOptions)
            {
                return await ExecuteCommandAsync(listGetByIndexOptions).ConfigureAwait(false);
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
            /// <param name="hashValuesOptions">Hash values options</param>
            /// <returns>Return cache result</returns>
            public static async Task<HashValuesResponse> ValuesAsync(HashValuesOptions hashValuesOptions)
            {
                return await ExecuteCommandAsync(hashValuesOptions).ConfigureAwait(false);
            }

            #endregion

            #region Set

            /// <summary>
            /// Set the value of the field field in the hash table hash to value
            /// If the given hash table does not exist, then a new hash table will be created and perform the operation
            /// If the field field already exists in the hash table, its old value will be overwritten by the new value
            /// </summary>
            /// <param name="hashSetOptions">Hash set options</param>
            /// <returns>Return cache result</returns>
            public static async Task<HashSetResponse> SetAsync(HashSetOptions hashSetOptions)
            {
                return await ExecuteCommandAsync(hashSetOptions).ConfigureAwait(false);
            }

            #endregion

            #region Length

            /// <summary>
            /// returns the number of fields in the hash table key
            /// </summary>
            /// <param name="hashLengthOptions">Hash length options</param>
            /// <returns>Return cache result</returns>
            public static async Task<HashLengthResponse> LengthAsync(HashLengthOptions hashLengthOptions)
            {
                return await ExecuteCommandAsync(hashLengthOptions).ConfigureAwait(false);
            }

            #endregion

            #region Keys

            /// <summary>
            /// Return all keys in the hash table key
            /// </summary>
            /// <param name="hashKeysOptions">Hash keys options</param>
            /// <returns>Return cache result</returns>
            public static async Task<HashKeysResponse> KeysAsync(HashKeysOptions hashKeysOptions)
            {
                return await ExecuteCommandAsync(hashKeysOptions).ConfigureAwait(false);
            }

            #endregion

            #region Increment

            /// <summary>
            /// Add incremental increment to the value of the field field in the hash table key
            /// </summary>
            /// <param name="hashIncrementOptions">Hash increment options</param>
            /// <returns>Return cache result</returns>
            public static async Task<HashIncrementResponse> IncrementAsync(HashIncrementOptions hashIncrementOptions)
            {
                return await ExecuteCommandAsync(hashIncrementOptions).ConfigureAwait(false);
            }

            #endregion

            #region Get

            /// <summary>
            /// Returns the value of the given field in the hash table
            /// </summary>
            /// <param name="hashGetOptions">Hash get options</param>
            /// <returns>Return cache result</returns>
            public static async Task<HashGetResponse> GetAsync(HashGetOptions hashGetOptions)
            {
                return await ExecuteCommandAsync(hashGetOptions).ConfigureAwait(false);
            }

            #endregion

            #region Get all

            /// <summary>
            /// Return the values ​​of all fields in the hash table key
            /// </summary>
            /// <param name="hashGetAllOptions">Hash get all options</param>
            /// <returns>Return cache result</returns>
            public static async Task<HashGetAllResponse> GetAllAsync(HashGetAllOptions hashGetAllOptions)
            {
                return await ExecuteCommandAsync(hashGetAllOptions).ConfigureAwait(false);
            }

            #endregion

            #region Exist

            /// <summary>
            /// Check if the given field exists in the hash of the hash table
            /// </summary>
            /// <param name="hashExistsOptions">Hash exists options</param>
            /// <returns>Return cache result</returns>
            public static async Task<HashExistsResponse> ExistAsync(HashExistsOptions hashExistsOptions)
            {
                return await ExecuteCommandAsync(hashExistsOptions).ConfigureAwait(false);
            }

            #endregion

            #region Delete

            /// <summary>
            /// Delete one or more specified fields in the hash table key, the non-existing fields will be ignored
            /// </summary>
            /// <param name="hashDeleteOptions">Hash delete options</param>
            /// <returns>Return cache result</returns>
            public static async Task<HashDeleteResponse> DeleteAsync(HashDeleteOptions hashDeleteOptions)
            {
                return await ExecuteCommandAsync(hashDeleteOptions).ConfigureAwait(false);
            }

            #endregion

            #region Decrement

            /// <summary>
            /// Is the value of the field in the hash table key minus the increment increment
            /// </summary>
            /// <param name="hashDecrementOptions">Hash decrement options</param>
            /// <returns>Return cache result</returns>
            public static async Task<HashDecrementResponse> DecrementAsync(HashDecrementOptions hashDecrementOptions)
            {
                return await ExecuteCommandAsync(hashDecrementOptions).ConfigureAwait(false);
            }

            #endregion

            #region Scan

            /// <summary>
            /// Each element returned is a key-value pair, a key-value pair consists of a key and a value
            /// </summary>
            /// <param name="hashScanOptions">Hash scan options</param>
            /// <returns>Return cache result</returns>
            public static async Task<HashScanResponse> ScanAsync(HashScanOptions hashScanOptions)
            {
                return await ExecuteCommandAsync(hashScanOptions).ConfigureAwait(false);
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
            /// <param name="setRemoveOptions">Set remove options</param>
            /// <returns>Return cache result</returns>
            public static async Task<SetRemoveResponse> RemoveAsync(SetRemoveOptions setRemoveOptions)
            {
                return await ExecuteCommandAsync(setRemoveOptions).ConfigureAwait(false);
            }

            #endregion

            #region Random members

            /// <summary>
            /// Then return a set of random elements in the collection
            /// </summary>
            /// <param name="setRandomMembersOptions">Set random members options</param>
            /// <returns>Return cache result</returns>
            public static async Task<SetRandomMembersResponse> RandomMembersAsync(SetRandomMembersOptions setRandomMembersOptions)
            {
                return await ExecuteCommandAsync(setRandomMembersOptions).ConfigureAwait(false);
            }

            #endregion

            #region Random member

            /// <summary>
            /// Returns a random element in the collection
            /// </summary>
            /// <param name="setRandomMemberOptions">Set random member options</param>
            /// <returns>Return cache result</returns>
            public static async Task<SetRandomMemberResponse> RandomMemberAsync(SetRandomMemberOptions setRandomMemberOptions)
            {
                return await ExecuteCommandAsync(setRandomMemberOptions).ConfigureAwait(false);
            }

            #endregion

            #region Pop

            /// <summary>
            /// Remove and return a random element in the collection
            /// </summary>
            /// <param name="setPopOptions">Set pop options</param>
            /// <returns>Return cache result</returns>
            public static async Task<SetPopResponse> PopAsync(SetPopOptions setPopOptions)
            {
                return await ExecuteCommandAsync(setPopOptions).ConfigureAwait(false);
            }

            #endregion

            #region Move

            /// <summary>
            /// Move the member element from the source collection to the destination collection
            /// </summary>
            /// <param name="setMoveOptions">Set move options</param>
            /// <returns>Return cache result</returns>
            public static async Task<SetMoveResponse> MoveAsync(SetMoveOptions setMoveOptions)
            {
                return await ExecuteCommandAsync(setMoveOptions).ConfigureAwait(false);
            }

            #endregion

            #region Members

            /// <summary>
            /// Return all members in the collection key
            /// </summary>
            /// <param name="setMembersOptions">Set members options</param>
            /// <returns>Return cache result</returns>
            public static async Task<SetMembersResponse> MembersAsync(SetMembersOptions setMembersOptions)
            {
                return await ExecuteCommandAsync(setMembersOptions).ConfigureAwait(false);
            }

            #endregion

            #region Length

            /// <summary>
            /// Returns the number of elements in the collection
            /// </summary>
            /// <param name="setLengthOptions">Set length options</param>
            /// <returns>Return cache result</returns>
            public static async Task<SetLengthResponse> LengthAsync(SetLengthOptions setLengthOptions)
            {
                return await ExecuteCommandAsync(setLengthOptions).ConfigureAwait(false);
            }

            #endregion

            #region Contains

            /// <summary>
            /// Determine whether the member element is a member of the set key
            /// </summary>
            /// <param name="setContainsOptions">Set contaims options</param>
            /// <returns>Return cache result</returns>
            public static async Task<SetContainsResponse> ContainsAsync(SetContainsOptions setContainsOptions)
            {
                return await ExecuteCommandAsync(setContainsOptions).ConfigureAwait(false);
            }

            #endregion

            #region Combine

            /// <summary>
            /// According to the operation mode, return the processing results of multiple collections
            /// </summary>
            /// <param name="setCombineOptions">Set combine options</param>
            /// <returns>Return cache result</returns>
            public static async Task<SetCombineResponse> CombineAsync(SetCombineOptions setCombineOptions)
            {
                return await ExecuteCommandAsync(setCombineOptions).ConfigureAwait(false);
            }

            #endregion

            #region Combine and store

            /// <summary>
            /// Return the processing results of multiple collections according to the operation mode, and store the results to the specified key value at the same time
            /// </summary>
            /// <param name="setCombineAndStoreOptions">Set combine and store options</param>
            /// <returns>Return cache result</returns>
            public static async Task<SetCombineAndStoreResponse> CombineAndStoreAsync(SetCombineAndStoreOptions setCombineAndStoreOptions)
            {
                return await ExecuteCommandAsync(setCombineAndStoreOptions).ConfigureAwait(false);
            }

            #endregion

            #region Add

            /// <summary>
            /// Add one or more member elements to the collection key, the member elements already in the collection will be ignored
            /// If the key does not exist, create a collection that contains only the member element as a member
            /// </summary>
            /// <param name="setAddOptions">Set add options</param>
            /// <returns>Return cache result</returns>
            public static async Task<SetAddResponse> AddAsync(SetAddOptions setAddOptions)
            {
                return await ExecuteCommandAsync(setAddOptions).ConfigureAwait(false);
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
            /// <param name="sortedSetScoreOptions">Sorted set score options</param>
            /// <returns>Return cache result</returns>
            public static async Task<SortedSetScoreResponse> ScoreAsync(SortedSetScoreOptions sortedSetScoreOptions)
            {
                return await ExecuteCommandAsync(sortedSetScoreOptions).ConfigureAwait(false);
            }

            #endregion

            #region Remove range by value

            /// <summary>
            /// Remove the elements in the specified range after sorting the elements
            /// </summary>
            /// <param name="sortedSetRemoveRangeByValueOptions">Sorted set remove range by value options</param>
            /// <returns>Return cache result</returns>
            public static async Task<SortedSetRemoveRangeByValueResponse> RemoveRangeByValueAsync(SortedSetRemoveRangeByValueOptions sortedSetRemoveRangeByValueOptions)
            {
                return await ExecuteCommandAsync(sortedSetRemoveRangeByValueOptions).ConfigureAwait(false);
            }

            #endregion

            #region Remove range by score

            /// <summary>
            /// Remove all members in the ordered set key whose score value is between min and max
            /// </summary>
            /// <param name="sortedSetRemoveRangeByScoreOptions">Sorted set remove range by score options</param>
            /// <returns>Return cache result</returns>
            public static async Task<SortedSetRemoveRangeByScoreResponse> RemoveRangeByScoreAsync(SortedSetRemoveRangeByScoreOptions sortedSetRemoveRangeByScoreOptions)
            {
                return await ExecuteCommandAsync(sortedSetRemoveRangeByScoreOptions).ConfigureAwait(false);
            }

            #endregion

            #region Remove range by rank

            /// <summary>
            /// Remove all members in the specified rank range in the ordered set key
            /// The subscript parameters start and stop are both based on 0, that is, 0 means the first member of the ordered set, 1 means the second member of the ordered set, and so on. 
            /// You can also use negative subscripts, with -1 for the last member, -2 for the penultimate member, and so on.
            /// </summary>
            /// <param name="sortedSetRemoveRangeByRankOptions">Sorted set range by rank options</param>
            /// <returns>Return cache result</returns>
            public static async Task<SortedSetRemoveRangeByRankResponse> RemoveRangeByRankAsync(SortedSetRemoveRangeByRankOptions sortedSetRemoveRangeByRankOptions)
            {
                return await ExecuteCommandAsync(sortedSetRemoveRangeByRankOptions).ConfigureAwait(false);
            }

            #endregion

            #region Remove

            /// <summary>
            /// Remove the specified element in the ordered collection
            /// </summary>
            /// <param name="sortedSetRemoveOptions">Sorted set remove options</param>
            /// <returns>Return cache result</returns>
            public static async Task<SortedSetRemoveResponse> RemoveAsync(SortedSetRemoveOptions sortedSetRemoveOptions)
            {
                return await ExecuteCommandAsync(sortedSetRemoveOptions).ConfigureAwait(false);
            }

            #endregion

            #region Rank

            /// <summary>
            /// Returns the ranking of the member member in the ordered set key. The members of the ordered set are arranged in order of increasing score value (from small to large) by default
            /// The ranking is based on 0, that is, the member with the smallest score is ranked 0
            /// </summary>
            /// <param name="sortedSetRankOptions">Sorted set rank options</param>
            /// <returns>Return cache result</returns>
            public static async Task<SortedSetRankResponse> RankAsync(SortedSetRankOptions sortedSetRankOptions)
            {
                return await ExecuteCommandAsync(sortedSetRankOptions).ConfigureAwait(false);
            }

            #endregion

            #region Range by value

            /// <summary>
            /// Returns the elements in the ordered set between min and max
            /// </summary>
            /// <param name="sortedSetRangeByValueOptions">Sorted set range by value options</param>
            /// <returns>Return cache result</returns>
            public static async Task<SortedSetRangeByValueResponse> RangeByValueAsync(SortedSetRangeByValueOptions sortedSetRangeByValueOptions)
            {
                return await ExecuteCommandAsync(sortedSetRangeByValueOptions).ConfigureAwait(false);
            }

            #endregion

            #region Range by score with scores

            /// <summary>
            /// Returns the value and score of the members in the specified interval in the ordered set key, the positions are arranged according to score
            /// </summary>
            /// <param name="sortedSetRangeByScoreWithScoresOptions">Sorted set range by score with scores options</param>
            /// <returns>Return cache result</returns>
            public static async Task<SortedSetRangeByScoreWithScoresResponse> RangeByScoreWithScoresAsync(SortedSetRangeByScoreWithScoresOptions sortedSetRangeByScoreWithScoresOptions)
            {
                return await ExecuteCommandAsync(sortedSetRangeByScoreWithScoresOptions).ConfigureAwait(false);
            }

            #endregion

            #region Range by score

            /// <summary>
            /// Return the value of the members in the specified interval in the ordered set key, the position is arranged by score
            /// </summary>
            /// <param name="sortedSetRangeByScoreOptions">Sorted set range by score options</param>
            /// <returns>Return cache result</returns>
            public static async Task<SortedSetRangeByScoreResponse> RangeByScoreAsync(SortedSetRangeByScoreOptions sortedSetRangeByScoreOptions)
            {
                return await ExecuteCommandAsync(sortedSetRangeByScoreOptions).ConfigureAwait(false);
            }

            #endregion

            #region Range by rank with scores

            /// <summary>
            /// Returns the value and score of the members in the specified interval in the ordered set key, the positions are arranged according to score
            /// </summary>
            /// <param name="sortedSetRangeByRankWithScoresOptions">Sorted set range by rank with scores options</param>
            /// <returns>Return cache result</returns>
            public static async Task<SortedSetRangeByRankWithScoresResponse> RangeByRankWithScoresAsync(SortedSetRangeByRankWithScoresOptions sortedSetRangeByRankWithScoresOptions)
            {
                return await ExecuteCommandAsync(sortedSetRangeByRankWithScoresOptions).ConfigureAwait(false);
            }

            #endregion

            #region Range by rank

            /// <summary>
            /// Return the value of the members in the specified interval in the ordered set key, the positions are arranged by score
            /// </summary>
            /// <param name="sortedSetRangeByRankOptions">Sorted set range by rank options</param>
            /// <returns>sorted set range by rank response</returns>
            public static async Task<SortedSetRangeByRankResponse> RangeByRankAsync(SortedSetRangeByRankOptions sortedSetRangeByRankOptions)
            {
                return await ExecuteCommandAsync(sortedSetRangeByRankOptions).ConfigureAwait(false);
            }

            #endregion

            #region Length by value

            /// <summary>
            /// Returns the number of members whose value is between min and max in the ordered set key
            /// </summary>
            /// <param name="sortedSetLengthByValueOptions">Sorted set length by value options</param>
            /// <returns>Return cache result</returns>
            public static async Task<SortedSetLengthByValueResponse> LengthByValueAsync(SortedSetLengthByValueOptions sortedSetLengthByValueOptions)
            {
                return await ExecuteCommandAsync(sortedSetLengthByValueOptions).ConfigureAwait(false);
            }

            #endregion

            #region Length

            /// <summary>
            /// Returns the number of members in the ordered set key whose score value is between min and max (including the score value equal to min or max by default)
            /// </summary>
            /// <param name="sortedSetLengthOptions">Sorted set length options</param>
            /// <returns>Return cache result</returns>
            public static async Task<SortedSetLengthResponse> LengthAsync(SortedSetLengthOptions sortedSetLengthOptions)
            {
                return await ExecuteCommandAsync(sortedSetLengthOptions).ConfigureAwait(false);
            }

            #endregion

            #region Increment

            /// <summary>
            /// Add the incremental increment to the score value of the member of the ordered set key
            /// </summary>
            /// <param name="sortedSetIncrementOptions">Sorted set increment options</param>
            /// <returns>Return cache result</returns>
            public static async Task<SortedSetIncrementResponse> IncrementAsync(SortedSetIncrementOptions sortedSetIncrementOptions)
            {
                return await ExecuteCommandAsync(sortedSetIncrementOptions).ConfigureAwait(false);
            }

            #endregion

            #region Decrement

            /// <summary>
            /// is the score value of the member of the ordered set key minus the increment increment
            /// </summary>
            /// <param name="sortedSetDecrementOptions">Sorted set decrement options</param>
            /// <returns>Return cache result</returns>
            public static async Task<SortedSetDecrementResponse> DecrementAsync(SortedSetDecrementOptions sortedSetDecrementOptions)
            {
                return await ExecuteCommandAsync(sortedSetDecrementOptions).ConfigureAwait(false);
            }

            #endregion

            #region Combine and store

            /// <summary>
            /// Aggregate multiple ordered collections and save to destination
            /// </summary>
            /// <param name="sortedSetCombineAndStoreOptions">Sorted set combine and store options</param>
            /// <returns>Return cache result</returns>
            public static async Task<SortedSetCombineAndStoreResponse> CombineAndStoreAsync(SortedSetCombineAndStoreOptions sortedSetCombineAndStoreOptions)
            {
                return await ExecuteCommandAsync(sortedSetCombineAndStoreOptions).ConfigureAwait(false);
            }

            #endregion

            #region Add

            /// <summary>
            /// Add one or more member elements and their score values ​​to the ordered set key
            /// </summary>
            /// <param name="sortedSetAddOptions">Sorted set add options</param>
            /// <returns>Return cache result</returns>
            public static async Task<SortedSetAddResponse> AddAsync(SortedSetAddOptions sortedSetAddOptions)
            {
                return await ExecuteCommandAsync(sortedSetAddOptions).ConfigureAwait(false);
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
            /// <param name="sortOptions">Sort options</param>
            /// <returns>Return cache result</returns>
            public static async Task<SortResponse> SortAsync(SortOptions sortOptions)
            {
                return await ExecuteCommandAsync(sortOptions).ConfigureAwait(false);
            }

            #endregion

            #region Sort and store

            /// <summary>
            /// Return or save the sorted elements in the given list, collection, ordered set key, and save to the specified key value
            /// </summary>
            /// <param name="sortAndStoreOptions">Sort and store options</param>
            /// <returns>Return cache result</returns>
            public static async Task<SortAndStoreResponse> SortAndStoreAsync(SortAndStoreOptions sortAndStoreOptions)
            {
                return await ExecuteCommandAsync(sortAndStoreOptions).ConfigureAwait(false);
            }

            #endregion

            #region Type

            /// <summary>
            /// Returns the type of value stored by key
            /// </summary>
            /// <param name="typeOptions">type options</param>
            /// <returns>Return cache result</returns>
            public static async Task<TypeResponse> TypeAsync(TypeOptions typeOptions)
            {
                return await ExecuteCommandAsync(typeOptions).ConfigureAwait(false);
            }

            #endregion

            #region Time to live

            /// <summary>
            /// Return the remaining time to live for a given key (TTL, time to live)
            /// </summary>
            /// <param name="timeToLiveOptions">Time to live options</param>
            /// <returns>Return cache result</returns>
            public static async Task<TimeToLiveResponse> TimeToLiveAsync(TimeToLiveOptions timeToLiveOptions)
            {
                return await ExecuteCommandAsync(timeToLiveOptions).ConfigureAwait(false);
            }

            #endregion

            #region Restore

            /// <summary>
            /// Deserialize the given serialized value and associate it with the given key
            /// </summary>
            /// <param name="restoreOptions">Restore options</param>
            /// <returns> Return cache result </returns>
            public static async Task<RestoreResponse> RestoreAsync(RestoreOptions restoreOptions)
            {
                return await ExecuteCommandAsync(restoreOptions).ConfigureAwait(false);
            }

            #endregion

            #region Rename

            /// <summary>
            /// Rename the key to newkey
            /// When the key and newkey are the same, or the key does not exist, an error is returned
            /// When newkey already exists, it will overwrite the old value
            /// </summary>
            /// <param name="renameOptions">Rename options</param>
            /// <returns>Return cache result</returns>
            public static async Task<RenameResponse> RenameAsync(RenameOptions renameOptions)
            {
                return await ExecuteCommandAsync(renameOptions).ConfigureAwait(false);
            }

            #endregion

            #region Random

            /// <summary>
            /// randomly return (do not delete) a key
            /// </summary>
            /// <param name="randomOptions">Random options</param>
            /// <returns>Return cache result</returns>
            public static async Task<RandomResponse> KeyRandomAsync(RandomOptions randomOptions)
            {
                return await ExecuteCommandAsync(randomOptions).ConfigureAwait(false);
            }

            #endregion

            #region Persist

            /// <summary>
            /// Remove the survival time of a given key, and convert this key from "volatile" (with survival time key) to "persistent" (a key with no survival time and never expires)
            /// </summary>
            /// <param name="persistOptions">Persist options</param>
            /// <returns>Return cache result</returns>
            public static async Task<PersistResponse> PersistAsync(PersistOptions persistOptions)
            {
                return await ExecuteCommandAsync(persistOptions).ConfigureAwait(false);
            }

            #endregion

            #region Move

            /// <summary>
            /// Move the key of the current database to the given database
            /// </summary>
            /// <param name="moveOptions">Move options</param>
            /// <returns>Return cache result</returns>
            public static async Task<MoveResponse> MoveAsync(MoveOptions moveOptions)
            {
                return await ExecuteCommandAsync(moveOptions).ConfigureAwait(false);
            }

            #endregion

            #region Migrate

            /// <summary>
            /// Transfer the key atomically from the current instance to the specified database of the target instance. Once the transfer is successful, the key is guaranteed to appear on the target instance, and the key on the current instance will be deleted.
            /// </summary>
            /// <param name="migrateOptions">Migrate options</param>
            /// <returns>Return cache result</returns>
            public static async Task<MigrateKeyResponse> MigrateAsync(MigrateKeyOptions migrateOptions)
            {
                return await ExecuteCommandAsync(migrateOptions).ConfigureAwait(false);
            }

            #endregion

            #region Expire

            /// <summary>
            /// Set the survival time for the given key. When the key expires (the survival time is 0), it will be automatically deleted
            /// </summary>
            /// <param name="expireOptions">Expire options</param>
            /// <returns>Return cache result</returns>
            public static async Task<ExpireResponse> ExpireAsync(ExpireOptions expireOptions)
            {
                return await ExecuteCommandAsync(expireOptions).ConfigureAwait(false);
            }

            #endregion;

            #region Dump

            /// <summary>
            /// Serialize the given key and return the serialized value. Use the RESTORE command to deserialize this value
            /// </summary>
            /// <param name="dumpOptions">Dump options</param>
            /// <returns>Return cache result</returns>
            public static async Task<DumpResponse> DumpAsync(DumpOptions dumpOptions)
            {
                return await ExecuteCommandAsync(dumpOptions).ConfigureAwait(false);
            }

            #endregion

            #region Delete

            /// <summary>
            /// Delete the specified key
            /// </summary>
            /// <param name="deleteOptions">Delete options</param>
            /// <returns>Return cache result</returns>
            public static async Task<DeleteResponse> DeleteAsync(DeleteOptions deleteOptions)
            {
                return await ExecuteCommandAsync(deleteOptions).ConfigureAwait(false);
            }

            #endregion

            #region Get keys

            /// <summary>
            /// Find all keys that match a given pattern
            /// </summary>
            /// <param name="getKeysOptions">Get keys options</param>
            /// <returns>Return cache result</returns>
            public static async Task<GetKeysResponse> GetKeysAsync(GetKeysOptions getKeysOptions)
            {
                return await ExecuteCommandAsync(getKeysOptions).ConfigureAwait(false);
            }

            #endregion

            #region Exist

            /// <summary>
            /// Check whether key exist
            /// </summary>
            /// <param name="existOptions">Exist options</param>
            /// <returns>Return cache result</returns>
            public static async Task<ExistResponse> ExistAsync(ExistOptions existOptions)
            {
                return await ExecuteCommandAsync(existOptions).ConfigureAwait(false);
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
            /// <param name="getAllDataBaseOptions">Get all database options</param>
            /// <returns>Return cache result</returns>
            public static async Task<GetAllDataBaseResponse> GetAllDataBaseAsync(CacheServer server, GetAllDataBaseOptions getAllDataBaseOptions)
            {
                return await getAllDataBaseOptions.ExecuteAsync(server).ConfigureAwait(false);
            }

            #endregion

            #region Query keys

            /// <summary>
            /// Query key value
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="getKeysOptions"> Get keys options </param>
            /// <returns>Return cache result</returns>
            public static async Task<GetKeysResponse> GetKeysAsync(CacheServer server, GetKeysOptions getKeysOptions)
            {
                return await getKeysOptions.ExecuteAsync(server).ConfigureAwait(false);
            }

            #endregion

            #region Clear data

            /// <summary>
            /// Clear all data in the cache database
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="clearDataOptions"> Clear data options </param>
            /// <returns>Return cache result</returns>
            public static async Task<ClearDataResponse> ClearDataAsync(CacheServer server, ClearDataOptions clearDataOptions)
            {
                return await clearDataOptions.ExecuteAsync(server).ConfigureAwait(false);
            }

            #endregion

            #region Get cache item detail

            /// <summary>
            /// Get data item details
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="getDetailOptions"> Get detail options </param>
            /// <returns>Return cache result</returns>
            public static async Task<GetDetailResponse> GetKeyDetailAsync(CacheServer server, GetDetailOptions getDetailOptions)
            {
                return await getDetailOptions.ExecuteAsync(server).ConfigureAwait(false);
            }

            #endregion

            #region Get server configuration

            /// <summary>
            /// Get server configuration
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="getServerConfigurationOptions">Get server configuration options</param>
            /// <returns>Return cache result</returns>
            public static async Task<GetServerConfigurationResponse> GetServerConfigurationAsync(CacheServer server, GetServerConfigurationOptions getServerConfigurationOptions)
            {
                return await getServerConfigurationOptions.ExecuteAsync(server).ConfigureAwait(false);
            }

            #endregion

            #region Save server configuration

            /// <summary>
            /// Save the server configuration
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="saveServerConfigurationOptions"> Save server configuration options </param>
            /// <returns>Return cache result</returns>
            public static async Task<SaveServerConfigurationResponse> SaveServerConfigurationAsync(CacheServer server, SaveServerConfigurationOptions saveServerConfigurationOptions)
            {
                return await saveServerConfigurationOptions.ExecuteAsync(server).ConfigureAwait(false);
            }

            #endregion
        }

        #endregion

        #region Execute command

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="options">Request options</param>
        /// <returns>Reurn cache result</returns>
        static async Task<TResponse> ExecuteCommandAsync<TResponse>(CacheOperationOptions<TResponse> options) where TResponse : CacheResponse, new()
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