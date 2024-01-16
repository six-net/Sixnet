using System;

namespace Sixnet.Development.Data.Event
{
    /// <summary>
    /// Defines data event type
    /// </summary>
    [Serializable]
    public enum DataEventType
    {
        /// <summary>
        /// Adding data
        /// </summary>
        Adding = 2,
        /// <summary>
        /// Added data
        /// </summary>
        Added = 3,
        /// <summary>
        /// Updating data
        /// </summary>
        Updating = 4,
        /// <summary>
        /// Updated data
        /// </summary>
        Updated = 5,
        /// <summary>
        /// deleting data
        /// </summary>
        Deleting = 6,
        /// <summary>
        /// Deleted data
        /// </summary>
        Deleted = 7,
        /// <summary>
        /// Querying data
        /// </summary>
        Querying = 8,
        /// <summary>
        /// Queried data
        /// </summary>
        Queried = 9,
        /// <summary>
        /// Getting value
        /// </summary>
        GettingValue = 10,
        /// <summary>
        /// Got value
        /// </summary>
        GotValue = 11,
        /// <summary>
        /// Checking data
        /// </summary>
        Checking = 12,
        /// <summary>
        /// Checked data
        /// </summary>
        Checked = 13
    }
}
