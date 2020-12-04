using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using EZNEW.Cache.Constant;
using EZNEW.Cache.SortedSet;
using EZNEW.Code;
using EZNEW.Selection;
using EZNEW.Cache.Provider.Memory.Abstractions;
using System.Collections;
using EZNEW.Cache.Keys;
using EZNEW.Cache.String;
using EZNEW.Cache.List;
using EZNEW.Cache.Set;
using EZNEW.Cache.Server;
using EZNEW.Cache.Hash;

namespace EZNEW.Cache.Provider.Memory
{
    /// <summary>
    /// In memory cache provider
    /// </summary>
    public class MemoryProvider : ICacheProvider
    {
        ///// <summary>
        ///// Memory cache
        ///// </summary>
        //static readonly MemoryCache MemoryCache = null;

        /// <summary>
        /// Default memory cache name
        /// </summary>
        const string DefaultMemoryCacheName = "EZNEW_MEMORYCACHEDEFAULTNAME";

        /// <summary>
        /// Memory cache collection
        /// </summary>
        static readonly Dictionary<string, MemoryCacheDatabase> MemoryCacheCollection = new Dictionary<string, MemoryCacheDatabase>();

        static MemoryProvider()
        {
            MemoryCacheCollection = new Dictionary<string, MemoryCacheDatabase>()
            {
                {
                    DefaultMemoryCacheName
                    ,new MemoryCacheDatabase()
                    {
                       Index=0,
                       Name=DefaultMemoryCacheName,
                       Store=new MemoryCache(Options.Create(new MemoryCacheOptions()))
                    }
                }
            };
        }

        #region String

        #region StringSetRange

        /// <summary>
        /// Overwrites part of the string stored at key, starting at the specified offset,
        /// for the entire length of value. If the offset is larger than the current length
        /// of the string at key, the string is padded with zero-bytes to make offset fit.
        /// Non-existing keys are considered as empty strings, so this options will make
        /// sure it holds a string large enough to be able to set value at offset.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="options">Options</param>
        /// <returns>Return string set range response</returns>
        public async Task<IEnumerable<StringSetRangeResponse>> StringSetRangeAsync(CacheServer server, StringSetRangeOptions options)
        {
            string key = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(key))
            {
                return WrapResponse(CacheResponse.FailResponse<StringSetRangeResponse>(CacheCodes.KeyIsNullOrEmpty));
            }
            if (options.Offset < 0)
            {
                return WrapResponse(CacheResponse.FailResponse<StringSetRangeResponse>(CacheCodes.OffsetLessZero));
            }
            var databases = GetDatabases(server);
            List<StringSetRangeResponse> responses = new List<StringSetRangeResponse>(databases.Count);
            foreach (var db in databases)
            {
                var found = db.Store.TryGetEntry(key, out ICacheEntry cacheEntry);
                var cacheValue = found ? cacheEntry?.Value?.ToString() ?? string.Empty : string.Empty;
                var currentLength = cacheValue.Length;
                var minLength = options.Offset;
                if (currentLength == minLength)
                {
                    cacheValue = cacheValue + options.Value ?? string.Empty;
                }
                else if (currentLength > minLength)
                {
                    cacheValue = cacheValue.Insert(minLength, options.Value);
                }
                else
                {
                    cacheValue += new string('\x00', minLength - currentLength) + options.Value;
                }
                if (found)
                {
                    cacheEntry.SetValue(cacheValue);
                }
                else
                {
                    using (var newEntry = db.Store.CreateEntry(key))
                    {
                        newEntry.Value = cacheValue;
                        SetExpiration(newEntry, options.Expiration);
                    }
                }
                var response = CacheResponse.SuccessResponse<StringSetRangeResponse>(server, db);
                response.NewValueLength = cacheValue?.Length ?? 0;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region StringSetBit

        /// <summary>
        /// Sets or clears the bit at offset in the string value stored at key. The bit is
        /// either set or cleared depending on value, which can be either 0 or 1. When key
        /// does not exist, a new string value is created.The string is grown to make sure
        /// it can hold a bit at offset.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return string set bit response</returns>
        public async Task<IEnumerable<StringSetBitResponse>> StringSetBitAsync(CacheServer server, StringSetBitOptions options)
        {
            string key = options?.Key?.GetActualKey() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(key))
            {
                return WrapResponse(CacheResponse.FailResponse<StringSetBitResponse>(CacheCodes.KeyIsNullOrEmpty));
            }
            if (options.Offset < 0)
            {
                return WrapResponse(CacheResponse.FailResponse<StringSetBitResponse>(CacheCodes.OffsetLessZero));
            }
            var databases = GetDatabases(server);
            List<StringSetBitResponse> responses = new List<StringSetBitResponse>(databases.Count);
            foreach (var db in databases)
            {
                var found = db.Store.TryGetEntry(key, out ICacheEntry cacheEntry);
                var bitValue = options.Bit ? '1' : '0';
                var oldBitValue = false;
                var cacheValue = found ? cacheEntry?.Value?.ToString() ?? string.Empty : string.Empty;

                string binaryValue = cacheValue.ToBinaryString(GetEncoding());
                var binaryArray = binaryValue.ToCharArray();
                if (binaryArray.Length > options.Offset)
                {
                    oldBitValue = binaryArray[options.Offset] == '1';
                    binaryArray[options.Offset] = bitValue;
                }
                else
                {
                    var diffLength = options.Offset - binaryArray.LongLength;
                    var diffArray = new char[diffLength + 1];
                    for (var r = 0; r < diffLength; r++)
                    {
                        diffArray[r] = '0';
                    }
                    diffArray[diffLength] = bitValue;
                    binaryArray = binaryArray.Concat(diffArray).ToArray();
                }
                cacheValue = new string(binaryArray);
                cacheValue = cacheValue.ToOriginalString(GetEncoding());

                if (found)
                {
                    cacheEntry.SetValue(cacheValue);
                }
                else
                {
                    using (var entry = db.Store.CreateEntry(key))
                    {
                        entry.Value = cacheValue;
                        SetExpiration(entry, options.Expiration);
                    }
                }
                var response = CacheResponse.SuccessResponse<StringSetBitResponse>(server, db);
                response.OldBitValue = oldBitValue;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region StringSet

        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten,
        /// regardless of its type.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return string set response</returns>
        public async Task<IEnumerable<StringSetResponse>> StringSetAsync(CacheServer server, StringSetOptions options)
        {
            if (options?.Items.IsNullOrEmpty() ?? true)
            {
                return WrapResponse(CacheResponse.FailResponse<StringSetResponse>(CacheCodes.ValuesIsNullOrEmpty));
            }
            List<StringEntrySetResult> results = new List<StringEntrySetResult>(options.Items.Count);
            var databases = GetDatabases(server);
            List<StringSetResponse> responses = new List<StringSetResponse>(databases.Count);
            foreach (var db in databases)
            {
                foreach (var data in options.Items)
                {
                    string cacheKey = data.Key?.GetActualKey() ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(cacheKey))
                    {
                        continue;
                    }
                    bool found = db.Store.TryGetEntry(cacheKey, out var nowEntry);
                    bool setCache = data.When == CacheSetWhen.Always
                        || data.When == CacheSetWhen.Exists && found
                        || data.When == CacheSetWhen.NotExists && !found;
                    if (!setCache)
                    {
                        continue;
                    }
                    using (var entry = db.Store.CreateEntry(cacheKey))
                    {
                        entry.Value = data.Value?.ToString() ?? string.Empty;
                        SetExpiration(entry, data.Expiration);
                    }
                    results.Add(new StringEntrySetResult()
                    {
                        Key = cacheKey,
                        SetSuccess = true
                    });
                }
                var response = CacheResponse.SuccessResponse<StringSetResponse>(server, db);
                response.Results = results;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region StringLength

        /// <summary>
        /// Returns the length of the string value stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return string length response</returns>
        public async Task<IEnumerable<StringLengthResponse>> StringLengthAsync(CacheServer server, StringLengthOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<StringLengthResponse>(CacheCodes.KeyIsNullOrEmpty));
            }
            var databases = GetDatabases(server);
            List<StringLengthResponse> responses = new List<StringLengthResponse>(databases.Count);
            foreach (var db in databases)
            {
                var response = CacheResponse.SuccessResponse<StringLengthResponse>(server, db);
                if (db.Store.TryGetValue<string>(cacheKey, out var value))
                {
                    response.Length = value?.Length ?? 0;
                }
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region StringIncrement

        /// <summary>
        /// Increments the string representing a floating point number stored at key by the
        /// specified increment. If the key does not exist, it is set to 0 before performing
        /// the operation. The precision of the output is fixed at 17 digits after the decimal
        /// point regardless of the actual internal precision of the computation.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return string increment response</returns>
        public async Task<IEnumerable<StringIncrementResponse>> StringIncrementAsync(CacheServer server, StringIncrementOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<StringIncrementResponse>(CacheCodes.KeyIsNullOrEmpty));
            }
            var databases = GetDatabases(server);
            List<StringIncrementResponse> responses = new List<StringIncrementResponse>(databases.Count);
            foreach (var db in databases)
            {
                StringIncrementResponse response = null;
                long nowValue = 0;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    if (long.TryParse(entry.Value?.ToString(), out nowValue))
                    {
                        nowValue += options.Value;
                        entry.SetValue(nowValue);
                    }
                    else
                    {
                        response = CacheResponse.FailResponse<StringIncrementResponse>(CacheCodes.ValueCannotBeCalculated, server: server, database: db);
                        responses.Add(response);
                        continue;
                    }
                }
                else
                {
                    using (entry = db.Store.CreateEntry(cacheKey))
                    {
                        nowValue = options.Value;
                        entry.Value = options.Value;
                        SetExpiration(entry, options.Expiration);
                    }
                }
                response = CacheResponse.SuccessResponse<StringIncrementResponse>(server, db);
                response.NewValue = nowValue;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region StringGetWithExpiry

        /// <summary>
        /// Get the value of key. If the key does not exist the special value nil is returned.
        /// An error is returned if the value stored at key is not a string, because GET
        /// only handles string values.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return string get with expiry response</returns>
        public async Task<IEnumerable<StringGetWithExpiryResponse>> StringGetWithExpiryAsync(CacheServer server, StringGetWithExpiryOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<StringGetWithExpiryResponse>(CacheCodes.KeyIsNullOrEmpty));
            }
            string nowValue = string.Empty;
            TimeSpan? expriy = null;
            var databases = GetDatabases(server);
            List<StringGetWithExpiryResponse> responses = new List<StringGetWithExpiryResponse>(databases.Count);
            foreach (var db in databases)
            {
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    nowValue = entry.Value?.ToString() ?? string.Empty;
                    expriy = GetExpiration(entry).Item2;
                }
                var response = CacheResponse.SuccessResponse<StringGetWithExpiryResponse>(server, db);
                response.Value = nowValue;
                response.Expiry = expriy;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region StringGetSet

        /// <summary>
        /// Atomically sets key to value and returns the old value stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return string get set response</returns>
        public async Task<IEnumerable<StringGetSetResponse>> StringGetSetAsync(CacheServer server, StringGetSetOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<StringGetSetResponse>(CacheCodes.KeyIsNullOrEmpty));
            }
            var oldValue = string.Empty;
            var databases = GetDatabases(server);
            List<StringGetSetResponse> responses = new List<StringGetSetResponse>(databases.Count);
            foreach (var db in databases)
            {
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    oldValue = entry.Value?.ToString() ?? string.Empty;
                    entry.SetValue(options.NewValue);
                }
                else
                {
                    using (entry = db.Store.CreateEntry(cacheKey))
                    {
                        entry.SetValue(options.NewValue);
                    }
                }
                var response = CacheResponse.SuccessResponse<StringGetSetResponse>(server, db);
                response.OldValue = oldValue;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region StringGetRange

        /// <summary>
        /// Returns the substring of the string value stored at key, determined by the offsets
        /// start and end (both are inclusive). Negative offsets can be used in order to
        /// provide an offset starting from the end of the string. So -1 means the last character,
        /// -2 the penultimate and so forth.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return string get range response</returns>
        public async Task<IEnumerable<StringGetRangeResponse>> StringGetRangeAsync(CacheServer server, StringGetRangeOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<StringGetRangeResponse>(CacheCodes.KeyIsNullOrEmpty));
            }
            var subValue = string.Empty;
            var databases = GetDatabases(server);
            List<StringGetRangeResponse> responses = new List<StringGetRangeResponse>(databases.Count);
            foreach (var db in databases)
            {
                if (db.Store.TryGetValue<string>(cacheKey, out var value) && !string.IsNullOrWhiteSpace(value))
                {
                    int start = options.Start;
                    int end = options.End;
                    int valueLength = (value ?? string.Empty).Length;
                    if (start < 0)
                    {
                        start = value.Length - Math.Abs(start);
                    }
                    if (start < 0 || start >= valueLength)
                    {
                        responses.Add(CacheResponse.FailResponse<StringGetRangeResponse>(CacheCodes.OffsetError, server: server, database: db));
                        continue;
                    }
                    if (end < 0)
                    {
                        end = value.Length - Math.Abs(end);
                    }
                    if (end < 0 || end >= valueLength)
                    {
                        responses.Add(CacheResponse.FailResponse<StringGetRangeResponse>(CacheCodes.OffsetError, server: server, database: db));
                        continue;
                    }
                    subValue = value.Substring(Math.Min(start, end), Math.Abs(end - start) + 1);
                }
                var response = CacheResponse.SuccessResponse<StringGetRangeResponse>(server, db);
                response.Value = subValue;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region StringGetBit

        /// <summary>
        /// Returns the bit value at offset in the string value stored at key. When offset
        /// is beyond the string length, the string is assumed to be a contiguous space with
        /// 0 bits
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return string get bit response</returns>
        public async Task<IEnumerable<StringGetBitResponse>> StringGetBitAsync(CacheServer server, StringGetBitOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<StringGetBitResponse>(CacheCodes.KeyIsNullOrEmpty));
            }
            var databases = GetDatabases(server);
            List<StringGetBitResponse> responses = new List<StringGetBitResponse>(databases.Count);
            foreach (var db in databases)
            {
                char bit = '0';
                if (db.Store.TryGetValue<string>(cacheKey, out var value) && !string.IsNullOrWhiteSpace(value))
                {
                    var binaryArray = value.ToBinaryString(GetEncoding()).ToCharArray();
                    var offset = options.Offset;
                    if (offset < 0)
                    {
                        offset = binaryArray.LongLength - Math.Abs(offset);
                    }
                    if (offset < 0 || offset >= binaryArray.LongLength)
                    {
                        responses.Add(CacheResponse.FailResponse<StringGetBitResponse>(CacheCodes.OffsetError, server: server, database: db));
                        continue;
                    }
                    bit = binaryArray[offset];
                }
                var response = CacheResponse.SuccessResponse<StringGetBitResponse>(server, db);
                response.Bit = bit == '1';
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region StringGet

        /// <summary>
        /// Returns the values of all specified keys. For every key that does not hold a
        /// string value or does not exist, the special value nil is returned.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return string get response</returns>
        public async Task<IEnumerable<StringGetResponse>> StringGetAsync(CacheServer server, StringGetOptions options)
        {
            if (options?.Keys.IsNullOrEmpty() ?? true)
            {
                return WrapResponse(CacheResponse.FailResponse<StringGetResponse>(CacheCodes.KeyIsNullOrEmpty));
            }
            var databases = GetDatabases(server);
            List<StringGetResponse> responses = new List<StringGetResponse>(databases.Count);
            foreach (var db in databases)
            {
                var datas = new List<CacheEntry>();
                foreach (var key in options.Keys)
                {
                    var cacheKey = key.GetActualKey();
                    if (string.IsNullOrWhiteSpace(cacheKey))
                    {
                        continue;
                    }
                    if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                    {
                        datas.Add(new CacheEntry()
                        {
                            Key = key,
                            Value = entry.Value?.ToString() ?? string.Empty,
                            Expiration = new CacheExpiration()
                            {
                                AbsoluteExpiration = entry.AbsoluteExpiration,
                                AbsoluteExpirationRelativeToNow = entry.AbsoluteExpirationRelativeToNow,
                                SlidingExpiration = entry.SlidingExpiration.HasValue
                            }
                        });
                    }
                }
                var response = CacheResponse.SuccessResponse<StringGetResponse>(server, db);
                response.Values = datas;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region StringDecrement

        /// <summary>
        /// Decrements the number stored at key by decrement. If the key does not exist,
        /// it is set to 0 before performing the operation. An error is returned if the key
        /// contains a value of the wrong type or contains a string that is not representable
        /// as integer. This operation is limited to 64 bit signed integers.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return string decrement response</returns>
        public async Task<IEnumerable<StringDecrementResponse>> StringDecrementAsync(CacheServer server, StringDecrementOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<StringDecrementResponse>(CacheCodes.KeyIsNullOrEmpty));
            }
            var databases = GetDatabases(server);
            List<StringDecrementResponse> responses = new List<StringDecrementResponse>(databases.Count);
            foreach (var db in databases)
            {
                long nowValue = 0;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    if (long.TryParse(entry.Value?.ToString(), out nowValue))
                    {
                        nowValue -= options.Value;
                        entry.SetValue(nowValue);
                    }
                    else
                    {
                        responses.Add(CacheResponse.FailResponse<StringDecrementResponse>(CacheCodes.ValueCannotBeCalculated, server: server, database: db));
                        continue;
                    }
                }
                else
                {
                    using (entry = db.Store.CreateEntry(cacheKey))
                    {
                        nowValue = options.Value;
                        entry.Value = options.Value;
                        SetExpiration(entry, options.Expiration);
                    }
                }
                var response = CacheResponse.SuccessResponse<StringDecrementResponse>(server, db);
                response.NewValue = nowValue;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region StringBitPosition

        /// <summary>
        /// Return the position of the first bit set to 1 or 0 in a string. The position
        /// is returned thinking at the string as an array of bits from left to right where
        /// the first byte most significant bit is at position 0, the second byte most significant
        /// bit is at position 8 and so forth. An start and end may be specified; these are
        /// in bytes, not bits; start and end can contain negative values in order to index
        /// bytes starting from the end of the string, where -1 is the last byte, -2 is the
        /// penultimate, and so forth.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return string bit position response</returns>
        public async Task<IEnumerable<StringBitPositionResponse>> StringBitPositionAsync(CacheServer server, StringBitPositionOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<StringBitPositionResponse>(CacheCodes.KeyIsNullOrEmpty));
            }
            if ((options.Start >= 0 && options.End < options.Start) || (options.Start < 0 && options.End > options.Start))
            {
                return WrapResponse(CacheResponse.FailResponse<StringBitPositionResponse>(CacheCodes.OffsetError));
            }
            var databases = GetDatabases(server);
            List<StringBitPositionResponse> responses = new List<StringBitPositionResponse>(databases.Count);
            foreach (var db in databases)
            {
                bool hasValue = false;
                long position = 0;
                if (db.Store.TryGetValue<string>(cacheKey, out var value) && !string.IsNullOrWhiteSpace(value))
                {
                    char[] valueArray = value.ToBinaryString(GetEncoding()).ToCharArray();
                    var matchBit = options.Bit ? '1' : '0';
                    var length = valueArray.LongLength;
                    var start = options.Start;
                    var end = options.End;
                    if (start < 0)
                    {
                        start = length - Math.Abs(start);
                    }
                    if (start < 0 || start >= length)
                    {
                        responses.Add(CacheResponse.FailResponse<StringBitPositionResponse>(CacheCodes.OffsetError, server: server, database: db));
                        continue;
                    }
                    if (end < 0)
                    {
                        end = length - Math.Abs(end);
                    }
                    if (end < 0 || end >= length)
                    {
                        responses.Add(CacheResponse.FailResponse<StringBitPositionResponse>(CacheCodes.OffsetError, server: server, database: db));
                        continue;
                    }
                    var begin = Math.Min(start, end);
                    end = Math.Max(start, end) + 1;
                    for (var i = begin; i < end; i++)
                    {
                        if (valueArray[i] == matchBit)
                        {
                            position = i;
                            hasValue = true;
                            break;
                        }
                    }
                }
                var response = CacheResponse.SuccessResponse<StringBitPositionResponse>(server, db);
                response.HasValue = hasValue;
                response.Position = position;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region StringBitOperation

        /// <summary>
        /// Perform a bitwise operation between multiple keys (containing string values)
        ///  and store the result in the destination key. The BITOP options supports four
        ///  bitwise operations; note that NOT is a unary operator: the second key should
        ///  be omitted in this case and only the first key will be considered. The result
        /// of the operation is always stored at destkey.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return string bit operation response</returns>
        public async Task<IEnumerable<StringBitOperationResponse>> StringBitOperationAsync(CacheServer server, StringBitOperationOptions options)
        {
            if (options.Keys.IsNullOrEmpty() || string.IsNullOrWhiteSpace(options.DestinationKey))
            {
                return WrapResponse(CacheResponse.FailResponse<StringBitOperationResponse>(CacheCodes.KeyIsNullOrEmpty));
            }
            if (options.Keys.Count > 1 && options.Bitwise == CacheBitwise.Not)
            {
                throw new NotSupportedException($" CacheBitwise.Not can only operate on one key");
            }
            var databases = GetDatabases(server);
            List<StringBitOperationResponse> responses = new List<StringBitOperationResponse>(databases.Count);
            foreach (var db in databases)
            {
                BitArray bitArray = null;
                foreach (var key in options.Keys)
                {
                    if (db.Store.TryGetEntry(key, out ICacheEntry cacheEntry))
                    {
                        var binaryString = (cacheEntry?.Value?.ToString() ?? string.Empty).ToBinaryString(GetEncoding());
                        var binaryArray = new BitArray(binaryString.Select(c => (int)c).ToArray());
                        if (bitArray == null)
                        {
                            bitArray = binaryArray;
                        }
                        else
                        {
                            bitArray = options.Bitwise switch
                            {
                                CacheBitwise.And => bitArray.And(binaryArray),
                                CacheBitwise.Or => bitArray.Or(binaryArray),
                                CacheBitwise.Xor => bitArray.Xor(binaryArray),
                                CacheBitwise.Not => binaryArray.Not(),
                                _ => throw new NotSupportedException()
                            };
                        }
                    }
                }
                if (bitArray == null)
                {
                    responses.Add(CacheResponse.FailResponse<StringBitOperationResponse>(CacheCodes.ValuesIsNullOrEmpty, server: server, database: db));
                    continue;
                }
                var bitString = string.Join("", bitArray.Cast<bool>().Select(c => c ? 1 : 0));
                var originalString = bitString.ToOriginalString(GetEncoding());
                var desResult = await StringSetAsync(server, new StringSetOptions()
                {
                    Items = new List<CacheEntry>()
                {
                    new CacheEntry()
                    {
                        Key=options.DestinationKey,
                        Type=CacheKeyType.String,
                        Value=originalString,
                        Expiration=options.Expiration
                    }
                }
                }).ConfigureAwait(false);
                if (!desResult.IsNullOrEmpty())
                {
                    responses.Add(new StringBitOperationResponse()
                    {
                        Success = true,
                        DestinationValueLength = originalString.Length,
                        CacheServer = server,
                        Database = db
                    });
                }
            }
            return responses;
        }

        #endregion

        #region StringBitCount

        /// <summary>
        /// Count the number of set bits (population counting) in a string. By default all
        /// the bytes contained in the string are examined.It is possible to specify the
        /// counting operation only in an interval passing the additional arguments start
        /// and end. Like for the GETRANGE options start and end can contain negative values
        /// in order to index bytes starting from the end of the string, where -1 is the
        /// last byte, -2 is the penultimate, and so forth.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return string bit count response</returns>
        public async Task<IEnumerable<StringBitCountResponse>> StringBitCountAsync(CacheServer server, StringBitCountOptions options)
        {
            if (string.IsNullOrWhiteSpace(options?.Key))
            {
                throw new ArgumentNullException($"{nameof(StringBitCountOptions)}.{nameof(StringBitCountOptions.Key)}");
            }
            var cacheKey = options.Key.GetActualKey();
            long bitCount = 0;
            var databases = GetDatabases(server);
            List<StringBitCountResponse> responses = new List<StringBitCountResponse>(databases.Count);
            foreach (var db in databases)
            {
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var value = entry.Value?.ToString() ?? string.Empty;
                    bitCount = value.ToBinaryString(GetEncoding()).Count(c => c == '1');
                }
                responses.Add(new StringBitCountResponse()
                {
                    Success = true,
                    BitNum = bitCount
                });
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region StringAppend

        /// <summary>
        /// If key already exists and is a string, this options appends the value at the
        /// end of the string. If key does not exist it is created and set as an empty string,
        /// so APPEND will be similar to SET in this special case.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return string append response</returns>
        public async Task<IEnumerable<StringAppendResponse>> StringAppendAsync(CacheServer server, StringAppendOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<StringAppendResponse>(CacheCodes.KeyIsNullOrEmpty));
            }
            long valueLength = 0;
            var databases = GetDatabases(server);
            List<StringAppendResponse> responses = new List<StringAppendResponse>(databases.Count);
            foreach (var db in databases)
            {
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var nowValue = entry.Value?.ToString() ?? string.Empty;
                    nowValue += options.Value ?? string.Empty;
                    valueLength = nowValue.Length;
                    entry.SetValue(nowValue);
                }
                else
                {
                    using (entry = db.Store.CreateEntry(cacheKey))
                    {
                        entry.SetValue(options.Value);
                        SetExpiration(entry, options.Expiration);
                    }
                }
                var response = CacheResponse.SuccessResponse<StringAppendResponse>(server, db);
                response.NewValueLength = valueLength;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #endregion

        #region List

        #region ListTrim

        /// <summary>
        /// Trim an existing list so that it will contain only the specified range of elements
        /// specified. Both start and stop are zero-based indexes, where 0 is the first element
        /// of the list (the head), 1 the next element and so on. For example: LTRIM foobar
        /// 0 2 will modify the list stored at foobar so that only the first three elements
        /// of the list will remain. start and end can also be negative numbers indicating
        /// offsets from the end of the list, where -1 is the last element of the list, -2
        /// the penultimate element and so on.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return list trim response</returns>
        public async Task<IEnumerable<ListTrimResponse>> ListTrimAsync(CacheServer server, ListTrimOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<ListTrimResponse>(CacheCodes.KeyIsNullOrEmpty));
            }
            var databases = GetDatabases(server);
            List<ListTrimResponse> responses = new List<ListTrimResponse>(databases.Count);
            foreach (var db in databases)
            {
                ListTrimResponse response = null;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    if (!(entry.Value is List<string> list))
                    {
                        responses.Add(CacheResponse.FailResponse<ListTrimResponse>(CacheCodes.ValueIsNotList, server: server, database: db));
                        continue;
                    }
                    var start = options.Start;
                    var end = options.Stop;
                    int count = list.Count;
                    if (start < 0)
                    {
                        start = count - Math.Abs(start);
                    }
                    if (start < 0 || start >= count)
                    {
                        responses.Add(CacheResponse.FailResponse<ListTrimResponse>(CacheCodes.OffsetError, server: server, database: db));
                        continue;
                    }
                    if (end < 0)
                    {
                        end = count - Math.Abs(end);
                    }
                    if (end < 0 || end >= count)
                    {
                        responses.Add(CacheResponse.FailResponse<ListTrimResponse>(CacheCodes.OffsetError, server: server, database: db));
                        continue;
                    }
                    var begin = Math.Min(start, end);
                    var takeCount = Math.Abs(end - start) + 1;
                    var nowList = list.Skip(begin).Take(takeCount).ToList();
                    entry.SetValue(nowList);
                    response = CacheResponse.SuccessResponse<ListTrimResponse>();
                }
                else
                {
                    response = CacheResponse.FailResponse<ListTrimResponse>(CacheCodes.KeyIsNotExist);
                }
                response.CacheServer = server;
                response.Database = db;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region ListSetByIndex

        /// <summary>
        /// Sets the list element at index to value. For more information on the index argument,
        ///  see ListGetByIndex. An error is returned for out of range indexes.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return list set by index response</returns>
        public async Task<IEnumerable<ListSetByIndexResponse>> ListSetByIndexAsync(CacheServer server, ListSetByIndexOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<ListSetByIndexResponse>(CacheCodes.KeyIsNullOrEmpty));
            }
            var databases = GetDatabases(server);
            List<ListSetByIndexResponse> responses = new List<ListSetByIndexResponse>(databases.Count);
            foreach (var db in databases)
            {
                ListSetByIndexResponse response = null;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var list = entry.Value as List<string>;
                    if (list == null)
                    {
                        responses.Add(CacheResponse.FailResponse<ListSetByIndexResponse>(CacheCodes.ValueIsNotList, server: server, database: db));
                        continue;
                    }
                    var index = options.Index;
                    if (index < 0)
                    {
                        index = list.Count - Math.Abs(index);
                    }
                    if (index < 0 || index >= list.Count)
                    {
                        responses.Add(CacheResponse.FailResponse<ListSetByIndexResponse>(CacheCodes.OffsetError, server: server, database: db));
                    }
                    list[index] = options.Value;
                    response = CacheResponse.SuccessResponse<ListSetByIndexResponse>();
                }
                else
                {
                    response = CacheResponse.FailResponse<ListSetByIndexResponse>(CacheCodes.KeyIsNotExist);
                }
                response.CacheServer = server;
                response.Database = db;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region ListRightPush

        /// <summary>
        /// Insert all the specified values at the tail of the list stored at key. If key
        /// does not exist, it is created as empty list before performing the push operation.
        /// Elements are inserted one after the other to the tail of the list, from the leftmost
        /// element to the rightmost element. So for instance the options RPUSH mylist a
        /// b c will result into a list containing a as first element, b as second element
        /// and c as third element.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return list right push</returns>
        public async Task<IEnumerable<ListRightPushResponse>> ListRightPushAsync(CacheServer server, ListRightPushOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<ListRightPushResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            if (options.Values.IsNullOrEmpty())
            {
                return WrapResponse(CacheResponse.FailResponse<ListRightPushResponse>(CacheCodes.ValuesIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<ListRightPushResponse> responses = new List<ListRightPushResponse>(databases.Count);
            foreach (var db in databases)
            {
                ListRightPushResponse response;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    if (!(entry.Value is List<string> list))
                    {
                        responses.Add(CacheResponse.FailResponse<ListRightPushResponse>(CacheCodes.ValueIsNotList, server: server, database: db));
                        continue;
                    }
                    list = list.Concat(options.Values).ToList();
                    entry.SetValue(list);
                    response = CacheResponse.SuccessResponse<ListRightPushResponse>();
                    response.NewListLength = list.Count;
                }
                else
                {
                    using (entry = db.Store.CreateEntry(cacheKey))
                    {
                        entry.SetValue(new List<string>(options.Values));
                        SetExpiration(entry, options.Expiration);
                    }
                    response = CacheResponse.SuccessResponse<ListRightPushResponse>();
                    response.NewListLength = options.Values.Count;
                }
                response.CacheServer = server;
                response.Database = db;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region ListRightPopLeftPush

        /// <summary>
        /// Atomically returns and removes the last element (tail) of the list stored at
        /// source, and pushes the element at the first element (head) of the list stored
        /// at destination.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return list right pop left response</returns>
        public async Task<IEnumerable<ListRightPopLeftPushResponse>> ListRightPopLeftPushAsync(CacheServer server, ListRightPopLeftPushOptions options)
        {
            string sourceCacheKey = options?.SourceKey?.GetActualKey();
            string destionationCacheKey = options?.DestinationKey?.GetActualKey();
            if (string.IsNullOrWhiteSpace(sourceCacheKey) || string.IsNullOrWhiteSpace(destionationCacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<ListRightPopLeftPushResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<ListRightPopLeftPushResponse> responses = new List<ListRightPopLeftPushResponse>(databases.Count);
            foreach (var db in databases)
            {
                ListRightPopLeftPushResponse response = null;
                if (db.Store.TryGetEntry(sourceCacheKey, out var entry) && entry != null)
                {
                    if (!(entry.Value is List<string> list))
                    {
                        responses.Add(CacheResponse.FailResponse<ListRightPopLeftPushResponse>(CacheCodes.ValueIsNotList, server: server, database: db));
                        continue;
                    }
                    if (list.Count < 1)
                    {
                        responses.Add(CacheResponse.FailResponse<ListRightPopLeftPushResponse>(CacheCodes.ListIsEmpty, server: server, database: db));
                        continue;
                    }
                    List<string> desList = null;
                    if (db.Store.TryGetEntry(destionationCacheKey, out var desEntry) && desEntry != null)
                    {
                        desList = desEntry.Value as List<string>;
                        if (desList == null)
                        {
                            responses.Add(CacheResponse.FailResponse<ListRightPopLeftPushResponse>(CacheCodes.ValueIsNotList, server: server, database: db));
                            continue;
                        }
                    }
                    var index = list.Count - 1;
                    string value = list[index];
                    list.RemoveAt(index);
                    if (desEntry == null)
                    {
                        using (desEntry = db.Store.CreateEntry(destionationCacheKey))
                        {
                            desEntry.Value = new List<string>() { value };
                            SetExpiration(desEntry, options.Expiration);
                        }
                    }
                    else
                    {
                        desList.Insert(0, value);
                    }
                    response = CacheResponse.SuccessResponse<ListRightPopLeftPushResponse>();
                    response.PopValue = value;
                }
                else
                {
                    response = CacheResponse.FailResponse<ListRightPopLeftPushResponse>(CacheCodes.KeyIsNotExist);
                }
                response.CacheServer = server;
                response.Database = db;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region ListRightPop

        /// <summary>
        /// Removes and returns the last element of the list stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return list right pop response</returns>
        public async Task<IEnumerable<ListRightPopResponse>> ListRightPopAsync(CacheServer server, ListRightPopOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<ListRightPopResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<ListRightPopResponse> responses = new List<ListRightPopResponse>(databases.Count);
            foreach (var db in databases)
            {
                ListRightPopResponse response = null;
                string value = string.Empty;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    if (!(entry.Value is List<string> list))
                    {
                        responses.Add(CacheResponse.FailResponse<ListRightPopResponse>(CacheCodes.ValueIsNotList, server: server, database: db));
                        continue;
                    }
                    if (list.Count < 1)
                    {
                        responses.Add(CacheResponse.FailResponse<ListRightPopResponse>(CacheCodes.ListIsEmpty, server: server, database: db));
                        continue;
                    }
                    var index = list.Count - 1;
                    value = list[index];
                    list.RemoveAt(index);
                    response = CacheResponse.SuccessResponse<ListRightPopResponse>();
                    response.PopValue = value;
                }
                else
                {
                    response = CacheResponse.FailResponse<ListRightPopResponse>(CacheCodes.KeyIsNotExist);
                }
                response.CacheServer = server;
                response.Database = db;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region ListRemove

        /// <summary>
        /// Removes the first count occurrences of elements equal to value from the list
        /// stored at key. The count argument influences the operation in the following way
        /// count > 0: Remove elements equal to value moving from head to tail. count less 0:
        /// Remove elements equal to value moving from tail to head. count = 0: Remove all
        /// elements equal to value.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return list remove response</returns>
        public async Task<IEnumerable<ListRemoveResponse>> ListRemoveAsync(CacheServer server, ListRemoveOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<ListRemoveResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<ListRemoveResponse> responses = new List<ListRemoveResponse>(databases.Count);
            foreach (var db in databases)
            {
                ListRemoveResponse response = null;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    if (!(entry.Value is List<string> list))
                    {
                        responses.Add(CacheResponse.FailResponse<ListRemoveResponse>(CacheCodes.ValueIsNotList, server: server, database: db));
                        continue;
                    }
                    var removeCount = 0;
                    if (options.Count == 0)
                    {
                        removeCount = list.RemoveAll(a => a == options.Value);
                    }
                    else
                    {
                        var count = Math.Abs(options.Count);
                        var findLast = options.Count < 0;
                        for (var i = 0; i < count; i++)
                        {
                            var index = findLast ? list.FindLastIndex(c => c == options.Value) : list.FindIndex(c => c == options.Value);
                            if (index < 0)
                            {
                                break;
                            }
                            removeCount++;
                            list.RemoveAt(index);
                        }
                    }
                    response = CacheResponse.SuccessResponse<ListRemoveResponse>();
                    response.RemoveCount = removeCount;
                }
                else
                {
                    response = CacheResponse.FailResponse<ListRemoveResponse>(CacheCodes.KeyIsNotExist);
                }
                response.CacheServer = server;
                response.Database = db;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region ListRange

        /// <summary>
        /// Returns the specified elements of the list stored at key. The offsets start and
        /// stop are zero-based indexes, with 0 being the first element of the list (the
        /// head of the list), 1 being the next element and so on. These offsets can also
        /// be negative numbers indicating offsets starting at the end of the list.For example,
        /// -1 is the last element of the list, -2 the penultimate, and so on. Note that
        /// if you have a list of numbers from 0 to 100, LRANGE list 0 10 will return 11
        /// elements, that is, the rightmost item is included.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return list range response</returns>
        public async Task<IEnumerable<ListRangeResponse>> ListRangeAsync(CacheServer server, ListRangeOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<ListRangeResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<ListRangeResponse> responses = new List<ListRangeResponse>(databases.Count);
            foreach (var db in databases)
            {
                ListRangeResponse response;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    if (!(entry.Value is List<string> list))
                    {
                        responses.Add(CacheResponse.FailResponse<ListRangeResponse>(CacheCodes.ValueIsNotList, server: server, database: db));
                        continue;
                    }
                    var start = options.Start;
                    if (start < 0)
                    {
                        start = list.Count - Math.Abs(start);
                    }
                    if (start < 0 || start >= list.Count)
                    {
                        responses.Add(CacheResponse.FailResponse<ListRangeResponse>(CacheCodes.OffsetError, server: server, database: db));
                        continue;
                    }
                    var end = options.Stop;
                    if (end < 0)
                    {
                        end = list.Count - Math.Abs(end);
                    }
                    if (end < 0 || end >= list.Count)
                    {
                        responses.Add(CacheResponse.FailResponse<ListRangeResponse>(CacheCodes.OffsetError, server: server, database: db));
                        continue;
                    }
                    var begin = Math.Min(start, end);
                    response = CacheResponse.SuccessResponse<ListRangeResponse>();
                    response.Values = list.GetRange(begin, Math.Abs(end - start) + 1).ToList();
                }
                else
                {
                    response = CacheResponse.FailResponse<ListRangeResponse>(CacheCodes.KeyIsNotExist);
                }
                response.CacheServer = server;
                response.Database = db;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region ListLength

        /// <summary>
        /// Returns the length of the list stored at key. If key does not exist, it is interpreted
        ///  as an empty list and 0 is returned.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return list length response</returns>
        public async Task<IEnumerable<ListLengthResponse>> ListLengthAsync(CacheServer server, ListLengthOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<ListLengthResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<ListLengthResponse> responses = new List<ListLengthResponse>(databases.Count);
            foreach (var db in databases)
            {
                int length = 0;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    if (!(entry.Value is List<string> list))
                    {
                        responses.Add(CacheResponse.FailResponse<ListLengthResponse>(CacheCodes.ValueIsNotList, server: server, database: db));
                        continue;
                    }
                    length = list.Count;
                }
                var response = CacheResponse.SuccessResponse<ListLengthResponse>();
                response.Length = length;
                response.CacheServer = server;
                response.Database = db;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region ListLeftPush

        /// <summary>
        /// Insert the specified value at the head of the list stored at key. If key does
        ///  not exist, it is created as empty list before performing the push operations.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return list left push response</returns>
        public async Task<IEnumerable<ListLeftPushResponse>> ListLeftPushAsync(CacheServer server, ListLeftPushOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<ListLeftPushResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            if (options.Values.IsNullOrEmpty())
            {
                return WrapResponse(CacheResponse.FailResponse<ListLeftPushResponse>(CacheCodes.ValuesIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<ListLeftPushResponse> responses = new List<ListLeftPushResponse>(databases.Count);
            foreach (var db in databases)
            {
                ListLeftPushResponse response;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    if (!(entry.Value is List<string> list))
                    {
                        responses.Add(CacheResponse.FailResponse<ListLeftPushResponse>(CacheCodes.ValueIsNotList, server: server, database: db));
                        continue;
                    }
                    list = options.Values.Concat(list).ToList();
                    entry.SetValue(list);
                    response = CacheResponse.SuccessResponse<ListLeftPushResponse>();
                    response.NewListLength = list.Count;
                }
                else
                {
                    using (entry = db.Store.CreateEntry(cacheKey))
                    {
                        entry.SetValue(new List<string>(options.Values));
                        SetExpiration(entry, options.Expiration);
                    }
                    response = CacheResponse.SuccessResponse<ListLeftPushResponse>();
                    response.NewListLength = options.Values.Count;
                }
                response.CacheServer = server;
                response.Database = db;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region ListLeftPop

        /// <summary>
        /// Removes and returns the first element of the list stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return list left pop response</returns>
        public async Task<IEnumerable<ListLeftPopResponse>> ListLeftPopAsync(CacheServer server, ListLeftPopOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<ListLeftPopResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<ListLeftPopResponse> responses = new List<ListLeftPopResponse>(databases.Count);
            foreach (var db in databases)
            {
                ListLeftPopResponse response = null;
                string value = string.Empty;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    if (!(entry.Value is List<string> list))
                    {
                        responses.Add(CacheResponse.FailResponse<ListLeftPopResponse>(CacheCodes.ValueIsNotList, server: server, database: db));
                        continue;
                    }
                    if (list.Count < 1)
                    {
                        responses.Add(CacheResponse.FailResponse<ListLeftPopResponse>(CacheCodes.ListIsEmpty, server: server, database: db));
                        continue;
                    }
                    value = list[0];
                    list.RemoveAt(0);
                    entry.SetValue(list);
                    response = CacheResponse.SuccessResponse<ListLeftPopResponse>();
                    response.PopValue = value;
                }
                else
                {
                    response = CacheResponse.FailResponse<ListLeftPopResponse>(CacheCodes.KeyIsNotExist);
                }
                response.CacheServer = server;
                response.Database = db;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region ListInsertBefore

        /// <summary>
        /// Inserts value in the list stored at key either before or after the reference
        /// value pivot. When key does not exist, it is considered an empty list and no operation
        /// is performed.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return list insert begore response</returns>
        public async Task<IEnumerable<ListInsertBeforeResponse>> ListInsertBeforeAsync(CacheServer server, ListInsertBeforeOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<ListInsertBeforeResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<ListInsertBeforeResponse> responses = new List<ListInsertBeforeResponse>(databases.Count);
            foreach (var db in databases)
            {
                int newLength = 0;
                bool hasInsertValue = false;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    if (!(entry.Value is List<string> list))
                    {
                        responses.Add(CacheResponse.FailResponse<ListInsertBeforeResponse>(CacheCodes.ValueIsNotList, server: server, database: db));
                        continue;
                    }
                    var index = list.FindIndex(c => c == options.PivotValue);
                    if (index >= 0)
                    {
                        list.Insert(index, options.InsertValue);
                        entry.SetValue(list);
                        hasInsertValue = true;
                    }
                    newLength = list.Count;
                }
                responses.Add(new ListInsertBeforeResponse()
                {
                    Success = hasInsertValue,
                    NewListLength = newLength,
                    CacheServer = server,
                    Database = db
                });
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region ListInsertAfter

        /// <summary>
        /// Inserts value in the list stored at key either before or after the reference
        /// value pivot. When key does not exist, it is considered an empty list and no operation
        /// is performed.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return list insert after response</returns>
        public async Task<IEnumerable<ListInsertAfterResponse>> ListInsertAfterAsync(CacheServer server, ListInsertAfterOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<ListInsertAfterResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<ListInsertAfterResponse> responses = new List<ListInsertAfterResponse>(databases.Count);
            foreach (var db in databases)
            {
                int newLength = 0;
                bool hasInsertValue = false;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var list = entry.Value as List<string>;
                    if (list == null)
                    {
                        responses.Add(CacheResponse.FailResponse<ListInsertAfterResponse>(CacheCodes.ValueIsNotList, server: server, database: db));
                        continue;
                    }
                    var index = list.FindIndex(c => c == options.PivotValue);
                    if (index >= 0)
                    {
                        list.Insert(index + 1, options.InsertValue);
                        entry.SetValue(list);
                        hasInsertValue = true;
                    }
                    newLength = list.Count;
                }
                responses.Add(new ListInsertAfterResponse()
                {
                    NewListLength = newLength,
                    Success = hasInsertValue,
                    CacheServer = server,
                    Database = db
                });
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region ListGetByIndex

        /// <summary>
        /// Returns the element at index index in the list stored at key. The index is zero-based,
        /// so 0 means the first element, 1 the second element and so on. Negative indices
        /// can be used to designate elements starting at the tail of the list. Here, -1
        /// means the last element, -2 means the penultimate and so forth.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return list get by index response</returns>
        public async Task<IEnumerable<ListGetByIndexResponse>> ListGetByIndexAsync(CacheServer server, ListGetByIndexOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<ListGetByIndexResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<ListGetByIndexResponse> responses = new List<ListGetByIndexResponse>(databases.Count);
            foreach (var db in databases)
            {
                var value = "";
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var list = entry.Value as List<string>;
                    if (list == null)
                    {
                        responses.Add(CacheResponse.FailResponse<ListGetByIndexResponse>(CacheCodes.ValueIsNotList, server: server, database: db));
                        continue;
                    }
                    var index = options.Index;
                    if (index < 0)
                    {
                        index = list.Count - Math.Abs(index);
                    }
                    if (index < 0 || index >= list.Count)
                    {
                        responses.Add(CacheResponse.FailResponse<ListGetByIndexResponse>(CacheCodes.OffsetError, server: server, database: db));
                    }
                    value = list[index];
                }
                var response = CacheResponse.SuccessResponse<ListGetByIndexResponse>();
                response.Value = value;
                response.CacheServer = server;
                response.Database = db;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #endregion

        #region Hash

        #region HashValues

        /// <summary>
        /// Returns all values in the hash stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return hash values response</returns>
        public async Task<IEnumerable<HashValuesResponse>> HashValuesAsync(CacheServer server, HashValuesOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<HashValuesResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<HashValuesResponse> responses = new List<HashValuesResponse>(databases.Count);
            foreach (var db in databases)
            {
                List<dynamic> values = null;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var dict = entry.Value as ConcurrentDictionary<string, dynamic>;
                    if (dict == null)
                    {
                        responses.Add(CacheResponse.FailResponse<HashValuesResponse>(CacheCodes.ValueIsNotDict, server: server, database: db));
                        continue;
                    }
                    values = dict.Values.ToList();
                }
                values = values ?? new List<dynamic>(0);
                var response = CacheResponse.SuccessResponse<HashValuesResponse>(server, db);
                response.Values = values;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region HashSet

        /// <summary>
        /// Sets field in the hash stored at key to value. If key does not exist, a new key
        ///  holding a hash is created. If field already exists in the hash, it is overwritten.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return hash set response</returns>
        public async Task<IEnumerable<HashSetResponse>> HashSetAsync(CacheServer server, HashSetOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<HashSetResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<HashSetResponse> responses = new List<HashSetResponse>(databases.Count);
            foreach (var db in databases)
            {
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var dict = entry.Value as ConcurrentDictionary<string, dynamic>;
                    if (dict == null)
                    {
                        responses.Add(CacheResponse.FailResponse<HashSetResponse>(CacheCodes.ValueIsNotDict, server: server, database: db));
                        continue;
                    }
                    foreach (var item in options.Items)
                    {
                        dict[item.Key] = item.Value;
                    }
                }
                else
                {
                    using (entry = db.Store.CreateEntry(cacheKey))
                    {
                        var value = new ConcurrentDictionary<string, dynamic>(options.Items);
                        entry.SetValue(value);
                        SetExpiration(entry, options.Expiration);
                    }
                }
                var response = CacheResponse.SuccessResponse<HashSetResponse>(server, db);
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region HashLength

        /// <summary>
        /// Returns the number of fields contained in the hash stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return hash length response</returns>
        public async Task<IEnumerable<HashLengthResponse>> HashLengthAsync(CacheServer server, HashLengthOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<HashLengthResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<HashLengthResponse> responses = new List<HashLengthResponse>(databases.Count);
            foreach (var db in databases)
            {
                int length = 0;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var dict = entry.Value as ConcurrentDictionary<string, dynamic>;
                    if (dict == null)
                    {
                        responses.Add(CacheResponse.FailResponse<HashLengthResponse>(CacheCodes.ValueIsNotDict, server: server, database: db));
                    }
                    length = dict.Keys.Count;
                }
                var response = CacheResponse.SuccessResponse<HashLengthResponse>(server, db);
                response.Length = length;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region HashKeys

        /// <summary>
        /// Returns all field names in the hash stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return hash keys response</returns>
        public async Task<IEnumerable<HashKeysResponse>> HashKeysAsync(CacheServer server, HashKeysOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<HashKeysResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<HashKeysResponse> responses = new List<HashKeysResponse>(databases.Count);
            foreach (var db in databases)
            {
                List<string> keys = null;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var dict = entry.Value as ConcurrentDictionary<string, dynamic>;
                    if (dict == null)
                    {
                        responses.Add(CacheResponse.FailResponse<HashKeysResponse>(CacheCodes.ValueIsNotDict, server: server, database: db));
                        continue;
                    }
                    keys = dict.Keys.ToList();
                }
                keys = keys ?? new List<string>(0);
                var response = CacheResponse.SuccessResponse<HashKeysResponse>(server, db);
                response.HashKeys = keys;
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region HashIncrement

        /// <summary>
        /// Increments the number stored at field in the hash stored at key by increment.
        /// If key does not exist, a new key holding a hash is created. If field does not
        /// exist or holds a string that cannot be interpreted as integer, the value is set
        /// to 0 before the operation is performed.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return hash increment response</returns>
        public async Task<IEnumerable<HashIncrementResponse>> HashIncrementAsync(CacheServer server, HashIncrementOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<HashIncrementResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<HashIncrementResponse> responses = new List<HashIncrementResponse>(databases.Count);
            foreach (var db in databases)
            {
                var newValue = options.IncrementValue;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var dict = entry.Value as ConcurrentDictionary<string, dynamic>;
                    if (dict == null)
                    {
                        responses.Add(CacheResponse.FailResponse<HashIncrementResponse>(CacheCodes.ValueIsNotDict, server: server, database: db));
                        continue;
                    }
                    if (dict.TryGetValue(options.HashField, out var value))
                    {
                        dict[options.HashField] = newValue = value + options.IncrementValue;
                    }
                    else
                    {
                        dict[options.HashField] = options.IncrementValue;
                    }
                }
                else
                {
                    using (entry = db.Store.CreateEntry(cacheKey))
                    {
                        var value = new ConcurrentDictionary<string, dynamic>();
                        value[options.HashField] = options.IncrementValue;
                        entry.SetValue(value);
                        SetExpiration(entry, options.Expiration);
                    }
                }
                var response = CacheResponse.SuccessResponse<HashIncrementResponse>(server, db);
                response.HashField = options.HashField;
                response.NewValue = newValue;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region HashGet

        /// <summary>
        /// Returns the value associated with field in the hash stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return hash get response</returns>
        public async Task<IEnumerable<HashGetResponse>> HashGetAsync(CacheServer server, HashGetOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<HashGetResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<HashGetResponse> responses = new List<HashGetResponse>(databases.Count);
            foreach (var db in databases)
            {
                dynamic value = null;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var dict = entry.Value as ConcurrentDictionary<string, dynamic>;
                    if (dict == null)
                    {
                        responses.Add(CacheResponse.FailResponse<HashGetResponse>(CacheCodes.ValueIsNotDict, server: server, database: db));
                        continue;
                    }
                    dict.TryGetValue(options.HashField, out value);
                }
                var response = CacheResponse.SuccessResponse<HashGetResponse>(server, db);
                response.Value = value;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region HashGetAll

        /// <summary>
        /// Returns all fields and values of the hash stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return hash get all response</returns>
        public async Task<IEnumerable<HashGetAllResponse>> HashGetAllAsync(CacheServer server, HashGetAllOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<HashGetAllResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<HashGetAllResponse> responses = new List<HashGetAllResponse>(databases.Count);
            foreach (var db in databases)
            {
                ConcurrentDictionary<string, dynamic> values = null;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var dict = entry.Value as ConcurrentDictionary<string, dynamic>;
                    if (dict == null)
                    {
                        responses.Add(CacheResponse.FailResponse<HashGetAllResponse>(CacheCodes.ValueIsNotDict, server: server, database: db));
                        continue;
                    }
                    values = new ConcurrentDictionary<string, dynamic>(dict);
                }
                var response = CacheResponse.SuccessResponse<HashGetAllResponse>(server, db);
                response.HashValues = values?.ToDictionary(c => c.Key, c => c.Value) ?? new Dictionary<string, dynamic>(0);
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region HashExist

        /// <summary>
        /// Returns if field is an existing field in the hash stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return hash exists response</returns>
        public async Task<IEnumerable<HashExistsResponse>> HashExistAsync(CacheServer server, HashExistsOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<HashExistsResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<HashExistsResponse> responses = new List<HashExistsResponse>(databases.Count);
            foreach (var db in databases)
            {
                bool existKey = false;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var dict = entry.Value as ConcurrentDictionary<string, dynamic>;
                    if (dict == null)
                    {
                        responses.Add(CacheResponse.FailResponse<HashExistsResponse>(CacheCodes.ValueIsNotDict, server: server, database: db));
                        continue;
                    }
                    existKey = dict.ContainsKey(options.HashField);
                }
                var response = CacheResponse.SuccessResponse<HashExistsResponse>(server, db);
                response.HasField = existKey;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region HashDelete

        /// <summary>
        /// Removes the specified fields from the hash stored at key. Non-existing fields
        /// are ignored. Non-existing keys are treated as empty hashes and this options returns 0
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return hash delete response</returns>
        public async Task<IEnumerable<HashDeleteResponse>> HashDeleteAsync(CacheServer server, HashDeleteOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<HashDeleteResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<HashDeleteResponse> responses = new List<HashDeleteResponse>(databases.Count);
            foreach (var db in databases)
            {
                bool remove = false;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var dict = entry.Value as ConcurrentDictionary<string, dynamic>;
                    if (dict == null)
                    {
                        responses.Add(CacheResponse.FailResponse<HashDeleteResponse>(CacheCodes.ValueIsNotDict, server: server, database: db));
                        continue;
                    }
                    foreach (var field in options.HashFields)
                    {
                        remove |= dict.TryRemove(field, out var value);
                    }
                }
                responses.Add(new HashDeleteResponse()
                {
                    Success = remove,
                    CacheServer = server,
                    Database = db
                });
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region HashDecrement

        /// <summary>
        /// Decrement the specified field of an hash stored at key, and representing a floating
        ///  point number, by the specified decrement. If the field does not exist, it is
        ///  set to 0 before performing the operation.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return hash decrement response</returns>
        public async Task<IEnumerable<HashDecrementResponse>> HashDecrementAsync(CacheServer server, HashDecrementOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                WrapResponse(CacheResponse.FailResponse<HashDecrementResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<HashDecrementResponse> responses = new List<HashDecrementResponse>(databases.Count);
            foreach (var db in databases)
            {
                var newValue = options.DecrementValue;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var dict = entry.Value as ConcurrentDictionary<string, dynamic>;
                    if (dict == null)
                    {
                        responses.Add(CacheResponse.FailResponse<HashDecrementResponse>(CacheCodes.ValueIsNotDict, server: server, database: db));
                        continue;
                    }
                    if (dict.TryGetValue(options.HashField, out var value))
                    {
                        dict[options.HashField] = newValue = value - options.DecrementValue;
                    }
                    else
                    {
                        dict[options.HashField] = options.DecrementValue;
                    }
                    entry.SetValue(dict);
                }
                else
                {
                    using (entry = db.Store.CreateEntry(cacheKey))
                    {
                        var value = new ConcurrentDictionary<string, dynamic>();
                        value[options.HashField] = options.DecrementValue;
                        entry.SetValue(value);
                        SetExpiration(entry, options.Expiration);
                    }
                }
                var response = CacheResponse.SuccessResponse<HashDecrementResponse>(server, db);
                response.HashField = options.HashField;
                response.NewValue = newValue;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region HashScan

        /// <summary>
        /// The HSCAN options is used to incrementally iterate over a hash
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return hash scan response</returns>
        public async Task<IEnumerable<HashScanResponse>> HashScanAsync(CacheServer server, HashScanOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                WrapResponse(CacheResponse.FailResponse<HashScanResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<HashScanResponse> responses = new List<HashScanResponse>(databases.Count);
            foreach (var db in databases)
            {
                Dictionary<string, dynamic> values = new Dictionary<string, dynamic>();
                if (options.PageSize > 0 && db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var dict = entry.Value as ConcurrentDictionary<string, dynamic>;
                    if (dict == null)
                    {
                        responses.Add(CacheResponse.FailResponse<HashScanResponse>(CacheCodes.ValueIsNotDict, server: server, database: db));
                        continue;
                    }
                    var pageSize = options.PageSize;
                    foreach (var item in dict)
                    {
                        bool accordWith = false;
                        switch (options.PatternType)
                        {
                            case KeyMatchPattern.StartWith:
                                accordWith = item.Key.StartsWith(options.Pattern);
                                break;
                            case KeyMatchPattern.EndWith:
                                accordWith = item.Key.EndsWith(options.Pattern);
                                break;
                            case KeyMatchPattern.Include:
                                accordWith = item.Key.Contains(options.Pattern);
                                break;
                        }
                        if (accordWith)
                        {
                            values.Add(item.Key, item.Value);
                            if (values.Count >= pageSize)
                            {
                                break;
                            }
                        }
                    }
                }
                var response = CacheResponse.SuccessResponse<HashScanResponse>(server, db);
                response.HashValues = values;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #endregion

        #region Set

        #region SetRemove

        /// <summary>
        /// Remove the specified member from the set stored at key. Specified members that
        /// are not a member of this set are ignored.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return set remove response</returns>
        public async Task<IEnumerable<SetRemoveResponse>> SetRemoveAsync(CacheServer server, SetRemoveOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<SetRemoveResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<SetRemoveResponse> responses = new List<SetRemoveResponse>(databases.Count);
            foreach (var db in databases)
            {
                int removeCount = 0;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null && !options.RemoveMembers.IsNullOrEmpty())
                {
                    var dict = entry.Value as ConcurrentDictionary<string, byte>;
                    if (dict == null)
                    {
                        responses.Add(CacheResponse.FailResponse<SetRemoveResponse>(CacheCodes.ValueIsNotSet, server: server, database: db));
                        continue;
                    }
                    foreach (var member in options.RemoveMembers)
                    {
                        if (dict.TryRemove(member, out var value))
                        {
                            removeCount++;
                        }
                    }
                }
                var response = CacheResponse.SuccessResponse<SetRemoveResponse>(server, db);
                response.RemoveCount = removeCount;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region SetRandomMembers

        /// <summary>
        /// Return an array of count distinct elements if count is positive. If called with
        /// a negative count the behavior changes and the options is allowed to return the
        /// same element multiple times. In this case the numer of returned elements is the
        /// absolute value of the specified count.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return set random members response</returns>
        public async Task<IEnumerable<SetRandomMembersResponse>> SetRandomMembersAsync(CacheServer server, SetRandomMembersOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<SetRandomMembersResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<SetRandomMembersResponse> responses = new List<SetRandomMembersResponse>(databases.Count);
            foreach (var db in databases)
            {
                List<string> members = new List<string>();
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null && options.Count != 0)
                {
                    var dict = entry.Value as ConcurrentDictionary<string, byte>;
                    if (dict == null)
                    {
                        responses.Add(CacheResponse.FailResponse<SetRandomMembersResponse>(CacheCodes.ValueIsNotSet, server: server, database: db));
                        continue;
                    }
                    bool allowSame = options.Count < 0;
                    var count = Math.Abs(options.Count);
                    var keys = dict.Keys;
                    if (keys.Count <= count)
                    {
                        members.AddRange(keys);
                    }
                    else if (allowSame)
                    {

                        for (var c = 0; c < count; c++)
                        {
                            var ranIndex = RandomNumberHelper.GetRandomNumber(keys.Count - 1);
                            var ranMember = keys.ElementAt(ranIndex);
                            members.Add(ranMember);
                        }
                    }
                    else
                    {
                        ShuffleNet<string> shuffle = new ShuffleNet<string>(keys);
                        members.AddRange(shuffle.TakeNextValues(count));
                    }
                }
                var response = CacheResponse.SuccessResponse<SetRandomMembersResponse>(server, db);
                response.Members = members;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region SetRandomMember

        /// <summary>
        /// Return a random element from the set value stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return set random member</returns>
        public async Task<IEnumerable<SetRandomMemberResponse>> SetRandomMemberAsync(CacheServer server, SetRandomMemberOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<SetRandomMemberResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<SetRandomMemberResponse> responses = new List<SetRandomMemberResponse>(databases.Count);
            foreach (var db in databases)
            {
                string member = string.Empty;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var dict = entry.Value as ConcurrentDictionary<string, byte>;
                    if (dict == null)
                    {
                        responses.Add(CacheResponse.FailResponse<SetRandomMemberResponse>(CacheCodes.ValueIsNotSet, server: server, database: db));
                        continue;
                    }
                    var keys = dict.Keys;
                    if (!keys.IsNullOrEmpty())
                    {
                        var ranIndex = RandomNumberHelper.GetRandomNumber(keys.Count - 1);
                        member = keys.ElementAt(ranIndex);
                    }
                }
                var response = CacheResponse.SuccessResponse<SetRandomMemberResponse>(server, db);
                response.Member = member;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region SetPop

        /// <summary>
        /// Removes and returns a random element from the set value stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return set pop response</returns>
        public async Task<IEnumerable<SetPopResponse>> SetPopAsync(CacheServer server, SetPopOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<SetPopResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<SetPopResponse> responses = new List<SetPopResponse>(databases.Count);
            foreach (var db in databases)
            {
                string member = string.Empty;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var dict = entry.Value as ConcurrentDictionary<string, byte>;
                    if (dict == null)
                    {
                        responses.Add(CacheResponse.FailResponse<SetPopResponse>(CacheCodes.ValueIsNotSet, server: server, database: db));
                        continue;
                    }
                    var keys = dict.Keys;
                    if (!keys.IsNullOrEmpty())
                    {
                        var ranIndex = RandomNumberHelper.GetRandomNumber(keys.Count - 1);
                        member = keys.ElementAt(ranIndex);
                        dict.TryRemove(member, out var value);
                    }
                }
                var response = CacheResponse.SuccessResponse<SetPopResponse>(server, db);
                response.PopValue = member;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region SetMove

        /// <summary>
        /// Move member from the set at source to the set at destination. This operation
        /// is atomic. In every given moment the element will appear to be a member of source
        /// or destination for other clients. When the specified element already exists in
        /// the destination set, it is only removed from the source set.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return set move response</returns>
        public async Task<IEnumerable<SetMoveResponse>> SetMoveAsync(CacheServer server, SetMoveOptions options)
        {
            string cacheKey = options?.SourceKey?.GetActualKey();
            string desKey = options?.DestinationKey?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey) || string.IsNullOrWhiteSpace(desKey))
            {
                return WrapResponse(CacheResponse.FailResponse<SetMoveResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<SetMoveResponse> responses = new List<SetMoveResponse>(databases.Count);
            foreach (var db in databases)
            {
                bool isRemove = false;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var dict = entry.Value as ConcurrentDictionary<string, byte>;
                    if (dict == null)
                    {
                        responses.Add(CacheResponse.FailResponse<SetMoveResponse>(CacheCodes.ValueIsNotSet, server: server, database: db));
                        continue;
                    }
                    db.Store.TryGetEntry(desKey, out var desEntry);
                    ConcurrentDictionary<string, byte> desDict = null;
                    if (desEntry != null)
                    {
                        desDict = desEntry.Value as ConcurrentDictionary<string, byte>;
                        if (desDict == null)
                        {
                            responses.Add(CacheResponse.FailResponse<SetMoveResponse>(CacheCodes.ValueIsNotSet, server: server, database: db));
                            continue;
                        }
                    }
                    if (dict.TryRemove(options.MoveMember, out var value))
                    {
                        isRemove = true;
                        if (desDict != null)
                        {
                            desDict[options.MoveMember] = 0;
                        }
                        else
                        {
                            using (desEntry = db.Store.CreateEntry(desKey))
                            {
                                desDict = new ConcurrentDictionary<string, byte>();
                                desDict.TryAdd(options.MoveMember, 0);
                                desEntry.SetValue(desDict);
                                SetExpiration(desEntry, options.Expiration);
                            }
                        }
                    }
                }
                responses.Add(new SetMoveResponse()
                {
                    Success = isRemove,
                    CacheServer = server,
                    Database = db
                });
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region SetMembers

        /// <summary>
        /// Returns all the members of the set value stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return set members response</returns>
        public async Task<IEnumerable<SetMembersResponse>> SetMembersAsync(CacheServer server, SetMembersOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<SetMembersResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<SetMembersResponse> responses = new List<SetMembersResponse>(databases.Count);
            foreach (var db in databases)
            {
                List<string> members = null;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var dict = entry.Value as ConcurrentDictionary<string, byte>;
                    if (dict == null)
                    {
                        responses.Add(CacheResponse.FailResponse<SetMembersResponse>(CacheCodes.ValueIsNotSet, server: server, database: db));
                        continue;
                    }
                    members = new List<string>(dict.Count);
                    members.AddRange(dict.Keys);
                }
                members = members ?? new List<string>(0);
                var response = CacheResponse.SuccessResponse<SetMembersResponse>(server, db);
                response.Members = members;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region SetLength

        /// <summary>
        /// Returns the set cardinality (number of elements) of the set stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return set length response</returns>
        public async Task<IEnumerable<SetLengthResponse>> SetLengthAsync(CacheServer server, SetLengthOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<SetLengthResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<SetLengthResponse> responses = new List<SetLengthResponse>(databases.Count);
            foreach (var db in databases)
            {
                int length = 0;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var dict = entry.Value as ConcurrentDictionary<string, byte>;
                    if (dict == null)
                    {
                        responses.Add(CacheResponse.FailResponse<SetLengthResponse>(CacheCodes.ValueIsNotSet, server: server, database: db));
                        continue;
                    }
                    length = dict.Count;
                }
                var response = CacheResponse.SuccessResponse<SetLengthResponse>(server, db);
                response.Length = length;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region SetContains

        /// <summary>
        /// Returns if member is a member of the set stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return set contains response</returns>
        public async Task<IEnumerable<SetContainsResponse>> SetContainsAsync(CacheServer server, SetContainsOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<SetContainsResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<SetContainsResponse> responses = new List<SetContainsResponse>(databases.Count);
            foreach (var db in databases)
            {
                bool existMember = false;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var dict = entry.Value as ConcurrentDictionary<string, byte>;
                    if (dict == null)
                    {
                        responses.Add(CacheResponse.FailResponse<SetContainsResponse>(CacheCodes.ValueIsNotSet, server: server, database: db));
                        continue;
                    }
                    existMember = dict.ContainsKey(options.Member);
                }
                var response = CacheResponse.SuccessResponse<SetContainsResponse>(server, db);
                response.ContainsValue = existMember;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region SetCombine

        /// <summary>
        /// Returns the members of the set resulting from the specified operation against
        /// the given sets.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return set combine response</returns>
        public async Task<IEnumerable<SetCombineResponse>> SetCombineAsync(CacheServer server, SetCombineOptions options)
        {
            if (options?.Keys.IsNullOrEmpty() ?? true)
            {
                return WrapResponse(CacheResponse.FailResponse<SetCombineResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<SetCombineResponse> responses = new List<SetCombineResponse>(databases.Count);
            foreach (var db in databases)
            {
                List<string> members = null;
                foreach (var key in options.Keys)
                {
                    var cacheKey = key?.GetActualKey() ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(cacheKey))
                    {
                        continue;
                    }
                    if (db.Store.TryGetEntry(cacheKey, out var nowEntry) && nowEntry != null)
                    {
                        var nowDict = nowEntry.Value as ConcurrentDictionary<string, byte>;
                        if (nowDict == null)
                        {
                            responses.Add(CacheResponse.FailResponse<SetCombineResponse>(CacheCodes.ValueIsNotSet, server: server, database: db));
                            continue;
                        }
                        if (nowDict.IsNullOrEmpty())
                        {
                            continue;
                        }
                        if (members.IsNullOrEmpty())
                        {
                            members = new List<string>(nowDict.Count);
                            members.AddRange(nowDict.Keys);
                        }
                        else
                        {
                            switch (options.CombineOperation)
                            {
                                case CombineOperation.Union:
                                    members = members.Union(nowDict.Keys).ToList();
                                    break;
                                case CombineOperation.Intersect:
                                    members = members.Intersect(nowDict.Keys).ToList();
                                    break;
                                case CombineOperation.Difference:
                                    members = members.Except(nowDict.Keys).ToList();
                                    break;
                            }
                        }
                    }
                }
                members = members ?? new List<string>(0);
                var response = CacheResponse.SuccessResponse<SetCombineResponse>(server, db);
                response.CombineValues = members;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region SetCombineAndStore

        /// <summary>
        /// This options is equal to SetCombine, but instead of returning the resulting set,
        ///  it is stored in destination. If destination already exists, it is overwritten.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return set combine and store response</returns>
        public async Task<IEnumerable<SetCombineAndStoreResponse>> SetCombineAndStoreAsync(CacheServer server, SetCombineAndStoreOptions options)
        {
            var desCacheKey = options.DestinationKey?.GetActualKey() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(desCacheKey) || (options?.SourceKeys.IsNullOrEmpty() ?? true))
            {
                return WrapResponse(CacheResponse.FailResponse<SetCombineAndStoreResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<SetCombineAndStoreResponse> responses = new List<SetCombineAndStoreResponse>(databases.Count);
            foreach (var db in databases)
            {
                List<string> members = null;
                foreach (var key in options.SourceKeys)
                {
                    var cacheKey = key?.GetActualKey() ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(cacheKey))
                    {
                        continue;
                    }
                    if (db.Store.TryGetEntry(cacheKey, out var nowEntry) && nowEntry != null)
                    {
                        var nowDict = nowEntry.Value as ConcurrentDictionary<string, byte>;
                        if (nowDict == null)
                        {
                            responses.Add(CacheResponse.FailResponse<SetCombineAndStoreResponse>(CacheCodes.ValueIsNotSet, server: server, database: db));
                            continue;
                        }
                        if (nowDict.IsNullOrEmpty())
                        {
                            continue;
                        }
                        if (members.IsNullOrEmpty())
                        {
                            members = new List<string>(nowDict.Count);
                            members.AddRange(nowDict.Keys);
                        }
                        else
                        {
                            switch (options.CombineOperation)
                            {
                                case CombineOperation.Union:
                                    members = members.Union(nowDict.Keys).ToList();
                                    break;
                                case CombineOperation.Intersect:
                                    members = members.Intersect(nowDict.Keys).ToList();
                                    break;
                                case CombineOperation.Difference:
                                    members = members.Except(nowDict.Keys).ToList();
                                    break;
                            }
                        }
                    }
                }
                members = members ?? new List<string>(0);
                db.Store.TryGetEntry(desCacheKey, out var desEntry);
                if (desEntry != null)
                {
                    var desDict = desEntry.Value as ConcurrentDictionary<string, byte>;
                    if (desDict == null)
                    {
                        responses.Add(CacheResponse.FailResponse<SetCombineAndStoreResponse>(CacheCodes.ValueIsNotSet, server: server, database: db));
                        continue;
                    }
                    foreach (var mem in members)
                    {
                        desDict[mem] = 0;
                    }
                }
                else
                {
                    using (desEntry = db.Store.CreateEntry(desCacheKey))
                    {
                        ConcurrentDictionary<string, byte> desDict = new ConcurrentDictionary<string, byte>();
                        members.ForEach(m =>
                        {
                            desDict.TryAdd(m, 0);
                        });
                        desEntry.SetValue(desDict);
                        SetExpiration(desEntry, options.Expiration);
                    }
                }
                var response = CacheResponse.SuccessResponse<SetCombineAndStoreResponse>(server, db);
                response.Count = members.Count;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region SetAdd

        /// <summary>
        /// Add the specified member to the set stored at key. Specified members that are
        /// already a member of this set are ignored. If key does not exist, a new set is
        /// created before adding the specified members.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return set add response</returns>
        public async Task<IEnumerable<SetAddResponse>> SetAddAsync(CacheServer server, SetAddOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<SetAddResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<SetAddResponse> responses = new List<SetAddResponse>(databases.Count);
            foreach (var db in databases)
            {
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var dict = entry.Value as ConcurrentDictionary<string, byte>;
                    if (dict == null)
                    {
                        responses.Add(CacheResponse.FailResponse<SetAddResponse>(CacheCodes.ValueIsNotSet, server: server, database: db));
                        continue;
                    }
                    foreach (var member in options.Members)
                    {
                        dict[member] = 0;
                    }
                }
                else
                {
                    using (entry = db.Store.CreateEntry(cacheKey))
                    {
                        ConcurrentDictionary<string, byte> desDict = new ConcurrentDictionary<string, byte>();
                        foreach (var member in options.Members)
                        {
                            desDict[member] = 0;
                        }
                        entry.SetValue(desDict);
                        SetExpiration(entry, options.Expiration);
                    }
                }
                responses.Add(new SetAddResponse()
                {
                    Success = true,
                    CacheServer = server,
                    Database = db
                });
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #endregion

        #region Sorted set

        #region SortedSetScore

        /// <summary>
        /// Returns the score of member in the sorted set at key; If member does not exist
        /// in the sorted set, or key does not exist, nil is returned.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return sorted set score response</returns>
        public async Task<IEnumerable<SortedSetScoreResponse>> SortedSetScoreAsync(CacheServer server, SortedSetScoreOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<SortedSetScoreResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<SortedSetScoreResponse> responses = new List<SortedSetScoreResponse>(databases.Count);
            foreach (var db in databases)
            {
                double? score = null;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var dict = entry.Value as ConcurrentDictionary<string, double>;
                    if (dict == null)
                    {
                        responses.Add(CacheResponse.FailResponse<SortedSetScoreResponse>(CacheCodes.ValueIsNotSortedSet, server: server, database: db));
                        continue;
                    }
                    if (dict.TryGetValue(options.Member, out var memberScore))
                    {
                        score = memberScore;
                    }
                }
                var response = CacheResponse.SuccessResponse<SortedSetScoreResponse>(server, db);
                response.Score = score;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region SortedSetRemoveRangeByValue

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order
        /// to force lexicographical ordering, this options removes all elements in the sorted
        /// set stored at key between the lexicographical range specified by min and max.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return sorted set remove range by value response</returns>
        public async Task<IEnumerable<SortedSetRemoveRangeByValueResponse>> SortedSetRemoveRangeByValueAsync(CacheServer server, SortedSetRemoveRangeByValueOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<SortedSetRemoveRangeByValueResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<SortedSetRemoveRangeByValueResponse> responses = new List<SortedSetRemoveRangeByValueResponse>(databases.Count);
            foreach (var db in databases)
            {
                int removeCount = 0;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var dict = entry.Value as ConcurrentDictionary<string, double>;
                    if (dict == null)
                    {
                        responses.Add(CacheResponse.FailResponse<SortedSetRemoveRangeByValueResponse>(CacheCodes.ValueIsNotSortedSet, server: server, database: db));
                        continue;
                    }
                    var min = options.MinValue;
                    var max = options.MaxValue;
                    if (string.Compare(min, max) > 0)
                    {
                        min = max;
                        max = options.MinValue;
                    }
                    var removeValues = dict.Where(c =>
                    {
                        return string.Compare(c.Key, min) >= 0 && string.Compare(c.Key, max) <= 0;
                    });
                    foreach (var removeItem in removeValues)
                    {
                        switch (options.Exclude)
                        {
                            case BoundaryExclude.Both:
                                if (removeItem.Key == min || removeItem.Key == max)
                                    continue;
                                break;
                            case BoundaryExclude.Start:
                                if (removeItem.Key == min)
                                    continue;
                                break;
                            case BoundaryExclude.Stop:
                                if (removeItem.Key == max)
                                    continue;
                                break;
                        }
                        if (dict.TryRemove(removeItem.Key, out var value))
                        {
                            removeCount++;
                        }
                    }
                }
                var response = CacheResponse.SuccessResponse<SortedSetRemoveRangeByValueResponse>(server, db);
                response.RemoveCount = removeCount;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region SortedSetRemoveRangeByScore

        /// <summary>
        /// Removes all elements in the sorted set stored at key with a score between min
        ///  and max (inclusive by default).
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return sorted set remove range by score response</returns>
        public async Task<IEnumerable<SortedSetRemoveRangeByScoreResponse>> SortedSetRemoveRangeByScoreAsync(CacheServer server, SortedSetRemoveRangeByScoreOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<SortedSetRemoveRangeByScoreResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<SortedSetRemoveRangeByScoreResponse> responses = new List<SortedSetRemoveRangeByScoreResponse>(databases.Count);
            foreach (var db in databases)
            {
                int removeCount = 0;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var dict = entry.Value as ConcurrentDictionary<string, double>;
                    if (dict == null)
                    {
                        responses.Add(CacheResponse.FailResponse<SortedSetRemoveRangeByScoreResponse>(CacheCodes.ValueIsNotSortedSet, server: server, database: db));
                        continue;
                    }
                    var min = options.Start;
                    var max = options.Stop;
                    if (min > max)
                    {
                        min = max;
                        max = options.Start;
                    }
                    var removeValues = dict.Where(c => c.Value >= min && c.Value <= max);
                    foreach (var removeItem in removeValues)
                    {
                        switch (options.Exclude)
                        {
                            case BoundaryExclude.Both:
                                if (removeItem.Value == min || removeItem.Value == max)
                                    continue;
                                break;
                            case BoundaryExclude.Start:
                                if (removeItem.Value == min)
                                    continue;
                                break;
                            case BoundaryExclude.Stop:
                                if (removeItem.Value == max)
                                    continue;
                                break;
                        }
                        if (dict.TryRemove(removeItem.Key, out var value))
                        {
                            removeCount++;
                        }
                    }
                }
                var response = CacheResponse.SuccessResponse<SortedSetRemoveRangeByScoreResponse>(server, db);
                response.RemoveCount = removeCount;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region SortedSetRemoveRangeByRank

        /// <summary>
        /// Removes all elements in the sorted set stored at key with rank between start
        /// and stop. Both start and stop are 0 -based indexes with 0 being the element with
        /// the lowest score. These indexes can be negative numbers, where they indicate
        /// offsets starting at the element with the highest score. For example: -1 is the
        /// element with the highest score, -2 the element with the second highest score
        /// and so forth.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return sorted set remove range by rank response</returns>
        public async Task<IEnumerable<SortedSetRemoveRangeByRankResponse>> SortedSetRemoveRangeByRankAsync(CacheServer server, SortedSetRemoveRangeByRankOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<SortedSetRemoveRangeByRankResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<SortedSetRemoveRangeByRankResponse> responses = new List<SortedSetRemoveRangeByRankResponse>(databases.Count);
            foreach (var db in databases)
            {
                int removeCount = 0;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var dict = entry.Value as ConcurrentDictionary<string, double>;
                    if (dict == null)
                    {
                        responses.Add(CacheResponse.FailResponse<SortedSetRemoveRangeByRankResponse>(CacheCodes.ValueIsNotSortedSet, server: server, database: db));
                        continue;
                    }
                    var min = options.Start;
                    var max = options.Stop;
                    var dataCount = dict.Count;
                    if (min < 0)
                    {
                        min = dataCount - Math.Abs(min);
                    }
                    if (max < 0)
                    {
                        max = dataCount - Math.Abs(max);
                    }
                    if (min > max)
                    {
                        min = max;
                        max = options.Start;
                    }
                    if (min < dataCount && max < dataCount)
                    {
                        int skipCount = min;
                        int takeCount = max - min + 1;
                        var removeItems = dict.OrderBy(c => c.Value).Skip(skipCount).Take(takeCount);
                        foreach (var rmi in removeItems)
                        {
                            if (dict.TryRemove(rmi.Key, out var value))
                            {
                                removeCount++;
                            }
                        }
                    }
                }
                var response = CacheResponse.SuccessResponse<SortedSetRemoveRangeByRankResponse>(server, db);
                response.RemoveCount = removeCount;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region SortedSetRemove

        /// <summary>
        /// Removes the specified members from the sorted set stored at key. Non existing
        /// members are ignored.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return sorted set remove response</returns>
        public async Task<IEnumerable<SortedSetRemoveResponse>> SortedSetRemoveAsync(CacheServer server, SortedSetRemoveOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<SortedSetRemoveResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<SortedSetRemoveResponse> responses = new List<SortedSetRemoveResponse>(databases.Count);
            foreach (var db in databases)
            {
                int removeCount = 0;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null && !options.RemoveMembers.IsNullOrEmpty())
                {
                    var dict = entry.Value as ConcurrentDictionary<string, double>;
                    if (dict == null)
                    {
                        responses.Add(CacheResponse.FailResponse<SortedSetRemoveResponse>(CacheCodes.ValueIsNotSortedSet, server: server, database: db));
                        continue;
                    }
                    foreach (var rmem in options.RemoveMembers)
                    {
                        if (dict.TryRemove(rmem, out var value))
                        {
                            removeCount++;
                        }
                    }
                }
                var response = CacheResponse.SuccessResponse<SortedSetRemoveResponse>(server, db);
                response.RemoveCount = removeCount;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region SortedSetRank

        /// <summary>
        /// Returns the rank of member in the sorted set stored at key, by default with the
        /// scores ordered from low to high. The rank (or index) is 0-based, which means
        /// that the member with the lowest score has rank 0.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return sorted set rank response</returns>
        public async Task<IEnumerable<SortedSetRankResponse>> SortedSetRankAsync(CacheServer server, SortedSetRankOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<SortedSetRankResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<SortedSetRankResponse> responses = new List<SortedSetRankResponse>(databases.Count);
            foreach (var db in databases)
            {
                long? rank = null;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var dict = entry.Value as ConcurrentDictionary<string, double>;
                    if (dict == null)
                    {
                        responses.Add(CacheResponse.FailResponse<SortedSetRankResponse>(CacheCodes.ValueIsNotSortedSet, server: server, database: db));
                        continue;
                    }
                    if (!dict.IsNullOrEmpty() && dict.ContainsKey(options.Member))
                    {
                        rank = -1;
                        IOrderedEnumerable<KeyValuePair<string, double>> ranks = null;
                        if (options.Order == CacheOrder.Ascending)
                        {
                            ranks = dict.OrderBy(c => c.Value);
                        }
                        else
                        {
                            ranks = dict.OrderByDescending(c => c.Value);
                        }
                        foreach (var item in ranks)
                        {
                            rank++;
                            if (item.Key == options.Member)
                            {
                                break;
                            }
                        }
                    }
                }
                var response = CacheResponse.SuccessResponse<SortedSetRankResponse>(server, db);
                response.Rank = rank;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region SortedSetRangeByValue

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order
        /// to force lexicographical ordering, this options returns all the elements in the
        /// sorted set at key with a value between min and max.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return sorted set range by value response</returns>
        public async Task<IEnumerable<SortedSetRangeByValueResponse>> SortedSetRangeByValueAsync(CacheServer server, SortedSetRangeByValueOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<SortedSetRangeByValueResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<SortedSetRangeByValueResponse> responses = new List<SortedSetRangeByValueResponse>(databases.Count);
            foreach (var db in databases)
            {
                List<string> members = null;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var dict = entry.Value as ConcurrentDictionary<string, double>;
                    if (dict == null)
                    {
                        responses.Add(CacheResponse.FailResponse<SortedSetRangeByValueResponse>(CacheCodes.ValueIsNotSortedSet, server: server, database: db));
                        continue;
                    }
                    var min = options.MinValue;
                    var max = options.MaxValue;
                    if (string.Compare(min, max) > 0)
                    {
                        min = max;
                        max = options.MinValue;
                    }
                    var values = dict.Where(c =>
                    {
                        return options.Exclude switch
                        {
                            BoundaryExclude.Both => string.Compare(c.Key, min) > 0 && string.Compare(c.Key, max) < 0,
                            BoundaryExclude.Start => string.Compare(c.Key, min) > 0 && string.Compare(c.Key, max) <= 0,
                            BoundaryExclude.Stop => string.Compare(c.Key, min) >= 0 && string.Compare(c.Key, max) < 0,
                            _ => string.Compare(c.Key, min) >= 0 && string.Compare(c.Key, max) <= 0
                        };
                    });
                    if (options.Order == CacheOrder.Descending)
                    {
                        values = values.OrderByDescending(c => c.Key);
                    }
                    else
                    {
                        values = values.OrderBy(c => c.Key);
                    }
                    if (options.Offset > 0)
                    {
                        values = values.Skip(options.Offset);
                    }
                    if (options.Count >= 0)
                    {
                        values = values.Take(options.Count);
                    }
                    members = values.Select(c => c.Key).ToList();
                }
                var response = CacheResponse.SuccessResponse<SortedSetRangeByValueResponse>(server, db);
                response.Members = members ?? new List<string>(0);
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region SortedSetRangeByScoreWithScores

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key. By default
        /// the elements are considered to be ordered from the lowest to the highest score.
        /// Lexicographical order is used for elements with equal score. Start and stop are
        /// used to specify the min and max range for score values. Similar to other range
        /// methods the values are inclusive.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return sorted set range by score with scores response</returns>
        public async Task<IEnumerable<SortedSetRangeByScoreWithScoresResponse>> SortedSetRangeByScoreWithScoresAsync(CacheServer server, SortedSetRangeByScoreWithScoresOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<SortedSetRangeByScoreWithScoresResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<SortedSetRangeByScoreWithScoresResponse> responses = new List<SortedSetRangeByScoreWithScoresResponse>(databases.Count);
            foreach (var db in databases)
            {
                List<SortedSetMember> members = null;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var dict = entry.Value as ConcurrentDictionary<string, double>;
                    if (dict == null)
                    {
                        responses.Add(CacheResponse.FailResponse<SortedSetRangeByScoreWithScoresResponse>(CacheCodes.ValueIsNotSortedSet, server: server, database: db));
                        continue;
                    }
                    var min = options.Start;
                    var max = options.Stop;
                    if (min > max)
                    {
                        min = max;
                        max = options.Start;
                    }
                    var values = dict.Where(c =>
                    {
                        return options.Exclude switch
                        {
                            BoundaryExclude.Both => c.Value > min && c.Value < max,
                            BoundaryExclude.Start => c.Value > min && c.Value <= max,
                            BoundaryExclude.Stop => c.Value >= min && c.Value < max,
                            _ => c.Value >= min && c.Value <= max,
                        };
                    });
                    if (options.Order == CacheOrder.Descending)
                    {
                        values = values.OrderByDescending(c => c.Value);
                    }
                    else
                    {
                        values = values.OrderBy(c => c.Value);
                    }
                    if (options.Offset > 0)
                    {
                        values = values.Skip(options.Offset);
                    }
                    if (options.Count >= 0)
                    {
                        values = values.Take(options.Count);
                    }
                    members = values.Select(c => new SortedSetMember()
                    {
                        Value = c.Key,
                        Score = c.Value
                    }).ToList();
                }
                var response = CacheResponse.SuccessResponse<SortedSetRangeByScoreWithScoresResponse>(server, db);
                response.Members = members;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region SortedSetRangeByScore

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key. By default
        /// the elements are considered to be ordered from the lowest to the highest score.
        /// Lexicographical order is used for elements with equal score. Start and stop are
        /// used to specify the min and max range for score values. Similar to other range
        /// methods the values are inclusive.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return sorted set range by score response</returns>
        public async Task<IEnumerable<SortedSetRangeByScoreResponse>> SortedSetRangeByScoreAsync(CacheServer server, SortedSetRangeByScoreOptions options)
        {
            var setResponses = await SortedSetRangeByScoreWithScoresAsync(server, new SortedSetRangeByScoreWithScoresOptions()
            {
                CacheObject = options.CacheObject,
                CommandFlags = options.CommandFlags,
                Exclude = options.Exclude,
                Key = options.Key,
                Order = options.Order,
                Offset = options.Offset,
                Count = options.Count,
                Start = options.Start,
                Stop = options.Stop
            }).ConfigureAwait(false);
            if (setResponses.IsNullOrEmpty())
            {
                return Array.Empty<SortedSetRangeByScoreResponse>();
            }
            List<SortedSetRangeByScoreResponse> responses = new List<SortedSetRangeByScoreResponse>(setResponses.GetCount());
            foreach (var setRes in setResponses)
            {
                var response = CacheResponse.SuccessResponse<SortedSetRangeByScoreResponse>(server, setRes.Database);
                response.Members = setRes.Members?.Select(c => c.Value).ToList() ?? new List<string>(0);
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region SortedSetRangeByRankWithScores

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key. By default
        /// the elements are considered to be ordered from the lowest to the highest score.
        /// Lexicographical order is used for elements with equal score. Both start and stop
        /// are zero-based indexes, where 0 is the first element, 1 is the next element and
        /// so on. They can also be negative numbers indicating offsets from the end of the
        /// sorted set, with -1 being the last element of the sorted set, -2 the penultimate
        /// element and so on.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return sorted set range by rank with scores response</returns>
        public async Task<IEnumerable<SortedSetRangeByRankWithScoresResponse>> SortedSetRangeByRankWithScoresAsync(CacheServer server, SortedSetRangeByRankWithScoresOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<SortedSetRangeByRankWithScoresResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<SortedSetRangeByRankWithScoresResponse> responses = new List<SortedSetRangeByRankWithScoresResponse>(databases.Count);
            foreach (var db in databases)
            {
                List<SortedSetMember> members = null;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var dict = entry.Value as ConcurrentDictionary<string, double>;
                    if (dict == null)
                    {
                        responses.Add(CacheResponse.FailResponse<SortedSetRangeByRankWithScoresResponse>(CacheCodes.ValueIsNotSortedSet, server: server, database: db));
                        continue;
                    }
                    var min = options.Start;
                    var max = options.Stop;
                    var dataCount = dict.Count;
                    if (min < 0)
                    {
                        min = dataCount - Math.Abs(min);
                    }
                    if (max < 0)
                    {
                        max = dataCount - Math.Abs(max);
                    }
                    if (min > max)
                    {
                        min = max;
                        max = options.Start;
                    }
                    if (min < dataCount)
                    {
                        int skipCount = min;
                        int takeCount = max - min + 1;
                        IEnumerable<KeyValuePair<string, double>> valueDict = dict;
                        if (options.Order == CacheOrder.Descending)
                        {
                            valueDict = dict.OrderByDescending(c => c.Value);
                        }
                        else
                        {
                            valueDict = dict.OrderBy(c => c.Value);
                        }
                        var items = valueDict.Skip(skipCount).Take(takeCount < 0 ? int.MaxValue : takeCount);
                        members = items.Select(c => new SortedSetMember()
                        {
                            Score = c.Value,
                            Value = c.Key
                        }).ToList();
                    }
                }
                var response = CacheResponse.SuccessResponse<SortedSetRangeByRankWithScoresResponse>(server, db);
                response.Members = members ?? new List<SortedSetMember>(0);
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region SortedSetRangeByRank

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key. By default
        /// the elements are considered to be ordered from the lowest to the highest score.
        /// Lexicographical order is used for elements with equal score. Both start and stop
        /// are zero-based indexes, where 0 is the first element, 1 is the next element and
        /// so on. They can also be negative numbers indicating offsets from the end of the
        /// sorted set, with -1 being the last element of the sorted set, -2 the penultimate
        /// element and so on.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return sorted set range by rank response</returns>
        public async Task<IEnumerable<SortedSetRangeByRankResponse>> SortedSetRangeByRankAsync(CacheServer server, SortedSetRangeByRankOptions options)
        {
            var setResponses = await SortedSetRangeByRankWithScoresAsync(server, new SortedSetRangeByRankWithScoresOptions()
            {
                CacheObject = options.CacheObject,
                CommandFlags = options.CommandFlags,
                Key = options.Key,
                Order = options.Order,
                Start = options.Start,
                Stop = options.Stop
            }).ConfigureAwait(false);
            if (setResponses.IsNullOrEmpty())
            {
                return Array.Empty<SortedSetRangeByRankResponse>();
            }
            List<SortedSetRangeByRankResponse> responses = new List<SortedSetRangeByRankResponse>(setResponses.GetCount());
            foreach (var setRes in setResponses)
            {
                var response = CacheResponse.SuccessResponse<SortedSetRangeByRankResponse>(server, setRes.Database);
                response.Members = setRes.Members?.Select(c => c.Value).ToList() ?? new List<string>(0);
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region SortedSetLengthByValue

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order
        /// to force lexicographical ordering, this options returns the number of elements
        /// in the sorted set at key with a value between min and max.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">response</param>
        /// <returns>Return sorted set lenght by value response</returns>
        public async Task<IEnumerable<SortedSetLengthByValueResponse>> SortedSetLengthByValueAsync(CacheServer server, SortedSetLengthByValueOptions options)
        {
            var setResponses = await SortedSetRangeByValueAsync(server, new SortedSetRangeByValueOptions()
            {
                CacheObject = options.CacheObject,
                CommandFlags = options.CommandFlags,
                Key = options.Key,
                MinValue = options.MinValue,
                MaxValue = options.MaxValue,
                Offset = 0,
                Count = -1
            }).ConfigureAwait(false);
            if (setResponses.IsNullOrEmpty())
            {
                return Array.Empty<SortedSetLengthByValueResponse>();
            }
            List<SortedSetLengthByValueResponse> responses = new List<SortedSetLengthByValueResponse>(setResponses.GetCount());
            foreach (var setRes in setResponses)
            {
                var response = CacheResponse.SuccessResponse<SortedSetLengthByValueResponse>(server, setRes.Database);
                response.Length = setRes.Members?.Count ?? 0;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region SortedSetLength

        /// <summary>
        /// Returns the sorted set cardinality (number of elements) of the sorted set stored
        /// at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return sorted set length response</returns>
        public async Task<IEnumerable<SortedSetLengthResponse>> SortedSetLengthAsync(CacheServer server, SortedSetLengthOptions options)
        {
            var setResponses = await SortedSetRangeByScoreAsync(server, new SortedSetRangeByScoreOptions()
            {
                CacheObject = options.CacheObject,
                CommandFlags = options.CommandFlags,
                Key = options.Key,
                Offset = 0,
                Count = -1,
            }).ConfigureAwait(false);
            if (setResponses.IsNullOrEmpty())
            {
                return Array.Empty<SortedSetLengthResponse>();
            }
            List<SortedSetLengthResponse> responses = new List<SortedSetLengthResponse>(setResponses.GetCount());
            foreach (var setRes in setResponses)
            {
                var response = CacheResponse.SuccessResponse<SortedSetLengthResponse>(server, setRes.Database);
                response.Length = setRes.Members?.Count ?? 0;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region SortedSetIncrement

        /// <summary>
        /// Increments the score of member in the sorted set stored at key by increment.
        /// If member does not exist in the sorted set, it is added with increment as its
        /// score (as if its previous score was 0.0).
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return sorted set increment response</returns>
        public async Task<IEnumerable<SortedSetIncrementResponse>> SortedSetIncrementAsync(CacheServer server, SortedSetIncrementOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<SortedSetIncrementResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<SortedSetIncrementResponse> responses = new List<SortedSetIncrementResponse>(databases.Count);
            foreach (var db in databases)
            {
                double score = 0;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var dict = entry.Value as ConcurrentDictionary<string, double>;
                    if (dict == null)
                    {
                        responses.Add(CacheResponse.FailResponse<SortedSetIncrementResponse>(CacheCodes.ValueIsNotSortedSet, server: server, database: db));
                        continue;
                    }
                    if (dict.TryGetValue(options.Member, out var memberScore))
                    {
                        score = memberScore + options.IncrementScore;
                        dict[options.Member] = score;
                    }
                }
                var response = CacheResponse.SuccessResponse<SortedSetIncrementResponse>(server, db);
                response.NewScore = score;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region SortedSetDecrement

        /// <summary>
        /// Decrements the score of member in the sorted set stored at key by decrement.
        /// If member does not exist in the sorted set, it is added with -decrement as its
        /// score (as if its previous score was 0.0).
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return sorted set decrement response</returns>
        public async Task<IEnumerable<SortedSetDecrementResponse>> SortedSetDecrementAsync(CacheServer server, SortedSetDecrementOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<SortedSetDecrementResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<SortedSetDecrementResponse> responses = new List<SortedSetDecrementResponse>(databases.Count);
            foreach (var db in databases)
            {
                double score = 0;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var dict = entry.Value as ConcurrentDictionary<string, double>;
                    if (dict == null)
                    {
                        responses.Add(CacheResponse.FailResponse<SortedSetDecrementResponse>(CacheCodes.ValueIsNotSortedSet, server: server, database: db));
                        continue;
                    }
                    if (dict.TryGetValue(options.Member, out var memberScore))
                    {
                        score = memberScore - options.DecrementScore;
                        dict[options.Member] = score;
                    }
                }
                var response = CacheResponse.SuccessResponse<SortedSetDecrementResponse>(server, db);
                response.NewScore = score;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region SortedSetCombineAndStore

        /// <summary>
        /// Computes a set operation over multiple sorted sets (optionally using per-set
        /// weights), and stores the result in destination, optionally performing a specific
        /// aggregation (defaults to sum)
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return sorted set combine and store response</returns>
        public async Task<IEnumerable<SortedSetCombineAndStoreResponse>> SortedSetCombineAndStoreAsync(CacheServer server, SortedSetCombineAndStoreOptions options)
        {
            var desCacheKey = options.DestinationKey?.GetActualKey() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(desCacheKey) || (options?.SourceKeys.IsNullOrEmpty() ?? true))
            {
                return WrapResponse(CacheResponse.FailResponse<SortedSetCombineAndStoreResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<SortedSetCombineAndStoreResponse> responses = new List<SortedSetCombineAndStoreResponse>(databases.Count);
            foreach (var db in databases)
            {
                HashSet<string> members = null;
                Dictionary<string, List<double>> allMembers = new Dictionary<string, List<double>>();
                for (int i = 0; i < options.SourceKeys.Count; i++)
                {
                    var key = options.SourceKeys[i];
                    var cacheKey = key?.GetActualKey() ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(cacheKey))
                    {
                        continue;
                    }
                    if (db.Store.TryGetEntry(cacheKey, out var nowEntry) && nowEntry != null)
                    {
                        var nowDict = nowEntry.Value as ConcurrentDictionary<string, double>;
                        if (nowDict == null)
                        {
                            responses.Add(CacheResponse.FailResponse<SortedSetCombineAndStoreResponse>(CacheCodes.ValueIsNotSet, server: server, database: db));
                            continue;
                        }
                        if (nowDict.IsNullOrEmpty())
                        {
                            continue;
                        }
                        if (members.IsNullOrEmpty())
                        {
                            members = new HashSet<string>(nowDict.Keys);
                        }
                        else
                        {
                            switch (options.CombineOperation)
                            {
                                case CombineOperation.Union:
                                    members.UnionWith(nowDict.Keys);
                                    break;
                                case CombineOperation.Intersect:
                                    members.IntersectWith(nowDict.Keys);
                                    break;
                                case CombineOperation.Difference:
                                    members.ExceptWith(nowDict.Keys);
                                    break;
                            }
                        }
                        double weight = 1;
                        if (options?.Weights?.Length >= i + 1)
                        {
                            weight = options.Weights[i];
                        }
                        foreach (var item in nowDict)
                        {
                            if (allMembers.TryGetValue(item.Key, out var scores) && !scores.IsNullOrEmpty())
                            {
                                scores.Add(item.Value * weight);
                            }
                            else
                            {
                                allMembers[item.Key] = new List<double>() { item.Value * weight };
                            }
                        }
                    }
                }
                Dictionary<string, double> resultItems = new Dictionary<string, double>();
                foreach (var member in members)
                {
                    double memberScore = 0;
                    if (allMembers.TryGetValue(member, out var scores) && !scores.IsNullOrEmpty())
                    {
                        memberScore = options.Aggregate switch
                        {
                            SetAggregate.Max => scores.Max(),
                            SetAggregate.Min => scores.Min(),
                            SetAggregate.Sum => scores.Sum(),
                            _ => 0
                        };
                    }
                    resultItems.Add(member, memberScore);
                }
                if (db.Store.TryGetEntry(desCacheKey, out var desEntry) && desEntry != null)
                {
                    var desDict = desEntry.Value as ConcurrentDictionary<string, double>;
                    if (desDict == null)
                    {
                        responses.Add(CacheResponse.FailResponse<SortedSetCombineAndStoreResponse>(CacheCodes.ValueIsNotSet, server: server, database: db));
                        continue;
                    }
                    desEntry.Value = resultItems;
                }
                else
                {
                    using (desEntry = db.Store.CreateEntry(desCacheKey))
                    {
                        desEntry.SetValue(resultItems);
                        SetExpiration(desEntry, options.Expiration);
                    }
                }
                var response = CacheResponse.SuccessResponse<SortedSetCombineAndStoreResponse>(server, db);
                response.NewSetLength = resultItems.Count;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region SortedSetAdd

        /// <summary>
        /// Adds all the specified members with the specified scores to the sorted set stored
        /// at key. If a specified member is already a member of the sorted set, the score
        /// is updated and the element reinserted at the right position to ensure the correct
        /// ordering.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return sorted set add response</returns>
        public async Task<IEnumerable<SortedSetAddResponse>> SortedSetAddAsync(CacheServer server, SortedSetAddOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<SortedSetAddResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            if (options.Members.IsNullOrEmpty())
            {
                return WrapResponse(CacheResponse.FailResponse<SortedSetAddResponse>(CacheCodes.ValuesIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<SortedSetAddResponse> responses = new List<SortedSetAddResponse>(databases.Count);
            foreach (var db in databases)
            {
                long length = 0;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var dict = entry.Value as ConcurrentDictionary<string, double>;
                    if (dict == null)
                    {
                        responses.Add(CacheResponse.FailResponse<SortedSetAddResponse>(CacheCodes.ValueIsNotSortedSet, server: server, database: db));
                        continue;
                    }
                    foreach (var mem in options.Members)
                    {
                        dict[mem.Value] = mem.Score;
                    }
                    length = dict.Count;
                }
                else
                {
                    using (entry = db.Store.CreateEntry(cacheKey))
                    {
                        ConcurrentDictionary<string, double> newDict = new ConcurrentDictionary<string, double>();
                        options.Members.ForEach(c =>
                        {
                            newDict.TryAdd(c.Value, c.Score);
                        });
                        length = newDict.Count;
                        entry.SetValue(newDict);
                        SetExpiration(entry, options.Expiration);
                    }
                }
                var response = CacheResponse.SuccessResponse<SortedSetAddResponse>(server, db);
                response.Length = length;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #endregion

        #region Sort

        #region Sort

        /// <summary>
        /// Sorts a list, set or sorted set (numerically or alphabetically, ascending by
        /// default){await Task.Delay(100);return null;} By default, the elements themselves are compared, but the values can
        /// also be used to perform external key-lookups using the by parameter. By default,
        /// the elements themselves are returned, but external key-lookups (one or many)
        /// can be performed instead by specifying the get parameter (note that # specifies
        /// the element itself, when used in get). Referring to the redis SORT documentation
        /// for examples is recommended. When used in hashes, by and get can be used to specify
        /// fields using -> notation (again, refer to redis documentation).
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return sort response</returns>
        public async Task<IEnumerable<SortResponse>> SortAsync(CacheServer server, SortOptions options)
        {
            var keyTypeResponses = await KeyTypeAsync(server, new TypeOptions()
            {
                CacheObject = options.CacheObject,
                CommandFlags = options.CommandFlags,
                Key = options.Key
            }).ConfigureAwait(false);
            List<SortResponse> responses = null;
            if (!keyTypeResponses.IsNullOrEmpty())
            {
                responses = new List<SortResponse>(keyTypeResponses.GetCount());
                Func<IEnumerable<string>, IEnumerable<string>> filterValueFuc = (originalValues) =>
                {
                    if (originalValues.IsNullOrEmpty() || originalValues.Count() <= options.Offset)
                    {
                        return Array.Empty<string>();
                    }
                    if (options.Order == CacheOrder.Descending)
                    {
                        originalValues = originalValues.OrderByDescending(c => c);
                    }
                    else
                    {
                        originalValues = originalValues.OrderBy(c => c);
                    }
                    if (options.Offset > 0)
                    {
                        originalValues = originalValues.Skip(options.Offset);
                    }
                    if (options.Count > 0)
                    {
                        originalValues = originalValues.Take(options.Count);
                    }
                    return originalValues;
                };

                foreach (var keyTypeRes in keyTypeResponses)
                {
                    IEnumerable<string> values = null;
                    bool support = true;
                    switch (keyTypeRes.KeyType)
                    {
                        case CacheKeyType.List:
                            var listResponses = await ListRangeAsync(server, new ListRangeOptions()
                            {
                                CacheObject = options.CacheObject,
                                CommandFlags = options.CommandFlags,
                                Key = options.Key,
                                Start = 0,
                                Stop = -1
                            }).ConfigureAwait(false);
                            foreach (var listRes in listResponses)
                            {
                                values = filterValueFuc(listRes.Values);
                                responses.Add(new SortResponse()
                                {
                                    Success = true,
                                    Values = values?.ToList() ?? new List<string>(0),
                                    CacheServer = server,
                                    Database = listRes.Database
                                });
                            }
                            break;
                        case CacheKeyType.Set:
                            var setResponses = await SetMembersAsync(server, new SetMembersOptions()
                            {
                                CacheObject = options.CacheObject,
                                CommandFlags = options.CommandFlags,
                                Key = options.Key
                            }).ConfigureAwait(false);
                            foreach (var setRes in setResponses)
                            {
                                values = filterValueFuc(setRes.Members);
                                responses.Add(new SortResponse()
                                {
                                    Success = true,
                                    Values = values?.ToList() ?? new List<string>(0),
                                    CacheServer = server,
                                    Database = setRes.Database
                                });
                            }
                            break;
                        case CacheKeyType.SortedSet:
                            var sortedSetResponses = await SortedSetRangeByRankWithScoresAsync(server, new SortedSetRangeByRankWithScoresOptions()
                            {
                                CacheObject = options.CacheObject,
                                CommandFlags = options.CommandFlags,
                                Key = options.Key,
                                Order = options.Order,
                                Start = 0,
                                Stop = -1
                            }).ConfigureAwait(false);
                            foreach (var sortedSetRes in sortedSetResponses)
                            {
                                IEnumerable<SortedSetMember> sortedSetMembers = sortedSetRes.Members ?? new List<SortedSetMember>(0);
                                if (sortedSetMembers.GetCount() <= options.Offset)
                                {
                                    values = Array.Empty<string>();
                                }
                                else
                                {
                                    if (options.Offset > 0)
                                    {
                                        sortedSetMembers = sortedSetMembers.Skip(options.Offset);
                                    }
                                    if (options.Count > 0)
                                    {
                                        sortedSetMembers = sortedSetMembers.Take(options.Count);
                                    }
                                    values = sortedSetMembers.Select(c => c.Value).ToList();
                                }
                                responses.Add(new SortResponse()
                                {
                                    Success = true,
                                    Values = values?.ToList() ?? new List<string>(0),
                                    CacheServer = server,
                                    Database = sortedSetRes.Database
                                });
                            }
                            break;
                        default:
                            support = false;
                            break;
                    }
                    if (!support)
                    {
                        responses.Add(CacheResponse.FailResponse<SortResponse>(CacheCodes.OperationIsNotSupported, server: server, database: keyTypeRes.Database));
                    }
                }
            }
            return await Task.FromResult(responses ?? new List<SortResponse>(0)).ConfigureAwait(false);
        }

        #endregion

        #region SortAndStore

        /// <summary>
        /// Sorts a list, set or sorted set (numerically or alphabetically, ascending by
        /// default){await Task.Delay(100);return null;} By default, the elements themselves are compared, but the values can
        /// also be used to perform external key-lookups using the by parameter. By default,
        /// the elements themselves are returned, but external key-lookups (one or many)
        /// can be performed instead by specifying the get parameter (note that # specifies
        /// the element itself, when used in get). Referring to the redis SORT documentation
        /// for examples is recommended. When used in hashes, by and get can be used to specify
        /// fields using -> notation (again, refer to redis documentation).
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return sort and store response</returns>
        public async Task<IEnumerable<SortAndStoreResponse>> SortAndStoreAsync(CacheServer server, SortAndStoreOptions options)
        {
            if (string.IsNullOrWhiteSpace(options?.SourceKey))
            {
                throw new ArgumentNullException($"{nameof(SortAndStoreOptions)}.{nameof(SortAndStoreOptions.SourceKey)}");
            }
            if (string.IsNullOrWhiteSpace(options?.DestinationKey))
            {
                throw new ArgumentNullException($"{nameof(SortAndStoreOptions)}.{nameof(SortAndStoreOptions.DestinationKey)}");
            }
            var sortResponses = await SortAsync(server, new SortOptions()
            {
                CacheObject = options.CacheObject,
                CommandFlags = options.CommandFlags,
                SortType = options.SortType,
                Count = options.Count,
                By = options.By,
                Gets = options.Gets,
                Key = options.SourceKey,
                Offset = options.Offset,
                Order = options.Order
            }).ConfigureAwait(false);
            List<SortAndStoreResponse> responses = null;
            if (!sortResponses.IsNullOrEmpty())
            {
                responses = new List<SortAndStoreResponse>(sortResponses.GetCount());
                foreach (var sortRes in sortResponses)
                {
                    var values = sortRes.Values;
                    await ListLeftPushAsync(server, new ListLeftPushOptions()
                    {
                        CacheObject = options.CacheObject,
                        CommandFlags = options.CommandFlags,
                        Expiration = options.Expiration,
                        Key = options.DestinationKey,
                        Values = values
                    }).ConfigureAwait(false);
                    responses.Add(CacheResponse.SuccessResponse<SortAndStoreResponse>(server, sortRes.Database));
                }
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #endregion

        #region Key

        #region KeyType

        /// <summary>
        /// Returns the string representation of the type of the value stored at key. The
        /// different types that can be returned are: string, list, set, zset and hash.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return key type response</returns>
        public async Task<IEnumerable<TypeResponse>> KeyTypeAsync(CacheServer server, TypeOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<TypeResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<TypeResponse> responses = new List<TypeResponse>(databases.Count);
            foreach (var db in databases)
            {
                TypeResponse response = null;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    CacheKeyType cacheKeyType = CacheKeyType.String;
                    if (entry.Value is List<string>)
                    {
                        cacheKeyType = CacheKeyType.List;
                    }
                    else if (entry.Value is ConcurrentDictionary<string, dynamic>)
                    {
                        cacheKeyType = CacheKeyType.Hash;
                    }
                    else if (entry.Value is ConcurrentDictionary<string, byte>)
                    {
                        cacheKeyType = CacheKeyType.Set;
                    }
                    else if (entry.Value is ConcurrentDictionary<string, double>)
                    {
                        cacheKeyType = CacheKeyType.SortedSet;
                    }
                    response = CacheResponse.SuccessResponse<TypeResponse>(server, db);
                    response.KeyType = cacheKeyType;
                }
                else
                {
                    response = CacheResponse.FailResponse<TypeResponse>(CacheCodes.KeyIsNotExist, server: server, database: db);
                }
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region KeyTimeToLive

        /// <summary>
        /// Returns the remaining time to live of a key that has a timeout. This introspection
        /// capability allows a Redis client to check how many seconds a given key will continue
        /// to be part of the dataset.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return key time to live response</returns>
        public async Task<IEnumerable<TimeToLiveResponse>> KeyTimeToLiveAsync(CacheServer server, TimeToLiveOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<TimeToLiveResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<TimeToLiveResponse> responses = new List<TimeToLiveResponse>(databases.Count);
            foreach (var db in databases)
            {
                TimeToLiveResponse response = null;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    response = CacheResponse.SuccessResponse<TimeToLiveResponse>();
                    var expiration = GetExpiration(entry);
                    response.TimeToLiveSeconds = (long)(expiration.Item2?.TotalSeconds ?? 0);
                }
                else
                {
                    response = CacheResponse.FailResponse<TimeToLiveResponse>(CacheCodes.KeyIsNotExist);
                }
                response.CacheServer = server;
                response.Database = db;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region KeyRestore

        /// <summary>
        /// Create a key associated with a value that is obtained by deserializing the provided
        /// serialized value (obtained via DUMP). If ttl is 0 the key is created without
        /// any expire, otherwise the specified expire time(in milliseconds) is set.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return key restore response</returns>
        public async Task<IEnumerable<RestoreResponse>> KeyRestoreAsync(CacheServer server, RestoreOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<RestoreResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<RestoreResponse> responses = new List<RestoreResponse>(databases.Count);
            foreach (var db in databases)
            {
                using (var entry = db.Store.CreateEntry(cacheKey))
                {
                    entry.SetValue(GetEncoding().GetString(options.Value));
                    SetExpiration(entry, options.Expiration);
                }
                responses.Add(CacheResponse.SuccessResponse<RestoreResponse>(server, db));
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region KeyRename

        /// <summary>
        /// Renames key to newkey. It returns an error when the source and destination names
        /// are the same, or when key does not exist.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return key rename response</returns>
        public async Task<IEnumerable<RenameResponse>> KeyRenameAsync(CacheServer server, RenameOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            string newCacheKey = options?.NewKey?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey) || string.IsNullOrWhiteSpace(newCacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<RenameResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<RenameResponse> responses = new List<RenameResponse>(databases.Count);
            foreach (var db in databases)
            {
                RenameResponse response = null;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    using (var newEntry = db.Store.CreateEntry(newCacheKey))
                    {
                        newEntry.SetValue(entry.Value);
                        newEntry.AbsoluteExpiration = entry.AbsoluteExpiration;
                        newEntry.AbsoluteExpirationRelativeToNow = entry.AbsoluteExpirationRelativeToNow;
                        newEntry.Priority = entry.Priority;
                        newEntry.Size = entry.Size;
                        newEntry.SlidingExpiration = entry.SlidingExpiration;
                    }
                    db.Store.Remove(cacheKey);
                    response = CacheResponse.SuccessResponse<RenameResponse>();
                }
                else
                {
                    response = CacheResponse.FailResponse<RenameResponse>(CacheCodes.KeyIsNotExist);
                }
                response.CacheServer = server;
                response.Database = db;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region KeyRandom

        /// <summary>
        /// Return a random key from the currently selected database.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return key random response</returns>
        public async Task<IEnumerable<RandomResponse>> KeyRandomAsync(CacheServer server, RandomOptions options)
        {
            var databases = GetDatabases(server);
            List<RandomResponse> responses = new List<RandomResponse>(databases.Count);
            foreach (var db in databases)
            {
                var response = CacheResponse.SuccessResponse<RandomResponse>(server, db);
                response.Key = db.Store.GetRandomKey();
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region KeyPersist

        /// <summary>
        /// Remove the existing timeout on key, turning the key from volatile (a key with
        /// an expire set) to persistent (a key that will never expire as no timeout is associated).
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return key persist response</returns>
        public async Task<IEnumerable<PersistResponse>> KeyPersistAsync(CacheServer server, PersistOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<PersistResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<PersistResponse> responses = new List<PersistResponse>(databases.Count);
            foreach (var db in databases)
            {
                PersistResponse response = null;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    using (var newEntry = db.Store.CreateEntry(cacheKey))
                    {
                        newEntry.SetValue(entry.Value);
                    }
                    response = CacheResponse.SuccessResponse<PersistResponse>();
                }
                else
                {
                    response = CacheResponse.FailResponse<PersistResponse>(CacheCodes.KeyIsNotExist);
                }
                response.CacheServer = server;
                response.Database = db;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region KeyMove

        /// <summary>
        /// Move key from the currently selected database (see SELECT) to the specified destination
        /// database. When key already exists in the destination database, or it does not
        /// exist in the source database, it does nothing. It is possible to use MOVE as
        /// a locking primitive because of this.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return key move response</returns>
        public async Task<IEnumerable<MoveResponse>> KeyMoveAsync(CacheServer server, MoveOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<MoveResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            MemoryCacheDatabase desDatabase = GetDatabase(options.DatabaseName);
            if (desDatabase == null)
            {
                return WrapResponse(CacheResponse.FailResponse<MoveResponse>("", "Destination database not find", server: server));
            }
            var databases = GetDatabases(server);
            List<MoveResponse> responses = new List<MoveResponse>(databases.Count);
            foreach (var db in databases)
            {
                if (db.Store.TryGetEntry(cacheKey, out var entry))
                {
                    using (var desEntry = desDatabase.Store.CreateEntry(cacheKey))
                    {
                        desEntry.Value = entry.Value;
                        SetExpiration(desEntry, GetCacheExpiration(entry));
                    }
                    responses.Add(CacheResponse.SuccessResponse<MoveResponse>(server, db));
                }
                else
                {
                    responses.Add(CacheResponse.FailResponse<MoveResponse>("", $"Not find key:{cacheKey}", server, db));
                }
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region KeyMigrate

        /// <summary>
        /// Atomically transfer a key from a source Redis instance to a destination Redis
        /// instance. On success the key is deleted from the original instance by default,
        /// and is guaranteed to exist in the target instance.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return key migrate response</returns>
        public async Task<IEnumerable<MigrateResponse>> KeyMigrateAsync(CacheServer server, MigrateOptions options)
        {
            return WrapResponse(await Task.FromResult(CacheResponse.FailResponse<MigrateResponse>(CacheCodes.OperationIsNotSupported, server: server)).ConfigureAwait(false));
        }

        #endregion

        #region KeyExpire

        /// <summary>
        /// Set a timeout on key. After the timeout has expired, the key will automatically
        /// be deleted. A key with an associated timeout is said to be volatile in Redis
        /// terminology.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return key expire response</returns>
        public async Task<IEnumerable<ExpireResponse>> KeyExpireAsync(CacheServer server, ExpireOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<ExpireResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<ExpireResponse> responses = new List<ExpireResponse>(databases.Count);
            foreach (var db in databases)
            {
                ExpireResponse response = null;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    SetExpiration(entry, options.Expiration);
                    response = CacheResponse.SuccessResponse<ExpireResponse>();
                }
                else
                {
                    response = CacheResponse.FailResponse<ExpireResponse>(CacheCodes.KeyIsNotExist);
                }
                response.CacheServer = server;
                response.Database = db;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion;

        #region KeyDump

        /// <summary>
        /// Serialize the value stored at key in a format and return it to
        /// the user. The returned value can be synthesized back into a Redis key using the
        /// RESTORE options.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return key dump response</returns>
        public async Task<IEnumerable<DumpResponse>> KeyDumpAsync(CacheServer server, DumpOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<DumpResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<DumpResponse> responses = new List<DumpResponse>(databases.Count);
            foreach (var db in databases)
            {
                DumpResponse response = null;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    response = CacheResponse.SuccessResponse<DumpResponse>();
                    response.ByteValues = GetEncoding().GetBytes(entry.Value?.ToString() ?? string.Empty);
                }
                else
                {
                    response = CacheResponse.FailResponse<DumpResponse>(CacheCodes.KeyIsNotExist);
                }
                response.CacheServer = server;
                response.Database = db;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region KeyDelete

        /// <summary>
        /// Removes the specified keys. A key is ignored if it does not exist.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return key delete response</returns>
        public async Task<IEnumerable<DeleteResponse>> KeyDeleteAsync(CacheServer server, DeleteOptions options)
        {
            if (options.Keys?.IsNullOrEmpty() ?? true)
            {
                return WrapResponse(CacheResponse.FailResponse<DeleteResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<DeleteResponse> responses = new List<DeleteResponse>(databases.Count);
            foreach (var db in databases)
            {
                long deleteCount = 0;
                foreach (var key in options.Keys)
                {
                    var cacheKey = key?.GetActualKey() ?? string.Empty;
                    if (db.Store.TryGetEntry(cacheKey, out var entry))
                    {
                        deleteCount++;
                        db.Store.Remove(cacheKey);
                    }
                }
                var response = CacheResponse.SuccessResponse<DeleteResponse>(server, db);
                response.DeleteCount = deleteCount;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region KeyExists

        /// <summary>
        /// Key exists
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return exists response</returns>
        public async Task<IEnumerable<ExistResponse>> KeyExistAsync(CacheServer server, ExistOptions options)
        {
            if (options.Keys?.IsNullOrEmpty() ?? true)
            {
                return WrapResponse(CacheResponse.FailResponse<ExistResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<ExistResponse> responses = new List<ExistResponse>(databases.Count);
            foreach (var db in databases)
            {
                long count = 0;
                foreach (var key in options.Keys)
                {
                    var cacheKey = key?.GetActualKey() ?? string.Empty;
                    if (db.Store.TryGetEntry(key?.GetActualKey(), out var entry))
                    {
                        count++;
                    }
                }
                var response = CacheResponse.SuccessResponse<ExistResponse>(server, db);
                response.KeyCount = count;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #endregion

        #region Server

        #region Get all data base

        /// <summary>
        /// Get all database
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return get all database response</returns>
        public async Task<IEnumerable<GetAllDataBaseResponse>> GetAllDataBaseAsync(CacheServer server, GetAllDataBaseOptions options)
        {
            var response = CacheResponse.SuccessResponse<GetAllDataBaseResponse>(server);
            response.Databases = GetDatabases(server).Select(c => { CacheDatabase database = c; return database; }).ToList();
            return WrapResponse(await Task.FromResult(response).ConfigureAwait(false));
        }

        #endregion

        #region Query keys

        /// <summary>
        /// Query keys
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return get keys response</returns>
        public async Task<IEnumerable<GetKeysResponse>> GetKeysAsync(CacheServer server, GetKeysOptions options)
        {
            var databases = GetDatabases(server);
            List<GetKeysResponse> responses = new List<GetKeysResponse>(databases.Count);
            foreach (var db in databases)
            {
                var allKeys = db.Store.GetAllKeys();
                Func<string, bool> where = c => true;
                int skip = 0;
                int count = allKeys.Count;
                if (options.Query != null)
                {
                    skip = (options.Query.Page - 1) * options.Query.PageSize;
                    count = options.Query.PageSize;
                    switch (options.Query.Type)
                    {
                        case KeyMatchPattern.EndWith:
                            where = c => c.EndsWith(options.Query.MateKey);
                            break;
                        case KeyMatchPattern.StartWith:
                            where = c => c.StartsWith(options.Query.MateKey);
                            break;
                        case KeyMatchPattern.Include:
                            where = c => c.Contains(options.Query.MateKey);
                            break;
                    }
                }
                var keys = allKeys.Where(c => where(c)).Skip(skip).Take(count).Select(c => ConstantCacheKey.Create(c)).ToList();
                var response = CacheResponse.SuccessResponse<GetKeysResponse>(server, db);
                response.Keys = new CachePaging<CacheKey>(options.Query?.Page ?? 1, options.Query?.PageSize ?? allKeys.Count, allKeys.Count, keys);
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region Clear data

        /// <summary>
        /// Clear database data
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return clear data response</returns>
        public async Task<IEnumerable<ClearDataResponse>> ClearDataAsync(CacheServer server, ClearDataOptions options)
        {
            var databases = GetDatabases(server);
            List<ClearDataResponse> responses = new List<ClearDataResponse>(databases.Count);
            foreach (var db in databases)
            {
                db.Store.Compact(1);
                responses.Add(CacheResponse.SuccessResponse<ClearDataResponse>(server, db));
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region Get cache item detail

        /// <summary>
        /// Get cache item detail
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return get key detail response</returns>
        public async Task<IEnumerable<GetDetailResponse>> GetKeyDetailAsync(CacheServer server, GetDetailOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return WrapResponse(CacheResponse.FailResponse<GetDetailResponse>(CacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var databases = GetDatabases(server);
            List<GetDetailResponse> responses = new List<GetDetailResponse>(databases.Count);
            foreach (var db in databases)
            {
                GetDetailResponse response = null;
                if (db.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    CacheKeyType cacheKeyType = CacheKeyType.String;
                    if (entry.Value is List<string>)
                    {
                        cacheKeyType = CacheKeyType.List;
                    }
                    else if (entry.Value is ConcurrentDictionary<string, dynamic>)
                    {
                        cacheKeyType = CacheKeyType.Hash;
                    }
                    else if (entry.Value is ConcurrentDictionary<string, byte>)
                    {
                        cacheKeyType = CacheKeyType.Set;
                    }
                    else if (entry.Value is ConcurrentDictionary<string, double>)
                    {
                        cacheKeyType = CacheKeyType.SortedSet;
                    }
                    response = CacheResponse.SuccessResponse<GetDetailResponse>();
                    response.CacheEntry = new CacheEntry()
                    {
                        Key = options.Key,
                        Value = entry.Value,
                        Type = cacheKeyType,
                        When = CacheSetWhen.Always,
                        Expiration = new CacheExpiration()
                        {
                            AbsoluteExpiration = entry.AbsoluteExpiration,
                            AbsoluteExpirationRelativeToNow = entry.AbsoluteExpirationRelativeToNow,
                            SlidingExpiration = entry.SlidingExpiration.HasValue
                        }
                    };
                }
                else
                {
                    response = CacheResponse.FailResponse<GetDetailResponse>(CacheCodes.KeyIsNotExist);
                }
                response.CacheServer = server;
                response.Database = db;
                responses.Add(response);
            }
            return await Task.FromResult(responses).ConfigureAwait(false);
        }

        #endregion

        #region Get server config

        /// <summary>
        /// Get server config
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return get server config response</returns>
        public async Task<IEnumerable<GetServerConfigurationResponse>> GetServerConfigurationAsync(CacheServer server, GetServerConfigurationOptions options)
        {
            return WrapResponse(await Task.FromResult(CacheResponse.FailResponse<GetServerConfigurationResponse>(CacheCodes.OperationIsNotSupported)).ConfigureAwait(false));
        }

        #endregion

        #region Save server configuration

        /// <summary>
        /// Save server config
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return save server config response</returns>
        public async Task<IEnumerable<SaveServerConfigurationResponse>> SaveServerConfigurationAsync(CacheServer server, SaveServerConfigurationOptions options)
        {
            return WrapResponse(await Task.FromResult(CacheResponse.FailResponse<SaveServerConfigurationResponse>(CacheCodes.OperationIsNotSupported)).ConfigureAwait(false));
        }

        #endregion

        #endregion

        #region Util

        /// <summary>
        /// Set expiration
        /// </summary>
        /// <param name="cacheEntry">Cache entry</param>
        /// <param name="expiration">Expiration</param>
        static void SetExpiration(ICacheEntry cacheEntry, CacheExpiration expiration)
        {
            if (expiration == null || cacheEntry == null)
            {
                return;
            }
            if (expiration.AbsoluteExpiration.HasValue)
            {
                cacheEntry.SetAbsoluteExpiration(expiration.AbsoluteExpiration.Value);
                return;
            }
            if (expiration.AbsoluteExpirationRelativeToNow.HasValue)
            {
                if (expiration.SlidingExpiration)
                {
                    cacheEntry.SetSlidingExpiration(expiration.AbsoluteExpirationRelativeToNow.Value);
                }
                else
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = expiration.AbsoluteExpirationRelativeToNow;
                }
            }
        }

        /// <summary>
        /// Get expiration time
        /// </summary>
        /// <param name="cacheEntry">Cache entry</param>
        /// <returns>Return expiration time</returns>
        static Tuple<bool, TimeSpan?> GetExpiration(ICacheEntry cacheEntry)
        {
            if (cacheEntry == null)
            {
                return new Tuple<bool, TimeSpan?>(false, null);
            }
            TimeSpan? value = null;
            bool slidingExpiration = false;
            if (cacheEntry.SlidingExpiration.HasValue)
            {
                value = cacheEntry.SlidingExpiration;
                slidingExpiration = true;
            }
            else if (cacheEntry.AbsoluteExpiration.HasValue)
            {
                var nowDate = DateTimeOffset.Now;
                if (cacheEntry.AbsoluteExpiration.Value <= nowDate)
                {
                    value = TimeSpan.Zero;
                }
                else
                {
                    value = cacheEntry.AbsoluteExpiration.Value - nowDate;
                }
            }
            return new Tuple<bool, TimeSpan?>(slidingExpiration, value);
        }

        static CacheExpiration GetCacheExpiration(ICacheEntry cacheEntry)
        {
            var expirationOptions = GetExpiration(cacheEntry);
            var expiration = new CacheExpiration()
            {
                SlidingExpiration = expirationOptions.Item1
            };
            if (expirationOptions.Item1)
            {
                expiration.AbsoluteExpirationRelativeToNow = expirationOptions.Item2;
            }
            else
            {
                expiration.AbsoluteExpiration = DateTimeOffset.Now.AddMilliseconds(expirationOptions.Item2?.TotalMilliseconds ?? 0);
            }
            return expiration;
        }

        /// <summary>
        /// Get encoding
        /// </summary>
        /// <returns></returns>
        static Encoding GetEncoding()
        {
            return CacheManager.Configuration.DefaultEncoding ?? Encoding.UTF8;
        }

        static IEnumerable<T> WrapResponse<T>(params T[] responses)
        {
            if (responses.IsNullOrEmpty())
            {
                return Array.Empty<T>();
            }
            return responses;
        }

        static MemoryCacheDatabase GetDatabase(string databaseName)
        {
            MemoryCacheCollection.TryGetValue(databaseName, out var database);
            return database;
        }

        static List<MemoryCacheDatabase> GetDatabases(IEnumerable<string> databaseNames)
        {
            if (databaseNames.IsNullOrEmpty())
            {
                databaseNames = new string[1] { DefaultMemoryCacheName };
            }
            return MemoryCacheCollection.Where(c => databaseNames.Contains(c.Key)).Select(c => c.Value).ToList();
        }

        static List<MemoryCacheDatabase> GetDatabases(CacheServer server)
        {
            return GetDatabases(server?.Databases);
        }

        /// <summary>
        /// Add databases
        /// </summary>
        /// <param name="databaseNames">Database names</param>
        public static void AddDatabase(params string[] databaseNames)
        {
            if (databaseNames.IsNullOrEmpty())
            {
                return;
            }
            foreach (var dbName in databaseNames)
            {
                MemoryCacheCollection[dbName] = new MemoryCacheDatabase()
                {
                    Index = 0,
                    Name = dbName,
                    Store = new MemoryCache(Options.Create(new MemoryCacheOptions()))
                };
            }
        }

        #endregion
    }
}
