using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.CQuery
{
    public class JoinItem
    {
        /// <summary>
        /// sort
        /// </summary>
        public int Sort
        {
            get; set;
        }

        /// <summary>
        /// join type
        /// </summary>
        public JoinType JoinType
        {
            get; set;
        }

        /// <summary>
        /// join fields
        /// key:source field,value:target field
        /// </summary>
        public Dictionary<string, string> JoinFields
        {
            get;set;
        }

        /// <summary>
        /// join query
        /// </summary>
        public IQuery JoinQuery
        {
            get; set;
        }

        /// <summary>
        /// join operator
        /// </summary>
        public JoinOperator Operator
        {
            get; set;
        }
    }
}
