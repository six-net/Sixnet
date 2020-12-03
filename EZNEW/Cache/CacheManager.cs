using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EZNEW.Cache.Command.Result;
using EZNEW.Cache.Hash.Request;
using EZNEW.Cache.Hash.Response;
using EZNEW.Cache.Keys.Request;
using EZNEW.Cache.Keys.Response;
using EZNEW.Cache.List.Request;
using EZNEW.Cache.List.Response;
using EZNEW.Cache.Provider.Memory;
using EZNEW.Cache.Server.Request;
using EZNEW.Cache.Server.Response;
using EZNEW.Cache.Set.Request;
using EZNEW.Cache.Set.Response;
using EZNEW.Cache.SortedSet.Request;
using EZNEW.Cache.SortedSet.Response;
using EZNEW.Cache.String.Request;
using EZNEW.Cache.String.Response;
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
            /// <param name="stringSetRangeOption">Set range option</param>
            /// <returns>Return cache set result</returns>
            public static async Task<CacheResult<StringSetRangeResponse>> SetRangeAsync(StringSetRangeOption stringSetRangeOption)
            {
                return await ExecuteCommandAsync(stringSetRangeOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Starting with offset, overwrite the string value stored by the overwrite key with the value parameter,
            /// Make sure the string is long enough to set the value to the specified offset, 
            /// If the original string length stored by the key is smaller than the offset (say, the string is only 5 characters long, but you set the offset to 10),  
            /// the space between the original character and the offset will be filled with zerobytes (zerobytes, "\x00")
            /// </summary>
            /// <param name="stringSetRangeOption">Set range option</param>
            /// <returns>Return cache set result</returns>
            public static CacheResult<StringSetRangeResponse> SetRange(StringSetRangeOption stringSetRangeOption)
            {
                return SetRangeAsync(stringSetRangeOption).Result;
            }

            #endregion

            #region Set bit

            /// <summary>
            /// To set or clear the bit (bit) at the specified offset for the string value stored by the key,
            /// When the key does not exist, a new string value is automatically generated.
            /// The string is stretched (grown) to ensure that it can hold the value at the specified offset. When the string value is stretched, the empty space is filled with 0
            /// The offset parameter must be greater than or equal to 0 and less than 2^32
            /// </summary>
            /// <param name="stringSetBitOption">Set bit option</param>
            /// <returns>Return cache set result</returns>
            public static async Task<CacheResult<StringSetBitResponse>> SetBitAsync(StringSetBitOption stringSetBitOption)
            {
                return await ExecuteCommandAsync(stringSetBitOption).ConfigureAwait(false);
            }

            /// <summary>
            /// To set or clear the bit (bit) at the specified offset for the string value stored by the key,
            /// When the key does not exist, a new string value is automatically generated.
            /// The string is stretched (grown) to ensure that it can hold the value at the specified offset. When the string value is stretched, the empty space is filled with 0
            /// The offset parameter must be greater than or equal to 0 and less than 2^32
            /// </summary>
            /// <param name="stringSetBitOption">Set bit option</param>
            /// <returns>Return cache set result</returns>
            public static CacheResult<StringSetBitResponse> SetBit(StringSetBitOption stringSetBitOption)
            {
                return ExecuteCommandAsync(stringSetBitOption).Result;
            }

            #endregion

            #region Set

            /// <summary>
            /// Associate the string value value to the key
            /// If the key already holds other values, SET will overwrite the old values, regardless of the type
            /// When the SET command sets a key with a time to live (TTL), the original TTL of the key will be cleared
            /// </summary>
            /// <param name="stringSetOption">String set option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<StringSetResponse>> SetAsync(StringSetOption stringSetOption)
            {
                return await ExecuteCommandAsync(stringSetOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Associate the string value value to the key
            /// If the key already holds other values, overwrite the old values
            /// When the SET command sets a key with a time to live (TTL), the original TTL of the key will be cleared
            /// </summary>
            /// <param name="stringSetOption">String set option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<StringSetResponse> Set(StringSetOption stringSetOption)
            {
                return ExecuteCommandAsync(stringSetOption).Result;
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
                return await SetAsync(new StringSetOption()
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
                return await SetAsync(new StringSetOption()
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
            /// <param name="stringLengthOption">String length option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<StringLengthResponse>> LengthAsync(StringLengthOption stringLengthOption)
            {
                return await ExecuteCommandAsync(stringLengthOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns the length of the string value stored by key
            /// </summary>
            /// <param name="stringLengthOption">String length option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<StringLengthResponse> Length(StringLengthOption stringLengthOption)
            {
                return ExecuteCommandAsync(stringLengthOption).Result;
            }

            #endregion

            #region Increment

            /// <summary>
            /// Add one to the numeric value stored by the key
            /// If the key key does not exist, then its value will be initialized to 0 first, and then execute the command
            /// If the value stored by the key cannot be interpreted as a number, the command will return an error
            /// </summary>
            /// <param name="stringIncrementOption">String increment option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<StringIncrementResponse>> IncrementAsync(StringIncrementOption stringIncrementOption)
            {
                return await ExecuteCommandAsync(stringIncrementOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Add one to the numeric value stored by the key
            /// If the key key does not exist, then its value will be initialized to 0 first, and then execute the command
            /// If the value stored by the key cannot be interpreted as a number, the command will return an error
            /// </summary>
            /// <param name="stringIncrementOption">String increment option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<StringIncrementResponse> Increment(StringIncrementOption stringIncrementOption)
            {
                return ExecuteCommandAsync(stringIncrementOption).Result;
            }

            #endregion

            #region Get with expiry

            /// <summary>
            /// Returns the string value associated with the key
            /// If the key key does not exist, then return an empty string, otherwise, return the value of the key key
            /// If the value of key is not of type string, then return an error.
            /// </summary>
            /// <param name="stringGetWithExpiryOption">String get with expiry option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<StringGetWithExpiryResponse>> GetWithExpiryAsync(StringGetWithExpiryOption stringGetWithExpiryOption)
            {
                return await ExecuteCommandAsync(stringGetWithExpiryOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns the string value associated with the key
            /// If the key key does not exist, then return an empty string, otherwise, return the value of the key key
            /// If the value of key is not of type string, then return an error.
            /// </summary>
            /// <param name="stringGetWithExpiryOption">String get with expiry option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<StringGetWithExpiryResponse> GetWithExpiry(StringGetWithExpiryOption stringGetWithExpiryOption)
            {
                return ExecuteCommandAsync(stringGetWithExpiryOption).Result;
            }

            #endregion

            #region Get set

            /// <summary>
            /// Set the value of the key key to value and return the old value of the key key before it is set
            /// When the key key exists but is not a string type, the command returns an error
            /// </summary>
            /// <param name="stringGetSetOption">String get set option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<StringGetSetResponse>> GetSetAsync(StringGetSetOption stringGetSetOption)
            {
                return await ExecuteCommandAsync(stringGetSetOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Set the value of the key key to value and return the old value of the key key before it is set
            /// When the key key exists but is not a string type, the command returns an error
            /// </summary>
            /// <param name="stringGetSetOption">String get set option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<StringGetSetResponse> GetSet(StringGetSetOption stringGetSetOption)
            {
                return ExecuteCommandAsync(stringGetSetOption).Result;
            }

            #endregion

            #region Get range

            /// <summary>
            /// Returns the specified part of the string value stored by the key key, the range of the string interception is determined by the two offsets of start and end (including start and end)
            /// Negative offset means counting from the end of the string, -1 means the last character, -2 means the penultimate character, and so on
            /// </summary>
            /// <param name="stringGetRangeOption">String get range option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<StringGetRangeResponse>> GetRangeAsync(StringGetRangeOption stringGetRangeOption)
            {
                return await ExecuteCommandAsync(stringGetRangeOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns the specified part of the string value stored by the key key, the range of the string interception is determined by the two offsets of start and end (including start and end)
            /// Negative offset means counting from the end of the string, -1 means the last character, -2 means the penultimate character, and so on
            /// </summary>
            /// <param name="stringGetRangeOption">String get range option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<StringGetRangeResponse> GetRange(StringGetRangeOption stringGetRangeOption)
            {
                return ExecuteCommandAsync(stringGetRangeOption).Result;
            }

            #endregion

            #region Get bit

            /// <summary>
            /// For the string value stored in key, get the bit at the specified offset
            /// When offset is greater than the length of the string value, or the key does not exist, return 0
            /// </summary>
            /// <param name="stringGetBitOption">String get bit option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<StringGetBitResponse>> GetBitAsync(StringGetBitOption stringGetBitOption)
            {
                return await ExecuteCommandAsync(stringGetBitOption).ConfigureAwait(false);
            }

            /// <summary>
            /// For the string value stored in key, get the bit at the specified offset
            /// When offset is greater than the length of the string value, or the key does not exist, return 0
            /// </summary>
            /// <param name="stringGetBitOption">String get bit option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<StringGetBitResponse> GetBit(StringGetBitOption stringGetBitOption)
            {
                return ExecuteCommandAsync(stringGetBitOption).Result;
            }

            #endregion

            #region Get

            /// <summary>
            /// Returns the value of the given string key or keys
            /// </summary>
            /// <param name="stringGetOption">String get option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<StringGetResponse>> GetAsync(StringGetOption stringGetOption)
            {
                return await ExecuteCommandAsync(stringGetOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns the value of the given string key or keys
            /// </summary>
            /// <param name="stringGetOption">String get option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<StringGetResponse> Get(StringGetOption stringGetOption)
            {
                return GetAsync(stringGetOption).Result;
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
                var result = await GetAsync(new StringGetOption()
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
            /// <param name="stringDecrementOption">String decrement option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<StringDecrementResponse>> DecrementAsync(StringDecrementOption stringDecrementOption)
            {
                return await ExecuteCommandAsync(stringDecrementOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Minus one for the numeric value stored for the key
            /// If the key key does not exist, the value of the key key will be initialized to 0 first, and then perform the operation
            /// If the value stored by the key cannot be interpreted as a number, an error will be returned
            /// </summary>
            /// <param name="stringDecrementOption">String decrement option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<StringDecrementResponse> Decrement(StringDecrementOption stringDecrementOption)
            {
                return DecrementAsync(stringDecrementOption).Result;
            }

            #endregion

            #region Bit position

            /// <summary>
            /// Returns the position of the first binary bit in the bitmap
            /// By default, the command will detect the entire bitmap, but the user can also specify the range to be detected through the optional start parameter and end parameter
            /// </summary>
            /// <param name="stringBitPositionOption">String bit position option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<StringBitPositionResponse>> BitPositionAsync(StringBitPositionOption stringBitPositionOption)
            {
                return await ExecuteCommandAsync(stringBitPositionOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns the position of the first binary bit in the bitmap
            /// By default, the command will detect the entire bitmap, but the user can also specify the range to be detected through the optional start parameter and end parameter
            /// </summary>
            /// <param name="stringBitPositionOption">String bit position option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<StringBitPositionResponse> BitPosition(StringBitPositionOption stringBitPositionOption)
            {
                return BitPositionAsync(stringBitPositionOption).Result;
            }

            #endregion

            #region Bit operation

            /// <summary>
            /// Perform bit operations on one or more string keys that hold binary bits, and save the result to destkey
            /// Except NOT operation, other operations can accept one or more keys as input
            /// </summary>
            /// <param name="stringBitOperationOption">String bit operation option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<StringBitOperationResponse>> BitOperationAsync(StringBitOperationOption stringBitOperationOption)
            {
                return await ExecuteCommandAsync(stringBitOperationOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Perform bit operations on one or more string keys that hold binary bits, and save the result to destkey
            /// Except NOT operation, other operations can accept one or more keys as input
            /// </summary>
            /// <param name="stringBitOperationOption">String bit operation option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<StringBitOperationResponse> BitOperation(StringBitOperationOption stringBitOperationOption)
            {
                return BitOperationAsync(stringBitOperationOption).Result;
            }

            #endregion

            #region Bit count

            /// <summary>
            /// Calculate the number of bits set to 1 in a given string.
            /// Under normal circumstances, the given entire string will be counted, by specifying additional start or end parameters, you can make the count only on a specific bit
            /// </summary>
            /// <param name="stringBitCountOption">String bit count option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<StringBitCountResponse>> BitCountAsync(StringBitCountOption stringBitCountOption)
            {
                return await ExecuteCommandAsync(stringBitCountOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Calculate the number of bits set to 1 in a given string.
            /// Under normal circumstances, the given entire string will be counted, by specifying additional start or end parameters, you can make the count only on a specific bit
            /// </summary>
            /// <param name="stringBitCountOption">String bit count option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<StringBitCountResponse> BitCount(StringBitCountOption stringBitCountOption)
            {
                return BitCountAsync(stringBitCountOption).Result;
            }

            #endregion

            #region Append

            /// <summary>
            /// If the key key already exists and its value is a string, the value will be appended to the end of the key key's existing value
            /// If the key does not exist, simply set the value of the key key to value
            /// </summary>
            /// <param name="stringAppendOption">String append option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<StringAppendResponse>> AppendAsync(StringAppendOption stringAppendOption)
            {
                return await ExecuteCommandAsync(stringAppendOption).ConfigureAwait(false);
            }

            /// <summary>
            /// If the key key already exists and its value is a string, the value will be appended to the end of the key key's existing value
            /// If the key does not exist, simply set the value of the key key to value
            /// </summary>
            /// <param name="stringAppendOption">String append option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<StringAppendResponse> Append(StringAppendOption stringAppendOption)
            {
                return AppendAsync(stringAppendOption).Result;
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
            /// <param name="listTrimOption">List trim option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<ListTrimResponse>> TrimAsync(ListTrimOption listTrimOption)
            {
                return await ExecuteCommandAsync(listTrimOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Trim a list, that is, let the list keep only the elements in the specified interval, and elements that are not in the specified interval will be deleted
            /// The subscript (index) parameters start and stop are both based on 0, that is, 0 represents the first element of the list, 1 represents the second element of the list, and so on
            /// You can also use negative subscripts, with -1 for the last element of the list, -2 for the penultimate element of the list, and so on
            /// When key is not a list type, an error is returned
            /// </summary>
            /// <param name="listTrimOption">List trim option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<ListTrimResponse> Trim(ListTrimOption listTrimOption)
            {
                return TrimAsync(listTrimOption).Result;
            }

            #endregion

            #region Set by index

            /// <summary>
            /// Set the value of the element whose index of the list key is index to value
            /// When the index parameter is out of range, or an operation is performed on an empty list (the key does not exist), an error is returned
            /// </summary>
            /// <param name="listSetByIndexOption">List set by index option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<ListSetByIndexResponse>> SetByIndexAsync(ListSetByIndexOption listSetByIndexOption)
            {
                return await ExecuteCommandAsync(listSetByIndexOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Set the value of the element whose index of the list key is index to value
            /// When the index parameter is out of range, or an operation is performed on an empty list (the key does not exist), an error is returned
            /// </summary>
            /// <param name="listSetByIndexOption">List set by index option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<ListSetByIndexResponse> SetByIndex(ListSetByIndexOption listSetByIndexOption)
            {
                return SetByIndexAsync(listSetByIndexOption).Result;
            }

            #endregion

            #region Right push

            /// <summary>
            /// Insert one or more values ​​into the end of the list key (far right).
            /// If there are multiple value values, then each value value is inserted into the end of the table in order from left to right
            /// </summary>
            /// <param name="listRightPushOption">List right push option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<ListRightPushResponse>> RightPushAsync(ListRightPushOption listRightPushOption)
            {
                return await ExecuteCommandAsync(listRightPushOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Insert one or more values ​​into the end of the list key (far right).
            /// If there are multiple value values, then each value value is inserted into the end of the table in order from left to right
            /// </summary>
            /// <param name="listRightPushOption">List right push option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<ListRightPushResponse> RightPush(ListRightPushOption listRightPushOption)
            {
                return RightPushAsync(listRightPushOption).Result;
            }

            #endregion

            #region Right pop left push

            /// <summary>
            /// Pop the last element (tail element) in the list source and return it to the client
            /// Insert the element popped by source into the destination list as the head element of the destination list
            /// </summary>
            /// <param name="listRightPopLeftPushOption">List right pop left push option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<ListRightPopLeftPushResponse>> RightPopLeftPushAsync(ListRightPopLeftPushOption listRightPopLeftPushOption)
            {
                return await ExecuteCommandAsync(listRightPopLeftPushOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Pop the last element (tail element) in the list source and return it to the client
            /// Insert the element popped by source into the destination list as the head element of the destination list
            /// </summary>
            /// <param name="listRightPopLeftPushOption">List right pop left push option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<ListRightPopLeftPushResponse> RightPopLeftPush(ListRightPopLeftPushOption listRightPopLeftPushOption)
            {
                return RightPopLeftPushAsync(listRightPopLeftPushOption).Result;
            }

            #endregion

            #region Right pop

            /// <summary>
            /// Remove and return the tail element of the list key.
            /// </summary>
            /// <param name="listRightPopOption">List right pop option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<ListRightPopResponse>> RightPopAsync(ListRightPopOption listRightPopOption)
            {
                return await ExecuteCommandAsync(listRightPopOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Remove and return the tail element of the list key.
            /// </summary>
            /// <param name="listRightPopOption">List right pop option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<ListRightPopResponse> RightPop(ListRightPopOption listRightPopOption)
            {
                return RightPopAsync(listRightPopOption).Result;
            }

            #endregion

            #region Remove

            /// <summary>
            /// According to the value of the parameter count, remove the elements equal to the parameter value in the list
            /// count is greater than 0: search from the beginning of the table to the end of the table, remove the elements equal to value, the number is count
            /// count is less than 0: search from the end of the table to the head of the table, remove the elements equal to value, the number is the absolute value of count
            /// count equals 0: remove all values ​​in the table that are equal to value
            /// </summary>
            /// <param name="listRemoveOption">List remove option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<ListRemoveResponse>> RemoveAsync(ListRemoveOption listRemoveOption)
            {
                return await ExecuteCommandAsync(listRemoveOption).ConfigureAwait(false);
            }

            /// <summary>
            /// According to the value of the parameter count, remove the elements equal to the parameter value in the list
            /// count is greater than 0: search from the beginning of the table to the end of the table, remove the elements equal to value, the number is count
            /// count is less than 0: search from the end of the table to the head of the table, remove the elements equal to value, the number is the absolute value of count
            /// count equals 0: remove all values ​​in the table that are equal to value
            /// </summary>
            /// <param name="listRemoveOption">List remove option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<ListRemoveResponse> Remove(ListRemoveOption listRemoveOption)
            {
                return RemoveAsync(listRemoveOption).Result;
            }

            #endregion

            #region Range

            /// <summary>
            /// Return the elements in the specified interval in the list key, the interval is specified by the offset start and stop
            /// The subscript (index) parameters start and stop are both based on 0, that is, 0 represents the first element of the list, 1 represents the second element of the list, and so on
            /// You can also use negative subscripts, with -1 for the last element of the list, -2 for the penultimate element of the list, and so on
            /// </summary>
            /// <param name="listRangeOption">List range option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<ListRangeResponse>> RangeAsync(ListRangeOption listRangeOption)
            {
                return await ExecuteCommandAsync(listRangeOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Return the elements in the specified interval in the list key, the interval is specified by the offset start and stop
            /// The subscript (index) parameters start and stop are both based on 0, that is, 0 represents the first element of the list, 1 represents the second element of the list, and so on
            /// You can also use negative subscripts, with -1 for the last element of the list, -2 for the penultimate element of the list, and so on
            /// </summary>
            /// <param name="listRangeOption">List range option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<ListRangeResponse> Range(ListRangeOption listRangeOption)
            {
                return RangeAsync(listRangeOption).Result;
            }

            #endregion

            #region Length

            /// <summary>
            /// Return the length of the list key
            /// If the key does not exist, the key is interpreted as an empty list and returns 0 
            /// If key is not a list type, return an error.
            /// </summary>
            /// <param name="listLengthOption">List length option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<ListLengthResponse>> LengthAsync(ListLengthOption listLengthOption)
            {
                return await ExecuteCommandAsync(listLengthOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Return the length of the list key
            /// If the key does not exist, the key is interpreted as an empty list and returns 0 
            /// If key is not a list type, return an error.
            /// </summary>
            /// <param name="listLengthOption">List length option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<ListLengthResponse> Length(ListLengthOption listLengthOption)
            {
                return LengthAsync(listLengthOption).Result;
            }

            #endregion

            #region Left push

            /// <summary>
            /// Insert one or more values ​​into the header of the list key
            /// If the key does not exist, an empty list will be created and the operation will be performed
            /// </summary>
            /// <param name="listLeftPushOption">List left push option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<ListLeftPushResponse>> LeftPushAsync(ListLeftPushOption listLeftPushOption)
            {
                return await ExecuteCommandAsync(listLeftPushOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Insert one or more values ​​into the header of the list key
            /// If the key does not exist, an empty list will be created and the operation will be performed
            /// </summary>
            /// <param name="listLeftPushOption">List left push option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<ListLeftPushResponse> LeftPush(ListLeftPushOption listLeftPushOption)
            {
                return LeftPushAsync(listLeftPushOption).Result;
            }

            #endregion

            #region Left pop

            /// <summary>
            /// Remove and return the head element of the list key
            /// </summary>
            /// <param name="listLeftPopOption">List left pop option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<ListLeftPopResponse>> LeftPopAsync(ListLeftPopOption listLeftPopOption)
            {
                return await ExecuteCommandAsync(listLeftPopOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Remove and return the head element of the list key
            /// </summary>
            /// <param name="listLeftPopOption">List left pop option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<ListLeftPopResponse> LeftPop(ListLeftPopOption listLeftPopOption)
            {
                return LeftPopAsync(listLeftPopOption).Result;
            }

            #endregion

            #region Insert before

            /// <summary>
            /// Insert the value value into the list key, before the value pivot
            /// </summary>
            /// <param name="listInsertBeforeOption">List insert before option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<ListInsertBeforeResponse>> InsertBeforeAsync(ListInsertBeforeOption listInsertBeforeOption)
            {
                return await ExecuteCommandAsync(listInsertBeforeOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Insert the value value into the list key, before the value pivot
            /// </summary>
            /// <param name="listInsertBeforeOption">List insert before option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<ListInsertBeforeResponse> InsertBefore(ListInsertBeforeOption listInsertBeforeOption)
            {
                return InsertBeforeAsync(listInsertBeforeOption).Result;
            }

            #endregion

            #region Insert after

            /// <summary>
            /// Insert the value value into the list key, after the value pivot
            /// </summary>
            /// <param name="listInsertAfterOption">List insert after option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<ListInsertAfterResponse>> InsertAfterAsync(ListInsertAfterOption listInsertAfterOption)
            {
                return await ExecuteCommandAsync(listInsertAfterOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Insert the value value into the list key, after the value pivot
            /// </summary>
            /// <param name="listInsertAfterOption">List insert after option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<ListInsertAfterResponse> InsertAfter(ListInsertAfterOption listInsertAfterOption)
            {
                return InsertAfterAsync(listInsertAfterOption).Result;
            }

            #endregion

            #region Get by index

            /// <summary>
            /// Return the element with index index in the list key
            /// The subscript (index) parameters start and stop are both based on 0, that is, 0 represents the first element of the list, 1 represents the second element of the list, and so on
            /// You can also use negative subscripts, with -1 for the last element of the list, -2 for the penultimate element of the list, and so on
            /// </summary>
            /// <param name="listGetByIndexOption">List get by index option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<ListGetByIndexResponse>> GetByIndexAsync(ListGetByIndexOption listGetByIndexOption)
            {
                return await ExecuteCommandAsync(listGetByIndexOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Return the element with index index in the list key
            /// The subscript (index) parameters start and stop are both based on 0, that is, 0 represents the first element of the list, 1 represents the second element of the list, and so on
            /// You can also use negative subscripts, with -1 for the last element of the list, -2 for the penultimate element of the list, and so on
            /// </summary>
            /// <param name="listGetByIndexOption">List get by index option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<ListGetByIndexResponse> GetByIndex(ListGetByIndexOption listGetByIndexOption)
            {
                return GetByIndexAsync(listGetByIndexOption).Result;
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
            /// <param name="hashValuesOption">Hash values option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<HashValuesResponse>> ValuesAsync(HashValuesOption hashValuesOption)
            {
                return await ExecuteCommandAsync(hashValuesOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Return the values ​​of all fields in the hash table key
            /// </summary>
            /// <param name="hashValuesOption">Hash values option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<HashValuesResponse> Values(HashValuesOption hashValuesOption)
            {
                return ValuesAsync(hashValuesOption).Result;
            }

            #endregion

            #region Set

            /// <summary>
            /// Set the value of the field field in the hash table hash to value
            /// If the given hash table does not exist, then a new hash table will be created and perform the operation
            /// If the field field already exists in the hash table, its old value will be overwritten by the new value
            /// </summary>
            /// <param name="hashSetOption">Hash set option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<HashSetResponse>> SetAsync(HashSetOption hashSetOption)
            {
                return await ExecuteCommandAsync(hashSetOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Set the value of the field field in the hash table hash to value
            /// If the given hash table does not exist, then a new hash table will be created and perform the operation
            /// If the field field already exists in the hash table, its old value will be overwritten by the new value
            /// </summary>
            /// <param name="hashSetOption">Hash set option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<HashSetResponse> Set(HashSetOption hashSetOption)
            {
                return SetAsync(hashSetOption).Result;
            }

            #endregion

            #region Length

            /// <summary>
            /// returns the number of fields in the hash table key
            /// </summary>
            /// <param name="hashLengthOption">Hash length option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<HashLengthResponse>> LengthAsync(HashLengthOption hashLengthOption)
            {
                return await ExecuteCommandAsync(hashLengthOption).ConfigureAwait(false);
            }

            /// <summary>
            /// returns the number of fields in the hash table key
            /// </summary>
            /// <param name="hashLengthOption">Hash length option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<HashLengthResponse> Length(HashLengthOption hashLengthOption)
            {
                return LengthAsync(hashLengthOption).Result;
            }

            #endregion

            #region Keys

            /// <summary>
            /// Return all keys in the hash table key
            /// </summary>
            /// <param name="hashKeysOption">Hash keys option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<HashKeysResponse>> KeysAsync(HashKeysOption hashKeysOption)
            {
                return await ExecuteCommandAsync(hashKeysOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Return all keys in the hash table key
            /// </summary>
            /// <param name="hashKeysOption">Hash keys option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<HashKeysResponse> Keys(HashKeysOption hashKeysOption)
            {
                return KeysAsync(hashKeysOption).Result;
            }

            #endregion

            #region Increment

            /// <summary>
            /// Add incremental increment to the value of the field field in the hash table key
            /// </summary>
            /// <param name="hashIncrementOption">Hash increment option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<HashIncrementResponse>> IncrementAsync(HashIncrementOption hashIncrementOption)
            {
                return await ExecuteCommandAsync(hashIncrementOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Add incremental increment to the value of the field field in the hash table key
            /// </summary>
            /// <param name="hashIncrementOption">Hash increment option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<HashIncrementResponse> Increment(HashIncrementOption hashIncrementOption)
            {
                return IncrementAsync(hashIncrementOption).Result;
            }

            #endregion

            #region Get

            /// <summary>
            /// Returns the value of the given field in the hash table
            /// </summary>
            /// <param name="hashGetOption">Hash get option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<HashGetResponse>> GetAsync(HashGetOption hashGetOption)
            {
                return await ExecuteCommandAsync(hashGetOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns the value of the given field in the hash table
            /// </summary>
            /// <param name="hashGetOption">Hash get option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<HashGetResponse> Get(HashGetOption hashGetOption)
            {
                return GetAsync(hashGetOption).Result;
            }

            #endregion

            #region Get all

            /// <summary>
            /// Return the values ​​of all fields in the hash table key
            /// </summary>
            /// <param name="hashGetAllOption">Hash get all option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<HashGetAllResponse>> GetAllAsync(HashGetAllOption hashGetAllOption)
            {
                return await ExecuteCommandAsync(hashGetAllOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Return the values ​​of all fields in the hash table key
            /// </summary>
            /// <param name="hashGetAllOption">Hash get all option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<HashGetAllResponse> GetAll(HashGetAllOption hashGetAllOption)
            {
                return GetAllAsync(hashGetAllOption).Result;
            }

            #endregion

            #region Exist

            /// <summary>
            /// Check if the given field exists in the hash of the hash table
            /// </summary>
            /// <param name="hashExistsOption">Hash exists option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<HashExistsResponse>> ExistAsync(HashExistsOption hashExistsOption)
            {
                return await ExecuteCommandAsync(hashExistsOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Check if the given field exists in the hash of the hash table
            /// </summary>
            /// <param name="hashExistsOption">Hash exists option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<HashExistsResponse> Exist(HashExistsOption hashExistsOption)
            {
                return ExistAsync(hashExistsOption).Result;
            }

            #endregion

            #region Delete

            /// <summary>
            /// Delete one or more specified fields in the hash table key, the non-existing fields will be ignored
            /// </summary>
            /// <param name="hashDeleteOption">Hash delete option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<HashDeleteResponse>> DeleteAsync(HashDeleteOption hashDeleteOption)
            {
                return await ExecuteCommandAsync(hashDeleteOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Delete one or more specified fields in the hash table key, the non-existing fields will be ignored
            /// </summary>
            /// <param name="hashDeleteOption">Hash delete option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<HashDeleteResponse> Delete(HashDeleteOption hashDeleteOption)
            {
                return DeleteAsync(hashDeleteOption).Result;
            }

            #endregion

            #region Decrement

            /// <summary>
            /// Is the value of the field in the hash table key minus the increment increment
            /// </summary>
            /// <param name="hashDecrementOption">Hash decrement option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<HashDecrementResponse>> DecrementAsync(HashDecrementOption hashDecrementOption)
            {
                return await ExecuteCommandAsync(hashDecrementOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Is the value of the field in the hash table key minus the increment increment
            /// </summary>
            /// <param name="hashDecrementOption">Hash decrement option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<HashDecrementResponse> Decrement(HashDecrementOption hashDecrementOption)
            {
                return DecrementAsync(hashDecrementOption).Result;
            }

            #endregion

            #region Scan

            /// <summary>
            /// Each element returned is a key-value pair, a key-value pair consists of a key and a value
            /// </summary>
            /// <param name="hashScanOption">Hash scan option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<HashScanResponse>> ScanAsync(HashScanOption hashScanOption)
            {
                return await ExecuteCommandAsync(hashScanOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Each element returned is a key-value pair, a key-value pair consists of a key and a value
            /// </summary>
            /// <param name="hashScanOption">Hash scan option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<HashScanResponse> Scan(HashScanOption hashScanOption)
            {
                return ScanAsync(hashScanOption).Result;
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
            /// <param name="setRemoveOption">Set remove option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SetRemoveResponse>> RemoveAsync(SetRemoveOption setRemoveOption)
            {
                return await ExecuteCommandAsync(setRemoveOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Remove one or more member elements in the collection key, non-existent member elements will be ignored
            /// </summary>
            /// <param name="setRemoveOption">Set remove option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SetRemoveResponse> Remove(SetRemoveOption setRemoveOption)
            {
                return RemoveAsync(setRemoveOption).Result;
            }

            #endregion

            #region Random members

            /// <summary>
            /// Then return a set of random elements in the collection
            /// </summary>
            /// <param name="setRandomMembersOption">Set random members option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SetRandomMembersResponse>> RandomMembersAsync(SetRandomMembersOption setRandomMembersOption)
            {
                return await ExecuteCommandAsync(setRandomMembersOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Return a set of random elements in the collection
            /// </summary>
            /// <param name="setRandomMembersOption">Set random members option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SetRandomMembersResponse> RandomMembers(SetRandomMembersOption setRandomMembersOption)
            {
                return RandomMembersAsync(setRandomMembersOption).Result;
            }

            #endregion

            #region Random member

            /// <summary>
            /// Returns a random element in the collection
            /// </summary>
            /// <param name="setRandomMemberOption">Set random member option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SetRandomMemberResponse>> RandomMemberAsync(SetRandomMemberOption setRandomMemberOption)
            {
                return await ExecuteCommandAsync(setRandomMemberOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns a random element in the collection
            /// </summary>
            /// <param name="setRandomMemberOption">Set random member option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SetRandomMemberResponse> RandomMember(SetRandomMemberOption setRandomMemberOption)
            {
                return RandomMemberAsync(setRandomMemberOption).Result;
            }

            #endregion

            #region Pop

            /// <summary>
            /// Remove and return a random element in the collection
            /// </summary>
            /// <param name="setPopOption">Set pop option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SetPopResponse>> PopAsync(SetPopOption setPopOption)
            {
                return await ExecuteCommandAsync(setPopOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Remove and return a random element in the collection
            /// </summary>
            /// <param name="setPopOption">Set pop option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SetPopResponse> Pop(SetPopOption setPopOption)
            {
                return PopAsync(setPopOption).Result;
            }

            #endregion

            #region Move

            /// <summary>
            /// Move the member element from the source collection to the destination collection
            /// </summary>
            /// <param name="setMoveOption">Set move option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SetMoveResponse>> MoveAsync(SetMoveOption setMoveOption)
            {
                return await ExecuteCommandAsync(setMoveOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Move the member element from the source collection to the destination collection
            /// </summary>
            /// <param name="setMoveOption">Set move option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SetMoveResponse> Move(SetMoveOption setMoveOption)
            {
                return MoveAsync(setMoveOption).Result;
            }

            #endregion

            #region Members

            /// <summary>
            /// Return all members in the collection key
            /// </summary>
            /// <param name="setMembersOption">Set members option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SetMembersResponse>> MembersAsync(SetMembersOption setMembersOption)
            {
                return await ExecuteCommandAsync(setMembersOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Return all members in the collection key
            /// </summary>
            /// <param name="setMembersOption">Set members option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SetMembersResponse> Members(SetMembersOption setMembersOption)
            {
                return MembersAsync(setMembersOption).Result;
            }

            #endregion

            #region Length

            /// <summary>
            /// Returns the number of elements in the collection
            /// </summary>
            /// <param name="setLengthOption">Set length option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SetLengthResponse>> LengthAsync(SetLengthOption setLengthOption)
            {
                return await ExecuteCommandAsync(setLengthOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns the number of elements in the collection
            /// </summary>
            /// <param name="setLengthOption">Set length option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SetLengthResponse> Length(SetLengthOption setLengthOption)
            {
                return LengthAsync(setLengthOption).Result;
            }

            #endregion

            #region Contains

            /// <summary>
            /// Determine whether the member element is a member of the set key
            /// </summary>
            /// <param name="setContainsOption">Set contaims option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SetContainsResponse>> ContainsAsync(SetContainsOption setContainsOption)
            {
                return await ExecuteCommandAsync(setContainsOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Determine whether the member element is a member of the set key
            /// </summary>
            /// <param name="setContainsOption">Set contaims option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SetContainsResponse> Contains(SetContainsOption setContainsOption)
            {
                return ContainsAsync(setContainsOption).Result;
            }

            #endregion

            #region Combine

            /// <summary>
            /// According to the operation mode, return the processing results of multiple collections
            /// </summary>
            /// <param name="setCombineOption">Set combine option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SetCombineResponse>> CombineAsync(SetCombineOption setCombineOption)
            {
                return await ExecuteCommandAsync(setCombineOption).ConfigureAwait(false);
            }

            /// <summary>
            /// According to the operation mode, return the processing results of multiple collections
            /// </summary>
            /// <param name="setCombineOption">Set combine option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SetCombineResponse> Combine(SetCombineOption setCombineOption)
            {
                return CombineAsync(setCombineOption).Result;
            }

            #endregion

            #region Combine and store

            /// <summary>
            /// Return the processing results of multiple collections according to the operation mode, and store the results to the specified key value at the same time
            /// </summary>
            /// <param name="setCombineAndStoreOption">Set combine and store option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SetCombineAndStoreResponse>> CombineAndStoreAsync(SetCombineAndStoreOption setCombineAndStoreOption)
            {
                return await ExecuteCommandAsync(setCombineAndStoreOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Return the processing results of multiple collections according to the operation mode, and store the results to the specified key value at the same time
            /// </summary>
            /// <param name="setCombineAndStoreOption">Set combine and store option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SetCombineAndStoreResponse> CombineAndStore(SetCombineAndStoreOption setCombineAndStoreOption)
            {
                return CombineAndStoreAsync(setCombineAndStoreOption).Result;
            }

            #endregion

            #region Add

            /// <summary>
            /// Add one or more member elements to the collection key, the member elements already in the collection will be ignored
            /// If the key does not exist, create a collection that contains only the member element as a member
            /// </summary>
            /// <param name="setAddOption">Set add option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SetAddResponse>> AddAsync(SetAddOption setAddOption)
            {
                return await ExecuteCommandAsync(setAddOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Add one or more member elements to the collection key, the member elements already in the collection will be ignored
            /// If the key does not exist, create a collection that contains only the member element as a member
            /// </summary>
            /// <param name="setAddOption">Set add option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SetAddResponse> Add(SetAddOption setAddOption)
            {
                return AddAsync(setAddOption).Result;
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
            /// <param name="sortedSetScoreOption">Sorted set score option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortedSetScoreResponse>> ScoreAsync(SortedSetScoreOption sortedSetScoreOption)
            {
                return await ExecuteCommandAsync(sortedSetScoreOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns the score value of the member member in the ordered set key
            /// </summary>
            /// <param name="sortedSetScoreOption">Sorted set score option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortedSetScoreResponse> Score(SortedSetScoreOption sortedSetScoreOption)
            {
                return ScoreAsync(sortedSetScoreOption).Result;
            }

            #endregion

            #region Remove range by value

            /// <summary>
            /// Remove the elements in the specified range after sorting the elements
            /// </summary>
            /// <param name="sortedSetRemoveRangeByValueOption">Sorted set remove range by value option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortedSetRemoveRangeByValueResponse>> RemoveRangeByValueAsync(SortedSetRemoveRangeByValueOption sortedSetRemoveRangeByValueOption)
            {
                return await ExecuteCommandAsync(sortedSetRemoveRangeByValueOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Remove the elements in the specified range after sorting the elements
            /// </summary>
            /// <param name="sortedSetRemoveRangeByValueOption">Sorted set remove range by value option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortedSetRemoveRangeByValueResponse> RemoveRangeByValue(SortedSetRemoveRangeByValueOption sortedSetRemoveRangeByValueOption)
            {
                return RemoveRangeByValueAsync(sortedSetRemoveRangeByValueOption).Result;
            }

            #endregion

            #region Remove range by score

            /// <summary>
            /// Remove all members in the ordered set key whose score value is between min and max
            /// </summary>
            /// <param name="sortedSetRemoveRangeByScoreOption">Sorted set remove range by score option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortedSetRemoveRangeByScoreResponse>> RemoveRangeByScoreAsync(SortedSetRemoveRangeByScoreOption sortedSetRemoveRangeByScoreOption)
            {
                return await ExecuteCommandAsync(sortedSetRemoveRangeByScoreOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Remove all members in the ordered set key whose score value is between min and max
            /// </summary>
            /// <param name="sortedSetRemoveRangeByScoreOption">Sorted set remove range by score option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortedSetRemoveRangeByScoreResponse> RemoveRangeByScore(SortedSetRemoveRangeByScoreOption sortedSetRemoveRangeByScoreOption)
            {
                return RemoveRangeByScoreAsync(sortedSetRemoveRangeByScoreOption).Result;
            }

            #endregion

            #region Remove range by rank

            /// <summary>
            /// Remove all members in the specified rank range in the ordered set key
            /// The subscript parameters start and stop are both based on 0, that is, 0 means the first member of the ordered set, 1 means the second member of the ordered set, and so on. 
            /// You can also use negative subscripts, with -1 for the last member, -2 for the penultimate member, and so on.
            /// </summary>
            /// <param name="sortedSetRemoveRangeByRankOption">Sorted set range by rank option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortedSetRemoveRangeByRankResponse>> RemoveRangeByRankAsync(SortedSetRemoveRangeByRankOption sortedSetRemoveRangeByRankOption)
            {
                return await ExecuteCommandAsync(sortedSetRemoveRangeByRankOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Remove all members in the specified rank range in the ordered set key
            /// The subscript parameters start and stop are both based on 0, that is, 0 means the first member of the ordered set, 1 means the second member of the ordered set, and so on. 
            /// You can also use negative subscripts, with -1 for the last member, -2 for the penultimate member, and so on.
            /// </summary>
            /// <param name="sortedSetRemoveRangeByRankOption">Sorted set range by rank option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortedSetRemoveRangeByRankResponse> RemoveRangeByRank(SortedSetRemoveRangeByRankOption sortedSetRemoveRangeByRankOption)
            {
                return RemoveRangeByRankAsync(sortedSetRemoveRangeByRankOption).Result;
            }

            #endregion

            #region Remove

            /// <summary>
            /// Remove the specified element in the ordered collection
            /// </summary>
            /// <param name="sortedSetRemoveOption">Sorted set remove option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortedSetRemoveResponse>> RemoveAsync(SortedSetRemoveOption sortedSetRemoveOption)
            {
                return await ExecuteCommandAsync(sortedSetRemoveOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Remove the specified element in the ordered collection
            /// </summary>
            /// <param name="sortedSetRemoveOption">Sorted set remove option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortedSetRemoveResponse> Remove(SortedSetRemoveOption sortedSetRemoveOption)
            {
                return RemoveAsync(sortedSetRemoveOption).Result;
            }

            #endregion

            #region Rank

            /// <summary>
            /// Returns the ranking of the member member in the ordered set key. The members of the ordered set are arranged in order of increasing score value (from small to large) by default
            /// The ranking is based on 0, that is, the member with the smallest score is ranked 0
            /// </summary>
            /// <param name="sortedSetRankOption">Sorted set rank option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortedSetRankResponse>> RankAsync(SortedSetRankOption sortedSetRankOption)
            {
                return await ExecuteCommandAsync(sortedSetRankOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns the ranking of the member member in the ordered set key. The members of the ordered set are arranged in order of increasing score value (from small to large) by default
            /// The ranking is based on 0, that is, the member with the smallest score is ranked 0
            /// </summary>
            /// <param name="sortedSetRankOption">Sorted set rank option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortedSetRankResponse> Rank(SortedSetRankOption sortedSetRankOption)
            {
                return RankAsync(sortedSetRankOption).Result;
            }

            #endregion

            #region Range by value

            /// <summary>
            /// Returns the elements in the ordered set between min and max
            /// </summary>
            /// <param name="sortedSetRangeByValueOption">Sorted set range by value option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortedSetRangeByValueResponse>> RangeByValueAsync(SortedSetRangeByValueOption sortedSetRangeByValueOption)
            {
                return await ExecuteCommandAsync(sortedSetRangeByValueOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns the elements in the ordered set between min and max
            /// </summary>
            /// <param name="sortedSetRangeByValueOption">Sorted set range by value option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortedSetRangeByValueResponse> RangeByValue(SortedSetRangeByValueOption sortedSetRangeByValueOption)
            {
                return RangeByValueAsync(sortedSetRangeByValueOption).Result;
            }

            #endregion

            #region Range by score with scores

            /// <summary>
            /// Returns the value and score of the members in the specified interval in the ordered set key, the positions are arranged according to score
            /// </summary>
            /// <param name="sortedSetRangeByScoreWithScoresOption">Sorted set range by score with scores option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortedSetRangeByScoreWithScoresResponse>> RangeByScoreWithScoresAsync(SortedSetRangeByScoreWithScoresOption sortedSetRangeByScoreWithScoresOption)
            {
                return await ExecuteCommandAsync(sortedSetRangeByScoreWithScoresOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns the value and score of the members in the specified interval in the ordered set key, the positions are arranged according to score
            /// </summary>
            /// <param name="sortedSetRangeByScoreWithScoresOption">Sorted set range by score with scores option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortedSetRangeByScoreWithScoresResponse> RangeByScoreWithScores(SortedSetRangeByScoreWithScoresOption sortedSetRangeByScoreWithScoresOption)
            {
                return RangeByScoreWithScoresAsync(sortedSetRangeByScoreWithScoresOption).Result;
            }

            #endregion

            #region Range by score

            /// <summary>
            /// Return the value of the members in the specified interval in the ordered set key, the position is arranged by score
            /// </summary>
            /// <param name="sortedSetRangeByScoreOption">Sorted set range by score option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortedSetRangeByScoreResponse>> RangeByScoreAsync(SortedSetRangeByScoreOption sortedSetRangeByScoreOption)
            {
                return await ExecuteCommandAsync(sortedSetRangeByScoreOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Return the value of the members in the specified interval in the ordered set key, the position is arranged by score
            /// </summary>
            /// <param name="sortedSetRangeByScoreOption">Sorted set range by score option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortedSetRangeByScoreResponse> RangeByScore(SortedSetRangeByScoreOption sortedSetRangeByScoreOption)
            {
                return RangeByScoreAsync(sortedSetRangeByScoreOption).Result;
            }

            #endregion

            #region Range by rank with scores

            /// <summary>
            /// Returns the value and score of the members in the specified interval in the ordered set key, the positions are arranged according to score
            /// </summary>
            /// <param name="sortedSetRangeByRankWithScoresOption">Sorted set range by rank with scores option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortedSetRangeByRankWithScoresResponse>> RangeByRankWithScoresAsync(SortedSetRangeByRankWithScoresOption sortedSetRangeByRankWithScoresOption)
            {
                return await ExecuteCommandAsync(sortedSetRangeByRankWithScoresOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns the value and score of the members in the specified interval in the ordered set key, the positions are arranged according to score
            /// </summary>
            /// <param name="sortedSetRangeByRankWithScoresOption">Sorted set range by rank with scores option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortedSetRangeByRankWithScoresResponse> RangeByRankWithScores(SortedSetRangeByRankWithScoresOption sortedSetRangeByRankWithScoresOption)
            {
                return RangeByRankWithScoresAsync(sortedSetRangeByRankWithScoresOption).Result;
            }

            #endregion

            #region Range by rank

            /// <summary>
            /// Return the value of the members in the specified interval in the ordered set key, the positions are arranged by score
            /// </summary>
            /// <param name="sortedSetRangeByRankOption">Sorted set range by rank option</param>
            /// <returns>sorted set range by rank response</returns>
            public static async Task<CacheResult<SortedSetRangeByRankResponse>> RangeByRankAsync(SortedSetRangeByRankOption sortedSetRangeByRankOption)
            {
                return await ExecuteCommandAsync(sortedSetRangeByRankOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Return the value of the members in the specified interval in the ordered set key, the positions are arranged by score
            /// </summary>
            /// <param name="sortedSetRangeByRankOption">Sorted set range by rank option</param>
            /// <returns>sorted set range by rank response</returns>
            public static CacheResult<SortedSetRangeByRankResponse> RangeByRank(SortedSetRangeByRankOption sortedSetRangeByRankOption)
            {
                return RangeByRankAsync(sortedSetRangeByRankOption).Result;
            }

            #endregion

            #region Length by value

            /// <summary>
            /// Returns the number of members whose value is between min and max in the ordered set key
            /// </summary>
            /// <param name="sortedSetLengthByValueOption">Sorted set length by value option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortedSetLengthByValueResponse>> LengthByValueAsync(SortedSetLengthByValueOption sortedSetLengthByValueOption)
            {
                return await ExecuteCommandAsync(sortedSetLengthByValueOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns the number of members whose value is between min and max in the ordered set key
            /// </summary>
            /// <param name="sortedSetLengthByValueOption">Sorted set length by value option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortedSetLengthByValueResponse> LengthByValue(SortedSetLengthByValueOption sortedSetLengthByValueOption)
            {
                return LengthByValueAsync(sortedSetLengthByValueOption).Result;
            }

            #endregion

            #region Length

            /// <summary>
            /// Returns the number of members in the ordered set key whose score value is between min and max (including the score value equal to min or max by default)
            /// </summary>
            /// <param name="sortedSetLengthOption">Sorted set length option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortedSetLengthResponse>> LengthAsync(SortedSetLengthOption sortedSetLengthOption)
            {
                return await ExecuteCommandAsync(sortedSetLengthOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns the number of members in the ordered set key whose score value is between min and max (including the score value equal to min or max by default)
            /// </summary>
            /// <param name="sortedSetLengthOption">Sorted set length option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortedSetLengthResponse> Length(SortedSetLengthOption sortedSetLengthOption)
            {
                return LengthAsync(sortedSetLengthOption).Result;
            }

            #endregion

            #region Increment

            /// <summary>
            /// Add the incremental increment to the score value of the member of the ordered set key
            /// </summary>
            /// <param name="sortedSetIncrementOption">Sorted set increment option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortedSetIncrementResponse>> IncrementAsync(SortedSetIncrementOption sortedSetIncrementOption)
            {
                return await ExecuteCommandAsync(sortedSetIncrementOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Add the incremental increment to the score value of the member of the ordered set key
            /// </summary>
            /// <param name="sortedSetIncrementOption">Sorted set increment option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortedSetIncrementResponse> Increment(SortedSetIncrementOption sortedSetIncrementOption)
            {
                return IncrementAsync(sortedSetIncrementOption).Result;
            }

            #endregion

            #region Decrement

            /// <summary>
            /// is the score value of the member of the ordered set key minus the increment increment
            /// </summary>
            /// <param name="sortedSetDecrementOption">Sorted set decrement option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortedSetDecrementResponse>> DecrementAsync(SortedSetDecrementOption sortedSetDecrementOption)
            {
                return await ExecuteCommandAsync(sortedSetDecrementOption).ConfigureAwait(false);
            }

            /// <summary>
            /// is the score value of the member of the ordered set key minus the increment increment
            /// </summary>
            /// <param name="sortedSetDecrementOption">Sorted set decrement option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortedSetDecrementResponse> Decrement(SortedSetDecrementOption sortedSetDecrementOption)
            {
                return DecrementAsync(sortedSetDecrementOption).Result;
            }

            #endregion

            #region Combine and store

            /// <summary>
            /// Aggregate multiple ordered collections and save to destination
            /// </summary>
            /// <param name="sortedSetCombineAndStoreOption">Sorted set combine and store option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortedSetCombineAndStoreResponse>> CombineAndStoreAsync(SortedSetCombineAndStoreOption sortedSetCombineAndStoreOption)
            {
                return await ExecuteCommandAsync(sortedSetCombineAndStoreOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Aggregate multiple ordered collections and save to destination
            /// </summary>
            /// <param name="sortedSetCombineAndStoreOption">Sorted set combine and store option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortedSetCombineAndStoreResponse> CombineAndStore(SortedSetCombineAndStoreOption sortedSetCombineAndStoreOption)
            {
                return CombineAndStoreAsync(sortedSetCombineAndStoreOption).Result;
            }

            #endregion

            #region Add

            /// <summary>
            /// Add one or more member elements and their score values ​​to the ordered set key
            /// </summary>
            /// <param name="sortedSetAddOption">Sorted set add option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortedSetAddResponse>> AddAsync(SortedSetAddOption sortedSetAddOption)
            {
                return await ExecuteCommandAsync(sortedSetAddOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Add one or more member elements and their score values ​​to the ordered set key
            /// </summary>
            /// <param name="sortedSetAddOption">Sorted set add option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortedSetAddResponse> Add(SortedSetAddOption sortedSetAddOption)
            {
                return AddAsync(sortedSetAddOption).Result;
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
            /// <param name="sortOption">Sort option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortResponse>> SortAsync(SortOption sortOption)
            {
                return await ExecuteCommandAsync(sortOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Return or save the sorted elements in the given list, collection, ordered set key
            /// </summary>
            /// <param name="sortOption">Sort option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortResponse> Sort(SortOption sortOption)
            {
                return SortAsync(sortOption).Result;
            }

            #endregion

            #region Sort and store

            /// <summary>
            /// Return or save the sorted elements in the given list, collection, ordered set key, and save to the specified key value
            /// </summary>
            /// <param name="sortAndStoreOption">Sort and store option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SortAndStoreResponse>> SortAndStoreAsync(SortAndStoreOption sortAndStoreOption)
            {
                return await ExecuteCommandAsync(sortAndStoreOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Return or save the sorted elements in the given list, collection, ordered set key, and save to the specified key value
            /// </summary>
            /// <param name="sortAndStoreOption">Sort and store option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SortAndStoreResponse> SortAndStore(SortAndStoreOption sortAndStoreOption)
            {
                return SortAndStoreAsync(sortAndStoreOption).Result;
            }

            #endregion

            #region Type

            /// <summary>
            /// Returns the type of value stored by key
            /// </summary>
            /// <param name="typeOption">type option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<TypeResponse>> TypeAsync(TypeOption typeOption)
            {
                return await ExecuteCommandAsync(typeOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns the type of value stored by key
            /// </summary>
            /// <param name="typeOption">type option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<TypeResponse> Type(TypeOption typeOption)
            {
                return TypeAsync(typeOption).Result;
            }

            #endregion

            #region Time to live

            /// <summary>
            /// Return the remaining time to live for a given key (TTL, time to live)
            /// </summary>
            /// <param name="timeToLiveOption">Time to live option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<TimeToLiveResponse>> TimeToLiveAsync(TimeToLiveOption timeToLiveOption)
            {
                return await ExecuteCommandAsync(timeToLiveOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Return the remaining time to live for a given key (TTL, time to live)
            /// </summary>
            /// <param name="timeToLiveOption">Time to live option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<TimeToLiveResponse> TimeToLive(TimeToLiveOption timeToLiveOption)
            {
                return TimeToLiveAsync(timeToLiveOption).Result;
            }

            #endregion

            #region Restore

            /// <summary>
            /// Deserialize the given serialized value and associate it with the given key
            /// </summary>
            /// <param name="restoreOption">Restore option</param>
            /// <returns> Request results </returns>
            public static async Task<CacheResult<RestoreResponse>> RestoreAsync(RestoreOption restoreOption)
            {
                return await ExecuteCommandAsync(restoreOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Deserialize the given serialized value and associate it with the given key
            /// </summary>
            /// <param name="restoreOption">Restore option</param>
            /// <returns> Request results </returns>
            public static CacheResult<RestoreResponse> Restore(RestoreOption restoreOption)
            {
                return RestoreAsync(restoreOption).Result;
            }

            #endregion

            #region Rename

            /// <summary>
            /// Rename the key to newkey
            /// When the key and newkey are the same, or the key does not exist, an error is returned
            /// When newkey already exists, it will overwrite the old value
            /// </summary>
            /// <param name="renameOption">Rename option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<RenameResponse>> RenameAsync(RenameOption renameOption)
            {
                return await ExecuteCommandAsync(renameOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Rename the key to newkey
            /// When the key and newkey are the same, or the key does not exist, an error is returned
            /// When newkey already exists, it will overwrite the old value
            /// </summary>
            /// <param name="renameOption">Rename option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<RenameResponse> Rename(RenameOption renameOption)
            {
                return RenameAsync(renameOption).Result;
            }

            #endregion

            #region Random

            /// <summary>
            /// randomly return (do not delete) a key
            /// </summary>
            /// <param name="randomOption">Random option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<RandomResponse>> KeyRandomAsync(RandomOption randomOption)
            {
                return await ExecuteCommandAsync(randomOption).ConfigureAwait(false);
            }

            /// <summary>
            /// randomly return (do not delete) a key
            /// </summary>
            /// <param name="randomOption">Random option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<RandomResponse> KeyRandom(RandomOption randomOption)
            {
                return KeyRandomAsync(randomOption).Result;
            }

            #endregion

            #region Persist

            /// <summary>
            /// Remove the survival time of a given key, and convert this key from "volatile" (with survival time key) to "persistent" (a key with no survival time and never expires)
            /// </summary>
            /// <param name="persistOption">Persist option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<PersistResponse>> PersistAsync(PersistOption persistOption)
            {
                return await ExecuteCommandAsync(persistOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Remove the survival time of a given key, and convert this key from "volatile" (with survival time key) to "persistent" (a key with no survival time and never expires)
            /// </summary>
            /// <param name="persistOption">Persist option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<PersistResponse> Persist(PersistOption persistOption)
            {
                return PersistAsync(persistOption).Result;
            }

            #endregion

            #region Move

            /// <summary>
            /// Move the key of the current database to the given database db
            /// </summary>
            /// <param name="moveOption">Move option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<MoveResponse>> MoveAsync(MoveOption moveOption)
            {
                return await ExecuteCommandAsync(moveOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Move the key of the current database to the given database db
            /// </summary>
            /// <param name="moveOption">Move option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<MoveResponse> Move(MoveOption moveOption)
            {
                return MoveAsync(moveOption).Result;
            }

            #endregion

            #region Migrate

            /// <summary>
            /// Transfer the key atomically from the current instance to the specified database of the target instance. Once the transfer is successful, the key is guaranteed to appear on the target instance, and the key on the current instance will be deleted.
            /// </summary>
            /// <param name="migrateOption">Migrate option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<MigrateResponse>> MigrateAsync(MigrateOption migrateOption)
            {
                return await ExecuteCommandAsync(migrateOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Transfer the key atomically from the current instance to the specified database of the target instance. Once the transfer is successful, the key is guaranteed to appear on the target instance, and the key on the current instance will be deleted.
            /// </summary>
            /// <param name="migrateOption">Migrate option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<MigrateResponse> Migrate(MigrateOption migrateOption)
            {
                return MigrateAsync(migrateOption).Result;
            }

            #endregion

            #region Expire

            /// <summary>
            /// Set the survival time for the given key. When the key expires (the survival time is 0), it will be automatically deleted
            /// </summary>
            /// <param name="expireOption">Expire option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<ExpireResponse>> ExpireAsync(ExpireOption expireOption)
            {
                return await ExecuteCommandAsync(expireOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Set the survival time for the given key. When the key expires (the survival time is 0), it will be automatically deleted
            /// </summary>
            /// <param name="expireOption">Expire option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<ExpireResponse> Expire(ExpireOption expireOption)
            {
                return ExpireAsync(expireOption).Result;
            }

            #endregion;

            #region Dump

            /// <summary>
            /// Serialize the given key and return the serialized value. Use the RESTORE command to deserialize this value
            /// </summary>
            /// <param name="dumpOption">Dump option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<DumpResponse>> DumpAsync(DumpOption dumpOption)
            {
                return await ExecuteCommandAsync(dumpOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Serialize the given key and return the serialized value. Use the RESTORE command to deserialize this value
            /// </summary>
            /// <param name="dumpOption">Dump option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<DumpResponse> Dump(DumpOption dumpOption)
            {
                return DumpAsync(dumpOption).Result;
            }

            #endregion

            #region Delete

            /// <summary>
            /// Delete the specified key
            /// </summary>
            /// <param name="deleteOption">Delete option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<DeleteResponse>> DeleteAsync(DeleteOption deleteOption)
            {
                return await ExecuteCommandAsync(deleteOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Delete the specified key
            /// </summary>
            /// <param name="deleteOption">Delete option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<DeleteResponse> Delete(DeleteOption deleteOption)
            {
                return DeleteAsync(deleteOption).Result;
            }

            #endregion

            #region Get keys

            /// <summary>
            /// Find all keys that match a given pattern
            /// </summary>
            /// <param name="getKeysOption">Get keys option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<GetKeysResponse>> GetKeysAsync(GetKeysOption getKeysOption)
            {
                return await ExecuteCommandAsync(getKeysOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Find all keys that match a given pattern
            /// </summary>
            /// <param name="getKeysOption">Get keys option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<GetKeysResponse> GetKeys(GetKeysOption getKeysOption)
            {
                return GetKeysAsync(getKeysOption).Result;
            }

            #endregion

            #region Exist

            /// <summary>
            /// Check whether key exist
            /// </summary>
            /// <param name="existOption">Exist option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<ExistResponse>> ExistAsync(ExistOption existOption)
            {
                return await ExecuteCommandAsync(existOption).ConfigureAwait(false);
            }

            /// <summary>
            /// Check whether key exist
            /// </summary>
            /// <param name="existOption">Exist option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<ExistResponse> Exist(ExistOption existOption)
            {
                return ExistAsync(existOption).Result;
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
            /// <param name="getAllDataBaseOption">Get all database option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<GetAllDataBaseResponse>> GetAllDataBaseAsync(CacheServer server, GetAllDataBaseOption getAllDataBaseOption)
            {
                return await getAllDataBaseOption.ExecuteAsync(server).ConfigureAwait(false);
            }

            /// <summary>
            /// Returns all cached databases in the server
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="getAllDataBaseOption">Get all database option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<GetAllDataBaseResponse> GetAllDataBase(CacheServer server, GetAllDataBaseOption getAllDataBaseOption)
            {
                return GetAllDataBaseAsync(server, getAllDataBaseOption).Result;
            }

            #endregion

            #region Query keys

            /// <summary>
            /// Query key value
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="getKeysOption"> Get keys option </param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<GetKeysResponse>> GetKeysAsync(CacheServer server, GetKeysOption getKeysOption)
            {
                return await getKeysOption.ExecuteAsync(server).ConfigureAwait(false);
            }

            /// <summary>
            /// Query key value
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="getKeysOption"> Get keys option </param>
            /// <returns>Return cache result</returns>
            public static CacheResult<GetKeysResponse> GetKeys(CacheServer server, GetKeysOption getKeysOption)
            {
                return GetKeysAsync(server, getKeysOption).Result;
            }

            #endregion

            #region Clear data

            /// <summary>
            /// Clear all data in the cache database
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="clearDataOption"> Clear data option </param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<ClearDataResponse>> ClearDataAsync(CacheServer server, ClearDataOption clearDataOption)
            {
                return await clearDataOption.ExecuteAsync(server).ConfigureAwait(false);
            }

            /// <summary>
            /// Clear all data in the cache database
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="clearDataOption"> Clear data option </param>
            /// <returns>Return cache result</returns>
            public static CacheResult<ClearDataResponse> ClearData(CacheServer server, ClearDataOption clearDataOption)
            {
                return ClearDataAsync(server, clearDataOption).Result;
            }

            #endregion

            #region Get cache item detail

            /// <summary>
            /// Get data item details
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="getDetailOption"> Get detail option </param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<GetDetailResponse>> GetKeyDetailAsync(CacheServer server, GetDetailOption getDetailOption)
            {
                return await getDetailOption.ExecuteAsync(server).ConfigureAwait(false);
            }

            /// <summary>
            /// Get data item details
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="getDetailOption"> Get detail option </param>
            /// <returns>Return cache result</returns>
            public static CacheResult<GetDetailResponse> GetKeyDetail(CacheServer server, GetDetailOption getDetailOption)
            {
                return GetKeyDetailAsync(server, getDetailOption).Result;
            }

            #endregion

            #region Get server configuration

            /// <summary>
            /// Get server configuration
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="getServerConfigurationOption">Get server configuration option</param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<GetServerConfigurationResponse>> GetServerConfigurationAsync(CacheServer server, GetServerConfigurationOption getServerConfigurationOption)
            {
                return await getServerConfigurationOption.ExecuteAsync(server).ConfigureAwait(false);
            }

            /// <summary>
            /// Get server configuration
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="getServerConfigurationOption">Get server configuration option</param>
            /// <returns>Return cache result</returns>
            public static CacheResult<GetServerConfigurationResponse> GetServerConfiguration(CacheServer server, GetServerConfigurationOption getServerConfigurationOption)
            {
                return GetServerConfigurationAsync(server, getServerConfigurationOption).Result;
            }

            #endregion

            #region Save server configuration

            /// <summary>
            /// Save the server configuration
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="saveServerConfigurationOption"> Save server configuration option </param>
            /// <returns>Return cache result</returns>
            public static async Task<CacheResult<SaveServerConfigurationResponse>> SaveServerConfigurationAsync(CacheServer server, SaveServerConfigurationOption saveServerConfigurationOption)
            {
                return await saveServerConfigurationOption.ExecuteAsync(server).ConfigureAwait(false);
            }

            /// <summary>
            /// Save the server configuration
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="saveServerConfigurationOption"> Save server configuration option </param>
            /// <returns>Return cache result</returns>
            public static CacheResult<SaveServerConfigurationResponse> SaveServerConfiguration(CacheServer server, SaveServerConfigurationOption saveServerConfigOption)
            {
                return SaveServerConfigurationAsync(server, saveServerConfigOption).Result;
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
            static Dictionary<CacheServerType, ICacheProvider> Providers = new Dictionary<CacheServerType, ICacheProvider>();

            /// <summary>
            /// Get cache servers operation proxy
            /// </summary>
            static Func<ICacheRequestOption, List<CacheServer>> GetCacheServersProxy;

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
            public static void ConfigureCacheServer(Func<ICacheRequestOption, List<CacheServer>> getCacheServerOperation)
            {
                GetCacheServersProxy = getCacheServerOperation;
            }

            /// <summary>
            /// Get cache servers
            /// </summary>
            /// <param name="cacheRequestOption">Cache request option</param>
            /// <returns>Return cache servers</returns>
            public static List<CacheServer> GetCacheServers<T>(CacheRequestOption<T> cacheRequestOption) where T : CacheResponse
            {
                return GetCacheServersProxy?.Invoke(cacheRequestOption) ?? new List<CacheServer>(0);
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
        /// <param name="requestOption">Request option</param>
        /// <returns>Reurn cache result</returns>
        static async Task<CacheResult<TResponse>> ExecuteCommandAsync<TResponse>(CacheRequestOption<TResponse> requestOption) where TResponse : CacheResponse
        {
            if (requestOption == null)
            {
                return CacheResult<TResponse>.EmptyResult();
            }
            return await requestOption.ExecuteAsync().ConfigureAwait(false) ?? CacheResult<TResponse>.EmptyResult();
        }

        #endregion
    }
}