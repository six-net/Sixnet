using System;
using System.Collections.Generic;
using System.Text;
using Sixnet.Development.Data.Command;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Entity;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Defines database query statement
    /// </summary>
    public class QueryDatabaseStatement : DatabaseStatement
    {
        /// <summary>
        /// Gets or sets the output fields
        /// </summary>
        public IEnumerable<ISixnetField> OutputFields { get; set; }

        /// <summary>
        /// Whether is complex target
        /// </summary>
        public bool ComplexTarget { get; set; }

        public static QueryDatabaseStatement Create(string script, DataCommandParameters parameters
            , IEnumerable<ISixnetField> outputFields = null
            , bool complexTarget = false)
        {
            return new QueryDatabaseStatement()
            {
                Script = script,
                Parameters = parameters,
                OutputFields = outputFields,
                ComplexTarget = complexTarget
            };
        }
    }
}
