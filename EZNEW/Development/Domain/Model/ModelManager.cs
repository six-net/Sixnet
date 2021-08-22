using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace EZNEW.Development.Domain.Model
{
    /// <summary>
    /// Aggregation manager
    /// </summary>
    public static class ModelManager
    {
        /// <summary>
        /// Virtual models
        /// </summary>
        static readonly Dictionary<Guid, bool> VirtualModels = new Dictionary<Guid, bool>();

        /// <summary>
        /// Configure model
        /// </summary>
        /// <typeparam name="TModel">Model</typeparam>
        internal static void ConfigureModel<TModel>() where TModel : IModel<TModel>
        {
            var type = typeof(TModel);
            ConfigureModel(type);
        }

        /// <summary>
        /// Configure model
        /// </summary>
        /// <param name="modelType">Model type</param>
        internal static void ConfigureModel(Type modelType, IEnumerable<Type> allTypes = null)
        {
            if (modelType == null)
            {
                return;
            }
            var modelAttributes = modelType.GetCustomAttributes(typeof(ModelMetadataAttribute), true);
            if (modelAttributes?.Any(c => (((ModelMetadataAttribute)c).Feature & AggregationFeature.Virtual) == AggregationFeature.Virtual) ?? false)
            {
                VirtualModels[modelType.GUID] = true;
            }
            allTypes ??= modelType.Assembly.GetTypes();
            var subTypes = allTypes.Where(t => t.BaseType == modelType);
            if (subTypes.IsNullOrEmpty())
            {
                return;
            }
            foreach (var subType in subTypes)
            {
                ConfigureModel(subType, allTypes);
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
            VirtualModels.TryGetValue(modelType.GUID, out var virtualModel);
            return virtualModel;
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
            return modelType.GetCustomAttribute<ModelMetadataAttribute>()?.EntityType;
        }
    }
}
