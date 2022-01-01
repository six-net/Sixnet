using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Development.Query
{
    /// <summary>
    /// Defines query condition contract
    /// </summary>
    public interface ICondition 
    {
        /// <summary>
        /// Gets or sets the connection operator
        /// </summary>
        CriterionConnectionOperator ConnectionOperator { get; set; }
    }
}
