using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Development.Query
{
    /// <summary>
    /// Defines join criterion contract
    /// </summary>
    public interface IJoinCriterion : IInnerClone<IJoinCriterion>
    {
        /// <summary>
        /// Determins whether is right criterion
        /// </summary>
        bool IsRightCriterion { get; set; }

        /// <summary>
        /// Gets or sets the connector
        /// </summary>
        CriterionConnector Connector { get; set; }
    }
}
