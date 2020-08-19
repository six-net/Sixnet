using System.Threading.Tasks;
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
using EZNEW.Cache.SortedSet.Request;
using EZNEW.Cache.SortedSet.Response;
using EZNEW.Cache.String.Request;
using EZNEW.Cache.String.Response;

namespace EZNEW.Cache
{
    /// <summary>
    /// Cache provider contract
    /// </summary>
    public interface ICacheProvider
    {
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
        Task<StringSetRangeResponse> StringSetRangeAsync(CacheServer server, StringSetRangeOption option);

        #endregion

        #region StringSetBit

        /// <summary>
        /// Sets or clears the bit at offset in the string value stored at key. The bit is
        /// either set or cleared depending on value, which can be either 0 or 1. When key
        /// does not exist, a new string value is created.The string is grown to make sure
        /// it can hold a bit at offset.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return string set bit response</returns>
        Task<StringSetBitResponse> StringSetBitAsync(CacheServer server, StringSetBitOption option);

        #endregion

        #region StringSet

        /// <summary>
        /// Set key to hold the string value. If key already holds a value, it is overwritten,
        /// regardless of its type.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option"></param>
        /// <returns>Return string set response</returns>
        Task<StringSetResponse> StringSetAsync(CacheServer server, StringSetOption option);

        #endregion

        #region StringLength

        /// <summary>
        /// Returns the length of the string value stored at key.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return string length response</returns>
        Task<StringLengthResponse> StringLengthAsync(CacheServer server, StringLengthOption option);

        #endregion

        #region StringIncrement

        /// <summary>
        /// Increments the string representing a floating point number stored at key by the
        /// specified increment. If the key does not exist, it is set to 0 before performing
        /// the operation. The precision of the output is fixed at 17 digits after the decimal
        /// point regardless of the actual internal precision of the computation.
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return string increment response</returns>
        Task<StringIncrementResponse> StringIncrementAsync(CacheServer server, StringIncrementOption option);

        #endregion

        #region StringGetWithExpiry

        /// <summary>
        /// Get the value of key. If the key does not exist the special value nil is returned.
        /// An error is returned if the value stored at key is not a string, because GET
        /// only handles string values.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return string get with expiry response</returns>
        Task<StringGetWithExpiryResponse> StringGetWithExpiryAsync(CacheServer server, StringGetWithExpiryOption option);

        #endregion

        #region StringGetSet

        /// <summary>
        /// Atomically sets key to value and returns the old value stored at key.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return string get set response</returns>
        Task<StringGetSetResponse> StringGetSetAsync(CacheServer server, StringGetSetOption option);

        #endregion

        #region StringGetRange

        /// <summary>
        /// Returns the substring of the string value stored at key, determined by the offsets
        /// start and end (both are inclusive). Negative offsets can be used in order to
        /// provide an offset starting from the end of the string. So -1 means the last character,
        /// -2 the penultimate and so forth.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return string get range response</returns>
        Task<StringGetRangeResponse> StringGetRangeAsync(CacheServer server, StringGetRangeOption option);

        #endregion

        #region StringGetBit

        /// <summary>
        /// Returns the bit value at offset in the string value stored at key. When offset
        /// is beyond the string length, the string is assumed to be a contiguous space with
        /// 0 bits
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return string get bit response</returns>
        Task<StringGetBitResponse> StringGetBitAsync(CacheServer server, StringGetBitOption option);

        #endregion

        #region StringGet

        /// <summary>
        /// Returns the values of all specified keys. For every key that does not hold a
        /// string value or does not exist, the special value nil is returned.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return string get response</returns>
        Task<StringGetResponse> StringGetAsync(CacheServer server, StringGetOption option);

        #endregion

        #region StringDecrement

        /// <summary>
        /// Decrements the number stored at key by decrement. If the key does not exist,
        /// it is set to 0 before performing the operation. An error is returned if the key
        /// contains a value of the wrong type or contains a string that is not representable
        /// as integer. This operation is limited to 64 bit signed integers.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return string decrement response</returns>
        Task<StringDecrementResponse> StringDecrementAsync(CacheServer server, StringDecrementOption option);

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
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return string bit position response</returns>
        Task<StringBitPositionResponse> StringBitPositionAsync(CacheServer server, StringBitPositionOption option);

        #endregion

        #region StringBitOperation

        /// <summary>
        /// Perform a bitwise operation between multiple keys (containing string values)
        ///  and store the result in the destination key. The BITOP option supports four
        ///  bitwise operations; note that NOT is a unary operator: the second key should
        ///  be omitted in this case and only the first key will be considered. The result
        /// of the operation is always stored at destkey.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return string bit operation response</returns>
        Task<StringBitOperationResponse> StringBitOperationAsync(CacheServer server, StringBitOperationOption option);

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
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return string bit count response</returns>
        Task<StringBitCountResponse> StringBitCountAsync(CacheServer server, StringBitCountOption option);

        #endregion

        #region StringAppend

        /// <summary>
        /// If key already exists and is a string, this option appends the value at the
        /// end of the string. If key does not exist it is created and set as an empty string,
        /// so APPEND will be similar to SET in this special case.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return string append response</returns>
        Task<StringAppendResponse> StringAppendAsync(CacheServer server, StringAppendOption option);

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
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return list trim response</returns>
        Task<ListTrimResponse> ListTrimAsync(CacheServer server, ListTrimOption option);

        #endregion

        #region ListSetByIndex

        /// <summary>
        /// Sets the list element at index to value. For more information on the index argument,
        ///  see ListGetByIndex. An error is returned for out of range indexes.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return list set by index response</returns>
        Task<ListSetByIndexResponse> ListSetByIndexAsync(CacheServer server, ListSetByIndexOption option);

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
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return list right push</returns>
        Task<ListRightPushResponse> ListRightPushAsync(CacheServer server, ListRightPushOption option);

        #endregion

        #region ListRightPopLeftPush

        /// <summary>
        /// Atomically returns and removes the last element (tail) of the list stored at
        /// source, and pushes the element at the first element (head) of the list stored
        /// at destination.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return list right pop left response</returns>
        Task<ListRightPopLeftPushResponse> ListRightPopLeftPushAsync(CacheServer server, ListRightPopLeftPushOption option);

        #endregion

        #region ListRightPop

        /// <summary>
        /// Removes and returns the last element of the list stored at key.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return list right pop response</returns>
        Task<ListRightPopResponse> ListRightPopAsync(CacheServer server, ListRightPopOption option);

        #endregion

        #region ListRemove

        /// <summary>
        /// Removes the first count occurrences of elements equal to value from the list
        /// stored at key. The count argument influences the operation in the following way
        /// count > 0: Remove elements equal to value moving from head to tail. count less 0:
        /// Remove elements equal to value moving from tail to head. count = 0: Remove all
        /// elements equal to value.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return list remove response</returns>
        Task<ListRemoveResponse> ListRemoveAsync(CacheServer server, ListRemoveOption option);

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
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return list range response</returns>
        Task<ListRangeResponse> ListRangeAsync(CacheServer server, ListRangeOption option);

        #endregion

        #region ListLength

        /// <summary>
        /// Returns the length of the list stored at key. If key does not exist, it is interpreted
        ///  as an empty list and 0 is returned.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return list length response</returns>
        Task<ListLengthResponse> ListLengthAsync(CacheServer server, ListLengthOption option);

        #endregion

        #region ListLeftPush

        /// <summary>
        /// Insert the specified value at the head of the list stored at key. If key does
        ///  not exist, it is created as empty list before performing the push operations.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return list left push response</returns>
        Task<ListLeftPushResponse> ListLeftPushAsync(CacheServer server, ListLeftPushOption option);

        #endregion

        #region ListLeftPop

        /// <summary>
        /// Removes and returns the first element of the list stored at key.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return list left pop response</returns>
        Task<ListLeftPopResponse> ListLeftPopAsync(CacheServer server, ListLeftPopOption option);

        #endregion

        #region ListInsertBefore

        /// <summary>
        /// Inserts value in the list stored at key either before or after the reference
        /// value pivot. When key does not exist, it is considered an empty list and no operation
        /// is performed.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return list insert begore response</returns>
        Task<ListInsertBeforeResponse> ListInsertBeforeAsync(CacheServer server, ListInsertBeforeOption option);

        #endregion

        #region ListInsertAfter

        /// <summary>
        /// Inserts value in the list stored at key either before or after the reference
        /// value pivot. When key does not exist, it is considered an empty list and no operation
        /// is performed.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return list insert after response</returns>
        Task<ListInsertAfterResponse> ListInsertAfterAsync(CacheServer server, ListInsertAfterOption option);

        #endregion

        #region ListGetByIndex

        /// <summary>
        /// Returns the element at index index in the list stored at key. The index is zero-based,
        /// so 0 means the first element, 1 the second element and so on. Negative indices
        /// can be used to designate elements starting at the tail of the list. Here, -1
        /// means the last element, -2 means the penultimate and so forth.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return list get by index response</returns>
        Task<ListGetByIndexResponse> ListGetByIndexAsync(CacheServer server, ListGetByIndexOption option);

        #endregion

        #endregion

        #region Hash

        #region HashValues

        /// <summary>
        /// Returns all values in the hash stored at key.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return hash values response</returns>
        Task<HashValuesResponse> HashValuesAsync(CacheServer server, HashValuesOption option);

        #endregion

        #region HashSet

        /// <summary>
        /// Sets field in the hash stored at key to value. If key does not exist, a new key
        ///  holding a hash is created. If field already exists in the hash, it is overwritten.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return hash set response</returns>
        Task<HashSetResponse> HashSetAsync(CacheServer server, HashSetOption option);

        #endregion

        #region HashLength

        /// <summary>
        /// Returns the number of fields contained in the hash stored at key.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return hash length response</returns>
        Task<HashLengthResponse> HashLengthAsync(CacheServer server, HashLengthOption option);

        #endregion

        #region HashKeys

        /// <summary>
        /// Returns all field names in the hash stored at key.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return hash keys response</returns>
        Task<HashKeysResponse> HashKeysAsync(CacheServer server, HashKeysOption option);

        #endregion

        #region HashIncrement

        /// <summary>
        /// Increments the number stored at field in the hash stored at key by increment.
        /// If key does not exist, a new key holding a hash is created. If field does not
        /// exist or holds a string that cannot be interpreted as integer, the value is set
        /// to 0 before the operation is performed.
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return hash increment response</returns>
        Task<HashIncrementResponse> HashIncrementAsync(CacheServer server, HashIncrementOption option);

        #endregion

        #region HashGet

        /// <summary>
        /// Returns the value associated with field in the hash stored at key.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return hash get response</returns>
        Task<HashGetResponse> HashGetAsync(CacheServer server, HashGetOption option);

        #endregion

        #region HashGetAll

        /// <summary>
        /// Returns all fields and values of the hash stored at key.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return hash get all response</returns>
        Task<HashGetAllResponse> HashGetAllAsync(CacheServer server, HashGetAllOption option);

        #endregion

        #region HashExists

        /// <summary>
        /// Returns if field is an existing field in the hash stored at key.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return hash exists response</returns>
        Task<HashExistsResponse> HashExistAsync(CacheServer server, HashExistsOption option);

        #endregion

        #region HashDelete

        /// <summary>
        /// Removes the specified fields from the hash stored at key. Non-existing fields
        /// are ignored. Non-existing keys are treated as empty hashes and this option returns 0
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return hash delete response</returns>
        Task<HashDeleteResponse> HashDeleteAsync(CacheServer server, HashDeleteOption option);

        #endregion

        #region HashDecrement

        /// <summary>
        /// Decrement the specified field of an hash stored at key, and representing a floating
        ///  point number, by the specified decrement. If the field does not exist, it is
        ///  set to 0 before performing the operation.
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return hash decrement response</returns>
        Task<HashDecrementResponse> HashDecrementAsync(CacheServer server, HashDecrementOption option);

        #endregion

        #region HashScan

        /// <summary>
        /// The hash scan option is used to incrementally iterate over a hash
        /// </summary>
        /// <param name="option">Option</param>
        /// <param name="server">Cache server</param>
        /// <returns>Return hash scan response</returns>
        Task<HashScanResponse> HashScanAsync(CacheServer server, HashScanOption option);

        #endregion

        #endregion

        #region Set

        #region SetRemove

        /// <summary>
        /// Remove the specified member from the set stored at key. Specified members that
        /// are not a member of this set are ignored.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return set remove response</returns>
        Task<SetRemoveResponse> SetRemoveAsync(CacheServer server, SetRemoveOption option);

        #endregion

        #region SetRandomMembers

        /// <summary>
        /// Return an array of count distinct elements if count is positive. If called with
        /// a negative count the behavior changes and the option is allowed to return the
        /// same element multiple times. In this case the numer of returned elements is the
        /// absolute value of the specified count.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return set random members response</returns>
        Task<SetRandomMembersResponse> SetRandomMembersAsync(CacheServer server, SetRandomMembersOption option);

        #endregion

        #region SetRandomMember

        /// <summary>
        /// Return a random element from the set value stored at key.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return set random member</returns>
        Task<SetRandomMemberResponse> SetRandomMemberAsync(CacheServer server, SetRandomMemberOption option);

        #endregion

        #region SetPop

        /// <summary>
        /// Removes and returns a random element from the set value stored at key.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return set pop response</returns>
        Task<SetPopResponse> SetPopAsync(CacheServer server, SetPopOption option);

        #endregion

        #region SetMove

        /// <summary>
        /// Move member from the set at source to the set at destination. This operation
        /// is atomic. In every given moment the element will appear to be a member of source
        /// or destination for other clients. When the specified element already exists in
        /// the destination set, it is only removed from the source set.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return set move response</returns>
        Task<SetMoveResponse> SetMoveAsync(CacheServer server, SetMoveOption option);

        #endregion

        #region SetMembers

        /// <summary>
        /// Returns all the members of the set value stored at key.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return set members response</returns>
        Task<SetMembersResponse> SetMembersAsync(CacheServer server, SetMembersOption option);

        #endregion

        #region SetLength

        /// <summary>
        /// Returns the set cardinality (number of elements) of the set stored at key.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return set length response</returns>
        Task<SetLengthResponse> SetLengthAsync(CacheServer server, SetLengthOption option);

        #endregion

        #region SetContains

        /// <summary>
        /// Returns if member is a member of the set stored at key.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return set contains response</returns>
        Task<SetContainsResponse> SetContainsAsync(CacheServer server, SetContainsOption option);

        #endregion

        #region SetCombine

        /// <summary>
        /// Returns the members of the set resulting from the specified operation against
        /// the given sets.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return set combine response</returns>
        Task<SetCombineResponse> SetCombineAsync(CacheServer server, SetCombineOption option);

        #endregion

        #region SetCombineAndStore

        /// <summary>
        /// This option is equal to SetCombine, but instead of returning the resulting set,
        ///  it is stored in destination. If destination already exists, it is overwritten.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return set combine and store response</returns>
        Task<SetCombineAndStoreResponse> SetCombineAndStoreAsync(CacheServer server, SetCombineAndStoreOption option);

        #endregion

        #region SetAdd

        /// <summary>
        /// Add the specified member to the set stored at key. Specified members that are
        /// already a member of this set are ignored. If key does not exist, a new set is
        /// created before adding the specified members.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return set add response</returns>
        Task<SetAddResponse> SetAddAsync(CacheServer server, SetAddOption option);

        #endregion

        #endregion

        #region Sorted set

        #region SortedSetScore

        /// <summary>
        /// Returns the score of member in the sorted set at key; If member does not exist
        /// in the sorted set, or key does not exist, nil is returned.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return sorted set score response</returns>
        Task<SortedSetScoreResponse> SortedSetScoreAsync(CacheServer server, SortedSetScoreOption option);

        #endregion

        #region SortedSetRemoveRangeByValue

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order
        /// to force lexicographical ordering, this option removes all elements in the sorted
        /// set stored at key between the lexicographical range specified by min and max.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return sorted set remove range by value response</returns>
        Task<SortedSetRemoveRangeByValueResponse> SortedSetRemoveRangeByValueAsync(CacheServer server, SortedSetRemoveRangeByValueOption option);

        #endregion

        #region SortedSetRemoveRangeByScore

        /// <summary>
        /// Removes all elements in the sorted set stored at key with a score between min
        ///  and max (inclusive by default).
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return sorted set remove range by score response</returns>
        Task<SortedSetRemoveRangeByScoreResponse> SortedSetRemoveRangeByScoreAsync(CacheServer server, SortedSetRemoveRangeByScoreOption option);

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
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return sorted set remove range by rank response</returns>
        Task<SortedSetRemoveRangeByRankResponse> SortedSetRemoveRangeByRankAsync(CacheServer server, SortedSetRemoveRangeByRankOption option);

        #endregion

        #region SortedSetRemove

        /// <summary>
        /// Removes the specified members from the sorted set stored at key. Non existing
        /// members are ignored.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return sorted set remove response</returns>
        Task<SortedSetRemoveResponse> SortedSetRemoveAsync(CacheServer server, SortedSetRemoveOption option);

        #endregion

        #region SortedSetRank

        /// <summary>
        /// Returns the rank of member in the sorted set stored at key, by default with the
        /// scores ordered from low to high. The rank (or index) is 0-based, which means
        /// that the member with the lowest score has rank 0.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return sorted set rank response</returns>
        Task<SortedSetRankResponse> SortedSetRankAsync(CacheServer server, SortedSetRankOption option);

        #endregion

        #region SortedSetRangeByValue

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order
        /// to force lexicographical ordering, this option returns all the elements in the
        /// sorted set at key with a value between min and max.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return sorted set range by value response</returns>
        Task<SortedSetRangeByValueResponse> SortedSetRangeByValueAsync(CacheServer server, SortedSetRangeByValueOption option);

        #endregion

        #region SortedSetRangeByScoreWithScores

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key. By default
        /// the elements are considered to be ordered from the lowest to the highest score.
        /// Lexicographical order is used for elements with equal score. Start and stop are
        /// used to specify the min and max range for score values. Similar to other range
        /// methods the values are inclusive.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return sorted set range by score with scores response</returns>
        Task<SortedSetRangeByScoreWithScoresResponse> SortedSetRangeByScoreWithScoresAsync(CacheServer server, SortedSetRangeByScoreWithScoresOption option);

        #endregion

        #region SortedSetRangeByScore

        /// <summary>
        /// Returns the specified range of elements in the sorted set stored at key. By default
        /// the elements are considered to be ordered from the lowest to the highest score.
        /// Lexicographical order is used for elements with equal score. Start and stop are
        /// used to specify the min and max range for score values. Similar to other range
        /// methods the values are inclusive.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return sorted set range by score response</returns>
        Task<SortedSetRangeByScoreResponse> SortedSetRangeByScoreAsync(CacheServer server, SortedSetRangeByScoreOption option);

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
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return sorted set range by rank with scores response</returns>
        Task<SortedSetRangeByRankWithScoresResponse> SortedSetRangeByRankWithScoresAsync(CacheServer server, SortedSetRangeByRankWithScoresOption option);

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
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return sorted set range by rank response</returns>
        Task<SortedSetRangeByRankResponse> SortedSetRangeByRankAsync(CacheServer server, SortedSetRangeByRankOption option);

        #endregion

        #region SortedSetLengthByValue

        /// <summary>
        /// When all the elements in a sorted set are inserted with the same score, in order
        /// to force lexicographical ordering, this option returns the number of elements
        /// in the sorted set at key with a value between min and max.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">response</param>
        /// <returns>Return sorted set lenght by value response</returns>
        Task<SortedSetLengthByValueResponse> SortedSetLengthByValueAsync(CacheServer server, SortedSetLengthByValueOption option);

        #endregion

        #region SortedSetLength

        /// <summary>
        /// Returns the sorted set cardinality (number of elements) of the sorted set stored
        /// at key.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return sorted set length response</returns>
        Task<SortedSetLengthResponse> SortedSetLengthAsync(CacheServer server, SortedSetLengthOption option);

        #endregion

        #region SortedSetIncrement

        /// <summary>
        /// Increments the score of member in the sorted set stored at key by increment.
        /// If member does not exist in the sorted set, it is added with increment as its
        /// score (as if its previous score was 0.0).
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return sorted set increment response</returns>
        Task<SortedSetIncrementResponse> SortedSetIncrementAsync(CacheServer server, SortedSetIncrementOption option);

        #endregion

        #region SortedSetDecrement

        /// <summary>
        /// Decrements the score of member in the sorted set stored at key by decrement.
        /// If member does not exist in the sorted set, it is added with -decrement as its
        /// score (as if its previous score was 0.0).
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return sorted set decrement response</returns>
        Task<SortedSetDecrementResponse> SortedSetDecrementAsync(CacheServer server, SortedSetDecrementOption option);

        #endregion

        #region SortedSetCombineAndStore

        /// <summary>
        /// Computes a set operation over multiple sorted sets (optionally using per-set
        /// weights), and stores the result in destination, optionally performing a specific
        /// aggregation (defaults to sum)
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return sorted set combine and store response</returns>
        Task<SortedSetCombineAndStoreResponse> SortedSetCombineAndStoreAsync(CacheServer server, SortedSetCombineAndStoreOption option);

        #endregion

        #region SortedSetAdd

        /// <summary>
        /// Adds all the specified members with the specified scores to the sorted set stored
        /// at key. If a specified member is already a member of the sorted set, the score
        /// is updated and the element reinserted at the right position to ensure the correct
        /// ordering.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return sorted set add response</returns>
        Task<SortedSetAddResponse> SortedSetAddAsync(CacheServer server, SortedSetAddOption option);

        #endregion

        #endregion

        #region Sort

        #region Sort

        /// <summary>
        /// Sorts a list, set or sorted set (numerically or alphabetically, ascending by
        /// default); By default, the elements themselves are compared, but the values can
        /// also be used to perform external key-lookups using the by parameter. By default,
        /// the elements themselves are returned, but external key-lookups (one or many)
        /// can be performed instead by specifying the get parameter (note that # specifies
        /// the element itself, when used in get). Referring to the redis SORT documentation
        /// for examples is recommended. When used in hashes, by and get can be used to specify
        /// fields using -> notation (again, refer to redis documentation).
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return sort response</returns>
        Task<SortResponse> SortAsync(CacheServer server, SortOption option);

        #endregion

        #region SortAndStore

        /// <summary>
        /// Sorts a list, set or sorted set (numerically or alphabetically, ascending by
        /// default); By default, the elements themselves are compared, but the values can
        /// also be used to perform external key-lookups using the by parameter. By default,
        /// the elements themselves are returned, but external key-lookups (one or many)
        /// can be performed instead by specifying the get parameter (note that # specifies
        /// the element itself, when used in get). Referring to the redis SORT documentation
        /// for examples is recommended. When used in hashes, by and get can be used to specify
        /// fields using -> notation (again, refer to redis documentation).
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return sort and store response</returns>
        Task<SortAndStoreResponse> SortAndStoreAsync(CacheServer server, SortAndStoreOption option);

        #endregion

        #endregion

        #region Key

        #region KeyType

        /// <summary>
        /// Returns the string representation of the type of the value stored at key. The
        /// different types that can be returned are: string, list, set, zset and hash.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return key type response</returns>
        Task<TypeResponse> KeyTypeAsync(CacheServer server, TypeOption option);

        #endregion

        #region KeyTimeToLive

        /// <summary>
        /// Returns the remaining time to live of a key that has a timeout. This introspection
        /// capability allows a Redis client to check how many seconds a given key will continue
        /// to be part of the dataset.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return key time to live response</returns>
        Task<TimeToLiveResponse> KeyTimeToLiveAsync(CacheServer server, TimeToLiveOption option);

        #endregion

        #region KeyRestore

        /// <summary>
        /// Create a key associated with a value that is obtained by deserializing the provided
        /// serialized value (obtained via DUMP). If ttl is 0 the key is created without
        /// any expire, otherwise the specified expire time(in milliseconds) is set.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return key restore response</returns>
        Task<RestoreResponse> KeyRestoreAsync(CacheServer server, RestoreOption option);

        #endregion

        #region KeyRename

        /// <summary>
        /// Renames key to newkey. It returns an error when the source and destination names
        /// are the same, or when key does not exist.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return key rename response</returns>
        Task<RenameResponse> KeyRenameAsync(CacheServer server, RenameOption option);

        #endregion

        #region KeyRandom

        /// <summary>
        /// Return a random key from the currently selected database.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return key random response</returns>
        Task<RandomResponse> KeyRandomAsync(CacheServer server, RandomOption option);

        #endregion

        #region KeyPersist

        /// <summary>
        /// Remove the existing timeout on key, turning the key from volatile (a key with
        /// an expire set) to persistent (a key that will never expire as no timeout is associated).
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return key persist response</returns>
        Task<PersistResponse> KeyPersistAsync(CacheServer server, PersistOption option);

        #endregion

        #region KeyMove

        /// <summary>
        /// Move key from the currently selected database (see SELECT) to the specified destination
        /// database. When key already exists in the destination database, or it does not
        /// exist in the source database, it does nothing. It is possible to use MOVE as
        /// a locking primitive because of this.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return key move response</returns>
        Task<MoveResponse> KeyMoveAsync(CacheServer server, MoveOption option);

        #endregion

        #region KeyMigrate

        /// <summary>
        /// Atomically transfer a key from a source Redis instance to a destination Redis
        /// instance. On success the key is deleted from the original instance by default,
        /// and is guaranteed to exist in the target instance.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return key migrate response</returns>
        Task<MigrateResponse> KeyMigrateAsync(CacheServer server, MigrateOption option);

        #endregion

        #region KeyExpire

        /// <summary>
        /// Set a timeout on key. After the timeout has expired, the key will automatically
        /// be deleted. A key with an associated timeout is said to be volatile in Redis
        /// terminology.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return key expire response</returns>
        Task<ExpireResponse> KeyExpireAsync(CacheServer server, ExpireOption option);

        #endregion;

        #region KeyDump

        /// <summary>
        /// Serialize the value stored at key in a Redis-specific format and return it to
        /// the user. The returned value can be synthesized back into a Redis key using the
        /// RESTORE option.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return key dump response</returns>
        Task<DumpResponse> KeyDumpAsync(CacheServer server, DumpOption option);

        #endregion

        #region KeyDelete

        /// <summary>
        /// Removes the specified keys. A key is ignored if it does not exist.
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return key delete response</returns>
        Task<DeleteResponse> KeyDeleteAsync(CacheServer server, DeleteOption option);

        #endregion

        #region KeyExists

        /// <summary>
        /// Key exists
        /// </summary>
        /// <param name="server">server</param>
        /// <param name="option">Option</param>
        /// <returns>Return exists key response</returns>
        Task<ExistResponse> KeyExistAsync(CacheServer server, ExistOption option);

        #endregion

        #endregion

        #region Server

        #region Get all data base

        /// <summary>
        /// Get all database
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return get all database response</returns>
        Task<GetAllDataBaseResponse> GetAllDataBaseAsync(CacheServer server, GetAllDataBaseOption option);

        #endregion

        #region query keys

        /// <summary>
        /// Query keys
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return get keys response</returns>
        Task<GetKeysResponse> GetKeysAsync(CacheServer server, GetKeysOption option);

        #endregion

        #region Clear data

        /// <summary>
        /// Clear database data
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return clear data response</returns>
        Task<ClearDataResponse> ClearDataAsync(CacheServer server, ClearDataOption option);

        #endregion

        #region Get cache item detail

        /// <summary>
        /// Get cache item detail
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return get key detail response</returns>
        Task<GetDetailResponse> GetKeyDetailAsync(CacheServer server, GetDetailOption option);

        #endregion

        #region Get server config

        /// <summary>
        /// Get server config
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return get server configuration response</returns>
        Task<GetServerConfigurationResponse> GetServerConfigurationAsync(CacheServer server, GetServerConfigurationOption option);

        #endregion

        #region Save server config

        /// <summary>
        /// Save server config
        /// </summary>
        /// <param name="server">Cache server</param>
        /// <param name="option">Option</param>
        /// <returns>Return save server config response</returns>
        Task<SaveServerConfigurationResponse> SaveServerConfigurationAsync(CacheServer server, SaveServerConfigurationOption option);

        #endregion

        #endregion
    }
}
