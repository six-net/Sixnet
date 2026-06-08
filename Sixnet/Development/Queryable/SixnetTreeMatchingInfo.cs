// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Field;

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Tree match info
    /// </summary>
    [Serializable]
    public class SixnetTreeMatchingInfo
    {
        /// <summary>
        /// Gets or sets the data field
        /// </summary>
        public ISixnetField DataField { get; set; }

        /// <summary>
        /// Gets or sets the parent field
        /// </summary>
        public ISixnetField ParentField { get; set; }

        /// <summary>
        /// Gets or sets the matching direction
        /// </summary>
        public SixnetTreeMatchingDirection Direction { get; set; }

        /// <summary>
        /// Clone a new treeinfo
        /// </summary>
        /// <returns></returns>
        public SixnetTreeMatchingInfo Clone()
        {
            return new SixnetTreeMatchingInfo()
            {
                DataField = DataField?.Clone(),
                Direction = Direction,
                ParentField = ParentField?.Clone()
            };
        }
    }
}
