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
    public class QueryDatabaseStatement : SixnetDatabaseStatement
    {
        /// <summary>
        /// Gets or sets the output fields
        /// </summary>
        public IEnumerable<ISixnetDataField> OutputFields { get; set; }

        /// <summary>
        /// Whether is complex target
        /// </summary>
        public bool ComplexTarget { get; set; }


        public static QueryDatabaseStatement Create(string script, DataCommandParameters parameters
            , IEnumerable<ISixnetDataField> outputFields = null
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
