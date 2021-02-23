using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EZNEW.Cache.Command.Result;
using EZNEW.Cache.Hash;
using EZNEW.Cache.Keys;
using EZNEW.Cache.List;
using EZNEW.Cache.Provider.Memory;
using EZNEW.Cache.Server;
using EZNEW.Cache.Set;
using EZNEW.Cache.SortedSet;
using EZNEW.Cache.String;
using EZNEW.Serialize;

namespace EZNEW.Cache
{
    /// <summary>
    /// Cache manager
    /// </summary>
    public static class CacheManager
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
        public static async Task<CacheResult<StringSetResponse>> SetDataAsync<T>(CacheKey key, T data, DateTimeOffset? absoluteExpiration = null, CacheSetWhen when = CacheSetWhen.Always, CacheObject cacheObject = null)
        {
            var value = JsonSerializeHelper.ObjectToJson(data);
            if (string.IsNullOrWhiteSpace(value))
            {
                return CacheResult<StringSetResponse>.EmptyResult();
            }
            var setResult = await String.SetAsync(key, value, absoluteExpiration, when, cacheObject).ConfigureAwait(false);
            return setResult;
        }

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
        public static CacheResult<StringSetResponse> SetData<T>(CacheKey key, T data, DateTimeOffset? absoluteExpiration = null, CacheSetWhen when = CacheSetWhen.Always, CacheObject cacheObject = null)
        {
            return SetDataAsync(key, data, absoluteExpiration, when, cacheObject).Result;
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
        public static async Task<CacheResult<StringSetResponse>> SetDataByRelativeExpirationAsync<T>(CacheKey key, T data, TimeSpan? absoluteExpirationRelativeToNow = null, bool slidingExpiration = true, CacheSetWhen when = CacheSetWhen.Always, CacheObject cacheObject = null)
        {
            var value = JsonSerializeHelper.ObjectToJson(data);
            if (string.IsNullOrWhiteSpace(value))
            {
                return CacheResult<StringSetResponse>.EmptyResult();
            }
            var setResult = await String.SetByRelativeExpirationAsync(key, value, absoluteExpirationRelativeToNow, slidingExpiration, when, cacheObject).ConfigureAwait(false);
            return setResult;
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
        public static CacheResult<StringSetResponse> SetDataByRelativeExpiration<T>(CacheKey key, T data, TimeSpan? absoluteExpirationRelativeToNow = null, bool slidingExpiration = true, CacheSetWhen when = CacheSetWhen.Always, CacheObject cacheObject = null)
        {
            return SetDataByRelativeExpirationAsync(key, data, absoluteExpirationRelativeToNow, slidingExpiration, when, cacheObject).Result;
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

        /// <summary>
        /// Get the data object according to the specified key value (use Json deserialization)
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="cacheObject">Cache object</param>
        /// <returns>Return data object</returns>
        public static T GetData<T>(CacheKey key, CacheObject cacheObject = null)
        {
            return GetDataAsync<T>(key, cacheObject).Result;
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
            var dataList = await String.GetAsync<T>(cacheKeys, cacheObject).ConfigureAwait(false);
            return dataList;
        }

        /// <summary>
        /// Get data list
        /// </summary>
        /// <param name="cacheKeys">Cache keys</param>
        /// <param name="cacheObject">Cache object</param>
        /// <returns>Return data list</returns>
        public static List<T> GetDataList<T>(IEnumerable<CacheKey> cacheKeys, CacheObject cacheObject = null)
        {
            return GetDataListAsync<T>(cacheKeys, cacheObject).Result;
        }

        #endregion

        #endregion

        #region String

        /// <summary>
        /// String data type operation
        /// </summary>
        public static class String
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
            public static async Task<CacheResult<StringSetRangeResponse>> SetRangeAsync(StringSetRangeOptions stringSetRangeOptions)
            {
                return await ExecuteCommandAsync(stringSetRangeOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Starting with offset, overwrite the string value stored by the overwrite key with the value parameter,
            /// Make sure the string is long enough to set the value to the specified offset, 
            /// If the original string length stored by the key is smaller than the offset (say, the string is only 5 characters long, but you set the offset to 10),  
            /// the space between the original character and the offset will be filled with zerobytes (zerobytes, "\x00")
            /// </summary>
            /// <param name="stringSetRangeOptions">Set range options</param>
            /// <returns>Return cache set result</returns>
            public static CacheResult<StringSetRangeResponse> SetRange(StringSetRangeOptions stringSetRangeOptions)
            {
                return SetRangeAsync(stringSetRangeOptions).Result;
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
            public static async Task<CacheResult<StringSetBitResponse>> SetBitAsync(StringSetBitOptions stringSetBitOptions)
            {
                return await ExecuteCommandAsync(stringSetBitOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// To set or clear the bit (bit) at the specified offset for the string value stored by the key,
            /// When the key does not exist, a new string value is automatically generated.
            /// The string is stretched (grown) to ensure that it can hold the value at the specified offset. When the string value is stretched, the empty space is filled with 0
            /// The offset parameter must be greater than or equal to 0 and less than 2^32
            /// </summary>
            /// <param name="stringSetBitOptions">Set bit options</param>
            /// <returns>Return cache set result</returns>
            public static CacheResult<StringSetBitResponse> SetBit(StringSetBitOptions stringSetBitOptions)
            {
                return ExecuteCommandAsync(stringSetBitOptions).Result;
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
            public static async Task<CacheResult<StringSetResponse>> SetAsync(StringSetOptions stringSetOptions)
            {
                return await ExecuteCommandAsync(stringSetOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Associate the string value value to the key
            /// If the key already holds other values, overwrite the old values
            /// When the SET command sets a key with a time to live (TTL), the original TTL of the key will be cleared
            /// </summary>
            /// <param name="stringSetOptions">String set options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<StringSetResponse> Set(StringSetOptions stringSetOptions)
            {
                return ExecuteCommandAsync(stringSetOptions).Result;
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
            public static async Task<CacheResult<StringSetResponse>> SetAsync(CacheKey key, string value, DateTimeOffset? absoluteExpiration = null, CacheSetWhen when = CacheSetWhen.Always, CacheObject cacheObject = null)
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
            /// <param name="absoluteExpiration">Absolute expiration time</param>
            /// <param name="when">Set value conditions</param>
            /// <param name="cacheObject">Cache object</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<StringSetResponse> Set(CacheKey key, string value, DateTimeOffset? absoluteExpiration = null, CacheSetWhen when = CacheSetWhen.Always, CacheObject cacheObject = null)
            {
                return SetAsync(key, value, absoluteExpiration, when, cacheObject).Result;
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
            public static async Task<CacheResult<StringSetResponse>> SetByRelativeExpirationAsync(CacheKey key, string value, TimeSpan? absoluteExpirationRelativeToNow = null, bool slidingExpiration = true, CacheSetWhen when = CacheSetWhen.Always, CacheObject cacheObject = null)
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
            public static CacheResult<StringSetResponse> SetByRelativeExpiration(CacheKey key, string value, TimeSpan? absoluteExpirationRelativeToNow = null, bool slidingExpiration = true, CacheSetWhen when = CacheSetWhen.Always, CacheObject cacheObject = null)
            {
                return SetByRelativeExpirationAsync(key, value, absoluteExpirationRelativeToNow, slidingExpiration, when, cacheObject).Result;
            }

            #endregion

            #region Length

            /// <summary>
            /// Returns the length of the string value stored by key
            /// </summary>
            /// <param name="stringLengthOptions">String length options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<StringLengthResponse>> LengthAsync(StringLengthOptions stringLengthOptions)
            {
                return await ExecuteCommandAsync(stringLengthOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns the length of the string value stored by key
            /// </summary>
            /// <param name="stringLengthOptions">String length options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<StringLengthResponse> Length(StringLengthOptions stringLengthOptions)
            {
                return ExecuteCommandAsync(stringLengthOptions).Result;
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
            public static async Task<CacheResult<StringIncrementResponse>> IncrementAsync(StringIncrementOptions stringIncrementOptions)
            {
                return await ExecuteCommandAsync(stringIncrementOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Add one to the numeric value stored by the key
            /// If the key key does not exist, then its value will be initialized to 0 first, and then execute the command
            /// If the value stored by the key cannot be interpreted as a number, the command will return an error
            /// </summary>
            /// <param name="stringIncrementOptions">String increment options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<StringIncrementResponse> Increment(StringIncrementOptions stringIncrementOptions)
            {
                return ExecuteCommandAsync(stringIncrementOptions).Result;
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
            public static async Task<CacheResult<StringGetWithExpiryResponse>> GetWithExpiryAsync(StringGetWithExpiryOptions stringGetWithExpiryOptions)
            {
                return await ExecuteCommandAsync(stringGetWithExpiryOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns the string value associated with the key
            /// If the key key does not exist, then return an empty string, otherwise, return the value of the key key
            /// If the value of key is not of type string, then return an error.
            /// </summary>
            /// <param name="stringGetWithExpiryOptions">String get with expiry options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<StringGetWithExpiryResponse> GetWithExpiry(StringGetWithExpiryOptions stringGetWithExpiryOptions)
            {
                return ExecuteCommandAsync(stringGetWithExpiryOptions).Result;
            }

            #endregion

            #region Get set

            /// <summary>
            /// Set the value of the key key to value and return the old value of the key key before it is set
            /// When the key key exists but is not a string type, the command returns an error
            /// </summary>
            /// <param name="stringGetSetOptions">String get set options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<StringGetSetResponse>> GetSetAsync(StringGetSetOptions stringGetSetOptions)
            {
                return await ExecuteCommandAsync(stringGetSetOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Set the value of the key key to value and return the old value of the key key before it is set
            /// When the key key exists but is not a string type, the command returns an error
            /// </summary>
            /// <param name="stringGetSetOptions">String get set options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<StringGetSetResponse> GetSet(StringGetSetOptions stringGetSetOptions)
            {
                return ExecuteCommandAsync(stringGetSetOptions).Result;
            }

            #endregion

            #region Get range

            /// <summary>
            /// Returns the specified part of the string value stored by the key key, the range of the string interception is determined by the two offsets of start and end (including start and end)
            /// Negative offset means counting from the end of the string, -1 means the last character, -2 means the penultimate character, and so on
            /// </summary>
            /// <param name="stringGetRangeOptions">String get range options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<StringGetRangeResponse>> GetRangeAsync(StringGetRangeOptions stringGetRangeOptions)
            {
                return await ExecuteCommandAsync(stringGetRangeOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns the specified part of the string value stored by the key key, the range of the string interception is determined by the two offsets of start and end (including start and end)
            /// Negative offset means counting from the end of the string, -1 means the last character, -2 means the penultimate character, and so on
            /// </summary>
            /// <param name="stringGetRangeOptions">String get range options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<StringGetRangeResponse> GetRange(StringGetRangeOptions stringGetRangeOptions)
            {
                return ExecuteCommandAsync(stringGetRangeOptions).Result;
            }

            #endregion

            #region Get bit

            /// <summary>
            /// For the string value stored in key, get the bit at the specified offset
            /// When offset is greater than the length of the string value, or the key does not exist, return 0
            /// </summary>
            /// <param name="stringGetBitOptions">String get bit options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<StringGetBitResponse>> GetBitAsync(StringGetBitOptions stringGetBitOptions)
            {
                return await ExecuteCommandAsync(stringGetBitOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// For the string value stored in key, get the bit at the specified offset
            /// When offset is greater than the length of the string value, or the key does not exist, return 0
            /// </summary>
            /// <param name="stringGetBitOptions">String get bit options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<StringGetBitResponse> GetBit(StringGetBitOptions stringGetBitOptions)
            {
                return ExecuteCommandAsync(stringGetBitOptions).Result;
            }

            #endregion

            #region Get

            /// <summary>
            /// Returns the value of the given string key or keys
            /// </summary>
            /// <param name="stringGetOptions">String get options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<StringGetResponse>> GetAsync(StringGetOptions stringGetOptions)
            {
                return await ExecuteCommandAsync(stringGetOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns the value of the given string key or keys
            /// </summary>
            /// <param name="stringGetOptions">String get options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<StringGetResponse> Get(StringGetOptions stringGetOptions)
            {
                return GetAsync(stringGetOptions).Result;
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
            /// Returns the string value associated with the key
            /// If the key key does not exist, then return an empty string, otherwise, return the value of the key key
            /// If the value of key is not of type string, then return an error.
            /// </summary>
            /// <param name="key">Cache key</param>
            /// <param name="cacheObject">Cache object</param>
            /// <returns>Return cache result</returns>
            public static string Get(CacheKey key, CacheObject cacheObject = null)
            {
                return GetAsync(key, cacheObject).Result;
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
                return JsonSerializeHelper.JsonToObject<T>(cacheValue);
            }

            /// <summary>
            /// Return the value associated with the key and convert it to the specified data object
            /// </summary>
            /// <param name="key">Cache key</param>
            /// <param name="cacheObject">Cache object</param>
            /// <returns>Return cache result</returns>
            public static T Get<T>(CacheKey key, CacheObject cacheObject = null)
            {
                return GetAsync<T>(key, cacheObject).Result;
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
                List<string> values = new List<string>();
                if (result?.Responses?.Count > 0)
                {
                    foreach (var response in result.Responses)
                    {
                        if (response.Values.IsNullOrEmpty())
                        {
                            continue;
                        }
                        values.AddRange(response.Values.Select(c => c.Value?.ToString() ?? string.Empty));
                    }
                }
                return values;
            }

            /// <summary>
            /// Returns the value of the given string key or keys
            /// </summary>
            /// <param name="keys">Cache keys​​</param>
            /// <param name="cacheObject">Cache object</param>
            /// <returns>return values</returns>
            public static List<string> Get(IEnumerable<CacheKey> keys, CacheObject cacheObject = null)
            {
                return GetAsync(keys, cacheObject).Result;
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
                    var data = JsonSerializeHelper.JsonToObject<T>(val);
                    datas.Add(data);
                }
                return datas;
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
                return GetAsync<T>(keys, cacheObject).Result;
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
            public static async Task<CacheResult<StringDecrementResponse>> DecrementAsync(StringDecrementOptions stringDecrementOptions)
            {
                return await ExecuteCommandAsync(stringDecrementOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Minus one for the numeric value stored for the key
            /// If the key key does not exist, the value of the key key will be initialized to 0 first, and then perform the operation
            /// If the value stored by the key cannot be interpreted as a number, an error will be returned
            /// </summary>
            /// <param name="stringDecrementOptions">String decrement options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<StringDecrementResponse> Decrement(StringDecrementOptions stringDecrementOptions)
            {
                return DecrementAsync(stringDecrementOptions).Result;
            }

            #endregion

            #region Bit position

            /// <summary>
            /// Returns the position of the first binary bit in the bitmap
            /// By default, the command will detect the entire bitmap, but the user can also specify the range to be detected through the optional start parameter and end parameter
            /// </summary>
            /// <param name="stringBitPositionOptions">String bit position options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<StringBitPositionResponse>> BitPositionAsync(StringBitPositionOptions stringBitPositionOptions)
            {
                return await ExecuteCommandAsync(stringBitPositionOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns the position of the first binary bit in the bitmap
            /// By default, the command will detect the entire bitmap, but the user can also specify the range to be detected through the optional start parameter and end parameter
            /// </summary>
            /// <param name="stringBitPositionOptions">String bit position options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<StringBitPositionResponse> BitPosition(StringBitPositionOptions stringBitPositionOptions)
            {
                return BitPositionAsync(stringBitPositionOptions).Result;
            }

            #endregion

            #region Bit operation

            /// <summary>
            /// Perform bit operations on one or more string keys that hold binary bits, and save the result to destkey
            /// Except NOT operation, other operations can accept one or more keys as input
            /// </summary>
            /// <param name="stringBitOperationOptions">String bit operation options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<StringBitOperationResponse>> BitOperationAsync(StringBitOperationOptions stringBitOperationOptions)
            {
                return await ExecuteCommandAsync(stringBitOperationOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Perform bit operations on one or more string keys that hold binary bits, and save the result to destkey
            /// Except NOT operation, other operations can accept one or more keys as input
            /// </summary>
            /// <param name="stringBitOperationOptions">String bit operation options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<StringBitOperationResponse> BitOperation(StringBitOperationOptions stringBitOperationOptions)
            {
                return BitOperationAsync(stringBitOperationOptions).Result;
            }

            #endregion

            #region Bit count

            /// <summary>
            /// Calculate the number of bits set to 1 in a given string.
            /// Under normal circumstances, the given entire string will be counted, by specifying additional start or end parameters, you can make the count only on a specific bit
            /// </summary>
            /// <param name="stringBitCountOptions">String bit count options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<StringBitCountResponse>> BitCountAsync(StringBitCountOptions stringBitCountOptions)
            {
                return await ExecuteCommandAsync(stringBitCountOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Calculate the number of bits set to 1 in a given string.
            /// Under normal circumstances, the given entire string will be counted, by specifying additional start or end parameters, you can make the count only on a specific bit
            /// </summary>
            /// <param name="stringBitCountOptions">String bit count options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<StringBitCountResponse> BitCount(StringBitCountOptions stringBitCountOptions)
            {
                return BitCountAsync(stringBitCountOptions).Result;
            }

            #endregion

            #region Append

            /// <summary>
            /// If the key key already exists and its value is a string, the value will be appended to the end of the key key's existing value
            /// If the key does not exist, simply set the value of the key key to value
            /// </summary>
            /// <param name="stringAppendOptions">String append options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<StringAppendResponse>> AppendAsync(StringAppendOptions stringAppendOptions)
            {
                return await ExecuteCommandAsync(stringAppendOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// If the key key already exists and its value is a string, the value will be appended to the end of the key key's existing value
            /// If the key does not exist, simply set the value of the key key to value
            /// </summary>
            /// <param name="stringAppendOptions">String append options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<StringAppendResponse> Append(StringAppendOptions stringAppendOptions)
            {
                return AppendAsync(stringAppendOptions).Result;
            }

            #endregion
        }

        #endregion

        #region List

        /// <summary>
        /// List data type operation
        /// </summary>
        public static class List
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
            public static async Task<CacheResult<ListTrimResponse>> TrimAsync(ListTrimOptions listTrimOptions)
            {
                return await ExecuteCommandAsync(listTrimOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Trim a list, that is, let the list keep only the elements in the specified interval, and elements that are not in the specified interval will be deleted
            /// The subscript (index) parameters start and stop are both based on 0, that is, 0 represents the first element of the list, 1 represents the second element of the list, and so on
            /// You can also use negative subscripts, with -1 for the last element of the list, -2 for the penultimate element of the list, and so on
            /// When key is not a list type, an error is returned
            /// </summary>
            /// <param name="listTrimOptions">List trim options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<ListTrimResponse> Trim(ListTrimOptions listTrimOptions)
            {
                return TrimAsync(listTrimOptions).Result;
            }

            #endregion

            #region Set by index

            /// <summary>
            /// Set the value of the element whose index of the list key is index to value
            /// When the index parameter is out of range, or an operation is performed on an empty list (the key does not exist), an error is returned
            /// </summary>
            /// <param name="listSetByIndexOptions">List set by index options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<ListSetByIndexResponse>> SetByIndexAsync(ListSetByIndexOptions listSetByIndexOptions)
            {
                return await ExecuteCommandAsync(listSetByIndexOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Set the value of the element whose index of the list key is index to value
            /// When the index parameter is out of range, or an operation is performed on an empty list (the key does not exist), an error is returned
            /// </summary>
            /// <param name="listSetByIndexOptions">List set by index options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<ListSetByIndexResponse> SetByIndex(ListSetByIndexOptions listSetByIndexOptions)
            {
                return SetByIndexAsync(listSetByIndexOptions).Result;
            }

            #endregion

            #region Right push

            /// <summary>
            /// Insert one or more values ​​into the end of the list key (far right).
            /// If there are multiple value values, then each value value is inserted into the end of the table in order from left to right
            /// </summary>
            /// <param name="listRightPushOptions">List right push options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<ListRightPushResponse>> RightPushAsync(ListRightPushOptions listRightPushOptions)
            {
                return await ExecuteCommandAsync(listRightPushOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Insert one or more values ​​into the end of the list key (far right).
            /// If there are multiple value values, then each value value is inserted into the end of the table in order from left to right
            /// </summary>
            /// <param name="listRightPushOptions">List right push options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<ListRightPushResponse> RightPush(ListRightPushOptions listRightPushOptions)
            {
                return RightPushAsync(listRightPushOptions).Result;
            }

            #endregion

            #region Right pop left push

            /// <summary>
            /// Pop the last element (tail element) in the list source and return it to the client
            /// Insert the element popped by source into the destination list as the head element of the destination list
            /// </summary>
            /// <param name="listRightPopLeftPushOptions">List right pop left push options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<ListRightPopLeftPushResponse>> RightPopLeftPushAsync(ListRightPopLeftPushOptions listRightPopLeftPushOptions)
            {
                return await ExecuteCommandAsync(listRightPopLeftPushOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Pop the last element (tail element) in the list source and return it to the client
            /// Insert the element popped by source into the destination list as the head element of the destination list
            /// </summary>
            /// <param name="listRightPopLeftPushOptions">List right pop left push options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<ListRightPopLeftPushResponse> RightPopLeftPush(ListRightPopLeftPushOptions listRightPopLeftPushOptions)
            {
                return RightPopLeftPushAsync(listRightPopLeftPushOptions).Result;
            }

            #endregion

            #region Right pop

            /// <summary>
            /// Remove and return the tail element of the list key.
            /// </summary>
            /// <param name="listRightPopOptions">List right pop options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<ListRightPopResponse>> RightPopAsync(ListRightPopOptions listRightPopOptions)
            {
                return await ExecuteCommandAsync(listRightPopOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Remove and return the tail element of the list key.
            /// </summary>
            /// <param name="listRightPopOptions">List right pop options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<ListRightPopResponse> RightPop(ListRightPopOptions listRightPopOptions)
            {
                return RightPopAsync(listRightPopOptions).Result;
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
            public static async Task<CacheResult<ListRemoveResponse>> RemoveAsync(ListRemoveOptions listRemoveOptions)
            {
                return await ExecuteCommandAsync(listRemoveOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// According to the value of the parameter count, remove the elements equal to the parameter value in the list
            /// count is greater than 0: search from the beginning of the table to the end of the table, remove the elements equal to value, the number is count
            /// count is less than 0: search from the end of the table to the head of the table, remove the elements equal to value, the number is the absolute value of count
            /// count equals 0: remove all values ​​in the table that are equal to value
            /// </summary>
            /// <param name="listRemoveOptions">List remove options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<ListRemoveResponse> Remove(ListRemoveOptions listRemoveOptions)
            {
                return RemoveAsync(listRemoveOptions).Result;
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
            public static async Task<CacheResult<ListRangeResponse>> RangeAsync(ListRangeOptions listRangeOptions)
            {
                return await ExecuteCommandAsync(listRangeOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Return the elements in the specified interval in the list key, the interval is specified by the offset start and stop
            /// The subscript (index) parameters start and stop are both based on 0, that is, 0 represents the first element of the list, 1 represents the second element of the list, and so on
            /// You can also use negative subscripts, with -1 for the last element of the list, -2 for the penultimate element of the list, and so on
            /// </summary>
            /// <param name="listRangeOptions">List range options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<ListRangeResponse> Range(ListRangeOptions listRangeOptions)
            {
                return RangeAsync(listRangeOptions).Result;
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
            public static async Task<CacheResult<ListLengthResponse>> LengthAsync(ListLengthOptions listLengthOptions)
            {
                return await ExecuteCommandAsync(listLengthOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Return the length of the list key
            /// If the key does not exist, the key is interpreted as an empty list and returns 0 
            /// If key is not a list type, return an error.
            /// </summary>
            /// <param name="listLengthOptions">List length options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<ListLengthResponse> Length(ListLengthOptions listLengthOptions)
            {
                return LengthAsync(listLengthOptions).Result;
            }

            #endregion

            #region Left push

            /// <summary>
            /// Insert one or more values ​​into the header of the list key
            /// If the key does not exist, an empty list will be created and the operation will be performed
            /// </summary>
            /// <param name="listLeftPushOptions">List left push options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<ListLeftPushResponse>> LeftPushAsync(ListLeftPushOptions listLeftPushOptions)
            {
                return await ExecuteCommandAsync(listLeftPushOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Insert one or more values ​​into the header of the list key
            /// If the key does not exist, an empty list will be created and the operation will be performed
            /// </summary>
            /// <param name="listLeftPushOptions">List left push options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<ListLeftPushResponse> LeftPush(ListLeftPushOptions listLeftPushOptions)
            {
                return LeftPushAsync(listLeftPushOptions).Result;
            }

            #endregion

            #region Left pop

            /// <summary>
            /// Remove and return the head element of the list key
            /// </summary>
            /// <param name="listLeftPopOptions">List left pop options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<ListLeftPopResponse>> LeftPopAsync(ListLeftPopOptions listLeftPopOptions)
            {
                return await ExecuteCommandAsync(listLeftPopOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Remove and return the head element of the list key
            /// </summary>
            /// <param name="listLeftPopOptions">List left pop options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<ListLeftPopResponse> LeftPop(ListLeftPopOptions listLeftPopOptions)
            {
                return LeftPopAsync(listLeftPopOptions).Result;
            }

            #endregion

            #region Insert before

            /// <summary>
            /// Insert the value value into the list key, before the value pivot
            /// </summary>
            /// <param name="listInsertBeforeOptions">List insert before options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<ListInsertBeforeResponse>> InsertBeforeAsync(ListInsertBeforeOptions listInsertBeforeOptions)
            {
                return await ExecuteCommandAsync(listInsertBeforeOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Insert the value value into the list key, before the value pivot
            /// </summary>
            /// <param name="listInsertBeforeOptions">List insert before options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<ListInsertBeforeResponse> InsertBefore(ListInsertBeforeOptions listInsertBeforeOptions)
            {
                return InsertBeforeAsync(listInsertBeforeOptions).Result;
            }

            #endregion

            #region Insert after

            /// <summary>
            /// Insert the value value into the list key, after the value pivot
            /// </summary>
            /// <param name="listInsertAfterOptions">List insert after options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<ListInsertAfterResponse>> InsertAfterAsync(ListInsertAfterOptions listInsertAfterOptions)
            {
                return await ExecuteCommandAsync(listInsertAfterOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Insert the value value into the list key, after the value pivot
            /// </summary>
            /// <param name="listInsertAfterOptions">List insert after options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<ListInsertAfterResponse> InsertAfter(ListInsertAfterOptions listInsertAfterOptions)
            {
                return InsertAfterAsync(listInsertAfterOptions).Result;
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
            public static async Task<CacheResult<ListGetByIndexResponse>> GetByIndexAsync(ListGetByIndexOptions listGetByIndexOptions)
            {
                return await ExecuteCommandAsync(listGetByIndexOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Return the element with index index in the list key
            /// The subscript (index) parameters start and stop are both based on 0, that is, 0 represents the first element of the list, 1 represents the second element of the list, and so on
            /// You can also use negative subscripts, with -1 for the last element of the list, -2 for the penultimate element of the list, and so on
            /// </summary>
            /// <param name="listGetByIndexOptions">List get by index options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<ListGetByIndexResponse> GetByIndex(ListGetByIndexOptions listGetByIndexOptions)
            {
                return GetByIndexAsync(listGetByIndexOptions).Result;
            }

            #endregion
        }

        #endregion

        #region Hash

        /// <summary>
        /// Hash data type operation
        /// </summary>
        public static class Hash
        {
            #region Values

            /// <summary>
            /// Return the values ​​of all fields in the hash table key
            /// </summary>
            /// <param name="hashValuesOptions">Hash values options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<HashValuesResponse>> ValuesAsync(HashValuesOptions hashValuesOptions)
            {
                return await ExecuteCommandAsync(hashValuesOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Return the values ​​of all fields in the hash table key
            /// </summary>
            /// <param name="hashValuesOptions">Hash values options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<HashValuesResponse> Values(HashValuesOptions hashValuesOptions)
            {
                return ValuesAsync(hashValuesOptions).Result;
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
            public static async Task<CacheResult<HashSetResponse>> SetAsync(HashSetOptions hashSetOptions)
            {
                return await ExecuteCommandAsync(hashSetOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Set the value of the field field in the hash table hash to value
            /// If the given hash table does not exist, then a new hash table will be created and perform the operation
            /// If the field field already exists in the hash table, its old value will be overwritten by the new value
            /// </summary>
            /// <param name="hashSetOptions">Hash set options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<HashSetResponse> Set(HashSetOptions hashSetOptions)
            {
                return SetAsync(hashSetOptions).Result;
            }

            #endregion

            #region Length

            /// <summary>
            /// returns the number of fields in the hash table key
            /// </summary>
            /// <param name="hashLengthOptions">Hash length options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<HashLengthResponse>> LengthAsync(HashLengthOptions hashLengthOptions)
            {
                return await ExecuteCommandAsync(hashLengthOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// returns the number of fields in the hash table key
            /// </summary>
            /// <param name="hashLengthOptions">Hash length options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<HashLengthResponse> Length(HashLengthOptions hashLengthOptions)
            {
                return LengthAsync(hashLengthOptions).Result;
            }

            #endregion

            #region Keys

            /// <summary>
            /// Return all keys in the hash table key
            /// </summary>
            /// <param name="hashKeysOptions">Hash keys options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<HashKeysResponse>> KeysAsync(HashKeysOptions hashKeysOptions)
            {
                return await ExecuteCommandAsync(hashKeysOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Return all keys in the hash table key
            /// </summary>
            /// <param name="hashKeysOptions">Hash keys options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<HashKeysResponse> Keys(HashKeysOptions hashKeysOptions)
            {
                return KeysAsync(hashKeysOptions).Result;
            }

            #endregion

            #region Increment

            /// <summary>
            /// Add incremental increment to the value of the field field in the hash table key
            /// </summary>
            /// <param name="hashIncrementOptions">Hash increment options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<HashIncrementResponse>> IncrementAsync(HashIncrementOptions hashIncrementOptions)
            {
                return await ExecuteCommandAsync(hashIncrementOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Add incremental increment to the value of the field field in the hash table key
            /// </summary>
            /// <param name="hashIncrementOptions">Hash increment options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<HashIncrementResponse> Increment(HashIncrementOptions hashIncrementOptions)
            {
                return IncrementAsync(hashIncrementOptions).Result;
            }

            #endregion

            #region Get

            /// <summary>
            /// Returns the value of the given field in the hash table
            /// </summary>
            /// <param name="hashGetOptions">Hash get options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<HashGetResponse>> GetAsync(HashGetOptions hashGetOptions)
            {
                return await ExecuteCommandAsync(hashGetOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns the value of the given field in the hash table
            /// </summary>
            /// <param name="hashGetOptions">Hash get options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<HashGetResponse> Get(HashGetOptions hashGetOptions)
            {
                return GetAsync(hashGetOptions).Result;
            }

            #endregion

            #region Get all

            /// <summary>
            /// Return the values ​​of all fields in the hash table key
            /// </summary>
            /// <param name="hashGetAllOptions">Hash get all options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<HashGetAllResponse>> GetAllAsync(HashGetAllOptions hashGetAllOptions)
            {
                return await ExecuteCommandAsync(hashGetAllOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Return the values ​​of all fields in the hash table key
            /// </summary>
            /// <param name="hashGetAllOptions">Hash get all options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<HashGetAllResponse> GetAll(HashGetAllOptions hashGetAllOptions)
            {
                return GetAllAsync(hashGetAllOptions).Result;
            }

            #endregion

            #region Exist

            /// <summary>
            /// Check if the given field exists in the hash of the hash table
            /// </summary>
            /// <param name="hashExistsOptions">Hash exists options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<HashExistsResponse>> ExistAsync(HashExistsOptions hashExistsOptions)
            {
                return await ExecuteCommandAsync(hashExistsOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Check if the given field exists in the hash of the hash table
            /// </summary>
            /// <param name="hashExistsOptions">Hash exists options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<HashExistsResponse> Exist(HashExistsOptions hashExistsOptions)
            {
                return ExistAsync(hashExistsOptions).Result;
            }

            #endregion

            #region Delete

            /// <summary>
            /// Delete one or more specified fields in the hash table key, the non-existing fields will be ignored
            /// </summary>
            /// <param name="hashDeleteOptions">Hash delete options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<HashDeleteResponse>> DeleteAsync(HashDeleteOptions hashDeleteOptions)
            {
                return await ExecuteCommandAsync(hashDeleteOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Delete one or more specified fields in the hash table key, the non-existing fields will be ignored
            /// </summary>
            /// <param name="hashDeleteOptions">Hash delete options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<HashDeleteResponse> Delete(HashDeleteOptions hashDeleteOptions)
            {
                return DeleteAsync(hashDeleteOptions).Result;
            }

            #endregion

            #region Decrement

            /// <summary>
            /// Is the value of the field in the hash table key minus the increment increment
            /// </summary>
            /// <param name="hashDecrementOptions">Hash decrement options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<HashDecrementResponse>> DecrementAsync(HashDecrementOptions hashDecrementOptions)
            {
                return await ExecuteCommandAsync(hashDecrementOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Is the value of the field in the hash table key minus the increment increment
            /// </summary>
            /// <param name="hashDecrementOptions">Hash decrement options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<HashDecrementResponse> Decrement(HashDecrementOptions hashDecrementOptions)
            {
                return DecrementAsync(hashDecrementOptions).Result;
            }

            #endregion

            #region Scan

            /// <summary>
            /// Each element returned is a key-value pair, a key-value pair consists of a key and a value
            /// </summary>
            /// <param name="hashScanOptions">Hash scan options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<HashScanResponse>> ScanAsync(HashScanOptions hashScanOptions)
            {
                return await ExecuteCommandAsync(hashScanOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Each element returned is a key-value pair, a key-value pair consists of a key and a value
            /// </summary>
            /// <param name="hashScanOptions">Hash scan options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<HashScanResponse> Scan(HashScanOptions hashScanOptions)
            {
                return ScanAsync(hashScanOptions).Result;
            }

            #endregion
        }

        #endregion

        #region Set

        /// <summary>
        /// Set data type operation
        /// </summary>
        public static class Set
        {
            #region Remove

            /// <summary>
            /// Remove one or more member elements in the collection key, non-existent member elements will be ignored
            /// </summary>
            /// <param name="setRemoveOptions">Set remove options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SetRemoveResponse>> RemoveAsync(SetRemoveOptions setRemoveOptions)
            {
                return await ExecuteCommandAsync(setRemoveOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Remove one or more member elements in the collection key, non-existent member elements will be ignored
            /// </summary>
            /// <param name="setRemoveOptions">Set remove options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SetRemoveResponse> Remove(SetRemoveOptions setRemoveOptions)
            {
                return RemoveAsync(setRemoveOptions).Result;
            }

            #endregion

            #region Random members

            /// <summary>
            /// Then return a set of random elements in the collection
            /// </summary>
            /// <param name="setRandomMembersOptions">Set random members options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SetRandomMembersResponse>> RandomMembersAsync(SetRandomMembersOptions setRandomMembersOptions)
            {
                return await ExecuteCommandAsync(setRandomMembersOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Return a set of random elements in the collection
            /// </summary>
            /// <param name="setRandomMembersOptions">Set random members options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SetRandomMembersResponse> RandomMembers(SetRandomMembersOptions setRandomMembersOptions)
            {
                return RandomMembersAsync(setRandomMembersOptions).Result;
            }

            #endregion

            #region Random member

            /// <summary>
            /// Returns a random element in the collection
            /// </summary>
            /// <param name="setRandomMemberOptions">Set random member options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SetRandomMemberResponse>> RandomMemberAsync(SetRandomMemberOptions setRandomMemberOptions)
            {
                return await ExecuteCommandAsync(setRandomMemberOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns a random element in the collection
            /// </summary>
            /// <param name="setRandomMemberOptions">Set random member options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SetRandomMemberResponse> RandomMember(SetRandomMemberOptions setRandomMemberOptions)
            {
                return RandomMemberAsync(setRandomMemberOptions).Result;
            }

            #endregion

            #region Pop

            /// <summary>
            /// Remove and return a random element in the collection
            /// </summary>
            /// <param name="setPopOptions">Set pop options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SetPopResponse>> PopAsync(SetPopOptions setPopOptions)
            {
                return await ExecuteCommandAsync(setPopOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Remove and return a random element in the collection
            /// </summary>
            /// <param name="setPopOptions">Set pop options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SetPopResponse> Pop(SetPopOptions setPopOptions)
            {
                return PopAsync(setPopOptions).Result;
            }

            #endregion

            #region Move

            /// <summary>
            /// Move the member element from the source collection to the destination collection
            /// </summary>
            /// <param name="setMoveOptions">Set move options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SetMoveResponse>> MoveAsync(SetMoveOptions setMoveOptions)
            {
                return await ExecuteCommandAsync(setMoveOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Move the member element from the source collection to the destination collection
            /// </summary>
            /// <param name="setMoveOptions">Set move options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SetMoveResponse> Move(SetMoveOptions setMoveOptions)
            {
                return MoveAsync(setMoveOptions).Result;
            }

            #endregion

            #region Members

            /// <summary>
            /// Return all members in the collection key
            /// </summary>
            /// <param name="setMembersOptions">Set members options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SetMembersResponse>> MembersAsync(SetMembersOptions setMembersOptions)
            {
                return await ExecuteCommandAsync(setMembersOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Return all members in the collection key
            /// </summary>
            /// <param name="setMembersOptions">Set members options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SetMembersResponse> Members(SetMembersOptions setMembersOptions)
            {
                return MembersAsync(setMembersOptions).Result;
            }

            #endregion

            #region Length

            /// <summary>
            /// Returns the number of elements in the collection
            /// </summary>
            /// <param name="setLengthOptions">Set length options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SetLengthResponse>> LengthAsync(SetLengthOptions setLengthOptions)
            {
                return await ExecuteCommandAsync(setLengthOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns the number of elements in the collection
            /// </summary>
            /// <param name="setLengthOptions">Set length options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SetLengthResponse> Length(SetLengthOptions setLengthOptions)
            {
                return LengthAsync(setLengthOptions).Result;
            }

            #endregion

            #region Contains

            /// <summary>
            /// Determine whether the member element is a member of the set key
            /// </summary>
            /// <param name="setContainsOptions">Set contaims options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SetContainsResponse>> ContainsAsync(SetContainsOptions setContainsOptions)
            {
                return await ExecuteCommandAsync(setContainsOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Determine whether the member element is a member of the set key
            /// </summary>
            /// <param name="setContainsOptions">Set contaims options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SetContainsResponse> Contains(SetContainsOptions setContainsOptions)
            {
                return ContainsAsync(setContainsOptions).Result;
            }

            #endregion

            #region Combine

            /// <summary>
            /// According to the operation mode, return the processing results of multiple collections
            /// </summary>
            /// <param name="setCombineOptions">Set combine options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SetCombineResponse>> CombineAsync(SetCombineOptions setCombineOptions)
            {
                return await ExecuteCommandAsync(setCombineOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// According to the operation mode, return the processing results of multiple collections
            /// </summary>
            /// <param name="setCombineOptions">Set combine options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SetCombineResponse> Combine(SetCombineOptions setCombineOptions)
            {
                return CombineAsync(setCombineOptions).Result;
            }

            #endregion

            #region Combine and store

            /// <summary>
            /// Return the processing results of multiple collections according to the operation mode, and store the results to the specified key value at the same time
            /// </summary>
            /// <param name="setCombineAndStoreOptions">Set combine and store options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SetCombineAndStoreResponse>> CombineAndStoreAsync(SetCombineAndStoreOptions setCombineAndStoreOptions)
            {
                return await ExecuteCommandAsync(setCombineAndStoreOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Return the processing results of multiple collections according to the operation mode, and store the results to the specified key value at the same time
            /// </summary>
            /// <param name="setCombineAndStoreOptions">Set combine and store options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SetCombineAndStoreResponse> CombineAndStore(SetCombineAndStoreOptions setCombineAndStoreOptions)
            {
                return CombineAndStoreAsync(setCombineAndStoreOptions).Result;
            }

            #endregion

            #region Add

            /// <summary>
            /// Add one or more member elements to the collection key, the member elements already in the collection will be ignored
            /// If the key does not exist, create a collection that contains only the member element as a member
            /// </summary>
            /// <param name="setAddOptions">Set add options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SetAddResponse>> AddAsync(SetAddOptions setAddOptions)
            {
                return await ExecuteCommandAsync(setAddOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Add one or more member elements to the collection key, the member elements already in the collection will be ignored
            /// If the key does not exist, create a collection that contains only the member element as a member
            /// </summary>
            /// <param name="setAddOptions">Set add options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SetAddResponse> Add(SetAddOptions setAddOptions)
            {
                return AddAsync(setAddOptions).Result;
            }

            #endregion
        }

        #endregion

        #region Sorted set

        /// <summary>
        /// Sorted set data type operation
        /// </summary>
        public static class SortedSet
        {
            #region Score

            /// <summary>
            /// Returns the score value of the member member in the ordered set key
            /// </summary>
            /// <param name="sortedSetScoreOptions">Sorted set score options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortedSetScoreResponse>> ScoreAsync(SortedSetScoreOptions sortedSetScoreOptions)
            {
                return await ExecuteCommandAsync(sortedSetScoreOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns the score value of the member member in the ordered set key
            /// </summary>
            /// <param name="sortedSetScoreOptions">Sorted set score options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortedSetScoreResponse> Score(SortedSetScoreOptions sortedSetScoreOptions)
            {
                return ScoreAsync(sortedSetScoreOptions).Result;
            }

            #endregion

            #region Remove range by value

            /// <summary>
            /// Remove the elements in the specified range after sorting the elements
            /// </summary>
            /// <param name="sortedSetRemoveRangeByValueOptions">Sorted set remove range by value options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortedSetRemoveRangeByValueResponse>> RemoveRangeByValueAsync(SortedSetRemoveRangeByValueOptions sortedSetRemoveRangeByValueOptions)
            {
                return await ExecuteCommandAsync(sortedSetRemoveRangeByValueOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Remove the elements in the specified range after sorting the elements
            /// </summary>
            /// <param name="sortedSetRemoveRangeByValueOptions">Sorted set remove range by value options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortedSetRemoveRangeByValueResponse> RemoveRangeByValue(SortedSetRemoveRangeByValueOptions sortedSetRemoveRangeByValueOptions)
            {
                return RemoveRangeByValueAsync(sortedSetRemoveRangeByValueOptions).Result;
            }

            #endregion

            #region Remove range by score

            /// <summary>
            /// Remove all members in the ordered set key whose score value is between min and max
            /// </summary>
            /// <param name="sortedSetRemoveRangeByScoreOptions">Sorted set remove range by score options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortedSetRemoveRangeByScoreResponse>> RemoveRangeByScoreAsync(SortedSetRemoveRangeByScoreOptions sortedSetRemoveRangeByScoreOptions)
            {
                return await ExecuteCommandAsync(sortedSetRemoveRangeByScoreOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Remove all members in the ordered set key whose score value is between min and max
            /// </summary>
            /// <param name="sortedSetRemoveRangeByScoreOptions">Sorted set remove range by score options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortedSetRemoveRangeByScoreResponse> RemoveRangeByScore(SortedSetRemoveRangeByScoreOptions sortedSetRemoveRangeByScoreOptions)
            {
                return RemoveRangeByScoreAsync(sortedSetRemoveRangeByScoreOptions).Result;
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
            public static async Task<CacheResult<SortedSetRemoveRangeByRankResponse>> RemoveRangeByRankAsync(SortedSetRemoveRangeByRankOptions sortedSetRemoveRangeByRankOptions)
            {
                return await ExecuteCommandAsync(sortedSetRemoveRangeByRankOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Remove all members in the specified rank range in the ordered set key
            /// The subscript parameters start and stop are both based on 0, that is, 0 means the first member of the ordered set, 1 means the second member of the ordered set, and so on. 
            /// You can also use negative subscripts, with -1 for the last member, -2 for the penultimate member, and so on.
            /// </summary>
            /// <param name="sortedSetRemoveRangeByRankOptions">Sorted set range by rank options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortedSetRemoveRangeByRankResponse> RemoveRangeByRank(SortedSetRemoveRangeByRankOptions sortedSetRemoveRangeByRankOptions)
            {
                return RemoveRangeByRankAsync(sortedSetRemoveRangeByRankOptions).Result;
            }

            #endregion

            #region Remove

            /// <summary>
            /// Remove the specified element in the ordered collection
            /// </summary>
            /// <param name="sortedSetRemoveOptions">Sorted set remove options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortedSetRemoveResponse>> RemoveAsync(SortedSetRemoveOptions sortedSetRemoveOptions)
            {
                return await ExecuteCommandAsync(sortedSetRemoveOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Remove the specified element in the ordered collection
            /// </summary>
            /// <param name="sortedSetRemoveOptions">Sorted set remove options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortedSetRemoveResponse> Remove(SortedSetRemoveOptions sortedSetRemoveOptions)
            {
                return RemoveAsync(sortedSetRemoveOptions).Result;
            }

            #endregion

            #region Rank

            /// <summary>
            /// Returns the ranking of the member member in the ordered set key. The members of the ordered set are arranged in order of increasing score value (from small to large) by default
            /// The ranking is based on 0, that is, the member with the smallest score is ranked 0
            /// </summary>
            /// <param name="sortedSetRankOptions">Sorted set rank options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortedSetRankResponse>> RankAsync(SortedSetRankOptions sortedSetRankOptions)
            {
                return await ExecuteCommandAsync(sortedSetRankOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns the ranking of the member member in the ordered set key. The members of the ordered set are arranged in order of increasing score value (from small to large) by default
            /// The ranking is based on 0, that is, the member with the smallest score is ranked 0
            /// </summary>
            /// <param name="sortedSetRankOptions">Sorted set rank options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortedSetRankResponse> Rank(SortedSetRankOptions sortedSetRankOptions)
            {
                return RankAsync(sortedSetRankOptions).Result;
            }

            #endregion

            #region Range by value

            /// <summary>
            /// Returns the elements in the ordered set between min and max
            /// </summary>
            /// <param name="sortedSetRangeByValueOptions">Sorted set range by value options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortedSetRangeByValueResponse>> RangeByValueAsync(SortedSetRangeByValueOptions sortedSetRangeByValueOptions)
            {
                return await ExecuteCommandAsync(sortedSetRangeByValueOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns the elements in the ordered set between min and max
            /// </summary>
            /// <param name="sortedSetRangeByValueOptions">Sorted set range by value options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortedSetRangeByValueResponse> RangeByValue(SortedSetRangeByValueOptions sortedSetRangeByValueOptions)
            {
                return RangeByValueAsync(sortedSetRangeByValueOptions).Result;
            }

            #endregion

            #region Range by score with scores

            /// <summary>
            /// Returns the value and score of the members in the specified interval in the ordered set key, the positions are arranged according to score
            /// </summary>
            /// <param name="sortedSetRangeByScoreWithScoresOptions">Sorted set range by score with scores options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortedSetRangeByScoreWithScoresResponse>> RangeByScoreWithScoresAsync(SortedSetRangeByScoreWithScoresOptions sortedSetRangeByScoreWithScoresOptions)
            {
                return await ExecuteCommandAsync(sortedSetRangeByScoreWithScoresOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns the value and score of the members in the specified interval in the ordered set key, the positions are arranged according to score
            /// </summary>
            /// <param name="sortedSetRangeByScoreWithScoresOptions">Sorted set range by score with scores options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortedSetRangeByScoreWithScoresResponse> RangeByScoreWithScores(SortedSetRangeByScoreWithScoresOptions sortedSetRangeByScoreWithScoresOptions)
            {
                return RangeByScoreWithScoresAsync(sortedSetRangeByScoreWithScoresOptions).Result;
            }

            #endregion

            #region Range by score

            /// <summary>
            /// Return the value of the members in the specified interval in the ordered set key, the position is arranged by score
            /// </summary>
            /// <param name="sortedSetRangeByScoreOptions">Sorted set range by score options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortedSetRangeByScoreResponse>> RangeByScoreAsync(SortedSetRangeByScoreOptions sortedSetRangeByScoreOptions)
            {
                return await ExecuteCommandAsync(sortedSetRangeByScoreOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Return the value of the members in the specified interval in the ordered set key, the position is arranged by score
            /// </summary>
            /// <param name="sortedSetRangeByScoreOptions">Sorted set range by score options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortedSetRangeByScoreResponse> RangeByScore(SortedSetRangeByScoreOptions sortedSetRangeByScoreOptions)
            {
                return RangeByScoreAsync(sortedSetRangeByScoreOptions).Result;
            }

            #endregion

            #region Range by rank with scores

            /// <summary>
            /// Returns the value and score of the members in the specified interval in the ordered set key, the positions are arranged according to score
            /// </summary>
            /// <param name="sortedSetRangeByRankWithScoresOptions">Sorted set range by rank with scores options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortedSetRangeByRankWithScoresResponse>> RangeByRankWithScoresAsync(SortedSetRangeByRankWithScoresOptions sortedSetRangeByRankWithScoresOptions)
            {
                return await ExecuteCommandAsync(sortedSetRangeByRankWithScoresOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns the value and score of the members in the specified interval in the ordered set key, the positions are arranged according to score
            /// </summary>
            /// <param name="sortedSetRangeByRankWithScoresOptions">Sorted set range by rank with scores options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortedSetRangeByRankWithScoresResponse> RangeByRankWithScores(SortedSetRangeByRankWithScoresOptions sortedSetRangeByRankWithScoresOptions)
            {
                return RangeByRankWithScoresAsync(sortedSetRangeByRankWithScoresOptions).Result;
            }

            #endregion

            #region Range by rank

            /// <summary>
            /// Return the value of the members in the specified interval in the ordered set key, the positions are arranged by score
            /// </summary>
            /// <param name="sortedSetRangeByRankOptions">Sorted set range by rank options</param>
            /// <returns>sorted set range by rank response</returns>
            public static async Task<CacheResult<SortedSetRangeByRankResponse>> RangeByRankAsync(SortedSetRangeByRankOptions sortedSetRangeByRankOptions)
            {
                return await ExecuteCommandAsync(sortedSetRangeByRankOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Return the value of the members in the specified interval in the ordered set key, the positions are arranged by score
            /// </summary>
            /// <param name="sortedSetRangeByRankOptions">Sorted set range by rank options</param>
            /// <returns>sorted set range by rank response</returns>
            public static CacheResult<SortedSetRangeByRankResponse> RangeByRank(SortedSetRangeByRankOptions sortedSetRangeByRankOptions)
            {
                return RangeByRankAsync(sortedSetRangeByRankOptions).Result;
            }

            #endregion

            #region Length by value

            /// <summary>
            /// Returns the number of members whose value is between min and max in the ordered set key
            /// </summary>
            /// <param name="sortedSetLengthByValueOptions">Sorted set length by value options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortedSetLengthByValueResponse>> LengthByValueAsync(SortedSetLengthByValueOptions sortedSetLengthByValueOptions)
            {
                return await ExecuteCommandAsync(sortedSetLengthByValueOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns the number of members whose value is between min and max in the ordered set key
            /// </summary>
            /// <param name="sortedSetLengthByValueOptions">Sorted set length by value options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortedSetLengthByValueResponse> LengthByValue(SortedSetLengthByValueOptions sortedSetLengthByValueOptions)
            {
                return LengthByValueAsync(sortedSetLengthByValueOptions).Result;
            }

            #endregion

            #region Length

            /// <summary>
            /// Returns the number of members in the ordered set key whose score value is between min and max (including the score value equal to min or max by default)
            /// </summary>
            /// <param name="sortedSetLengthOptions">Sorted set length options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortedSetLengthResponse>> LengthAsync(SortedSetLengthOptions sortedSetLengthOptions)
            {
                return await ExecuteCommandAsync(sortedSetLengthOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns the number of members in the ordered set key whose score value is between min and max (including the score value equal to min or max by default)
            /// </summary>
            /// <param name="sortedSetLengthOptions">Sorted set length options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortedSetLengthResponse> Length(SortedSetLengthOptions sortedSetLengthOptions)
            {
                return LengthAsync(sortedSetLengthOptions).Result;
            }

            #endregion

            #region Increment

            /// <summary>
            /// Add the incremental increment to the score value of the member of the ordered set key
            /// </summary>
            /// <param name="sortedSetIncrementOptions">Sorted set increment options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortedSetIncrementResponse>> IncrementAsync(SortedSetIncrementOptions sortedSetIncrementOptions)
            {
                return await ExecuteCommandAsync(sortedSetIncrementOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Add the incremental increment to the score value of the member of the ordered set key
            /// </summary>
            /// <param name="sortedSetIncrementOptions">Sorted set increment options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortedSetIncrementResponse> Increment(SortedSetIncrementOptions sortedSetIncrementOptions)
            {
                return IncrementAsync(sortedSetIncrementOptions).Result;
            }

            #endregion

            #region Decrement

            /// <summary>
            /// is the score value of the member of the ordered set key minus the increment increment
            /// </summary>
            /// <param name="sortedSetDecrementOptions">Sorted set decrement options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortedSetDecrementResponse>> DecrementAsync(SortedSetDecrementOptions sortedSetDecrementOptions)
            {
                return await ExecuteCommandAsync(sortedSetDecrementOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// is the score value of the member of the ordered set key minus the increment increment
            /// </summary>
            /// <param name="sortedSetDecrementOptions">Sorted set decrement options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortedSetDecrementResponse> Decrement(SortedSetDecrementOptions sortedSetDecrementOptions)
            {
                return DecrementAsync(sortedSetDecrementOptions).Result;
            }

            #endregion

            #region Combine and store

            /// <summary>
            /// Aggregate multiple ordered collections and save to destination
            /// </summary>
            /// <param name="sortedSetCombineAndStoreOptions">Sorted set combine and store options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortedSetCombineAndStoreResponse>> CombineAndStoreAsync(SortedSetCombineAndStoreOptions sortedSetCombineAndStoreOptions)
            {
                return await ExecuteCommandAsync(sortedSetCombineAndStoreOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Aggregate multiple ordered collections and save to destination
            /// </summary>
            /// <param name="sortedSetCombineAndStoreOptions">Sorted set combine and store options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortedSetCombineAndStoreResponse> CombineAndStore(SortedSetCombineAndStoreOptions sortedSetCombineAndStoreOptions)
            {
                return CombineAndStoreAsync(sortedSetCombineAndStoreOptions).Result;
            }

            #endregion

            #region Add

            /// <summary>
            /// Add one or more member elements and their score values ​​to the ordered set key
            /// </summary>
            /// <param name="sortedSetAddOptions">Sorted set add options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortedSetAddResponse>> AddAsync(SortedSetAddOptions sortedSetAddOptions)
            {
                return await ExecuteCommandAsync(sortedSetAddOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Add one or more member elements and their score values ​​to the ordered set key
            /// </summary>
            /// <param name="sortedSetAddOptions">Sorted set add options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortedSetAddResponse> Add(SortedSetAddOptions sortedSetAddOptions)
            {
                return AddAsync(sortedSetAddOptions).Result;
            }

            #endregion
        }

        #endregion

        #region Key

        /// <summary>
        /// Key operation
        /// </summary>
        public static class Keys
        {
            #region Sort

            /// <summary>
            /// Return or save the sorted elements in the given list, collection, ordered set key
            /// </summary>
            /// <param name="sortOptions">Sort options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortResponse>> SortAsync(SortOptions sortOptions)
            {
                return await ExecuteCommandAsync(sortOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Return or save the sorted elements in the given list, collection, ordered set key
            /// </summary>
            /// <param name="sortOptions">Sort options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortResponse> Sort(SortOptions sortOptions)
            {
                return SortAsync(sortOptions).Result;
            }

            #endregion

            #region Sort and store

            /// <summary>
            /// Return or save the sorted elements in the given list, collection, ordered set key, and save to the specified key value
            /// </summary>
            /// <param name="sortAndStoreOptions">Sort and store options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortAndStoreResponse>> SortAndStoreAsync(SortAndStoreOptions sortAndStoreOptions)
            {
                return await ExecuteCommandAsync(sortAndStoreOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Return or save the sorted elements in the given list, collection, ordered set key, and save to the specified key value
            /// </summary>
            /// <param name="sortAndStoreOptions">Sort and store options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortAndStoreResponse> SortAndStore(SortAndStoreOptions sortAndStoreOptions)
            {
                return SortAndStoreAsync(sortAndStoreOptions).Result;
            }

            #endregion

            #region Type

            /// <summary>
            /// Returns the type of value stored by key
            /// </summary>
            /// <param name="typeOptions">type options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<TypeResponse>> TypeAsync(TypeOptions typeOptions)
            {
                return await ExecuteCommandAsync(typeOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns the type of value stored by key
            /// </summary>
            /// <param name="typeOptions">type options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<TypeResponse> Type(TypeOptions typeOptions)
            {
                return TypeAsync(typeOptions).Result;
            }

            #endregion

            #region Time to live

            /// <summary>
            /// Return the remaining time to live for a given key (TTL, time to live)
            /// </summary>
            /// <param name="timeToLiveOptions">Time to live options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<TimeToLiveResponse>> TimeToLiveAsync(TimeToLiveOptions timeToLiveOptions)
            {
                return await ExecuteCommandAsync(timeToLiveOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Return the remaining time to live for a given key (TTL, time to live)
            /// </summary>
            /// <param name="timeToLiveOptions">Time to live options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<TimeToLiveResponse> TimeToLive(TimeToLiveOptions timeToLiveOptions)
            {
                return TimeToLiveAsync(timeToLiveOptions).Result;
            }

            #endregion

            #region Restore

            /// <summary>
            /// Deserialize the given serialized value and associate it with the given key
            /// </summary>
            /// <param name="restoreOptions">Restore options</param>
            /// <returns> Return cache result </returns>
            public static async Task<CacheResult<RestoreResponse>> RestoreAsync(RestoreOptions restoreOptions)
            {
                return await ExecuteCommandAsync(restoreOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Deserialize the given serialized value and associate it with the given key
            /// </summary>
            /// <param name="restoreOptions">Restore options</param>
            /// <returns> Return cache result </returns>
            public static CacheResult<RestoreResponse> Restore(RestoreOptions restoreOptions)
            {
                return RestoreAsync(restoreOptions).Result;
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
            public static async Task<CacheResult<RenameResponse>> RenameAsync(RenameOptions renameOptions)
            {
                return await ExecuteCommandAsync(renameOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Rename the key to newkey
            /// When the key and newkey are the same, or the key does not exist, an error is returned
            /// When newkey already exists, it will overwrite the old value
            /// </summary>
            /// <param name="renameOptions">Rename options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<RenameResponse> Rename(RenameOptions renameOptions)
            {
                return RenameAsync(renameOptions).Result;
            }

            #endregion

            #region Random

            /// <summary>
            /// randomly return (do not delete) a key
            /// </summary>
            /// <param name="randomOptions">Random options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<RandomResponse>> KeyRandomAsync(RandomOptions randomOptions)
            {
                return await ExecuteCommandAsync(randomOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// randomly return (do not delete) a key
            /// </summary>
            /// <param name="randomOptions">Random options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<RandomResponse> KeyRandom(RandomOptions randomOptions)
            {
                return KeyRandomAsync(randomOptions).Result;
            }

            #endregion

            #region Persist

            /// <summary>
            /// Remove the survival time of a given key, and convert this key from "volatile" (with survival time key) to "persistent" (a key with no survival time and never expires)
            /// </summary>
            /// <param name="persistOptions">Persist options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<PersistResponse>> PersistAsync(PersistOptions persistOptions)
            {
                return await ExecuteCommandAsync(persistOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Remove the survival time of a given key, and convert this key from "volatile" (with survival time key) to "persistent" (a key with no survival time and never expires)
            /// </summary>
            /// <param name="persistOptions">Persist options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<PersistResponse> Persist(PersistOptions persistOptions)
            {
                return PersistAsync(persistOptions).Result;
            }

            #endregion

            #region Move

            /// <summary>
            /// Move the key of the current database to the given database
            /// </summary>
            /// <param name="moveOptions">Move options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<MoveResponse>> MoveAsync(MoveOptions moveOptions)
            {
                return await ExecuteCommandAsync(moveOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Move the key of the current database to the given database
            /// </summary>
            /// <param name="moveOptions">Move options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<MoveResponse> Move(MoveOptions moveOptions)
            {
                return MoveAsync(moveOptions).Result;
            }

            #endregion

            #region Migrate

            /// <summary>
            /// Transfer the key atomically from the current instance to the specified database of the target instance. Once the transfer is successful, the key is guaranteed to appear on the target instance, and the key on the current instance will be deleted.
            /// </summary>
            /// <param name="migrateOptions">Migrate options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<MigrateResponse>> MigrateAsync(MigrateOptions migrateOptions)
            {
                return await ExecuteCommandAsync(migrateOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Transfer the key atomically from the current instance to the specified database of the target instance. Once the transfer is successful, the key is guaranteed to appear on the target instance, and the key on the current instance will be deleted.
            /// </summary>
            /// <param name="migrateOptions">Migrate options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<MigrateResponse> Migrate(MigrateOptions migrateOptions)
            {
                return MigrateAsync(migrateOptions).Result;
            }

            #endregion

            #region Expire

            /// <summary>
            /// Set the survival time for the given key. When the key expires (the survival time is 0), it will be automatically deleted
            /// </summary>
            /// <param name="expireOptions">Expire options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<ExpireResponse>> ExpireAsync(ExpireOptions expireOptions)
            {
                return await ExecuteCommandAsync(expireOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Set the survival time for the given key. When the key expires (the survival time is 0), it will be automatically deleted
            /// </summary>
            /// <param name="expireOptions">Expire options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<ExpireResponse> Expire(ExpireOptions expireOptions)
            {
                return ExpireAsync(expireOptions).Result;
            }

            #endregion;

            #region Dump

            /// <summary>
            /// Serialize the given key and return the serialized value. Use the RESTORE command to deserialize this value
            /// </summary>
            /// <param name="dumpOptions">Dump options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<DumpResponse>> DumpAsync(DumpOptions dumpOptions)
            {
                return await ExecuteCommandAsync(dumpOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Serialize the given key and return the serialized value. Use the RESTORE command to deserialize this value
            /// </summary>
            /// <param name="dumpOptions">Dump options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<DumpResponse> Dump(DumpOptions dumpOptions)
            {
                return DumpAsync(dumpOptions).Result;
            }

            #endregion

            #region Delete

            /// <summary>
            /// Delete the specified key
            /// </summary>
            /// <param name="deleteOptions">Delete options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<DeleteResponse>> DeleteAsync(DeleteOptions deleteOptions)
            {
                return await ExecuteCommandAsync(deleteOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Delete the specified key
            /// </summary>
            /// <param name="deleteOptions">Delete options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<DeleteResponse> Delete(DeleteOptions deleteOptions)
            {
                return DeleteAsync(deleteOptions).Result;
            }

            #endregion

            #region Get keys

            /// <summary>
            /// Find all keys that match a given pattern
            /// </summary>
            /// <param name="getKeysOptions">Get keys options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<GetKeysResponse>> GetKeysAsync(GetKeysOptions getKeysOptions)
            {
                return await ExecuteCommandAsync(getKeysOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Find all keys that match a given pattern
            /// </summary>
            /// <param name="getKeysOptions">Get keys options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<GetKeysResponse> GetKeys(GetKeysOptions getKeysOptions)
            {
                return GetKeysAsync(getKeysOptions).Result;
            }

            #endregion

            #region Exist

            /// <summary>
            /// Check whether key exist
            /// </summary>
            /// <param name="existOptions">Exist options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<ExistResponse>> ExistAsync(ExistOptions existOptions)
            {
                return await ExecuteCommandAsync(existOptions).ConfigureAwait(false);
            }

            /// <summary>
            /// Check whether key exist
            /// </summary>
            /// <param name="existOptions">Exist options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<ExistResponse> Exist(ExistOptions existOptions)
            {
                return ExistAsync(existOptions).Result;
            }

            #endregion
        }

        #endregion

        #region Server

        /// <summary>
        /// Cache server operation
        /// </summary>
        public static class Server
        {
            #region Get all data base

            /// <summary>
            /// Returns all cached databases in the server
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="getAllDataBaseOptions">Get all database options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<GetAllDataBaseResponse>> GetAllDataBaseAsync(CacheServer server, GetAllDataBaseOptions getAllDataBaseOptions)
            {
                return await getAllDataBaseOptions.ExecuteAsync(server).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns all cached databases in the server
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="getAllDataBaseOptions">Get all database options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<GetAllDataBaseResponse> GetAllDataBase(CacheServer server, GetAllDataBaseOptions getAllDataBaseOptions)
            {
                return GetAllDataBaseAsync(server, getAllDataBaseOptions).Result;
            }

            #endregion

            #region Query keys

            /// <summary>
            /// Query key value
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="getKeysOptions"> Get keys options </param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<GetKeysResponse>> GetKeysAsync(CacheServer server, GetKeysOptions getKeysOptions)
            {
                return await getKeysOptions.ExecuteAsync(server).ConfigureAwait(false);
            }

            /// <summary>
            /// Query key value
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="getKeysOptions"> Get keys options </param>
            /// <returns>Return cache result</returns>
            public static CacheResult<GetKeysResponse> GetKeys(CacheServer server, GetKeysOptions getKeysOptions)
            {
                return GetKeysAsync(server, getKeysOptions).Result;
            }

            #endregion

            #region Clear data

            /// <summary>
            /// Clear all data in the cache database
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="clearDataOptions"> Clear data options </param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<ClearDataResponse>> ClearDataAsync(CacheServer server, ClearDataOptions clearDataOptions)
            {
                return await clearDataOptions.ExecuteAsync(server).ConfigureAwait(false);
            }

            /// <summary>
            /// Clear all data in the cache database
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="clearDataOptions"> Clear data options </param>
            /// <returns>Return cache result</returns>
            public static CacheResult<ClearDataResponse> ClearData(CacheServer server, ClearDataOptions clearDataOptions)
            {
                return ClearDataAsync(server, clearDataOptions).Result;
            }

            #endregion

            #region Get cache item detail

            /// <summary>
            /// Get data item details
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="getDetailOptions"> Get detail options </param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<GetDetailResponse>> GetKeyDetailAsync(CacheServer server, GetDetailOptions getDetailOptions)
            {
                return await getDetailOptions.ExecuteAsync(server).ConfigureAwait(false);
            }

            /// <summary>
            /// Get data item details
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="getDetailOptions"> Get detail options </param>
            /// <returns>Return cache result</returns>
            public static CacheResult<GetDetailResponse> GetKeyDetail(CacheServer server, GetDetailOptions getDetailOptions)
            {
                return GetKeyDetailAsync(server, getDetailOptions).Result;
            }

            #endregion

            #region Get server configuration

            /// <summary>
            /// Get server configuration
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="getServerConfigurationOptions">Get server configuration options</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<GetServerConfigurationResponse>> GetServerConfigurationAsync(CacheServer server, GetServerConfigurationOptions getServerConfigurationOptions)
            {
                return await getServerConfigurationOptions.ExecuteAsync(server).ConfigureAwait(false);
            }

            /// <summary>
            /// Get server configuration
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="getServerConfigurationOptions">Get server configuration options</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<GetServerConfigurationResponse> GetServerConfiguration(CacheServer server, GetServerConfigurationOptions getServerConfigurationOptions)
            {
                return GetServerConfigurationAsync(server, getServerConfigurationOptions).Result;
            }

            #endregion

            #region Save server configuration

            /// <summary>
            /// Save the server configuration
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="saveServerConfigurationOptions"> Save server configuration options </param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SaveServerConfigurationResponse>> SaveServerConfigurationAsync(CacheServer server, SaveServerConfigurationOptions saveServerConfigurationOptions)
            {
                return await saveServerConfigurationOptions.ExecuteAsync(server).ConfigureAwait(false);
            }

            /// <summary>
            /// Save the server configuration
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="saveServerConfigOptions"> Save server configuration options </param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SaveServerConfigurationResponse> SaveServerConfiguration(CacheServer server, SaveServerConfigurationOptions saveServerConfigOptions)
            {
                return SaveServerConfigurationAsync(server, saveServerConfigOptions).Result;
            }

            #endregion
        }

        #endregion

        #region Configuration

        /// <summary>
        /// Cache configuration
        /// </summary>
        public static class Configuration
        {
            #region Fields

            /// <summary>
            /// Cache providers
            /// </summary>
            static readonly Dictionary<CacheServerType, ICacheProvider> Providers = new Dictionary<CacheServerType, ICacheProvider>();

            /// <summary>
            /// Get cache servers operation proxy
            /// </summary>
            static Func<ICacheOptions, List<CacheServer>> GetCacheServersProxy;

            /// <summary>
            /// Get global cache key prefix operation
            /// </summary>
            static Func<List<string>> GetGlobalCacheKeyPrefixs;

            /// <summary>
            /// Get cache object prefixs
            /// </summary>
            static Func<CacheObject, List<string>> GetCacheObjectPrefixs = o =>
            {
                string objectName = o?.ObjectName;
                if (!string.IsNullOrWhiteSpace(objectName))
                {
                    return new List<string>() { objectName };
                }
                return new List<string>(0);
            };

            /// <summary>
            /// Gets or sets the each key name split word
            /// </summary>
            public static string KeyNameSplitWord = ":";

            /// <summary>
            /// Gets or sets the name&value split word
            /// </summary>
            public static string NameValueSplitWord = "$";

            /// <summary>
            /// Gets or sets the cache default encoding
            /// </summary>
            public static Encoding DefaultEncoding = Encoding.UTF8;

            /// <summary>
            /// Gets or sets whether throw exception when not get any database
            /// </summary>
            public static bool ThrowNoDatabaseException = false;

            #endregion

            static Configuration()
            {
                ConfigureCacheProvider(CacheServerType.InMemory, new MemoryProvider());
            }

            #region Cache server

            /// <summary>
            /// Configure cache server
            /// </summary>
            /// <param name="getCacheServerOperation">Get cache server operation</param>
            public static void ConfigureCacheServer(Func<ICacheOptions, List<CacheServer>> getCacheServerOperation)
            {
                GetCacheServersProxy = getCacheServerOperation;
            }

            /// <summary>
            /// Get cache servers
            /// </summary>
            /// <param name="cacheOptions">Cache options</param>
            /// <returns>Return cache servers</returns>
            public static List<CacheServer> GetCacheServers<T>(CacheOptions<T> cacheOptions) where T : CacheResponse
            {
                return GetCacheServersProxy?.Invoke(cacheOptions) ?? new List<CacheServer>(0);
            }

            #endregion

            #region Cache provider

            /// <summary>
            /// Configure cache provider
            /// </summary>
            /// <param name="serverType">Cache server type</param>
            /// <param name="cacheProvider">Cache provider</param>
            public static void ConfigureCacheProvider(CacheServerType serverType, ICacheProvider cacheProvider)
            {
                Providers[serverType] = cacheProvider;
            }

            /// <summary>
            /// Get cache provider
            /// </summary>
            /// <param name="serverType">Server type</param>
            /// <returns>Return cache provider</returns>
            public static ICacheProvider GetCacheProvider(CacheServerType serverType)
            {
                Providers.TryGetValue(serverType, out var provider);
                return provider;
            }

            #endregion

            #region Cache key global prefix

            /// <summary>
            /// Config global cache key prefixs
            /// </summary>
            /// <param name="getGlobalCacheKeyPrefixsOperation">Get global cache key prefixs operation</param>
            public static void ConfigureGlobalPrefix(Func<List<string>> getGlobalCacheKeyPrefixsOperation)
            {
                GetGlobalCacheKeyPrefixs = getGlobalCacheKeyPrefixsOperation;
            }

            /// <summary>
            /// Get global cache key prefixs
            /// </summary>
            /// <returns>Return global cache key prefixs</returns>
            public static List<string> GetGlobalPrefixs()
            {
                return GetGlobalCacheKeyPrefixs?.Invoke() ?? new List<string>(0);
            }

            #endregion

            #region Object prefix

            public static void ConfigureObjectPrefix(Func<CacheObject, List<string>> getCacheObjectPrefixOperation)
            {
                GetCacheObjectPrefixs = getCacheObjectPrefixOperation;
            }

            /// <summary>
            /// Get cache object prefixs
            /// </summary>
            /// <param name="cacheObject">Cache object</param>
            /// <returns>Return cache object prefixs</returns>
            public static List<string> GetObjectPrefixs(CacheObject cacheObject)
            {
                return GetCacheObjectPrefixs?.Invoke(cacheObject) ?? new List<string>(0);
            }

            #endregion
        }

        #endregion

        #region Execute command

        /// <summary>
        /// Execute cache operation
        /// </summary>
        /// <param name="requestOptions">Request options</param>
        /// <returns>Reurn cache result</returns>
        static async Task<CacheResult<TResponse>> ExecuteCommandAsync<TResponse>(CacheOptions<TResponse> requestOptions) where TResponse : CacheResponse
        {
            if (requestOptions == null)
            {
                return CacheResult<TResponse>.EmptyResult();
            }
            return await requestOptions.ExecuteAsync().ConfigureAwait(false) ?? CacheResult<TResponse>.EmptyResult();
        }

        #endregion
    }
}