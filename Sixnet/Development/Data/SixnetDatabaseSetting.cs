// "Company © 2025. All rights reserved."

using System.Data;

using Sixnet.Development.Data.Database;
using Sixnet.Development.Data.Parameter.Handler;
using Sixnet.Development.Entity;

namespace Sixnet.Development.Data
{
    /// <summary>
    /// Database setting
    /// </summary>
    public class SixnetDatabaseSetting
    {
        #region Fields

        private Dictionary<Type, SixnetEntitySetting> _entities;
        private Dictionary<DbType, ISixnetDataCommandParameterHandler> _parameterHandlers;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the batch setting
        /// </summary>
        public SixnetDatabaseBatchSetting BatchSetting { get; set; }

        /// <summary>
        /// Gets or sets the data isolation level
        /// </summary>
        public SixnetDataIsolationLevel? IsolationLevel { get; set; }

        /// <summary>
        /// Gets or sets the default database word and name pattern
        /// </summary>
        public SixnetDatabaseWordAndNamePattern? DatabaseWordAndNamePattern { get; set; }

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
        public void ConfigureEntity(Type entityType, Action<SixnetEntitySetting> configure)
        {
            if (entityType == null || configure == null)
            {
                return;
            }
            _entities ??= new Dictionary<Type, SixnetEntitySetting>();
            if (!_entities.TryGetValue(entityType, out var entitySetting))
            {
                entitySetting = new SixnetEntitySetting();
                _entities[entityType] = entitySetting;
            }
            configure(entitySetting);

        }

        /// <summary>
        /// Get entity setting
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <returns></returns>
        public SixnetEntitySetting GetEntitySetting(Type entityType)
        {
            if (entityType == null)
            {
                return null;
            }
            SixnetEntitySetting entitySetting = null;
            _entities?.TryGetValue(entityType, out entitySetting);
            return entitySetting;
        }

        #endregion

        #endregion
    }
}
