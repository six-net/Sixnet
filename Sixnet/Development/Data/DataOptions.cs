using AutoMapper;
using Microsoft.Extensions.Options;
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
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace Sixnet.Development.Data
{
    /// <summary>
    /// Database options
    /// </summary>
    public class DataOptions
    {
        #region Fields

        readonly Dictionary<DatabaseType, ISixnetDatabaseProvider> _databaseProviders = new();
        readonly Dictionary<string, ISixnetFieldFormatter> _fieldFormatters = new();
        readonly List<ISixnetDataCommandStartingEventHandler> _dataCommandStartingEventHandlers = new();
        readonly List<ISixnetDataCommandCallbackEventHandler> _dataCommandCallbackEventHandlers = new();
        readonly Dictionary<Type, ISixnetCondition> _typeFilters = new();
        FieldRole _ignoreFilterFieldRole = FieldRole.None;
        readonly Dictionary<string, ISixnetDataCommandParameterHandler> _parameterHandlers = new(); // key: "{DatabaseType}_{DbType}"
        readonly Dictionary<string, ISixnetSplitTableProvider> _splitTableProviders = new(); // key: provider name
        readonly Dictionary<DatabaseType, DatabaseBatchSetting> _batchSettings = new();
        readonly Dictionary<DatabaseType, DataIsolationLevel> _databaseDataIsolationLevels = new()
        {
            { DatabaseType.MySQL, DataIsolationLevel.RepeatableRead },
            { DatabaseType.SQLServer, DataIsolationLevel.ReadCommitted },
            { DatabaseType.Oracle, DataIsolationLevel.ReadCommitted }
        };
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

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the database options
        /// </summary>
        public Dictionary<DatabaseType, DatabaseSetting> Databases { get; set; }

        /// <summary>
        /// Gets or sets the Database servers
        /// </summary>
        public List<DatabaseServer> Servers { get; set; }

        /// <summary>
        /// Gets or sets the get data command database server
        /// </summary>
        public Func<SixnetDataCommand, List<DatabaseServer>> GetDataCommandDatabaseServers { get; set; }

        /// <summary>
        /// Gets or sets the get database connection
        /// </summary>
        public Func<DatabaseServer, IDbConnection> GetDatabaseConnection { get; set; } = null;

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

        #endregion

        #region Constructor

        public DataOptions()
        {
            AddDefaultParameterHandler();
            SubscribeDefaultCommandStartingEvent();
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
            _databaseProviders[databaseType] = databaseProvider;
        }

        /// <summary>
        /// Get database provider
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <returns></returns>
        public ISixnetDatabaseProvider GetDatabaseProvider(DatabaseType databaseType)
        {
            _databaseProviders.TryGetValue(databaseType, out var databaseProvider);
            return databaseProvider;
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
        public ISixnetFieldFormatter GetFieldFormatter(string formatterName)
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
        /// Get command starting event handlers
        /// </summary>
        internal List<ISixnetDataCommandStartingEventHandler> GetCommandStartingEventHandlers()
        {
            return _dataCommandStartingEventHandlers;
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
        public List<ISixnetDataCommandCallbackEventHandler> GetCommandCallbackEventHandlers()
        {
            return _dataCommandCallbackEventHandlers;
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
        /// Gets custom data filter func
        /// </summary>
        public Func<QueryableFilterContext, ISixnetQueryable> GetCustomDataFilter { get; set; }

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
        /// Get type filters
        /// </summary>
        /// <returns></returns>
        internal Dictionary<Type, ISixnetCondition> GetTypeFilters()
        {
            return _typeFilters;
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
            var databaseTypeKey = GetParameterHandlerKey(databaseType, dbType);
            _parameterHandlers[databaseTypeKey] = handler;
        }

        /// <summary>
        /// Remove parameter handler
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <param name="dbType">Database data type</param>
        public void RemoveParameterHandler(DatabaseType databaseType, DbType dbType)
        {
            var databaseTypeKey = GetParameterHandlerKey(databaseType, dbType);
            if (_parameterHandlers.ContainsKey(databaseTypeKey))
            {
                _parameterHandlers.Remove(databaseTypeKey);
            }
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

        /// <summary>
        /// Get database type parameter handler format key
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <param name="dbType">Database data type</param>
        /// <returns></returns>
        string GetParameterHandlerKey(DatabaseType databaseType, DbType dbType)
        {
            return string.Format("{0}_{1}", (int)databaseType, (int)dbType);
        }

        /// <summary>
        /// Gets parameter handler
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <param name="dbType">dbType</param>
        /// <returns></returns>
        public ISixnetDataCommandParameterHandler GetParameterHandler(DatabaseType databaseType, DbType dbType)
        {
            var key = GetParameterHandlerKey(databaseType, dbType);
            _parameterHandlers.TryGetValue(key, out var handler);
            return handler;
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
            if (setting != null)
            {
                _batchSettings[databaseType] = setting;
            }
        }

        /// <summary>
        /// Get batch setting
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <returns></returns>
        public DatabaseBatchSetting GetBatchSetting(DatabaseType databaseType)
        {
            _batchSettings.TryGetValue(databaseType, out var config);
            return config;
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
            _databaseDataIsolationLevels[databaseType] = dataIsolationLevel;
        }

        /// <summary>
        /// Get database default isolation level
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <returns></returns>
        public DataIsolationLevel? GetDefaultIsolationLevel(DatabaseType databaseType)
        {
            if (_databaseDataIsolationLevels.ContainsKey(databaseType))
            {
                return _databaseDataIsolationLevels[databaseType];
            }
            return null;
        }

        /// <summary>
        /// Get system isolation level by data isolation level
        /// </summary>
        /// <param name="dataIsolationLevel">Data isolation level</param>
        /// <returns></returns>
        public IsolationLevel? GetSystemIsolationLevel(DataIsolationLevel? dataIsolationLevel)
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
        /// Get entity setting
        /// </summary>
        /// <param name="databaseType">Database type</param>
        /// <param name="entityType">Entity type</param>
        /// <returns></returns>
        public EntitySetting GetEntitySetting(DatabaseType databaseType, Type entityType)
        {
            if (entityType != null)
            {
                return null;
            }
            if (Databases?.ContainsKey(databaseType) ?? false)
            {
                var databaseSetting = Databases[databaseType];
                if (databaseSetting?.Entities?.ContainsKey(entityType) ?? false)
                {
                    return databaseSetting.Entities[entityType];
                }
            }
            return null;
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
        public ISixnetSplitTableProvider GetSplitTableProvider(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }
            _splitTableProviders.TryGetValue(name, out var provider);
            return provider;
        }

        #endregion
    }
}
