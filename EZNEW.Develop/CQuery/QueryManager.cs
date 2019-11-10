using EZNEW.Develop.Entity;
using EZNEW.Framework.ExpressionUtil;
using EZNEW.Framework.Extension;
using EZNEW.Framework.Fault;
using System;
using System.Collections.Concurrent;
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
        /// query model entity
        /// key:query model type guid
        /// </summary>
        static ConcurrentDictionary<Guid, Type> queryModelEntityRelations = new ConcurrentDictionary<Guid, Type>();

        #region Propertys

        /// <summary>
        /// global query filter
        /// </summary>
        public static Func<GlobalConditionFilter, GlobalConditionFilterResult> GetGlobalCondition { get; set; }

        #endregion

        #region Methods

        #region set query model relation entity

        /// <summary>
        /// set query model relation entity type
        /// </summary>
        /// <param name="typeGuid">query model type guid</param>
        /// <param name="entityType">relation type</param>
        public static void SetQueryModelRelatioEntity(Guid typeGuid, Type entityType)
        {
            if (entityType == null)
            {
                return;
            }
            queryModelEntityRelations[typeGuid] = entityType;
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
        /// config query model relation entity type
        /// </summary>
        /// <typeparam name="QT">query model type</typeparam>
        internal static void ConfigQueryModelRelationEntity<QT>()
        {
            SetQueryModelRelationEntity(typeof(QT));
        }

        /// <summary>
        /// config query model relation entity type
        /// </summary>
        /// <param name="queryModelType">query model type</param>
        public static void SetQueryModelRelationEntity(Type queryModelType)
        {
            if (queryModelType == null)
            {
                return;
            }
            if (queryModelEntityRelations.ContainsKey(queryModelType.GUID))
            {
                return;
            }
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
            queryModelEntityRelations.TryGetValue(queryModelType.GUID, out Type entityType);
            if (entityType == null)
            {
                SetQueryModelRelationEntity(queryModelType);
                queryModelEntityRelations.TryGetValue(queryModelType.GUID, out entityType);
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

        #region append global condition

        /// <summary>
        /// global condition filter
        /// </summary>
        /// <param name="conditionFilter">condition filter</param>
        /// <returns>filter result</returns>
        public static GlobalConditionFilterResult GlobalConditionFilter(GlobalConditionFilter conditionFilter)
        {
            if (conditionFilter == null)
            {
                throw new EZNEWException("GlobalConditionFilter is null");
            }
            if (conditionFilter.EntityType == null)
            {
                throw new EZNEWException("GlobalConditionFilter.EntityType is null");
            }
            if (conditionFilter.OriginQuery == null)
            {
                conditionFilter.OriginQuery = QueryFactory.Create();
                conditionFilter.OriginQuery.SetEntityType(conditionFilter.EntityType);
            }
            GlobalConditionFilterResult globalConditionResult = null;
            if (GetGlobalCondition != null && conditionFilter.OriginQuery.AllowSetGlobalCondition())
            {
                globalConditionResult = GetGlobalCondition(conditionFilter);
            }
            return globalConditionResult;
        }

        #endregion

        #endregion
    }
}
