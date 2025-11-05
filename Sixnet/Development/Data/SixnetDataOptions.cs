// "Company © 2025. All rights reserved."

using System.Data;

using Sixnet.Development.Data.Command;
using Sixnet.Development.Data.Command.Event;
using Sixnet.Development.Data.Database;
using Sixnet.Development.Data.Field.Formatting;
using Sixnet.Development.Data.Parameter.Handler;
using Sixnet.Development.Data.ParameterHandler.Handler;
using Sixnet.Development.Entity;
using Sixnet.Development.Queryable;
using Sixnet.Exceptions;
using Sixnet.Expressions.Linq;

namespace Sixnet.Development.Data
{
    /// <summary>
    /// Sixnet database options
    /// </summary>
    public class SixnetDataOptions
    {
        #region Fields

        readonly Dictionary<DatabaseType, DatabaseSetting> _databaseSettings = new();
        readonly Dictionary<string, ISixnetFieldFormatter> _fieldFormatters = new();
        readonly List<ISixnetDataCommandStartingEventHandler> _dataCommandStartingEventHandlers = new();
        readonly List<ISixnetDataCommandCallbackEventHandler> _dataCommandCallbackEventHandlers = new();
        readonly Dictionary<Type, ISixnetCondition> _typeFilters = new();
        readonly Dictionary<string, ISixnetSplitTableProvider> _splitTableProviders = new(); // key: provider name
        readonly Dictionary<DataIsolationLevel, IsolationLevel> _systemDataIsolationLevels = new()
        {
            { DataIsolationLevel.Chaos, IsolationLevel.Chaos },
            { DataIsolationLevel.ReadCommitted, IsolationLevel.ReadCommitted },
            { DataIsolationLevel.ReadUncommitted, IsolationLevel.ReadUncommitted },
            { DataIsolationLevel.RepeatableRead, IsolationLevel.RepeatableRead },
            { DataIsolationLevel.Serializable, IsolationLevel.Serializable },
            { DataIsolationLevel.Snapshot, IsolationLevel.Snapshot },
            { DataIsolationLevel.Unspecified, IsolationLevel.Unspecified }
        };
        FieldRole _ignoreFilterFieldRole = FieldRole.None;
        Func<SixnetDataCommand, List<DatabaseServer>> _getDataCommandDatabaseServers;
        Func<DatabaseServer, IDbConnection> _getDatabaseConnection;
        Func<QueryableFilterContext, ISixnetQueryable> _getCustomContextFilter;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Database servers
        /// </summary>
        public List<DatabaseServer> Servers { get; set; }

        /// <summary>
        /// Whether disable logical delete.
        /// Performing a physical delete when it is true
        /// </summary>
        public bool DisableLogicalDelete { get; set; }

        /// <summary>
        /// Gets or sets the paging total count field name
        /// </summary>
        public string PagingTotalFieldName { get; set; } = "SixnetPagingTotalDataCount";

        /// <summary>
        /// Gets or sets the paging total count split field name
        /// </summary>
        public string PagingTotalSplitFieldName { get; set; } = "SixnetPagingTotalDataCountSplit";

        /// <summary>
        /// Gets or sets the default paging size.
        /// Default is 20
        /// </summary>
        public int DefaultPagingSize { get; set; } = 20;

        /// <summary>
        /// Gets or sets the data command database server match pattern
        /// </summary>
        public DatabaseServerMatchPattern DatabaseServerMatchPattern { get; set; } = DatabaseServerMatchPattern.Default;

        /// <summary>
        /// Whether auto create split table
        /// Default is true
        /// </summary>
        public bool AutoCreateSplitTable { get; set; } = true;

        /// <summary>
        /// Whether insert increment field.
        /// Default is false
        /// </summary>
        public bool InsertIncrementField { get; set; }

        /// <summary>
        /// Gets or sets the default database word and name pattern
        /// </summary>
        public DatabaseWordAndNamePattern DatabaseWordAndNamePattern { get; set; } = DatabaseWordAndNamePattern.Original;

        /// <summary>
        /// Gets or sets the default database word and name separator
        /// </summary>
        public string DatabaseWordAndNameSeparator { get; set; } = "_";

        #endregion

        #region Constructor

        public SixnetDataOptions()
        {
            AddDefaultParameterHandler();
            SubscribeDefaultCommandStartingEvent();
        }

        #endregion

        #region Connection

        /// <summary>
        /// Configure connection
        /// </summary>
        /// <param name="configure">Configure</param>
        public void ConfigureConnection(Func<DatabaseServer, IDbConnection> configure)
        {
            _getDatabaseConnection = configure;
        }

        /// <summary>
        /// Get connection
        /// </summary>
        /// <param name="server">Database server</param>
        /// <returns></returns>
        internal IDbConnection GetConnection(DatabaseServer server)
        {
            return _getDatabaseConnection?.Invoke(server);
        }

        #endregion

        #region Servers

        /// <summary>
        /// Configure data command servers
        /// </summary>
        /// <param name="configure">Configure</param>
        public void ConfigureDataCommandServers(Func<SixnetDataCommand, List<DatabaseServer>> configure)
        {
            _getDataCommandDatabaseServers = configure;
        }

        /// <summary>
        /// Get data command database servers
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns></returns>
        internal List<DatabaseServer> GetDataCommandDatabaseServers(SixnetDataCommand command)
        {
            List<DatabaseServer> servers = null;
            if (_getDataCommandDatabaseServers == null)
            {
                switch (DatabaseServerMatchPattern)
                {
                    case DatabaseServerMatchPattern.Default:
                        servers = GetDefaultDatabaseServers();
                        break;
                    case DatabaseServerMatchPattern.All:
                        servers = GetAllDatabaseServers();
                        break;
                }
            }
            else
            {
                servers = _getDataCommandDatabaseServers.Invoke(command);
            }
            return servers ?? new List<DatabaseServer>(0);
        }

        /// <summary>
        /// Get default database servers
        /// </summary>
        /// <returns></returns>
        internal List<DatabaseServer> GetDefaultDatabaseServers()
        {
            var allServers = GetAllDatabaseServers();
            if (allServers.IsNullOrEmpty())
            {
                return new List<DatabaseServer>(0);
            }
            var defaultServers = allServers.Where(c => c != null && c.Role == DatabaseServerRole.Default);
            if (defaultServers.IsNullOrEmpty() && allServers.Count == 1)
            {
                return new List<DatabaseServer>(1) { allServers.First() };
            }
            return defaultServers?.ToList() ?? new List<DatabaseServer>(0);
        }

        /// <summary>
        /// Get all config database servers
        /// </summary>
        /// <returns></returns>
        internal List<DatabaseServer> GetAllDatabaseServers()
        {
            return Servers ?? new List<DatabaseServer>(0);
        }

        /// <summary>
        /// Get config database server by names
        /// </summary>
        /// <param name="serverName">Database server name</param>
        /// <returns></returns>
        internal List<DatabaseServer> GetDatabaseServers(string serverName)
        {
            return GetDatabaseServers(new string[1] { serverName });
        }

        /// <summary>
        /// Get config database server by names
        /// </summary>
        /// <param name="serverNames">Database server names</param>
        /// <returns></returns>
        internal List<DatabaseServer> GetDatabaseServers(IEnumerable<string> serverNames)
        {
            if (serverNames.IsNullOrEmpty())
            {
                return new List<DatabaseServer>(0);
            }
            var allServers = GetAllDatabaseServers();
            return allServers?.Where(s => serverNames.Contains(s.Name)).ToList()
                ?? new List<DatabaseServer>(0);
        }

        #endregion

        #region Database provider

        /// <summary>
        /// Add database provider
        /// </summary>
        /// <param name="databaseType"></param>
        /// <param name="databaseProvider"></param>
        public void AddDatabaseProvider(DatabaseType databaseType, ISixnetDatabaseProvider databaseProvider)
        {
            SixnetDirectThrower.ThrowArgNullIf(databaseProvider == null, nameof(databaseProvider));

            GetDatabaseSetting(databaseType).DatabaseProvider = databaseProvider;
        }

        /// <summary>
        /// Get database provider
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <returns></returns>
        internal ISixnetDatabaseProvider GetDatabaseProvider(DatabaseType databaseType)
        {
            return GetDatabaseSetting(databaseType).DatabaseProvider;
        }

        #endregion

        #region Field formatter

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
                _fieldFormatters[formatterName] = formatter;
            }
        }

        /// <summary>
        /// Get field formatter
        /// </summary>
        /// <param name="formatterName">Formatter name</param>
        /// <returns></returns>
        internal ISixnetFieldFormatter GetFieldFormatter(string formatterName)
        {
            if (string.IsNullOrWhiteSpace(formatterName))
            {
                return null;
            }
            _fieldFormatters.TryGetValue(formatterName, out var formatter);
            return formatter;
        }

        #endregion

        #region Data command event

        /// <summary>
        /// Subscribe data command starting event
        /// </summary>
        /// <param name="handlers"></param>
        public void SubscribeCommandStartingEvent(params ISixnetDataCommandStartingEventHandler[] handlers)
        {
            if (!handlers.IsNullOrEmpty())
            {
                _dataCommandStartingEventHandlers.AddRange(handlers);
            }
        }

        /// <summary>
        /// Subscribe data command callback event
        /// </summary>
        /// <param name="handlers"></param>
        public void SubscribeCommandCallbackEvent(params ISixnetDataCommandCallbackEventHandler[] handlers)
        {
            if (!handlers.IsNullOrEmpty())
            {
                _dataCommandCallbackEventHandlers.AddRange(handlers);
            }
        }

        /// <summary>
        /// Get command callback event handlers
        /// </summary>
        /// <returns></returns>
        internal List<ISixnetDataCommandCallbackEventHandler> GetCommandCallbackEventHandlers()
        {
            return _dataCommandCallbackEventHandlers;
        }

        /// <summary>
        /// Get command starting event handlers
        /// </summary>
        internal List<ISixnetDataCommandStartingEventHandler> GetCommandStartingEventHandlers()
        {
            return _dataCommandStartingEventHandlers;
        }

        /// <summary>
        /// Subscribe default data command starting event
        /// </summary>
        void SubscribeDefaultCommandStartingEvent()
        {
            _dataCommandStartingEventHandlers.Add(new InterceptParameterDataCommandStartingHandler());
            _dataCommandStartingEventHandlers.Add(new HandleQueryableDataCommandStartingEventHandler());
        }

        #endregion

        #region Data filter

        /// <summary>
        /// Configure custom filter
        /// </summary>
        /// <param name="configure">Configure</param>
        public void ConfigureCustomFilter(Func<QueryableFilterContext, ISixnetQueryable> configure)
        {
            _getCustomContextFilter = configure;
        }

        /// <summary>
        /// Ignore role filter
        /// </summary>
        /// <param name="fieldRoles">Field roles</param>
        public void IgnoreRoleFilter(params FieldRole[] fieldRoles)
        {
            if (!fieldRoles.IsNullOrEmpty())
            {
                foreach (var role in fieldRoles)
                {
                    _ignoreFilterFieldRole |= role;
                }
            }
        }

        /// <summary>
        /// Has ignored role filter
        /// </summary>
        /// <param name="fieldRole">Field role</param>
        /// <returns></returns>
        public bool HasIgnoredRoleFilter(FieldRole fieldRole)
        {
            return (_ignoreFilterFieldRole & fieldRole) == fieldRole;
        }

        /// <summary>
        /// Add data filter
        /// </summary>
        /// <typeparam name="TFilter"></typeparam>
        /// <param name="condition">Condition</param>
        public void AddFilter<TFilter>(ISixnetCondition condition)
        {
            if (condition != null)
            {
                _typeFilters[typeof(TFilter)] = condition;
            }
        }

        /// <summary>
        /// Add data filter
        /// </summary>
        /// <typeparam name="TFilter"></typeparam>
        /// <param name="filter">Filter expression</param>
        public void AddFilter<TFilter>(Expression<Func<TFilter, bool>> filter)
        {
            var condition = SixnetExpressionHelper.GetQueryable(filter, CriterionConnector.And);
            AddFilter<TFilter>(condition);
        }

        /// <summary>
        /// Whether is filter type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsFilterType(Type type)
        {
            return type != null && _typeFilters.ContainsKey(type);
        }

        /// <summary>
        /// Get type filters
        /// </summary>
        /// <returns></returns>
        internal Dictionary<Type, ISixnetCondition> GetTypeFilters()
        {
            return _typeFilters;
        }

        /// <summary>
        /// Get custom filter
        /// </summary>
        /// <returns></returns>
        internal Func<QueryableFilterContext, ISixnetQueryable> GetCustomFilter()
        {
            return _getCustomContextFilter;
        }

        #endregion

        #region Parameter handler

        /// <summary>
        /// Add parameter handler
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <param name="dbType">Database data type</param>
        /// <param name="parameterHandler">Parameter handler</param>
        public void AddParameterHandler(DatabaseType databaseType, DbType dbType, ISixnetDataCommandParameterHandler handler)
        {
            SixnetDirectThrower.ThrowArgNullIf(handler == null, nameof(handler));
            GetDatabaseSetting(databaseType).AddParameterHandler(dbType, handler);
        }

        /// <summary>
        /// Remove parameter handler
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <param name="dbType">Database data type</param>
        public void RemoveParameterHandler(DatabaseType databaseType, DbType dbType)
        {
            GetDatabaseSetting(databaseType).RemoveParameterHandler(dbType);
        }

        /// <summary>
        /// Gets parameter handler
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <param name="dbType">dbType</param>
        /// <returns></returns>
        internal ISixnetDataCommandParameterHandler GetParameterHandler(DatabaseType databaseType, DbType dbType)
        {
            return GetDatabaseSetting(databaseType).GetParameterHandler(dbType);
        }

        /// <summary>
        /// Configure default parameter handler
        /// </summary>
        void AddDefaultParameterHandler()
        {
            var datetimeOffsetHandler = new DateTimeOffsetParameterHandler();
            var boolToIntegerHandler = new BooleanToIntegerParameterHandler();
            var guidHandler = new GuidFormattingParameterHandler();
            var sbyteHandler = new SByteToShortParameterHandler();
            var uintHandler = new UIntToLongParameterHandler();
            var ushortHandler = new UShortToIntParameterHandler();
            var ulongHandler = new ULongToDecimalParameterHandler();
            var timespanHandler = new TimeSpanParameterHandler();
            var charHandler = new CharToStringParameterHandler();
            var ulongToStringHandler = new ULongToStringParameterHandler();
            var nullCharHandler = new NullCharacterParameterHandler();

            #region MySQL

            //DateTimeOffset
            AddParameterHandler(DatabaseType.MySQL, DbType.DateTimeOffset, datetimeOffsetHandler);

            #endregion

            #region Oracle

            //boolean
            AddParameterHandler(DatabaseType.Oracle, DbType.Boolean, boolToIntegerHandler);
            //Guid
            AddParameterHandler(DatabaseType.Oracle, DbType.Guid, guidHandler);
            //SByte
            AddParameterHandler(DatabaseType.Oracle, DbType.SByte, sbyteHandler);
            //UInt
            AddParameterHandler(DatabaseType.Oracle, DbType.UInt32, uintHandler);
            //UShort
            AddParameterHandler(DatabaseType.Oracle, DbType.UInt16, ushortHandler);
            //ULong
            AddParameterHandler(DatabaseType.Oracle, DbType.UInt64, ulongHandler);
            //Timespan
            AddParameterHandler(DatabaseType.Oracle, DbType.Time, timespanHandler);
            //Char
            AddParameterHandler(DatabaseType.Oracle, DbType.StringFixedLength, charHandler);

            #endregion

            #region SQLite

            //DateTimeOffset
            AddParameterHandler(DatabaseType.SQLite, DbType.DateTimeOffset, datetimeOffsetHandler);
            //ULong
            AddParameterHandler(DatabaseType.SQLite, DbType.UInt64, ulongToStringHandler);

            #endregion

            #region SQL Server

            //SByte
            AddParameterHandler(DatabaseType.SQLServer, DbType.SByte, sbyteHandler);
            //UInt
            AddParameterHandler(DatabaseType.SQLServer, DbType.UInt32, uintHandler);
            //UShort
            AddParameterHandler(DatabaseType.SQLServer, DbType.UInt16, ushortHandler);
            //ULong
            AddParameterHandler(DatabaseType.SQLServer, DbType.UInt64, ulongHandler);

            #endregion

            #region PostgreSQL

            //UInt
            AddParameterHandler(DatabaseType.PostgreSQL, DbType.UInt32, uintHandler);
            //ULong
            AddParameterHandler(DatabaseType.PostgreSQL, DbType.UInt64, ulongHandler);
            //UShort
            AddParameterHandler(DatabaseType.PostgreSQL, DbType.UInt16, ushortHandler);
            //null char
            AddParameterHandler(DatabaseType.PostgreSQL, DbType.AnsiString, nullCharHandler);
            AddParameterHandler(DatabaseType.PostgreSQL, DbType.AnsiStringFixedLength, nullCharHandler);
            AddParameterHandler(DatabaseType.PostgreSQL, DbType.String, nullCharHandler);
            AddParameterHandler(DatabaseType.PostgreSQL, DbType.StringFixedLength, nullCharHandler);

            #endregion
        }

        #endregion

        #region Batch setting

        /// <summary>
        /// Set batch setting
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <param name="setting">Batch setting</param>
        public void SetBatchSetting(DatabaseType databaseType, DatabaseBatchSetting setting)
        {
            GetDatabaseSetting(databaseType).BatchSetting = setting;
        }

        /// <summary>
        /// Get batch setting
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <returns></returns>
        internal DatabaseBatchSetting GetBatchSetting(DatabaseType databaseType)
        {
            return GetDatabaseSetting(databaseType).BatchSetting;
        }

        #endregion

        #region Isolation level

        /// <summary>
        /// Set database default isolation level
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <param name="dataIsolationLevel">Data isolation level</param>
        public void SetDefaultIsolationLevel(DatabaseType databaseType, DataIsolationLevel dataIsolationLevel)
        {
            GetDatabaseSetting(databaseType).IsolationLevel = dataIsolationLevel;
        }

        /// <summary>
        /// Get database default isolation level
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <returns></returns>
        internal DataIsolationLevel? GetDefaultIsolationLevel(DatabaseType databaseType)
        {
            return GetDatabaseSetting(databaseType).IsolationLevel;
        }

        /// <summary>
        /// Get system isolation level by data isolation level
        /// </summary>
        /// <param name="dataIsolationLevel">Data isolation level</param>
        /// <returns></returns>
        internal IsolationLevel? GetSystemIsolationLevel(DataIsolationLevel? dataIsolationLevel)
        {
            IsolationLevel? isolationLevel = null;
            if (dataIsolationLevel.HasValue && _systemDataIsolationLevels.ContainsKey(dataIsolationLevel.Value))
            {
                isolationLevel = _systemDataIsolationLevels[dataIsolationLevel.Value];
            }
            return isolationLevel;
        }

        #endregion

        #region Entity setting

        /// <summary>
        /// Configure entity
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <param name="entityType">Entity type</param>
        /// <param name="configure">Configure</param>
        public void ConfigureEntity(DatabaseType databaseType, Type entityType, Action<EntitySetting> configure)
        {
            GetDatabaseSetting(databaseType).ConfigureEntity(entityType, configure);
        }

        /// <summary>
        /// Get entity setting
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <param name="entityType">Entity type</param>
        /// <returns></returns>
        internal EntitySetting GetEntitySetting(DatabaseType databaseType, Type entityType)
        {
            return GetDatabaseSetting(databaseType).GetEntitySetting(entityType);
        }

        #endregion

        #region Increment

        /// <summary>
        /// Whether allow insert increment field
        /// </summary>
        /// <param name="commandExecutionContext">Command execution context</param>
        /// <returns></returns>
        internal bool AllowInsertIncrementField(DataCommandExecutionContext commandExecutionContext)
        {
            var databaseSetting = GetDatabaseSetting(commandExecutionContext.Server.DatabaseType);
            var isInsert = databaseSetting.InsertIncrementField.HasValue ? databaseSetting.InsertIncrementField.Value : InsertIncrementField;
            return commandExecutionContext.Command.Options?.AllowInsertIncrementField(isInsert)
                   ?? isInsert;
        }

        #endregion

        #region Split table provider

        /// <summary>
        /// Add split table provider
        /// </summary>
        /// <param name="name">Provider name</param>
        /// <param name="provider">Provider</param>
        public void AddSplitTableProvider(string name, ISixnetSplitTableProvider provider)
        {
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(name), nameof(name));
            SixnetDirectThrower.ThrowArgNullIf(provider == null, nameof(provider));
            _splitTableProviders[name] = provider;
        }

        /// <summary>
        /// Get split table provider
        /// </summary>
        /// <param name="name">Provider name</param>
        /// <returns></returns>
        internal ISixnetSplitTableProvider GetSplitTableProvider(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }
            _splitTableProviders.TryGetValue(name, out var provider);
            return provider;
        }

        #endregion

        #region Format database word and name

        /// <summary>
        /// Format database word and name
        /// </summary>
        /// <param name="databaseType">Databae type</param>
        /// <param name="orginalValue">Orginal value</param>
        /// <returns></returns>
        public string FormatDatabaseWordAndName(DatabaseType databaseType, string orginalValue)
        {
            if (string.IsNullOrWhiteSpace(orginalValue))
            {
                return string.Empty;
            }
            var namePattern = DatabaseWordAndNamePattern;
            var nameSeparator = DatabaseWordAndNameSeparator;
            var databaseSetting = GetDatabaseSetting(databaseType);
            if (databaseSetting?.DatabaseWordAndNamePattern != null)
            {
                namePattern = databaseSetting.DatabaseWordAndNamePattern.Value;
            }
            if (!string.IsNullOrWhiteSpace(databaseSetting?.DatabaseWordAndNameSeparator))
            {
                nameSeparator = databaseSetting.DatabaseWordAndNameSeparator;
            }
            var formattedValue = orginalValue;
            switch (namePattern)
            {
                case DatabaseWordAndNamePattern.Uppercase:
                    formattedValue = orginalValue.ToUpper();
                    break;
                case DatabaseWordAndNamePattern.Lowercase:
                    formattedValue = orginalValue.ToLower();
                    break;
                case DatabaseWordAndNamePattern.UppercaseWithSeparator:
                    formattedValue = orginalValue.ToSeparatorCase(nameSeparator, true);
                    break;
                case DatabaseWordAndNamePattern.LowercaseWithSeparator:
                    formattedValue = orginalValue.ToSeparatorCase(nameSeparator, false);
                    break;
                case DatabaseWordAndNamePattern.Reverse:
                    formattedValue = new string(orginalValue.Reverse().ToArray());
                    break;
                case DatabaseWordAndNamePattern.UppercaseReverse:
                    formattedValue = new string(orginalValue.Reverse().ToArray()).ToUpper();
                    break;
                case DatabaseWordAndNamePattern.LowercaseReverse:
                    formattedValue = new string(orginalValue.Reverse().ToArray()).ToLower();
                    break;
                case DatabaseWordAndNamePattern.UppercaseReverseWithSeparator:
                    formattedValue = new string(orginalValue.ToSeparatorCase(nameSeparator, true).Reverse().ToArray());
                    break;
                case DatabaseWordAndNamePattern.LowercaseReverseWithSeparator:
                    formattedValue = new string(orginalValue.ToSeparatorCase(nameSeparator, false).Reverse().ToArray());
                    break;
            }
            return formattedValue;
        }

        #endregion

        #region Database setting

        /// <summary>
        /// Get database setting
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <returns></returns>
        DatabaseSetting GetDatabaseSetting(DatabaseType databaseType)
        {
            if (!_databaseSettings.TryGetValue(databaseType, out var setting) || setting == null)
            {
                lock (_databaseSettings)
                {
                    if (!_databaseSettings.TryGetValue(databaseType, out setting) || setting == null)
                    {
                        setting = new DatabaseSetting();
                        _databaseSettings[databaseType] = setting;
                    }
                }
            }
            return setting;
        }

        #endregion
    }
}
