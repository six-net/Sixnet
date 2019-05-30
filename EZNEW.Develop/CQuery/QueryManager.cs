using EZNEW.Develop.Entity;
using EZNEW.Framework.ExpressionUtil;
using EZNEW.Framework.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.CQuery
{
    /// <summary>
    /// Query Config
    /// </summary>
    public static class QueryManager
    {
        /// <summary>
        /// query model entity s
        /// key:query model type guid
        /// </summary>
        static Dictionary<Guid, Type> queryModelEntityRelations = new Dictionary<Guid, Type>();

        /// <summary>
        /// alread config query model guids
        /// </summary>
        static HashSet<Guid> alreadConfigQueryEntitys = new HashSet<Guid>();

        #region set query model relation entity

        /// <summary>
        /// set query model relation entity type
        /// </summary>
        /// <param name="typeGuid">query model </param>
        /// <param name="entityType">relation type</param>
        public static void SetQueryModelRelatioEntity(Guid typeGuid, Type entityType)
        {
            if (entityType == null)
            {
                return;
            }
            if (queryModelEntityRelations.ContainsKey(typeGuid))
            {
                queryModelEntityRelations[typeGuid] = entityType;
            }
            else
            {
                queryModelEntityRelations.Add(typeGuid, entityType);
            }
        }

        /// <summary>
        /// set query model relation entity type
        /// </summary>
        /// <param name="queryModelType">query model type</param>
        /// <param name="entityType">relation entity type</param>
        public static void SetQueryModelRelatioEntity(Type queryModelType, Type entityType)
        {
            if (queryModelType == null || entityType == null)
            {
                return;
            }
            SetQueryModelRelatioEntity(queryModelType.GUID, entityType);
        }

        /// <summary>
        /// set query model relation entity type
        /// </summary>
        /// <typeparam name="QT">query model type</typeparam>
        /// <typeparam name="RT">entity type</typeparam>
        public static void SetQueryModelRelatioEntity<QT, RT>() where QT : QueryModel<QT>
        {
            SetQueryModelRelatioEntity(typeof(QT), typeof(RT));
        }

        /// <summary>
        /// set query model relation entity type
        /// </summary>
        /// <typeparam name="QT">query model</typeparam>
        internal static void ConfigQueryModelRelationEntity<QT>()
        {
            ConfigQueryModelRelationEntity(typeof(QT));
        }

        /// <summary>
        /// set query model relation entity type
        /// </summary>
        /// <param name="queryModelType">query model type</param>
        public static void ConfigQueryModelRelationEntity(Type queryModelType)
        {
            if (queryModelType == null)
            {
                return;
            }
            var typeGuid = queryModelType.GUID;
            if (queryModelEntityRelations.ContainsKey(typeGuid) || alreadConfigQueryEntitys.Contains(typeGuid))
            {
                return;
            }
            alreadConfigQueryEntitys.Add(typeGuid);
            var attributes = queryModelType.GetCustomAttributes(typeof(QueryEntityAttribute), true);
            if (attributes.IsNullOrEmpty())
            {
                return;
            }
            var configAttribute = attributes[0] as QueryEntityAttribute;
            if (configAttribute == null)
            {
                return;
            }
            var relevanceType = configAttribute.RelevanceType;
            SetQueryModelRelatioEntity(queryModelType, relevanceType);
            EntityManager.ConfigEntity(relevanceType);
        }

        #endregion

        #region get query model relation entity

        /// <summary>
        /// get query model relation entity type
        /// </summary>
        /// <param name="queryModelAssemblyQualifiedName">query model type assembly qualified name</param>
        /// <returns></returns>
        public static Type GetQueryModelRelationEntityType(string queryModelAssemblyQualifiedName)
        {
            if (queryModelAssemblyQualifiedName.IsNullOrEmpty())
            {
                return null;
            }
            var type = Type.GetType(queryModelAssemblyQualifiedName);
            return GetQueryModelRelationEntityType(type);
        }

        /// <summary>
        /// get query model relation entity type
        /// </summary>
        /// <param name="queryModelType">query model type</param>
        /// <returns></returns>
        public static Type GetQueryModelRelationEntityType(Type queryModelType)
        {
            if (queryModelType == null)
            {
                return null;
            }
            var typeGuid = queryModelType.GUID;
            queryModelEntityRelations.TryGetValue(typeGuid, out Type entityType);
            if (entityType == null && !alreadConfigQueryEntitys.Contains(typeGuid))
            {
                ConfigQueryModelRelationEntity(queryModelType);
                queryModelEntityRelations.TryGetValue(typeGuid, out entityType);
            }
            return entityType;
        }

        /// <summary>
        /// get query model relation entity type
        /// </summary>
        /// <typeparam name="T">query model type</typeparam>
        /// <returns></returns>
        public static Type GetQueryModelRelationEntityType<T>()
        {
            return GetQueryModelRelationEntityType(typeof(T));
        }

        #endregion
    }
}
