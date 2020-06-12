using System;
using System.Collections.Generic;

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
        public int Sort
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the join type
        /// </summary>
        public JoinType JoinType
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the join fields
        /// key:source field,value:target field
        /// </summary>
        public Dictionary<string, string> JoinFields
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the join query object
        /// </summary>
        public IQuery JoinQuery
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the join operator
        /// </summary>
        public JoinOperator Operator
        {
            get; set;
        }
    }
}
