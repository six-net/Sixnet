using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EZNEW.Application;
using EZNEW.Development.Entity;
using EZNEW.Development.Query;

namespace EZNEW.Development.Domain.Model
{
    /// <summary>
    /// Aggregation manager
    /// </summary>
    public static class ModelManager
    {
        #region Fields

        /// <summary>
        /// Model configuration
        /// </summary>
        internal static readonly Dictionary<Guid, ModelMetadataAttribute> ModelMetadatas = new Dictionary<Guid, ModelMetadataAttribute>();

        /// <summary>
        /// Entity contract type
        /// </summary>
        static readonly Type entityContractType = typeof(IEntity);

        #endregion

        #region Methods

        /// <summary>
        /// Configure model
        /// </summary>
        /// <typeparam name="TModel">Model</typeparam>
        internal static void ConfigureModel<TModel>() where TModel : IModel<TModel>
        {
            var type = typeof(TModel);
            if (type == null || ModelMetadatas.ContainsKey(type.GUID))
            {
                return;
            }
            ConfigureModel(type, ApplicationManager.GetAllConventionTypes()?.ToDictionary(c => c.FullName, c => c));
        }

        /// <summary>
        /// Configure model
        /// </summary>
        /// <param name="modelType">Model type</param>
        internal static void ConfigureModel(Type modelType, Dictionary<string, Type> allTypeDict)
        {
            if (modelType == null || ModelMetadatas.ContainsKey(modelType.GUID))
            {
                return;
            }
            var modelAttribute = modelType.GetCustomAttribute<ModelMetadataAttribute>(true) ?? new ModelMetadataAttribute();
            var isEntity = entityContractType.IsAssignableFrom(modelType);
            if (isEntity)
            {
                modelAttribute.EntityType = modelType;
            }
            else
            {
                if (modelAttribute.EntityType == null)
                {
                    //Mapping default entity
                    var defaultEntityName = $"{modelType.Name}Entity";
                    var entityKey = allTypeDict.Keys.FirstOrDefault(c => c.EndsWith(defaultEntityName,StringComparison.OrdinalIgnoreCase));
                    if (!string.IsNullOrWhiteSpace(entityKey) && allTypeDict.TryGetValue(entityKey,out var entityType) && entityContractType.IsAssignableFrom(entityType))
                    {
                        modelAttribute.EntityType = entityType;
                    }
                }
                EntityManager.SetRelationModel(modelAttribute.EntityType, modelType);
                QueryManager.ConfigureQueryModelRelationEntity(modelType, modelAttribute.EntityType);
            }
            modelAttribute.ModelType = modelType;
            ModelMetadatas[modelType.GUID] = modelAttribute;

            //Configure sub types
            var subTypes = allTypeDict.Values.Where(t => t.BaseType == modelType);
            if (subTypes.IsNullOrEmpty())
            {
                return;
            }
            foreach (var subType in subTypes)
            {
                ConfigureModel(subType, allTypeDict);
            }
        }

        /// <summary>
        /// Determine whether is virtual model
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <returns>Return model whether is virtual model</returns>
        internal static bool IsVirtualModel<TModel>()
        {
            var type = typeof(TModel);
            return IsVirtualModel(type);
        }

        /// <summary>
        /// Determine whether is virtual model
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <returns>Return model whether is virtual model</returns>
        internal static bool IsVirtualModel(Type modelType)
        {
            if (modelType == null)
            {
                return false;
            }
            ModelMetadatas.TryGetValue(modelType.GUID, out var modelMetadata);
            return modelMetadata != null && (modelMetadata.Feature & ModelFeature.Virtual) == ModelFeature.Virtual;
        }

        /// <summary>
        /// Get model relation entity
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <returns></returns>
        internal static Type GetModelRelationEntityType(Type modelType)
        {
            if (modelType == null)
            {
                return null;
            }
            ModelMetadatas.TryGetValue(modelType.GUID, out var modelMetadata);
            return modelMetadata?.EntityType;
        }

        /// <summary>
        /// Set model relation entity type
        /// </summary>
        /// <param name="entityType"></param>
        internal static void SetModelRelationEntityType(Type modelType, Type entityType)
        {
            ModelMetadatas.TryGetValue(modelType.GUID, out var modelMetadata);
            if (modelMetadata != null)
            {
                modelMetadata.EntityType = entityType;
            }
        }

        #endregion
    }
}
