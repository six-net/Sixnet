using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Development.Query
{
    /// <summary>
    /// Defines query join criterion
    /// </summary>
    public class QueryJoinCriterion : IJoinCriterion
    {
        /// <summary>
        /// Determins whether is right criterion
        /// </summary>
        public bool IsRightCriterion { get; set; } = false;

        /// <summary>
        /// Gets or sets the connector
        /// </summary>
        public CriterionConnector Connector { get; set; } = CriterionConnector.And;

        /// <summary>
        /// Gets or sets the join criteria
        /// </summary>
        public IQuery Criteria { get; set; }

        public static QueryJoinCriterion Create(IQuery query, bool isRightCriterion = false, CriterionConnector connector = CriterionConnector.And)
        {
            return new QueryJoinCriterion()
            {
                Criteria = query,
                Connector = connector,
                IsRightCriterion = isRightCriterion
            };
        }

        public IJoinCriterion Clone()
        {
            return new QueryJoinCriterion()
            {
                IsRightCriterion = IsRightCriterion,
                Connector = Connector,
                Criteria = Criteria?.Clone(),
            };
        }
    }
}
