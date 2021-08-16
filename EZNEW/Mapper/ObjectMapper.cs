using System;
using EZNEW.Exceptions;
using AutoMapper;
using EZNEW.Application;

namespace EZNEW.Mapper
{
    /// <summary>
    /// Object mapper
    /// </summary>
    public static class ObjectMapper
    {
        static ObjectMapper()
        {
            ApplicationInitializer.Init();
        }

        /// <summary>
        /// Default mapper builder
        /// </summary>
        static readonly ConventionMapperBuilder DefaultMapperBuilder = new ConventionMapperBuilder();

        /// <summary>
        /// Get or set current object mapper
        /// </summary>
        public static IMapper Current { get; internal set; }

        /// <summary>
        /// Configure map
        /// </summary>
        /// <param name="configureAction">Configure action</param>
        /// <returns></returns>
        public static void ConfigureMap(Action<IMapperConfigurationExpression> configureAction)
        {
            if (configureAction == null)
            {
                return;
            }
            DefaultMapperBuilder.ConfigureMap(configureAction);
        }

        /// <summary>
        /// build mapper
        /// </summary>
        /// <returns></returns>
        public static IMapper BuildMapper()
        {
            if (Current == null || Current is ConventionMapper)
            {
                Current = DefaultMapperBuilder.CreateMapper();
            }
            return Current;
        }

        /// <summary>
        /// Set mapper
        /// </summary>
        /// <param name="mapper">Mapper</param>
        public static void SetMapper(IMapper mapper)
        {
            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }
            Current = mapper;
        }

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
            if (targetType.IsAssignableFrom(sourceObject.GetType()))
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
