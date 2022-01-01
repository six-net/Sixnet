using System;

namespace EZNEW.Selection
{
    /// <summary>
    /// Define a selection matching pattern
    /// </summary>
    [Serializable]
    public enum SelectionMatchPattern
    {
        /// <summary>
        /// The first data
        /// </summary>
        First = 2,
        /// <summary>
        /// The latest data
        /// </summary>
        Latest = 4,
        /// <summary>
        /// Randomly selected data
        /// </summary>
        Random = 8,
        /// <summary>
        /// Equally likely to randomly select data
        /// </summary>
        EquiprobableRandom = 16,
        /// <summary>
        /// Take turns to choose
        /// </summary>
        Polling = 32
    }
}
