using System;
using System.Collections.Generic;
using System.Data;
using Sixnet.Development.Data.Database;
using Sixnet.Development.Data.Parameter.Handler;
using Sixnet.Development.Entity;

namespace Sixnet.Development.Data
{
    /// <summary>
    /// Database setting
    /// </summary>
    public class DatabaseSetting
    {
        #region Fields

        private Dictionary<Type, EntitySetting> _entities;
        private Dictionary<DbType, ISixnetDataCommandParameterHandler> _parameterHandlers;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the batch setting
        /// </summary>
        public DatabaseBatchSetting BatchSetting { get; set; }

        /// <summary>
        /// Gets or sets the data isolation level
        /// </summary>
        public DataIsolationLevel? IsolationLevel { get; set; }

        /// <summary>
        /// Gets or sets the default database word and name pattern
        /// </summary>
        public DatabaseWordAndNamePattern? DatabaseWordAndNamePattern { get; set; }

        /// <summary>
        /// Gets or sets the default database word and name separator
        /// </summary>
        public string DatabaseWordAndNameSeparator { get; set; }

        /// <summary>
        /// Gets or sets the database provider
        /// </summary>
        public ISixnetDatabaseProvider DatabaseProvider { get; set; }

        /// <summary>
        /// Whether insert increment field.
        /// </summary>
        public bool? InsertIncrementField { get; set; }

        #endregion

        #region Methods

        #region Parameter handler

        /// <summary>
        /// Add parameter handler
        /// </summary>
        /// <param name="dbType">Database data type</param>
        /// <param name="handler">Parameter handler</param>
        public void AddParameterHandler(DbType dbType, ISixnetDataCommandParameterHandler handler)
        {
            _parameterHandlers ??= new Dictionary<DbType, ISixnetDataCommandParameterHandler>();
            _parameterHandlers[dbType] = handler;
        }

        /// <summary>
        /// Remove parameter handler
        /// </summary>
        /// <param name="dbType">Data type</param>
        public void RemoveParameterHandler(DbType dbType)
        {
            _parameterHandlers?.Remove(dbType);
        }

        /// <summary>
        /// Get parameter handlers
        /// </summary>
        /// <param name="dbType">Data type</param>
        /// <returns></returns>
        public ISixnetDataCommandParameterHandler GetParameterHandler(DbType dbType)
        {
            ISixnetDataCommandParameterHandler handler = null;
            _parameterHandlers?.TryGetValue(dbType, out handler);
            return handler;
        }

        #endregion

        #region Entity setting

        /// <summary>
        /// Configure entity
        /// </summary>
        /// <param name="entityType">Entity type</param>
        public void ConfigureEntity(Type entityType, Action<EntitySetting> configure)
        {
            if (entityType == null || configure == null)
            {
                return;
            }
            _entities ??= new Dictionary<Type, EntitySetting>();
            if (!_entities.TryGetValue(entityType, out var entitySetting))
            {
                entitySetting = new EntitySetting();
                _entities[entityType] = entitySetting;
            }
            configure(entitySetting);

        }

        /// <summary>
        /// Get entity setting
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <returns></returns>
        public EntitySetting GetEntitySetting(Type entityType)
        {
            if (entityType == null)
            {
                return null;
            }
            EntitySetting entitySetting = null;
            _entities?.TryGetValue(entityType, out entitySetting);
            return entitySetting;
        }

        #endregion

        #endregion
    }
}
