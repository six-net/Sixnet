using Sixnet.Development.Data.Command;
using Sixnet.Development.Data.Command.Event;
using Sixnet.Development.Data.Database;
using Sixnet.Development.Data.Field.Formatting;
using Sixnet.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;

namespace Sixnet.Development.Data
{
    /// <summary>
    /// Database options
    /// </summary>
    public class DataOptions
    {
        internal Dictionary<DatabaseServerType, ISixnetDatabaseProvider> DatabaseProviders = new();
        internal Dictionary<string, ISixnetFieldFormatter> FieldFormatters = new();
        internal List<ISixnetDataCommandStartingEventHandler> DataCommandStartingEventHandlers = new();
        internal List<ISixnetDataCommandCallbackEventHandler> DataCommandCallbackEventHandlers = new();

        /// <summary>
        /// Database options
        /// </summary>
        public Dictionary<DatabaseServerType, DatabaseServerOptions> Databases { get; set; }

        /// <summary>
        /// Database servers
        /// </summary>
        public List<SixnetDatabaseServer> Servers { get; set; }

        /// <summary>
        /// Get data command database server func
        /// </summary>
        public Func<SixnetDataCommand, List<SixnetDatabaseServer>> GetDataCommandDatabaseServerFunc { get; set; } = null;

        /// <summary>
        /// Get database connection func
        /// </summary>
        public Func<SixnetDatabaseServer, IDbConnection> GetDatabaseConnectionFunc { get; set; } = null;

        /// <summary>
        /// Whether disable logical delete
        /// </summary>
        public bool DisableLogicalDelete { get; set; }

        /// <summary>
        /// Add database provider
        /// </summary>
        /// <param name="databaseServerType"></param>
        /// <param name="databaseProvider"></param>
        public void AddDatabaseProvider(DatabaseServerType databaseServerType, ISixnetDatabaseProvider databaseProvider)
        {
            SixnetDirectThrower.ThrowArgNullIf(databaseProvider == null, nameof(databaseProvider));
            DatabaseProviders[databaseServerType] = databaseProvider;
        }

        /// <summary>
        /// Add field formatter
        /// </summary>
        /// <param name="formatterName"></param>
        /// <param name="formatter"></param>
        public void AddFieldFormatter(string formatterName, Func<FormatFieldContext, string> formatter)
        {
            if (!string.IsNullOrWhiteSpace(formatterName) && formatter != null)
            {
                AddFieldFormatter(formatterName, new DefaultFieldFormatter(formatter));
            }
        }

        /// <summary>
        /// Add field formatter
        /// </summary>
        /// <param name="formatterName"></param>
        /// <param name="formatter"></param>
        public void AddFieldFormatter(string formatterName, ISixnetFieldFormatter formatter)
        {
            if (!string.IsNullOrWhiteSpace(formatterName) && formatter != null)
            {
                FieldFormatters[formatterName] = formatter;
            }
        }

        /// <summary>
        /// Subscribe data command starting event
        /// </summary>
        /// <param name="handlers"></param>
        public void SubscribeDataCommandStartingEvent(params ISixnetDataCommandStartingEventHandler[] handlers)
        {
            if (!handlers.IsNullOrEmpty())
            {
                DataCommandStartingEventHandlers.AddRange(handlers);
            }
        }

        /// <summary>
        /// Subscribe data command callback event
        /// </summary>
        /// <param name="handlers"></param>
        public void SubscribeDataCommandCallbackEvent(params ISixnetDataCommandCallbackEventHandler[] handlers)
        {
            if (!handlers.IsNullOrEmpty())
            {
                DataCommandCallbackEventHandlers.AddRange(handlers);
            }
        }
    }
}
