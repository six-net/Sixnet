using System.Collections.Generic;

namespace Sixnet.Cache
{
    /// <summary>
    /// Cache code
    /// </summary>
    public static class SixnetCacheCodes
    {
        #region Codes

        /// <summary>
        /// key is null or empty
        /// </summary>
        public const string KeyIsNullOrEmpty = "3100";

        /// <summary>
        /// key is not exist
        /// </summary>
        public const string KeyIsNotExist = "3101";

        /// <summary>
        /// offset less zero
        /// </summary>
        public const string OffsetLessZero = "4100";

        /// <summary>
        /// offset error
        /// </summary>
        public const string OffsetError = "41001";

        /// <summary>
        /// values is null or empty
        /// </summary>
        public const string ValuesIsNullOrEmpty = "5100";

        /// <summary>
        /// value is not list
        /// </summary>
        public const string ValueIsNotList = "5101";

        /// <summary>
        /// list is empty
        /// </summary>
        public const string ListIsEmpty = "5102";

        /// <summary>
        /// value is not dict
        /// </summary>
        public const string ValueIsNotDict = "5103";

        /// <summary>
        /// value is not set
        /// </summary>
        public const string ValueIsNotSet = "5104";

        /// <summary>
        /// value is not sorted set
        /// </summary>
        public const string ValueIsNotSortedSet = "5104";

        /// <summary>
        /// value cannot be calculated
        /// </summary>
        public const string ValueCannotBeCalculated = "5105";

        /// <summary>
        /// operation is not supported
        /// </summary>
        public const string OperationIsNotSupported = "0000";

        /// <summary>
        /// no execution results were returned
        /// </summary>
        public const string NoResults = "5000";

        /// <summary>
        /// not configuration cache server
        /// </summary>
        public const string ServerIsNull = "5001";

        /// <summary>
        /// not configuration database
        /// </summary>
        public const string DatabaseIsNull = "5001";

        #endregion

        #region Messages

        /// <summary>
        /// Code messages
        /// </summary>
        public static Dictionary<string, string> CodeMessages = new Dictionary<string, string>()
        {
            { KeyIsNullOrEmpty,"The cache key value is empty"},
            { OffsetLessZero,"The offset is less than 0"},
            { ValuesIsNullOrEmpty,"Value is empty"},
            { OffsetError,"Error setting offset"},
            { ValueIsNotList,"The value is not a list type"},
            { ListIsEmpty,"The list is empty"},
            { ValueIsNotDict,"The value is not a Hash object"},
            { ValueCannotBeCalculated,"The value cannot be calculated"},
            { OperationIsNotSupported,"The operation is not supported"},
            { NoResults,"No results were returned, and possibly no cache operations were performed"},
            { ServerIsNull,"Not set cache server"},
            { DatabaseIsNull,"Not set cache database"},
        };

        #endregion
    }
}
