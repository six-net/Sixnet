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
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public async Task<StringSetRangeResult> StringSetRangeAsync(CacheServer server, StringSetRangeParameter parameter)
        {
            string key = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(key))
            {
                return CacheResult.FailResponse<StringSetRangeResult>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            if (parameter.Offset < 0)
            {
                return CacheResult.FailResponse<StringSetRangeResult>(SixnetCacheCodes.OffsetLessZero);
            }
            var database = GetDatabase(server);
            if (database == null)
            {
                return CacheResult.FailResponse<StringSetRangeResult>(SixnetCacheCodes.DatabaseIsNull);
            }
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
            var response = CacheResult.SuccessResponse<StringSetRangeResult>(server, database);
            response.NewValueLength = cacheValue?.Length ?? 0;
            return await Task.FromResult(response).ConfigureAwait(false);
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
        public async Task<StringSetBitResult> StringSetBitAsync(CacheServer server, StringSetBitParameter parameter)
        {
            var key = parameter?.Key?.GetActualKey() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(key))
            {
                return CacheResult.FailResponse<StringSetBitResult>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            if (parameter.Offset < 0)
            {
                return CacheResult.FailResponse<StringSetBitResult>(SixnetCacheCodes.OffsetLessZero);
            }
            var database = GetDatabase(server);
            var found = database.Store.TryGetEntry(key, out ICacheEntry cacheEntry);
            var bitValue = parameter.Bit ? '1' : '0';
            var oldBitValue = false;
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
            var response = CacheResult.SuccessResponse<StringSetBitResult>(server, database);
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
        public async Task<StringSetResult> StringSetAsync(CacheServer server, StringSetParameter parameter)
        {
            if (parameter?.Items.IsNullOrEmpty() ?? true)
            {
                return CacheResult.FailResponse<StringSetResult>(SixnetCacheCodes.ValuesIsNullOrEmpty);
            }
            var results = new List<StringEntrySetResult>(parameter.Items.Count);
            var database = GetDatabase(server);
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
                results.Add(new StringEntrySetResult()
                {
                    Key = cacheKey,
                    SetSuccess = true
                });
            }
            var response = CacheResult.SuccessResponse<StringSetResult>(server, database);
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
        public async Task<StringLengthResult> StringLengthAsync(CacheServer server, StringLengthParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<StringLengthResult>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            var database = GetDatabase(server);
            var response = CacheResult.SuccessResponse<StringLengthResult>(server, database);
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
        public async Task<StringIncrementResult> StringIncrementAsync(CacheServer server, StringIncrementParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<StringIncrementResult>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            var database = GetDatabase(server);
            StringIncrementResult response = null;
            long nowValue = 0;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (long.TryParse(entry.Value?.ToString(), out nowValue))
                {
                    nowValue += parameter.Value;
                    entry.SetValue(nowValue);
                }
                else
                {
                    response = CacheResult.FailResponse<StringIncrementResult>(SixnetCacheCodes.ValueCannotBeCalculated, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
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
            response = CacheResult.SuccessResponse<StringIncrementResult>(server, database);
            response.NewValue = nowValue;
            return await Task.FromResult(response).ConfigureAwait(false);
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
        public async Task<StringGetWithExpiryResult> StringGetWithExpiryAsync(CacheServer server, StringGetWithExpiryParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<StringGetWithExpiryResult>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            var nowValue = string.Empty;
            TimeSpan? expriy = null;
            var database = GetDatabase(server);
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                nowValue = entry.Value?.ToString() ?? string.Empty;
                expriy = GetExpiration(entry).Item2;
            }
            var response = CacheResult.SuccessResponse<StringGetWithExpiryResult>(server, database);
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
        public async Task<StringGetSetResult> StringGetSetAsync(CacheServer server, StringGetSetParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<StringGetSetResult>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            var oldValue = string.Empty;
            var database = GetDatabase(server);
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
            var response = CacheResult.SuccessResponse<StringGetSetResult>(server, database);
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
        public async Task<StringGetRangeResult> StringGetRangeAsync(CacheServer server, StringGetRangeParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<StringGetRangeResult>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            var subValue = string.Empty;
            var database = GetDatabase(server);
            StringGetRangeResult response = null;
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
                    response = CacheResult.FailResponse<StringGetRangeResult>(SixnetCacheCodes.OffsetError, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                if (end < 0)
                {
                    end = value.Length - Math.Abs(end);
                }
                if (end < 0 || end >= valueLength)
                {
                    response = CacheResult.FailResponse<StringGetRangeResult>(SixnetCacheCodes.OffsetError, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                subValue = value.Substring(Math.Min(start, end), Math.Abs(end - start) + 1);
            }
            response = CacheResult.SuccessResponse<StringGetRangeResult>(server, database);
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
        public async Task<StringGetBitResult> StringGetBitAsync(CacheServer server, StringGetBitParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<StringGetBitResult>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            var database = GetDatabase(server);
            StringGetBitResult response = null;
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
                    response = CacheResult.FailResponse<StringGetBitResult>(SixnetCacheCodes.OffsetError, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                bit = binaryArray[offset];
            }
            response = CacheResult.SuccessResponse<StringGetBitResult>(server, database);
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
        public async Task<StringGetResult> StringGetAsync(CacheServer server, StringGetParameter parameter)
        {
            if (parameter?.Keys.IsNullOrEmpty() ?? true)
            {
                return CacheResult.FailResponse<StringGetResult>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            var database = GetDatabase(server);
            var datas = new List<CacheEntry>();
            foreach (var key in parameter.Keys)
            {
                var cacheKey = key.GetActualKey();
                if (string.IsNullOrWhiteSpace(cacheKey))
                {
                    continue;
                }
                if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
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
            var response = CacheResult.SuccessResponse<StringGetResult>(server, database);
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
        public async Task<StringDecrementResult> StringDecrementAsync(CacheServer server, StringDecrementParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<StringDecrementResult>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            var database = GetDatabase(server);
            long nowValue = 0;
            StringDecrementResult response = null;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (long.TryParse(entry.Value?.ToString(), out nowValue))
                {
                    nowValue -= parameter.Value;
                    entry.SetValue(nowValue);
                }
                else
                {
                    response = CacheResult.FailResponse<StringDecrementResult>(SixnetCacheCodes.ValueCannotBeCalculated, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
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
            response = CacheResult.SuccessResponse<StringDecrementResult>(server, database);
            response.NewValue = nowValue;
            return await Task.FromResult(response).ConfigureAwait(false);
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
        public async Task<StringBitPositionResult> StringBitPositionAsync(CacheServer server, StringBitPositionParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<StringBitPositionResult>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            if ((parameter.Start >= 0 && parameter.End < parameter.Start) || (parameter.Start < 0 && parameter.End > parameter.Start))
            {
                return CacheResult.FailResponse<StringBitPositionResult>(SixnetCacheCodes.OffsetError);
            }
            var database = GetDatabase(server);
            StringBitPositionResult response = null;
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
                    response = CacheResult.FailResponse<StringBitPositionResult>(SixnetCacheCodes.OffsetError, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                if (end < 0)
                {
                    end = length - Math.Abs(end);
                }
                if (end < 0 || end >= length)
                {
                    response = CacheResult.FailResponse<StringBitPositionResult>(SixnetCacheCodes.OffsetError, server: server, database: database);
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
            response = CacheResult.SuccessResponse<StringBitPositionResult>(server, database);
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
        public async Task<StringBitOperationResult> StringBitOperationAsync(CacheServer server, StringBitOperationParameter parameter)
        {
            if (parameter.Keys.IsNullOrEmpty() || string.IsNullOrWhiteSpace(parameter.DestinationKey))
            {
                return CacheResult.FailResponse<StringBitOperationResult>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            if (parameter.Keys.Count > 1 && parameter.Bitwise == CacheBitwise.Not)
            {
                throw new NotSupportedException($" CacheBitwise.Not can only operate on one key");
            }
            var database = GetDatabase(server);
            BitArray bitArray = null;
            StringBitOperationResult response = null;
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
                return CacheResult.FailResponse<StringBitOperationResult>(SixnetCacheCodes.ValuesIsNullOrEmpty, server: server, database: database);
            }
            var bitString = string.Join("", bitArray.Cast<bool>().Select(c => c ? 1 : 0));
            var originalString = bitString.ToOriginalString(GetEncoding());
            var setRes = await StringSetAsync(server, new StringSetParameter()
            {
                Items = new List<CacheEntry>()
                {
                    new CacheEntry()
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
                response = new StringBitOperationResult()
                {
                    Success = true,
                    DestinationValueLength = originalString.Length,
                    CacheServer = server,
                    Database = database
                };
            }
            else
            {
                response = CacheResult.FailResponse<StringBitOperationResult>(setRes.Code, setRes.Message, server, database);
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
        public async Task<StringBitCountResult> StringBitCountAsync(CacheServer server, StringBitCountParameter parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter?.Key))
            {
                throw new ArgumentNullException($"{nameof(StringBitCountParameter)}.{nameof(StringBitCountParameter.Key)}");
            }
            var cacheKey = parameter.Key.GetActualKey();
            var bitCount = 0;
            var database = GetDatabase(server);
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var value = entry.Value?.ToString() ?? string.Empty;
                bitCount = value.ToBinaryString(GetEncoding()).Count(c => c == '1');
            }
            var response = new StringBitCountResult()
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
        public async Task<StringAppendResult> StringAppendAsync(CacheServer server, StringAppendParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<StringAppendResult>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            long valueLength = 0;
            var database = GetDatabase(server);
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
            var response = CacheResult.SuccessResponse<StringAppendResult>(server, database);
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
        public async Task<ListTrimResult> ListTrimAsync(CacheServer server, ListTrimParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<ListTrimResult>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            var database = GetDatabase(server);
            ListTrimResult response = null;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not List<string> list)
                {
                    response = CacheResult.FailResponse<ListTrimResult>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
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
                    response = CacheResult.FailResponse<ListTrimResult>(SixnetCacheCodes.OffsetError, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                if (end < 0)
                {
                    end = count - Math.Abs(end);
                }
                if (end < 0 || end >= count)
                {
                    response = CacheResult.FailResponse<ListTrimResult>(SixnetCacheCodes.OffsetError, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                var begin = Math.Min(start, end);
                var takeCount = Math.Abs(end - start) + 1;
                var nowList = list.Skip(begin).Take(takeCount).ToList();
                entry.SetValue(nowList);
                response = CacheResult.SuccessResponse<ListTrimResult>();
            }
            else
            {
                response = CacheResult.FailResponse<ListTrimResult>(SixnetCacheCodes.KeyIsNotExist);
            }
            response.CacheServer = server;
            response.Database = database;
            return await Task.FromResult(response).ConfigureAwait(false);
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
        public async Task<ListSetByIndexResult> ListSetByIndexAsync(CacheServer server, ListSetByIndexParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<ListSetByIndexResult>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            var database = GetDatabase(server);
            ListSetByIndexResult response = null;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not List<string> list)
                {
                    response = CacheResult.FailResponse<ListSetByIndexResult>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                var index = parameter.Index;
                if (index < 0)
                {
                    index = list.Count - Math.Abs(index);
                }
                if (index < 0 || index >= list.Count)
                {
                    response = CacheResult.FailResponse<ListSetByIndexResult>(SixnetCacheCodes.OffsetError, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                list[index] = parameter.Value;
                response = CacheResult.SuccessResponse<ListSetByIndexResult>();
            }
            else
            {
                response = CacheResult.FailResponse<ListSetByIndexResult>(SixnetCacheCodes.KeyIsNotExist);
            }
            response.CacheServer = server;
            response.Database = database;
            return await Task.FromResult(response).ConfigureAwait(false);
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
        public async Task<ListRightPushResult> ListRightPushAsync(CacheServer server, ListRightPushParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<ListRightPushResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            if (parameter.Values.IsNullOrEmpty())
            {
                return CacheResult.FailResponse<ListRightPushResult>(SixnetCacheCodes.ValuesIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            ListRightPushResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (!(entry.Value is List<string> list))
                {
                    response = CacheResult.FailResponse<ListRightPushResult>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                list = list.Concat(parameter.Values).ToList();
                entry.SetValue(list);
                response = CacheResult.SuccessResponse<ListRightPushResult>();
                response.NewListLength = list.Count;
            }
            else
            {
                using (entry = database.Store.CreateEntry(cacheKey))
                {
                    entry.SetValue(new List<string>(parameter.Values));
                    SetExpiration(entry, parameter.Expiration);
                }
                response = CacheResult.SuccessResponse<ListRightPushResult>();
                response.NewListLength = parameter.Values.Count;
            }
            response.CacheServer = server;
            response.Database = database;
            return await Task.FromResult(response).ConfigureAwait(false);
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
        public async Task<ListRightPopLeftPushResult> ListRightPopLeftPushAsync(CacheServer server, ListRightPopLeftPushParameter parameter)
        {
            var sourceCacheKey = parameter?.SourceKey?.GetActualKey();
            var destionationCacheKey = parameter?.DestinationKey?.GetActualKey();
            if (string.IsNullOrWhiteSpace(sourceCacheKey) || string.IsNullOrWhiteSpace(destionationCacheKey))
            {
                return CacheResult.FailResponse<ListRightPopLeftPushResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            ListRightPopLeftPushResult response = null;
            if (database.Store.TryGetEntry(sourceCacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not List<string> list)
                {
                    response = CacheResult.FailResponse<ListRightPopLeftPushResult>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                if (list.Count < 1)
                {
                    response = CacheResult.FailResponse<ListRightPopLeftPushResult>(SixnetCacheCodes.ListIsEmpty, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                List<string> desList = null;
                if (database.Store.TryGetEntry(destionationCacheKey, out var desEntry) && desEntry != null)
                {
                    desList = desEntry.Value as List<string>;
                    if (desList == null)
                    {
                        response = CacheResult.FailResponse<ListRightPopLeftPushResult>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                        return await Task.FromResult(response).ConfigureAwait(false);
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
                response = CacheResult.SuccessResponse<ListRightPopLeftPushResult>();
                response.PopValue = value;
            }
            else
            {
                response = CacheResult.FailResponse<ListRightPopLeftPushResult>(SixnetCacheCodes.KeyIsNotExist);
            }
            response.CacheServer = server;
            response.Database = database;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region ListRightPop

        /// <summary>
        /// Removes and returns the last element of the list stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return list right pop result</returns>
        public async Task<ListRightPopResult> ListRightPopAsync(CacheServer server, ListRightPopParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<ListRightPopResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            ListRightPopResult response = null;
            var value = string.Empty;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (!(entry.Value is List<string> list))
                {
                    response = CacheResult.FailResponse<ListRightPopResult>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                if (list.Count < 1)
                {
                    response = CacheResult.FailResponse<ListRightPopResult>(SixnetCacheCodes.ListIsEmpty, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                var index = list.Count - 1;
                value = list[index];
                list.RemoveAt(index);
                response = CacheResult.SuccessResponse<ListRightPopResult>();
                response.PopValue = value;
            }
            else
            {
                response = CacheResult.FailResponse<ListRightPopResult>(SixnetCacheCodes.KeyIsNotExist);
            }
            response.CacheServer = server;
            response.Database = database;
            return await Task.FromResult(response).ConfigureAwait(false);
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
        public async Task<ListRemoveResult> ListRemoveAsync(CacheServer server, ListRemoveParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<ListRemoveResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            ListRemoveResult response = null;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not List<string> list)
                {
                    response = CacheResult.FailResponse<ListRemoveResult>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
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
                response = CacheResult.SuccessResponse<ListRemoveResult>();
                response.RemoveCount = removeCount;
            }
            else
            {
                response = CacheResult.FailResponse<ListRemoveResult>(SixnetCacheCodes.KeyIsNotExist);
            }
            response.CacheServer = server;
            response.Database = database;
            return await Task.FromResult(response).ConfigureAwait(false);
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
        public async Task<ListRangeResult> ListRangeAsync(CacheServer server, ListRangeParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<ListRangeResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            ListRangeResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not List<string> list)
                {
                    response = CacheResult.FailResponse<ListRangeResult>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                var start = parameter.Start;
                if (start < 0)
                {
                    start = list.Count - Math.Abs(start);
                }
                if (start < 0 || start >= list.Count)
                {
                    response = CacheResult.FailResponse<ListRangeResult>(SixnetCacheCodes.OffsetError, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                var end = parameter.Stop;
                if (end < 0)
                {
                    end = list.Count - Math.Abs(end);
                }
                if (end < 0 || end >= list.Count)
                {
                    response = CacheResult.FailResponse<ListRangeResult>(SixnetCacheCodes.OffsetError, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                var begin = Math.Min(start, end);
                response = CacheResult.SuccessResponse<ListRangeResult>();
                response.Values = list.GetRange(begin, Math.Abs(end - start) + 1).ToList();
            }
            else
            {
                response = CacheResult.FailResponse<ListRangeResult>(SixnetCacheCodes.KeyIsNotExist);
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
        public async Task<ListLengthResult> ListLengthAsync(CacheServer server, ListLengthParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<ListLengthResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var length = 0;
            ListLengthResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not List<string> list)
                {
                    response = CacheResult.FailResponse<ListLengthResult>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                length = list.Count;
            }
            response = CacheResult.SuccessResponse<ListLengthResult>();
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
        public async Task<ListLeftPushResult> ListLeftPushAsync(CacheServer server, ListLeftPushParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<ListLeftPushResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            if (parameter.Values.IsNullOrEmpty())
            {
                return CacheResult.FailResponse<ListLeftPushResult>(SixnetCacheCodes.ValuesIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            ListLeftPushResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not List<string> list)
                {
                    response = CacheResult.FailResponse<ListLeftPushResult>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                list = parameter.Values.Concat(list).ToList();
                entry.SetValue(list);
                response = CacheResult.SuccessResponse<ListLeftPushResult>();
                response.NewListLength = list.Count;
            }
            else
            {
                using (entry = database.Store.CreateEntry(cacheKey))
                {
                    entry.SetValue(new List<string>(parameter.Values));
                    SetExpiration(entry, parameter.Expiration);
                }
                response = CacheResult.SuccessResponse<ListLeftPushResult>();
                response.NewListLength = parameter.Values.Count;
            }
            response.CacheServer = server;
            response.Database = database;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region ListLeftPop

        /// <summary>
        /// Removes and returns the first element of the list stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return list left pop result</returns>
        public async Task<ListLeftPopResult> ListLeftPopAsync(CacheServer server, ListLeftPopParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<ListLeftPopResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            ListLeftPopResult response = null;
            string value = string.Empty;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not List<string> list)
                {
                    response = CacheResult.FailResponse<ListLeftPopResult>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                if (list.Count < 1)
                {
                    response = CacheResult.FailResponse<ListLeftPopResult>(SixnetCacheCodes.ListIsEmpty, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                value = list[0];
                list.RemoveAt(0);
                entry.SetValue(list);
                response = CacheResult.SuccessResponse<ListLeftPopResult>();
                response.PopValue = value;
            }
            else
            {
                response = CacheResult.FailResponse<ListLeftPopResult>(SixnetCacheCodes.KeyIsNotExist);
            }
            response.CacheServer = server;
            response.Database = database;
            return await Task.FromResult(response).ConfigureAwait(false);
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
        public async Task<ListInsertBeforeResult> ListInsertBeforeAsync(CacheServer server, ListInsertBeforeParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<ListInsertBeforeResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            int newLength = 0;
            bool hasInsertValue = false;
            ListInsertBeforeResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not List<string> list)
                {
                    response = CacheResult.FailResponse<ListInsertBeforeResult>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
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
            response = new ListInsertBeforeResult()
            {
                Success = hasInsertValue,
                NewListLength = newLength,
                CacheServer = server,
                Database = database
            };
            return await Task.FromResult(response).ConfigureAwait(false);
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
        public async Task<ListInsertAfterResult> ListInsertAfterAsync(CacheServer server, ListInsertAfterParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<ListInsertAfterResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var newLength = 0;
            var hasInsertValue = false;
            ListInsertAfterResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var list = entry.Value as List<string>;
                if (list == null)
                {
                    response = CacheResult.FailResponse<ListInsertAfterResult>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
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
            response = new ListInsertAfterResult()
            {
                NewListLength = newLength,
                Success = hasInsertValue,
                CacheServer = server,
                Database = database
            };
            return await Task.FromResult(response).ConfigureAwait(false);
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
        public async Task<ListGetByIndexResult> ListGetByIndexAsync(CacheServer server, ListGetByIndexParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<ListGetByIndexResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var value = "";
            ListGetByIndexResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not List<string> list)
                {
                    response = CacheResult.FailResponse<ListGetByIndexResult>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                var index = parameter.Index;
                if (index < 0)
                {
                    index = list.Count - Math.Abs(index);
                }
                if (index < 0 || index >= list.Count)
                {
                    response = CacheResult.FailResponse<ListGetByIndexResult>(SixnetCacheCodes.OffsetError, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                value = list[index];
            }
            response = CacheResult.SuccessResponse<ListGetByIndexResult>();
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
        public async Task<HashValuesResult> HashValuesAsync(CacheServer server, HashValuesParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<HashValuesResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            List<dynamic> values = null;
            HashValuesResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, dynamic> dict)
                {
                    response = CacheResult.FailResponse<HashValuesResult>(SixnetCacheCodes.ValueIsNotDict, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                values = dict.Values.ToList();
            }
            values ??= new List<dynamic>(0);
            response = CacheResult.SuccessResponse<HashValuesResult>(server, database);
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
        public async Task<HashSetResult> HashSetAsync(CacheServer server, HashSetParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<HashSetResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            HashSetResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, dynamic> dict)
                {
                    response = CacheResult.FailResponse<HashSetResult>(SixnetCacheCodes.ValueIsNotDict, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
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
            response = CacheResult.SuccessResponse<HashSetResult>(server, database);
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region HashLength

        /// <summary>
        /// Returns the number of fields contained in the hash stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return hash length result</returns>
        public async Task<HashLengthResult> HashLengthAsync(CacheServer server, HashLengthParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<HashLengthResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            int length = 0;
            HashLengthResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, dynamic> dict)
                {
                    response = CacheResult.FailResponse<HashLengthResult>(SixnetCacheCodes.ValueIsNotDict, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                length = dict.Keys.Count;
            }
            response = CacheResult.SuccessResponse<HashLengthResult>(server, database);
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
        public async Task<HashKeysResult> HashKeysAsync(CacheServer server, HashKeysParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<HashKeysResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            HashKeysResult response;
            List<string> keys = null;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, dynamic> dict)
                {
                    response = CacheResult.FailResponse<HashKeysResult>(SixnetCacheCodes.ValueIsNotDict, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                keys = dict.Keys.ToList();
            }
            keys ??= new List<string>(0);
            response = CacheResult.SuccessResponse<HashKeysResult>(server, database);
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
        public async Task<HashIncrementResult> HashIncrementAsync(CacheServer server, HashIncrementParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<HashIncrementResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var newValue = parameter.IncrementValue;
            HashIncrementResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, dynamic> dict)
                {
                    response = CacheResult.FailResponse<HashIncrementResult>(SixnetCacheCodes.ValueIsNotDict, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
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
            response = CacheResult.SuccessResponse<HashIncrementResult>(server, database);
            response.HashField = parameter.HashField;
            response.NewValue = newValue;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region HashGet

        /// <summary>
        /// Returns the value associated with field in the hash stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return hash get result</returns>
        public async Task<HashGetResult> HashGetAsync(CacheServer server, HashGetParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<HashGetResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            dynamic value = null;
            HashGetResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, dynamic> dict)
                {
                    response = CacheResult.FailResponse<HashGetResult>(SixnetCacheCodes.ValueIsNotDict, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                dict.TryGetValue(parameter.HashField, out value);
            }
            response = CacheResult.SuccessResponse<HashGetResult>(server, database);
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
        public async Task<HashGetAllResult> HashGetAllAsync(CacheServer server, HashGetAllParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<HashGetAllResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            ConcurrentDictionary<string, dynamic> values = null;
            HashGetAllResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, dynamic> dict)
                {
                    response = CacheResult.FailResponse<HashGetAllResult>(SixnetCacheCodes.ValueIsNotDict, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false); ;
                }
                values = new ConcurrentDictionary<string, dynamic>(dict);
            }
            response = CacheResult.SuccessResponse<HashGetAllResult>(server, database);
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
        public async Task<HashExistsResult> HashExistAsync(CacheServer server, HashExistsParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<HashExistsResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var existKey = false;
            HashExistsResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, dynamic> dict)
                {
                    response = CacheResult.FailResponse<HashExistsResult>(SixnetCacheCodes.ValueIsNotDict, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                existKey = dict.ContainsKey(parameter.HashField);
            }
            response = CacheResult.SuccessResponse<HashExistsResult>(server, database);
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
        public async Task<HashDeleteResult> HashDeleteAsync(CacheServer server, HashDeleteParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<HashDeleteResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            HashDeleteResult response;
            var remove = false;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, dynamic> dict)
                {
                    response = CacheResult.FailResponse<HashDeleteResult>(SixnetCacheCodes.ValueIsNotDict, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                foreach (var field in parameter.HashFields)
                {
                    remove |= dict.TryRemove(field, out var value);
                }
            }
            response = new HashDeleteResult()
            {
                Success = remove,
                CacheServer = server,
                Database = database
            };
            return await Task.FromResult(response).ConfigureAwait(false);
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
        public async Task<HashDecrementResult> HashDecrementAsync(CacheServer server, HashDecrementParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                CacheResult.FailResponse<HashDecrementResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var newValue = parameter.DecrementValue;
            HashDecrementResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, dynamic> dict)
                {
                    response = CacheResult.FailResponse<HashDecrementResult>(SixnetCacheCodes.ValueIsNotDict, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
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
            response = CacheResult.SuccessResponse<HashDecrementResult>(server, database);
            response.HashField = parameter.HashField;
            response.NewValue = newValue;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region HashScan

        /// <summary>
        /// The HSCAN options is used to incrementally iterate over a hash
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return hash scan result</returns>
        public async Task<HashScanResult> HashScanAsync(CacheServer server, HashScanParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                CacheResult.FailResponse<HashScanResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var values = new Dictionary<string, dynamic>();
            HashScanResult response;
            if (parameter.PageSize > 0 && database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, dynamic>;
                if (dict == null)
                {
                    response = CacheResult.FailResponse<HashScanResult>(SixnetCacheCodes.ValueIsNotDict, server: server, database: database);
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
            response = CacheResult.SuccessResponse<HashScanResult>(server, database);
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
        public async Task<SetRemoveResult> SetRemoveAsync(CacheServer server, SetRemoveParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<SetRemoveResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var removeCount = 0;
            SetRemoveResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null && !parameter.RemoveMembers.IsNullOrEmpty())
            {
                if (entry.Value is not ConcurrentDictionary<string, byte> dict)
                {
                    response = CacheResult.FailResponse<SetRemoveResult>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                foreach (var member in parameter.RemoveMembers)
                {
                    if (dict.TryRemove(member, out var value))
                    {
                        removeCount++;
                    }
                }
            }
            response = CacheResult.SuccessResponse<SetRemoveResult>(server, database);
            response.RemoveCount = removeCount;
            return await Task.FromResult(response).ConfigureAwait(false);
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
        public async Task<SetRandomMembersResult> SetRandomMembersAsync(CacheServer server, SetRandomMembersParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<SetRandomMembersResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var members = new List<string>();
            SetRandomMembersResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null && parameter.Count != 0)
            {
                if (entry.Value is not ConcurrentDictionary<string, byte> dict)
                {
                    response = CacheResult.FailResponse<SetRandomMembersResult>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                var allowSame = parameter.Count < 0;
                var count = Math.Abs(parameter.Count);
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
                    var shuffle = new SixnetShuffleNet<string>(keys);
                    members.AddRange(shuffle.TakeNextValues(count));
                }
            }
            response = CacheResult.SuccessResponse<SetRandomMembersResult>(server, database);
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
        public async Task<SetRandomMemberResult> SetRandomMemberAsync(CacheServer server, SetRandomMemberParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<SetRandomMemberResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var member = string.Empty;
            SetRandomMemberResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, byte> dict)
                {
                    response = CacheResult.FailResponse<SetRandomMemberResult>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                var keys = dict.Keys;
                if (!keys.IsNullOrEmpty())
                {
                    var ranIndex = RandomNumberHelper.GetRandomNumber(keys.Count - 1);
                    member = keys.ElementAt(ranIndex);
                }
            }
            response = CacheResult.SuccessResponse<SetRandomMemberResult>(server, database);
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
        public async Task<SetPopResult> SetPopAsync(CacheServer server, SetPopParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<SetPopResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var member = string.Empty;
            SetPopResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, byte> dict)
                {
                    response = CacheResult.FailResponse<SetPopResult>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                var keys = dict.Keys;
                if (!keys.IsNullOrEmpty())
                {
                    var ranIndex = RandomNumberHelper.GetRandomNumber(keys.Count - 1);
                    member = keys.ElementAt(ranIndex);
                    dict.TryRemove(member, out var value);
                }
            }
            response = CacheResult.SuccessResponse<SetPopResult>(server, database);
            response.PopValue = member;
            return await Task.FromResult(response).ConfigureAwait(false);
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
        public async Task<SetMoveResult> SetMoveAsync(CacheServer server, SetMoveParameter parameter)
        {
            var cacheKey = parameter?.SourceKey?.GetActualKey();
            var desKey = parameter?.DestinationKey?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey) || string.IsNullOrWhiteSpace(desKey))
            {
                return CacheResult.FailResponse<SetMoveResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var isRemove = false;
            SetMoveResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, byte> dict)
                {
                    response = CacheResult.FailResponse<SetMoveResult>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                database.Store.TryGetEntry(desKey, out var desEntry);
                ConcurrentDictionary<string, byte> desDict = null;
                if (desEntry != null)
                {
                    desDict = desEntry.Value as ConcurrentDictionary<string, byte>;
                    if (desDict == null)
                    {
                        response = CacheResult.FailResponse<SetMoveResult>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                        return await Task.FromResult(response).ConfigureAwait(false);
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
            response = new SetMoveResult()
            {
                Success = isRemove,
                CacheServer = server,
                Database = database
            };
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region SetMembers

        /// <summary>
        /// Returns all the members of the set value stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return set members result</returns>
        public async Task<SetMembersResult> SetMembersAsync(CacheServer server, SetMembersParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<SetMembersResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            List<string> members = null;
            SetMembersResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, byte> dict)
                {
                    response = CacheResult.FailResponse<SetMembersResult>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                members = new List<string>(dict.Count);
                members.AddRange(dict.Keys);
            }
            members ??= new List<string>(0);
            response = CacheResult.SuccessResponse<SetMembersResult>(server, database);
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
        public async Task<SetLengthResult> SetLengthAsync(CacheServer server, SetLengthParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<SetLengthResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var length = 0;
            SetLengthResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, byte> dict)
                {
                    response = CacheResult.FailResponse<SetLengthResult>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                length = dict.Count;
            }
            response = CacheResult.SuccessResponse<SetLengthResult>(server, database);
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
        public async Task<SetContainsResult> SetContainsAsync(CacheServer server, SetContainsParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<SetContainsResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var existMember = false;
            SetContainsResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, byte> dict)
                {
                    response = CacheResult.FailResponse<SetContainsResult>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                existMember = dict.ContainsKey(parameter.Member);
            }
            response = CacheResult.SuccessResponse<SetContainsResult>(server, database);
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
        public async Task<SetCombineResult> SetCombineAsync(CacheServer server, SetCombineParameter parameter)
        {
            if (parameter?.Keys.IsNullOrEmpty() ?? true)
            {
                return CacheResult.FailResponse<SetCombineResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
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
            var response = CacheResult.SuccessResponse<SetCombineResult>(server, database);
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
        public async Task<SetCombineAndStoreResult> SetCombineAndStoreAsync(CacheServer server, SetCombineAndStoreParameter parameter)
        {
            var desCacheKey = parameter.DestinationKey?.GetActualKey() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(desCacheKey) || (parameter?.SourceKeys.IsNullOrEmpty() ?? true))
            {
                return CacheResult.FailResponse<SetCombineAndStoreResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            List<string> members = null;
            SetCombineAndStoreResult response;
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
                        response = CacheResult.FailResponse<SetCombineAndStoreResult>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                        return await Task.FromResult(response).ConfigureAwait(false);
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
            database.Store.TryGetEntry(desCacheKey, out var desEntry);
            if (desEntry != null)
            {
                if (desEntry.Value is not ConcurrentDictionary<string, byte> desDict)
                {
                    response = CacheResult.FailResponse<SetCombineAndStoreResult>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
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
            response = CacheResult.SuccessResponse<SetCombineAndStoreResult>(server, database);
            response.Count = members.Count;
            return await Task.FromResult(response).ConfigureAwait(false);
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
        public async Task<SetAddResult> SetAddAsync(CacheServer server, SetAddParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<SetAddResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            SetAddResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, byte> dict)
                {
                    response = CacheResult.FailResponse<SetAddResult>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
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
            response = new SetAddResult()
            {
                Success = true,
                CacheServer = server,
                Database = database
            };
            return await Task.FromResult(response).ConfigureAwait(false);
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
        public async Task<SortedSetScoreResult> SortedSetScoreAsync(CacheServer server, SortedSetScoreParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<SortedSetScoreResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            double? score = null;
            SortedSetScoreResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, double> dict)
                {
                    response = CacheResult.FailResponse<SortedSetScoreResult>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                if (dict.TryGetValue(parameter.Member, out var memberScore))
                {
                    score = memberScore;
                }
            }
            response = CacheResult.SuccessResponse<SortedSetScoreResult>(server, database);
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
        public async Task<SortedSetRemoveRangeByValueResult> SortedSetRemoveRangeByValueAsync(CacheServer server, SortedSetRemoveRangeByValueParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<SortedSetRemoveRangeByValueResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var removeCount = 0;
            SortedSetRemoveRangeByValueResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, double> dict)
                {
                    response = CacheResult.FailResponse<SortedSetRemoveRangeByValueResult>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
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
            response = CacheResult.SuccessResponse<SortedSetRemoveRangeByValueResult>(server, database);
            response.RemoveCount = removeCount;
            return await Task.FromResult(response).ConfigureAwait(false);
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
        public async Task<SortedSetRemoveRangeByScoreResult> SortedSetRemoveRangeByScoreAsync(CacheServer server, SortedSetRemoveRangeByScoreParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<SortedSetRemoveRangeByScoreResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var removeCount = 0;
            SortedSetRemoveRangeByScoreResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, double> dict)
                {
                    response = CacheResult.FailResponse<SortedSetRemoveRangeByScoreResult>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
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
            response = CacheResult.SuccessResponse<SortedSetRemoveRangeByScoreResult>(server, database);
            response.RemoveCount = removeCount;
            return await Task.FromResult(response).ConfigureAwait(false);
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
        public async Task<SortedSetRemoveRangeByRankResult> SortedSetRemoveRangeByRankAsync(CacheServer server, SortedSetRemoveRangeByRankParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<SortedSetRemoveRangeByRankResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            int removeCount = 0;
            SortedSetRemoveRangeByRankResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, double> dict)
                {
                    response = CacheResult.FailResponse<SortedSetRemoveRangeByRankResult>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
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
            response = CacheResult.SuccessResponse<SortedSetRemoveRangeByRankResult>(server, database);
            response.RemoveCount = removeCount;
            return await Task.FromResult(response).ConfigureAwait(false);
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
        public async Task<SortedSetRemoveResult> SortedSetRemoveAsync(CacheServer server, SortedSetRemoveParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<SortedSetRemoveResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var removeCount = 0;
            SortedSetRemoveResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null && !parameter.RemoveMembers.IsNullOrEmpty())
            {
                if (entry.Value is not ConcurrentDictionary<string, double> dict)
                {
                    response = CacheResult.FailResponse<SortedSetRemoveResult>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                foreach (var rmem in parameter.RemoveMembers)
                {
                    if (dict.TryRemove(rmem, out var value))
                    {
                        removeCount++;
                    }
                }
            }
            response = CacheResult.SuccessResponse<SortedSetRemoveResult>(server, database);
            response.RemoveCount = removeCount;
            return await Task.FromResult(response).ConfigureAwait(false);
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
        public async Task<SortedSetRankResult> SortedSetRankAsync(CacheServer server, SortedSetRankParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<SortedSetRankResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            long? rank = null;
            SortedSetRankResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, double> dict)
                {
                    response = CacheResult.FailResponse<SortedSetRankResult>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
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
            response = CacheResult.SuccessResponse<SortedSetRankResult>(server, database);
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
        public async Task<SortedSetRangeByValueResult> SortedSetRangeByValueAsync(CacheServer server, SortedSetRangeByValueParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<SortedSetRangeByValueResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            List<string> members = null;
            SortedSetRangeByValueResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, double>;
                if (dict == null)
                {
                    response = CacheResult.FailResponse<SortedSetRangeByValueResult>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
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
            response = CacheResult.SuccessResponse<SortedSetRangeByValueResult>(server, database);
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
        public async Task<SortedSetRangeByScoreWithScoresResult> SortedSetRangeByScoreWithScoresAsync(CacheServer server, SortedSetRangeByScoreWithScoresParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<SortedSetRangeByScoreWithScoresResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            List<SortedSetMember> members = null;
            SortedSetRangeByScoreWithScoresResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, double> dict)
                {
                    response = CacheResult.FailResponse<SortedSetRangeByScoreWithScoresResult>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
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
                members = values.Select(c => new SortedSetMember()
                {
                    Value = c.Key,
                    Score = c.Value
                }).ToList();
            }
            response = CacheResult.SuccessResponse<SortedSetRangeByScoreWithScoresResult>(server, database);
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
        public async Task<SortedSetRangeByScoreResult> SortedSetRangeByScoreAsync(CacheServer server, SortedSetRangeByScoreParameter parameter)
        {
            var setResponse = await SortedSetRangeByScoreWithScoresAsync(server, new SortedSetRangeByScoreWithScoresParameter()
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
            SortedSetRangeByScoreResult response;
            if (setResponse?.Success ?? false)
            {
                response = CacheResult.SuccessResponse<SortedSetRangeByScoreResult>(server, setResponse?.Database);
                response.Members = setResponse?.Members?.Select(c => c.Value).ToList() ?? new List<string>(0);
            }
            else
            {
                response = CacheResult.FailResponse<SortedSetRangeByScoreResult>(setResponse.Code, setResponse.Message);
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
        public async Task<SortedSetRangeByRankWithScoresResult> SortedSetRangeByRankWithScoresAsync(CacheServer server, SortedSetRangeByRankWithScoresParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<SortedSetRangeByRankWithScoresResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            List<SortedSetMember> members = null;
            SortedSetRangeByRankWithScoresResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, double> dict)
                {
                    response = CacheResult.FailResponse<SortedSetRangeByRankWithScoresResult>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
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
                    members = items.Select(c => new SortedSetMember()
                    {
                        Score = c.Value,
                        Value = c.Key
                    }).ToList();
                }
            }
            response = CacheResult.SuccessResponse<SortedSetRangeByRankWithScoresResult>(server, database);
            response.Members = members ?? new List<SortedSetMember>(0);
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
        public async Task<SortedSetRangeByRankResult> SortedSetRangeByRankAsync(CacheServer server, SortedSetRangeByRankParameter parameter)
        {
            var setResponse = await SortedSetRangeByRankWithScoresAsync(server, new SortedSetRangeByRankWithScoresParameter()
            {
                CacheObject = parameter.CacheObject,
                CommandFlags = parameter.CommandFlags,
                Key = parameter.Key,
                Order = parameter.Order,
                Start = parameter.Start,
                Stop = parameter.Stop
            }).ConfigureAwait(false);
            SortedSetRangeByRankResult response;
            if (setResponse?.Success ?? false)
            {
                response = CacheResult.SuccessResponse<SortedSetRangeByRankResult>(server, setResponse.Database);
                response.Members = setResponse.Members?.Select(c => c.Value).ToList() ?? new List<string>(0);
            }
            else
            {
                response = CacheResult.FailResponse<SortedSetRangeByRankResult>(setResponse.Code, setResponse.Message);
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
        public async Task<SortedSetLengthByValueResult> SortedSetLengthByValueAsync(CacheServer server, SortedSetLengthByValueParameter parameter)
        {
            var setResponse = await SortedSetRangeByValueAsync(server, new SortedSetRangeByValueParameter()
            {
                CacheObject = parameter.CacheObject,
                CommandFlags = parameter.CommandFlags,
                Key = parameter.Key,
                MinValue = parameter.MinValue,
                MaxValue = parameter.MaxValue,
                Offset = 0,
                Count = -1
            }).ConfigureAwait(false);
            SortedSetLengthByValueResult response;
            if (setResponse?.Success ?? false)
            {
                response = CacheResult.SuccessResponse<SortedSetLengthByValueResult>(server, setResponse.Database);
                response.Length = setResponse.Members?.Count ?? 0;
            }
            else
            {
                response = CacheResult.FailResponse<SortedSetLengthByValueResult>(setResponse.Code, setResponse.Message);
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
        public async Task<SortedSetLengthResult> SortedSetLengthAsync(CacheServer server, SortedSetLengthParameter parameter)
        {
            var setResponse = await SortedSetRangeByScoreAsync(server, new SortedSetRangeByScoreParameter()
            {
                CacheObject = parameter.CacheObject,
                CommandFlags = parameter.CommandFlags,
                Key = parameter.Key,
                Offset = 0,
                Count = -1,
            }).ConfigureAwait(false);
            SortedSetLengthResult response;
            if (setResponse?.Success ?? false)
            {
                response = CacheResult.SuccessResponse<SortedSetLengthResult>(server, setResponse.Database);
                response.Length = setResponse.Members?.Count ?? 0;
            }
            else
            {
                response = CacheResult.FailResponse<SortedSetLengthResult>(setResponse.Code, setResponse.Message);
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
        public async Task<SortedSetIncrementResult> SortedSetIncrementAsync(CacheServer server, SortedSetIncrementParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<SortedSetIncrementResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            double score = 0;
            SortedSetIncrementResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, double> dict)
                {
                    response = CacheResult.FailResponse<SortedSetIncrementResult>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                if (dict.TryGetValue(parameter.Member, out var memberScore))
                {
                    score = memberScore + parameter.IncrementScore;
                    dict[parameter.Member] = score;
                }
            }
            response = CacheResult.SuccessResponse<SortedSetIncrementResult>(server, database);
            response.NewScore = score;
            return await Task.FromResult(response).ConfigureAwait(false);
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
        public async Task<SortedSetDecrementResult> SortedSetDecrementAsync(CacheServer server, SortedSetDecrementParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<SortedSetDecrementResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            SortedSetDecrementResult response;
            double score = 0;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, double> dict)
                {
                    response = CacheResult.FailResponse<SortedSetDecrementResult>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
                }
                if (dict.TryGetValue(parameter.Member, out var memberScore))
                {
                    score = memberScore - parameter.DecrementScore;
                    dict[parameter.Member] = score;
                }
            }
            response = CacheResult.SuccessResponse<SortedSetDecrementResult>(server, database);
            response.NewScore = score;
            return await Task.FromResult(response).ConfigureAwait(false);
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
        public async Task<SortedSetCombineAndStoreResult> SortedSetCombineAndStoreAsync(CacheServer server, SortedSetCombineAndStoreParameter parameter)
        {
            var desCacheKey = parameter.DestinationKey?.GetActualKey() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(desCacheKey) || (parameter?.SourceKeys.IsNullOrEmpty() ?? true))
            {
                return CacheResult.FailResponse<SortedSetCombineAndStoreResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            HashSet<string> members = null;
            var allMembers = new Dictionary<string, List<double>>();
            SortedSetCombineAndStoreResult response;
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
                        response = CacheResult.FailResponse<SortedSetCombineAndStoreResult>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                        return await Task.FromResult(response).ConfigureAwait(false);
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
            if (database.Store.TryGetEntry(desCacheKey, out var desEntry) && desEntry != null)
            {
                if (desEntry.Value is not ConcurrentDictionary<string, double> desDict)
                {
                    response = CacheResult.FailResponse<SortedSetCombineAndStoreResult>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
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
            response = CacheResult.SuccessResponse<SortedSetCombineAndStoreResult>(server, database);
            response.NewSetLength = resultItems.Count;
            return await Task.FromResult(response).ConfigureAwait(false);
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
        public async Task<SortedSetAddResult> SortedSetAddAsync(CacheServer server, SortedSetAddParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<SortedSetAddResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            if (parameter.Members.IsNullOrEmpty())
            {
                return CacheResult.FailResponse<SortedSetAddResult>(SixnetCacheCodes.ValuesIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            long length = 0;
            SortedSetAddResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, double> dict)
                {
                    response = CacheResult.FailResponse<SortedSetAddResult>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
                    return await Task.FromResult(response).ConfigureAwait(false);
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
            response = CacheResult.SuccessResponse<SortedSetAddResult>(server, database);
            response.Length = length;
            return await Task.FromResult(response).ConfigureAwait(false);
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
        public async Task<SortResult> SortAsync(CacheServer server, SortParameter parameter)
        {
            var keyTypeResponse = await KeyTypeAsync(server, new TypeParameter()
            {
                CacheObject = parameter.CacheObject,
                CommandFlags = parameter.CommandFlags,
                Key = parameter.Key
            }).ConfigureAwait(false);
            SortResult response;
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
                        var listResponse = await ListRangeAsync(server, new ListRangeParameter()
                        {
                            CacheObject = parameter.CacheObject,
                            CommandFlags = parameter.CommandFlags,
                            Key = parameter.Key,
                            Start = 0,
                            Stop = -1
                        }).ConfigureAwait(false);
                        values = filterValueFuc(listResponse?.Values);
                        response = new SortResult()
                        {
                            Success = true,
                            Values = values?.ToList() ?? new List<string>(0),
                            CacheServer = server,
                            Database = listResponse.Database
                        };
                        break;
                    case CacheKeyType.Set:
                        var setResponse = await SetMembersAsync(server, new SetMembersParameter()
                        {
                            CacheObject = parameter.CacheObject,
                            CommandFlags = parameter.CommandFlags,
                            Key = parameter.Key
                        }).ConfigureAwait(false);
                        values = filterValueFuc(setResponse?.Members);
                        response = new SortResult()
                        {
                            Success = true,
                            Values = values?.ToList() ?? new List<string>(0),
                            CacheServer = server,
                            Database = setResponse.Database
                        };
                        break;
                    case CacheKeyType.SortedSet:
                        var sortedSetResponse = await SortedSetRangeByRankWithScoresAsync(server, new SortedSetRangeByRankWithScoresParameter()
                        {
                            CacheObject = parameter.CacheObject,
                            CommandFlags = parameter.CommandFlags,
                            Key = parameter.Key,
                            Order = parameter.Order,
                            Start = 0,
                            Stop = -1
                        }).ConfigureAwait(false);
                        IEnumerable<SortedSetMember> sortedSetMembers = sortedSetResponse?.Members ?? new List<SortedSetMember>(0);
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
                        response = new SortResult()
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
                response = CacheResult.FailResponse<SortResult>(keyTypeResponse?.Code, keyTypeResponse?.Message, server: server, database: keyTypeResponse?.Database);
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
        public async Task<SortAndStoreResult> SortAndStoreAsync(CacheServer server, SortAndStoreParameter parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter?.SourceKey))
            {
                throw new ArgumentNullException($"{nameof(SortAndStoreParameter)}.{nameof(SortAndStoreParameter.SourceKey)}");
            }
            if (string.IsNullOrWhiteSpace(parameter?.DestinationKey))
            {
                throw new ArgumentNullException($"{nameof(SortAndStoreParameter)}.{nameof(SortAndStoreParameter.DestinationKey)}");
            }
            var sortResponse = await SortAsync(server, new SortParameter()
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
            SortAndStoreResult response;
            if (sortResponse?.Success ?? false)
            {
                var values = sortResponse?.Values;
                await ListLeftPushAsync(server, new ListLeftPushParameter()
                {
                    CacheObject = parameter.CacheObject,
                    CommandFlags = parameter.CommandFlags,
                    Expiration = parameter.Expiration,
                    Key = parameter.DestinationKey,
                    Values = values
                }).ConfigureAwait(false);
                response = CacheResult.SuccessResponse<SortAndStoreResult>(server, sortResponse.Database);
            }
            else
            {
                response = CacheResult.FailResponse<SortAndStoreResult>(sortResponse.Code, sortResponse.Message);
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
        public async Task<TypeResult> KeyTypeAsync(CacheServer server, TypeParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<TypeResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            TypeResult response = null;
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
                response = CacheResult.SuccessResponse<TypeResult>(server, database);
                response.KeyType = cacheKeyType;
            }
            else
            {
                response = CacheResult.FailResponse<TypeResult>(SixnetCacheCodes.KeyIsNotExist, server: server, database: database);
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
        public async Task<TimeToLiveResult> KeyTimeToLiveAsync(CacheServer server, TimeToLiveParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<TimeToLiveResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            TimeToLiveResult response = null;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                response = CacheResult.SuccessResponse<TimeToLiveResult>();
                var expiration = GetExpiration(entry);
                response.TimeToLiveSeconds = (long)(expiration.Item2?.TotalSeconds ?? 0);
            }
            else
            {
                response = CacheResult.FailResponse<TimeToLiveResult>(SixnetCacheCodes.KeyIsNotExist);
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
        public async Task<RestoreResult> KeyRestoreAsync(CacheServer server, RestoreParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<RestoreResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            using (var entry = database.Store.CreateEntry(cacheKey))
            {
                entry.SetValue(GetEncoding().GetString(parameter.Value));
                SetExpiration(entry, parameter.Expiration);
            }
            return await Task.FromResult(CacheResult.SuccessResponse<RestoreResult>(server, database)).ConfigureAwait(false);
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
        public async Task<RenameResult> KeyRenameAsync(CacheServer server, RenameParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            string newCacheKey = parameter?.NewKey?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey) || string.IsNullOrWhiteSpace(newCacheKey))
            {
                return CacheResult.FailResponse<RenameResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            RenameResult response = null;
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
                response = CacheResult.SuccessResponse<RenameResult>();
            }
            else
            {
                response = CacheResult.FailResponse<RenameResult>(SixnetCacheCodes.KeyIsNotExist);
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
        public async Task<RandomResult> KeyRandomAsync(CacheServer server, RandomParameter parameter)
        {
            var database = GetDatabase(server);
            var response = CacheResult.SuccessResponse<RandomResult>(server, database);
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
        public async Task<PersistResult> KeyPersistAsync(CacheServer server, PersistParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<PersistResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            PersistResult response = null;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                using (var newEntry = database.Store.CreateEntry(cacheKey))
                {
                    newEntry.SetValue(entry.Value);
                }
                response = CacheResult.SuccessResponse<PersistResult>();
            }
            else
            {
                response = CacheResult.FailResponse<PersistResult>(SixnetCacheCodes.KeyIsNotExist);
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
        public async Task<MoveResult> KeyMoveAsync(CacheServer server, MoveParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<MoveResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var desDatabase = GetDatabase(parameter.DatabaseName);
            if (desDatabase == null)
            {
                return CacheResult.FailResponse<MoveResult>("", "Destination database not find", server: server);
            }
            var database = GetDatabase(server);
            MoveResult response;
            if (database.Store.TryGetEntry(cacheKey, out var entry))
            {
                using (var desEntry = desDatabase.Store.CreateEntry(cacheKey))
                {
                    desEntry.Value = entry.Value;
                    SetExpiration(desEntry, GetCacheExpiration(entry));
                }
                response = CacheResult.SuccessResponse<MoveResult>(server, database);
            }
            else
            {
                response = CacheResult.FailResponse<MoveResult>("", $"Not find key:{cacheKey}", server, database);
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
        public async Task<MigrateKeyResult> KeyMigrateAsync(CacheServer server, MigrateKeyParameter parameter)
        {
            return await Task.FromResult(CacheResult.FailResponse<MigrateKeyResult>(SixnetCacheCodes.OperationIsNotSupported, server: server)).ConfigureAwait(false);
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
        public async Task<ExpireResult> KeyExpireAsync(CacheServer server, ExpireParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<ExpireResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            ExpireResult response = null;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                SetExpiration(entry, parameter.Expiration);
                response = CacheResult.SuccessResponse<ExpireResult>();
            }
            else
            {
                response = CacheResult.FailResponse<ExpireResult>(SixnetCacheCodes.KeyIsNotExist);
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
        public async Task<DumpResult> KeyDumpAsync(CacheServer server, DumpParameter parameter)
        {
            var cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<DumpResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            DumpResult response = null;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                response = CacheResult.SuccessResponse<DumpResult>();
                response.ByteValues = GetEncoding().GetBytes(entry.Value?.ToString() ?? string.Empty);
            }
            else
            {
                response = CacheResult.FailResponse<DumpResult>(SixnetCacheCodes.KeyIsNotExist);
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
        public async Task<DeleteResult> KeyDeleteAsync(CacheServer server, DeleteParameter parameter)
        {
            if (parameter.Keys?.IsNullOrEmpty() ?? true)
            {
                return CacheResult.FailResponse<DeleteResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
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
            var response = CacheResult.SuccessResponse<DeleteResult>(server, database);
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
        public async Task<ExistResult> KeyExistAsync(CacheServer server, ExistParameter parameter)
        {
            if (parameter.Keys?.IsNullOrEmpty() ?? true)
            {
                return CacheResult.FailResponse<ExistResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
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
            var response = CacheResult.SuccessResponse<ExistResult>(server, database);
            response.KeyCount = count;
            return await Task.FromResult(response).ConfigureAwait(false);
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
        public async Task<GetAllDataBaseResult> GetAllDataBaseAsync(CacheServer server, GetAllDataBaseParameter parameter)
        {
            var response = CacheResult.SuccessResponse<GetAllDataBaseResult>(server);
            response.Databases = MemoryCacheCollection.Select(c => c.Value as CacheDatabase).ToList();
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
        public async Task<GetKeysResult> GetKeysAsync(CacheServer server, GetKeysParameter parameter)
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
            var response = CacheResult.SuccessResponse<GetKeysResult>(server, database);
            response.Keys = new CachePaging<CacheKey>(parameter.Query?.Page ?? 1, parameter.Query?.PageSize ?? allKeys.Count, allKeys.Count, keys);
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
        public async Task<ClearDataResult> ClearDataAsync(CacheServer server, ClearDataParameter parameter)
        {
            var database = GetDatabase(server);
            database.Store.Compact(1);
            return await Task.FromResult(CacheResult.SuccessResponse<ClearDataResult>(server, database)).ConfigureAwait(false);
        }

        #endregion

        #region Get cache item detail

        /// <summary>
        /// Get cache item detail
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return get key detail result</returns>
        public async Task<GetDetailResult> GetKeyDetailAsync(CacheServer server, GetDetailParameter parameter)
        {
            string cacheKey = parameter?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResult.FailResponse<GetDetailResult>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            GetDetailResult response = null;
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
                response = CacheResult.SuccessResponse<GetDetailResult>();
                response.CacheEntry = new CacheEntry()
                {
                    Key = parameter.Key,
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
                response = CacheResult.FailResponse<GetDetailResult>(SixnetCacheCodes.KeyIsNotExist);
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
        public async Task<GetServerConfigurationResult> GetServerConfigurationAsync(CacheServer server, GetServerConfigurationParameter parameter)
        {
            return await Task.FromResult(CacheResult.FailResponse<GetServerConfigurationResult>(SixnetCacheCodes.OperationIsNotSupported)).ConfigureAwait(false);
        }

        #endregion

        #region Save server configuration

        /// <summary>
        /// Save server config
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return save server config result</returns>
        public async Task<SaveServerConfigurationResult> SaveServerConfigurationAsync(CacheServer server, SaveServerConfigurationParameter parameter)
        {
            return await Task.FromResult(CacheResult.FailResponse<SaveServerConfigurationResult>(SixnetCacheCodes.OperationIsNotSupported)).ConfigureAwait(false);
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

        static MemoryCacheDatabase GetDatabase(CacheServer server)
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
