using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;

namespace EZNEW.Develop.CQuery
{
    /// <summary>
    /// Join item
    /// </summary>
    [Serializable]
    public class JoinItem
    {
        /// <summary>
        /// Gets or sets the join item sort
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// Gets or sets the join type
        /// </summary>
        public JoinType JoinType { get; set; }

        /// <summary>
        /// Gets or sets the join fields
        /// key:source field,value:target field
        /// </summary>
        public Dictionary<string, string> JoinFields { get; set; }

        /// <summary>
        /// Gets or sets the join query object
        /// </summary>
        public IQuery JoinQuery { get; set; }

        /// <summary>
        /// Gets or sets the extra query
        /// </summary>
        public IQuery ExtraQuery { get; set; }

        /// <summary>
        /// Gets or sets the join operator
        /// </summary>
        public JoinOperator Operator { get; set; }

        /// <summary>
        /// Clone a new join item
        /// </summary>
        /// <returns></returns>
        public JoinItem Clone()
        {
            return new JoinItem()
            {
                Sort = Sort,
                JoinType = JoinType,
                JoinFields = JoinFields?.ToDictionary(c => c.Key, c => c.Value),
                JoinQuery = JoinQuery?.Clone(),
                ExtraQuery = ExtraQuery?.Clone(),
                Operator = Operator
            };
        }
    }
}
