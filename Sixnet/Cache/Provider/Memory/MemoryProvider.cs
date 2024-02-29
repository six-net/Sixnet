using Sixnet.Algorithm.Selection;
using Sixnet.Cache.Hash.Options;
using Sixnet.Cache.Hash.Response;
using Sixnet.Cache.Keys.Options;
using Sixnet.Cache.Keys.Response;
using Sixnet.Cache.List.Options;
using Sixnet.Cache.List.Response;
using Sixnet.Cache.Provider.Memory.Abstractions;
using Sixnet.Cache.Server.Options;
using Sixnet.Cache.Server.Response;
using Sixnet.Cache.Set.Options;
using Sixnet.Cache.Set.Response;
using Sixnet.Cache.SortedSet;
using Sixnet.Cache.SortedSet.Options;
using Sixnet.Cache.SortedSet.Response;
using Sixnet.Cache.String;
using Sixnet.Cache.String.Response;
using Sixnet.Code;
using Sixnet.Exceptions;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Sixnet.Cache.Provider.Memory
{
    /// <summary>
    /// In memory cache provider
    /// </summary>
    public partial class MemoryProvider : ISixnetCacheProvider
    {
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
        public StringSetRangeResponse StringSetRange(CacheServer server, StringSetRangeOptions options)
        {
            string key = options?.Key?.GetActualKey();

            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(key), $"{nameof(StringSetRangeOptions)}.{nameof(StringSetRangeOptions.Key)}");
            SixnetDirectThrower.ThrowArgErrorIf(options.Offset < 0, $"{nameof(StringSetRangeOptions)}.{nameof(StringSetRangeOptions.Offset)} can't be less than 0");
            if (options.Offset < 0)
            {
                return CacheResponse.FailResponse<StringSetRangeResponse>(SixnetCacheCodes.OffsetLessZero);
            }
            var database = GetDatabase(server);
            if (database == null)
            {
                return CacheResponse.FailResponse<StringSetRangeResponse>(SixnetCacheCodes.DatabaseIsNull);
            }
            var found = database.Store.TryGetEntry(key, out ICacheEntry cacheEntry);
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
                using (var newEntry = database.Store.CreateEntry(key))
                {
                    newEntry.Value = cacheValue;
                    SetExpiration(newEntry, options.Expiration);
                }
            }
            var response = CacheResponse.SuccessResponse<StringSetRangeResponse>(server, database);
            response.NewValueLength = cacheValue?.Length ?? 0;
            return response;
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
        public StringSetBitResponse StringSetBit(CacheServer server, StringSetBitOptions options)
        {
            var key = options?.Key?.GetActualKey() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(key))
            {
                return CacheResponse.FailResponse<StringSetBitResponse>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            if (options.Offset < 0)
            {
                return CacheResponse.FailResponse<StringSetBitResponse>(SixnetCacheCodes.OffsetLessZero);
            }
            var database = GetDatabase(server);
            var found = database.Store.TryGetEntry(key, out ICacheEntry cacheEntry);
            var bitValue = options.Bit ? '1' : '0';
            var oldBitValue = false;
            var cacheValue = found ? cacheEntry?.Value?.ToString() ?? string.Empty : string.Empty;

            var binaryValue = cacheValue.ToBinaryString(GetEncoding());
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
                using (var entry = database.Store.CreateEntry(key))
                {
                    entry.Value = cacheValue;
                    SetExpiration(entry, options.Expiration);
                }
            }
            var response = CacheResponse.SuccessResponse<StringSetBitResponse>(server, database);
            response.OldBitValue = oldBitValue;
            return response;
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
        public StringSetResponse StringSet(CacheServer server, StringSetOptions options)
        {
            if (options?.Items.IsNullOrEmpty() ?? true)
            {
                return CacheResponse.FailResponse<StringSetResponse>(SixnetCacheCodes.ValuesIsNullOrEmpty);
            }
            var results = new List<StringEntrySetResult>(options.Items.Count);
            var database = GetDatabase(server);
            foreach (var data in options.Items)
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
                lock (database)
                {
                    found = database.Store.TryGetEntry(cacheKey, out nowEntry);
                    setCache = data.When == CacheSetWhen.Always
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
            }
            var response = CacheResponse.SuccessResponse<StringSetResponse>(server, database);
            response.Results = results;
            return response;

        }

        #endregion

        #region StringLength

        /// <summary>
        /// Returns the length of the string value stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return string length response</returns>
        public StringLengthResponse StringLength(CacheServer server, StringLengthOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<StringLengthResponse>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            var database = GetDatabase(server);
            var response = CacheResponse.SuccessResponse<StringLengthResponse>(server, database);
            if (database.Store.TryGetValue<string>(cacheKey, out var value))
            {
                response.Length = value?.Length ?? 0;
            }
            return response;

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
        public StringIncrementResponse StringIncrement(CacheServer server, StringIncrementOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<StringIncrementResponse>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            var database = GetDatabase(server);
            StringIncrementResponse response = null;
            long nowValue = 0;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (long.TryParse(entry.Value?.ToString(), out nowValue))
                {
                    nowValue += options.Value;
                    entry.SetValue(nowValue);
                }
                else
                {
                    response = CacheResponse.FailResponse<StringIncrementResponse>(SixnetCacheCodes.ValueCannotBeCalculated, server: server, database: database);
                    return response;
                }
            }
            else
            {
                using (entry = database.Store.CreateEntry(cacheKey))
                {
                    nowValue = options.Value;
                    entry.Value = options.Value;
                    SetExpiration(entry, options.Expiration);
                }
            }
            response = CacheResponse.SuccessResponse<StringIncrementResponse>(server, database);
            response.NewValue = nowValue;
            return response;
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
        public StringGetWithExpiryResponse StringGetWithExpiry(CacheServer server, StringGetWithExpiryOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<StringGetWithExpiryResponse>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            var nowValue = string.Empty;
            TimeSpan? expriy = null;
            var database = GetDatabase(server);
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                nowValue = entry.Value?.ToString() ?? string.Empty;
                expriy = GetExpiration(entry).Item2;
            }
            var response = CacheResponse.SuccessResponse<StringGetWithExpiryResponse>(server, database);
            response.Value = nowValue;
            response.Expiry = expriy;
            return response;
        }

        #endregion

        #region StringGetSet

        /// <summary>
        /// Atomically sets key to value and returns the old value stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return string get set response</returns>
        public StringGetSetResponse StringGetSet(CacheServer server, StringGetSetOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<StringGetSetResponse>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            var oldValue = string.Empty;
            var database = GetDatabase(server);
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                oldValue = entry.Value?.ToString() ?? string.Empty;
                entry.SetValue(options.NewValue);
            }
            else
            {
                using (entry = database.Store.CreateEntry(cacheKey))
                {
                    entry.SetValue(options.NewValue);
                }
            }
            var response = CacheResponse.SuccessResponse<StringGetSetResponse>(server, database);
            response.OldValue = oldValue;
            return response;
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
        public StringGetRangeResponse StringGetRange(CacheServer server, StringGetRangeOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<StringGetRangeResponse>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            var subValue = string.Empty;
            var database = GetDatabase(server);
            StringGetRangeResponse response = null;
            if (database.Store.TryGetValue<string>(cacheKey, out var value) && !string.IsNullOrWhiteSpace(value))
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
                    response = CacheResponse.FailResponse<StringGetRangeResponse>(SixnetCacheCodes.OffsetError, server: server, database: database);
                    return response;
                }
                if (end < 0)
                {
                    end = value.Length - Math.Abs(end);
                }
                if (end < 0 || end >= valueLength)
                {
                    response = CacheResponse.FailResponse<StringGetRangeResponse>(SixnetCacheCodes.OffsetError, server: server, database: database);
                    return response;
                }
                subValue = value.Substring(Math.Min(start, end), Math.Abs(end - start) + 1);
            }
            response = CacheResponse.SuccessResponse<StringGetRangeResponse>(server, database);
            response.Value = subValue;
            return response;
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
        public StringGetBitResponse StringGetBit(CacheServer server, StringGetBitOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<StringGetBitResponse>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            var database = GetDatabase(server);
            StringGetBitResponse response = null;
            char bit = '0';
            if (database.Store.TryGetValue<string>(cacheKey, out var value) && !string.IsNullOrWhiteSpace(value))
            {
                var binaryArray = value.ToBinaryString(GetEncoding()).ToCharArray();
                var offset = options.Offset;
                if (offset < 0)
                {
                    offset = binaryArray.LongLength - Math.Abs(offset);
                }
                if (offset < 0 || offset >= binaryArray.LongLength)
                {
                    response = CacheResponse.FailResponse<StringGetBitResponse>(SixnetCacheCodes.OffsetError, server: server, database: database);
                    return response;
                }
                bit = binaryArray[offset];
            }
            response = CacheResponse.SuccessResponse<StringGetBitResponse>(server, database);
            response.Bit = bit == '1';
            return response;
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
        public StringGetResponse StringGet(CacheServer server, StringGetOptions options)
        {
            if (options?.Keys.IsNullOrEmpty() ?? true)
            {
                return CacheResponse.FailResponse<StringGetResponse>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            var database = GetDatabase(server);
            var datas = new List<CacheEntry>();
            foreach (var key in options.Keys)
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
            var response = CacheResponse.SuccessResponse<StringGetResponse>(server, database);
            response.Values = datas;
            return response;
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
        public StringDecrementResponse StringDecrement(CacheServer server, StringDecrementOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<StringDecrementResponse>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            var database = GetDatabase(server);
            long nowValue = 0;
            StringDecrementResponse response = null;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (long.TryParse(entry.Value?.ToString(), out nowValue))
                {
                    nowValue -= options.Value;
                    entry.SetValue(nowValue);
                }
                else
                {
                    response = CacheResponse.FailResponse<StringDecrementResponse>(SixnetCacheCodes.ValueCannotBeCalculated, server: server, database: database);
                    return response;
                }
            }
            else
            {
                using (entry = database.Store.CreateEntry(cacheKey))
                {
                    nowValue = options.Value;
                    entry.Value = options.Value;
                    SetExpiration(entry, options.Expiration);
                }
            }
            response = CacheResponse.SuccessResponse<StringDecrementResponse>(server, database);
            response.NewValue = nowValue;
            return response;
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
        public StringBitPositionResponse StringBitPosition(CacheServer server, StringBitPositionOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<StringBitPositionResponse>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            if ((options.Start >= 0 && options.End < options.Start) || (options.Start < 0 && options.End > options.Start))
            {
                return CacheResponse.FailResponse<StringBitPositionResponse>(SixnetCacheCodes.OffsetError);
            }
            var database = GetDatabase(server);
            StringBitPositionResponse response = null;
            bool hasValue = false;
            long position = 0;
            if (database.Store.TryGetValue<string>(cacheKey, out var value) && !string.IsNullOrWhiteSpace(value))
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
                    response = CacheResponse.FailResponse<StringBitPositionResponse>(SixnetCacheCodes.OffsetError, server: server, database: database);
                    return response;
                }
                if (end < 0)
                {
                    end = length - Math.Abs(end);
                }
                if (end < 0 || end >= length)
                {
                    response = CacheResponse.FailResponse<StringBitPositionResponse>(SixnetCacheCodes.OffsetError, server: server, database: database);
                    return response;
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
            response = CacheResponse.SuccessResponse<StringBitPositionResponse>(server, database);
            response.HasValue = hasValue;
            response.Position = position;
            return response;
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
        public StringBitOperationResponse StringBitOperation(CacheServer server, StringBitOperationOptions options)
        {
            if (options.Keys.IsNullOrEmpty() || string.IsNullOrWhiteSpace(options.DestinationKey))
            {
                return CacheResponse.FailResponse<StringBitOperationResponse>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            if (options.Keys.Count > 1 && options.Bitwise == CacheBitwise.Not)
            {
                throw new NotSupportedException($" CacheBitwise.Not can only operate on one key");
            }
            var database = GetDatabase(server);
            BitArray bitArray = null;
            StringBitOperationResponse response = null;
            foreach (var key in options.Keys)
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
                return CacheResponse.FailResponse<StringBitOperationResponse>(SixnetCacheCodes.ValuesIsNullOrEmpty, server: server, database: database);
            }
            var bitString = string.Join("", bitArray.Cast<bool>().Select(c => c ? 1 : 0));
            var originalString = bitString.ToOriginalString(GetEncoding());
            var setRes = StringSet(server, new StringSetOptions()
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
            });
            if (setRes?.Success ?? false)
            {
                response = new StringBitOperationResponse()
                {
                    Success = true,
                    DestinationValueLength = originalString.Length,
                    CacheServer = server,
                    Database = database
                };
            }
            else
            {
                response = CacheResponse.FailResponse<StringBitOperationResponse>(setRes.Code, setRes.Message, server, database);
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
        /// <param name="options">Options</param>
        /// <returns>Return string bit count response</returns>
        public StringBitCountResponse StringBitCount(CacheServer server, StringBitCountOptions options)
        {
            if (string.IsNullOrWhiteSpace(options?.Key))
            {
                throw new ArgumentNullException($"{nameof(StringBitCountOptions)}.{nameof(StringBitCountOptions.Key)}");
            }
            var cacheKey = options.Key.GetActualKey();
            var bitCount = 0;
            var database = GetDatabase(server);
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var value = entry.Value?.ToString() ?? string.Empty;
                bitCount = value.ToBinaryString(GetEncoding()).Count(c => c == '1');
            }
            var response = new StringBitCountResponse()
            {
                Success = true,
                BitNum = bitCount
            };
            return response;
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
        public StringAppendResponse StringAppend(CacheServer server, StringAppendOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<StringAppendResponse>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            long valueLength = 0;
            var database = GetDatabase(server);
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var nowValue = entry.Value?.ToString() ?? string.Empty;
                nowValue += options.Value ?? string.Empty;
                valueLength = nowValue.Length;
                entry.SetValue(nowValue);
            }
            else
            {
                using (entry = database.Store.CreateEntry(cacheKey))
                {
                    entry.SetValue(options.Value);
                    SetExpiration(entry, options.Expiration);
                }
            }
            var response = CacheResponse.SuccessResponse<StringAppendResponse>(server, database);
            response.NewValueLength = valueLength;
            return response;
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
        public ListTrimResponse ListTrim(CacheServer server, ListTrimOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<ListTrimResponse>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            var database = GetDatabase(server);
            ListTrimResponse response = null;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not List<string> list)
                {
                    response = CacheResponse.FailResponse<ListTrimResponse>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                    return response;
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
                    response = CacheResponse.FailResponse<ListTrimResponse>(SixnetCacheCodes.OffsetError, server: server, database: database);
                    return response;
                }
                if (end < 0)
                {
                    end = count - Math.Abs(end);
                }
                if (end < 0 || end >= count)
                {
                    response = CacheResponse.FailResponse<ListTrimResponse>(SixnetCacheCodes.OffsetError, server: server, database: database);
                    return response;
                }
                var begin = Math.Min(start, end);
                var takeCount = Math.Abs(end - start) + 1;
                var nowList = list.Skip(begin).Take(takeCount).ToList();
                entry.SetValue(nowList);
                response = CacheResponse.SuccessResponse<ListTrimResponse>();
            }
            else
            {
                response = CacheResponse.FailResponse<ListTrimResponse>(SixnetCacheCodes.KeyIsNotExist);
            }
            response.CacheServer = server;
            response.Database = database;
            return response;
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
        public ListSetByIndexResponse ListSetByIndex(CacheServer server, ListSetByIndexOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<ListSetByIndexResponse>(SixnetCacheCodes.KeyIsNullOrEmpty);
            }
            var database = GetDatabase(server);
            ListSetByIndexResponse response = null;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not List<string> list)
                {
                    response = CacheResponse.FailResponse<ListSetByIndexResponse>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                    return response;
                }
                var index = options.Index;
                if (index < 0)
                {
                    index = list.Count - Math.Abs(index);
                }
                if (index < 0 || index >= list.Count)
                {
                    response = CacheResponse.FailResponse<ListSetByIndexResponse>(SixnetCacheCodes.OffsetError, server: server, database: database);
                    return response;
                }
                list[index] = options.Value;
                response = CacheResponse.SuccessResponse<ListSetByIndexResponse>();
            }
            else
            {
                response = CacheResponse.FailResponse<ListSetByIndexResponse>(SixnetCacheCodes.KeyIsNotExist);
            }
            response.CacheServer = server;
            response.Database = database;
            return response;
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
        public ListRightPushResponse ListRightPush(CacheServer server, ListRightPushOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<ListRightPushResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            if (options.Values.IsNullOrEmpty())
            {
                return CacheResponse.FailResponse<ListRightPushResponse>(SixnetCacheCodes.ValuesIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            ListRightPushResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (!(entry.Value is List<string> list))
                {
                    response = CacheResponse.FailResponse<ListRightPushResponse>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                    return response;
                }
                list = list.Concat(options.Values).ToList();
                entry.SetValue(list);
                response = CacheResponse.SuccessResponse<ListRightPushResponse>();
                response.NewListLength = list.Count;
            }
            else
            {
                using (entry = database.Store.CreateEntry(cacheKey))
                {
                    entry.SetValue(new List<string>(options.Values));
                    SetExpiration(entry, options.Expiration);
                }
                response = CacheResponse.SuccessResponse<ListRightPushResponse>();
                response.NewListLength = options.Values.Count;
            }
            response.CacheServer = server;
            response.Database = database;
            return response;
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
        public ListRightPopLeftPushResponse ListRightPopLeftPush(CacheServer server, ListRightPopLeftPushOptions options)
        {
            var sourceCacheKey = options?.SourceKey?.GetActualKey();
            var destionationCacheKey = options?.DestinationKey?.GetActualKey();
            if (string.IsNullOrWhiteSpace(sourceCacheKey) || string.IsNullOrWhiteSpace(destionationCacheKey))
            {
                return CacheResponse.FailResponse<ListRightPopLeftPushResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            ListRightPopLeftPushResponse response = null;
            if (database.Store.TryGetEntry(sourceCacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not List<string> list)
                {
                    response = CacheResponse.FailResponse<ListRightPopLeftPushResponse>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                    return response;
                }
                if (list.Count < 1)
                {
                    response = CacheResponse.FailResponse<ListRightPopLeftPushResponse>(SixnetCacheCodes.ListIsEmpty, server: server, database: database);
                    return response;
                }
                List<string> desList = null;
                if (database.Store.TryGetEntry(destionationCacheKey, out var desEntry) && desEntry != null)
                {
                    desList = desEntry.Value as List<string>;
                    if (desList == null)
                    {
                        response = CacheResponse.FailResponse<ListRightPopLeftPushResponse>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                        return response;
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
                response = CacheResponse.FailResponse<ListRightPopLeftPushResponse>(SixnetCacheCodes.KeyIsNotExist);
            }
            response.CacheServer = server;
            response.Database = database;
            return response;
        }

        #endregion

        #region ListRightPop

        /// <summary>
        /// Removes and returns the last element of the list stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return list right pop response</returns>
        public ListRightPopResponse ListRightPop(CacheServer server, ListRightPopOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<ListRightPopResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            ListRightPopResponse response = null;
            var value = string.Empty;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (!(entry.Value is List<string> list))
                {
                    response = CacheResponse.FailResponse<ListRightPopResponse>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                    return response;
                }
                if (list.Count < 1)
                {
                    response = CacheResponse.FailResponse<ListRightPopResponse>(SixnetCacheCodes.ListIsEmpty, server: server, database: database);
                    return response;
                }
                var index = list.Count - 1;
                value = list[index];
                list.RemoveAt(index);
                response = CacheResponse.SuccessResponse<ListRightPopResponse>();
                response.PopValue = value;
            }
            else
            {
                response = CacheResponse.FailResponse<ListRightPopResponse>(SixnetCacheCodes.KeyIsNotExist);
            }
            response.CacheServer = server;
            response.Database = database;
            return response;
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
        public ListRemoveResponse ListRemove(CacheServer server, ListRemoveOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<ListRemoveResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            ListRemoveResponse response = null;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not List<string> list)
                {
                    response = CacheResponse.FailResponse<ListRemoveResponse>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                    return response;
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
                response = CacheResponse.FailResponse<ListRemoveResponse>(SixnetCacheCodes.KeyIsNotExist);
            }
            response.CacheServer = server;
            response.Database = database;
            return response;
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
        public ListRangeResponse ListRange(CacheServer server, ListRangeOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<ListRangeResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            ListRangeResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not List<string> list)
                {
                    response = CacheResponse.FailResponse<ListRangeResponse>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                    return response;
                }
                var start = options.Start;
                if (start < 0)
                {
                    start = list.Count - Math.Abs(start);
                }
                if (start < 0 || start >= list.Count)
                {
                    response = CacheResponse.FailResponse<ListRangeResponse>(SixnetCacheCodes.OffsetError, server: server, database: database);
                    return response;
                }
                var end = options.Stop;
                if (end < 0)
                {
                    end = list.Count - Math.Abs(end);
                }
                if (end < 0 || end >= list.Count)
                {
                    response = CacheResponse.FailResponse<ListRangeResponse>(SixnetCacheCodes.OffsetError, server: server, database: database);
                    return response;
                }
                var begin = Math.Min(start, end);
                response = CacheResponse.SuccessResponse<ListRangeResponse>();
                response.Values = list.GetRange(begin, Math.Abs(end - start) + 1).ToList();
            }
            else
            {
                response = CacheResponse.FailResponse<ListRangeResponse>(SixnetCacheCodes.KeyIsNotExist);
            }
            response.CacheServer = server;
            response.Database = database;
            return response;
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
        public ListLengthResponse ListLength(CacheServer server, ListLengthOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<ListLengthResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var length = 0;
            ListLengthResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not List<string> list)
                {
                    response = CacheResponse.FailResponse<ListLengthResponse>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                    return response;
                }
                length = list.Count;
            }
            response = CacheResponse.SuccessResponse<ListLengthResponse>();
            response.Length = length;
            response.CacheServer = server;
            response.Database = database;
            return response;
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
        public ListLeftPushResponse ListLeftPush(CacheServer server, ListLeftPushOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<ListLeftPushResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            if (options.Values.IsNullOrEmpty())
            {
                return CacheResponse.FailResponse<ListLeftPushResponse>(SixnetCacheCodes.ValuesIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            ListLeftPushResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not List<string> list)
                {
                    response = CacheResponse.FailResponse<ListLeftPushResponse>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                    return response;
                }
                list = options.Values.Concat(list).ToList();
                entry.SetValue(list);
                response = CacheResponse.SuccessResponse<ListLeftPushResponse>();
                response.NewListLength = list.Count;
            }
            else
            {
                using (entry = database.Store.CreateEntry(cacheKey))
                {
                    entry.SetValue(new List<string>(options.Values));
                    SetExpiration(entry, options.Expiration);
                }
                response = CacheResponse.SuccessResponse<ListLeftPushResponse>();
                response.NewListLength = options.Values.Count;
            }
            response.CacheServer = server;
            response.Database = database;
            return response;
        }

        #endregion

        #region ListLeftPop

        /// <summary>
        /// Removes and returns the first element of the list stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return list left pop response</returns>
        public ListLeftPopResponse ListLeftPop(CacheServer server, ListLeftPopOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<ListLeftPopResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            ListLeftPopResponse response = null;
            string value = string.Empty;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not List<string> list)
                {
                    response = CacheResponse.FailResponse<ListLeftPopResponse>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                    return response;
                }
                if (list.Count < 1)
                {
                    response = CacheResponse.FailResponse<ListLeftPopResponse>(SixnetCacheCodes.ListIsEmpty, server: server, database: database);
                    return response;
                }
                value = list[0];
                list.RemoveAt(0);
                entry.SetValue(list);
                response = CacheResponse.SuccessResponse<ListLeftPopResponse>();
                response.PopValue = value;
            }
            else
            {
                response = CacheResponse.FailResponse<ListLeftPopResponse>(SixnetCacheCodes.KeyIsNotExist);
            }
            response.CacheServer = server;
            response.Database = database;
            return response;
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
        public ListInsertBeforeResponse ListInsertBefore(CacheServer server, ListInsertBeforeOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<ListInsertBeforeResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            int newLength = 0;
            bool hasInsertValue = false;
            ListInsertBeforeResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not List<string> list)
                {
                    response = CacheResponse.FailResponse<ListInsertBeforeResponse>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                    return response;
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
            response = new ListInsertBeforeResponse()
            {
                Success = hasInsertValue,
                NewListLength = newLength,
                CacheServer = server,
                Database = database
            };
            return response;
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
        public ListInsertAfterResponse ListInsertAfter(CacheServer server, ListInsertAfterOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<ListInsertAfterResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var newLength = 0;
            var hasInsertValue = false;
            ListInsertAfterResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var list = entry.Value as List<string>;
                if (list == null)
                {
                    response = CacheResponse.FailResponse<ListInsertAfterResponse>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                    return response;
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
            response = new ListInsertAfterResponse()
            {
                NewListLength = newLength,
                Success = hasInsertValue,
                CacheServer = server,
                Database = database
            };
            return response;
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
        public ListGetByIndexResponse ListGetByIndex(CacheServer server, ListGetByIndexOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<ListGetByIndexResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var value = "";
            ListGetByIndexResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not List<string> list)
                {
                    response = CacheResponse.FailResponse<ListGetByIndexResponse>(SixnetCacheCodes.ValueIsNotList, server: server, database: database);
                    return response;
                }
                var index = options.Index;
                if (index < 0)
                {
                    index = list.Count - Math.Abs(index);
                }
                if (index < 0 || index >= list.Count)
                {
                    response = CacheResponse.FailResponse<ListGetByIndexResponse>(SixnetCacheCodes.OffsetError, server: server, database: database);
                    return response;
                }
                value = list[index];
            }
            response = CacheResponse.SuccessResponse<ListGetByIndexResponse>();
            response.Value = value;
            response.CacheServer = server;
            response.Database = database;
            return response;
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
        public HashValuesResponse HashValues(CacheServer server, HashValuesOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<HashValuesResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            List<dynamic> values = null;
            HashValuesResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, dynamic> dict)
                {
                    response = CacheResponse.FailResponse<HashValuesResponse>(SixnetCacheCodes.ValueIsNotDict, server: server, database: database);
                    return response;
                }
                values = dict.Values.ToList();
            }
            values ??= new List<dynamic>(0);
            response = CacheResponse.SuccessResponse<HashValuesResponse>(server, database);
            response.Values = values;
            return response;
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
        public HashSetResponse HashSet(CacheServer server, HashSetOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<HashSetResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            HashSetResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, dynamic> dict)
                {
                    response = CacheResponse.FailResponse<HashSetResponse>(SixnetCacheCodes.ValueIsNotDict, server: server, database: database);
                    return response;
                }
                foreach (var item in options.Items)
                {
                    dict[item.Key] = item.Value;
                }
            }
            else
            {
                using (entry = database.Store.CreateEntry(cacheKey))
                {
                    var value = new ConcurrentDictionary<string, dynamic>(options.Items);
                    entry.SetValue(value);
                    SetExpiration(entry, options.Expiration);
                }
            }
            response = CacheResponse.SuccessResponse<HashSetResponse>(server, database);
            return response;
        }

        #endregion

        #region HashLength

        /// <summary>
        /// Returns the number of fields contained in the hash stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return hash length response</returns>
        public HashLengthResponse HashLength(CacheServer server, HashLengthOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<HashLengthResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            int length = 0;
            HashLengthResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, dynamic> dict)
                {
                    response = CacheResponse.FailResponse<HashLengthResponse>(SixnetCacheCodes.ValueIsNotDict, server: server, database: database);
                    return response;
                }
                length = dict.Keys.Count;
            }
            response = CacheResponse.SuccessResponse<HashLengthResponse>(server, database);
            response.Length = length;
            return response;
        }

        #endregion

        #region HashKeys

        /// <summary>
        /// Returns all field names in the hash stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return hash keys response</returns>
        public HashKeysResponse HashKeys(CacheServer server, HashKeysOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<HashKeysResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            HashKeysResponse response;
            List<string> keys = null;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, dynamic> dict)
                {
                    response = CacheResponse.FailResponse<HashKeysResponse>(SixnetCacheCodes.ValueIsNotDict, server: server, database: database);
                    return response;
                }
                keys = dict.Keys.ToList();
            }
            keys ??= new List<string>(0);
            response = CacheResponse.SuccessResponse<HashKeysResponse>(server, database);
            response.HashKeys = keys;
            return response;
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
        public HashIncrementResponse HashIncrement(CacheServer server, HashIncrementOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<HashIncrementResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var newValue = options.IncrementValue;
            HashIncrementResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, dynamic> dict)
                {
                    response = CacheResponse.FailResponse<HashIncrementResponse>(SixnetCacheCodes.ValueIsNotDict, server: server, database: database);
                    return response;
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
                using (entry = database.Store.CreateEntry(cacheKey))
                {
                    var value = new ConcurrentDictionary<string, dynamic>();
                    value[options.HashField] = options.IncrementValue;
                    entry.SetValue(value);
                    SetExpiration(entry, options.Expiration);
                }
            }
            response = CacheResponse.SuccessResponse<HashIncrementResponse>(server, database);
            response.HashField = options.HashField;
            response.NewValue = newValue;
            return response;
        }

        #endregion

        #region HashGet

        /// <summary>
        /// Returns the value associated with field in the hash stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return hash get response</returns>
        public HashGetResponse HashGet(CacheServer server, HashGetOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<HashGetResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            dynamic value = null;
            HashGetResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, dynamic> dict)
                {
                    response = CacheResponse.FailResponse<HashGetResponse>(SixnetCacheCodes.ValueIsNotDict, server: server, database: database);
                    return response;
                }
                dict.TryGetValue(options.HashField, out value);
            }
            response = CacheResponse.SuccessResponse<HashGetResponse>(server, database);
            response.Value = value;
            return response;
        }

        #endregion

        #region HashGetAll

        /// <summary>
        /// Returns all fields and values of the hash stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return hash get all response</returns>
        public HashGetAllResponse HashGetAll(CacheServer server, HashGetAllOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<HashGetAllResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            ConcurrentDictionary<string, dynamic> values = null;
            HashGetAllResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, dynamic> dict)
                {
                    response = CacheResponse.FailResponse<HashGetAllResponse>(SixnetCacheCodes.ValueIsNotDict, server: server, database: database);
                    return response; ;
                }
                values = new ConcurrentDictionary<string, dynamic>(dict);
            }
            response = CacheResponse.SuccessResponse<HashGetAllResponse>(server, database);
            response.HashValues = values?.ToDictionary(c => c.Key, c => c.Value) ?? new Dictionary<string, dynamic>(0);
            return response;
        }

        #endregion

        #region HashExist

        /// <summary>
        /// Returns if field is an existing field in the hash stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return hash exists response</returns>
        public HashExistsResponse HashExist(CacheServer server, HashExistsOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<HashExistsResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var existKey = false;
            HashExistsResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, dynamic> dict)
                {
                    response = CacheResponse.FailResponse<HashExistsResponse>(SixnetCacheCodes.ValueIsNotDict, server: server, database: database);
                    return response;
                }
                existKey = dict.ContainsKey(options.HashField);
            }
            response = CacheResponse.SuccessResponse<HashExistsResponse>(server, database);
            response.HasField = existKey;
            return response;
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
        public HashDeleteResponse HashDelete(CacheServer server, HashDeleteOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<HashDeleteResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            HashDeleteResponse response;
            var remove = false;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, dynamic> dict)
                {
                    response = CacheResponse.FailResponse<HashDeleteResponse>(SixnetCacheCodes.ValueIsNotDict, server: server, database: database);
                    return response;
                }
                foreach (var field in options.HashFields)
                {
                    remove |= dict.TryRemove(field, out var value);
                }
            }
            response = new HashDeleteResponse()
            {
                Success = remove,
                CacheServer = server,
                Database = database
            };
            return response;
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
        public HashDecrementResponse HashDecrement(CacheServer server, HashDecrementOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                CacheResponse.FailResponse<HashDecrementResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var newValue = options.DecrementValue;
            HashDecrementResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, dynamic> dict)
                {
                    response = CacheResponse.FailResponse<HashDecrementResponse>(SixnetCacheCodes.ValueIsNotDict, server: server, database: database);
                    return response;
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
                using (entry = database.Store.CreateEntry(cacheKey))
                {
                    var value = new ConcurrentDictionary<string, dynamic>();
                    value[options.HashField] = options.DecrementValue;
                    entry.SetValue(value);
                    SetExpiration(entry, options.Expiration);
                }
            }
            response = CacheResponse.SuccessResponse<HashDecrementResponse>(server, database);
            response.HashField = options.HashField;
            response.NewValue = newValue;
            return response;
        }

        #endregion

        #region HashScan

        /// <summary>
        /// The HSCAN options is used to incrementally iterate over a hash
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return hash scan response</returns>
        public HashScanResponse HashScan(CacheServer server, HashScanOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                CacheResponse.FailResponse<HashScanResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var values = new Dictionary<string, dynamic>();
            HashScanResponse response;
            if (options.PageSize > 0 && database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, dynamic>;
                if (dict == null)
                {
                    response = CacheResponse.FailResponse<HashScanResponse>(SixnetCacheCodes.ValueIsNotDict, server: server, database: database);
                    return response;
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
            response = CacheResponse.SuccessResponse<HashScanResponse>(server, database);
            response.HashValues = values;
            return response;
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
        public SetRemoveResponse SetRemove(CacheServer server, SetRemoveOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<SetRemoveResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var removeCount = 0;
            SetRemoveResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null && !options.RemoveMembers.IsNullOrEmpty())
            {
                if (entry.Value is not ConcurrentDictionary<string, byte> dict)
                {
                    response = CacheResponse.FailResponse<SetRemoveResponse>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                    return response;
                }
                foreach (var member in options.RemoveMembers)
                {
                    if (dict.TryRemove(member, out var value))
                    {
                        removeCount++;
                    }
                }
            }
            response = CacheResponse.SuccessResponse<SetRemoveResponse>(server, database);
            response.RemoveCount = removeCount;
            return response;
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
        public SetRandomMembersResponse SetRandomMembers(CacheServer server, SetRandomMembersOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<SetRandomMembersResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var members = new List<string>();
            SetRandomMembersResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null && options.Count != 0)
            {
                if (entry.Value is not ConcurrentDictionary<string, byte> dict)
                {
                    response = CacheResponse.FailResponse<SetRandomMembersResponse>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                    return response;
                }
                var allowSame = options.Count < 0;
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
                    var shuffle = new SixnetShuffleNet<string>(keys);
                    members.AddRange(shuffle.TakeNextValues(count));
                }
            }
            response = CacheResponse.SuccessResponse<SetRandomMembersResponse>(server, database);
            response.Members = members;
            return response;
        }

        #endregion

        #region SetRandomMember

        /// <summary>
        /// Return a random element from the set value stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return set random member</returns>
        public SetRandomMemberResponse SetRandomMember(CacheServer server, SetRandomMemberOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<SetRandomMemberResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var member = string.Empty;
            SetRandomMemberResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, byte> dict)
                {
                    response = CacheResponse.FailResponse<SetRandomMemberResponse>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                    return response;
                }
                var keys = dict.Keys;
                if (!keys.IsNullOrEmpty())
                {
                    var ranIndex = RandomNumberHelper.GetRandomNumber(keys.Count - 1);
                    member = keys.ElementAt(ranIndex);
                }
            }
            response = CacheResponse.SuccessResponse<SetRandomMemberResponse>(server, database);
            response.Member = member;
            return response;
        }

        #endregion

        #region SetPop

        /// <summary>
        /// Removes and returns a random element from the set value stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return set pop response</returns>
        public SetPopResponse SetPop(CacheServer server, SetPopOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<SetPopResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var member = string.Empty;
            SetPopResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, byte> dict)
                {
                    response = CacheResponse.FailResponse<SetPopResponse>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                    return response;
                }
                var keys = dict.Keys;
                if (!keys.IsNullOrEmpty())
                {
                    var ranIndex = RandomNumberHelper.GetRandomNumber(keys.Count - 1);
                    member = keys.ElementAt(ranIndex);
                    dict.TryRemove(member, out var value);
                }
            }
            response = CacheResponse.SuccessResponse<SetPopResponse>(server, database);
            response.PopValue = member;
            return response;
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
        public SetMoveResponse SetMove(CacheServer server, SetMoveOptions options)
        {
            var cacheKey = options?.SourceKey?.GetActualKey();
            var desKey = options?.DestinationKey?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey) || string.IsNullOrWhiteSpace(desKey))
            {
                return CacheResponse.FailResponse<SetMoveResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var isRemove = false;
            SetMoveResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, byte> dict)
                {
                    response = CacheResponse.FailResponse<SetMoveResponse>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                    return response;
                }
                database.Store.TryGetEntry(desKey, out var desEntry);
                ConcurrentDictionary<string, byte> desDict = null;
                if (desEntry != null)
                {
                    desDict = desEntry.Value as ConcurrentDictionary<string, byte>;
                    if (desDict == null)
                    {
                        response = CacheResponse.FailResponse<SetMoveResponse>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                        return response;
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
                        using (desEntry = database.Store.CreateEntry(desKey))
                        {
                            desDict = new ConcurrentDictionary<string, byte>();
                            desDict.TryAdd(options.MoveMember, 0);
                            desEntry.SetValue(desDict);
                            SetExpiration(desEntry, options.Expiration);
                        }
                    }
                }
            }
            response = new SetMoveResponse()
            {
                Success = isRemove,
                CacheServer = server,
                Database = database
            };
            return response;
        }

        #endregion

        #region SetMembers

        /// <summary>
        /// Returns all the members of the set value stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return set members response</returns>
        public SetMembersResponse SetMembers(CacheServer server, SetMembersOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<SetMembersResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            List<string> members = null;
            SetMembersResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, byte> dict)
                {
                    response = CacheResponse.FailResponse<SetMembersResponse>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                    return response;
                }
                members = new List<string>(dict.Count);
                members.AddRange(dict.Keys);
            }
            members ??= new List<string>(0);
            response = CacheResponse.SuccessResponse<SetMembersResponse>(server, database);
            response.Members = members;
            return response;
        }

        #endregion

        #region SetLength

        /// <summary>
        /// Returns the set cardinality (number of elements) of the set stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return set length response</returns>
        public SetLengthResponse SetLength(CacheServer server, SetLengthOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<SetLengthResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var length = 0;
            SetLengthResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, byte> dict)
                {
                    response = CacheResponse.FailResponse<SetLengthResponse>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                    return response;
                }
                length = dict.Count;
            }
            response = CacheResponse.SuccessResponse<SetLengthResponse>(server, database);
            response.Length = length;
            return response;
        }

        #endregion

        #region SetContains

        /// <summary>
        /// Returns if member is a member of the set stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return set contains response</returns>
        public SetContainsResponse SetContains(CacheServer server, SetContainsOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<SetContainsResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var existMember = false;
            SetContainsResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, byte> dict)
                {
                    response = CacheResponse.FailResponse<SetContainsResponse>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                    return response;
                }
                existMember = dict.ContainsKey(options.Member);
            }
            response = CacheResponse.SuccessResponse<SetContainsResponse>(server, database);
            response.ContainsValue = existMember;
            return response;
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
        public SetCombineResponse SetCombine(CacheServer server, SetCombineOptions options)
        {
            if (options?.Keys.IsNullOrEmpty() ?? true)
            {
                return CacheResponse.FailResponse<SetCombineResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            List<IEnumerable<string>> allKeyValues = new List<IEnumerable<string>>();
            foreach (var key in options.Keys)
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
                switch (options.CombineOperation)
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
            var response = CacheResponse.SuccessResponse<SetCombineResponse>(server, database);
            response.CombineValues = members.ToList();
            return response;
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
        public SetCombineAndStoreResponse SetCombineAndStore(CacheServer server, SetCombineAndStoreOptions options)
        {
            var desCacheKey = options.DestinationKey?.GetActualKey() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(desCacheKey) || (options?.SourceKeys.IsNullOrEmpty() ?? true))
            {
                return CacheResponse.FailResponse<SetCombineAndStoreResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            List<string> members = null;
            SetCombineAndStoreResponse response;
            foreach (var key in options.SourceKeys)
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
                        response = CacheResponse.FailResponse<SetCombineAndStoreResponse>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                        return response;
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
            members ??= new List<string>(0);
            database.Store.TryGetEntry(desCacheKey, out var desEntry);
            if (desEntry != null)
            {
                if (desEntry.Value is not ConcurrentDictionary<string, byte> desDict)
                {
                    response = CacheResponse.FailResponse<SetCombineAndStoreResponse>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                    return response;
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
                    SetExpiration(desEntry, options.Expiration);
                }
            }
            response = CacheResponse.SuccessResponse<SetCombineAndStoreResponse>(server, database);
            response.Count = members.Count;
            return response;
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
        public SetAddResponse SetAdd(CacheServer server, SetAddOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<SetAddResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            SetAddResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, byte> dict)
                {
                    response = CacheResponse.FailResponse<SetAddResponse>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                    return response;
                }
                foreach (var member in options.Members)
                {
                    dict[member] = 0;
                }
            }
            else
            {
                using (entry = database.Store.CreateEntry(cacheKey))
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
            response = new SetAddResponse()
            {
                Success = true,
                CacheServer = server,
                Database = database
            };
            return response;
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
        public SortedSetScoreResponse SortedSetScore(CacheServer server, SortedSetScoreOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<SortedSetScoreResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            double? score = null;
            SortedSetScoreResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, double> dict)
                {
                    response = CacheResponse.FailResponse<SortedSetScoreResponse>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
                    return response;
                }
                if (dict.TryGetValue(options.Member, out var memberScore))
                {
                    score = memberScore;
                }
            }
            response = CacheResponse.SuccessResponse<SortedSetScoreResponse>(server, database);
            response.Score = score;
            return response;
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
        public SortedSetRemoveRangeByValueResponse SortedSetRemoveRangeByValue(CacheServer server, SortedSetRemoveRangeByValueOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<SortedSetRemoveRangeByValueResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var removeCount = 0;
            SortedSetRemoveRangeByValueResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, double> dict)
                {
                    response = CacheResponse.FailResponse<SortedSetRemoveRangeByValueResponse>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
                    return response;
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
            response = CacheResponse.SuccessResponse<SortedSetRemoveRangeByValueResponse>(server, database);
            response.RemoveCount = removeCount;
            return response;
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
        public SortedSetRemoveRangeByScoreResponse SortedSetRemoveRangeByScore(CacheServer server, SortedSetRemoveRangeByScoreOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<SortedSetRemoveRangeByScoreResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var removeCount = 0;
            SortedSetRemoveRangeByScoreResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, double> dict)
                {
                    response = CacheResponse.FailResponse<SortedSetRemoveRangeByScoreResponse>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
                    return response;
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
            response = CacheResponse.SuccessResponse<SortedSetRemoveRangeByScoreResponse>(server, database);
            response.RemoveCount = removeCount;
            return response;
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
        public SortedSetRemoveRangeByRankResponse SortedSetRemoveRangeByRank(CacheServer server, SortedSetRemoveRangeByRankOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<SortedSetRemoveRangeByRankResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            int removeCount = 0;
            SortedSetRemoveRangeByRankResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, double> dict)
                {
                    response = CacheResponse.FailResponse<SortedSetRemoveRangeByRankResponse>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
                    return response;
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
            response = CacheResponse.SuccessResponse<SortedSetRemoveRangeByRankResponse>(server, database);
            response.RemoveCount = removeCount;
            return response;
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
        public SortedSetRemoveResponse SortedSetRemove(CacheServer server, SortedSetRemoveOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<SortedSetRemoveResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            var removeCount = 0;
            SortedSetRemoveResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null && !options.RemoveMembers.IsNullOrEmpty())
            {
                if (entry.Value is not ConcurrentDictionary<string, double> dict)
                {
                    response = CacheResponse.FailResponse<SortedSetRemoveResponse>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
                    return response;
                }
                foreach (var rmem in options.RemoveMembers)
                {
                    if (dict.TryRemove(rmem, out var value))
                    {
                        removeCount++;
                    }
                }
            }
            response = CacheResponse.SuccessResponse<SortedSetRemoveResponse>(server, database);
            response.RemoveCount = removeCount;
            return response;
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
        public SortedSetRankResponse SortedSetRank(CacheServer server, SortedSetRankOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<SortedSetRankResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            long? rank = null;
            SortedSetRankResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, double> dict)
                {
                    response = CacheResponse.FailResponse<SortedSetRankResponse>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
                    return response;
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
            response = CacheResponse.SuccessResponse<SortedSetRankResponse>(server, database);
            response.Rank = rank;
            return response;
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
        public SortedSetRangeByValueResponse SortedSetRangeByValue(CacheServer server, SortedSetRangeByValueOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<SortedSetRangeByValueResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            List<string> members = null;
            SortedSetRangeByValueResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, double>;
                if (dict == null)
                {
                    response = CacheResponse.FailResponse<SortedSetRangeByValueResponse>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
                    return response;
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
            response = CacheResponse.SuccessResponse<SortedSetRangeByValueResponse>(server, database);
            response.Members = members ?? new List<string>(0);
            return response;
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
        public SortedSetRangeByScoreWithScoresResponse SortedSetRangeByScoreWithScores(CacheServer server, SortedSetRangeByScoreWithScoresOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<SortedSetRangeByScoreWithScoresResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            List<SortedSetMember> members = null;
            SortedSetRangeByScoreWithScoresResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, double> dict)
                {
                    response = CacheResponse.FailResponse<SortedSetRangeByScoreWithScoresResponse>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
                    return response;
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
            response = CacheResponse.SuccessResponse<SortedSetRangeByScoreWithScoresResponse>(server, database);
            response.Members = members;
            return response;
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
        public SortedSetRangeByScoreResponse SortedSetRangeByScore(CacheServer server, SortedSetRangeByScoreOptions options)
        {
            var setResponse = SortedSetRangeByScoreWithScores(server, new SortedSetRangeByScoreWithScoresOptions()
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
            });
            SortedSetRangeByScoreResponse response;
            if (setResponse?.Success ?? false)
            {
                response = CacheResponse.SuccessResponse<SortedSetRangeByScoreResponse>(server, setResponse?.Database);
                response.Members = setResponse?.Members?.Select(c => c.Value).ToList() ?? new List<string>(0);
            }
            else
            {
                response = CacheResponse.FailResponse<SortedSetRangeByScoreResponse>(setResponse.Code, setResponse.Message);
            }
            return response;
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
        public SortedSetRangeByRankWithScoresResponse SortedSetRangeByRankWithScores(CacheServer server, SortedSetRangeByRankWithScoresOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<SortedSetRangeByRankWithScoresResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            List<SortedSetMember> members = null;
            SortedSetRangeByRankWithScoresResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, double> dict)
                {
                    response = CacheResponse.FailResponse<SortedSetRangeByRankWithScoresResponse>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
                    return response;
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
            response = CacheResponse.SuccessResponse<SortedSetRangeByRankWithScoresResponse>(server, database);
            response.Members = members ?? new List<SortedSetMember>(0);
            return response;
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
        public SortedSetRangeByRankResponse SortedSetRangeByRank(CacheServer server, SortedSetRangeByRankOptions options)
        {
            var setResponse = SortedSetRangeByRankWithScores(server, new SortedSetRangeByRankWithScoresOptions()
            {
                CacheObject = options.CacheObject,
                CommandFlags = options.CommandFlags,
                Key = options.Key,
                Order = options.Order,
                Start = options.Start,
                Stop = options.Stop
            });
            SortedSetRangeByRankResponse response;
            if (setResponse?.Success ?? false)
            {
                response = CacheResponse.SuccessResponse<SortedSetRangeByRankResponse>(server, setResponse.Database);
                response.Members = setResponse.Members?.Select(c => c.Value).ToList() ?? new List<string>(0);
            }
            else
            {
                response = CacheResponse.FailResponse<SortedSetRangeByRankResponse>(setResponse.Code, setResponse.Message);
            }
            return response;
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
        public SortedSetLengthByValueResponse SortedSetLengthByValue(CacheServer server, SortedSetLengthByValueOptions options)
        {
            var setResponse = SortedSetRangeByValue(server, new SortedSetRangeByValueOptions()
            {
                CacheObject = options.CacheObject,
                CommandFlags = options.CommandFlags,
                Key = options.Key,
                MinValue = options.MinValue,
                MaxValue = options.MaxValue,
                Offset = 0,
                Count = -1
            });
            SortedSetLengthByValueResponse response;
            if (setResponse?.Success ?? false)
            {
                response = CacheResponse.SuccessResponse<SortedSetLengthByValueResponse>(server, setResponse.Database);
                response.Length = setResponse.Members?.Count ?? 0;
            }
            else
            {
                response = CacheResponse.FailResponse<SortedSetLengthByValueResponse>(setResponse.Code, setResponse.Message);
            }
            return response;
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
        public SortedSetLengthResponse SortedSetLength(CacheServer server, SortedSetLengthOptions options)
        {
            var setResponse = SortedSetRangeByScore(server, new SortedSetRangeByScoreOptions()
            {
                CacheObject = options.CacheObject,
                CommandFlags = options.CommandFlags,
                Key = options.Key,
                Offset = 0,
                Count = -1,
            });
            SortedSetLengthResponse response;
            if (setResponse?.Success ?? false)
            {
                response = CacheResponse.SuccessResponse<SortedSetLengthResponse>(server, setResponse.Database);
                response.Length = setResponse.Members?.Count ?? 0;
            }
            else
            {
                response = CacheResponse.FailResponse<SortedSetLengthResponse>(setResponse.Code, setResponse.Message);
            }
            return response;
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
        public SortedSetIncrementResponse SortedSetIncrement(CacheServer server, SortedSetIncrementOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<SortedSetIncrementResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            double score = 0;
            SortedSetIncrementResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, double> dict)
                {
                    response = CacheResponse.FailResponse<SortedSetIncrementResponse>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
                    return response;
                }
                if (dict.TryGetValue(options.Member, out var memberScore))
                {
                    score = memberScore + options.IncrementScore;
                    dict[options.Member] = score;
                }
            }
            response = CacheResponse.SuccessResponse<SortedSetIncrementResponse>(server, database);
            response.NewScore = score;
            return response;
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
        public SortedSetDecrementResponse SortedSetDecrement(CacheServer server, SortedSetDecrementOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<SortedSetDecrementResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            SortedSetDecrementResponse response;
            double score = 0;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, double> dict)
                {
                    response = CacheResponse.FailResponse<SortedSetDecrementResponse>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
                    return response;
                }
                if (dict.TryGetValue(options.Member, out var memberScore))
                {
                    score = memberScore - options.DecrementScore;
                    dict[options.Member] = score;
                }
            }
            response = CacheResponse.SuccessResponse<SortedSetDecrementResponse>(server, database);
            response.NewScore = score;
            return response;
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
        public SortedSetCombineAndStoreResponse SortedSetCombineAndStore(CacheServer server, SortedSetCombineAndStoreOptions options)
        {
            var desCacheKey = options.DestinationKey?.GetActualKey() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(desCacheKey) || (options?.SourceKeys.IsNullOrEmpty() ?? true))
            {
                return CacheResponse.FailResponse<SortedSetCombineAndStoreResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            HashSet<string> members = null;
            var allMembers = new Dictionary<string, List<double>>();
            SortedSetCombineAndStoreResponse response;
            for (int i = 0; i < options.SourceKeys.Count; i++)
            {
                var key = options.SourceKeys[i];
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
                        response = CacheResponse.FailResponse<SortedSetCombineAndStoreResponse>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                        return response;
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
            var resultItems = new Dictionary<string, double>();
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
            if (database.Store.TryGetEntry(desCacheKey, out var desEntry) && desEntry != null)
            {
                if (desEntry.Value is not ConcurrentDictionary<string, double> desDict)
                {
                    response = CacheResponse.FailResponse<SortedSetCombineAndStoreResponse>(SixnetCacheCodes.ValueIsNotSet, server: server, database: database);
                    return response;
                }
                desEntry.Value = resultItems;
            }
            else
            {
                using (desEntry = database.Store.CreateEntry(desCacheKey))
                {
                    desEntry.SetValue(resultItems);
                    SetExpiration(desEntry, options.Expiration);
                }
            }
            response = CacheResponse.SuccessResponse<SortedSetCombineAndStoreResponse>(server, database);
            response.NewSetLength = resultItems.Count;
            return response;
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
        public SortedSetAddResponse SortedSetAdd(CacheServer server, SortedSetAddOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<SortedSetAddResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            if (options.Members.IsNullOrEmpty())
            {
                return CacheResponse.FailResponse<SortedSetAddResponse>(SixnetCacheCodes.ValuesIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            long length = 0;
            SortedSetAddResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (entry.Value is not ConcurrentDictionary<string, double> dict)
                {
                    response = CacheResponse.FailResponse<SortedSetAddResponse>(SixnetCacheCodes.ValueIsNotSortedSet, server: server, database: database);
                    return response;
                }
                foreach (var mem in options.Members)
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
                    options.Members.ForEach(c =>
                    {
                        newDict.TryAdd(c.Value, c.Score);
                    });
                    length = newDict.Count;
                    entry.SetValue(newDict);
                    SetExpiration(entry, options.Expiration);
                }
            }
            response = CacheResponse.SuccessResponse<SortedSetAddResponse>(server, database);
            response.Length = length;
            return response;
        }

        #endregion

        #endregion

        #region Sort

        #region Sort

        /// <summary>
        /// Sorts a list, set or sorted set (numerically or alphabetically, ascending by
        /// default){Task.Delay(100);return null;} By default, the elements themselves are compared, but the values can
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
        public SortResponse Sort(CacheServer server, SortOptions options)
        {
            var keyTypeResponse = KeyType(server, new TypeOptions()
            {
                CacheObject = options.CacheObject,
                CommandFlags = options.CommandFlags,
                Key = options.Key
            });
            SortResponse response;
            if (keyTypeResponse?.Success ?? false)
            {
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

                IEnumerable<string> values = null;
                switch (keyTypeResponse.KeyType)
                {
                    case CacheKeyType.List:
                        var listResponse = ListRange(server, new ListRangeOptions()
                        {
                            CacheObject = options.CacheObject,
                            CommandFlags = options.CommandFlags,
                            Key = options.Key,
                            Start = 0,
                            Stop = -1
                        });
                        values = filterValueFuc(listResponse?.Values);
                        response = new SortResponse()
                        {
                            Success = true,
                            Values = values?.ToList() ?? new List<string>(0),
                            CacheServer = server,
                            Database = listResponse.Database
                        };
                        break;
                    case CacheKeyType.Set:
                        var setResponse = SetMembers(server, new SetMembersOptions()
                        {
                            CacheObject = options.CacheObject,
                            CommandFlags = options.CommandFlags,
                            Key = options.Key
                        });
                        values = filterValueFuc(setResponse?.Members);
                        response = new SortResponse()
                        {
                            Success = true,
                            Values = values?.ToList() ?? new List<string>(0),
                            CacheServer = server,
                            Database = setResponse.Database
                        };
                        break;
                    case CacheKeyType.SortedSet:
                        var sortedSetResponse = SortedSetRangeByRankWithScores(server, new SortedSetRangeByRankWithScoresOptions()
                        {
                            CacheObject = options.CacheObject,
                            CommandFlags = options.CommandFlags,
                            Key = options.Key,
                            Order = options.Order,
                            Start = 0,
                            Stop = -1
                        });
                        IEnumerable<SortedSetMember> sortedSetMembers = sortedSetResponse?.Members ?? new List<SortedSetMember>(0);
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
                        response = new SortResponse()
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
                response = CacheResponse.FailResponse<SortResponse>(keyTypeResponse?.Code, keyTypeResponse?.Message, server: server, database: keyTypeResponse?.Database);
            }
            return response;
        }

        #endregion

        #region SortAndStore

        /// <summary>
        /// Sorts a list, set or sorted set (numerically or alphabetically, ascending by
        /// default){Task.Delay(100);return null;} By default, the elements themselves are compared, but the values can
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
        public SortAndStoreResponse SortAndStore(CacheServer server, SortAndStoreOptions options)
        {
            if (string.IsNullOrWhiteSpace(options?.SourceKey))
            {
                throw new ArgumentNullException($"{nameof(SortAndStoreOptions)}.{nameof(SortAndStoreOptions.SourceKey)}");
            }
            if (string.IsNullOrWhiteSpace(options?.DestinationKey))
            {
                throw new ArgumentNullException($"{nameof(SortAndStoreOptions)}.{nameof(SortAndStoreOptions.DestinationKey)}");
            }
            var sortResponse = Sort(server, new SortOptions()
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
            });
            SortAndStoreResponse response;
            if (sortResponse?.Success ?? false)
            {
                var values = sortResponse?.Values;
                ListLeftPush(server, new ListLeftPushOptions()
                {
                    CacheObject = options.CacheObject,
                    CommandFlags = options.CommandFlags,
                    Expiration = options.Expiration,
                    Key = options.DestinationKey,
                    Values = values
                });
                response = CacheResponse.SuccessResponse<SortAndStoreResponse>(server, sortResponse.Database);
            }
            else
            {
                response = CacheResponse.FailResponse<SortAndStoreResponse>(sortResponse.Code, sortResponse.Message);
            }
            return response;
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
        public TypeResponse KeyType(CacheServer server, TypeOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<TypeResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            TypeResponse response = null;
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
                response = CacheResponse.SuccessResponse<TypeResponse>(server, database);
                response.KeyType = cacheKeyType;
            }
            else
            {
                response = CacheResponse.FailResponse<TypeResponse>(SixnetCacheCodes.KeyIsNotExist, server: server, database: database);
            }
            return response;
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
        public TimeToLiveResponse KeyTimeToLive(CacheServer server, TimeToLiveOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<TimeToLiveResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            TimeToLiveResponse response = null;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                response = CacheResponse.SuccessResponse<TimeToLiveResponse>();
                var expiration = GetExpiration(entry);
                response.TimeToLiveSeconds = (long)(expiration.Item2?.TotalSeconds ?? 0);
            }
            else
            {
                response = CacheResponse.FailResponse<TimeToLiveResponse>(SixnetCacheCodes.KeyIsNotExist);
            }
            response.CacheServer = server;
            response.Database = database;
            return response;
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
        public RestoreResponse KeyRestore(CacheServer server, RestoreOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<RestoreResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            using (var entry = database.Store.CreateEntry(cacheKey))
            {
                entry.SetValue(GetEncoding().GetString(options.Value));
                SetExpiration(entry, options.Expiration);
            }
            return CacheResponse.SuccessResponse<RestoreResponse>(server, database);
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
        public RenameResponse KeyRename(CacheServer server, RenameOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            string newCacheKey = options?.NewKey?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey) || string.IsNullOrWhiteSpace(newCacheKey))
            {
                return CacheResponse.FailResponse<RenameResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            RenameResponse response = null;
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
                response = CacheResponse.SuccessResponse<RenameResponse>();
            }
            else
            {
                response = CacheResponse.FailResponse<RenameResponse>(SixnetCacheCodes.KeyIsNotExist);
            }
            response.CacheServer = server;
            response.Database = database;
            return response;
        }

        #endregion

        #region KeyRandom

        /// <summary>
        /// Return a random key from the currently selected database.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return key random response</returns>
        public RandomResponse KeyRandom(CacheServer server, RandomOptions options)
        {
            var database = GetDatabase(server);
            var response = CacheResponse.SuccessResponse<RandomResponse>(server, database);
            response.Key = database.Store.GetRandomKey();
            return response;
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
        public PersistResponse KeyPersist(CacheServer server, PersistOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<PersistResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            PersistResponse response = null;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                using (var newEntry = database.Store.CreateEntry(cacheKey))
                {
                    newEntry.SetValue(entry.Value);
                }
                response = CacheResponse.SuccessResponse<PersistResponse>();
            }
            else
            {
                response = CacheResponse.FailResponse<PersistResponse>(SixnetCacheCodes.KeyIsNotExist);
            }
            response.CacheServer = server;
            response.Database = database;
            return response;
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
        public MoveResponse KeyMove(CacheServer server, MoveOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<MoveResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var desDatabase = GetDatabase(options.DatabaseName);
            if (desDatabase == null)
            {
                return CacheResponse.FailResponse<MoveResponse>("", "Destination database not find", server: server);
            }
            var database = GetDatabase(server);
            MoveResponse response;
            if (database.Store.TryGetEntry(cacheKey, out var entry))
            {
                using (var desEntry = desDatabase.Store.CreateEntry(cacheKey))
                {
                    desEntry.Value = entry.Value;
                    SetExpiration(desEntry, GetCacheExpiration(entry));
                }
                response = CacheResponse.SuccessResponse<MoveResponse>(server, database);
            }
            else
            {
                response = CacheResponse.FailResponse<MoveResponse>("", $"Not find key:{cacheKey}", server, database);
            }
            return response;
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
        public MigrateKeyResponse KeyMigrate(CacheServer server, MigrateKeyOptions options)
        {
            return CacheResponse.FailResponse<MigrateKeyResponse>(SixnetCacheCodes.OperationIsNotSupported, server: server);
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
        public ExpireResponse KeyExpire(CacheServer server, ExpireOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<ExpireResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            ExpireResponse response = null;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                SetExpiration(entry, options.Expiration);
                response = CacheResponse.SuccessResponse<ExpireResponse>();
            }
            else
            {
                response = CacheResponse.FailResponse<ExpireResponse>(SixnetCacheCodes.KeyIsNotExist);
            }
            response.CacheServer = server;
            response.Database = database;
            return response;
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
        public DumpResponse KeyDump(CacheServer server, DumpOptions options)
        {
            var cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<DumpResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            DumpResponse response = null;
            if (database.Store.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                response = CacheResponse.SuccessResponse<DumpResponse>();
                response.ByteValues = GetEncoding().GetBytes(entry.Value?.ToString() ?? string.Empty);
            }
            else
            {
                response = CacheResponse.FailResponse<DumpResponse>(SixnetCacheCodes.KeyIsNotExist);
            }
            response.CacheServer = server;
            response.Database = database;
            return response;
        }

        #endregion

        #region KeyDelete

        /// <summary>
        /// Removes the specified keys. A key is ignored if it does not exist.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return key delete response</returns>
        public DeleteResponse KeyDelete(CacheServer server, DeleteOptions options)
        {
            if (options.Keys?.IsNullOrEmpty() ?? true)
            {
                return CacheResponse.FailResponse<DeleteResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            long deleteCount = 0;
            foreach (var key in options.Keys)
            {
                var cacheKey = key?.GetActualKey() ?? string.Empty;
                if (database.Store.TryGetEntry(cacheKey, out var entry))
                {
                    deleteCount++;
                    database.Store.Remove(cacheKey);
                }
            }
            var response = CacheResponse.SuccessResponse<DeleteResponse>(server, database);
            response.DeleteCount = deleteCount;
            return response;
        }

        #endregion

        #region KeyExists

        /// <summary>
        /// Key exists
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return exists response</returns>
        public ExistResponse KeyExist(CacheServer server, ExistOptions options)
        {
            if (options.Keys?.IsNullOrEmpty() ?? true)
            {
                return CacheResponse.FailResponse<ExistResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            long count = 0;
            foreach (var key in options.Keys)
            {
                var cacheKey = key?.GetActualKey() ?? string.Empty;
                if (database.Store.TryGetEntry(key?.GetActualKey(), out var entry))
                {
                    count++;
                }
            }
            var response = CacheResponse.SuccessResponse<ExistResponse>(server, database);
            response.KeyCount = count;
            return response;
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
        public GetAllDataBaseResponse GetAllDataBase(CacheServer server, GetAllDataBaseOptions options)
        {
            var response = CacheResponse.SuccessResponse<GetAllDataBaseResponse>(server);
            response.Databases = MemoryCacheCollection.Select(c => c.Value as CacheDatabase).ToList();
            return response;
        }

        #endregion

        #region Query keys

        /// <summary>
        /// Query keys
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return get keys response</returns>
        public GetKeysResponse GetKeys(CacheServer server, GetKeysOptions options)
        {
            var database = GetDatabase(server);
            var allKeys = database.Store.GetAllKeys();
            Func<string, bool> where = c => true;
            var skip = 0;
            var count = allKeys.Count;
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
            var response = CacheResponse.SuccessResponse<GetKeysResponse>(server, database);
            response.Keys = new CachePaging<CacheKey>(options.Query?.Page ?? 1, options.Query?.PageSize ?? allKeys.Count, allKeys.Count, keys);
            return response;
        }

        #endregion

        #region Clear data

        /// <summary>
        /// Clear database data
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return clear data response</returns>
        public ClearDataResponse ClearData(CacheServer server, ClearDataOptions options)
        {
            var database = GetDatabase(server);
            database.Store.Compact(1);
            return CacheResponse.SuccessResponse<ClearDataResponse>(server, database);
        }

        #endregion

        #region Get cache item detail

        /// <summary>
        /// Get cache item detail
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return get key detail response</returns>
        public GetDetailResponse GetKeyDetail(CacheServer server, GetDetailOptions options)
        {
            string cacheKey = options?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<GetDetailResponse>(SixnetCacheCodes.KeyIsNullOrEmpty, server: server);
            }
            var database = GetDatabase(server);
            GetDetailResponse response = null;
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
                response = CacheResponse.FailResponse<GetDetailResponse>(SixnetCacheCodes.KeyIsNotExist);
            }
            response.CacheServer = server;
            response.Database = database;
            return response;
        }

        #endregion

        #region Get server config

        /// <summary>
        /// Get server config
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return get server config response</returns>
        public GetServerConfigurationResponse GetServerConfiguration(CacheServer server, GetServerConfigurationOptions options)
        {
            return CacheResponse.FailResponse<GetServerConfigurationResponse>(SixnetCacheCodes.OperationIsNotSupported);
        }

        #endregion

        #region Save server configuration

        /// <summary>
        /// Save server config
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="options">Options</param>
        /// <returns>Return save server config response</returns>
        public SaveServerConfigurationResponse SaveServerConfiguration(CacheServer server, SaveServerConfigurationOptions options)
        {
            return CacheResponse.FailResponse<SaveServerConfigurationResponse>(SixnetCacheCodes.OperationIsNotSupported);
        }

        #endregion

        #endregion
    }
}
