using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sixnet.Cache.Constant;
using Sixnet.Cache.Hash;
using Sixnet.Cache.Keys;
using Sixnet.Cache.List;
using Sixnet.Cache.Provider.Memory;
using Sixnet.Cache.Server;
using Sixnet.Cache.Set;
using Sixnet.Cache.SortedSet;
using Sixnet.Cache.String;
using System.Threading.Tasks;
using Sixnet.Serialization;
using Sixnet.Cache;
using Sixnet.Cache.String.Response;
using Sixnet.Cache.List.Response;
using Sixnet.Cache.List.Options;
using Sixnet.Cache.Hash.Response;
using Sixnet.Cache.Hash.Options;
using Sixnet.Cache.Set.Response;
using Sixnet.Cache.Set.Options;
using Sixnet.Cache.SortedSet.Response;
using Sixnet.Cache.SortedSet.Options;
using Sixnet.Cache.Keys.Response;
using Sixnet.Cache.Keys.Options;
using Sixnet.Cache.Server.Response;
using Sixnet.Cache.Server.Options;

namespace Sixnet.Cache
{
    /// <summary>
    /// Cache manager
    /// </summary>
    public static partial class CacheManager
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
        public static StringSetResponse SetData<T>(CacheKey key, T data, DateTimeOffset? absoluteExpiration = null, CacheSetWhen when = CacheSetWhen.Always, CacheObject cacheObject = null)
        {
            var value = JsonSerializer.Serialize(data);
            if (string.IsNullOrWhiteSpace(value))
            {
                return CacheResponse.FailResponse<StringSetResponse>(CacheCodes.ValuesIsNullOrEmpty);
            }
            return String.Set(key, value, absoluteExpiration, when, cacheObject);
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
        public static StringSetResponse SetDataByRelativeExpiration<T>(CacheKey key, T data, TimeSpan? absoluteExpirationRelativeToNow = null, bool slidingExpiration = true, CacheSetWhen when = CacheSetWhen.Always, CacheObject cacheObject = null)
        {
            var value = JsonSerializer.Serialize(data);
            if (string.IsNullOrWhiteSpace(value))
            {
                return CacheResponse.FailResponse<StringSetResponse>(CacheCodes.ValuesIsNullOrEmpty);
            }
            return String.SetByRelativeExpiration(key, value, absoluteExpirationRelativeToNow, slidingExpiration, when, cacheObject);
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
        public static T GetData<T>(CacheKey key, CacheObject cacheObject = null)
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
        public static List<T> GetDataList<T>(IEnumerable<CacheKey> cacheKeys, CacheObject cacheObject = null)
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
            /// <param name="stringSetRangeOptions">Set range options</param>
            /// <returns>Return cache set result</returns>
            public static StringSetRangeResponse SetRange(StringSetRangeOptions stringSetRangeOptions)
            {
                return ExecuteCommand(stringSetRangeOptions);
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
            public static StringSetBitResponse SetBit(StringSetBitOptions stringSetBitOptions)
            {
                return ExecuteCommand(stringSetBitOptions);
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
            public static StringSetResponse Set(StringSetOptions stringSetOptions)
            {
                return ExecuteCommand(stringSetOptions);
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
            public static StringSetResponse Set(CacheKey key, string value, DateTimeOffset? absoluteExpiration = null, CacheSetWhen when = CacheSetWhen.Always, CacheObject cacheObject = null)
            {
                return Set(new StringSetOptions()
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
            public static StringSetResponse SetByRelativeExpiration(CacheKey key, string value, TimeSpan? absoluteExpirationRelativeToNow = null, bool slidingExpiration = true, CacheSetWhen when = CacheSetWhen.Always, CacheObject cacheObject = null)
            {
                return Set(new StringSetOptions()
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
            /// <param name="stringLengthOptions">String length options</param>
            /// <returns>Return cache result</returns>
            public static StringLengthResponse Length(StringLengthOptions stringLengthOptions)
            {
                return ExecuteCommand(stringLengthOptions);
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
            public static StringIncrementResponse Increment(StringIncrementOptions stringIncrementOptions)
            {
                return ExecuteCommand(stringIncrementOptions);
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
            public static StringGetWithExpiryResponse GetWithExpiry(StringGetWithExpiryOptions stringGetWithExpiryOptions)
            {
                return ExecuteCommand(stringGetWithExpiryOptions);
            }

            #endregion

            #region Get set

            /// <summary>
            /// Set the value of the key key to value and return the old value of the key key before it is set
            /// When the key key exists but is not a string type, the command returns an error
            /// </summary>
            /// <param name="stringGetSetOptions">String get set options</param>
            /// <returns>Return cache result</returns>
            public static StringGetSetResponse GetSet(StringGetSetOptions stringGetSetOptions)
            {
                return ExecuteCommand(stringGetSetOptions);
            }

            #endregion

            #region Get range

            /// <summary>
            /// Returns the specified part of the string value stored by the key key, the range of the string interception is determined by the two offsets of start and end (including start and end)
            /// Negative offset means counting from the end of the string, -1 means the last character, -2 means the penultimate character, and so on
            /// </summary>
            /// <param name="stringGetRangeOptions">String get range options</param>
            /// <returns>Return cache result</returns>
            public static StringGetRangeResponse GetRange(StringGetRangeOptions stringGetRangeOptions)
            {
                return ExecuteCommand(stringGetRangeOptions);
            }

            #endregion

            #region Get bit

            /// <summary>
            /// For the string value stored in key, get the bit at the specified offset
            /// When offset is greater than the length of the string value, or the key does not exist, return 0
            /// </summary>
            /// <param name="stringGetBitOptions">String get bit options</param>
            /// <returns>Return cache result</returns>
            public static StringGetBitResponse GetBit(StringGetBitOptions stringGetBitOptions)
            {
                return ExecuteCommand(stringGetBitOptions);
            }

            #endregion

            #region Get

            /// <summary>
            /// Returns the value of the given string key or keys
            /// </summary>
            /// <param name="stringGetOptions">String get options</param>
            /// <returns>Return cache result</returns>
            public static StringGetResponse Get(StringGetOptions stringGetOptions)
            {
                return ExecuteCommand(stringGetOptions);
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
                return JsonSerializer.Deserialize<T>(cacheValue);
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
                var result = Get(new StringGetOptions()
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
                    var data = JsonSerializer.Deserialize<T>(val);
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
            public static StringDecrementResponse Decrement(StringDecrementOptions stringDecrementOptions)
            {
                return ExecuteCommand(stringDecrementOptions);
            }

            #endregion

            #region Bit position

            /// <summary>
            /// Returns the position of the first binary bit in the bitmap
            /// By default, the command will detect the entire bitmap, but the user can also specify the range to be detected through the optional start parameter and end parameter
            /// </summary>
            /// <param name="stringBitPositionOptions">String bit position options</param>
            /// <returns>Return cache result</returns>
            public static StringBitPositionResponse BitPosition(StringBitPositionOptions stringBitPositionOptions)
            {
                return ExecuteCommand(stringBitPositionOptions);
            }

            #endregion

            #region Bit operation

            /// <summary>
            /// Perform bit operations on one or more string keys that hold binary bits, and save the result to destkey
            /// Except NOT operation, other operations can accept one or more keys as input
            /// </summary>
            /// <param name="stringBitOperationOptions">String bit operation options</param>
            /// <returns>Return cache result</returns>
            public static StringBitOperationResponse BitOperation(StringBitOperationOptions stringBitOperationOptions)
            {
                return ExecuteCommand(stringBitOperationOptions);
            }

            #endregion

            #region Bit count

            /// <summary>
            /// Calculate the number of bits set to 1 in a given string.
            /// Under normal circumstances, the given entire string will be counted, by specifying additional start or end parameters, you can make the count only on a specific bit
            /// </summary>
            /// <param name="stringBitCountOptions">String bit count options</param>
            /// <returns>Return cache result</returns>
            public static StringBitCountResponse BitCount(StringBitCountOptions stringBitCountOptions)
            {
                return ExecuteCommand(stringBitCountOptions);
            }

            #endregion

            #region Append

            /// <summary>
            /// If the key key already exists and its value is a string, the value will be appended to the end of the key key's existing value
            /// If the key does not exist, simply set the value of the key key to value
            /// </summary>
            /// <param name="stringAppendOptions">String append options</param>
            /// <returns>Return cache result</returns>
            public static StringAppendResponse Append(StringAppendOptions stringAppendOptions)
            {
                return ExecuteCommand(stringAppendOptions);
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
            public static ListTrimResponse Trim(ListTrimOptions listTrimOptions)
            {
                return ExecuteCommand(listTrimOptions);
            }

            #endregion

            #region Set by index

            /// <summary>
            /// Set the value of the element whose index of the list key is index to value
            /// When the index parameter is out of range, or an operation is performed on an empty list (the key does not exist), an error is returned
            /// </summary>
            /// <param name="listSetByIndexOptions">List set by index options</param>
            /// <returns>Return cache result</returns>
            public static ListSetByIndexResponse SetByIndex(ListSetByIndexOptions listSetByIndexOptions)
            {
                return ExecuteCommand(listSetByIndexOptions);
            }

            #endregion

            #region Right push

            /// <summary>
            /// Insert one or more values ​​into the end of the list key (far right).
            /// If there are multiple value values, then each value value is inserted into the end of the table in order from left to right
            /// </summary>
            /// <param name="listRightPushOptions">List right push options</param>
            /// <returns>Return cache result</returns>
            public static ListRightPushResponse RightPush(ListRightPushOptions listRightPushOptions)
            {
                return ExecuteCommand(listRightPushOptions);
            }

            #endregion

            #region Right pop left push

            /// <summary>
            /// Pop the last element (tail element) in the list source and return it to the client
            /// Insert the element popped by source into the destination list as the head element of the destination list
            /// </summary>
            /// <param name="listRightPopLeftPushOptions">List right pop left push options</param>
            /// <returns>Return cache result</returns>
            public static ListRightPopLeftPushResponse RightPopLeftPush(ListRightPopLeftPushOptions listRightPopLeftPushOptions)
            {
                return ExecuteCommand(listRightPopLeftPushOptions);
            }

            #endregion

            #region Right pop

            /// <summary>
            /// Remove and return the tail element of the list key.
            /// </summary>
            /// <param name="listRightPopOptions">List right pop options</param>
            /// <returns>Return cache result</returns>
            public static ListRightPopResponse RightPop(ListRightPopOptions listRightPopOptions)
            {
                return ExecuteCommand(listRightPopOptions);
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
            public static ListRemoveResponse Remove(ListRemoveOptions listRemoveOptions)
            {
                return ExecuteCommand(listRemoveOptions);
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
            public static ListRangeResponse Range(ListRangeOptions listRangeOptions)
            {
                return ExecuteCommand(listRangeOptions);
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
            public static ListLengthResponse Length(ListLengthOptions listLengthOptions)
            {
                return ExecuteCommand(listLengthOptions);
            }

            #endregion

            #region Left push

            /// <summary>
            /// Insert one or more values ​​into the header of the list key
            /// If the key does not exist, an empty list will be created and the operation will be performed
            /// </summary>
            /// <param name="listLeftPushOptions">List left push options</param>
            /// <returns>Return cache result</returns>
            public static ListLeftPushResponse LeftPush(ListLeftPushOptions listLeftPushOptions)
            {
                return ExecuteCommand(listLeftPushOptions);
            }

            #endregion

            #region Left pop

            /// <summary>
            /// Remove and return the head element of the list key
            /// </summary>
            /// <param name="listLeftPopOptions">List left pop options</param>
            /// <returns>Return cache result</returns>
            public static ListLeftPopResponse LeftPop(ListLeftPopOptions listLeftPopOptions)
            {
                return ExecuteCommand(listLeftPopOptions);
            }

            #endregion

            #region Insert before

            /// <summary>
            /// Insert the value value into the list key, before the value pivot
            /// </summary>
            /// <param name="listInsertBeforeOptions">List insert before options</param>
            /// <returns>Return cache result</returns>
            public static ListInsertBeforeResponse InsertBefore(ListInsertBeforeOptions listInsertBeforeOptions)
            {
                return ExecuteCommand(listInsertBeforeOptions);
            }

            #endregion

            #region Insert after

            /// <summary>
            /// Insert the value value into the list key, after the value pivot
            /// </summary>
            /// <param name="listInsertAfterOptions">List insert after options</param>
            /// <returns>Return cache result</returns>
            public static ListInsertAfterResponse InsertAfter(ListInsertAfterOptions listInsertAfterOptions)
            {
                return ExecuteCommand(listInsertAfterOptions);
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
            public static ListGetByIndexResponse GetByIndex(ListGetByIndexOptions listGetByIndexOptions)
            {
                return ExecuteCommand(listGetByIndexOptions);
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
            public static HashValuesResponse Values(HashValuesOptions hashValuesOptions)
            {
                return ExecuteCommand(hashValuesOptions);
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
            public static HashSetResponse Set(HashSetOptions hashSetOptions)
            {
                return ExecuteCommand(hashSetOptions);
            }

            #endregion

            #region Length

            /// <summary>
            /// returns the number of fields in the hash table key
            /// </summary>
            /// <param name="hashLengthOptions">Hash length options</param>
            /// <returns>Return cache result</returns>
            public static HashLengthResponse Length(HashLengthOptions hashLengthOptions)
            {
                return ExecuteCommand(hashLengthOptions);
            }

            #endregion

            #region Keys

            /// <summary>
            /// Return all keys in the hash table key
            /// </summary>
            /// <param name="hashKeysOptions">Hash keys options</param>
            /// <returns>Return cache result</returns>
            public static HashKeysResponse Keys(HashKeysOptions hashKeysOptions)
            {
                return ExecuteCommand(hashKeysOptions);
            }

            #endregion

            #region Increment

            /// <summary>
            /// Add incremental increment to the value of the field field in the hash table key
            /// </summary>
            /// <param name="hashIncrementOptions">Hash increment options</param>
            /// <returns>Return cache result</returns>
            public static HashIncrementResponse Increment(HashIncrementOptions hashIncrementOptions)
            {
                return ExecuteCommand(hashIncrementOptions);
            }

            #endregion

            #region Get

            /// <summary>
            /// Returns the value of the given field in the hash table
            /// </summary>
            /// <param name="hashGetOptions">Hash get options</param>
            /// <returns>Return cache result</returns>
            public static HashGetResponse Get(HashGetOptions hashGetOptions)
            {
                return ExecuteCommand(hashGetOptions);
            }

            #endregion

            #region Get all

            /// <summary>
            /// Return the values ​​of all fields in the hash table key
            /// </summary>
            /// <param name="hashGetAllOptions">Hash get all options</param>
            /// <returns>Return cache result</returns>
            public static HashGetAllResponse GetAll(HashGetAllOptions hashGetAllOptions)
            {
                return ExecuteCommand(hashGetAllOptions);
            }

            #endregion

            #region Exist

            /// <summary>
            /// Check if the given field exists in the hash of the hash table
            /// </summary>
            /// <param name="hashExistsOptions">Hash exists options</param>
            /// <returns>Return cache result</returns>
            public static HashExistsResponse Exist(HashExistsOptions hashExistsOptions)
            {
                return ExecuteCommand(hashExistsOptions);
            }

            #endregion

            #region Delete

            /// <summary>
            /// Delete one or more specified fields in the hash table key, the non-existing fields will be ignored
            /// </summary>
            /// <param name="hashDeleteOptions">Hash delete options</param>
            /// <returns>Return cache result</returns>
            public static HashDeleteResponse Delete(HashDeleteOptions hashDeleteOptions)
            {
                return ExecuteCommand(hashDeleteOptions);
            }

            #endregion

            #region Decrement

            /// <summary>
            /// Is the value of the field in the hash table key minus the increment increment
            /// </summary>
            /// <param name="hashDecrementOptions">Hash decrement options</param>
            /// <returns>Return cache result</returns>
            public static HashDecrementResponse Decrement(HashDecrementOptions hashDecrementOptions)
            {
                return ExecuteCommand(hashDecrementOptions);
            }

            #endregion

            #region Scan

            /// <summary>
            /// Each element returned is a key-value pair, a key-value pair consists of a key and a value
            /// </summary>
            /// <param name="hashScanOptions">Hash scan options</param>
            /// <returns>Return cache result</returns>
            public static HashScanResponse Scan(HashScanOptions hashScanOptions)
            {
                return ExecuteCommand(hashScanOptions);
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
            public static SetRemoveResponse Remove(SetRemoveOptions setRemoveOptions)
            {
                return ExecuteCommand(setRemoveOptions);
            }

            #endregion

            #region Random members

            /// <summary>
            /// Then return a set of random elements in the collection
            /// </summary>
            /// <param name="setRandomMembersOptions">Set random members options</param>
            /// <returns>Return cache result</returns>
            public static SetRandomMembersResponse RandomMembers(SetRandomMembersOptions setRandomMembersOptions)
            {
                return ExecuteCommand(setRandomMembersOptions);
            }

            #endregion

            #region Random member

            /// <summary>
            /// Returns a random element in the collection
            /// </summary>
            /// <param name="setRandomMemberOptions">Set random member options</param>
            /// <returns>Return cache result</returns>
            public static SetRandomMemberResponse RandomMember(SetRandomMemberOptions setRandomMemberOptions)
            {
                return ExecuteCommand(setRandomMemberOptions);
            }

            #endregion

            #region Pop

            /// <summary>
            /// Remove and return a random element in the collection
            /// </summary>
            /// <param name="setPopOptions">Set pop options</param>
            /// <returns>Return cache result</returns>
            public static SetPopResponse Pop(SetPopOptions setPopOptions)
            {
                return ExecuteCommand(setPopOptions);
            }

            #endregion

            #region Move

            /// <summary>
            /// Move the member element from the source collection to the destination collection
            /// </summary>
            /// <param name="setMoveOptions">Set move options</param>
            /// <returns>Return cache result</returns>
            public static SetMoveResponse Move(SetMoveOptions setMoveOptions)
            {
                return ExecuteCommand(setMoveOptions);
            }

            #endregion

            #region Members

            /// <summary>
            /// Return all members in the collection key
            /// </summary>
            /// <param name="setMembersOptions">Set members options</param>
            /// <returns>Return cache result</returns>
            public static SetMembersResponse Members(SetMembersOptions setMembersOptions)
            {
                return ExecuteCommand(setMembersOptions);
            }

            #endregion

            #region Length

            /// <summary>
            /// Returns the number of elements in the collection
            /// </summary>
            /// <param name="setLengthOptions">Set length options</param>
            /// <returns>Return cache result</returns>
            public static SetLengthResponse Length(SetLengthOptions setLengthOptions)
            {
                return ExecuteCommand(setLengthOptions);
            }

            #endregion

            #region Contains

            /// <summary>
            /// Determine whether the member element is a member of the set key
            /// </summary>
            /// <param name="setContainsOptions">Set contaims options</param>
            /// <returns>Return cache result</returns>
            public static SetContainsResponse Contains(SetContainsOptions setContainsOptions)
            {
                return ExecuteCommand(setContainsOptions);
            }

            #endregion

            #region Combine

            /// <summary>
            /// According to the operation mode, return the processing results of multiple collections
            /// </summary>
            /// <param name="setCombineOptions">Set combine options</param>
            /// <returns>Return cache result</returns>
            public static SetCombineResponse Combine(SetCombineOptions setCombineOptions)
            {
                return ExecuteCommand(setCombineOptions);
            }

            #endregion

            #region Combine and store

            /// <summary>
            /// Return the processing results of multiple collections according to the operation mode, and store the results to the specified key value at the same time
            /// </summary>
            /// <param name="setCombineAndStoreOptions">Set combine and store options</param>
            /// <returns>Return cache result</returns>
            public static SetCombineAndStoreResponse CombineAndStore(SetCombineAndStoreOptions setCombineAndStoreOptions)
            {
                return ExecuteCommand(setCombineAndStoreOptions);
            }

            #endregion

            #region Add

            /// <summary>
            /// Add one or more member elements to the collection key, the member elements already in the collection will be ignored
            /// If the key does not exist, create a collection that contains only the member element as a member
            /// </summary>
            /// <param name="setAddOptions">Set add options</param>
            /// <returns>Return cache result</returns>
            public static SetAddResponse Add(SetAddOptions setAddOptions)
            {
                return ExecuteCommand(setAddOptions);
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
            public static SortedSetScoreResponse Score(SortedSetScoreOptions sortedSetScoreOptions)
            {
                return ExecuteCommand(sortedSetScoreOptions);
            }

            #endregion

            #region Remove range by value

            /// <summary>
            /// Remove the elements in the specified range after sorting the elements
            /// </summary>
            /// <param name="sortedSetRemoveRangeByValueOptions">Sorted set remove range by value options</param>
            /// <returns>Return cache result</returns>
            public static SortedSetRemoveRangeByValueResponse RemoveRangeByValue(SortedSetRemoveRangeByValueOptions sortedSetRemoveRangeByValueOptions)
            {
                return ExecuteCommand(sortedSetRemoveRangeByValueOptions);
            }

            #endregion

            #region Remove range by score

            /// <summary>
            /// Remove all members in the ordered set key whose score value is between min and max
            /// </summary>
            /// <param name="sortedSetRemoveRangeByScoreOptions">Sorted set remove range by score options</param>
            /// <returns>Return cache result</returns>
            public static SortedSetRemoveRangeByScoreResponse RemoveRangeByScore(SortedSetRemoveRangeByScoreOptions sortedSetRemoveRangeByScoreOptions)
            {
                return ExecuteCommand(sortedSetRemoveRangeByScoreOptions);
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
            public static SortedSetRemoveRangeByRankResponse RemoveRangeByRank(SortedSetRemoveRangeByRankOptions sortedSetRemoveRangeByRankOptions)
            {
                return ExecuteCommand(sortedSetRemoveRangeByRankOptions);
            }

            #endregion

            #region Remove

            /// <summary>
            /// Remove the specified element in the ordered collection
            /// </summary>
            /// <param name="sortedSetRemoveOptions">Sorted set remove options</param>
            /// <returns>Return cache result</returns>
            public static SortedSetRemoveResponse Remove(SortedSetRemoveOptions sortedSetRemoveOptions)
            {
                return ExecuteCommand(sortedSetRemoveOptions);
            }

            #endregion

            #region Rank

            /// <summary>
            /// Returns the ranking of the member member in the ordered set key. The members of the ordered set are arranged in order of increasing score value (from small to large) by default
            /// The ranking is based on 0, that is, the member with the smallest score is ranked 0
            /// </summary>
            /// <param name="sortedSetRankOptions">Sorted set rank options</param>
            /// <returns>Return cache result</returns>
            public static SortedSetRankResponse Rank(SortedSetRankOptions sortedSetRankOptions)
            {
                return ExecuteCommand(sortedSetRankOptions);
            }

            #endregion

            #region Range by value

            /// <summary>
            /// Returns the elements in the ordered set between min and max
            /// </summary>
            /// <param name="sortedSetRangeByValueOptions">Sorted set range by value options</param>
            /// <returns>Return cache result</returns>
            public static SortedSetRangeByValueResponse RangeByValue(SortedSetRangeByValueOptions sortedSetRangeByValueOptions)
            {
                return ExecuteCommand(sortedSetRangeByValueOptions);
            }

            #endregion

            #region Range by score with scores

            /// <summary>
            /// Returns the value and score of the members in the specified interval in the ordered set key, the positions are arranged according to score
            /// </summary>
            /// <param name="sortedSetRangeByScoreWithScoresOptions">Sorted set range by score with scores options</param>
            /// <returns>Return cache result</returns>
            public static SortedSetRangeByScoreWithScoresResponse RangeByScoreWithScores(SortedSetRangeByScoreWithScoresOptions sortedSetRangeByScoreWithScoresOptions)
            {
                return ExecuteCommand(sortedSetRangeByScoreWithScoresOptions);
            }

            #endregion

            #region Range by score

            /// <summary>
            /// Return the value of the members in the specified interval in the ordered set key, the position is arranged by score
            /// </summary>
            /// <param name="sortedSetRangeByScoreOptions">Sorted set range by score options</param>
            /// <returns>Return cache result</returns>
            public static SortedSetRangeByScoreResponse RangeByScore(SortedSetRangeByScoreOptions sortedSetRangeByScoreOptions)
            {
                return ExecuteCommand(sortedSetRangeByScoreOptions);
            }

            #endregion

            #region Range by rank with scores

            /// <summary>
            /// Returns the value and score of the members in the specified interval in the ordered set key, the positions are arranged according to score
            /// </summary>
            /// <param name="sortedSetRangeByRankWithScoresOptions">Sorted set range by rank with scores options</param>
            /// <returns>Return cache result</returns>
            public static SortedSetRangeByRankWithScoresResponse RangeByRankWithScores(SortedSetRangeByRankWithScoresOptions sortedSetRangeByRankWithScoresOptions)
            {
                return ExecuteCommand(sortedSetRangeByRankWithScoresOptions);
            }

            #endregion

            #region Range by rank

            /// <summary>
            /// Return the value of the members in the specified interval in the ordered set key, the positions are arranged by score
            /// </summary>
            /// <param name="sortedSetRangeByRankOptions">Sorted set range by rank options</param>
            /// <returns>sorted set range by rank response</returns>
            public static SortedSetRangeByRankResponse RangeByRank(SortedSetRangeByRankOptions sortedSetRangeByRankOptions)
            {
                return ExecuteCommand(sortedSetRangeByRankOptions);
            }

            #endregion

            #region Length by value

            /// <summary>
            /// Returns the number of members whose value is between min and max in the ordered set key
            /// </summary>
            /// <param name="sortedSetLengthByValueOptions">Sorted set length by value options</param>
            /// <returns>Return cache result</returns>
            public static SortedSetLengthByValueResponse LengthByValue(SortedSetLengthByValueOptions sortedSetLengthByValueOptions)
            {
                return ExecuteCommand(sortedSetLengthByValueOptions);
            }

            #endregion

            #region Length

            /// <summary>
            /// Returns the number of members in the ordered set key whose score value is between min and max (including the score value equal to min or max by default)
            /// </summary>
            /// <param name="sortedSetLengthOptions">Sorted set length options</param>
            /// <returns>Return cache result</returns>
            public static SortedSetLengthResponse Length(SortedSetLengthOptions sortedSetLengthOptions)
            {
                return ExecuteCommand(sortedSetLengthOptions);
            }

            #endregion

            #region Increment

            /// <summary>
            /// Add the incremental increment to the score value of the member of the ordered set key
            /// </summary>
            /// <param name="sortedSetIncrementOptions">Sorted set increment options</param>
            /// <returns>Return cache result</returns>
            public static SortedSetIncrementResponse Increment(SortedSetIncrementOptions sortedSetIncrementOptions)
            {
                return ExecuteCommand(sortedSetIncrementOptions);
            }

            #endregion

            #region Decrement

            /// <summary>
            /// is the score value of the member of the ordered set key minus the increment increment
            /// </summary>
            /// <param name="sortedSetDecrementOptions">Sorted set decrement options</param>
            /// <returns>Return cache result</returns>
            public static SortedSetDecrementResponse Decrement(SortedSetDecrementOptions sortedSetDecrementOptions)
            {
                return ExecuteCommand(sortedSetDecrementOptions);
            }

            #endregion

            #region Combine and store

            /// <summary>
            /// Aggregate multiple ordered collections and save to destination
            /// </summary>
            /// <param name="sortedSetCombineAndStoreOptions">Sorted set combine and store options</param>
            /// <returns>Return cache result</returns>
            public static SortedSetCombineAndStoreResponse CombineAndStore(SortedSetCombineAndStoreOptions sortedSetCombineAndStoreOptions)
            {
                return ExecuteCommand(sortedSetCombineAndStoreOptions);
            }

            #endregion

            #region Add

            /// <summary>
            /// Add one or more member elements and their score values ​​to the ordered set key
            /// </summary>
            /// <param name="sortedSetAddOptions">Sorted set add options</param>
            /// <returns>Return cache result</returns>
            public static SortedSetAddResponse Add(SortedSetAddOptions sortedSetAddOptions)
            {
                return ExecuteCommand(sortedSetAddOptions);
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
            public static SortResponse Sort(SortOptions sortOptions)
            {
                return ExecuteCommand(sortOptions);
            }

            #endregion

            #region Sort and store

            /// <summary>
            /// Return or save the sorted elements in the given list, collection, ordered set key, and save to the specified key value
            /// </summary>
            /// <param name="sortAndStoreOptions">Sort and store options</param>
            /// <returns>Return cache result</returns>
            public static SortAndStoreResponse SortAndStore(SortAndStoreOptions sortAndStoreOptions)
            {
                return ExecuteCommand(sortAndStoreOptions);
            }

            #endregion

            #region Type

            /// <summary>
            /// Returns the type of value stored by key
            /// </summary>
            /// <param name="typeOptions">type options</param>
            /// <returns>Return cache result</returns>
            public static TypeResponse Type(TypeOptions typeOptions)
            {
                return ExecuteCommand(typeOptions);
            }

            #endregion

            #region Time to live

            /// <summary>
            /// Return the remaining time to live for a given key (TTL, time to live)
            /// </summary>
            /// <param name="timeToLiveOptions">Time to live options</param>
            /// <returns>Return cache result</returns>
            public static TimeToLiveResponse TimeToLive(TimeToLiveOptions timeToLiveOptions)
            {
                return ExecuteCommand(timeToLiveOptions);
            }

            #endregion

            #region Restore

            /// <summary>
            /// Deserialize the given serialized value and associate it with the given key
            /// </summary>
            /// <param name="restoreOptions">Restore options</param>
            /// <returns> Return cache result </returns>
            public static RestoreResponse Restore(RestoreOptions restoreOptions)
            {
                return ExecuteCommand(restoreOptions);
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
            public static RenameResponse Rename(RenameOptions renameOptions)
            {
                return ExecuteCommand(renameOptions);
            }

            #endregion

            #region Random

            /// <summary>
            /// randomly return (do not delete) a key
            /// </summary>
            /// <param name="randomOptions">Random options</param>
            /// <returns>Return cache result</returns>
            public static RandomResponse KeyRandom(RandomOptions randomOptions)
            {
                return ExecuteCommand(randomOptions);
            }

            #endregion

            #region Persist

            /// <summary>
            /// Remove the survival time of a given key, and convert this key from "volatile" (with survival time key) to "persistent" (a key with no survival time and never expires)
            /// </summary>
            /// <param name="persistOptions">Persist options</param>
            /// <returns>Return cache result</returns>
            public static PersistResponse Persist(PersistOptions persistOptions)
            {
                return ExecuteCommand(persistOptions);
            }

            #endregion

            #region Move

            /// <summary>
            /// Move the key of the current database to the given database
            /// </summary>
            /// <param name="moveOptions">Move options</param>
            /// <returns>Return cache result</returns>
            public static MoveResponse Move(MoveOptions moveOptions)
            {
                return ExecuteCommand(moveOptions);
            }

            #endregion

            #region Migrate

            /// <summary>
            /// Transfer the key atomically from the current instance to the specified database of the target instance. Once the transfer is successful, the key is guaranteed to appear on the target instance, and the key on the current instance will be deleted.
            /// </summary>
            /// <param name="migrateOptions">Migrate options</param>
            /// <returns>Return cache result</returns>
            public static MigrateKeyResponse Migrate(MigrateKeyOptions migrateOptions)
            {
                return ExecuteCommand(migrateOptions);
            }

            #endregion

            #region Expire

            /// <summary>
            /// Set the survival time for the given key. When the key expires (the survival time is 0), it will be automatically deleted
            /// </summary>
            /// <param name="expireOptions">Expire options</param>
            /// <returns>Return cache result</returns>
            public static ExpireResponse Expire(ExpireOptions expireOptions)
            {
                return ExecuteCommand(expireOptions);
            }

            #endregion;

            #region Dump

            /// <summary>
            /// Serialize the given key and return the serialized value. Use the RESTORE command to deserialize this value
            /// </summary>
            /// <param name="dumpOptions">Dump options</param>
            /// <returns>Return cache result</returns>
            public static DumpResponse Dump(DumpOptions dumpOptions)
            {
                return ExecuteCommand(dumpOptions);
            }

            #endregion

            #region Delete

            /// <summary>
            /// Delete the specified key
            /// </summary>
            /// <param name="deleteOptions">Delete options</param>
            /// <returns>Return cache result</returns>
            public static DeleteResponse Delete(DeleteOptions deleteOptions)
            {
                return ExecuteCommand(deleteOptions);
            }

            #endregion

            #region Get keys

            /// <summary>
            /// Find all keys that match a given pattern
            /// </summary>
            /// <param name="getKeysOptions">Get keys options</param>
            /// <returns>Return cache result</returns>
            public static GetKeysResponse GetKeys(GetKeysOptions getKeysOptions)
            {
                return ExecuteCommand(getKeysOptions);
            }

            #endregion

            #region Exist

            /// <summary>
            /// Check whether key exist
            /// </summary>
            /// <param name="existOptions">Exist options</param>
            /// <returns>Return cache result</returns>
            public static ExistResponse Exist(ExistOptions existOptions)
            {
                return ExecuteCommand(existOptions);
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
            public static GetAllDataBaseResponse GetAllDataBase(CacheServer server, GetAllDataBaseOptions getAllDataBaseOptions)
            {
                return getAllDataBaseOptions.Execute(server);
            }

            #endregion

            #region Query keys

            /// <summary>
            /// Query key value
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="getKeysOptions"> Get keys options </param>
            /// <returns>Return cache result</returns>
            public static GetKeysResponse GetKeys(CacheServer server, GetKeysOptions getKeysOptions)
            {
                return getKeysOptions.Execute(server);
            }

            #endregion

            #region Clear data

            /// <summary>
            /// Clear all data in the cache database
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="clearDataOptions"> Clear data options </param>
            /// <returns>Return cache result</returns>
            public static ClearDataResponse ClearData(CacheServer server, ClearDataOptions clearDataOptions)
            {
                return clearDataOptions.Execute(server);
            }

            #endregion

            #region Get cache item detail

            /// <summary>
            /// Get data item details
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="getDetailOptions"> Get detail options </param>
            /// <returns>Return cache result</returns>
            public static GetDetailResponse GetKeyDetail(CacheServer server, GetDetailOptions getDetailOptions)
            {
                return getDetailOptions.Execute(server);
            }

            #endregion

            #region Get server configuration

            /// <summary>
            /// Get server configuration
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="getServerConfigurationOptions">Get server configuration options</param>
            /// <returns>Return cache result</returns>
            public static GetServerConfigurationResponse GetServerConfiguration(CacheServer server, GetServerConfigurationOptions getServerConfigurationOptions)
            {
                return getServerConfigurationOptions.Execute(server);
            }

            #endregion

            #region Save server configuration

            /// <summary>
            /// Save the server configuration
            /// </summary>
            /// <param name="server"> server information </param>
            /// <param name="saveServerConfigurationOptions"> Save server configuration options </param>
            /// <returns>Return cache result</returns>
            public static SaveServerConfigurationResponse SaveServerConfiguration(CacheServer server, SaveServerConfigurationOptions saveServerConfigurationOptions)
            {
                return saveServerConfigurationOptions.Execute(server);
            }

            #endregion
        }

        #endregion

        #region Configuration

        /// <summary>
        /// Cache configuration
        /// </summary>
        public static partial class Configuration
        {
            #region Fields

            /// <summary>
            /// Cache providers
            /// </summary>
            static readonly Dictionary<CacheServerType, ICacheProvider> Providers = new Dictionary<CacheServerType, ICacheProvider>();

            /// <summary>
            /// Get cache servers operation proxy
            /// </summary>
            static Func<ICacheOptions, CacheServer> GetCacheServersProxy;

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

            /// <summary>
            /// Default InMemory server
            /// </summary>
            public readonly static CacheServer DefaultInMemoryServer = new("SIXNET_DEFAULT_IN_MEMORY_SERVER_NAME", CacheServerType.InMemory);

            /// <summary>
            /// Split table cache object name
            /// </summary>
            public const string SplitTableCacheObjectName = "SIXNET_SPLIT_TABLE_CACHE_OBJECT_NAME";

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
            public static void ConfigureCacheServer(Func<ICacheOptions, CacheServer> getCacheServerOperation)
            {
                GetCacheServersProxy = getCacheServerOperation;
            }

            /// <summary>
            /// Get cache server
            /// </summary>
            /// <param name="cacheOptions">Cache options</param>
            /// <returns>Return cache server</returns>
            public static CacheServer GetCacheServer<T>(CacheOptions<T> cacheOptions) where T : CacheResponse, new()
            {
                return GetCacheServersProxy?.Invoke(cacheOptions);
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
        /// <param name="options">Request options</param>
        /// <returns>Reurn cache result</returns>
        static TResponse ExecuteCommand<TResponse>(CacheOptions<TResponse> options) where TResponse : CacheResponse, new()
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            return options.Execute();
        }

        #endregion
    }
}
