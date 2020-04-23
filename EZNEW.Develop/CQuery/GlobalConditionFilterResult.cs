using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.CQuery
{
    /// <summary>
    /// global condition filter result
    /// </summary>
    public class GlobalConditionFilterResult
    {
        /// <summary>
        /// condition
        /// </summary>
        public IQuery Condition
        {
            get; set;
        }

        /// <summary>
        /// append method
        /// </summary>
        public QueryOperator AppendMethod
        {
            get; set;
        } = QueryOperator.AND;

        /// <summary>
        /// append origin query
        /// </summary>
        /// <param name="originQuery">origin query</param>
        public void AppendTo(IQuery originQuery)
        {
            originQuery?.SetGlobalCondition(Condition, AppendMethod);
        }
    }
}
