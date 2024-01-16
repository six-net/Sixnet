using System;
using System.Collections.Generic;
using System.Linq;

namespace Sixnet.Code
{
    /// <summary>
    /// Provides the functionality of generate serial number
    /// </summary>
    public static class SerialNumber
    {
        /// <summary>
        /// Generator groups
        /// </summary>
        static readonly Dictionary<string, SnowflakeNet> GeneratorGroups = new();

        /// <summary>
        /// Default generator
        /// </summary>
        static readonly SnowflakeNet DefaultGenerator = new(1, 1);

        #region Registers a serial number generator for the specified groups

        /// <summary>
        /// Register a serial number generator for the specified groups
        /// </summary>
        /// param name="groups">Groups</param>
        /// <param name="dataCenterId">Data center id(1-31)</param>
        /// <param name="worderId">Work id(1-31)</param>
        /// <param name="sequence">Sequence</param>
        /// <param name="startDate">Start date</param>
        public static void RegisterGenerator(IEnumerable<string> groups, long dataCenterId, long workerId, long sequence = 0L, DateTime? startDate = null)
        {
            if (groups.IsNullOrEmpty())
            {
                return;
            }
            groups = groups.Distinct();
            foreach (string group in groups)
            {
                if (!GeneratorGroups.ContainsKey(group))
                {
                    GeneratorGroups.Add(group, new SnowflakeNet(dataCenterId, workerId, sequence, startDate));
                }
            }
        }

        /// <summary>
        /// Register a serial number generator for the specified type
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="dataCenterId">Data center id</param>
        /// <param name="workerId">Worker id</param>
        /// <param name="sequence">Sequece</param>
        /// <param name="startDate">Start date</param>
        public static void RegisterGenerator<T>(long dataCenterId, long workerId, long sequence = 0L, DateTime? startDate = null)
        {
            RegisterGenerator(new string[1] { typeof(T).FullName }, dataCenterId, workerId, sequence, startDate);
        }

        #endregion

        #region Generates a serial number by group

        /// <summary>
        /// Generates a serial number by group
        /// Use the default generator to create number if 'group' is null or empty ,or group didn't register
        /// </summary>
        /// <param name="group">Group</param>
        /// <returns>Return a serial number</returns>
        public static long GenerateSerialNumber(string group = "")
        {
            if (string.IsNullOrWhiteSpace(group) || !GeneratorGroups.ContainsKey(group))
            {
                return DefaultGenerator.GenerateId();
            }
            return GeneratorGroups[group].GenerateId();
        }

        /// <summary>
        /// Generates a serial number by data type
        /// Use the default generator to create number if the type didn't register
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static long GenerateSerialNumber<T>()
        {
            return GenerateSerialNumber(typeof(T).FullName);
        }

        #endregion
    }
}
