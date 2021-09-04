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
        /// <typeparam name="TDestination">Target data type</typeparam>
        /// <param name="sourceObject">Source object</param>
        /// <returns>Return the target data object</returns>
        public static TDestination MapTo<TDestination>(object sourceObject)
        {
            if (sourceObject == null)
            {
                return default;
            }
            var targetType = typeof(TDestination);
            if (targetType.IsAssignableFrom(sourceObject.GetType()))
            {
                return (TDestination)sourceObject;
            }
            if (Current == null)
            {
                throw new EZNEWException($"{nameof(Current)} mapper is not initialized");
            }
            return Current.MapTo<TDestination>(sourceObject);
        }

        /// <summary>
        /// Convert object
        /// </summary>
        /// <param name="destinationType">Destionation type</param>
        /// <param name="source">Souce object</param>
        /// <returns>Return a destionation object</returns>
        public static object MapTo(Type destinationType, object source)
        {
            var sourceType = source?.GetType();
            if (sourceType == null)
            {
                return null;
            }
            if (destinationType.IsAssignableFrom(sourceType))
            {
                return source;
            }
            if (Current == null)
            {
                throw new EZNEWException($"{nameof(Current)} mapper is not initialized");
            }
            return Current.MapTo(destinationType, source);
        }
    }
}
