using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using EZNEW.Cache.Constant;
using EZNEW.Cache.Hash.Request;
using EZNEW.Cache.Hash.Response;
using EZNEW.Cache.Keys.Request;
using EZNEW.Cache.Keys.Response;
using EZNEW.Cache.List.Request;
using EZNEW.Cache.List.Response;
using EZNEW.Cache.Server.Request;
using EZNEW.Cache.Server.Response;
using EZNEW.Cache.Set.Request;
using EZNEW.Cache.Set.Response;
using EZNEW.Cache.SortedSet;
using EZNEW.Cache.SortedSet.Request;
using EZNEW.Cache.SortedSet.Response;
using EZNEW.Cache.String.Request;
using EZNEW.Cache.String.Response;
using EZNEW.Code;
using EZNEW.Selection;
using EZNEW.Cache.Provider.Memory.Abstractions;
using System.Collections;
using System.IO;

namespace EZNEW.Cache.Provider.Memory
{
    /// <summary>
    /// In memory cache provider
    /// </summary>
    public class MemoryProvider : ICacheProvider
    {
        /// <summary>
        /// Memory cache
        /// </summary>
        static MemoryCache MemoryCache = null;

        static MemoryProvider()
        {
            MemoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
        }

        #region String

        #region StringSetRange

        /// <summary>
        /// Overwrites part of the string stored at key, starting at the specified offset,
        /// for the entire length of value. If the offset is larger than the current length
        /// of the string at key, the string is padded with zero-bytes to make offset fit.
        /// Non-existing keys are considered as empty strings, so this option will make
        /// sure it holds a string large enough to be able to set value at offset.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return string set range response</returns>
        public async Task<StringSetRangeResponse> StringSetRangeAsync(CacheServer server, StringSetRangeOption option)
        {
            string key = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(key))
            {
                return await Task.FromResult(CacheResponse.FailResponse<StringSetRangeResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            if (option.Offset < 0)
            {
                return await Task.FromResult(CacheResponse.FailResponse<StringSetRangeResponse>(CacheCodes.OffsetLessZero)).ConfigureAwait(false);
            }
            var found = MemoryCache.TryGetEntry(key, out ICacheEntry cacheEntry);
            var cacheValue = found ? cacheEntry?.Value?.ToString() ?? string.Empty : string.Empty;
            var currentLength = cacheValue.Length;
            var minLength = option.Offset;
            if (currentLength == minLength)
            {
                cacheValue = cacheValue + option.Value ?? string.Empty;
            }
            else if (currentLength > minLength)
            {
                cacheValue = cacheValue.Insert(minLength, option.Value);
            }
            else
            {
                cacheValue += new string('\x00', minLength - currentLength) + option.Value;
            }
            if (found)
            {
                cacheEntry.SetValue(cacheValue);
            }
            else
            {
                using (var newEntry = MemoryCache.CreateEntry(key))
                {
                    newEntry.Value = cacheValue;
                    SetExpiration(newEntry, option.Expiration);
                }
            }
            var response = CacheResponse.SuccessResponse<StringSetRangeResponse>();
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
        /// <param name="option">Option</param>
        /// <returns>Return string set bit response</returns>
        public async Task<StringSetBitResponse> StringSetBitAsync(CacheServer server, StringSetBitOption option)
        {
            string key = option?.Key?.GetActualKey() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(key))
            {
                return await Task.FromResult(CacheResponse.FailResponse<StringSetBitResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            if (option.Offset < 0)
            {
                return await Task.FromResult(CacheResponse.FailResponse<StringSetBitResponse>(CacheCodes.OffsetLessZero)).ConfigureAwait(false);
            }
            var found = MemoryCache.TryGetEntry(key, out ICacheEntry cacheEntry);
            var bitValue = option.Bit ? '1' : '0';
            var oldBitValue = false;
            var cacheValue = found ? cacheEntry?.Value?.ToString() ?? string.Empty : string.Empty;

            string binaryValue = cacheValue.ToBinaryString(GetEncoding());
            var binaryArray = binaryValue.ToCharArray();
            if (binaryArray.Length > option.Offset)
            {
                oldBitValue = binaryArray[option.Offset] == '1';
                binaryArray[option.Offset] = bitValue;
            }
            else
            {
                var diffLength = option.Offset - binaryArray.LongLength;
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
                using (var entry = MemoryCache.CreateEntry(key))
                {
                    entry.Value = cacheValue;
                    SetExpiration(entry, option.Expiration);
                }
            }
            var response = CacheResponse.SuccessResponse<StringSetBitResponse>();
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
        /// <param name="option">Option</param>
        /// <returns>Return string set response</returns>
        public async Task<StringSetResponse> StringSetAsync(CacheServer server, StringSetOption option)
        {
            if (option?.Items.IsNullOrEmpty() ?? true)
            {
                return CacheResponse.FailResponse<StringSetResponse>(CacheCodes.ValuesIsNullOrEmpty);
            }
            List<StringEntrySetResult> results = new List<StringEntrySetResult>(option.Items.Count);
            foreach (var data in option.Items)
            {
                string cacheKey = data.Key?.GetActualKey() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(cacheKey))
                {
                    continue;
                }
                bool found = MemoryCache.TryGetEntry(cacheKey, out var nowEntry);
                bool setCache = data.When == CacheSetWhen.Always
                    || data.When == CacheSetWhen.Exists && found
                    || data.When == CacheSetWhen.NotExists && !found;
                if (!setCache)
                {
                    continue;
                }
                using (var entry = MemoryCache.CreateEntry(cacheKey))
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
            var response = CacheResponse.SuccessResponse<StringSetResponse>();
            response.Results = results;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region StringLength

        /// <summary>
        /// Returns the length of the string value stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return string length response</returns>
        public async Task<StringLengthResponse> StringLengthAsync(CacheServer server, StringLengthOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<StringLengthResponse>(CacheCodes.KeyIsNullOrEmpty);
            }
            var response = CacheResponse.SuccessResponse<StringLengthResponse>();
            if (MemoryCache.TryGetValue<string>(cacheKey, out var value))
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
        /// <typeparam name="T">data type</typeparam>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return string increment response</returns>
        public async Task<StringIncrementResponse> StringIncrementAsync(CacheServer server, StringIncrementOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<StringIncrementResponse>(CacheCodes.KeyIsNullOrEmpty);
            }
            long nowValue = 0;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (long.TryParse(entry.Value?.ToString(), out nowValue))
                {
                    nowValue += option.Value;
                    entry.SetValue(nowValue);
                }
                else
                {
                    return CacheResponse.FailResponse<StringIncrementResponse>(CacheCodes.ValueCannotBeCalculated);
                }
            }
            else
            {
                using (entry = MemoryCache.CreateEntry(cacheKey))
                {
                    nowValue = option.Value;
                    entry.Value = option.Value;
                    SetExpiration(entry, option.Expiration);
                }
            }
            var response = CacheResponse.SuccessResponse<StringIncrementResponse>();
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
        /// <param name="option">Option</param>
        /// <returns>Return string get with expiry response</returns>
        public async Task<StringGetWithExpiryResponse> StringGetWithExpiryAsync(CacheServer server, StringGetWithExpiryOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<StringGetWithExpiryResponse>(CacheCodes.KeyIsNullOrEmpty);
            }
            string nowValue = string.Empty;
            TimeSpan? expriy = null;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                nowValue = entry.Value?.ToString() ?? string.Empty;
                expriy = GetExpiration(entry);
            }
            var response = CacheResponse.SuccessResponse<StringGetWithExpiryResponse>();
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
        /// <param name="option">Option</param>
        /// <returns>Return string get set response</returns>
        public async Task<StringGetSetResponse> StringGetSetAsync(CacheServer server, StringGetSetOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<StringGetSetResponse>(CacheCodes.KeyIsNullOrEmpty);
            }
            var oldValue = string.Empty;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                oldValue = entry.Value?.ToString() ?? string.Empty;
                entry.SetValue(option.NewValue);
            }
            else
            {
                using (entry = MemoryCache.CreateEntry(cacheKey))
                {
                    entry.SetValue(option.NewValue);
                }
            }
            var response = CacheResponse.SuccessResponse<StringGetSetResponse>();
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
        /// <param name="option">Option</param>
        /// <returns>Return string get range response</returns>
        public async Task<StringGetRangeResponse> StringGetRangeAsync(CacheServer server, StringGetRangeOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<StringGetRangeResponse>(CacheCodes.KeyIsNullOrEmpty);
            }
            var subValue = string.Empty;
            if (MemoryCache.TryGetValue<string>(cacheKey, out var value) && !string.IsNullOrWhiteSpace(value))
            {
                int start = option.Start;
                int end = option.End;
                int valueLength = (value ?? string.Empty).Length;
                if (start < 0)
                {
                    start = value.Length - Math.Abs(start);
                }
                if (start < 0 || start >= valueLength)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<StringGetRangeResponse>(CacheCodes.OffsetError)).ConfigureAwait(false);
                }
                if (end < 0)
                {
                    end = value.Length - Math.Abs(end);
                }
                if (end < 0 || end >= valueLength)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<StringGetRangeResponse>(CacheCodes.OffsetError)).ConfigureAwait(false);
                }
                subValue = value.Substring(Math.Min(start, end), Math.Abs(end - start) + 1);
            }
            var response = CacheResponse.SuccessResponse<StringGetRangeResponse>();
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
        /// <param name="option">Option</param>
        /// <returns>Return string get bit response</returns>
        public async Task<StringGetBitResponse> StringGetBitAsync(CacheServer server, StringGetBitOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<StringGetBitResponse>(CacheCodes.KeyIsNullOrEmpty);
            }
            char bit = '0';
            if (MemoryCache.TryGetValue<string>(cacheKey, out var value) && !string.IsNullOrWhiteSpace(value))
            {
                var binaryArray = value.ToBinaryString(GetEncoding()).ToCharArray();
                var offset = option.Offset;
                if (offset < 0)
                {
                    offset = binaryArray.LongLength - Math.Abs(offset);
                }
                if (offset < 0 || offset >= binaryArray.LongLength)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<StringGetBitResponse>(CacheCodes.OffsetError)).ConfigureAwait(false);
                }
                bit = binaryArray[offset];
            }
            var response = CacheResponse.SuccessResponse<StringGetBitResponse>();
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
        /// <param name="option">Option</param>
        /// <returns>Return string get response</returns>
        public async Task<StringGetResponse> StringGetAsync(CacheServer server, StringGetOption option)
        {
            if (option?.Keys.IsNullOrEmpty() ?? true)
            {
                return CacheResponse.FailResponse<StringGetResponse>(CacheCodes.KeyIsNullOrEmpty);
            }
            var datas = new List<CacheEntry>();
            foreach (var key in option.Keys)
            {
                var cacheKey = key.GetActualKey();
                if (string.IsNullOrWhiteSpace(cacheKey))
                {
                    continue;
                }
                if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
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
            var response = CacheResponse.SuccessResponse<StringGetResponse>();
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
        /// <typeparam name="T">data type</typeparam>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return string decrement response</returns>
        public async Task<StringDecrementResponse> StringDecrementAsync(CacheServer server, StringDecrementOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<StringDecrementResponse>(CacheCodes.KeyIsNullOrEmpty);
            }
            long nowValue = 0;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                if (long.TryParse(entry.Value?.ToString(), out nowValue))
                {
                    nowValue -= option.Value;
                    entry.SetValue(nowValue);
                }
                else
                {
                    return CacheResponse.FailResponse<StringDecrementResponse>(CacheCodes.ValueCannotBeCalculated);
                }
            }
            else
            {
                using (entry = MemoryCache.CreateEntry(cacheKey))
                {
                    nowValue = option.Value;
                    entry.Value = option.Value;
                    SetExpiration(entry, option.Expiration);
                }
            }
            var response = CacheResponse.SuccessResponse<StringDecrementResponse>();
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
        /// <param name="option">Option</param>
        /// <returns>Return string bit position response</returns>
        public async Task<StringBitPositionResponse> StringBitPositionAsync(CacheServer server, StringBitPositionOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<StringBitPositionResponse>(CacheCodes.KeyIsNullOrEmpty);
            }
            if ((option.Start >= 0 && option.End < option.Start) || (option.Start < 0 && option.End > option.Start))
            {
                return CacheResponse.FailResponse<StringBitPositionResponse>(CacheCodes.OffsetError);
            }
            bool hasValue = false;
            long position = 0;
            if (MemoryCache.TryGetValue<string>(cacheKey, out var value) && !string.IsNullOrWhiteSpace(value))
            {
                char[] valueArray = value.ToBinaryString(GetEncoding()).ToCharArray();
                var matchBit = option.Bit ? '1' : '0';
                var length = valueArray.LongLength;
                var start = option.Start;
                var end = option.End;
                if (start < 0)
                {
                    start = length - Math.Abs(start);
                }
                if (start < 0 || start >= length)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<StringBitPositionResponse>(CacheCodes.OffsetError)).ConfigureAwait(false);
                }
                if (end < 0)
                {
                    end = length - Math.Abs(end);
                }
                if (end < 0 || end >= length)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<StringBitPositionResponse>(CacheCodes.OffsetError)).ConfigureAwait(false);
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
            var response = CacheResponse.SuccessResponse<StringBitPositionResponse>();
            response.HasValue = hasValue;
            response.Position = position;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region StringBitOperation

        /// <summary>
        /// Perform a bitwise operation between multiple keys (containing string values)
        ///  and store the result in the destination key. The BITOP option supports four
        ///  bitwise operations; note that NOT is a unary operator: the second key should
        ///  be omitted in this case and only the first key will be considered. The result
        /// of the operation is always stored at destkey.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return string bit operation response</returns>
        public async Task<StringBitOperationResponse> StringBitOperationAsync(CacheServer server, StringBitOperationOption option)
        {
            if (option.Keys.IsNullOrEmpty() || string.IsNullOrWhiteSpace(option.DestinationKey))
            {
                return CacheResponse.FailResponse<StringBitOperationResponse>(CacheCodes.KeyIsNullOrEmpty);
            }
            if (option.Keys.Count > 1 && option.Bitwise == CacheBitwise.Not)
            {
                throw new NotSupportedException($" CacheBitwise.Not can only operate on one key");
            }
            BitArray bitArray = null;
            foreach (var key in option.Keys)
            {
                if (MemoryCache.TryGetEntry(key, out ICacheEntry cacheEntry))
                {
                    var binaryString = (cacheEntry?.Value?.ToString() ?? string.Empty).ToBinaryString(GetEncoding());
                    var binaryArray = new BitArray(binaryString.Select(c => (int)c).ToArray());
                    if (bitArray == null)
                    {
                        bitArray = binaryArray;
                    }
                    else
                    {
                        bitArray = option.Bitwise switch
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
                return CacheResponse.FailResponse<StringBitOperationResponse>(CacheCodes.ValuesIsNullOrEmpty);
            }
            var bitString = string.Join("", bitArray.Cast<bool>().Select(c => c ? 1 : 0));
            var originalString = bitString.ToOriginalString(GetEncoding());
            var desResult = await StringSetAsync(server, new StringSetOption()
            {
                Items = new List<CacheEntry>()
                {
                    new CacheEntry()
                    {
                        Key=option.DestinationKey,
                        Type=CacheKeyType.String,
                        Value=originalString,
                        Expiration=option.Expiration
                    }
                }
            });
            if (desResult.Success)
            {
                return new StringBitOperationResponse()
                {
                    Success = true,
                    DestinationValueLength = originalString.Length
                };
            }
            return CacheResponse.FailResponse<StringBitOperationResponse>("");
        }

        #endregion

        #region StringBitCount

        /// <summary>
        /// Count the number of set bits (population counting) in a string. By default all
        /// the bytes contained in the string are examined.It is possible to specify the
        /// counting operation only in an interval passing the additional arguments start
        /// and end. Like for the GETRANGE option start and end can contain negative values
        /// in order to index bytes starting from the end of the string, where -1 is the
        /// last byte, -2 is the penultimate, and so forth.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return string bit count response</returns>
        public async Task<StringBitCountResponse> StringBitCountAsync(CacheServer server, StringBitCountOption option)
        {
            if (string.IsNullOrWhiteSpace(option?.Key))
            {
                throw new ArgumentNullException($"{nameof(StringBitCountOption)}.{nameof(StringBitCountOption.Key)}");
            }
            var cacheKey = option.Key.GetActualKey();
            long bitCount = 0;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var value = entry.Value?.ToString() ?? string.Empty;
                bitCount = value.ToBinaryString(GetEncoding()).Count(c => c == '1');
            }
            return await Task.FromResult(new StringBitCountResponse()
            {
                Success = true,
                BitNum = bitCount
            }).ConfigureAwait(false);
        }

        #endregion

        #region StringAppend

        /// <summary>
        /// If key already exists and is a string, this option appends the value at the
        /// end of the string. If key does not exist it is created and set as an empty string,
        /// so APPEND will be similar to SET in this special case.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return string append response</returns>
        public async Task<StringAppendResponse> StringAppendAsync(CacheServer server, StringAppendOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return CacheResponse.FailResponse<StringAppendResponse>(CacheCodes.KeyIsNullOrEmpty);
            }
            long valueLength = 0;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var nowValue = entry.Value?.ToString() ?? string.Empty;
                nowValue += option.Value ?? string.Empty;
                valueLength = nowValue.Length;
                entry.SetValue(nowValue);
            }
            else
            {
                using (entry = MemoryCache.CreateEntry(cacheKey))
                {
                    entry.SetValue(option.Value);
                    SetExpiration(entry, option.Expiration);
                }
            }
            var response = CacheResponse.SuccessResponse<StringAppendResponse>();
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
        /// <param name="option">Option</param>
        /// <returns>Return list trim response</returns>
        public async Task<ListTrimResponse> ListTrimAsync(CacheServer server, ListTrimOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<ListTrimResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            ListTrimResponse response = null;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var list = entry.Value as List<string>;
                if (list == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<ListTrimResponse>(CacheCodes.ValueIsNotList)).ConfigureAwait(false);
                }
                var start = option.Start;
                var end = option.Stop;
                int count = list.Count;
                if (start < 0)
                {
                    start = count - Math.Abs(start);
                }
                if (start < 0 || start >= count)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<ListTrimResponse>(CacheCodes.OffsetError)).ConfigureAwait(false);
                }
                if (end < 0)
                {
                    end = count - Math.Abs(end);
                }
                if (end < 0 || end >= count)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<ListTrimResponse>(CacheCodes.OffsetError)).ConfigureAwait(false);
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
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region ListSetByIndex

        /// <summary>
        /// Sets the list element at index to value. For more information on the index argument,
        ///  see ListGetByIndex. An error is returned for out of range indexes.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return list set by index response</returns>
        public async Task<ListSetByIndexResponse> ListSetByIndexAsync(CacheServer server, ListSetByIndexOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<ListSetByIndexResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            ListSetByIndexResponse response = null;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var list = entry.Value as List<string>;
                if (list == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<ListSetByIndexResponse>(CacheCodes.ValueIsNotList)).ConfigureAwait(false);
                }
                var index = option.Index;
                if (index < 0)
                {
                    index = list.Count - Math.Abs(index);
                }
                if (index < 0 || index >= list.Count)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<ListSetByIndexResponse>(CacheCodes.OffsetError)).ConfigureAwait(false);
                }
                list[index] = option.Value;
                response = CacheResponse.SuccessResponse<ListSetByIndexResponse>();
            }
            else
            {
                response = CacheResponse.FailResponse<ListSetByIndexResponse>(CacheCodes.KeyIsNotExist);
            }
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region ListRightPush

        /// <summary>
        /// Insert all the specified values at the tail of the list stored at key. If key
        /// does not exist, it is created as empty list before performing the push operation.
        /// Elements are inserted one after the other to the tail of the list, from the leftmost
        /// element to the rightmost element. So for instance the option RPUSH mylist a
        /// b c will result into a list containing a as first element, b as second element
        /// and c as third element.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return list right push</returns>
        public async Task<ListRightPushResponse> ListRightPushAsync(CacheServer server, ListRightPushOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<ListRightPushResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            if (option.Values.IsNullOrEmpty())
            {
                return await Task.FromResult(CacheResponse.FailResponse<ListRightPushResponse>(CacheCodes.ValuesIsNullOrEmpty)).ConfigureAwait(false);
            }
            ListRightPushResponse response = null;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var list = entry.Value as List<string>;
                if (list == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<ListRightPushResponse>(CacheCodes.ValueIsNotList)).ConfigureAwait(false);
                }
                list = list.Concat(option.Values).ToList();
                entry.SetValue(list);
                response = CacheResponse.SuccessResponse<ListRightPushResponse>();
                response.NewListLength = list.Count;
            }
            else
            {
                using (entry = MemoryCache.CreateEntry(cacheKey))
                {
                    entry.SetValue(new List<string>(option.Values));
                    SetExpiration(entry, option.Expiration);
                }
                response = CacheResponse.SuccessResponse<ListRightPushResponse>();
                response.NewListLength = option.Values.Count;
            }
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
        /// <param name="option">Option</param>
        /// <returns>Return list right pop left response</returns>
        public async Task<ListRightPopLeftPushResponse> ListRightPopLeftPushAsync(CacheServer server, ListRightPopLeftPushOption option)
        {
            string sourceCacheKey = option?.SourceKey?.GetActualKey();
            string destionationCacheKey = option?.DestinationKey?.GetActualKey();
            if (string.IsNullOrWhiteSpace(sourceCacheKey) || string.IsNullOrWhiteSpace(destionationCacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<ListRightPopLeftPushResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            ListRightPopLeftPushResponse response = null;
            string value = string.Empty;
            if (MemoryCache.TryGetEntry(sourceCacheKey, out var entry) && entry != null)
            {
                var list = entry.Value as List<string>;
                if (list == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<ListRightPopLeftPushResponse>(CacheCodes.ValueIsNotList)).ConfigureAwait(false);
                }
                if (list.Count < 1)
                {
                    response = CacheResponse.FailResponse<ListRightPopLeftPushResponse>(CacheCodes.ListIsEmpty);
                }

                List<string> desList = null;
                if (MemoryCache.TryGetEntry(destionationCacheKey, out var desEntry) && desEntry != null)
                {
                    desList = desEntry.Value as List<string>;
                    if (desList == null)
                    {
                        return await Task.FromResult(CacheResponse.FailResponse<ListRightPopLeftPushResponse>(CacheCodes.ValueIsNotList)).ConfigureAwait(false);
                    }
                }
                var index = list.Count - 1;
                value = list[index];
                list.RemoveAt(index);
                if (desEntry == null)
                {
                    using (desEntry = MemoryCache.CreateEntry(destionationCacheKey))
                    {
                        desEntry.Value = new List<string>() { value };
                        SetExpiration(desEntry, option.Expiration);
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
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region ListRightPop

        /// <summary>
        /// Removes and returns the last element of the list stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return list right pop response</returns>
        public async Task<ListRightPopResponse> ListRightPopAsync(CacheServer server, ListRightPopOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<ListRightPopResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            ListRightPopResponse response = null;
            string value = string.Empty;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var list = entry.Value as List<string>;
                if (list == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<ListRightPopResponse>(CacheCodes.ValueIsNotList)).ConfigureAwait(false);
                }
                if (list.Count < 1)
                {
                    response = CacheResponse.FailResponse<ListRightPopResponse>(CacheCodes.ListIsEmpty);
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
            return await Task.FromResult<ListRightPopResponse>(response).ConfigureAwait(false);
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
        /// <param name="option">Option</param>
        /// <returns>Return list remove response</returns>
        public async Task<ListRemoveResponse> ListRemoveAsync(CacheServer server, ListRemoveOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<ListRemoveResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            ListRemoveResponse response = null;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var list = entry.Value as List<string>;
                if (list == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<ListRemoveResponse>(CacheCodes.ValueIsNotList)).ConfigureAwait(false);
                }
                var removeCount = 0;
                if (option.Count == 0)
                {
                    removeCount = list.RemoveAll(a => a == option.Value);
                }
                else
                {
                    var count = Math.Abs(option.Count);
                    var findLast = option.Count < 0;
                    for (var i = 0; i < count; i++)
                    {
                        var index = findLast ? list.FindLastIndex(c => c == option.Value) : list.FindIndex(c => c == option.Value);
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
        /// <param name="option">Option</param>
        /// <returns>Return list range response</returns>
        public async Task<ListRangeResponse> ListRangeAsync(CacheServer server, ListRangeOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<ListRangeResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            ListRangeResponse response = null;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var list = entry.Value as List<string>;
                if (list == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<ListRangeResponse>(CacheCodes.ValueIsNotList)).ConfigureAwait(false);
                }
                var start = option.Start;
                if (start < 0)
                {
                    start = list.Count - Math.Abs(start);
                }
                if (start < 0 || start >= list.Count)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<ListRangeResponse>(CacheCodes.OffsetError)).ConfigureAwait(false);
                }
                var end = option.Stop;
                if (end < 0)
                {
                    end = list.Count - Math.Abs(end);
                }
                if (end < 0 || end >= list.Count)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<ListRangeResponse>(CacheCodes.OffsetError)).ConfigureAwait(false);
                }
                var begin = Math.Min(start, end);
                response = CacheResponse.SuccessResponse<ListRangeResponse>();
                response.Values = list.GetRange(begin, Math.Abs(end - start) + 1).ToList();
            }
            else
            {
                response = CacheResponse.FailResponse<ListRangeResponse>(CacheCodes.KeyIsNotExist);
            }
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region ListLength

        /// <summary>
        /// Returns the length of the list stored at key. If key does not exist, it is interpreted
        ///  as an empty list and 0 is returned.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return list length response</returns>
        public async Task<ListLengthResponse> ListLengthAsync(CacheServer server, ListLengthOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<ListLengthResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            int length = 0;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var list = entry.Value as List<string>;
                if (list == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<ListLengthResponse>(CacheCodes.ValueIsNotList)).ConfigureAwait(false);
                }
                length = list.Count;
            }
            var response = CacheResponse.SuccessResponse<ListLengthResponse>();
            response.Length = length;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region ListLeftPush

        /// <summary>
        /// Insert the specified value at the head of the list stored at key. If key does
        ///  not exist, it is created as empty list before performing the push operations.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return list left push response</returns>
        public async Task<ListLeftPushResponse> ListLeftPushAsync(CacheServer server, ListLeftPushOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<ListLeftPushResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            if (option.Values.IsNullOrEmpty())
            {
                return await Task.FromResult(CacheResponse.FailResponse<ListLeftPushResponse>(CacheCodes.ValuesIsNullOrEmpty)).ConfigureAwait(false);
            }
            ListLeftPushResponse response = null;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                List<string> list = entry.Value as List<string>;
                if (list == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<ListLeftPushResponse>(CacheCodes.ValueIsNotList)).ConfigureAwait(false);
                }
                list = option.Values.Concat(list).ToList();
                entry.SetValue(list);
                response = CacheResponse.SuccessResponse<ListLeftPushResponse>();
                response.NewListLength = list.Count;
            }
            else
            {
                using (entry = MemoryCache.CreateEntry(cacheKey))
                {
                    entry.SetValue(new List<string>(option.Values));
                    SetExpiration(entry, option.Expiration);
                }
                response = CacheResponse.SuccessResponse<ListLeftPushResponse>();
                response.NewListLength = option.Values.Count;
            }
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region ListLeftPop

        /// <summary>
        /// Removes and returns the first element of the list stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return list left pop response</returns>
        public async Task<ListLeftPopResponse> ListLeftPopAsync(CacheServer server, ListLeftPopOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<ListLeftPopResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            ListLeftPopResponse response = null;
            string value = string.Empty;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var list = entry.Value as List<string>;
                if (list == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<ListLeftPopResponse>(CacheCodes.ValueIsNotList)).ConfigureAwait(false);
                }
                if (list.Count < 1)
                {
                    response = CacheResponse.FailResponse<ListLeftPopResponse>(CacheCodes.ListIsEmpty);
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
            return await Task.FromResult<ListLeftPopResponse>(response).ConfigureAwait(false);
        }

        #endregion

        #region ListInsertBefore

        /// <summary>
        /// Inserts value in the list stored at key either before or after the reference
        /// value pivot. When key does not exist, it is considered an empty list and no operation
        /// is performed.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return list insert begore response</returns>
        public async Task<ListInsertBeforeResponse> ListInsertBeforeAsync(CacheServer server, ListInsertBeforeOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<ListInsertBeforeResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            int newLength = 0;
            bool hasInsertValue = false;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var list = entry.Value as List<string>;
                if (list == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<ListInsertBeforeResponse>(CacheCodes.ValueIsNotList)).ConfigureAwait(false);
                }
                var index = list.FindIndex(c => c == option.PivotValue);
                if (index >= 0)
                {
                    list.Insert(index, option.InsertValue);
                    entry.SetValue(list);
                    hasInsertValue = true;
                }
                newLength = list.Count;
            }
            return await Task.FromResult(new ListInsertBeforeResponse()
            {
                Success = hasInsertValue,
                NewListLength = newLength
            }).ConfigureAwait(false);
        }

        #endregion

        #region ListInsertAfter

        /// <summary>
        /// Inserts value in the list stored at key either before or after the reference
        /// value pivot. When key does not exist, it is considered an empty list and no operation
        /// is performed.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return list insert after response</returns>
        public async Task<ListInsertAfterResponse> ListInsertAfterAsync(CacheServer server, ListInsertAfterOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<ListInsertAfterResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            int newLength = 0;
            bool hasInsertValue = false;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var list = entry.Value as List<string>;
                if (list == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<ListInsertAfterResponse>(CacheCodes.ValueIsNotList)).ConfigureAwait(false);
                }
                var index = list.FindIndex(c => c == option.PivotValue);
                if (index >= 0)
                {
                    list.Insert(index + 1, option.InsertValue);
                    entry.SetValue(list);
                    hasInsertValue = true;
                }
                newLength = list.Count;
            }
            return await Task.FromResult(new ListInsertAfterResponse()
            {
                NewListLength = newLength,
                Success = hasInsertValue
            }).ConfigureAwait(false);
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
        /// <param name="option">Option</param>
        /// <returns>Return list get by index response</returns>
        public async Task<ListGetByIndexResponse> ListGetByIndexAsync(CacheServer server, ListGetByIndexOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<ListGetByIndexResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            var value = "";
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var list = entry.Value as List<string>;
                if (list == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<ListGetByIndexResponse>(CacheCodes.ValueIsNotList)).ConfigureAwait(false);
                }
                var index = option.Index;
                if (index < 0)
                {
                    index = list.Count - Math.Abs(index);
                }
                if (index < 0 || index >= list.Count)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<ListGetByIndexResponse>(CacheCodes.OffsetError)).ConfigureAwait(false);
                }
                value = list[index];
            }
            var response = CacheResponse.SuccessResponse<ListGetByIndexResponse>();
            response.Value = value;
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
        /// <param name="option">Option</param>
        /// <returns>Return hash values response</returns>
        public async Task<HashValuesResponse> HashValuesAsync(CacheServer server, HashValuesOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<HashValuesResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            List<dynamic> values = null;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, dynamic>;
                if (dict == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<HashValuesResponse>(CacheCodes.ValueIsNotDict)).ConfigureAwait(false);
                }
                values = dict.Values.ToList();
            }
            values = values ?? new List<dynamic>(0);
            var response = CacheResponse.SuccessResponse<HashValuesResponse>();
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
        /// <param name="option">Option</param>
        /// <returns>Return hash set response</returns>
        public async Task<HashSetResponse> HashSetAsync(CacheServer server, HashSetOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<HashSetResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, dynamic>;
                if (dict == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<HashSetResponse>(CacheCodes.ValueIsNotDict)).ConfigureAwait(false);
                }
                foreach (var item in option.Items)
                {
                    dict[item.Key] = item.Value;
                }
            }
            else
            {
                using (entry = MemoryCache.CreateEntry(cacheKey))
                {
                    var value = new ConcurrentDictionary<string, dynamic>(option.Items);
                    entry.SetValue(value);
                    SetExpiration(entry, option.Expiration);
                }
            }
            var response = CacheResponse.SuccessResponse<HashSetResponse>();
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region HashLength

        /// <summary>
        /// Returns the number of fields contained in the hash stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return hash length response</returns>
        public async Task<HashLengthResponse> HashLengthAsync(CacheServer server, HashLengthOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<HashLengthResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            int length = 0;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, dynamic>;
                if (dict == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<HashLengthResponse>(CacheCodes.ValueIsNotDict)).ConfigureAwait(false);
                }
                length = dict.Keys.Count;
            }
            var response = CacheResponse.SuccessResponse<HashLengthResponse>();
            response.Length = length;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region HashKeys

        /// <summary>
        /// Returns all field names in the hash stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return hash keys response</returns>
        public async Task<HashKeysResponse> HashKeysAsync(CacheServer server, HashKeysOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<HashKeysResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            List<string> keys = null;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, dynamic>;
                if (dict == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<HashKeysResponse>(CacheCodes.ValueIsNotDict)).ConfigureAwait(false);
                }
                keys = dict.Keys.ToList();
            }
            keys = keys ?? new List<string>(0);
            var response = CacheResponse.SuccessResponse<HashKeysResponse>();
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
        /// <typeparam name="T">data type</typeparam>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return hash increment response</returns>
        public async Task<HashIncrementResponse> HashIncrementAsync(CacheServer server, HashIncrementOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<HashIncrementResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            var newValue = option.IncrementValue;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, dynamic>;
                if (dict == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<HashIncrementResponse>(CacheCodes.ValueIsNotDict)).ConfigureAwait(false);
                }
                if (dict.TryGetValue(option.HashField, out var value))
                {
                    dict[option.HashField] = newValue = value + option.IncrementValue;
                }
                else
                {
                    dict[option.HashField] = option.IncrementValue;
                }
            }
            else
            {
                using (entry = MemoryCache.CreateEntry(cacheKey))
                {
                    var value = new ConcurrentDictionary<string, dynamic>();
                    value[option.HashField] = option.IncrementValue;
                    entry.SetValue(value);
                    SetExpiration(entry, option.Expiration);
                }
            }
            var response = CacheResponse.SuccessResponse<HashIncrementResponse>();
            response.HashField = option.HashField;
            response.NewValue = newValue;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region HashGet

        /// <summary>
        /// Returns the value associated with field in the hash stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return hash get response</returns>
        public async Task<HashGetResponse> HashGetAsync(CacheServer server, HashGetOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<HashGetResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            dynamic value = null;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, dynamic>;
                if (dict == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<HashGetResponse>(CacheCodes.ValueIsNotDict)).ConfigureAwait(false);
                }
                dict.TryGetValue(option.HashField, out value);
            }
            var response = CacheResponse.SuccessResponse<HashGetResponse>();
            response.Value = value;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region HashGetAll

        /// <summary>
        /// Returns all fields and values of the hash stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return hash get all response</returns>
        public async Task<HashGetAllResponse> HashGetAllAsync(CacheServer server, HashGetAllOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<HashGetAllResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            ConcurrentDictionary<string, dynamic> values = null;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, dynamic>;
                if (dict == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<HashGetAllResponse>(CacheCodes.ValueIsNotDict)).ConfigureAwait(false);
                }
                values = new ConcurrentDictionary<string, dynamic>(dict);
            }
            var response = CacheResponse.SuccessResponse<HashGetAllResponse>();
            response.HashValues = values?.ToDictionary(c => c.Key, c => c.Value) ?? new Dictionary<string, dynamic>(0);
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region HashExist

        /// <summary>
        /// Returns if field is an existing field in the hash stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return hash exists response</returns>
        public async Task<HashExistsResponse> HashExistAsync(CacheServer server, HashExistsOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<HashExistsResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            bool existKey = false;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, dynamic>;
                if (dict == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<HashExistsResponse>(CacheCodes.ValueIsNotDict)).ConfigureAwait(false);
                }
                existKey = dict.ContainsKey(option.HashField);
            }
            var response = CacheResponse.SuccessResponse<HashExistsResponse>();
            response.HasField = existKey;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region HashDelete

        /// <summary>
        /// Removes the specified fields from the hash stored at key. Non-existing fields
        /// are ignored. Non-existing keys are treated as empty hashes and this option returns 0
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return hash delete response</returns>
        public async Task<HashDeleteResponse> HashDeleteAsync(CacheServer server, HashDeleteOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<HashDeleteResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            bool remove = false;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, dynamic>;
                if (dict == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<HashDeleteResponse>(CacheCodes.ValueIsNotDict)).ConfigureAwait(false);
                }
                foreach (var field in option.HashFields)
                {
                    remove |= dict.TryRemove(field, out var value);
                }
            }
            return await Task.FromResult(new HashDeleteResponse()
            {
                Success = remove
            }).ConfigureAwait(false);
        }

        #endregion

        #region HashDecrement

        /// <summary>
        /// Decrement the specified field of an hash stored at key, and representing a floating
        ///  point number, by the specified decrement. If the field does not exist, it is
        ///  set to 0 before performing the operation.
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return hash decrement response</returns>
        public async Task<HashDecrementResponse> HashDecrementAsync(CacheServer server, HashDecrementOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<HashDecrementResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            var newValue = option.DecrementValue;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, dynamic>;
                if (dict == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<HashDecrementResponse>(CacheCodes.ValueIsNotDict)).ConfigureAwait(false);
                }
                if (dict.TryGetValue(option.HashField, out var value))
                {
                    dict[option.HashField] = newValue = value - option.DecrementValue;
                }
                else
                {
                    dict[option.HashField] = option.DecrementValue;
                }
                entry.SetValue(dict);
            }
            else
            {
                using (entry = MemoryCache.CreateEntry(cacheKey))
                {
                    var value = new ConcurrentDictionary<string, dynamic>();
                    value[option.HashField] = option.DecrementValue;
                    entry.SetValue(value);
                    SetExpiration(entry, option.Expiration);
                }
            }
            var response = CacheResponse.SuccessResponse<HashDecrementResponse>();
            response.HashField = option.HashField;
            response.NewValue = newValue;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region HashScan

        /// <summary>
        /// The HSCAN option is used to incrementally iterate over a hash
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return hash scan response</returns>
        public async Task<HashScanResponse> HashScanAsync(CacheServer server, HashScanOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<HashScanResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            Dictionary<string, dynamic> values = new Dictionary<string, dynamic>();
            if (option.PageSize > 0 && MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, dynamic>;
                if (dict == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<HashScanResponse>(CacheCodes.ValueIsNotDict)).ConfigureAwait(false);
                }
                var pageSize = option.PageSize;
                foreach (var item in dict)
                {
                    bool accordWith = false;
                    switch (option.PatternType)
                    {
                        case PatternType.StartWith:
                            accordWith = item.Key.StartsWith(option.Pattern);
                            break;
                        case PatternType.EndWith:
                            accordWith = item.Key.EndsWith(option.Pattern);
                            break;
                        case PatternType.Include:
                            accordWith = item.Key.Contains(option.Pattern);
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
            var response = CacheResponse.SuccessResponse<HashScanResponse>();
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
        /// <param name="option">Option</param>
        /// <returns>Return set remove response</returns>
        public async Task<SetRemoveResponse> SetRemoveAsync(CacheServer server, SetRemoveOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<SetRemoveResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            int removeCount = 0;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null && !option.RemoveMembers.IsNullOrEmpty())
            {
                var dict = entry.Value as ConcurrentDictionary<string, byte>;
                if (dict == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<SetRemoveResponse>(CacheCodes.ValueIsNotSet)).ConfigureAwait(false);
                }
                foreach (var member in option.RemoveMembers)
                {
                    if (dict.TryRemove(member, out var value))
                    {
                        removeCount++;
                    }
                }
            }
            var response = CacheResponse.SuccessResponse<SetRemoveResponse>();
            response.RemoveCount = removeCount;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region SetRandomMembers

        /// <summary>
        /// Return an array of count distinct elements if count is positive. If called with
        /// a negative count the behavior changes and the option is allowed to return the
        /// same element multiple times. In this case the numer of returned elements is the
        /// absolute value of the specified count.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return set random members response</returns>
        public async Task<SetRandomMembersResponse> SetRandomMembersAsync(CacheServer server, SetRandomMembersOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<SetRandomMembersResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            List<string> members = new List<string>();
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null && option.Count != 0)
            {
                var dict = entry.Value as ConcurrentDictionary<string, byte>;
                if (dict == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<SetRandomMembersResponse>(CacheCodes.ValueIsNotSet)).ConfigureAwait(false);
                }
                bool allowSame = option.Count < 0;
                var count = Math.Abs(option.Count);
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
            var response = CacheResponse.SuccessResponse<SetRandomMembersResponse>();
            response.Members = members;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region SetRandomMember

        /// <summary>
        /// Return a random element from the set value stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return set random member</returns>
        public async Task<SetRandomMemberResponse> SetRandomMemberAsync(CacheServer server, SetRandomMemberOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<SetRandomMemberResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            string member = string.Empty;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, byte>;
                if (dict == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<SetRandomMemberResponse>(CacheCodes.ValueIsNotSet)).ConfigureAwait(false);
                }
                var keys = dict.Keys;
                if (!keys.IsNullOrEmpty())
                {
                    var ranIndex = RandomNumberHelper.GetRandomNumber(keys.Count - 1);
                    member = keys.ElementAt(ranIndex);
                }
            }
            var response = CacheResponse.SuccessResponse<SetRandomMemberResponse>();
            response.Member = member;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region SetPop

        /// <summary>
        /// Removes and returns a random element from the set value stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return set pop response</returns>
        public async Task<SetPopResponse> SetPopAsync(CacheServer server, SetPopOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<SetPopResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            string member = string.Empty;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, byte>;
                if (dict == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<SetPopResponse>(CacheCodes.ValueIsNotSet)).ConfigureAwait(false);
                }
                var keys = dict.Keys;
                if (!keys.IsNullOrEmpty())
                {
                    var ranIndex = RandomNumberHelper.GetRandomNumber(keys.Count - 1);
                    member = keys.ElementAt(ranIndex);
                    dict.TryRemove(member, out var value);
                }
            }
            var response = CacheResponse.SuccessResponse<SetPopResponse>();
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
        /// <param name="option">Option</param>
        /// <returns>Return set move response</returns>
        public async Task<SetMoveResponse> SetMoveAsync(CacheServer server, SetMoveOption option)
        {
            string cacheKey = option?.SourceKey?.GetActualKey();
            string desKey = option?.DestinationKey?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey) || string.IsNullOrWhiteSpace(desKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<SetMoveResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            bool isRemove = false;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, byte>;
                if (dict == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<SetMoveResponse>(CacheCodes.ValueIsNotSet)).ConfigureAwait(false);
                }
                MemoryCache.TryGetEntry(desKey, out var desEntry);
                ConcurrentDictionary<string, byte> desDict = null;
                if (desEntry != null)
                {
                    desDict = desEntry.Value as ConcurrentDictionary<string, byte>;
                    if (desDict == null)
                    {
                        return await Task.FromResult(CacheResponse.FailResponse<SetMoveResponse>(CacheCodes.ValueIsNotSet)).ConfigureAwait(false);
                    }
                }
                if (dict.TryRemove(option.MoveMember, out var value))
                {
                    isRemove = true;
                    if (desDict != null)
                    {
                        desDict[option.MoveMember] = 0;
                    }
                    else
                    {
                        using (desEntry = MemoryCache.CreateEntry(desKey))
                        {
                            desDict = new ConcurrentDictionary<string, byte>();
                            desDict.TryAdd(option.MoveMember, 0);
                            desEntry.SetValue(desDict);
                            SetExpiration(desEntry, option.Expiration);
                        }
                    }
                }
            }
            return await Task.FromResult(new SetMoveResponse()
            {
                Success = isRemove
            }).ConfigureAwait(false);
        }

        #endregion

        #region SetMembers

        /// <summary>
        /// Returns all the members of the set value stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return set members response</returns>
        public async Task<SetMembersResponse> SetMembersAsync(CacheServer server, SetMembersOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<SetMembersResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            List<string> members = null;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, byte>;
                if (dict == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<SetMembersResponse>(CacheCodes.ValueIsNotSet)).ConfigureAwait(false);
                }
                members = new List<string>(dict.Count);
                members.AddRange(dict.Keys);
            }
            members = members ?? new List<string>(0);
            var response = CacheResponse.SuccessResponse<SetMembersResponse>();
            response.Members = members;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region SetLength

        /// <summary>
        /// Returns the set cardinality (number of elements) of the set stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return set length response</returns>
        public async Task<SetLengthResponse> SetLengthAsync(CacheServer server, SetLengthOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<SetLengthResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            int length = 0;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, byte>;
                if (dict == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<SetLengthResponse>(CacheCodes.ValueIsNotSet)).ConfigureAwait(false);
                }
                length = dict.Count;
            }
            var response = CacheResponse.SuccessResponse<SetLengthResponse>();
            response.Length = length;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region SetContains

        /// <summary>
        /// Returns if member is a member of the set stored at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return set contains response</returns>
        public async Task<SetContainsResponse> SetContainsAsync(CacheServer server, SetContainsOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<SetContainsResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            bool existMember = false;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, byte>;
                if (dict == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<SetContainsResponse>(CacheCodes.ValueIsNotSet)).ConfigureAwait(false);
                }
                existMember = dict.ContainsKey(option.Member);
            }
            var response = CacheResponse.SuccessResponse<SetContainsResponse>();
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
        /// <param name="option">Option</param>
        /// <returns>Return set combine response</returns>
        public async Task<SetCombineResponse> SetCombineAsync(CacheServer server, SetCombineOption option)
        {
            if (option?.Keys.IsNullOrEmpty() ?? true)
            {
                return await Task.FromResult(CacheResponse.FailResponse<SetCombineResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            List<string> members = null;
            foreach (var key in option.Keys)
            {
                var cacheKey = key?.GetActualKey() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(cacheKey))
                {
                    continue;
                }
                if (MemoryCache.TryGetEntry(cacheKey, out var nowEntry) && nowEntry != null)
                {
                    var nowDict = nowEntry.Value as ConcurrentDictionary<string, byte>;
                    if (nowDict == null)
                    {
                        return await Task.FromResult(CacheResponse.FailResponse<SetCombineResponse>(CacheCodes.ValueIsNotSet)).ConfigureAwait(false);
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
                        switch (option.CombineOperation)
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
            var response = CacheResponse.SuccessResponse<SetCombineResponse>();
            response.CombineValues = members;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region SetCombineAndStore

        /// <summary>
        /// This option is equal to SetCombine, but instead of returning the resulting set,
        ///  it is stored in destination. If destination already exists, it is overwritten.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return set combine and store response</returns>
        public async Task<SetCombineAndStoreResponse> SetCombineAndStoreAsync(CacheServer server, SetCombineAndStoreOption option)
        {
            var desCacheKey = option.DestinationKey?.GetActualKey() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(desCacheKey) || (option?.SourceKeys.IsNullOrEmpty() ?? true))
            {
                return await Task.FromResult(CacheResponse.FailResponse<SetCombineAndStoreResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            List<string> members = null;
            foreach (var key in option.SourceKeys)
            {
                var cacheKey = key?.GetActualKey() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(cacheKey))
                {
                    continue;
                }
                if (MemoryCache.TryGetEntry(cacheKey, out var nowEntry) && nowEntry != null)
                {
                    var nowDict = nowEntry.Value as ConcurrentDictionary<string, byte>;
                    if (nowDict == null)
                    {
                        return await Task.FromResult(CacheResponse.FailResponse<SetCombineAndStoreResponse>(CacheCodes.ValueIsNotSet)).ConfigureAwait(false);
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
                        switch (option.CombineOperation)
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
            MemoryCache.TryGetEntry(desCacheKey, out var desEntry);
            if (desEntry != null)
            {
                var desDict = desEntry.Value as ConcurrentDictionary<string, byte>;
                if (desDict == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<SetCombineAndStoreResponse>(CacheCodes.ValueIsNotSet)).ConfigureAwait(false);
                }
                foreach (var mem in members)
                {
                    desDict[mem] = 0;
                }
            }
            else
            {
                using (desEntry = MemoryCache.CreateEntry(desCacheKey))
                {
                    ConcurrentDictionary<string, byte> desDict
                        = new ConcurrentDictionary<string, byte>();
                    members.ForEach(m =>
                    {
                        desDict.TryAdd(m, 0);
                    });
                    desEntry.SetValue(desDict);
                    SetExpiration(desEntry, option.Expiration);
                }
            }
            var response = CacheResponse.SuccessResponse<SetCombineAndStoreResponse>();
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
        /// <param name="option">Option</param>
        /// <returns>Return set add response</returns>
        public async Task<SetAddResponse> SetAddAsync(CacheServer server, SetAddOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<SetAddResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, byte>;
                if (dict == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<SetAddResponse>(CacheCodes.ValueIsNotSet)).ConfigureAwait(false);
                }
                foreach (var member in option.Members)
                {
                    dict[member] = 0;
                }
            }
            else
            {
                using (entry = MemoryCache.CreateEntry(cacheKey))
                {
                    ConcurrentDictionary<string, byte> desDict
                        = new ConcurrentDictionary<string, byte>();
                    foreach (var member in option.Members)
                    {
                        desDict[member] = 0;
                    }
                    entry.SetValue(desDict);
                    SetExpiration(entry, option.Expiration);
                }
            }
            return await Task.FromResult(new SetAddResponse()
            {
                Success = true
            }).ConfigureAwait(false);
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
        /// <param name="option">Option</param>
        /// <returns>Return sorted set score response</returns>
        public async Task<SortedSetScoreResponse> SortedSetScoreAsync(CacheServer server, SortedSetScoreOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<SortedSetScoreResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            double? score = null;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, double>;
                if (dict == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<SortedSetScoreResponse>(CacheCodes.ValueIsNotSortedSet)).ConfigureAwait(false);
                }
                if (dict.TryGetValue(option.Member, out var memberScore))
                {
                    score = memberScore;
                }
            }
            var response = CacheResponse.SuccessResponse<SortedSetScoreResponse>();
            response.Score = score;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region SortedSetRemoveRangeByValue

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order
        /// to force lexicographical ordering, this option removes all elements in the sorted
        /// set stored at key between the lexicographical range specified by min and max.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return sorted set remove range by value response</returns>
        public async Task<SortedSetRemoveRangeByValueResponse> SortedSetRemoveRangeByValueAsync(CacheServer server, SortedSetRemoveRangeByValueOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<SortedSetRemoveRangeByValueResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            int removeCount = 0;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, double>;
                if (dict == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<SortedSetRemoveRangeByValueResponse>(CacheCodes.ValueIsNotSortedSet)).ConfigureAwait(false);
                }
                var min = option.MinValue;
                var max = option.MaxValue;
                if (string.Compare(min, max) > 0)
                {
                    min = max;
                    max = option.MinValue;
                }
                var removeValues = dict.Where(c =>
                {
                    return string.Compare(c.Key, min) >= 0 && string.Compare(c.Key, max) <= 0;
                });
                foreach (var removeItem in removeValues)
                {
                    switch (option.Exclude)
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
            var response = CacheResponse.SuccessResponse<SortedSetRemoveRangeByValueResponse>();
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
        /// <param name="option">Option</param>
        /// <returns>Return sorted set remove range by score response</returns>
        public async Task<SortedSetRemoveRangeByScoreResponse> SortedSetRemoveRangeByScoreAsync(CacheServer server, SortedSetRemoveRangeByScoreOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<SortedSetRemoveRangeByScoreResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            int removeCount = 0;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, double>;
                if (dict == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<SortedSetRemoveRangeByScoreResponse>(CacheCodes.ValueIsNotSortedSet)).ConfigureAwait(false);
                }
                var min = option.Start;
                var max = option.Stop;
                if (min > max)
                {
                    min = max;
                    max = option.Start;
                }
                var removeValues = dict.Where(c => c.Value >= min && c.Value <= max);
                foreach (var removeItem in removeValues)
                {
                    switch (option.Exclude)
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
            var response = CacheResponse.SuccessResponse<SortedSetRemoveRangeByScoreResponse>();
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
        /// <param name="option">Option</param>
        /// <returns>Return sorted set remove range by rank response</returns>
        public async Task<SortedSetRemoveRangeByRankResponse> SortedSetRemoveRangeByRankAsync(CacheServer server, SortedSetRemoveRangeByRankOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<SortedSetRemoveRangeByRankResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            int removeCount = 0;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, double>;
                if (dict == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<SortedSetRemoveRangeByRankResponse>(CacheCodes.ValueIsNotSortedSet)).ConfigureAwait(false);
                }
                var min = option.Start;
                var max = option.Stop;
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
                    max = option.Start;
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
            var response = CacheResponse.SuccessResponse<SortedSetRemoveRangeByRankResponse>();
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
        /// <param name="option">Option</param>
        /// <returns>Return sorted set remove response</returns>
        public async Task<SortedSetRemoveResponse> SortedSetRemoveAsync(CacheServer server, SortedSetRemoveOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<SortedSetRemoveResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            int removeCount = 0;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null && !option.RemoveMembers.IsNullOrEmpty())
            {
                var dict = entry.Value as ConcurrentDictionary<string, double>;
                if (dict == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<SortedSetRemoveResponse>(CacheCodes.ValueIsNotSortedSet)).ConfigureAwait(false);
                }
                foreach (var rmem in option.RemoveMembers)
                {
                    if (dict.TryRemove(rmem, out var value))
                    {
                        removeCount++;
                    }
                }
            }
            var response = CacheResponse.SuccessResponse<SortedSetRemoveResponse>();
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
        /// <param name="option">Option</param>
        /// <returns>Return sorted set rank response</returns>
        public async Task<SortedSetRankResponse> SortedSetRankAsync(CacheServer server, SortedSetRankOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<SortedSetRankResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            long? rank = null;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, double>;
                if (dict == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<SortedSetRankResponse>(CacheCodes.ValueIsNotSortedSet)).ConfigureAwait(false);
                }
                if (!dict.IsNullOrEmpty() && dict.ContainsKey(option.Member))
                {
                    rank = -1;
                    IOrderedEnumerable<KeyValuePair<string, double>> ranks = null;
                    if (option.Order == CacheOrder.Ascending)
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
                        if (item.Key == option.Member)
                        {
                            break;
                        }
                    }
                }
            }
            var response = CacheResponse.SuccessResponse<SortedSetRankResponse>();
            response.Rank = rank;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region SortedSetRangeByValue

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order
        /// to force lexicographical ordering, this option returns all the elements in the
        /// sorted set at key with a value between min and max.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return sorted set range by value response</returns>
        public async Task<SortedSetRangeByValueResponse> SortedSetRangeByValueAsync(CacheServer server, SortedSetRangeByValueOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<SortedSetRangeByValueResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            List<string> members = null;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, double>;
                if (dict == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<SortedSetRangeByValueResponse>(CacheCodes.ValueIsNotSortedSet)).ConfigureAwait(false);
                }
                var min = option.MinValue;
                var max = option.MaxValue;
                if (string.Compare(min, max) > 0)
                {
                    min = max;
                    max = option.MinValue;
                }
                var values = dict.Where(c =>
                {
                    return option.Exclude switch
                    {
                        BoundaryExclude.Both => string.Compare(c.Key, min) > 0 && string.Compare(c.Key, max) < 0,
                        BoundaryExclude.Start => string.Compare(c.Key, min) > 0 && string.Compare(c.Key, max) <= 0,
                        BoundaryExclude.Stop => string.Compare(c.Key, min) >= 0 && string.Compare(c.Key, max) < 0,
                        _ => string.Compare(c.Key, min) >= 0 && string.Compare(c.Key, max) <= 0
                    };
                });
                if (option.Order == CacheOrder.Descending)
                {
                    values = values.OrderByDescending(c => c.Key);
                }
                else
                {
                    values = values.OrderBy(c => c.Key);
                }
                if (option.Offset > 0)
                {
                    values = values.Skip(option.Offset);
                }
                if (option.Count >= 0)
                {
                    values = values.Take(option.Count);
                }
                members = values.Select(c => c.Key).ToList();
            }
            var response = CacheResponse.SuccessResponse<SortedSetRangeByValueResponse>();
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
        /// <param name="option">Option</param>
        /// <returns>Return sorted set range by score with scores response</returns>
        public async Task<SortedSetRangeByScoreWithScoresResponse> SortedSetRangeByScoreWithScoresAsync(CacheServer server, SortedSetRangeByScoreWithScoresOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<SortedSetRangeByScoreWithScoresResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            List<SortedSetMember> members = null;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, double>;
                if (dict == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<SortedSetRangeByScoreWithScoresResponse>(CacheCodes.ValueIsNotSortedSet)).ConfigureAwait(false);
                }
                var min = option.Start;
                var max = option.Stop;
                if (min > max)
                {
                    min = max;
                    max = option.Start;
                }
                var values = dict.Where(c =>
                {
                    return option.Exclude switch
                    {
                        BoundaryExclude.Both => c.Value > min && c.Value < max,
                        BoundaryExclude.Start => c.Value > min && c.Value <= max,
                        BoundaryExclude.Stop => c.Value >= min && c.Value < max,
                        _ => c.Value >= min && c.Value <= max,
                    };
                });
                if (option.Order == CacheOrder.Descending)
                {
                    values = values.OrderByDescending(c => c.Value);
                }
                else
                {
                    values = values.OrderBy(c => c.Value);
                }
                if (option.Offset > 0)
                {
                    values = values.Skip(option.Offset);
                }
                if (option.Count >= 0)
                {
                    values = values.Take(option.Count);
                }
                members = values.Select(c => new SortedSetMember()
                {
                    Value = c.Key,
                    Score = c.Value
                }).ToList();
            }
            var response = CacheResponse.SuccessResponse<SortedSetRangeByScoreWithScoresResponse>();
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
        /// <param name="option">Option</param>
        /// <returns>Return sorted set range by score response</returns>
        public async Task<SortedSetRangeByScoreResponse> SortedSetRangeByScoreAsync(CacheServer server, SortedSetRangeByScoreOption option)
        {
            var result = await SortedSetRangeByScoreWithScoresAsync(server, new SortedSetRangeByScoreWithScoresOption()
            {
                CacheObject = option.CacheObject,
                CommandFlags = option.CommandFlags,
                Exclude = option.Exclude,
                Key = option.Key,
                Order = option.Order,
                Offset = option.Offset,
                Count = option.Count,
                Start = option.Start,
                Stop = option.Stop
            }).ConfigureAwait(false);
            if (result == null || !result.Success)
            {
                return CacheResponse.FailResponse<SortedSetRangeByScoreResponse>(result?.Code, result?.Message);
            }
            var response = CacheResponse.SuccessResponse<SortedSetRangeByScoreResponse>();
            response.Members = result.Members?.Select(c => c.Value).ToList() ?? new List<string>(0);
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
        /// <param name="option">Option</param>
        /// <returns>Return sorted set range by rank with scores response</returns>
        public async Task<SortedSetRangeByRankWithScoresResponse> SortedSetRangeByRankWithScoresAsync(CacheServer server, SortedSetRangeByRankWithScoresOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<SortedSetRangeByRankWithScoresResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            List<SortedSetMember> members = null;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, double>;
                if (dict == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<SortedSetRangeByRankWithScoresResponse>(CacheCodes.ValueIsNotSortedSet)).ConfigureAwait(false);
                }
                var min = option.Start;
                var max = option.Stop;
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
                    max = option.Start;
                }
                if (min < dataCount)
                {
                    int skipCount = min;
                    int takeCount = max - min + 1;
                    IEnumerable<KeyValuePair<string, double>> valueDict = dict;
                    if (option.Order == CacheOrder.Descending)
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
            var response = CacheResponse.SuccessResponse<SortedSetRangeByRankWithScoresResponse>();
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
        /// <param name="option">Option</param>
        /// <returns>Return sorted set range by rank response</returns>
        public async Task<SortedSetRangeByRankResponse> SortedSetRangeByRankAsync(CacheServer server, SortedSetRangeByRankOption option)
        {
            var result = await SortedSetRangeByRankWithScoresAsync(server, new SortedSetRangeByRankWithScoresOption()
            {
                CacheObject = option.CacheObject,
                CommandFlags = option.CommandFlags,
                Key = option.Key,
                Order = option.Order,
                Start = option.Start,
                Stop = option.Stop
            }).ConfigureAwait(false);
            if (result == null || !result.Success)
            {
                return CacheResponse.FailResponse<SortedSetRangeByRankResponse>(result?.Code, result?.Message);
            }
            var response = CacheResponse.SuccessResponse<SortedSetRangeByRankResponse>();
            response.Members = result.Members?.Select(c => c.Value).ToList() ?? new List<string>(0);
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region SortedSetLengthByValue

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order
        /// to force lexicographical ordering, this option returns the number of elements
        /// in the sorted set at key with a value between min and max.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">response</param>
        /// <returns>Return sorted set lenght by value response</returns>
        public async Task<SortedSetLengthByValueResponse> SortedSetLengthByValueAsync(CacheServer server, SortedSetLengthByValueOption option)
        {
            var result = await SortedSetRangeByValueAsync(server, new SortedSetRangeByValueOption()
            {
                CacheObject = option.CacheObject,
                CommandFlags = option.CommandFlags,
                Key = option.Key,
                MinValue = option.MinValue,
                MaxValue = option.MaxValue,
                Offset = 0,
                Count = -1
            }).ConfigureAwait(false);
            if (result == null || !result.Success)
            {
                return CacheResponse.FailResponse<SortedSetLengthByValueResponse>(result?.Code, result?.Message);
            }
            var response = CacheResponse.SuccessResponse<SortedSetLengthByValueResponse>();
            response.Length = result.Members?.Count ?? 0;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region SortedSetLength

        /// <summary>
        /// Returns the sorted set cardinality (number of elements) of the sorted set stored
        /// at key.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return sorted set length response</returns>
        public async Task<SortedSetLengthResponse> SortedSetLengthAsync(CacheServer server, SortedSetLengthOption option)
        {
            var result = await SortedSetRangeByScoreAsync(server, new SortedSetRangeByScoreOption()
            {
                CacheObject = option.CacheObject,
                CommandFlags = option.CommandFlags,
                Key = option.Key,
                Offset = 0,
                Count = -1,
            }).ConfigureAwait(false);
            if (result == null || !result.Success)
            {
                return CacheResponse.FailResponse<SortedSetLengthResponse>(result?.Code, result?.Message);
            }
            var response = CacheResponse.SuccessResponse<SortedSetLengthResponse>();
            response.Length = result.Members?.Count ?? 0;
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
        /// <param name="option">Option</param>
        /// <returns>Return sorted set increment response</returns>
        public async Task<SortedSetIncrementResponse> SortedSetIncrementAsync(CacheServer server, SortedSetIncrementOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<SortedSetIncrementResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            double score = 0;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, double>;
                if (dict == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<SortedSetIncrementResponse>(CacheCodes.ValueIsNotSortedSet)).ConfigureAwait(false);
                }
                if (dict.TryGetValue(option.Member, out var memberScore))
                {
                    score = memberScore + option.IncrementScore;
                    dict[option.Member] = score;
                }
            }
            var response = CacheResponse.SuccessResponse<SortedSetIncrementResponse>();
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
        /// <param name="option">Option</param>
        /// <returns>Return sorted set decrement response</returns>
        public async Task<SortedSetDecrementResponse> SortedSetDecrementAsync(CacheServer server, SortedSetDecrementOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<SortedSetDecrementResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            double score = 0;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, double>;
                if (dict == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<SortedSetDecrementResponse>(CacheCodes.ValueIsNotSortedSet)).ConfigureAwait(false);
                }
                if (dict.TryGetValue(option.Member, out var memberScore))
                {
                    score = memberScore - option.DecrementScore;
                    dict[option.Member] = score;
                }
            }
            var response = CacheResponse.SuccessResponse<SortedSetDecrementResponse>();
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
        /// <param name="option">Option</param>
        /// <returns>Return sorted set combine and store response</returns>
        public async Task<SortedSetCombineAndStoreResponse> SortedSetCombineAndStoreAsync(CacheServer server, SortedSetCombineAndStoreOption option)
        {
            var desCacheKey = option.DestinationKey?.GetActualKey() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(desCacheKey) || (option?.SourceKeys.IsNullOrEmpty() ?? true))
            {
                return await Task.FromResult(CacheResponse.FailResponse<SortedSetCombineAndStoreResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            HashSet<string> members = null;
            Dictionary<string, List<double>> allMembers = new Dictionary<string, List<double>>();
            for (int i = 0; i < option.SourceKeys.Count; i++)
            {
                var key = option.SourceKeys[i];
                var cacheKey = key?.GetActualKey() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(cacheKey))
                {
                    continue;
                }
                if (MemoryCache.TryGetEntry(cacheKey, out var nowEntry) && nowEntry != null)
                {
                    var nowDict = nowEntry.Value as ConcurrentDictionary<string, double>;
                    if (nowDict == null)
                    {
                        return await Task.FromResult(CacheResponse.FailResponse<SortedSetCombineAndStoreResponse>(CacheCodes.ValueIsNotSet)).ConfigureAwait(false);
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
                        switch (option.CombineOperation)
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
                    if (option?.Weights?.Length >= i + 1)
                    {
                        weight = option.Weights[i];
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
                    memberScore = option.Aggregate switch
                    {
                        SetAggregate.Max => scores.Max(),
                        SetAggregate.Min => scores.Min(),
                        SetAggregate.Sum => scores.Sum(),
                        _ => 0
                    };
                }
                resultItems.Add(member, memberScore);
            }
            if (MemoryCache.TryGetEntry(desCacheKey, out var desEntry) && desEntry != null)
            {
                var desDict = desEntry.Value as ConcurrentDictionary<string, double>;
                if (desDict == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<SortedSetCombineAndStoreResponse>(CacheCodes.ValueIsNotSet)).ConfigureAwait(false);
                }
                desEntry.Value = resultItems;
            }
            else
            {
                using (desEntry = MemoryCache.CreateEntry(desCacheKey))
                {
                    desEntry.SetValue(resultItems);
                    SetExpiration(desEntry, option.Expiration);
                }
            }
            var response = CacheResponse.SuccessResponse<SortedSetCombineAndStoreResponse>();
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
        /// <param name="option">Option</param>
        /// <returns>Return sorted set add response</returns>
        public async Task<SortedSetAddResponse> SortedSetAddAsync(CacheServer server, SortedSetAddOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<SortedSetAddResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            if (option.Members.IsNullOrEmpty())
            {
                return await Task.FromResult(CacheResponse.FailResponse<SortedSetAddResponse>(CacheCodes.ValuesIsNullOrEmpty)).ConfigureAwait(false);
            }
            long length = 0;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                var dict = entry.Value as ConcurrentDictionary<string, double>;
                if (dict == null)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<SortedSetAddResponse>(CacheCodes.ValueIsNotSortedSet)).ConfigureAwait(false);
                }
                foreach (var mem in option.Members)
                {
                    dict[mem.Value] = mem.Score;
                }
                length = dict.Count;
            }
            else
            {
                using (entry = MemoryCache.CreateEntry(cacheKey))
                {
                    ConcurrentDictionary<string, double> newDict = new ConcurrentDictionary<string, double>();
                    option.Members.ForEach(c =>
                    {
                        newDict.TryAdd(c.Value, c.Score);
                    });
                    length = newDict.Count;
                    entry.SetValue(newDict);
                    SetExpiration(entry, option.Expiration);
                }
            }
            var response = CacheResponse.SuccessResponse<SortedSetAddResponse>();
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
        /// <param name="option">Option</param>
        /// <returns>Return sort response</returns>
        public async Task<SortResponse> SortAsync(CacheServer server, SortOption option)
        {
            var keyTypeResult = await KeyTypeAsync(server, new TypeOption()
            {
                CacheObject = option.CacheObject,
                CommandFlags = option.CommandFlags,
                Key = option.Key
            }).ConfigureAwait(false);
            if (keyTypeResult.Success)
            {
                Func<IEnumerable<string>, IEnumerable<string>> filterValueFuc = (originalValues) =>
                {
                    if (originalValues.IsNullOrEmpty() || originalValues.Count() <= option.Offset)
                    {
                        return Array.Empty<string>();
                    }
                    if (option.Order == CacheOrder.Descending)
                    {
                        originalValues = originalValues.OrderByDescending(c => c);
                    }
                    else
                    {
                        originalValues = originalValues.OrderBy(c => c);
                    }
                    if (option.Offset > 0)
                    {
                        originalValues = originalValues.Skip(option.Offset);
                    }
                    if (option.Count > 0)
                    {
                        originalValues = originalValues.Take(option.Count);
                    }
                    return originalValues;
                };

                IEnumerable<string> values = null;
                bool support = true;
                switch (keyTypeResult.KeyType)
                {
                    case CacheKeyType.List:
                        IEnumerable<string> listValues = (await ListRangeAsync(server, new ListRangeOption()
                        {
                            CacheObject = option.CacheObject,
                            CommandFlags = option.CommandFlags,
                            Key = option.Key,
                            Start = 0,
                            Stop = -1
                        }).ConfigureAwait(false))?.Values ?? new List<string>(0);
                        values = filterValueFuc(listValues);
                        break;
                    case CacheKeyType.Set:
                        IEnumerable<string> setMembers = (await SetMembersAsync(server, new SetMembersOption()
                        {
                            CacheObject = option.CacheObject,
                            CommandFlags = option.CommandFlags,
                            Key = option.Key
                        }).ConfigureAwait(false))?.Members ?? new List<string>(0);
                        values = filterValueFuc(setMembers);
                        break;
                    case CacheKeyType.SortedSet:
                        IEnumerable<SortedSetMember> sortedSetMembers = (await SortedSetRangeByRankWithScoresAsync(server, new SortedSetRangeByRankWithScoresOption()
                        {
                            CacheObject = option.CacheObject,
                            CommandFlags = option.CommandFlags,
                            Key = option.Key,
                            Order = option.Order,
                            Start = 0,
                            Stop = -1
                        }).ConfigureAwait(false))?.Members ?? new List<SortedSetMember>(0);
                        if (sortedSetMembers.Count() <= option.Offset)
                        {
                            values = Array.Empty<string>();
                        }
                        else
                        {
                            if (option.Offset > 0)
                            {
                                sortedSetMembers = sortedSetMembers.Skip(option.Offset);
                            }
                            if (option.Count > 0)
                            {
                                sortedSetMembers = sortedSetMembers.Take(option.Count);
                            }
                            values = sortedSetMembers.Select(c => c.Value).ToList();
                        }
                        break;
                    default:
                        support = false;
                        break;
                }
                if (!support)
                {
                    return await Task.FromResult(CacheResponse.FailResponse<SortResponse>(CacheCodes.OperationIsNotSupported)).ConfigureAwait(false);
                }
                return await Task.FromResult(new SortResponse()
                {
                    Success = true,
                    Values = values?.ToList() ?? new List<string>(0)
                }).ConfigureAwait(false);
            }
            return await Task.FromResult(CacheResponse.FailResponse<SortResponse>(CacheCodes.OperationIsNotSupported)).ConfigureAwait(false);
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
        /// <param name="option">Option</param>
        /// <returns>Return sort and store response</returns>
        public async Task<SortAndStoreResponse> SortAndStoreAsync(CacheServer server, SortAndStoreOption option)
        {
            if (string.IsNullOrWhiteSpace(option?.SourceKey))
            {
                throw new ArgumentNullException($"{nameof(SortAndStoreOption)}.{nameof(SortAndStoreOption.SourceKey)}");
            }
            if (string.IsNullOrWhiteSpace(option?.DestinationKey))
            {
                throw new ArgumentNullException($"{nameof(SortAndStoreOption)}.{nameof(SortAndStoreOption.DestinationKey)}");
            }
            var sortResult = await SortAsync(server, new SortOption()
            {
                CacheObject = option.CacheObject,
                CommandFlags = option.CommandFlags,
                SortType = option.SortType,
                Count = option.Count,
                By = option.By,
                Gets = option.Gets,
                Key = option.SourceKey,
                Offset = option.Offset,
                Order = option.Order
            }).ConfigureAwait(false);

            if (sortResult?.Success ?? false)
            {
                var values = sortResult.Values;
                await ListLeftPushAsync(server, new ListLeftPushOption()
                {
                    CacheObject = option.CacheObject,
                    CommandFlags = option.CommandFlags,
                    Expiration = option.Expiration,
                    Key = option.DestinationKey,
                    Values = values
                }).ConfigureAwait(false);
                return CacheResponse.SuccessResponse<SortAndStoreResponse>();
            }
            return CacheResponse.FailResponse<SortAndStoreResponse>(CacheCodes.OperationIsNotSupported);
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
        /// <param name="option">Option</param>
        /// <returns>Return key type response</returns>
        public async Task<TypeResponse> KeyTypeAsync(CacheServer server, TypeOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<TypeResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            TypeResponse response = null;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
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
                response = TypeResponse.SuccessResponse<TypeResponse>();
                response.KeyType = cacheKeyType;
            }
            else
            {
                response = CacheResponse.FailResponse<TypeResponse>(CacheCodes.KeyIsNotExist);
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
        /// <param name="option">Option</param>
        /// <returns>Return key time to live response</returns>
        public async Task<TimeToLiveResponse> KeyTimeToLiveAsync(CacheServer server, TimeToLiveOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<TimeToLiveResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            TimeToLiveResponse response = null;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                response = CacheResponse.SuccessResponse<TimeToLiveResponse>();
                var expiration = GetExpiration(entry);
                response.TimeToLiveSeconds = (long)(expiration?.TotalSeconds ?? 0);
            }
            else
            {
                response = CacheResponse.FailResponse<TimeToLiveResponse>(CacheCodes.KeyIsNotExist);
            }
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
        /// <param name="option">Option</param>
        /// <returns>Return key restore response</returns>
        public async Task<RestoreResponse> KeyRestoreAsync(CacheServer server, RestoreOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<RestoreResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            using (var entry = MemoryCache.CreateEntry(cacheKey))
            {
                entry.SetValue(GetEncoding().GetString(option.Value));
                SetExpiration(entry, option.Expiration);
            }
            var response = CacheResponse.SuccessResponse<RestoreResponse>();
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region KeyRename

        /// <summary>
        /// Renames key to newkey. It returns an error when the source and destination names
        /// are the same, or when key does not exist.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return key rename response</returns>
        public async Task<RenameResponse> KeyRenameAsync(CacheServer server, RenameOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            string newCacheKey = option?.NewKey?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey) || string.IsNullOrWhiteSpace(newCacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<RenameResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            RenameResponse response = null;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                using (var newEntry = MemoryCache.CreateEntry(newCacheKey))
                {
                    newEntry.SetValue(entry.Value);
                    newEntry.AbsoluteExpiration = entry.AbsoluteExpiration;
                    newEntry.AbsoluteExpirationRelativeToNow = entry.AbsoluteExpirationRelativeToNow;
                    newEntry.Priority = entry.Priority;
                    newEntry.Size = entry.Size;
                    newEntry.SlidingExpiration = entry.SlidingExpiration;
                }
                MemoryCache.Remove(cacheKey);
                response = CacheResponse.SuccessResponse<RenameResponse>();
            }
            else
            {
                response = CacheResponse.FailResponse<RenameResponse>(CacheCodes.KeyIsNotExist);
            }
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region KeyRandom

        /// <summary>
        /// Return a random key from the currently selected database.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return key random response</returns>
        public async Task<RandomResponse> KeyRandomAsync(CacheServer server, RandomOption option)
        {
            var response = CacheResponse.SuccessResponse<RandomResponse>();
            response.Key = MemoryCache.GetRandomKey();
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region KeyPersist

        /// <summary>
        /// Remove the existing timeout on key, turning the key from volatile (a key with
        /// an expire set) to persistent (a key that will never expire as no timeout is associated).
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return key persist response</returns>
        public async Task<PersistResponse> KeyPersistAsync(CacheServer server, PersistOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<PersistResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            PersistResponse response = null;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                using (var newEntry = MemoryCache.CreateEntry(cacheKey))
                {
                    newEntry.SetValue(entry.Value);
                }
                response = CacheResponse.SuccessResponse<PersistResponse>();
            }
            else
            {
                response = CacheResponse.FailResponse<PersistResponse>(CacheCodes.KeyIsNotExist);
            }
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
        /// <param name="option">Option</param>
        /// <returns>Return key move response</returns>
        public async Task<MoveResponse> KeyMoveAsync(CacheServer server, MoveOption option)
        {
            return await Task.FromResult(CacheResponse.FailResponse<MoveResponse>(CacheCodes.OperationIsNotSupported)).ConfigureAwait(false);
        }

        #endregion

        #region KeyMigrate

        /// <summary>
        /// Atomically transfer a key from a source Redis instance to a destination Redis
        /// instance. On success the key is deleted from the original instance by default,
        /// and is guaranteed to exist in the target instance.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return key migrate response</returns>
        public async Task<MigrateResponse> KeyMigrateAsync(CacheServer server, MigrateOption option)
        {
            return await Task.FromResult(CacheResponse.FailResponse<MigrateResponse>(CacheCodes.OperationIsNotSupported)).ConfigureAwait(false);
        }

        #endregion

        #region KeyExpire

        /// <summary>
        /// Set a timeout on key. After the timeout has expired, the key will automatically
        /// be deleted. A key with an associated timeout is said to be volatile in Redis
        /// terminology.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return key expire response</returns>
        public async Task<ExpireResponse> KeyExpireAsync(CacheServer server, ExpireOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<ExpireResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            ExpireResponse response = null;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                SetExpiration(entry, option.Expiration);
                response = CacheResponse.SuccessResponse<ExpireResponse>();
            }
            else
            {
                response = CacheResponse.FailResponse<ExpireResponse>(CacheCodes.KeyIsNotExist);
            }
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion;

        #region KeyDump

        /// <summary>
        /// Serialize the value stored at key in a format and return it to
        /// the user. The returned value can be synthesized back into a Redis key using the
        /// RESTORE option.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return key dump response</returns>
        public async Task<DumpResponse> KeyDumpAsync(CacheServer server, DumpOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<DumpResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            DumpResponse response = null;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
            {
                response = CacheResponse.SuccessResponse<DumpResponse>();
                response.ByteValues = GetEncoding().GetBytes(entry.Value?.ToString() ?? string.Empty);
            }
            else
            {
                response = CacheResponse.FailResponse<DumpResponse>(CacheCodes.KeyIsNotExist);
            }
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region KeyDelete

        /// <summary>
        /// Removes the specified keys. A key is ignored if it does not exist.
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return key delete response</returns>
        public async Task<DeleteResponse> KeyDeleteAsync(CacheServer server, DeleteOption option)
        {
            if (option.Keys?.IsNullOrEmpty() ?? true)
            {
                return await Task.FromResult(CacheResponse.FailResponse<DeleteResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            long deleteCount = 0;
            foreach (var key in option.Keys)
            {
                var cacheKey = key?.GetActualKey() ?? string.Empty;
                if (MemoryCache.TryGetEntry(cacheKey, out var entry))
                {
                    deleteCount++;
                    MemoryCache.Remove(cacheKey);
                }
            }
            var response = CacheResponse.SuccessResponse<DeleteResponse>();
            response.DeleteCount = deleteCount;
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region KeyExists

        /// <summary>
        /// Key exists
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return exists response</returns>
        public async Task<ExistResponse> KeyExistAsync(CacheServer server, ExistOption option)
        {
            if (option.Keys?.IsNullOrEmpty() ?? true)
            {
                return await Task.FromResult(CacheResponse.FailResponse<ExistResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            long count = 0;
            foreach (var key in option.Keys)
            {
                var cacheKey = key?.GetActualKey() ?? string.Empty;
                if (MemoryCache.TryGetEntry(key?.GetActualKey(), out var entry))
                {
                    count++;
                }
            }
            var response = CacheResponse.SuccessResponse<ExistResponse>();
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
        /// <param name="option">Option</param>
        /// <returns>Return get all database response</returns>
        public async Task<GetAllDataBaseResponse> GetAllDataBaseAsync(CacheServer server, GetAllDataBaseOption option)
        {
            var response = CacheResponse.SuccessResponse<GetAllDataBaseResponse>();
            response.Databases = new List<CacheDatabase>()
            {
                new CacheDatabase()
                {
                    Index=0,
                    Name="MemoryCache"
                }
            };
            return await Task.FromResult<GetAllDataBaseResponse>(response).ConfigureAwait(false);
        }

        #endregion

        #region Query keys

        /// <summary>
        /// Query keys
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return get keys response</returns>
        public async Task<GetKeysResponse> GetKeysAsync(CacheServer server, GetKeysOption option)
        {
            var allKeys = MemoryCache.GetAllKeys();
            Func<string, bool> where = c => true;
            int skip = 0;
            int count = allKeys.Count;
            if (option.Query != null)
            {
                skip = (option.Query.Page - 1) * option.Query.PageSize;
                count = option.Query.PageSize;
                switch (option.Query.Type)
                {
                    case PatternType.EndWith:
                        where = c => c.EndsWith(option.Query.MateKey);
                        break;
                    case PatternType.StartWith:
                        where = c => c.StartsWith(option.Query.MateKey);
                        break;
                    case PatternType.Include:
                        where = c => c.Contains(option.Query.MateKey);
                        break;
                }
            }
            var keys = allKeys.Where(c => where(c)).Skip(skip).Take(count).Select(c => ConstantCacheKey.Create(c)).ToList();
            var response = CacheResponse.SuccessResponse<GetKeysResponse>();
            response.Keys = new CachePaging<CacheKey>(option.Query?.Page ?? 1, option.Query?.PageSize ?? allKeys.Count, allKeys.Count, keys);
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region Clear data

        /// <summary>
        /// Clear database data
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return clear data response</returns>
        public async Task<ClearDataResponse> ClearDataAsync(CacheServer server, ClearDataOption option)
        {
            MemoryCache.Compact(1);
            return await Task.FromResult(CacheResponse.SuccessResponse<ClearDataResponse>()).ConfigureAwait(false);
        }

        #endregion

        #region Get cache item detail

        /// <summary>
        /// Get cache item detail
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return get key detail response</returns>
        public async Task<GetDetailResponse> GetKeyDetailAsync(CacheServer server, GetDetailOption option)
        {
            string cacheKey = option?.Key?.GetActualKey();
            if (string.IsNullOrWhiteSpace(cacheKey))
            {
                return await Task.FromResult(CacheResponse.FailResponse<GetDetailResponse>(CacheCodes.KeyIsNullOrEmpty)).ConfigureAwait(false);
            }
            GetDetailResponse response = null;
            if (MemoryCache.TryGetEntry(cacheKey, out var entry) && entry != null)
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
                    Key = option.Key,
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
            return await Task.FromResult(response).ConfigureAwait(false);
        }

        #endregion

        #region Get server config

        /// <summary>
        /// Get server config
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return get server config response</returns>
        public async Task<GetServerConfigurationResponse> GetServerConfigurationAsync(CacheServer server, GetServerConfigurationOption option)
        {
            return await Task.FromResult(CacheResponse.FailResponse<GetServerConfigurationResponse>(CacheCodes.OperationIsNotSupported)).ConfigureAwait(false);
        }

        #endregion

        #region Save server configuration

        /// <summary>
        /// Save server config
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="option">Option</param>
        /// <returns>Return save server config response</returns>
        public async Task<SaveServerConfigurationResponse> SaveServerConfigurationAsync(CacheServer server, SaveServerConfigurationOption option)
        {
            return await Task.FromResult(CacheResponse.FailResponse<SaveServerConfigurationResponse>(CacheCodes.OperationIsNotSupported)).ConfigureAwait(false);
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
        static TimeSpan? GetExpiration(ICacheEntry cacheEntry)
        {
            if (cacheEntry == null)
            {
                return null;
            }
            if (cacheEntry.SlidingExpiration.HasValue)
            {
                return cacheEntry.SlidingExpiration;
            }
            else if (cacheEntry.AbsoluteExpiration.HasValue)
            {
                var nowDate = DateTimeOffset.Now;
                if (cacheEntry.AbsoluteExpiration.Value <= nowDate)
                {
                    return TimeSpan.Zero;
                }
                return cacheEntry.AbsoluteExpiration.Value - nowDate;
            }
            return null;
        }

        /// <summary>
        /// Get encoding
        /// </summary>
        /// <returns></returns>
        static Encoding GetEncoding()
        {
            return CacheManager.Configuration.DefaultEncoding ?? Encoding.UTF8;
        }

        #endregion
    }
}
