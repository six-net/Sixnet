using Sixnet.Development.Data.Parameter.Handler;
using System;
using System.Collections.Generic;
using System.Data;

namespace Sixnet.Development.Data
{
    /// <summary>
    /// Database server options
    /// </summary>
    public class DatabaseServerOptions
    {
        internal Dictionary<DbType, ISixnetDataCommandParameterHandler> ParameterHandlers = new();
        internal HashSet<DbType> RemovedParameterHandlerDbTypes = new();

        /// <summary>
        /// Gets or sets the database provider type full name
        /// </summary>
        public string DatabaseProviderFullTypeName { get; set; }

        /// <summary>
        /// Gets or sets the entity options
        /// Key:entity type
        /// </summary>
        public Dictionary<Type, DataEntityOptions> Entities { get; set; }

        /// <summary>
        /// Gets or sets the batch execution options
        /// </summary>
        public DatabaseBatchExecutionOptions BatchExecution { get; set; }

        /// <summary>
        /// Gets or sets the data isolation level
        /// </summary>
        public DataIsolationLevel? DataIsolationLevel { get; set; }

        /// <summary>
        /// Add parameter handler
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="handler"></param>
        public void AddParameterHandler(DbType dbType,  ISixnetDataCommandParameterHandler handler) 
        {
            if(handler!=null)
            {
                ParameterHandlers[dbType] = handler;
            }
        }

        /// <summary>
        /// Add parameter handler
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="handler"></param>
        public void AddParameterHandler(DbType dbType, Func<DataCommandParameterItem, DataCommandParameterItem> handler)
        {
            if (handler != null)
            {
                AddParameterHandler(dbType, new DefaultDataCommandParameterHandler(handler));
            }
        }

        /// <summary>
        /// Remove parameter handler
        /// </summary>
        /// <param name="dbType">DbType</param>
        public void RemoveParameterHandler(DbType dbType)
        {
            RemovedParameterHandlerDbTypes.Add(dbType);
        }
    }
}
