using System;
using Sixnet.Development.Data.Field;

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Tree match options
    /// </summary>
    [Serializable]
    public class TreeMatchOptions
    {
        /// <summary>
        /// Gets or sets the data field
        /// </summary>
        public ISixnetDataField DataField { get; set; }

        /// <summary>
        /// Gets or sets the parent field
        /// </summary>
        public ISixnetDataField ParentField { get; set; }

        /// <summary>
        /// Gets or sets the matching direction
        /// </summary>
        public TreeMatchingDirection Direction { get; set; }

        /// <summary>
        /// Clone a new treeinfo
        /// </summary>
        /// <returns></returns>
        public TreeMatchOptions Clone()
        {
            return new TreeMatchOptions()
            {
                DataField = DataField?.Clone(),
                Direction = Direction,
                ParentField = ParentField?.Clone()
            };
        }
    }
}
