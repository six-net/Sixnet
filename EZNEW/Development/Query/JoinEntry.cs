using System;
using System.Collections.Generic;
using System.Linq;

namespace EZNEW.Development.Query
{
    /// <summary>
    /// Defines join entry
    /// </summary>
    [Serializable]
    public class JoinEntry
    {
        /// <summary>
        /// Gets or sets the join item sort
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// Gets or sets the join type
        /// </summary>
        public JoinType Type { get; set; }

        /// <summary>
        /// Gets or sets the join criteria
        /// </summary>
        public List<IJoinCriterion> JoinCriteria { get; set; }

        /// <summary>
        /// Gets or sets the join query object
        /// </summary>
        public IQuery JoinObjectFilter { get; set; }

        /// <summary>
        /// Gets or sets the extra query
        /// </summary>
        public IQuery JoinObjectExtraFilter { get; set; }

        /// <summary>
        /// Clone a new join item
        /// </summary>
        /// <returns></returns>
        public JoinEntry Clone()
        {
            return new JoinEntry()
            {
                Sort = Sort,
                Type = Type,
                JoinCriteria = JoinCriteria?.Select(c => c?.Clone()).ToList(),
                JoinObjectFilter = JoinObjectFilter?.Clone(),
                JoinObjectExtraFilter = JoinObjectExtraFilter?.Clone()
            };
        }
    }
}
