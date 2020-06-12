using System;
using System.Collections.Generic;
using System.Linq;

namespace EZNEW.Code
{
    /// <summary>
    /// Provides the functionality of generate serial number
    /// </summary>
    public static class SerialNumber
    {
        /// <summary>
        /// Generator groups
        /// </summary>
        static readonly Dictionary<string, SnowflakeNet> GeneratorGroups = new Dictionary<string, SnowflakeNet>();

        /// <summary>
        /// Default generator
        /// </summary>
        static readonly SnowflakeNet DefaultGenerator = new SnowflakeNet(1, 1);

        #region Registers a serial number generator for the specified groups

        /// <summary>
        /// Registers a serial number generator for the specified groups
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

        #endregion
    }
}
