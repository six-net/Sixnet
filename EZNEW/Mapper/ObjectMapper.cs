using EZNEW.Fault;
using EZNEW.DependencyInjection;

namespace EZNEW.Mapper
{
    /// <summary>
    /// Object mapper
    /// </summary>
    public static class ObjectMapper
    {
        static ObjectMapper()
        {
            Current = ContainerManager.Resolve<IMapper>();
        }

        /// <summary>
        /// Get or set current object mapper
        /// </summary>
        public static IMapper Current { get; set; }

        /// <summary>
        /// Convert object
        /// </summary>
        /// <typeparam name="TTarget">Target data type</typeparam>
        /// <param name="sourceObject">Source object</param>
        /// <returns>Return the target data object</returns>
        public static TTarget MapTo<TTarget>(object sourceObject)
        {
            if (sourceObject == null)
            {
                return default;
            }
            var targetType = typeof(TTarget);
            if (targetType == sourceObject.GetType())
            {
                return (TTarget)sourceObject;
            }
            if (Current == null)
            {
                throw new EZNEWException($"{nameof(Current)} mapper is not initialized");
            }
            return Current.MapTo<TTarget>(sourceObject);
        }
    }
}
