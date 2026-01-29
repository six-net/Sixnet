// "Company © 2025. All rights reserved."

using System.Collections;
using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using Sixnet.Algorithm.Selection;
using Sixnet.Cache.Hash.Parameters;
using Sixnet.Cache.Hash.Results;
using Sixnet.Cache.Keys.Parameters;
using Sixnet.Cache.Keys.Results;
using Sixnet.Cache.List.Parameters;
using Sixnet.Cache.List.Results;
using Sixnet.Cache.Provider.Memory.Abstractions;
using Sixnet.Cache.Server.Parameters;
using Sixnet.Cache.Server.Response;
using Sixnet.Cache.Set.Parameters;
using Sixnet.Cache.Set.Results;
using Sixnet.Cache.SortedSet;
using Sixnet.Cache.SortedSet.Parameters;
using Sixnet.Cache.SortedSet.Results;
using Sixnet.Cache.String.Parameters;
using Sixnet.Cache.String.Results;
using Sixnet.Code;

namespace Sixnet.Cache.Provider.Memory
{
    /// <summary>
    /// In memory cache provider
    /// </summary>
    public partial class MemoryProvider : ISixnetCacheProvider
    {
        /// <summary>
        /// Default memory cache name
        /// </summary>
        const string DefaultMemoryCacheName = "SIXNET_MEMORY_CACHE_DEFAULT_NAME";

        /// <summary>
        /// Memory cache collection
        /// </summary>
        static readonly Dictionary<string, MemoryCacheDatabase> MemoryCacheCollection = new();

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
        /// <param name="parameter">Parameter</param>
        /// <returns>Return string set range result</returns>
        public Task<SixnetStringSetRangeResult> StringSetRangeAsync(SixnetCacheServer server, SixnetStringSetRangeParameter parameter)
        {
            string key = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(key))
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetStringSetRangeResult>(SixnetCacheCodes.KeyIsNullOrEmpty));
            }
            if (parameter.Offset < 0)
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetStringSetRangeResult>(SixnetCacheCodes.OffsetLessZero));
            }
            var database = GetDatabase(server);
            if (database == null)
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetStringSetRangeResult>(SixnetCacheCodes.DatabaseIsNull));
            }
            lock (database)
            {
                var found = database.Store.TryGetEntry(key, out ICacheEntry cacheEntry);
                var cacheValue = found ? cacheEntry?.Value?.ToString() ?? string.Empty : string.Empty;
                var currentLength = cacheValue.Length;
                var minLength = parameter.Offset;
                if (currentLength == minLength)
                {
                    cacheValue = cacheValue + parameter.Value ?? string.Empty;
                }
                else if (currentLength > minLength)
                {
                    cacheValue = cacheValue.Insert(minLength, parameter.Value);
                }
                else
                {
                    cacheValue += new string('\x00', minLength - currentLength) + parameter.Value;
                }
                if (found)
                {
                    cacheEntry.SetValue(cacheValue);
                }
                else
                {
                    using (var newEntry = database.Store.CreateEntry(key))
                    {
                        newEntry.Value = cacheValue;
                        SetExpiration(newEntry, parameter.Expiration);
                    }
                }
                var response = SixnetCacheResult.SuccessResponse<SixnetStringSetRangeResult>(server, database);
                response.NewValueLength = cacheValue?.Length ?? 0;
                return Task.FromResult(response);
            }
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
        /// <param name="parameter">Parameter</param>
        /// <returns>Return string set bit result</returns>
        public async Task<SixnetStringSetBitResult> StringSetBitAsync(SixnetCacheServer server, SixnetStringSetBitParameter parameter)
        {
            var key = parameter?.Key?.GetActualKey() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(key))
            {
                return SixnetCacheResult.FailResponse<SixnetStringSetBitResult>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            if (parameter.Offset < 0)
            {
                return SixnetCacheResult.FailResponse<SixnetStringSetBitResult>(SixnetCacheCodes.OffsetLessZero);
            }
            var database = GetDatabase(server);
            var oldBitValue = false;
            lock (database)
            {
                var found = database.Store.TryGetEntry(key, out ICacheEntry cacheEntry);
                var bitValue = parameter.Bit ? '1' : '0';
                var cacheValue = found ? cacheEntry?.Value?.ToString() ?? string.Empty : string.Empty;

                var binaryValue = cacheValue.ToBinaryString(GetEncoding());
                var binaryArray = binaryValue.ToCharArray();
                if (binaryArray.Length > parameter.Offset)
                {
                    oldBitValue = binaryArray[parameter.Offset] == '1';
                    binaryArray[parameter.Offset] = bitValue;
                }
                else
                {
                    var diffLength = parameter.Offset - binaryArray.LongLength;
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
                    using (var entry = database.Store.CreateEntry(key))
                    {
                        entry.Value = cacheValue;
                        SetExpiration(entry, parameter.Expiration);
                    }
                }
            }
            var response = SixnetCacheResult.SuccessResponse<SixnetStringSetBitResult>(server, database);
            response.OldBitValue = oldBitValue;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region StringSet

        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten,
        /// regardless of its type.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return string set result</returns>
        public async Task<SixnetStringSetResult> StringSetAsync(SixnetCacheServer server, SixnetStringSetParameter parameter)
        {
            if (parameter?.Items.IsNullOrEmpty() ?? true)
            {
                return SixnetCacheResult.FailResponse<SixnetStringSetResult>(SixnetCacheCodes.ValuesIsNullOrEmpty);
            }
            var results = new List<SixnetStringEntrySetResult>(parameter.Items.Count);
            var database = GetDatabase(server);
            lock (database)
            {
                foreach (var data in parameter.Items)
                {
                    var cacheKey = data.Key?.GetActualKey() ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(cacheKey))
                    {
                        continue;
                    }
                    var found = database.Store.TryGetEntry(cacheKey, out var nowEntry);
                    var setCache = data.When == CacheSetWhen.Always
                        || data.When == CacheSetWhen.Exists && found
                        || data.When == CacheSetWhen.NotExists && !found;
                    if (!setCache)
                    {
                        continue;
                    }
                    using (var entry = database.Store.CreateEntry(cacheKey))
                    {
                        entry.Value = data.Value?.ToString() ?? string.Empty;
                        SetExpiration(entry, data.Expiration);
                    }
                    results.Add(new SixnetStringEntrySetResult()
                    {
                        Key = cacheKey,
                        SetSuccess = true
                    });
                }
            }
            var response = SixnetCacheResult.SuccessResponse<SixnetStringSetResult>(server, database);
            response.Results = results;
            return await Task.FromResult(response).ConfigureAwait(false);

        }

        #endregion

        #region StringLength

        /// <summary>
        /// Returns the length of the string value stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return string length result</returns>
        public async Task<SixnetStringLengthResult> StringLengthAsync(SixnetCacheServer server, SixnetStringLengthParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetStringLengthResult>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            var database = GetDatabase(server);
            var response = SixnetCacheResult.SuccessResponse<SixnetStringLengthResult>(server, database);
            if (database.Store.TryGetValue<string>(cacheKey, out var value))
            {
                response.Length = value?.Length ?? 0;
            }
            return await Task.FromResult(response).ConfigureAwait(false);

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
        /// <param name="parameter">Parameter</param>
        /// <returns>Return string increment result</returns>
        public Task<SixnetStringIncrementResult> StringIncrementAsync(SixnetCacheServer server, SixnetStringIncrementParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetStringIncrementResult>(SixnetCacheCodes.KeyIsNullOrEmpty));
            }
            var database = GetDatabase(server);
            SixnetStringIncrementResult response = null;
            long nowValue = 0;
            lock (database)
            {
                if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    if (long.TryParse(entry.Value?.ToString(), out nowValue))
                    {
                        nowValue += parameter.Value;
                        entry.SetValue(nowValue);
                    }
                    else
                    {
                        response = SixnetCacheResult.FailResponse<SixnetStringIncrementResult>(SixnetCacheCodes.ValueCannotBeCalculated, server: server, database: database);
                        return Task.FromResult(response);
                    }
                }
                else
                {
                    using (entry = database.Store.CreateEntry(cacheKey))
                    {
                        nowValue = parameter.Value;
                        entry.Value = parameter.Value;
                        SetExpiration(entry, parameter.Expiration);
                    }
                }
            }
            response = SixnetCacheResult.SuccessResponse<SixnetStringIncrementResult>(server, database);
            response.NewValue = nowValue;
            return Task.FromResult(response);
        }

        #endregion

        #region StringGetWithExpiry

        /// <summary>
        /// Get the value of key. If the key does not exist the special value nil is returned.
        /// An error is returned if the value stored at key is not a string, because GET
        /// only handles string values.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return string get with expiry result</returns>
        public async Task<SixnetStringGetWithExpiryResult> StringGetWithExpiryAsync(SixnetCacheServer server, SixnetStringGetWithExpiryParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetStringGetWithExpiryResult>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            var nowValue = string.Empty;
            TimeSpan? expriy = null;
            var database = GetDatabase(server);
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                nowValue = entry.Value?.ToString() ?? string.Empty;
                expriy = GetExpiration(entry).Item2;
            }
            var response = SixnetCacheResult.SuccessResponse<SixnetStringGetWithExpiryResult>(server, database);
            response.Value = nowValue;
            response.Expiry = expriy;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region StringGetSet

        /// <summary>
        /// Atomically sets key to value and returns the old value stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return string get set result</returns>
        public async Task<SixnetStringGetSetResult> StringGetSetAsync(SixnetCacheServer server, SixnetStringGetSetParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetStringGetSetResult>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            var oldValue = string.Empty;
            var database = GetDatabase(server);
            lock (database)
            {
                if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    oldValue = entry.Value?.ToString() ?? string.Empty;
                    entry.SetValue(parameter.NewValue);
                }
                else
                {
                    using (entry = database.Store.CreateEntry(cacheKey))
                    {
                        entry.SetValue(parameter.NewValue);
                    }
                }
            }
            var response = SixnetCacheResult.SuccessResponse<SixnetStringGetSetResult>(server, database);
            response.OldValue = oldValue;
            return await Task.FromResult(response).ConfigureAwait(false);
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
        /// <param name="parameter">Parameter</param>
        /// <returns>Return string get range result</returns>
        public async Task<SixnetStringGetRangeResult> StringGetRangeAsync(SixnetCacheServer server, SixnetStringGetRangeParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetStringGetRangeResult>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            var subValue = string.Empty;
            var database = GetDatabase(server);
            SixnetStringGetRangeResult response = null;
            if (database.Store.TryGetValue<string>(cacheKey, out var value) && !string.IsNullOrWhiteSpace(value))
            {
                int start = parameter.Start;
                int end = parameter.End;
                int valueLength = (value ?? string.Empty).Length;
                if (start < 0)
                {
                    start = value.Length - Math.Abs(start);
                }
                if (start < 0 || start >= valueLength)
                {
                    response = SixnetCacheResult.FailResponse<SixnetStringGetRangeResult>(SixnetCacheCodes.OffsetError, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                if (end < 0)
                {
                    end = value.Length - Math.Abs(end);
                }
                if (end < 0 || end >= valueLength)
                {
                    response = SixnetCacheResult.FailResponse<SixnetStringGetRangeResult>(SixnetCacheCodes.OffsetError, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                subValue = value.Substring(Math.Min(start, end), Math.Abs(end - start) + 1);
            }
            response = SixnetCacheResult.SuccessResponse<SixnetStringGetRangeResult>(server, database);
            response.Value = subValue;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region StringGetBit

        /// <summary>
        /// Returns the bit value at offset in the string value stored at key. When offset
        /// is beyond the string length, the string is assumed to be a contiguous space with
        /// 0 bits
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return string get bit result</returns>
        public async Task<SixnetStringGetBitResult> StringGetBitAsync(SixnetCacheServer server, SixnetStringGetBitParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetStringGetBitResult>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            var database = GetDatabase(server);
            SixnetStringGetBitResult response = null;
            char bit = '0';
            if (database.Store.TryGetValue<string>(cacheKey, out var value) && !string.IsNullOrWhiteSpace(value))
            {
                var binaryArray = value.ToBinaryString(GetEncoding()).ToCharArray();
                var offset = parameter.Offset;
                if (offset < 0)
                {
                    offset = binaryArray.LongLength - Math.Abs(offset);
                }
                if (offset < 0 || offset >= binaryArray.LongLength)
                {
                    response = SixnetCacheResult.FailResponse<SixnetStringGetBitResult>(SixnetCacheCodes.OffsetError, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                bit = binaryArray[offset];
            }
            response = SixnetCacheResult.SuccessResponse<SixnetStringGetBitResult>(server, database);
            response.Bit = bit == '1';
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region StringGet

        /// <summary>
        /// Returns the values of all specified keys. For every key that does not hold a
        /// string value or does not exist, the special value nil is returned.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return string get result</returns>
        public async Task<SixnetStringGetResult> StringGetAsync(SixnetCacheServer server, SixnetStringGetParameter parameter)
        {
            if (parameter?.Keys.IsNullOrEmpty() ?? true)
            {
                return SixnetCacheResult.FailResponse<SixnetStringGetResult>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            var database = GetDatabase(server);
            var datas = new List<SixnetCacheEntry>();
            foreach (var key in parameter.Keys)
            {
                var cacheKey = key.GetActualKey();
                if (string.IsNullOrWhiteSpace(cacheKey))
                {
                    continue;
                }
                if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    datas.Add(new SixnetCacheEntry()
                    {
                        Key = key,
                        Value = entry.Value?.ToString() ?? string.Empty,
                        Expiration = new SixnetCacheExpiration()
                        {
                            AbsoluteExpiration = entry.AbsoluteExpiration,
                            AbsoluteExpirationRelativeToNow = entry.AbsoluteExpirationRelativeToNow,
                            SlidingExpiration = entry.SlidingExpiration.HasValue
                        }
                    });
                }
            }
            var response = SixnetCacheResult.SuccessResponse<SixnetStringGetResult>(server, database);
            response.Values = datas;
            return await Task.FromResult(response).ConfigureAwait(false);
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
        /// <param name="parameter">Parameter</param>
        /// <returns>Return string decrement result</returns>
        public Task<SixnetStringDecrementResult> StringDecrementAsync(SixnetCacheServer server, SixnetStringDecrementParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetStringDecrementResult>(SixnetCacheCodes.KeyIsNullOrEmpty));
            }
            var database = GetDatabase(server);
            long nowValue = 0;
            SixnetStringDecrementResult response = null;
            lock (database)
            {
                if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    if (long.TryParse(entry.Value?.ToString(), out nowValue))
                    {
                        nowValue -= parameter.Value;
                        entry.SetValue(nowValue);
                    }
                    else
                    {
                        response = SixnetCacheResult.FailResponse<SixnetStringDecrementResult>(SixnetCacheCodes.ValueCannotBeCalculated, server: server, database: database);
                        return Task.FromResult(response);
                    }
                }
                else
                {
                    using (entry = database.Store.CreateEntry(cacheKey))
                    {
                        nowValue = parameter.Value;
                        entry.Value = parameter.Value;
                        SetExpiration(entry, parameter.Expiration);
                    }
                }
            }
            response = SixnetCacheResult.SuccessResponse<SixnetStringDecrementResult>(server, database);
            response.NewValue = nowValue;
            return Task.FromResult(response);
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
        /// <param name="parameter">Parameter</param>
        /// <returns>Return string bit position result</returns>
        public async Task<SixnetStringBitPositionResult> StringBitPositionAsync(SixnetCacheServer server, SixnetStringBitPositionParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetStringBitPositionResult>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            if ((parameter.Start >= 0 && parameter.End < parameter.Start) || (parameter.Start < 0 && parameter.End > parameter.Start))
            {
                return SixnetCacheResult.FailResponse<SixnetStringBitPositionResult>(SixnetCacheCodes.OffsetError);
            }
            var database = GetDatabase(server);
            SixnetStringBitPositionResult response = null;
            bool hasValue = false;
            long position = 0;
            if (database.Store.TryGetValue<string>(cacheKey, out var value) && !string.IsNullOrWhiteSpace(value))
            {
                char[] valueArray = value.ToBinaryString(GetEncoding()).ToCharArray();
                var matchBit = parameter.Bit ? '1' : '0';
                var length = valueArray.LongLength;
                var start = parameter.Start;
                var end = parameter.End;
                if (start < 0)
                {
                    start = length - Math.Abs(start);
                }
                if (start < 0 || start >= length)
                {
                    response = SixnetCacheResult.FailResponse<SixnetStringBitPositionResult>(SixnetCacheCodes.OffsetError, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                if (end < 0)
                {
                    end = length - Math.Abs(end);
                }
                if (end < 0 || end >= length)
                {
                    response = SixnetCacheResult.FailResponse<SixnetStringBitPositionResult>(SixnetCacheCodes.OffsetError, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
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
            response = SixnetCacheResult.SuccessResponse<SixnetStringBitPositionResult>(server, database);
            response.HasValue = hasValue;
            response.Position = position;
            return await Task.FromResult(response).ConfigureAwait(false);
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
        /// <param name="parameter">Parameter</param>
        /// <returns>Return string bit operation result</returns>
        public async Task<SixnetStringBitOperationResult> StringBitOperationAsync(SixnetCacheServer server, SixnetStringBitOperationParameter parameter)
        {
            if (parameter.Keys.IsNullOrEmpty() || string.IsNullOrWhiteSpace(parameter.DestinationKey))
            {
                return SixnetCacheResult.FailResponse<SixnetStringBitOperationResult>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            if (parameter.Keys.Count > 1 && parameter.Bitwise == CacheBitwise.Not)
            {
                throw new NotSupportedException($" CacheBitwise.Not can only operate on one key");
            }
            var database = GetDatabase(server);
            BitArray bitArray = null;
            SixnetStringBitOperationResult response = null;
            foreach (var key in parameter.Keys)
            {
                if (database.Store.TryGetEntry(key, out ICacheEntry cacheEntry))
                {
                    var binaryString = (cacheEntry?.Value?.ToString() ?? string.Empty).ToBinaryString(GetEncoding());
                    var binaryArray = new BitArray(binaryString.Select(c => (int)c).ToArray());
                    if (bitArray == null)
                    {
                        bitArray = binaryArray;
                    }
                    else
                    {
                        bitArray = parameter.Bitwise switch
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
                return SixnetCacheResult.FailResponse<SixnetStringBitOperationResult>(SixnetCacheCodes.ValuesIsNullOrEmpty, server: server, database: database);
            }
            var bitString = string.Join("", bitArray.Cast<bool>().Select(c => c ? 1 : 0));
            var originalString = bitString.ToOriginalString(GetEncoding());
            var setRes = await StringSetAsync(server, new SixnetStringSetParameter()
            {
                Items = new List<SixnetCacheEntry>()
                {
                    new SixnetCacheEntry()
                    {
                        Key=parameter.DestinationKey,
                        Type=CacheKeyType.String,
                        Value=originalString,
                        Expiration=parameter.Expiration
                    }
                }
            }).ConfigureAwait(false);
            if (setRes?.Success ?? false)
            {
                response = new SixnetStringBitOperationResult()
                {
                    Success = true,
                    DestinationValueLength = originalString.Length,
                    CacheServer = server,
                    Database = database
                };
            }
            else
            {
                response = SixnetCacheResult.FailResponse<SixnetStringBitOperationResult>(setRes.Code, setRes.Message, server, database);
            }
            return response;
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
        /// <param name="parameter">Parameter</param>
        /// <returns>Return string bit count result</returns>
        public async Task<SixnetStringBitCountResult> StringBitCountAsync(SixnetCacheServer server, SixnetStringBitCountParameter parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter?.Key))
            {
                throw new ArgumentNullException($"{nameof(SixnetStringBitCountParameter)}.{nameof(SixnetStringBitCountParameter.Key)}");
            }
            var cacheKey = parameter.Key.GetActualKey();
            var bitCount = 0;
            var database = GetDatabase(server);
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var value = entry.Value?.ToString() ?? string.Empty;
                bitCount = value.ToBinaryString(GetEncoding()).Count(c => c == '1');
            }
            var response = new SixnetStringBitCountResult()
            {
                Success = true,
                BitNum = bitCount
            };
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region StringAppend

        /// <summary>
        /// If key already exists and is a string, this options appends the value at the
        /// end of the string. If key does not exist it is created and set as an empty string,
        /// so APPEND will be similar to SET in this special case.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return string append result</returns>
        public async Task<SixnetStringAppendResult> StringAppendAsync(SixnetCacheServer server, SixnetStringAppendParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetStringAppendResult>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            long valueLength = 0;
            var database = GetDatabase(server);
            lock (database)
            {
                if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var nowValue = entry.Value?.ToString() ?? string.Empty;
                    nowValue += parameter.Value ?? string.Empty;
                    valueLength = nowValue.Length;
                    entry.SetValue(nowValue);
                }
                else
                {
                    using (entry = database.Store.CreateEntry(cacheKey))
                    {
                        entry.SetValue(parameter.Value);
                        SetExpiration(entry, parameter.Expiration);
                    }
                }
            }
            var response = SixnetCacheResult.SuccessResponse<SixnetStringAppendResult>(server, database);
            response.NewValueLength = valueLength;
            return await Task.FromResult(response).ConfigureAwait(false);
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
        /// <param name="parameter">Parameter</param>
        /// <returns>Return list trim result</returns>
        public Task<SixnetListTrimResult> ListTrimAsync(SixnetCacheServer server, SixnetListTrimParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetListTrimResult>(SixnetCacheCodes.KeyIsNullOrEmpty));
            }
            var database = GetDatabase(server);
            SixnetListTrimResult response = null;
            lock (database)
            {
                if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    if (entry.Value is not List<string> list)
                    {
                        response = SixnetCacheResult.FailResponse<SixnetListTrimResult>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                        return Task.FromResult(response);
                    }
                    var start = parameter.Start;
                    var end = parameter.Stop;
                    int count = list.Count;
                    if (start < 0)
                    {
                        start = count - Math.Abs(start);
                    }
                    if (start < 0 || start >= count)
                    {
                        response = SixnetCacheResult.FailResponse<SixnetListTrimResult>(SixnetCacheCodes.OffsetError, server: server, database: database);
                        return Task.FromResult(response);
                    }
                    if (end < 0)
                    {
                        end = count - Math.Abs(end);
                    }
                    if (end < 0 || end >= count)
                    {
                        response = SixnetCacheResult.FailResponse<SixnetListTrimResult>(SixnetCacheCodes.OffsetError, server: server, database: database);
                        return Task.FromResult(response);
                    }
                    var begin = Math.Min(start, end);
                    var takeCount = Math.Abs(end - start) + 1;
                    var nowList = list.Skip(begin).Take(takeCount).ToList();
                    entry.SetValue(nowList);
                    response = SixnetCacheResult.SuccessResponse<SixnetListTrimResult>();
                }
                else
                {
                    response = SixnetCacheResult.FailResponse<SixnetListTrimResult>(SixnetCacheCodes.KeyIsNotExist);
                }
            }
            response.CacheServer = server;
            response.Database = database;
            return Task.FromResult(response);
        }

        #endregion

        #region ListSetByIndex

        /// <summary>
        /// Sets the list element at index to value. For more information on the index argument,
        ///  see ListGetByIndex. An error is returned for out of range indexes.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return list set by index result</returns>
        public Task<SixnetListSetByIndexResult> ListSetByIndexAsync(SixnetCacheServer server, SixnetListSetByIndexParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetListSetByIndexResult>(SixnetCacheCodes.KeyIsNullOrEmpty));
            }
            var database = GetDatabase(server);
            SixnetListSetByIndexResult response = null;
            lock (database)
            {
                if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    if (entry.Value is not List<string> list)
                    {
                        response = SixnetCacheResult.FailResponse<SixnetListSetByIndexResult>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                        return Task.FromResult(response);
                    }
                    var index = parameter.Index;
                    if (index < 0)
                    {
                        index = list.Count - Math.Abs(index);
                    }
                    if (index < 0 || index >= list.Count)
                    {
                        response = SixnetCacheResult.FailResponse<SixnetListSetByIndexResult>(SixnetCacheCodes.OffsetError, server: server, database: database);
                        return Task.FromResult(response);
                    }
                    list[index] = parameter.Value;
                    response = SixnetCacheResult.SuccessResponse<SixnetListSetByIndexResult>();
                }
                else
                {
                    response = SixnetCacheResult.FailResponse<SixnetListSetByIndexResult>(SixnetCacheCodes.KeyIsNotExist);
                }
            }
            response.CacheServer = server;
            response.Database = database;
            return Task.FromResult(response);
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
        /// <param name="parameter">Parameter</param>
        /// <returns>Return list right push</returns>
        public Task<SixnetListRightPushResult> ListRightPushAsync(SixnetCacheServer server, SixnetListRightPushParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetListRightPushResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server));
            }
            if (parameter.Values.IsNullOrEmpty())
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetListRightPushResult>(SixnetCacheCodes.ValuesIsNullOrEmpty, server: server));
            }
            var database = GetDatabase(server);
            SixnetListRightPushResult response;
            lock (database)
            {
                if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    if (!(entry.Value is List<string> list))
                    {
                        response = SixnetCacheResult.FailResponse<SixnetListRightPushResult>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                        return Task.FromResult(response);
                    }
                    list = list.Concat(parameter.Values).ToList();
                    entry.SetValue(list);
                    response = SixnetCacheResult.SuccessResponse<SixnetListRightPushResult>();
                    response.NewListLength = list.Count;
                }
                else
                {
                    using (entry = database.Store.CreateEntry(cacheKey))
                    {
                        entry.SetValue(new List<string>(parameter.Values));
                        SetExpiration(entry, parameter.Expiration);
                    }
                    response = SixnetCacheResult.SuccessResponse<SixnetListRightPushResult>();
                    response.NewListLength = parameter.Values.Count;
                }
            }
            response.CacheServer = server;
            response.Database = database;
            return Task.FromResult(response);
        }

        #endregion

        #region ListRightPopLeftPush

        /// <summary>
        /// Atomically returns and removes the last element (tail) of the list stored at
        /// source, and pushes the element at the first element (head) of the list stored
        /// at destination.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return list right pop left result</returns>
        public Task<SixnetListRightPopLeftPushResult> ListRightPopLeftPushAsync(SixnetCacheServer server, SixnetListRightPopLeftPushParameter parameter)
        {
            var sourceCacheKey = parameter?.SourceKey?.GetActualKey();
            var destionationCacheKey = parameter?.DestinationKey?.GetActualKey();
            if (string.IsNullOrWhiteSpace(sourceCacheKey) || string.IsNullOrWhiteSpace(destionationCacheKey))
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetListRightPopLeftPushResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var database = GetDatabase(server);
            SixnetListRightPopLeftPushResult response = null;
            lock (database)
            {
                if (database.Store.TryGetEntry(sourceCacheKey, out var entry) && entry != null)
                {
                    if (entry.Value is not List<string> list)
                    {
                        response = SixnetCacheResult.FailResponse<SixnetListRightPopLeftPushResult>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                        return Task.FromResult(response);
                    }
                    if (list.Count < 1)
                    {
                        response = SixnetCacheResult.FailResponse<SixnetListRightPopLeftPushResult>(SixnetCacheCodes.ListIsEmpty, server: server, database: database);
                        return Task.FromResult(response);
                    }
                    List<string> desList = null;
                    if (database.Store.TryGetEntry(destionationCacheKey, out var desEntry) && desEntry != null)
                    {
                        desList = desEntry.Value as List<string>;
                        if (desList == null)
                        {
                            response = SixnetCacheResult.FailResponse<SixnetListRightPopLeftPushResult>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                            return Task.FromResult(response);
                        }
                    }
                    var index = list.Count - 1;
                    var value = list[index];
                    list.RemoveAt(index);
                    if (desEntry == null)
                    {
                        using (desEntry = database.Store.CreateEntry(destionationCacheKey))
                        {
                            desEntry.Value = new List<string>() { value };
                            SetExpiration(desEntry, parameter.Expiration);
                        }
                    }
                    else
                    {
                        desList.Insert(0, value);
                    }
                    response = SixnetCacheResult.SuccessResponse<SixnetListRightPopLeftPushResult>();
                    response.PopValue = value;
                }
                else
                {
                    response = SixnetCacheResult.FailResponse<SixnetListRightPopLeftPushResult>(SixnetCacheCodes.KeyIsNotExist);
                }
            }
            response.CacheServer = server;
            response.Database = database;
            return Task.FromResult(response);
        }

        #endregion

        #region ListRightPop

        /// <summary>
        /// Removes and returns the last element of the list stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return list right pop result</returns>
        public Task<SixnetListRightPopResult> ListRightPopAsync(SixnetCacheServer server, SixnetListRightPopParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetListRightPopResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var database = GetDatabase(server);
            SixnetListRightPopResult response = null;
            lock (database)
            {
                var value = string.Empty;
                if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    if (!(entry.Value is List<string> list))
                    {
                        response = SixnetCacheResult.FailResponse<SixnetListRightPopResult>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                        return Task.FromResult(response);
                    }
                    if (list.Count < 1)
                    {
                        response = SixnetCacheResult.FailResponse<SixnetListRightPopResult>(SixnetCacheCodes.ListIsEmpty, server: server, database: database);
                        return Task.FromResult(response);
                    }
                    var index = list.Count - 1;
                    value = list[index];
                    list.RemoveAt(index);
                    response = SixnetCacheResult.SuccessResponse<SixnetListRightPopResult>();
                    response.PopValue = value;
                }
                else
                {
                    response = SixnetCacheResult.FailResponse<SixnetListRightPopResult>(SixnetCacheCodes.KeyIsNotExist);
                }
            }
            response.CacheServer = server;
            response.Database = database;
            return Task.FromResult(response);
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
        /// <param name="parameter">Parameter</param>
        /// <returns>Return list remove result</returns>
        public Task<SixnetListRemoveResult> ListRemoveAsync(SixnetCacheServer server, SixnetListRemoveParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetListRemoveResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var database = GetDatabase(server);
            SixnetListRemoveResult response = null;
            lock (database)
            {
                if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    if (entry.Value is not List<string> list)
                    {
                        response = SixnetCacheResult.FailResponse<SixnetListRemoveResult>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                        return Task.FromResult(response);
                    }
                    var removeCount = 0;
                    if (parameter.Count == 0)
                    {
                        removeCount = list.RemoveAll(a => a == parameter.Value);
                    }
                    else
                    {
                        var count = Math.Abs(parameter.Count);
                        var findLast = parameter.Count < 0;
                        for (var i = 0; i < count; i++)
                        {
                            var index = findLast ? list.FindLastIndex(c => c == parameter.Value) : list.FindIndex(c => c == parameter.Value);
                            if (index < 0)
                            {
                                break;
                            }
                            removeCount++;
                            list.RemoveAt(index);
                        }
                    }
                    response = SixnetCacheResult.SuccessResponse<SixnetListRemoveResult>();
                    response.RemoveCount = removeCount;
                }
                else
                {
                    response = SixnetCacheResult.FailResponse<SixnetListRemoveResult>(SixnetCacheCodes.KeyIsNotExist);
                }
            }
            response.CacheServer = server;
            response.Database = database;
            return Task.FromResult(response);
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
        /// <param name="parameter">Parameter</param>
        /// <returns>Return list range result</returns>
        public async Task<SixnetListRangeResult> ListRangeAsync(SixnetCacheServer server, SixnetListRangeParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetListRangeResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            SixnetListRangeResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not List<string> list)
                {
                    response = SixnetCacheResult.FailResponse<SixnetListRangeResult>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                var start = parameter.Start;
                if (start < 0)
                {
                    start = list.Count - Math.Abs(start);
                }
                if (start < 0 || start >= list.Count)
                {
                    response = SixnetCacheResult.FailResponse<SixnetListRangeResult>(SixnetCacheCodes.OffsetError, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                var end = parameter.Stop;
                if (end < 0)
                {
                    end = list.Count - Math.Abs(end);
                }
                if (end < 0 || end >= list.Count)
                {
                    response = SixnetCacheResult.FailResponse<SixnetListRangeResult>(SixnetCacheCodes.OffsetError, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                var begin = Math.Min(start, end);
                response = SixnetCacheResult.SuccessResponse<SixnetListRangeResult>();
                response.Values = list.GetRange(begin, Math.Abs(end - start) + 1).ToList();
            }
            else
            {
                response = SixnetCacheResult.FailResponse<SixnetListRangeResult>(SixnetCacheCodes.KeyIsNotExist);
            }
            response.CacheServer = server;
            response.Database = database;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region ListLength

        /// <summary>
        /// Returns the length of the list stored at key. If key does not exist, it is interpreted
        ///  as an empty list and 0 is returned.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return list length result</returns>
        public async Task<SixnetListLengthResult> ListLengthAsync(SixnetCacheServer server, SixnetListLengthParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetListLengthResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var length = 0;
            SixnetListLengthResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not List<string> list)
                {
                    response = SixnetCacheResult.FailResponse<SixnetListLengthResult>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                length = list.Count;
            }
            response = SixnetCacheResult.SuccessResponse<SixnetListLengthResult>();
            response.Length = length;
            response.CacheServer = server;
            response.Database = database;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region ListLeftPush

        /// <summary>
        /// Insert the specified value at the head of the list stored at key. If key does
        ///  not exist, it is created as empty list before performing the push operations.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return list left push result</returns>
        public Task<SixnetListLeftPushResult> ListLeftPushAsync(SixnetCacheServer server, SixnetListLeftPushParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetListLeftPushResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server));
            }
            if (parameter.Values.IsNullOrEmpty())
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetListLeftPushResult>(SixnetCacheCodes.ValuesIsNullOrEmpty, server: server));
            }
            var database = GetDatabase(server);
            SixnetListLeftPushResult response;
            lock (database)
            {
                if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    if (entry.Value is not List<string> list)
                    {
                        response = SixnetCacheResult.FailResponse<SixnetListLeftPushResult>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                        return Task.FromResult(response);
                    }
                    list = parameter.Values.Concat(list).ToList();
                    entry.SetValue(list);
                    response = SixnetCacheResult.SuccessResponse<SixnetListLeftPushResult>();
                    response.NewListLength = list.Count;
                }
                else
                {
                    using (entry = database.Store.CreateEntry(cacheKey))
                    {
                        entry.SetValue(new List<string>(parameter.Values));
                        SetExpiration(entry, parameter.Expiration);
                    }
                    response = SixnetCacheResult.SuccessResponse<SixnetListLeftPushResult>();
                    response.NewListLength = parameter.Values.Count;
                }
            }
            response.CacheServer = server;
            response.Database = database;
            return Task.FromResult(response);
        }

        #endregion

        #region ListLeftPop

        /// <summary>
        /// Removes and returns the first element of the list stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return list left pop result</returns>
        public Task<SixnetListLeftPopResult> ListLeftPopAsync(SixnetCacheServer server, SixnetListLeftPopParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetListLeftPopResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var database = GetDatabase(server);
            SixnetListLeftPopResult response = null;
            lock (database)
            {
                string value = string.Empty;
                if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    if (entry.Value is not List<string> list)
                    {
                        response = SixnetCacheResult.FailResponse<SixnetListLeftPopResult>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                        return Task.FromResult(response);
                    }
                    if (list.Count < 1)
                    {
                        response = SixnetCacheResult.FailResponse<SixnetListLeftPopResult>(SixnetCacheCodes.ListIsEmpty, server: server, database: database);
                        return Task.FromResult(response);
                    }
                    value = list[0];
                    list.RemoveAt(0);
                    entry.SetValue(list);
                    response = SixnetCacheResult.SuccessResponse<SixnetListLeftPopResult>();
                    response.PopValue = value;
                }
                else
                {
                    response = SixnetCacheResult.FailResponse<SixnetListLeftPopResult>(SixnetCacheCodes.KeyIsNotExist);
                }
            }
            response.CacheServer = server;
            response.Database = database;
            return Task.FromResult(response);
        }

        #endregion

        #region ListInsertBefore

        /// <summary>
        /// Inserts value in the list stored at key either before or after the reference
        /// value pivot. When key does not exist, it is considered an empty list and no operation
        /// is performed.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return list insert begore result</returns>
        public Task<SixnetListInsertBeforeResult> ListInsertBeforeAsync(SixnetCacheServer server, SixnetListInsertBeforeParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetListInsertBeforeResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var database = GetDatabase(server);
            int newLength = 0;
            bool hasInsertValue = false;
            SixnetListInsertBeforeResult response;
            lock (database)
            {
                if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    if (entry.Value is not List<string> list)
                    {
                        response = SixnetCacheResult.FailResponse<SixnetListInsertBeforeResult>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                        return Task.FromResult(response);
                    }
                    var index = list.FindIndex(c => c == parameter.PivotValue);
                    if (index >= 0)
                    {
                        list.Insert(index, parameter.InsertValue);
                        entry.SetValue(list);
                        hasInsertValue = true;
                    }
                    newLength = list.Count;
                }
            }
            response = new SixnetListInsertBeforeResult()
            {
                Success = hasInsertValue,
                NewListLength = newLength,
                CacheServer = server,
                Database = database
            };
            return Task.FromResult(response);
        }

        #endregion

        #region ListInsertAfter

        /// <summary>
        /// Inserts value in the list stored at key either before or after the reference
        /// value pivot. When key does not exist, it is considered an empty list and no operation
        /// is performed.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return list insert after result</returns>
        public Task<SixnetListInsertAfterResult> ListInsertAfterAsync(SixnetCacheServer server, SixnetListInsertAfterParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetListInsertAfterResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var database = GetDatabase(server);
            var newLength = 0;
            var hasInsertValue = false;
            SixnetListInsertAfterResult response;
            lock (database)
            {
                if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    var list = entry.Value as List<string>;
                    if (list == null)
                    {
                        response = SixnetCacheResult.FailResponse<SixnetListInsertAfterResult>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                        return Task.FromResult(response);
                    }
                    var index = list.FindIndex(c => c == parameter.PivotValue);
                    if (index >= 0)
                    {
                        list.Insert(index + 1, parameter.InsertValue);
                        entry.SetValue(list);
                        hasInsertValue = true;
                    }
                    newLength = list.Count;
                }
            }
            response = new SixnetListInsertAfterResult()
            {
                NewListLength = newLength,
                Success = hasInsertValue,
                CacheServer = server,
                Database = database
            };
            return Task.FromResult(response);
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
        /// <param name="parameter">Parameter</param>
        /// <returns>Return list get by index result</returns>
        public async Task<SixnetListGetByIndexResult> ListGetByIndexAsync(SixnetCacheServer server, SixnetListGetByIndexParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetListGetByIndexResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var value = "";
            SixnetListGetByIndexResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not List<string> list)
                {
                    response = SixnetCacheResult.FailResponse<SixnetListGetByIndexResult>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                var index = parameter.Index;
                if (index < 0)
                {
                    index = list.Count - Math.Abs(index);
                }
                if (index < 0 || index >= list.Count)
                {
                    response = SixnetCacheResult.FailResponse<SixnetListGetByIndexResult>(SixnetCacheCodes.OffsetError, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                value = list[index];
            }
            response = SixnetCacheResult.SuccessResponse<SixnetListGetByIndexResult>();
            response.Value = value;
            response.CacheServer = server;
            response.Database = database;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #endregion

        #region Hash

        #region HashValues

        /// <summary>
        /// Returns all values in the hash stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return hash values result</returns>
        public async Task<SixnetHashValuesResult> HashValuesAsync(SixnetCacheServer server, SixnetHashValuesParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetHashValuesResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            List<dynamic> values = null;
            SixnetHashValuesResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, dynamic> dict)
                {
                    response = SixnetCacheResult.FailResponse<SixnetHashValuesResult>(SixnetCacheCodes.ValueIsNotDict, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                values = dict.Values.ToList();
            }
            values ??= new List<dynamic>(0);
            response = SixnetCacheResult.SuccessResponse<SixnetHashValuesResult>(server, database);
            response.Values = values;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region HashSet

        /// <summary>
        /// Sets field in the hash stored at key to value. If key does not exist, a new key
        ///  holding a hash is created. If field already exists in the hash, it is overwritten.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return hash set result</returns>
        public Task<SixnetHashSetResult> HashSetAsync(SixnetCacheServer server, SixnetHashSetParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetHashSetResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var database = GetDatabase(server);
            SixnetHashSetResult response;
            lock (database)
            {
                if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    if (entry.Value is not ConcurrentDictionary<string, dynamic> dict)
                    {
                        response = SixnetCacheResult.FailResponse<SixnetHashSetResult>(SixnetCacheCodes.ValueIsNotDict, server: server, database: database);
                        return Task.FromResult(response);
                    }
                    foreach (var item in parameter.Items)
                    {
                        dict[item.Key] = item.Value;
                    }
                }
                else
                {
                    using (entry = database.Store.CreateEntry(cacheKey))
                    {
                        var value = new ConcurrentDictionary<string, dynamic>(parameter.Items);
                        entry.SetValue(value);
                        SetExpiration(entry, parameter.Expiration);
                    }
                }
            }
            response = SixnetCacheResult.SuccessResponse<SixnetHashSetResult>(server, database);
            return Task.FromResult(response);
        }

        #endregion

        #region HashLength

        /// <summary>
        /// Returns the number of fields contained in the hash stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return hash length result</returns>
        public async Task<SixnetHashLengthResult> HashLengthAsync(SixnetCacheServer server, SixnetHashLengthParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetHashLengthResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            int length = 0;
            SixnetHashLengthResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, dynamic> dict)
                {
                    response = SixnetCacheResult.FailResponse<SixnetHashLengthResult>(SixnetCacheCodes.ValueIsNotDict, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                length = dict.Keys.Count;
            }
            response = SixnetCacheResult.SuccessResponse<SixnetHashLengthResult>(server, database);
            response.Length = length;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region HashKeys

        /// <summary>
        /// Returns all field names in the hash stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return hash keys result</returns>
        public async Task<SixnetHashKeysResult> HashKeysAsync(SixnetCacheServer server, SixnetHashKeysParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetHashKeysResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            SixnetHashKeysResult response;
            List<string> keys = null;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, dynamic> dict)
                {
                    response = SixnetCacheResult.FailResponse<SixnetHashKeysResult>(SixnetCacheCodes.ValueIsNotDict, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                keys = dict.Keys.ToList();
            }
            keys ??= new List<string>(0);
            response = SixnetCacheResult.SuccessResponse<SixnetHashKeysResult>(server, database);
            response.HashKeys = keys;
            return await Task.FromResult(response).ConfigureAwait(false);
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
        /// <param name="parameter">Parameter</param>
        /// <returns>Return hash increment result</returns>
        public Task<SixnetHashIncrementResult> HashIncrementAsync(SixnetCacheServer server, SixnetHashIncrementParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetHashIncrementResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var database = GetDatabase(server);
            var newValue = parameter.IncrementValue;
            SixnetHashIncrementResult response;
            lock (database)
            {
                if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    if (entry.Value is not ConcurrentDictionary<string, dynamic> dict)
                    {
                        response = SixnetCacheResult.FailResponse<SixnetHashIncrementResult>(SixnetCacheCodes.ValueIsNotDict, server: server, database: database);
                        return Task.FromResult(response);
                    }
                    if (dict.TryGetValue(parameter.HashField, out var value))
                    {
                        dict[parameter.HashField] = newValue = value + parameter.IncrementValue;
                    }
                    else
                    {
                        dict[parameter.HashField] = parameter.IncrementValue;
                    }
                }
                else
                {
                    using (entry = database.Store.CreateEntry(cacheKey))
                    {
                        var value = new ConcurrentDictionary<string, dynamic>();
                        value[parameter.HashField] = parameter.IncrementValue;
                        entry.SetValue(value);
                        SetExpiration(entry, parameter.Expiration);
                    }
                }
            }
            response = SixnetCacheResult.SuccessResponse<SixnetHashIncrementResult>(server, database);
            response.HashField = parameter.HashField;
            response.NewValue = newValue;
            return Task.FromResult(response);
        }

        #endregion

        #region HashGet

        /// <summary>
        /// Returns the value associated with field in the hash stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return hash get result</returns>
        public async Task<SixnetHashGetResult> HashGetAsync(SixnetCacheServer server, SixnetHashGetParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetHashGetResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            dynamic value = null;
            SixnetHashGetResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, dynamic> dict)
                {
                    response = SixnetCacheResult.FailResponse<SixnetHashGetResult>(SixnetCacheCodes.ValueIsNotDict, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                dict.TryGetValue(parameter.HashField, out value);
            }
            response = SixnetCacheResult.SuccessResponse<SixnetHashGetResult>(server, database);
            response.Value = value;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region HashGetAll

        /// <summary>
        /// Returns all fields and values of the hash stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return hash get all result</returns>
        public async Task<SixnetHashGetAllResult> HashGetAllAsync(SixnetCacheServer server, SixnetHashGetAllParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetHashGetAllResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            ConcurrentDictionary<string, dynamic> values = null;
            SixnetHashGetAllResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, dynamic> dict)
                {
                    response = SixnetCacheResult.FailResponse<SixnetHashGetAllResult>(SixnetCacheCodes.ValueIsNotDict, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false); ;
                }
                values = new ConcurrentDictionary<string, dynamic>(dict);
            }
            response = SixnetCacheResult.SuccessResponse<SixnetHashGetAllResult>(server, database);
            response.HashValues = values?.ToDictionary(c => c.Key, c => c.Value) ?? new Dictionary<string, dynamic>(0);
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region HashExist

        /// <summary>
        /// Returns if field is an existing field in the hash stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return hash exists result</returns>
        public async Task<SixnetHashExistsResult> HashExistAsync(SixnetCacheServer server, SixnetHashExistsParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetHashExistsResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var existKey = false;
            SixnetHashExistsResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, dynamic> dict)
                {
                    response = SixnetCacheResult.FailResponse<SixnetHashExistsResult>(SixnetCacheCodes.ValueIsNotDict, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                existKey = dict.ContainsKey(parameter.HashField);
            }
            response = SixnetCacheResult.SuccessResponse<SixnetHashExistsResult>(server, database);
            response.HasField = existKey;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region HashDelete

        /// <summary>
        /// Removes the specified fields from the hash stored at key. Non-existing fields
        /// are ignored. Non-existing keys are treated as empty hashes and this options returns 0
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return hash delete result</returns>
        public Task<SixnetHashDeleteResult> HashDeleteAsync(SixnetCacheServer server, SixnetHashDeleteParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetHashDeleteResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var database = GetDatabase(server);
            SixnetHashDeleteResult response;
            var remove = false;
            lock (database)
            {
                if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    if (entry.Value is not ConcurrentDictionary<string, dynamic> dict)
                    {
                        response = SixnetCacheResult.FailResponse<SixnetHashDeleteResult>(SixnetCacheCodes.ValueIsNotDict, server: server, database: database);
                        return Task.FromResult(response);
                    }
                    foreach (var field in parameter.HashFields)
                    {
                        remove |= dict.TryRemove(field, out var value);
                    }
                }
            }
            response = new SixnetHashDeleteResult()
            {
                Success = remove,
                CacheServer = server,
                Database = database
            };
            return Task.FromResult(response);
        }

        #endregion

        #region HashDecrement

        /// <summary>
        /// Decrement the specified field of an hash stored at key, and representing a floating
        ///  point number, by the specified decrement. If the field does not exist, it is
        ///  set to 0 before performing the operation.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return hash decrement result</returns>
        public Task<SixnetHashDecrementResult> HashDecrementAsync(SixnetCacheServer server, SixnetHashDecrementParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetHashDecrementResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var database = GetDatabase(server);
            var newValue = parameter.DecrementValue;
            SixnetHashDecrementResult response;
            lock (database)
            {
                if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    if (entry.Value is not ConcurrentDictionary<string, dynamic> dict)
                    {
                        response = SixnetCacheResult.FailResponse<SixnetHashDecrementResult>(SixnetCacheCodes.ValueIsNotDict, server: server, database: database);
                        return Task.FromResult(response);
                    }
                    if (dict.TryGetValue(parameter.HashField, out var value))
                    {
                        dict[parameter.HashField] = newValue = value - parameter.DecrementValue;
                    }
                    else
                    {
                        dict[parameter.HashField] = parameter.DecrementValue;
                    }
                    entry.SetValue(dict);
                }
                else
                {
                    using (entry = database.Store.CreateEntry(cacheKey))
                    {
                        var value = new ConcurrentDictionary<string, dynamic>();
                        value[parameter.HashField] = parameter.DecrementValue;
                        entry.SetValue(value);
                        SetExpiration(entry, parameter.Expiration);
                    }
                }
            }
            response = SixnetCacheResult.SuccessResponse<SixnetHashDecrementResult>(server, database);
            response.HashField = parameter.HashField;
            response.NewValue = newValue;
            return Task.FromResult(response);
        }

        #endregion

        #region HashScan

        /// <summary>
        /// The HSCAN options is used to incrementally iterate over a hash
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return hash scan result</returns>
        public async Task<SixnetHashScanResult> HashScanAsync(SixnetCacheServer server, SixnetHashScanParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                SixnetCacheResult.FailResponse<SixnetHashScanResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var values = new Dictionary<string, dynamic>();
            SixnetHashScanResult response;
            if (parameter.PageSize > 0 && database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, dynamic>;
                if (dict == null)
                {
                    response = SixnetCacheResult.FailResponse<SixnetHashScanResult>(SixnetCacheCodes.ValueIsNotDict, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                var pageSize = parameter.PageSize;
                foreach (var item in dict)
                {
                    bool accordWith = false;
                    switch (parameter.PatternType)
                    {
                        case KeyMatchPattern.StartWith:
                            accordWith = item.Key.StartsWith(parameter.Pattern);
                            break;
                        case KeyMatchPattern.EndWith:
                            accordWith = item.Key.EndsWith(parameter.Pattern);
                            break;
                        case KeyMatchPattern.Include:
                            accordWith = item.Key.Contains(parameter.Pattern);
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
            response = SixnetCacheResult.SuccessResponse<SixnetHashScanResult>(server, database);
            response.HashValues = values;
            return await Task.FromResult(response).ConfigureAwait(false);
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
        /// <param name="parameter">Parameter</param>
        /// <returns>Return set remove result</returns>
        public Task<SixnetSetRemoveResult> SetRemoveAsync(SixnetCacheServer server, SixnetSetRemoveParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetSetRemoveResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var database = GetDatabase(server);
            var removeCount = 0;
            SixnetSetRemoveResult response;
            lock (database)
            {
                if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null && !parameter.Members.IsNullOrEmpty())
                {
                    if (entry.Value is not ConcurrentDictionary<string, byte> dict)
                    {
                        response = SixnetCacheResult.FailResponse<SixnetSetRemoveResult>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                        return Task.FromResult(response);
                    }
                    foreach (var member in parameter.Members)
                    {
                        if (dict.TryRemove(member, out var value))
                        {
                            removeCount++;
                        }
                    }
                }
            }
            response = SixnetCacheResult.SuccessResponse<SixnetSetRemoveResult>(server, database);
            response.RemoveCount = removeCount;
            return Task.FromResult(response);
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
        /// <param name="parameter">Parameter</param>
        /// <returns>Return set random members result</returns>
        public async Task<SixnetSetRandomMembersResult> SetRandomMembersAsync(SixnetCacheServer server, SixnetSetRandomMembersParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetSetRandomMembersResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var members = new List<string>();
            SixnetSetRandomMembersResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null && parameter.Count != 0)
            {
                if (entry.Value is not ConcurrentDictionary<string, byte> dict)
                {
                    response = SixnetCacheResult.FailResponse<SixnetSetRandomMembersResult>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                var allowSame = parameter.Count < 0;
                var count = Math.Abs(parameter.Count);
                var keys = dict.Keys.ToList();
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
                    var shuffle = new SixnetShuffleNet<string>(keys);
                    members.AddRange(shuffle.TakeNextValues(count));
                }
            }
            response = SixnetCacheResult.SuccessResponse<SixnetSetRandomMembersResult>(server, database);
            response.Members = members;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region SetRandomMember

        /// <summary>
        /// Return a random element from the set value stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return set random member</returns>
        public async Task<SixnetSetRandomMemberResult> SetRandomMemberAsync(SixnetCacheServer server, SixnetSetRandomMemberParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetSetRandomMemberResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var member = string.Empty;
            SixnetSetRandomMemberResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, byte> dict)
                {
                    response = SixnetCacheResult.FailResponse<SixnetSetRandomMemberResult>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                var keys = dict.Keys.ToList();
                if (!keys.IsNullOrEmpty())
                {
                    var ranIndex = RandomNumberHelper.GetRandomNumber(keys.Count - 1);
                    member = keys.ElementAt(ranIndex);
                }
            }
            response = SixnetCacheResult.SuccessResponse<SixnetSetRandomMemberResult>(server, database);
            response.Member = member;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region SetPop

        /// <summary>
        /// Removes and returns a random element from the set value stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return set pop result</returns>
        public Task<SixnetSetPopResult> SetPopAsync(SixnetCacheServer server, SixnetSetPopParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetSetPopResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var database = GetDatabase(server);
            var member = string.Empty;
            SixnetSetPopResult response;
            lock (database)
            {
                if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    if (entry.Value is not ConcurrentDictionary<string, byte> dict)
                    {
                        response = SixnetCacheResult.FailResponse<SixnetSetPopResult>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                        return Task.FromResult(response);
                    }
                    var keys = dict.Keys;
                    if (!keys.IsNullOrEmpty())
                    {
                        var ranIndex = RandomNumberHelper.GetRandomNumber(keys.Count - 1);
                        member = keys.ElementAt(ranIndex);
                        dict.TryRemove(member, out var value);
                    }
                }
            }
            response = SixnetCacheResult.SuccessResponse<SixnetSetPopResult>(server, database);
            response.PopValue = member;
            return Task.FromResult(response);
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
        /// <param name="parameter">Parameter</param>
        /// <returns>Return set move result</returns>
        public Task<SixnetSetMoveResult> SetMoveAsync(SixnetCacheServer server, SixnetSetMoveParameter parameter)
        {
            var cacheKey = parameter?.SourceKey?.GetActualKey();
            var desKey = parameter?.DestinationKey?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey) || string.IsNullOrWhiteSpace(desKey))
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetSetMoveResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var database = GetDatabase(server);
            var isRemove = false;
            SixnetSetMoveResult response;
            lock (database)
            {
                if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    if (entry.Value is not ConcurrentDictionary<string, byte> dict)
                    {
                        response = SixnetCacheResult.FailResponse<SixnetSetMoveResult>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                        return Task.FromResult(response);
                    }
                    database.Store.TryGetEntry(desKey, out var desEntry);
                    ConcurrentDictionary<string, byte> desDict = null;
                    if (desEntry != null)
                    {
                        desDict = desEntry.Value as ConcurrentDictionary<string, byte>;
                        if (desDict == null)
                        {
                            response = SixnetCacheResult.FailResponse<SixnetSetMoveResult>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                            return Task.FromResult(response);
                        }
                    }
                    if (dict.TryRemove(parameter.MoveMember, out var value))
                    {
                        isRemove = true;
                        if (desDict != null)
                        {
                            desDict[parameter.MoveMember] = 0;
                        }
                        else
                        {
                            using (desEntry = database.Store.CreateEntry(desKey))
                            {
                                desDict = new ConcurrentDictionary<string, byte>();
                                desDict.TryAdd(parameter.MoveMember, 0);
                                desEntry.SetValue(desDict);
                                SetExpiration(desEntry, parameter.Expiration);
                            }
                        }
                    }
                }
            }
            response = new SixnetSetMoveResult()
            {
                Success = isRemove,
                CacheServer = server,
                Database = database
            };
            return Task.FromResult(response);
        }

        #endregion

        #region SetMembers

        /// <summary>
        /// Returns all the members of the set value stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return set members result</returns>
        public async Task<SixnetSetMembersResult> SetMembersAsync(SixnetCacheServer server, SixnetSetMembersParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetSetMembersResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            List<string> members = null;
            SixnetSetMembersResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, byte> dict)
                {
                    response = SixnetCacheResult.FailResponse<SixnetSetMembersResult>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                members = new List<string>(dict.Count);
                members.AddRange(dict.Keys);
            }
            members ??= new List<string>(0);
            response = SixnetCacheResult.SuccessResponse<SixnetSetMembersResult>(server, database);
            response.Members = members;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region SetLength

        /// <summary>
        /// Returns the set cardinality (number of elements) of the set stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return set length result</returns>
        public async Task<SixnetSetLengthResult> SetLengthAsync(SixnetCacheServer server, SixnetSetLengthParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetSetLengthResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var length = 0;
            SixnetSetLengthResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, byte> dict)
                {
                    response = SixnetCacheResult.FailResponse<SixnetSetLengthResult>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                length = dict.Count;
            }
            response = SixnetCacheResult.SuccessResponse<SixnetSetLengthResult>(server, database);
            response.Length = length;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region SetContains

        /// <summary>
        /// Returns if member is a member of the set stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return set contains result</returns>
        public async Task<SixnetSetContainsResult> SetContainsAsync(SixnetCacheServer server, SixnetSetContainsParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetSetContainsResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var existMember = false;
            SixnetSetContainsResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, byte> dict)
                {
                    response = SixnetCacheResult.FailResponse<SixnetSetContainsResult>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                existMember = dict.ContainsKey(parameter.Member);
            }
            response = SixnetCacheResult.SuccessResponse<SixnetSetContainsResult>(server, database);
            response.ContainsValue = existMember;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region SetCombine

        /// <summary>
        /// Returns the members of the set resulting from the specified operation against
        /// the given sets.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return set combine result</returns>
        public async Task<SixnetSetCombineResult> SetCombineAsync(SixnetCacheServer server, SixnetSetCombineParameter parameter)
        {
            if (parameter?.Keys.IsNullOrEmpty() ?? true)
            {
                return SixnetCacheResult.FailResponse<SixnetSetCombineResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            List<IEnumerable<string>> allKeyValues = new List<IEnumerable<string>>();
            foreach (var key in parameter.Keys)
            {
                var cacheKey = key?.GetActualKey() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(cacheKey))
                {
                    allKeyValues.Add(Array.Empty<string>());
                    continue;
                }
                if (database.Store.TryGetEntry(cacheKey, out var nowEntry) && nowEntry != null)
                {
                    var nowDict = nowEntry.Value as ConcurrentDictionary<string, byte>;
                    if (nowDict.IsNullOrEmpty())
                    {
                        allKeyValues.Add(Array.Empty<string>());
                    }
                    else
                    {
                        allKeyValues.Add(nowDict.Keys);
                    }
                }
                else
                {
                    allKeyValues.Add(Array.Empty<string>());
                }
            }
            IEnumerable<string> members = null;
            int keyIndex = 0;
            foreach (var keyValue in allKeyValues)
            {
                if (keyIndex++ == 0)
                {
                    members = keyValue;
                }
                switch (parameter.CombineOperation)
                {
                    case CombineOperation.Union:
                        members = members.Union(keyValue);
                        break;
                    case CombineOperation.Intersect:
                        members = members.Intersect(keyValue);
                        break;
                    case CombineOperation.Difference:
                        members = members.Except(keyValue);
                        break;
                }
            }
            members ??= Array.Empty<string>();
            var response = SixnetCacheResult.SuccessResponse<SixnetSetCombineResult>(server, database);
            response.CombineValues = members.ToList();
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region SetCombineAndStore

        /// <summary>
        /// This options is equal to SetCombine, but instead of returning the resulting set,
        ///  it is stored in destination. If destination already exists, it is overwritten.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return set combine and store result</returns>
        public Task<SixnetSetCombineAndStoreResult> SetCombineAndStoreAsync(SixnetCacheServer server, SixnetSetCombineAndStoreParameter parameter)
        {
            var desCacheKey = parameter.DestinationKey?.GetActualKey() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(desCacheKey) || (parameter?.SourceKeys.IsNullOrEmpty() ?? true))
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetSetCombineAndStoreResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var database = GetDatabase(server);
            List<string> members = null;
            SixnetSetCombineAndStoreResult response;
            foreach (var key in parameter.SourceKeys)
            {
                var cacheKey = key?.GetActualKey() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(cacheKey))
                {
                    continue;
                }
                if (database.Store.TryGetEntry(cacheKey, out var nowEntry) && nowEntry != null)
                {
                    if (nowEntry.Value is not ConcurrentDictionary<string, byte> nowDict)
                    {
                        response = SixnetCacheResult.FailResponse<SixnetSetCombineAndStoreResult>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                        return Task.FromResult(response);
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
                        switch (parameter.CombineOperation)
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
            members ??= new List<string>(0);
            lock (database)
            {
                database.Store.TryGetEntry(desCacheKey, out var desEntry);
                if (desEntry != null)
                {
                    if (desEntry.Value is not ConcurrentDictionary<string, byte> desDict)
                    {
                        response = SixnetCacheResult.FailResponse<SixnetSetCombineAndStoreResult>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                        return Task.FromResult(response);
                    }
                    foreach (var mem in members)
                    {
                        desDict[mem] = 0;
                    }
                }
                else
                {
                    using (desEntry = database.Store.CreateEntry(desCacheKey))
                    {
                        ConcurrentDictionary<string, byte> desDict = new ConcurrentDictionary<string, byte>();
                        members.ForEach(m =>
                        {
                            desDict.TryAdd(m, 0);
                        });
                        desEntry.SetValue(desDict);
                        SetExpiration(desEntry, parameter.Expiration);
                    }
                }
            }
            response = SixnetCacheResult.SuccessResponse<SixnetSetCombineAndStoreResult>(server, database);
            response.Count = members.Count;
            return Task.FromResult(response);
        }

        #endregion

        #region SetAdd

        /// <summary>
        /// Add the specified member to the set stored at key. Specified members that are
        /// already a member of this set are ignored. If key does not exist, a new set is
        /// created before adding the specified members.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return set add result</returns>
        public Task<SixnetSetAddResult> SetAddAsync(SixnetCacheServer server, SixnetSetAddParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetSetAddResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var database = GetDatabase(server);
            SixnetSetAddResult response;
            lock (database)
            {
                if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    if (entry.Value is not ConcurrentDictionary<string, byte> dict)
                    {
                        response = SixnetCacheResult.FailResponse<SixnetSetAddResult>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                        return Task.FromResult(response);
                    }
                    foreach (var member in parameter.Members)
                    {
                        dict[member] = 0;
                    }
                }
                else
                {
                    using (entry = database.Store.CreateEntry(cacheKey))
                    {
                        ConcurrentDictionary<string, byte> desDict = new ConcurrentDictionary<string, byte>();
                        foreach (var member in parameter.Members)
                        {
                            desDict[member] = 0;
                        }
                        entry.SetValue(desDict);
                        SetExpiration(entry, parameter.Expiration);
                    }
                }
            }
            response = new SixnetSetAddResult()
            {
                Success = true,
                CacheServer = server,
                Database = database
            };
            return Task.FromResult(response);
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
        /// <param name="parameter">Parameter</param>
        /// <returns>Return sorted set score result</returns>
        public async Task<SixnetSortedSetScoreResult> SortedSetScoreAsync(SixnetCacheServer server, SixnetSortedSetScoreParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetSortedSetScoreResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            double? score = null;
            SixnetSortedSetScoreResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, double> dict)
                {
                    response = SixnetCacheResult.FailResponse<SixnetSortedSetScoreResult>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                if (dict.TryGetValue(parameter.Member, out var memberScore))
                {
                    score = memberScore;
                }
            }
            response = SixnetCacheResult.SuccessResponse<SixnetSortedSetScoreResult>(server, database);
            response.Score = score;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region SortedSetRemoveRangeByValue

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order
        /// to force lexicographical ordering, this options removes all elements in the sorted
        /// set stored at key between the lexicographical range specified by min and max.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return sorted set remove range by value result</returns>
        public Task<SixnetSortedSetRemoveRangeByValueResult> SortedSetRemoveRangeByValueAsync(SixnetCacheServer server, SixnetSortedSetRemoveRangeByValueParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetSortedSetRemoveRangeByValueResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var database = GetDatabase(server);
            var removeCount = 0;
            SixnetSortedSetRemoveRangeByValueResult response;
            lock (database)
            {
                if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    if (entry.Value is not ConcurrentDictionary<string, double> dict)
                    {
                        response = SixnetCacheResult.FailResponse<SixnetSortedSetRemoveRangeByValueResult>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
                        return Task.FromResult(response);
                    }
                    var min = parameter.MinValue;
                    var max = parameter.MaxValue;
                    if (string.Compare(min, max) > 0)
                    {
                        min = max;
                        max = parameter.MinValue;
                    }
                    var removeValues = dict.Where(c =>
                    {
                        return string.Compare(c.Key, min) >= 0 && string.Compare(c.Key, max) <= 0;
                    });
                    foreach (var removeItem in removeValues)
                    {
                        switch (parameter.Exclude)
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
            }
            response = SixnetCacheResult.SuccessResponse<SixnetSortedSetRemoveRangeByValueResult>(server, database);
            response.RemoveCount = removeCount;
            return Task.FromResult(response);
        }

        #endregion

        #region SortedSetRemoveRangeByScore

        /// <summary>
        /// Removes all elements in the sorted set stored at key with a score between min
        ///  and max (inclusive by default).
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return sorted set remove range by score result</returns>
        public Task<SixnetSortedSetRemoveRangeByScoreResult> SortedSetRemoveRangeByScoreAsync(SixnetCacheServer server, SixnetSortedSetRemoveRangeByScoreParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetSortedSetRemoveRangeByScoreResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var database = GetDatabase(server);
            var removeCount = 0;
            SixnetSortedSetRemoveRangeByScoreResult response;
            lock (database)
            {
                if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    if (entry.Value is not ConcurrentDictionary<string, double> dict)
                    {
                        response = SixnetCacheResult.FailResponse<SixnetSortedSetRemoveRangeByScoreResult>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
                        return Task.FromResult(response);
                    }
                    var min = parameter.Start;
                    var max = parameter.Stop;
                    if (min > max)
                    {
                        min = max;
                        max = parameter.Start;
                    }
                    var removeValues = dict.Where(c => c.Value >= min && c.Value <= max);
                    foreach (var removeItem in removeValues)
                    {
                        switch (parameter.Exclude)
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
            }
            response = SixnetCacheResult.SuccessResponse<SixnetSortedSetRemoveRangeByScoreResult>(server, database);
            response.RemoveCount = removeCount;
            return Task.FromResult(response);
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
        /// <param name="parameter">Parameter</param>
        /// <returns>Return sorted set remove range by rank result</returns>
        public Task<SixnetSortedSetRemoveRangeByRankResult> SortedSetRemoveRangeByRankAsync(SixnetCacheServer server, SixnetSortedSetRemoveRangeByRankParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetSortedSetRemoveRangeByRankResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var database = GetDatabase(server);
            int removeCount = 0;
            SixnetSortedSetRemoveRangeByRankResult response;
            lock (database)
            {
                if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    if (entry.Value is not ConcurrentDictionary<string, double> dict)
                    {
                        response = SixnetCacheResult.FailResponse<SixnetSortedSetRemoveRangeByRankResult>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
                        return Task.FromResult(response);
                    }
                    var min = parameter.Start;
                    var max = parameter.Stop;
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
                        max = parameter.Start;
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
            }
            response = SixnetCacheResult.SuccessResponse<SixnetSortedSetRemoveRangeByRankResult>(server, database);
            response.RemoveCount = removeCount;
            return Task.FromResult(response);
        }

        #endregion

        #region SortedSetRemove

        /// <summary>
        /// Removes the specified members from the sorted set stored at key. Non existing
        /// members are ignored.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return sorted set remove result</returns>
        public Task<SixnetSortedSetRemoveResult> SortedSetRemoveAsync(SixnetCacheServer server, SixnetSortedSetRemoveParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetSortedSetRemoveResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var database = GetDatabase(server);
            var removeCount = 0;
            SixnetSortedSetRemoveResult response;
            lock (database)
            {
                if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null && !parameter.RemoveMembers.IsNullOrEmpty())
                {
                    if (entry.Value is not ConcurrentDictionary<string, double> dict)
                    {
                        response = SixnetCacheResult.FailResponse<SixnetSortedSetRemoveResult>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
                        return Task.FromResult(response);
                    }
                    foreach (var rmem in parameter.RemoveMembers)
                    {
                        if (dict.TryRemove(rmem, out var value))
                        {
                            removeCount++;
                        }
                    }
                }
            }
            response = SixnetCacheResult.SuccessResponse<SixnetSortedSetRemoveResult>(server, database);
            response.RemoveCount = removeCount;
            return Task.FromResult(response);
        }

        #endregion

        #region SortedSetRank

        /// <summary>
        /// Returns the rank of member in the sorted set stored at key, by default with the
        /// scores ordered from low to high. The rank (or index) is 0-based, which means
        /// that the member with the lowest score has rank 0.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return sorted set rank result</returns>
        public async Task<SixnetSortedSetRankResult> SortedSetRankAsync(SixnetCacheServer server, SixnetSortedSetRankParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetSortedSetRankResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            long? rank = null;
            SixnetSortedSetRankResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, double> dict)
                {
                    response = SixnetCacheResult.FailResponse<SixnetSortedSetRankResult>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                if (!dict.IsNullOrEmpty() && dict.ContainsKey(parameter.Member))
                {
                    rank = -1;
                    IOrderedEnumerable<KeyValuePair<string, double>> ranks = null;
                    if (parameter.Order == CacheOrder.Ascending)
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
                        if (item.Key == parameter.Member)
                        {
                            break;
                        }
                    }
                }
            }
            response = SixnetCacheResult.SuccessResponse<SixnetSortedSetRankResult>(server, database);
            response.Rank = rank;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region SortedSetRangeByValue

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order
        /// to force lexicographical ordering, this options returns all the elements in the
        /// sorted set at key with a value between min and max.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return sorted set range by value result</returns>
        public async Task<SixnetSortedSetRangeByValueResult> SortedSetRangeByValueAsync(SixnetCacheServer server, SixnetSortedSetRangeByValueParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetSortedSetRangeByValueResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            List<string> members = null;
            SixnetSortedSetRangeByValueResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, double>;
                if (dict == null)
                {
                    response = SixnetCacheResult.FailResponse<SixnetSortedSetRangeByValueResult>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                var min = parameter.MinValue;
                var max = parameter.MaxValue;
                if (string.Compare(min, max) > 0)
                {
                    min = max;
                    max = parameter.MinValue;
                }
                var values = dict.Where(c =>
                {
                    return parameter.Exclude switch
                    {
                        BoundaryExclude.Both => string.Compare(c.Key, min) > 0 && string.Compare(c.Key, max) < 0,
                        BoundaryExclude.Start => string.Compare(c.Key, min) > 0 && string.Compare(c.Key, max) <= 0,
                        BoundaryExclude.Stop => string.Compare(c.Key, min) >= 0 && string.Compare(c.Key, max) < 0,
                        _ => string.Compare(c.Key, min) >= 0 && string.Compare(c.Key, max) <= 0
                    };
                });
                if (parameter.Order == CacheOrder.Descending)
                {
                    values = values.OrderByDescending(c => c.Key);
                }
                else
                {
                    values = values.OrderBy(c => c.Key);
                }
                if (parameter.Offset > 0)
                {
                    values = values.Skip(parameter.Offset);
                }
                if (parameter.Count >= 0)
                {
                    values = values.Take(parameter.Count);
                }
                members = values.Select(c => c.Key).ToList();
            }
            response = SixnetCacheResult.SuccessResponse<SixnetSortedSetRangeByValueResult>(server, database);
            response.Members = members ?? new List<string>(0);
            return await Task.FromResult(response).ConfigureAwait(false);
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
        /// <param name="parameter">Parameter</param>
        /// <returns>Return sorted set range by score with scores result</returns>
        public async Task<SixnetSortedSetRangeByScoreWithScoresResult> SortedSetRangeByScoreWithScoresAsync(SixnetCacheServer server, SixnetSortedSetRangeByScoreWithScoresParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetSortedSetRangeByScoreWithScoresResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            List<SixnetSortedSetMember> members = null;
            SixnetSortedSetRangeByScoreWithScoresResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, double> dict)
                {
                    response = SixnetCacheResult.FailResponse<SixnetSortedSetRangeByScoreWithScoresResult>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                var min = parameter.Start;
                var max = parameter.Stop;
                if (min > max)
                {
                    min = max;
                    max = parameter.Start;
                }
                var values = dict.Where(c =>
                {
                    return parameter.Exclude switch
                    {
                        BoundaryExclude.Both => c.Value > min && c.Value < max,
                        BoundaryExclude.Start => c.Value > min && c.Value <= max,
                        BoundaryExclude.Stop => c.Value >= min && c.Value < max,
                        _ => c.Value >= min && c.Value <= max,
                    };
                });
                if (parameter.Order == CacheOrder.Descending)
                {
                    values = values.OrderByDescending(c => c.Value);
                }
                else
                {
                    values = values.OrderBy(c => c.Value);
                }
                if (parameter.Offset > 0)
                {
                    values = values.Skip(parameter.Offset);
                }
                if (parameter.Count >= 0)
                {
                    values = values.Take(parameter.Count);
                }
                members = values.Select(c => new SixnetSortedSetMember()
                {
                    Value = c.Key,
                    Score = c.Value
                }).ToList();
            }
            response = SixnetCacheResult.SuccessResponse<SixnetSortedSetRangeByScoreWithScoresResult>(server, database);
            response.Members = members;
            return await Task.FromResult(response).ConfigureAwait(false);
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
        /// <param name="parameter">Parameter</param>
        /// <returns>Return sorted set range by score result</returns>
        public async Task<SixnetSortedSetRangeByScoreResult> SortedSetRangeByScoreAsync(SixnetCacheServer server, SixnetSortedSetRangeByScoreParameter parameter)
        {
            var setResponse = await SortedSetRangeByScoreWithScoresAsync(server, new SixnetSortedSetRangeByScoreWithScoresParameter()
            {
                CacheObject = parameter.CacheObject,
                CommandFlags = parameter.CommandFlags,
                Exclude = parameter.Exclude,
                Key = parameter.Key,
                Order = parameter.Order,
                Offset = parameter.Offset,
                Count = parameter.Count,
                Start = parameter.Start,
                Stop = parameter.Stop
            }).ConfigureAwait(false);
            SixnetSortedSetRangeByScoreResult response;
            if (setResponse?.Success ?? false)
            {
                response = SixnetCacheResult.SuccessResponse<SixnetSortedSetRangeByScoreResult>(server, setResponse?.Database);
                response.Members = setResponse?.Members?.Select(c => c.Value).ToList() ?? new List<string>(0);
            }
            else
            {
                response = SixnetCacheResult.FailResponse<SixnetSortedSetRangeByScoreResult>(setResponse.Code, setResponse.Message);
            }
            return await Task.FromResult(response).ConfigureAwait(false);
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
        /// <param name="parameter">Parameter</param>
        /// <returns>Return sorted set range by rank with scores result</returns>
        public async Task<SixnetSortedSetRangeByRankWithScoresResult> SortedSetRangeByRankWithScoresAsync(SixnetCacheServer server, SixnetSortedSetRangeByRankWithScoresParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetSortedSetRangeByRankWithScoresResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            List<SixnetSortedSetMember> members = null;
            SixnetSortedSetRangeByRankWithScoresResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, double> dict)
                {
                    response = SixnetCacheResult.FailResponse<SixnetSortedSetRangeByRankWithScoresResult>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                var min = parameter.Start;
                var max = parameter.Stop;
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
                    max = parameter.Start;
                }
                if (min < dataCount)
                {
                    int skipCount = min;
                    int takeCount = max - min + 1;
                    IEnumerable<KeyValuePair<string, double>> valueDict = dict;
                    if (parameter.Order == CacheOrder.Descending)
                    {
                        valueDict = dict.OrderByDescending(c => c.Value);
                    }
                    else
                    {
                        valueDict = dict.OrderBy(c => c.Value);
                    }
                    var items = valueDict.Skip(skipCount).Take(takeCount < 0 ? int.MaxValue : takeCount);
                    members = items.Select(c => new SixnetSortedSetMember()
                    {
                        Score = c.Value,
                        Value = c.Key
                    }).ToList();
                }
            }
            response = SixnetCacheResult.SuccessResponse<SixnetSortedSetRangeByRankWithScoresResult>(server, database);
            response.Members = members ?? new List<SixnetSortedSetMember>(0);
            return await Task.FromResult(response).ConfigureAwait(false);
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
        /// <param name="parameter">Parameter</param>
        /// <returns>Return sorted set range by rank result</returns>
        public async Task<SixnetSortedSetRangeByRankResult> SortedSetRangeByRankAsync(SixnetCacheServer server, SixnetSortedSetRangeByRankParameter parameter)
        {
            var setResponse = await SortedSetRangeByRankWithScoresAsync(server, new SixnetSortedSetRangeByRankWithScoresParameter()
            {
                CacheObject = parameter.CacheObject,
                CommandFlags = parameter.CommandFlags,
                Key = parameter.Key,
                Order = parameter.Order,
                Start = parameter.Start,
                Stop = parameter.Stop
            }).ConfigureAwait(false);
            SixnetSortedSetRangeByRankResult response;
            if (setResponse?.Success ?? false)
            {
                response = SixnetCacheResult.SuccessResponse<SixnetSortedSetRangeByRankResult>(server, setResponse.Database);
                response.Members = setResponse.Members?.Select(c => c.Value).ToList() ?? new List<string>(0);
            }
            else
            {
                response = SixnetCacheResult.FailResponse<SixnetSortedSetRangeByRankResult>(setResponse.Code, setResponse.Message);
            }
            return await Task.FromResult(response).ConfigureAwait(false);
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
        /// <returns>Return sorted set lenght by value result</returns>
        public async Task<SixnetSortedSetLengthByValueResult> SortedSetLengthByValueAsync(SixnetCacheServer server, SixnetSortedSetLengthByValueParameter parameter)
        {
            var setResponse = await SortedSetRangeByValueAsync(server, new SixnetSortedSetRangeByValueParameter()
            {
                CacheObject = parameter.CacheObject,
                CommandFlags = parameter.CommandFlags,
                Key = parameter.Key,
                MinValue = parameter.MinValue,
                MaxValue = parameter.MaxValue,
                Offset = 0,
                Count = -1
            }).ConfigureAwait(false);
            SixnetSortedSetLengthByValueResult response;
            if (setResponse?.Success ?? false)
            {
                response = SixnetCacheResult.SuccessResponse<SixnetSortedSetLengthByValueResult>(server, setResponse.Database);
                response.Length = setResponse.Members?.Count ?? 0;
            }
            else
            {
                response = SixnetCacheResult.FailResponse<SixnetSortedSetLengthByValueResult>(setResponse.Code, setResponse.Message);
            }
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region SortedSetLength

        /// <summary>
        /// Returns the sorted set cardinality (number of elements) of the sorted set stored
        /// at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return sorted set length result</returns>
        public async Task<SixnetSortedSetLengthResult> SortedSetLengthAsync(SixnetCacheServer server, SixnetSortedSetLengthParameter parameter)
        {
            var setResponse = await SortedSetRangeByScoreAsync(server, new SixnetSortedSetRangeByScoreParameter()
            {
                CacheObject = parameter.CacheObject,
                CommandFlags = parameter.CommandFlags,
                Key = parameter.Key,
                Offset = 0,
                Count = -1,
            }).ConfigureAwait(false);
            SixnetSortedSetLengthResult response;
            if (setResponse?.Success ?? false)
            {
                response = SixnetCacheResult.SuccessResponse<SixnetSortedSetLengthResult>(server, setResponse.Database);
                response.Length = setResponse.Members?.Count ?? 0;
            }
            else
            {
                response = SixnetCacheResult.FailResponse<SixnetSortedSetLengthResult>(setResponse.Code, setResponse.Message);
            }
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region SortedSetIncrement

        /// <summary>
        /// Increments the score of member in the sorted set stored at key by increment.
        /// If member does not exist in the sorted set, it is added with increment as its
        /// score (as if its previous score was 0.0).
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return sorted set increment result</returns>
        public Task<SixnetSortedSetIncrementResult> SortedSetIncrementAsync(SixnetCacheServer server, SixnetSortedSetIncrementParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetSortedSetIncrementResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var database = GetDatabase(server);
            double score = 0;
            SixnetSortedSetIncrementResult response;
            lock (database)
            {
                if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    if (entry.Value is not ConcurrentDictionary<string, double> dict)
                    {
                        response = SixnetCacheResult.FailResponse<SixnetSortedSetIncrementResult>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
                        return Task.FromResult(response);
                    }
                    if (dict.TryGetValue(parameter.Member, out var memberScore))
                    {
                        score = memberScore + parameter.IncrementScore;
                        dict[parameter.Member] = score;
                    }
                }
            }
            response = SixnetCacheResult.SuccessResponse<SixnetSortedSetIncrementResult>(server, database);
            response.NewScore = score;
            return Task.FromResult(response);
        }

        #endregion

        #region SortedSetDecrement

        /// <summary>
        /// Decrements the score of member in the sorted set stored at key by decrement.
        /// If member does not exist in the sorted set, it is added with -decrement as its
        /// score (as if its previous score was 0.0).
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return sorted set decrement result</returns>
        public Task<SixnetSortedSetDecrementResult> SortedSetDecrementAsync(SixnetCacheServer server, SixnetSortedSetDecrementParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetSortedSetDecrementResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var database = GetDatabase(server);
            SixnetSortedSetDecrementResult response;
            double score = 0;
            lock (database)
            {
                if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    if (entry.Value is not ConcurrentDictionary<string, double> dict)
                    {
                        response = SixnetCacheResult.FailResponse<SixnetSortedSetDecrementResult>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
                        return Task.FromResult(response);
                    }
                    if (dict.TryGetValue(parameter.Member, out var memberScore))
                    {
                        score = memberScore - parameter.DecrementScore;
                        dict[parameter.Member] = score;
                    }
                }
            }
            response = SixnetCacheResult.SuccessResponse<SixnetSortedSetDecrementResult>(server, database);
            response.NewScore = score;
            return Task.FromResult(response);
        }

        #endregion

        #region SortedSetCombineAndStore

        /// <summary>
        /// Computes a set operation over multiple sorted sets (optionally using per-set
        /// weights), and stores the result in destination, optionally performing a specific
        /// aggregation (defaults to sum)
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return sorted set combine and store result</returns>
        public Task<SixnetSortedSetCombineAndStoreResult> SortedSetCombineAndStoreAsync(SixnetCacheServer server, SixnetSortedSetCombineAndStoreParameter parameter)
        {
            var desCacheKey = parameter.DestinationKey?.GetActualKey() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(desCacheKey) || (parameter?.SourceKeys.IsNullOrEmpty() ?? true))
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetSortedSetCombineAndStoreResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server));
            }
            var database = GetDatabase(server);
            HashSet<string> members = null;
            var allMembers = new Dictionary<string, List<double>>();
            SixnetSortedSetCombineAndStoreResult response;
            for (int i = 0; i < parameter.SourceKeys.Count; i++)
            {
                var key = parameter.SourceKeys[i];
                var cacheKey = key?.GetActualKey() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(cacheKey))
                {
                    continue;
                }
                if (database.Store.TryGetEntry(cacheKey, out var nowEntry) && nowEntry != null)
                {
                    var nowDict = nowEntry.Value as ConcurrentDictionary<string, double>;
                    if (nowDict == null)
                    {
                        response = SixnetCacheResult.FailResponse<SixnetSortedSetCombineAndStoreResult>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                        return Task.FromResult(response);
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
                        switch (parameter.CombineOperation)
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
                    if (parameter?.Weights?.Length >= i + 1)
                    {
                        weight = parameter.Weights[i];
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
            var resultItems = new Dictionary<string, double>();
            foreach (var member in members)
            {
                double memberScore = 0;
                if (allMembers.TryGetValue(member, out var scores) && !scores.IsNullOrEmpty())
                {
                    memberScore = parameter.Aggregate switch
                    {
                        SetAggregate.Max => scores.Max(),
                        SetAggregate.Min => scores.Min(),
                        SetAggregate.Sum => scores.Sum(),
                        _ => 0
                    };
                }
                resultItems.Add(member, memberScore);
            }
            lock (database)
            {
                if (database.Store.TryGetEntry(desCacheKey, out var desEntry) && desEntry != null)
                {
                    if (desEntry.Value is not ConcurrentDictionary<string, double> desDict)
                    {
                        response = SixnetCacheResult.FailResponse<SixnetSortedSetCombineAndStoreResult>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                        return Task.FromResult(response);
                    }
                    desEntry.Value = resultItems;
                }
                else
                {
                    using (desEntry = database.Store.CreateEntry(desCacheKey))
                    {
                        desEntry.SetValue(resultItems);
                        SetExpiration(desEntry, parameter.Expiration);
                    }
                }
            }
            response = SixnetCacheResult.SuccessResponse<SixnetSortedSetCombineAndStoreResult>(server, database);
            response.NewSetLength = resultItems.Count;
            return Task.FromResult(response);
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
        /// <param name="parameter">Parameter</param>
        /// <returns>Return sorted set add result</returns>
        public Task<SixnetSortedSetAddResult> SortedSetAddAsync(SixnetCacheServer server, SixnetSortedSetAddParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetSortedSetAddResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server));
            }
            if (parameter.Members.IsNullOrEmpty())
            {
                return Task.FromResult(SixnetCacheResult.FailResponse<SixnetSortedSetAddResult>(SixnetCacheCodes.ValuesIsNullOrEmpty, server: server));
            }
            var database = GetDatabase(server);
            long length = 0;
            SixnetSortedSetAddResult response;
            lock (database)
            {
                if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    if (entry.Value is not ConcurrentDictionary<string, double> dict)
                    {
                        response = SixnetCacheResult.FailResponse<SixnetSortedSetAddResult>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
                        return Task.FromResult(response);
                    }
                    foreach (var mem in parameter.Members)
                    {
                        dict[mem.Value] = mem.Score;
                    }
                    length = dict.Count;
                }
                else
                {
                    using (entry = database.Store.CreateEntry(cacheKey))
                    {
                        var newDict = new ConcurrentDictionary<string, double>();
                        parameter.Members.ForEach(c =>
                        {
                            newDict.TryAdd(c.Value, c.Score);
                        });
                        length = newDict.Count;
                        entry.SetValue(newDict);
                        SetExpiration(entry, parameter.Expiration);
                    }
                }
            }
            response = SixnetCacheResult.SuccessResponse<SixnetSortedSetAddResult>(server, database);
            response.Length = length;
            return Task.FromResult(response);
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
        /// <param name="parameter">Parameter</param>
        /// <returns>Return sort result</returns>
        public async Task<SixnetSortResult> SortAsync(SixnetCacheServer server, SixnetSortParameter parameter)
        {
            var keyTypeResponse = await KeyTypeAsync(server, new SixnetTypeParameter()
            {
                CacheObject = parameter.CacheObject,
                CommandFlags = parameter.CommandFlags,
                Key = parameter.Key
            }).ConfigureAwait(false);
            SixnetSortResult response;
            if (keyTypeResponse?.Success ?? false)
            {
                Func<IEnumerable<string>, IEnumerable<string>> filterValueFuc = (originalValues) =>
                {
                    if (originalValues.IsNullOrEmpty() || originalValues.Count() <= parameter.Offset)
                    {
                        return Array.Empty<string>();
                    }
                    if (parameter.Order == CacheOrder.Descending)
                    {
                        originalValues = originalValues.OrderByDescending(c => c);
                    }
                    else
                    {
                        originalValues = originalValues.OrderBy(c => c);
                    }
                    if (parameter.Offset > 0)
                    {
                        originalValues = originalValues.Skip(parameter.Offset);
                    }
                    if (parameter.Count > 0)
                    {
                        originalValues = originalValues.Take(parameter.Count);
                    }
                    return originalValues;
                };

                IEnumerable<string> values = null;
                switch (keyTypeResponse.KeyType)
                {
                    case CacheKeyType.List:
                        var listResponse = await ListRangeAsync(server, new SixnetListRangeParameter()
                        {
                            CacheObject = parameter.CacheObject,
                            CommandFlags = parameter.CommandFlags,
                            Key = parameter.Key,
                            Start = 0,
                            Stop = -1
                        }).ConfigureAwait(false);
                        values = filterValueFuc(listResponse?.Values);
                        response = new SixnetSortResult()
                        {
                            Success = true,
                            Values = values?.ToList() ?? new List<string>(0),
                            CacheServer = server,
                            Database = listResponse.Database
                        };
                        break;
                    case CacheKeyType.Set:
                        var setResponse = await SetMembersAsync(server, new SixnetSetMembersParameter()
                        {
                            CacheObject = parameter.CacheObject,
                            CommandFlags = parameter.CommandFlags,
                            Key = parameter.Key
                        }).ConfigureAwait(false);
                        values = filterValueFuc(setResponse?.Members);
                        response = new SixnetSortResult()
                        {
                            Success = true,
                            Values = values?.ToList() ?? new List<string>(0),
                            CacheServer = server,
                            Database = setResponse.Database
                        };
                        break;
                    case CacheKeyType.SortedSet:
                        var sortedSetResponse = await SortedSetRangeByRankWithScoresAsync(server, new SixnetSortedSetRangeByRankWithScoresParameter()
                        {
                            CacheObject = parameter.CacheObject,
                            CommandFlags = parameter.CommandFlags,
                            Key = parameter.Key,
                            Order = parameter.Order,
                            Start = 0,
                            Stop = -1
                        }).ConfigureAwait(false);
                        IEnumerable<SixnetSortedSetMember> sortedSetMembers = sortedSetResponse?.Members ?? new List<SixnetSortedSetMember>(0);
                        if (sortedSetMembers.GetCount() <= parameter.Offset)
                        {
                            values = Array.Empty<string>();
                        }
                        else
                        {
                            if (parameter.Offset > 0)
                            {
                                sortedSetMembers = sortedSetMembers.Skip(parameter.Offset);
                            }
                            if (parameter.Count > 0)
                            {
                                sortedSetMembers = sortedSetMembers.Take(parameter.Count);
                            }
                            values = sortedSetMembers.Select(c => c.Value).ToList();
                        }
                        response = new SixnetSortResult()
                        {
                            Success = true,
                            Values = values?.ToList() ?? new List<string>(0),
                            CacheServer = server,
                            Database = sortedSetResponse.Database
                        };
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            else
            {
                response = SixnetCacheResult.FailResponse<SixnetSortResult>(keyTypeResponse?.Code, keyTypeResponse?.Message, server: server, database: keyTypeResponse?.Database);
            }
            return await Task.FromResult(response).ConfigureAwait(false);
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
        /// <param name="parameter">Parameter</param>
        /// <returns>Return sort and store result</returns>
        public async Task<SixnetSortAndStoreResult> SortAndStoreAsync(SixnetCacheServer server, SixnetSortAndStoreParameter parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter?.SourceKey))
            {
                throw new ArgumentNullException($"{nameof(SixnetSortAndStoreParameter)}.{nameof(SixnetSortAndStoreParameter.SourceKey)}");
            }
            if (string.IsNullOrWhiteSpace(parameter?.DestinationKey))
            {
                throw new ArgumentNullException($"{nameof(SixnetSortAndStoreParameter)}.{nameof(SixnetSortAndStoreParameter.DestinationKey)}");
            }
            var sortResponse = await SortAsync(server, new SixnetSortParameter()
            {
                CacheObject = parameter.CacheObject,
                CommandFlags = parameter.CommandFlags,
                SortType = parameter.SortType,
                Count = parameter.Count,
                By = parameter.By,
                Gets = parameter.Gets,
                Key = parameter.SourceKey,
                Offset = parameter.Offset,
                Order = parameter.Order
            }).ConfigureAwait(false);
            SixnetSortAndStoreResult response;
            if (sortResponse?.Success ?? false)
            {
                var values = sortResponse?.Values;
                await ListLeftPushAsync(server, new SixnetListLeftPushParameter()
                {
                    CacheObject = parameter.CacheObject,
                    CommandFlags = parameter.CommandFlags,
                    Expiration = parameter.Expiration,
                    Key = parameter.DestinationKey,
                    Values = values
                }).ConfigureAwait(false);
                response = SixnetCacheResult.SuccessResponse<SixnetSortAndStoreResult>(server, sortResponse.Database);
            }
            else
            {
                response = SixnetCacheResult.FailResponse<SixnetSortAndStoreResult>(sortResponse.Code, sortResponse.Message);
            }
            return await Task.FromResult(response).ConfigureAwait(false);
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
        /// <param name="parameter">Parameter</param>
        /// <returns>Return key type result</returns>
        public async Task<SixnetTypeResult> KeyTypeAsync(SixnetCacheServer server, SixnetTypeParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetTypeResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            SixnetTypeResult response = null;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
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
                response = SixnetCacheResult.SuccessResponse<SixnetTypeResult>(server, database);
                response.KeyType = cacheKeyType;
            }
            else
            {
                response = SixnetCacheResult.FailResponse<SixnetTypeResult>(SixnetCacheCodes.KeyIsNotExist, server: server, database: database);
            }
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region KeyTimeToLive

        /// <summary>
        /// Returns the remaining time to live of a key that has a timeout. This introspection
        /// capability allows a Redis client to check how many seconds a given key will continue
        /// to be part of the dataset.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return key time to live result</returns>
        public async Task<SixnetTimeToLiveResult> KeyTimeToLiveAsync(SixnetCacheServer server, SixnetTimeToLiveParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetTimeToLiveResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            SixnetTimeToLiveResult response = null;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                response = SixnetCacheResult.SuccessResponse<SixnetTimeToLiveResult>();
                var expiration = GetExpiration(entry);
                response.TimeToLiveSeconds = (long)(expiration.Item2?.TotalSeconds ?? 0);
            }
            else
            {
                response = SixnetCacheResult.FailResponse<SixnetTimeToLiveResult>(SixnetCacheCodes.KeyIsNotExist);
            }
            response.CacheServer = server;
            response.Database = database;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region KeyRestore

        /// <summary>
        /// Create a key associated with a value that is obtained by deserializing the provided
        /// serialized value (obtained via DUMP). If ttl is 0 the key is created without
        /// any expire, otherwise the specified expire time(in milliseconds) is set.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return key restore result</returns>
        public async Task<SixnetRestoreResult> KeyRestoreAsync(SixnetCacheServer server, SixnetRestoreParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetRestoreResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            lock (database)
            {
                using (var entry = database.Store.CreateEntry(cacheKey))
                {
                    entry.SetValue(GetEncoding().GetString(parameter.Value));
                    SetExpiration(entry, parameter.Expiration);
                }
            }
            return await Task.FromResult(SixnetCacheResult.SuccessResponse<SixnetRestoreResult>(server, database)).ConfigureAwait(false);
        }

        #endregion

        #region KeyRename

        /// <summary>
        /// Renames key to newkey. It returns an error when the source and destination names
        /// are the same, or when key does not exist.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return key rename result</returns>
        public async Task<SixnetRenameResult> KeyRenameAsync(SixnetCacheServer server, SixnetRenameParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            string newCacheKey = parameter?.NewKey?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey) || string.IsNullOrWhiteSpace(newCacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetRenameResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            SixnetRenameResult response = null;
            lock (database)
            {
                if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    using (var newEntry = database.Store.CreateEntry(newCacheKey))
                    {
                        newEntry.SetValue(entry.Value);
                        newEntry.AbsoluteExpiration = entry.AbsoluteExpiration;
                        newEntry.AbsoluteExpirationRelativeToNow = entry.AbsoluteExpirationRelativeToNow;
                        newEntry.Priority = entry.Priority;
                        newEntry.Size = entry.Size;
                        newEntry.SlidingExpiration = entry.SlidingExpiration;
                    }
                    database.Store.Remove(cacheKey);
                    response = SixnetCacheResult.SuccessResponse<SixnetRenameResult>();
                }
                else
                {
                    response = SixnetCacheResult.FailResponse<SixnetRenameResult>(SixnetCacheCodes.KeyIsNotExist);
                }
            }
            response.CacheServer = server;
            response.Database = database;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region KeyRandom

        /// <summary>
        /// Return a random key from the currently selected database.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return key random result</returns>
        public async Task<SixnetRandomResult> KeyRandomAsync(SixnetCacheServer server, SixnetRandomParameter parameter)
        {
            var database = GetDatabase(server);
            var response = SixnetCacheResult.SuccessResponse<SixnetRandomResult>(server, database);
            response.Key = database.Store.GetRandomKey();
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region KeyPersist

        /// <summary>
        /// Remove the existing timeout on key, turning the key from volatile (a key with
        /// an expire set) to persistent (a key that will never expire as no timeout is associated).
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return key persist result</returns>
        public async Task<SixnetPersistResult> KeyPersistAsync(SixnetCacheServer server, SixnetPersistParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetPersistResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            SixnetPersistResult response = null;
            lock (database)
            {
                if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
                {
                    using (var newEntry = database.Store.CreateEntry(cacheKey))
                    {
                        newEntry.SetValue(entry.Value);
                    }
                    response = SixnetCacheResult.SuccessResponse<SixnetPersistResult>();
                }
                else
                {
                    response = SixnetCacheResult.FailResponse<SixnetPersistResult>(SixnetCacheCodes.KeyIsNotExist);
                }
            }
            response.CacheServer = server;
            response.Database = database;
            return await Task.FromResult(response).ConfigureAwait(false);
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
        /// <param name="parameter">Parameter</param>
        /// <returns>Return key move result</returns>
        public async Task<SixnetMoveResult> KeyMoveAsync(SixnetCacheServer server, SixnetMoveParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetMoveResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var desDatabase = GetDatabase(parameter.DatabaseName);
            if (desDatabase == null)
            {
                return SixnetCacheResult.FailResponse<SixnetMoveResult>("", "Destination database not find", server: server);
            }
            var database = GetDatabase(server);
            SixnetMoveResult response;
            lock (database)
            {
                if (database.Store.TryGetEntry(cacheKey, out var entry))
                {
                    using (var desEntry = desDatabase.Store.CreateEntry(cacheKey))
                    {
                        desEntry.Value = entry.Value;
                        SetExpiration(desEntry, GetCacheExpiration(entry));
                    }
                    response = SixnetCacheResult.SuccessResponse<SixnetMoveResult>(server, database);
                }
                else
                {
                    response = SixnetCacheResult.FailResponse<SixnetMoveResult>("", $"Not find key:{cacheKey}", server, database);
                }
            }
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region KeyMigrate

        /// <summary>
        /// Atomically transfer a key from a source Redis instance to a destination Redis
        /// instance. On success the key is deleted from the original instance by default,
        /// and is guaranteed to exist in the target instance.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return key migrate result</returns>
        public async Task<SixnetMigrateKeyResult> KeyMigrateAsync(SixnetCacheServer server, SixnetMigrateKeyParameter parameter)
        {
            return await Task.FromResult(SixnetCacheResult.FailResponse<SixnetMigrateKeyResult>(SixnetCacheCodes.OperationIsNotSupported, server: server)).ConfigureAwait(false);
        }

        #endregion

        #region KeyExpire

        /// <summary>
        /// Set a timeout on key. After the timeout has expired, the key will automatically
        /// be deleted. A key with an associated timeout is said to be volatile in Redis
        /// terminology.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return key expire result</returns>
        public async Task<SixnetExpireResult> KeyExpireAsync(SixnetCacheServer server, SixnetExpireParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetExpireResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            SixnetExpireResult response = null;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                SetExpiration(entry, parameter.Expiration);
                response = SixnetCacheResult.SuccessResponse<SixnetExpireResult>();
            }
            else
            {
                response = SixnetCacheResult.FailResponse<SixnetExpireResult>(SixnetCacheCodes.KeyIsNotExist);
            }
            response.CacheServer = server;
            response.Database = database;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion;

        #region KeyDump

        /// <summary>
        /// Serialize the value stored at key in a format and return it to
        /// the user. The returned value can be synthesized back into a Redis key using the
        /// RESTORE parameter.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return key dump result</returns>
        public async Task<SixnetDumpResult> KeyDumpAsync(SixnetCacheServer server, SixnetDumpParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetDumpResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            SixnetDumpResult response = null;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                response = SixnetCacheResult.SuccessResponse<SixnetDumpResult>();
                response.ByteValues = GetEncoding().GetBytes(entry.Value?.ToString() ?? string.Empty);
            }
            else
            {
                response = SixnetCacheResult.FailResponse<SixnetDumpResult>(SixnetCacheCodes.KeyIsNotExist);
            }
            response.CacheServer = server;
            response.Database = database;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region KeyDelete

        /// <summary>
        /// Removes the specified keys. A key is ignored if it does not exist.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return key delete result</returns>
        public async Task<SixnetDeleteResult> KeyDeleteAsync(SixnetCacheServer server, SixnetDeleteParameter parameter)
        {
            if (parameter.Keys?.IsNullOrEmpty() ?? true)
            {
                return SixnetCacheResult.FailResponse<SixnetDeleteResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            long deleteCount = 0;
            foreach (var key in parameter.Keys)
            {
                var cacheKey = key?.GetActualKey() ?? string.Empty;
                if (database.Store.TryGetEntry(cacheKey, out var entry))
                {
                    deleteCount++;
                    database.Store.Remove(cacheKey);
                }
            }
            var response = SixnetCacheResult.SuccessResponse<SixnetDeleteResult>(server, database);
            response.DeleteCount = deleteCount;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region KeyExists

        /// <summary>
        /// Key exists
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return exists result</returns>
        public async Task<SixnetExistResult> KeyExistAsync(SixnetCacheServer server, SixnetExistParameter parameter)
        {
            if (parameter.Keys?.IsNullOrEmpty() ?? true)
            {
                return SixnetCacheResult.FailResponse<SixnetExistResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            long count = 0;
            foreach (var key in parameter.Keys)
            {
                var cacheKey = key?.GetActualKey() ?? string.Empty;
                if (database.Store.TryGetEntry(key?.GetActualKey(), out var entry))
                {
                    count++;
                }
            }
            var response = SixnetCacheResult.SuccessResponse<SixnetExistResult>(server, database);
            response.KeyCount = count;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region KeyScan

        /// <summary>
        /// Key scan
        /// </summary>
        /// <param name="server">server</param>
        /// <param name="parameter">parameter</param>
        /// <returns></returns>
        public Task<SixnetScanResult> KeyScanAsync(SixnetCacheServer server, SixnetScanParameter parameter)
        {
            var database = GetDatabase(server);
            var keyPattern = new Regex(parameter.Pattern);
            var resultKeys = database.Store.GetAllKeys()?.Where(c => keyPattern.IsMatch(c)).ToList();
            return Task.FromResult(new SixnetScanResult()
            {
                Cursor = 0,
                Keys = resultKeys?.Select(c => { SixnetCacheKey key = ConstantCacheKey.Create(c); return key; }).ToList()
            });
        }

        #endregion

        #endregion

        #region Server

        #region Get all data base

        /// <summary>
        /// Get all database
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return get all database result</returns>
        public async Task<SixnetGetAllDataBaseResult> GetAllDataBaseAsync(SixnetCacheServer server, SixnetGetAllDataBaseParameter parameter)
        {
            var response = SixnetCacheResult.SuccessResponse<SixnetGetAllDataBaseResult>(server);
            response.Databases = MemoryCacheCollection.Select(c => c.Value as SixnetCacheDatabase).ToList();
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region Query keys

        /// <summary>
        /// Query keys
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return get keys result</returns>
        public async Task<SixnetGetKeysResult> GetKeysAsync(SixnetCacheServer server, SixnetGetKeysParameter parameter)
        {
            var database = GetDatabase(server);
            var allKeys = database.Store.GetAllKeys();
            Func<string, bool> where = c => true;
            var skip = 0;
            var count = allKeys.Count;
            if (parameter.Query != null)
            {
                skip = (parameter.Query.Page - 1) * parameter.Query.PageSize;
                count = parameter.Query.PageSize;
                switch (parameter.Query.Type)
                {
                    case KeyMatchPattern.EndWith:
                        where = c => c.EndsWith(parameter.Query.MateKey);
                        break;
                    case KeyMatchPattern.StartWith:
                        where = c => c.StartsWith(parameter.Query.MateKey);
                        break;
                    case KeyMatchPattern.Include:
                        where = c => c.Contains(parameter.Query.MateKey);
                        break;
                }
            }
            var keys = allKeys.Where(c => where(c)).Skip(skip).Take(count).Select(c => ConstantCacheKey.Create(c)).ToList();
            var response = SixnetCacheResult.SuccessResponse<SixnetGetKeysResult>(server, database);
            response.Keys = new SixnetCachePaging<SixnetCacheKey>(parameter.Query?.Page ?? 1, parameter.Query?.PageSize ?? allKeys.Count, allKeys.Count, keys);
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region Clear data

        /// <summary>
        /// Clear database data
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return clear data result</returns>
        public async Task<SixnetClearDataResult> ClearDataAsync(SixnetCacheServer server, SixnetClearDataParameter parameter)
        {
            var database = GetDatabase(server);
            lock (database)
            {
                database.Store.Compact(1);
            }
            return await Task.FromResult(SixnetCacheResult.SuccessResponse<SixnetClearDataResult>(server, database)).ConfigureAwait(false);
        }

        #endregion

        #region Get cache item detail

        /// <summary>
        /// Get cache item detail
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return get key detail result</returns>
        public async Task<SixnetGetDetailResult> GetKeyDetailAsync(SixnetCacheServer server, SixnetGetDetailParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return SixnetCacheResult.FailResponse<SixnetGetDetailResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            SixnetGetDetailResult response = null;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var cacheKeyType = CacheKeyType.String;
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
                response = SixnetCacheResult.SuccessResponse<SixnetGetDetailResult>();
                response.CacheEntry = new SixnetCacheEntry()
                {
                    Key = parameter.Key,
                    Value = entry.Value,
                    Type = cacheKeyType,
                    When = CacheSetWhen.Always,
                    Expiration = new SixnetCacheExpiration()
                    {
                        AbsoluteExpiration = entry.AbsoluteExpiration,
                        AbsoluteExpirationRelativeToNow = entry.AbsoluteExpirationRelativeToNow,
                        SlidingExpiration = entry.SlidingExpiration.HasValue
                    }
                };
            }
            else
            {
                response = SixnetCacheResult.FailResponse<SixnetGetDetailResult>(SixnetCacheCodes.KeyIsNotExist);
            }
            response.CacheServer = server;
            response.Database = database;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region Get server config

        /// <summary>
        /// Get server config
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return get server config result</returns>
        public async Task<SixnetGetServerConfigurationResult> GetServerConfigurationAsync(SixnetCacheServer server, SixnetGetServerConfigurationParameter parameter)
        {
            return await Task.FromResult(SixnetCacheResult.FailResponse<SixnetGetServerConfigurationResult>(SixnetCacheCodes.OperationIsNotSupported)).ConfigureAwait(false);
        }

        #endregion

        #region Save server configuration

        /// <summary>
        /// Save server config
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return save server config result</returns>
        public async Task<SixnetSaveServerConfigurationResult> SaveServerConfigurationAsync(SixnetCacheServer server, SixnetSaveServerConfigurationParameter parameter)
        {
            return await Task.FromResult(SixnetCacheResult.FailResponse<SixnetSaveServerConfigurationResult>(SixnetCacheCodes.OperationIsNotSupported)).ConfigureAwait(false);
        }

        #endregion

        #endregion

        #region Util

        /// <summary>
        /// Set expiration
        /// </summary>
        /// <param name="cacheEntry">Cache entry</param>
        /// <param name="expiration">Expiration</param>
        static void SetExpiration(ICacheEntry cacheEntry, SixnetCacheExpiration expiration)
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

        static SixnetCacheExpiration GetCacheExpiration(ICacheEntry cacheEntry)
        {
            var expirationOptions = GetExpiration(cacheEntry);
            var expiration = new SixnetCacheExpiration()
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
            return SixnetCacher.Encoding ?? Encoding.UTF8;
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
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                databaseName = DefaultMemoryCacheName;
            }
            MemoryCacheCollection.TryGetValue(databaseName, out var database);
            return database;
        }

        static MemoryCacheDatabase GetDatabase(SixnetCacheServer server)
        {
            return GetDatabase(server?.Database);
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
