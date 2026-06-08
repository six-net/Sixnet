// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Defines join entry
    /// </summary>
    [Serializable]
    public class SixnetJoinEntry
    {
        /// <summary>
        /// Gets or sets the join type
        /// </summary>
        public SixnetJoinType Type { get; set; }

        /// <summary>
        /// Join connection
        /// </summary>
        public ISixnetQueryable Connection { get; set; }

        /// <summary>
        /// Gets or sets the join target queryable
        /// </summary>
        public ISixnetQueryable Target { get; set; }

        /// <summary>
        /// Join index
        /// </summary>
        public int Index { get; internal set; }

        /// <summary>
        /// Clone a new join entry
        /// </summary>
        /// <returns></returns>
        public SixnetJoinEntry Clone()
        {
            return new SixnetJoinEntry()
            {
                Type = Type,
                Connection = Connection?.Clone(),
                Target = Target?.Clone(),
                Index = Index
            };
        }
    }
}
