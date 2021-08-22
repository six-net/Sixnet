using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace EZNEW.Development.Domain.Aggregation
{
    /// <summary>
    /// Aggregation manager
    /// </summary>
    public static class AggregationManager
    {
        /// <summary>
        /// Virtual aggregations
        /// </summary>
        static readonly Dictionary<Guid, bool> VirtualAggregations = new Dictionary<Guid, bool>();

        /// <summary>
        /// Configure aggregation model
        /// </summary>
        /// <typeparam name="TAggregationModel">Aggregation model</typeparam>
        internal static void ConfigureAggregationModel<TAggregationModel>() where TAggregationModel : IAggregationRoot<TAggregationModel>
        {
            var type = typeof(TAggregationModel);
            ConfigureAggregationModel(type);
        }

        /// <summary>
        /// Configure aggregation model
        /// </summary>
        /// <param name="modelType">Aggregation model type</param>
        internal static void ConfigureAggregationModel(Type modelType, IEnumerable<Type> allTypes = null)
        {
            if (modelType == null)
            {
                return;
            }
            var modelAttributes = modelType.GetCustomAttributes(typeof(AggregationModelAttribute), true);
            if (modelAttributes?.Any(c => (((AggregationModelAttribute)c).Feature & AggregationFeature.Virtual) == AggregationFeature.Virtual) ?? false)
            {
                VirtualAggregations[modelType.GUID] = true;
            }
            allTypes ??= modelType.Assembly.GetTypes();
            var subTypes = allTypes.Where(t => t.BaseType == modelType);
            if (subTypes.IsNullOrEmpty())
            {
                return;
            }
            foreach (var subType in subTypes)
            {
                ConfigureAggregationModel(subType, allTypes);
            }
        }

        /// <summary>
        /// Determine whether is virtual model
        /// </summary>
        /// <typeparam name="TAggregationModel">Aggregation model type</typeparam>
        /// <returns>Return model whether is virtual model</returns>
        internal static bool IsVirtualAggregation<TAggregationModel>()
        {
            var type = typeof(TAggregationModel);
            return IsVirtualAggregation(type);
        }

        /// <summary>
        /// Determine whether is virtual model
        /// </summary>
        /// <param name="modelType">Aggregation model type</param>
        /// <returns>Return model whether is virtual model</returns>
        internal static bool IsVirtualAggregation(Type modelType)
        {
            if (modelType == null)
            {
                return false;
            }
            VirtualAggregations.TryGetValue(modelType.GUID, out var virtualModel);
            return virtualModel;
        }

        /// <summary>
        /// Determines whether is a entity model
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <returns></returns>
        internal static bool IsEntityAggregation(Type modelType)
        {
            if (modelType != null)
            {
                var modelAttributes = modelType.GetCustomAttributes(typeof(AggregationModelAttribute), false);
                return modelAttributes?.Any(c => (((AggregationModelAttribute)c).Feature & AggregationFeature.Virtual) == AggregationFeature.Virtual) ?? false;
            }
            return false;
        }

        /// <summary>
        /// Get model relation entity
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <returns></returns>
        internal static Type GetAggregationModelRelationEntityType(Type modelType)
        {
            if (modelType == null)
            {
                return null;
            }
            return modelType.GetCustomAttribute<AggregationModelAttribute>()?.EntityType;
        }
    }
}
