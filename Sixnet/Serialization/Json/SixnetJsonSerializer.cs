using Sixnet.DependencyInjection;
using System;

namespace Sixnet.Serialization.Json
{
    /// <summary>
    /// Json serialize helper
    /// </summary>
    public static class SixnetJsonSerializer
    {
        /// <summary>
        /// Default json serialization provider
        /// </summary>
        static readonly IJsonSerializationProvider _defaultJsonSerializationProvider = new DefaultJsonSerializationProvider();

        /// <summary>
        /// Serialize an object to a json string
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="obj">Object</param>
        /// <param name="configure">Configure json options. Use the global json options if is null</param>
        /// <returns></returns>
        public static string Serialize<T>(T obj, Action<SixnetJsonSerializationOptions> configure = null)
        {
            return GetJsonSerializationProvider().Serialize(obj, GetJsonOptions(configure));
        }

        /// <summary>
        /// Deserialize a json string to an object
        /// </summary>
        /// <param name="json">Json string</param>
        /// <param name="configure">Configure json options. Use the global json options if is null</param>
        /// <returns></returns>
        public static T Deserialize<T>(string json, Action<SixnetJsonSerializationOptions> configure = null)
        {
            return GetJsonSerializationProvider().Deserialize<T>(json, GetJsonOptions(configure));
        }

        /// <summary>
        /// Get json serialization provider
        /// </summary>
        /// <returns></returns>
        static IJsonSerializationProvider GetJsonSerializationProvider()
        {
            return SixnetContainer.GetService<IJsonSerializationProvider>() ?? _defaultJsonSerializationProvider;
        }

        /// <summary>
        /// Get json options
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        static SixnetJsonSerializationOptions GetJsonOptions(Action<SixnetJsonSerializationOptions> configure = null)
        {
            if (configure == null)
            {
                return SixnetContainer.GetOptions<SixnetJsonSerializationOptions>();
            }
            else
            {
                var options = new SixnetJsonSerializationOptions();
                configure(options);
                return options;
            }
        }
    }
}
