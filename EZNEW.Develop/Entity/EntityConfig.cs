using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.Entity
{
    /// <summary>
    /// entity config
    /// </summary>
    public class EntityConfig
    {
        #region Propertys

        /// <summary>
        /// table name
        /// </summary>
        public string TableName
        {
            get; set;
        }

        /// <summary>
        /// all fields
        /// </summary>
        public List<EntityField> AllFields
        {
            get; set;
        }

        /// <summary>
        /// query fields
        /// </summary>
        public List<EntityField> QueryFields
        {
            get; set;
        }

        /// <summary>
        /// primary keys
        /// </summary>
        public List<EntityField> PrimaryKeys
        {
            get; set;
        }

        /// <summary>
        /// cache keys
        /// </summary>
        public List<EntityField> CacheKeys
        {
            get; set;
        }

        /// <summary>
        /// cache prefix keys
        /// </summary>
        public List<EntityField> CachePrefixKeys
        {
            get;set;
        }

        /// <summary>
        /// cache ignore keys
        /// </summary>
        public List<EntityField> CacheIgnoreKeys
        {
            get;set;
        }

        /// <summary>
        /// version key
        /// </summary>
        public EntityField VersionField
        {
            get; set;
        }

        /// <summary>
        /// refresh date key
        /// </summary>
        public EntityField RefreshDateField
        {
            get; set;
        }

        /// <summary>
        /// relation fields
        /// key:relation type id
        /// value: key->field,value:relation field
        /// </summary>
        public Dictionary<Guid, Dictionary<string, string>> RelationFields
        {
            get; set;
        }

        #endregion
    }
}
