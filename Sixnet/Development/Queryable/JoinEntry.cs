using System;

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Defines join entry
    /// </summary>
    [Serializable]
    public class JoinEntry
    {
        /// <summary>
        /// Gets or sets the join type
        /// </summary>
        public JoinType Type { get; set; }

        /// <summary>
        /// Join connection
        /// </summary>
        public ISixnetQueryable Connection { get; set; }

        /// <summary>
        /// Gets or sets the join target queryable
        /// </summary>
        public ISixnetQueryable TargetQueryable { get; set; }

        /// <summary>
        /// Join index
        /// </summary>
        public int Index { get; internal set; }

        /// <summary>
        /// Clone a new join entry
        /// </summary>
        /// <returns></returns>
        public JoinEntry Clone()
        {
            return new JoinEntry()
            {
                Type = Type,
                Connection = Connection?.Clone(),
                TargetQueryable = TargetQueryable?.Clone(),
                Index = Index
            };
        }
    }
}
