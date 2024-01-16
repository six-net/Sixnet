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
    public class DatabaseQueryStatement : DatabaseStatement
    {
        /// <summary>
        /// Gets or sets the output fields
        /// </summary>
        public IEnumerable<IDataField> OutputFields { get; set; }

        /// <summary>
        /// Whether is complex target
        /// </summary>
        public bool ComplexTarget { get; set; }


        public static DatabaseQueryStatement Create(string script, DataCommandParameters parameters
            , IEnumerable<IDataField> outputFields = null
            , bool complexTarget = false)
        {
            return new DatabaseQueryStatement()
            {
                Script = script,
                Parameters = parameters,
                OutputFields = outputFields,
                ComplexTarget = complexTarget
            };
        }
    }
}
