using System;
using Sixnet.Development.Data.Field;

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Tree info
    /// </summary>
    [Serializable]
    public class TreeInfo
    {
        /// <summary>
        /// Gets or sets the data field
        /// </summary>
        public IDataField DataField { get; set; }

        /// <summary>
        /// Gets or sets the parent field
        /// </summary>
        public IDataField ParentField { get; set; }

        /// <summary>
        /// Gets or sets the matching direction
        /// </summary>
        public TreeMatchingDirection Direction { get; set; }

        /// <summary>
        /// Clone a new treeinfo
        /// </summary>
        /// <returns></returns>
        public TreeInfo Clone()
        {
            return new TreeInfo()
            {
                DataField = DataField?.Clone(),
                Direction = Direction,
                ParentField = ParentField?.Clone()
            };
        }
    }
}
