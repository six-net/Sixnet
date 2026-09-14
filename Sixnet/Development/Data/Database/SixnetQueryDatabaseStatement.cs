// "Company © 2025. All rights reserved."

using System.Reflection.Metadata;

using Sixnet.Development.Data.Command;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Queryable;
using Sixnet.Logging;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Defines database query statement
    /// </summary>
    public class SixnetQueryDatabaseStatement : SixnetDatabaseStatement
    {
        private SixnetQueryDatabaseStatement() { }

        /// <summary>
        /// Gets or sets the output fields
        /// </summary>
        public IEnumerable<ISixnetField> OutputFields { get; set; }

        /// <summary>
        /// Whether is complex target
        /// </summary>
        public bool ComplexTarget { get; set; }

        public static SixnetQueryDatabaseStatement Create(SixnetDatabaseType databaseType, SixnetQueryableLocation queryableLocation, string script, SixnetDataCommandParameters parameters
            , IEnumerable<ISixnetField> outputFields = null
            , bool complexTarget = false)
        {

            return Create(databaseType, queryableLocation, data =>
            {
                data.Script = script;
                data.Parameters = parameters;
                data.OutputFields = outputFields;
                data.ComplexTarget = complexTarget;
            });
        }

        public static SixnetQueryDatabaseStatement Create(SixnetDatabaseType databaseType, SixnetQueryableLocation queryableLocation, Action<SixnetQueryDatabaseStatement> configure)
        {
            var data = new SixnetQueryDatabaseStatement();
            configure?.Invoke(data);

            if (queryableLocation == SixnetQueryableLocation.Top)
            {
                SixnetFrameworkLogManager.LogDatabaseScript(data.GetType(), databaseType, data.Script, data.Parameters);
            }

            return data;
        }
    }
}
