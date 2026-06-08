// "Company © 2025. All rights reserved."

using System.Data;

using Sixnet.Cache.String.Parameters;
using Sixnet.Development.Data.Command;
using Sixnet.Development.Data.Command.Events;
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

        readonly Dictionary<SixnetDatabaseType, SixnetDatabaseSetting> _databaseSettings = new();
        readonly Dictionary<string, ISixnetFieldFormatter> _fieldFormatters = new();
        readonly List<ISixnetDataCommandStartingEventHandler> _dataCommandStartingEventHandlers = new();
        readonly List<ISixnetDataCommandCallbackEventHandler> _dataCommandCallbackEventHandlers = new();
        readonly Dictionary<Type, ISixnetCondition> _typeFilters = new();
        readonly Dictionary<string, ISixnetSplitTableProvider> _splitTableProviders = new()
        {
            { SixnetDefaultLocalizationSplitTableProvider.Name, new SixnetDefaultLocalizationSplitTableProvider() }
        }; // key: provider name
        readonly Dictionary<SixnetDataIsolationLevel, IsolationLevel> _systemDataIsolationLevels = new()
        {
            { SixnetDataIsolationLevel.Chaos, IsolationLevel.Chaos },
            { SixnetDataIsolationLevel.ReadCommitted, IsolationLevel.ReadCommitted },
            { SixnetDataIsolationLevel.ReadUncommitted, IsolationLevel.ReadUncommitted },
            { SixnetDataIsolationLevel.RepeatableRead, IsolationLevel.RepeatableRead },
            { SixnetDataIsolationLevel.Serializable, IsolationLevel.Serializable },
            { SixnetDataIsolationLevel.Snapshot, IsolationLevel.Snapshot },
            { SixnetDataIsolationLevel.Unspecified, IsolationLevel.Unspecified }
        };
        readonly Dictionary<SixnetDatabaseType, string> _databaseDefaultSchemas = new()
        {
            { SixnetDatabaseType.SQLServer, "dbo" }
        };
        SixnetFieldRole _ignoreFilterFieldRole = SixnetFieldRole.None;
        Func<SixnetDataCommand, List<SixnetDatabaseServer>> _getDataCommandDatabaseServersFunc;
        Func<SixnetDatabaseServer, IDbConnection> _getDatabaseConnectionFunc;
        Func<SixnetQueryableFilterContext, ISixnetQueryable> _getCustomContextFilterFunc;
        Func<SixnetDataCommandExecutionContext, SixnetEntityConfiguration, string> _getDatabaseSchemaFunc;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Database servers
        /// </summary>
        public List<SixnetDatabaseServer> Servers { get; set; }

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
        public SixnetDatabaseServerMatchPattern DatabaseServerMatchPattern { get; set; } = SixnetDatabaseServerMatchPattern.Default;

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
        public SixnetDatabaseWordAndNamePattern DatabaseWordAndNamePattern { get; set; } = SixnetDatabaseWordAndNamePattern.Original;

        /// <summary>
        /// Gets or sets the default database word and name separator
        /// </summary>
        public string DatabaseWordAndNameSeparator { get; set; } = "_";

        /// <summary>
        /// Gets or sets the default default command timeout(in seconds)
        /// </summary>
        public int? DefaultCommandTimeout { get; set; }

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
        public void ConfigureConnection(Func<SixnetDatabaseServer, IDbConnection> configure)
        {
            _getDatabaseConnectionFunc = configure;
        }

        /// <summary>
        /// Get connection
        /// </summary>
        /// <param name="server">Database server</param>
        /// <returns></returns>
        internal IDbConnection GetConnection(SixnetDatabaseServer server)
        {
            return _getDatabaseConnectionFunc?.Invoke(server);
        }

        #endregion

        #region Servers

        /// <summary>
        /// Configure data command servers
        /// </summary>
        /// <param name="configure">Configure</param>
        public void ConfigureDataCommandServers(Func<SixnetDataCommand, List<SixnetDatabaseServer>> configure)
        {
            _getDataCommandDatabaseServersFunc = configure;
        }

        /// <summary>
        /// Get data command database servers
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns></returns>
        internal List<SixnetDatabaseServer> GetDataCommandDatabaseServers(SixnetDataCommand command)
        {
            List<SixnetDatabaseServer> servers = null;
            if (_getDataCommandDatabaseServersFunc == null)
            {
                switch (DatabaseServerMatchPattern)
                {
                    case SixnetDatabaseServerMatchPattern.Default:
                        servers = GetDefaultDatabaseServers();
                        break;
                    case SixnetDatabaseServerMatchPattern.All:
                        servers = GetAllDatabaseServers();
                        break;
                }
            }
            else
            {
                servers = _getDataCommandDatabaseServersFunc.Invoke(command);
            }
            return servers ?? new List<SixnetDatabaseServer>(0);
        }

        /// <summary>
        /// Get default database servers
        /// </summary>
        /// <returns></returns>
        internal List<SixnetDatabaseServer> GetDefaultDatabaseServers()
        {
            var allServers = GetAllDatabaseServers();
            if (allServers.IsNullOrEmpty())
            {
                return new List<SixnetDatabaseServer>(0);
            }
            var defaultServers = allServers.Where(c => c != null && c.Role == SixnetDatabaseServerRole.Default);
            if (defaultServers.IsNullOrEmpty() && allServers.Count == 1)
            {
                return new List<SixnetDatabaseServer>(1) { allServers.First() };
            }
            return defaultServers?.ToList() ?? new List<SixnetDatabaseServer>(0);
        }

        /// <summary>
        /// Get all config database servers
        /// </summary>
        /// <returns></returns>
        internal List<SixnetDatabaseServer> GetAllDatabaseServers()
        {
            return Servers ?? new List<SixnetDatabaseServer>(0);
        }

        /// <summary>
        /// Get config database server by names
        /// </summary>
        /// <param name="serverName">Database server name</param>
        /// <returns></returns>
        internal List<SixnetDatabaseServer> GetDatabaseServers(string serverName)
        {
            return GetDatabaseServers(new string[1] { serverName });
        }

        /// <summary>
        /// Get config database server by names
        /// </summary>
        /// <param name="serverNames">Database server names</param>
        /// <returns></returns>
        internal List<SixnetDatabaseServer> GetDatabaseServers(IEnumerable<string> serverNames)
        {
            if (serverNames.IsNullOrEmpty())
            {
                return new List<SixnetDatabaseServer>(0);
            }
            var allServers = GetAllDatabaseServers();
            return allServers?.Where(s => serverNames.Contains(s.Name)).ToList()
                ?? new List<SixnetDatabaseServer>(0);
        }

        #endregion

        #region Database provider

        /// <summary>
        /// Add database provider
        /// </summary>
        /// <param name="databaseType"></param>
        /// <param name="databaseProvider"></param>
        public void AddDatabaseProvider(SixnetDatabaseType databaseType, ISixnetDatabaseProvider databaseProvider)
        {
            SixnetDirectThrower.ThrowArgNullIf(databaseProvider == null, nameof(databaseProvider));

            GetDatabaseSetting(databaseType).DatabaseProvider = databaseProvider;
        }

        /// <summary>
        /// Get database provider
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <returns></returns>
        internal ISixnetDatabaseProvider GetDatabaseProvider(SixnetDatabaseType databaseType)
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
        public void AddFieldFormatter(string formatterName, Func<SixnetFormatFieldContext, string> formatter)
        {
            if (!string.IsNullOrWhiteSpace(formatterName) && formatter != null)
            {
                AddFieldFormatter(formatterName, new SixnetDefaultFieldFormatter(formatter));
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
            _dataCommandStartingEventHandlers.Add(new SixnetInterceptParameterDataCommandStartingHandler());
            _dataCommandStartingEventHandlers.Add(new SixnetHandleQueryableDataCommandStartingEventHandler());
        }

        #endregion

        #region Data filter

        /// <summary>
        /// Configure custom filter
        /// </summary>
        /// <param name="configure">Configure</param>
        public void ConfigureCustomFilter(Func<SixnetQueryableFilterContext, ISixnetQueryable> configure)
        {
            _getCustomContextFilterFunc = configure;
        }

        /// <summary>
        /// Ignore role filter
        /// </summary>
        /// <param name="fieldRoles">Field roles</param>
        public void IgnoreRoleFilter(params SixnetFieldRole[] fieldRoles)
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
        public bool HasIgnoredRoleFilter(SixnetFieldRole fieldRole)
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
            var condition = SixnetExpressionHelper.GetQueryable(filter, SixnetCriterionConnector.And);
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
        internal Func<SixnetQueryableFilterContext, ISixnetQueryable> GetCustomFilter()
        {
            return _getCustomContextFilterFunc;
        }

        #endregion

        #region Parameter handler

        /// <summary>
        /// Add parameter handler
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <param name="dbType">Database data type</param>
        /// <param name="parameterHandler">Parameter handler</param>
        public void AddParameterHandler(SixnetDatabaseType databaseType, DbType dbType, ISixnetDataCommandParameterHandler handler)
        {
            SixnetDirectThrower.ThrowArgNullIf(handler == null, nameof(handler));
            GetDatabaseSetting(databaseType).AddParameterHandler(dbType, handler);
        }

        /// <summary>
        /// Remove parameter handler
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <param name="dbType">Database data type</param>
        public void RemoveParameterHandler(SixnetDatabaseType databaseType, DbType dbType)
        {
            GetDatabaseSetting(databaseType).RemoveParameterHandler(dbType);
        }

        /// <summary>
        /// Gets parameter handler
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <param name="dbType">dbType</param>
        /// <returns></returns>
        internal ISixnetDataCommandParameterHandler GetParameterHandler(SixnetDatabaseType databaseType, DbType dbType)
        {
            return GetDatabaseSetting(databaseType).GetParameterHandler(dbType);
        }

        /// <summary>
        /// Configure default parameter handler
        /// </summary>
        void AddDefaultParameterHandler()
        {
            var datetimeOffsetHandler = new SixnetDateTimeOffsetParameterHandler();
            var boolToIntegerHandler = new SixnetBooleanToIntegerParameterHandler();
            var guidHandler = new SixnetGuidFormattingParameterHandler();
            var sbyteHandler = new SixnetSByteToShortParameterHandler();
            var uintHandler = new SixnetUIntToLongParameterHandler();
            var ushortHandler = new SixnetUShortToIntParameterHandler();
            var ulongHandler = new SixnetULongToDecimalParameterHandler();
            var timespanHandler = new SixnetTimeSpanParameterHandler();
            var charHandler = new SixnetCharToStringParameterHandler();
            var ulongToStringHandler = new SixnetULongToStringParameterHandler();
            var nullCharHandler = new SixnetNullCharacterParameterHandler();

            #region MySQL

            //DateTimeOffset
            AddParameterHandler(SixnetDatabaseType.MySQL, DbType.DateTimeOffset, datetimeOffsetHandler);

            #endregion

            #region Oracle

            //boolean
            AddParameterHandler(SixnetDatabaseType.Oracle, DbType.Boolean, boolToIntegerHandler);
            //Guid
            AddParameterHandler(SixnetDatabaseType.Oracle, DbType.Guid, guidHandler);
            //SByte
            AddParameterHandler(SixnetDatabaseType.Oracle, DbType.SByte, sbyteHandler);
            //UInt
            AddParameterHandler(SixnetDatabaseType.Oracle, DbType.UInt32, uintHandler);
            //UShort
            AddParameterHandler(SixnetDatabaseType.Oracle, DbType.UInt16, ushortHandler);
            //ULong
            AddParameterHandler(SixnetDatabaseType.Oracle, DbType.UInt64, ulongHandler);
            //Timespan
            AddParameterHandler(SixnetDatabaseType.Oracle, DbType.Time, timespanHandler);
            //Char
            AddParameterHandler(SixnetDatabaseType.Oracle, DbType.StringFixedLength, charHandler);

            #endregion

            #region SQLite

            //DateTimeOffset
            AddParameterHandler(SixnetDatabaseType.SQLite, DbType.DateTimeOffset, datetimeOffsetHandler);
            //ULong
            AddParameterHandler(SixnetDatabaseType.SQLite, DbType.UInt64, ulongToStringHandler);

            #endregion

            #region SQL Server

            //SByte
            AddParameterHandler(SixnetDatabaseType.SQLServer, DbType.SByte, sbyteHandler);
            //UInt
            AddParameterHandler(SixnetDatabaseType.SQLServer, DbType.UInt32, uintHandler);
            //UShort
            AddParameterHandler(SixnetDatabaseType.SQLServer, DbType.UInt16, ushortHandler);
            //ULong
            AddParameterHandler(SixnetDatabaseType.SQLServer, DbType.UInt64, ulongHandler);

            #endregion

            #region PostgreSQL

            //UInt
            AddParameterHandler(SixnetDatabaseType.PostgreSQL, DbType.UInt32, uintHandler);
            //ULong
            AddParameterHandler(SixnetDatabaseType.PostgreSQL, DbType.UInt64, ulongHandler);
            //UShort
            AddParameterHandler(SixnetDatabaseType.PostgreSQL, DbType.UInt16, ushortHandler);
            //null char
            AddParameterHandler(SixnetDatabaseType.PostgreSQL, DbType.AnsiString, nullCharHandler);
            AddParameterHandler(SixnetDatabaseType.PostgreSQL, DbType.AnsiStringFixedLength, nullCharHandler);
            AddParameterHandler(SixnetDatabaseType.PostgreSQL, DbType.String, nullCharHandler);
            AddParameterHandler(SixnetDatabaseType.PostgreSQL, DbType.StringFixedLength, nullCharHandler);

            #endregion
        }

        #endregion

        #region Batch setting

        /// <summary>
        /// Set batch setting
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <param name="setting">Batch setting</param>
        public void SetBatchSetting(SixnetDatabaseType databaseType, SixnetDatabaseBatchSetting setting)
        {
            GetDatabaseSetting(databaseType).BatchSetting = setting;
        }

        /// <summary>
        /// Get batch setting
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <returns></returns>
        internal SixnetDatabaseBatchSetting GetBatchSetting(SixnetDatabaseType databaseType)
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
        public void SetDefaultIsolationLevel(SixnetDatabaseType databaseType, SixnetDataIsolationLevel dataIsolationLevel)
        {
            GetDatabaseSetting(databaseType).IsolationLevel = dataIsolationLevel;
        }

        /// <summary>
        /// Get database default isolation level
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <returns></returns>
        internal SixnetDataIsolationLevel? GetDefaultIsolationLevel(SixnetDatabaseType databaseType)
        {
            return GetDatabaseSetting(databaseType).IsolationLevel;
        }

        /// <summary>
        /// Get system isolation level by data isolation level
        /// </summary>
        /// <param name="dataIsolationLevel">Data isolation level</param>
        /// <returns></returns>
        internal IsolationLevel? GetSystemIsolationLevel(SixnetDataIsolationLevel? dataIsolationLevel)
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
        public void ConfigureEntity(SixnetDatabaseType databaseType, Type entityType, Action<SixnetEntitySetting> configure)
        {
            GetDatabaseSetting(databaseType).ConfigureEntity(entityType, configure);
        }

        /// <summary>
        /// Get entity setting
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <param name="entityType">Entity type</param>
        /// <returns></returns>
        internal SixnetEntitySetting GetEntitySetting(SixnetDatabaseType databaseType, Type entityType)
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
        internal bool AllowInsertIncrementField(SixnetDataCommandExecutionContext commandExecutionContext)
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
        /// <param name="objectName">Object name</param>
        /// <returns></returns>
        public SixnetDatabaseObjectName FormatDatabaseObjectName(SixnetDatabaseType databaseType, SixnetDatabaseObjectName objectName)
        {
            var orginalValue = objectName.Name;
            if (string.IsNullOrWhiteSpace(orginalValue))
            {
                return objectName;
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
                case SixnetDatabaseWordAndNamePattern.Uppercase:
                    formattedValue = orginalValue.ToUpper();
                    break;
                case SixnetDatabaseWordAndNamePattern.Lowercase:
                    formattedValue = orginalValue.ToLower();
                    break;
                case SixnetDatabaseWordAndNamePattern.UppercaseWithSeparator:
                    formattedValue = orginalValue.ToSeparatorCase(nameSeparator, true);
                    break;
                case SixnetDatabaseWordAndNamePattern.LowercaseWithSeparator:
                    formattedValue = orginalValue.ToSeparatorCase(nameSeparator, false);
                    break;
                case SixnetDatabaseWordAndNamePattern.Reverse:
                    formattedValue = new string(orginalValue.Reverse().ToArray());
                    break;
                case SixnetDatabaseWordAndNamePattern.UppercaseReverse:
                    formattedValue = new string(orginalValue.Reverse().ToArray()).ToUpper();
                    break;
                case SixnetDatabaseWordAndNamePattern.LowercaseReverse:
                    formattedValue = new string(orginalValue.Reverse().ToArray()).ToLower();
                    break;
                case SixnetDatabaseWordAndNamePattern.UppercaseReverseWithSeparator:
                    formattedValue = new string(orginalValue.ToSeparatorCase(nameSeparator, true).Reverse().ToArray());
                    break;
                case SixnetDatabaseWordAndNamePattern.LowercaseReverseWithSeparator:
                    formattedValue = new string(orginalValue.ToSeparatorCase(nameSeparator, false).Reverse().ToArray());
                    break;
            }
            var newObjectName = objectName.Clone();
            newObjectName.Name = formattedValue;
            return newObjectName;
        }

        #endregion

        #region Database setting

        /// <summary>
        /// Get database setting
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <returns></returns>
        SixnetDatabaseSetting GetDatabaseSetting(SixnetDatabaseType databaseType)
        {
            if (!_databaseSettings.TryGetValue(databaseType, out var setting) || setting == null)
            {
                lock (_databaseSettings)
                {
                    if (!_databaseSettings.TryGetValue(databaseType, out setting) || setting == null)
                    {
                        setting = new SixnetDatabaseSetting();
                        _databaseSettings[databaseType] = setting;
                    }
                }
            }
            return setting;
        }

        #endregion

        #region Schemas

        /// <summary>
        /// Set default schema
        /// </summary>
        /// <param name="databaseType"></param>
        /// <param name="schema"></param>
        public void SetDatabaseDefaultSchema(SixnetDatabaseType databaseType, string schema)
        {
            _databaseDefaultSchemas[databaseType] = schema;
        }

        /// <summary>
        /// Get default schema
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        string GetDatabaseDefaultSchema(SixnetDataCommandExecutionContext context)
        {
            if (context?.DatabaseConnection == null)
            {
                return string.Empty;
            }
            _databaseDefaultSchemas.TryGetValue(context.DatabaseConnection.DatabaseServer.DatabaseType, out var schema);
            if (string.IsNullOrWhiteSpace(schema))
            {
                schema = context.DatabaseConnection.DatabaseServer.DatabaseType switch
                {
                    SixnetDatabaseType.SQLServer => "dbo",
                    SixnetDatabaseType.PostgreSQL => "public",
                    SixnetDatabaseType.Oracle => context.DatabaseConnection.Meta.UserName,
                    SixnetDatabaseType.DaMeng => context.DatabaseConnection.Meta.UserName,
                    SixnetDatabaseType.Kingbase => "public",
                    _ => string.Empty,
                };
            }
            return schema;
        }

        /// <summary>
        /// Configure database schema
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public void ConfigureDatabaseSchema(Func<SixnetDataCommandExecutionContext, SixnetEntityConfiguration, string> configure)
        {
            _getDatabaseSchemaFunc = configure;
        }

        /// <summary>
        /// Get database schema
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string GetDatabaseSchema(SixnetDataCommandExecutionContext context, SixnetEntityConfiguration entityConfiguration)
        {
            var schema = _getDatabaseSchemaFunc?.Invoke(context, entityConfiguration);
            if (string.IsNullOrWhiteSpace(schema))
            {
                schema = GetDatabaseDefaultSchema(context);
            }
            return schema;
        }

        #endregion
    }
}
