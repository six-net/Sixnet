using System;
using System.Collections.Generic;
using System.Text;
using EZNEW.Develop.CQuery;

namespace EZNEW.Configuration
{
    public partial class ConfigurationManager
    {
        internal static class QueryModel
        {
            static QueryModel()
            {
                InternalConfigurationManager.Confirure();
            }

            /// <summary>
            /// Value:Entity type
            /// Key:Query model type guid
            /// </summary>
            static readonly Dictionary<Guid, Type> QueryModelRelationEntities = new Dictionary<Guid, Type>();

            #region Query model relation entity

            /// <summary>
            /// Configure query model relation entity type
            /// </summary>
            /// <param name="queryModelTypeGuid">Query model type guid</param>
            /// <param name="entityType">Relation entity type</param>
            internal static void ConfigureQueryModelRelationEntity(Guid queryModelTypeGuid, Type entityType)
            {
                if (entityType == null)
                {
                    return;
                }
                QueryModelRelationEntities[queryModelTypeGuid] = entityType;
            }

            /// <summary>
            /// Configure query model relation entity type
            /// </summary>
            /// <param name="queryModelType">Query model type</param>
            /// <param name="entityType">Relation entity type</param>
            internal static void ConfigureQueryModelRelationEntity(Type queryModelType, Type entityType)
            {
                if (queryModelType == null || entityType == null)
                {
                    return;
                }
                ConfigureQueryModelRelationEntity(queryModelType.GUID, entityType);
            }

            /// <summary>
            /// Configure query model relation entity type
            /// </summary>
            /// <param name="queryModelType">Query model type</param>
            internal static void ConfigureQueryModelRelationEntity(Type queryModelType)
            {
                if (queryModelType == null)
                {
                    return;
                }
                if (QueryModelRelationEntities.ContainsKey(queryModelType.GUID))
                {
                    return;
                }
                var attributes = queryModelType.GetCustomAttributes(typeof(QueryEntityAttribute), true);
                if (attributes.IsNullOrEmpty())
                {
                    return;
                }
                QueryEntityAttribute configAttribute = attributes[0] as QueryEntityAttribute;
                if (configAttribute == null)
                {
                    return;
                }
                var relevanceType = configAttribute.RelevanceType;
                ConfigureQueryModelRelationEntity(queryModelType, relevanceType);
            }

            /// <summary>
            /// Get query model relation entity type
            /// </summary>
            /// <param name="queryModelType">Query model type</param>
            /// <returns>Return entity type</returns>
            internal static Type GetQueryModelRelationEntityType(Type queryModelType)
            {
                if (queryModelType == null)
                {
                    return null;
                }
                if (Entity.EntityConfigurations.ContainsKey(queryModelType.GUID))
                {
                    return queryModelType;
                }
                QueryModelRelationEntities.TryGetValue(queryModelType.GUID, out Type entityType);
                return entityType;
            }

            #endregion
        }
    }
}
