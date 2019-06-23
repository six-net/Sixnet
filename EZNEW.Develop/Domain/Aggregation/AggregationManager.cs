using EZNEW.Framework.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EZNEW.Develop.Domain.Aggregation
{
    /// <summary>
    /// aggregation manager
    /// </summary>
    public static class AggregationManager
    {
        /// <summary>
        /// virtual aggregation
        /// </summary>
        static Dictionary<Guid, bool> virtualAggregationCollection = new Dictionary<Guid, bool>();

        /// <summary>
        /// config aggregation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        internal static void ConfigAggregationConfig<T>() where T : AggregationRoot<T>
        {
            var type = typeof(T);
            ConfigAggregationConfig(type);
        }

        /// <summary>
        /// config aggregation
        /// </summary>
        /// <param name="modelType"></param>
        internal static void ConfigAggregationConfig(Type modelType, IEnumerable<Type> allTypes = null)
        {
            if (modelType == null)
            {
                return;
            }
            var virtualAttributes = modelType.GetCustomAttributes(typeof(VirtualAggregationAttribute), true);
            if (!virtualAttributes.IsNullOrEmpty())
            {
                virtualAggregationCollection[modelType.GUID] = true;
            }
            allTypes = allTypes ?? modelType.Assembly.GetTypes();
            var subTypes = allTypes.Where(t => t.BaseType == modelType);
            if (subTypes.IsNullOrEmpty())
            {
                return;
            }
            foreach (var subType in subTypes)
            {
                ConfigAggregationConfig(subType, allTypes);
            }
        }

        /// <summary>
        /// is virtual aggregation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal static bool IsVirtualAggregation<T>()
        {
            var type = typeof(T);
            return IsVirtualAggregation(type);
        }

        /// <summary>
        /// is virtual aggregation
        /// </summary>
        /// <param name="modelType">model type</param>
        /// <returns></returns>
        internal static bool IsVirtualAggregation(Type modelType)
        {
            if (modelType == null)
            {
                return false;
            }
            virtualAggregationCollection.TryGetValue(modelType.GUID, out var virtualModel);
            return virtualModel;
        }
    }
}
