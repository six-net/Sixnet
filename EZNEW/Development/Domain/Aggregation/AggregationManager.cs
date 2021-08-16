using System;
using System.Collections.Generic;
using System.Linq;

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
        internal static void ConfigureAggregationModel<TAggregationModel>() where TAggregationModel : AggregationRoot<TAggregationModel>
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
            var virtualAttributes = modelType.GetCustomAttributes(typeof(VirtualAggregationAttribute), true);
            if (!virtualAttributes.IsNullOrEmpty())
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
    }
}
