// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sixnet.Development.Data.Client;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Sixnet database update context
    /// </summary>
    public class SixnetUpdateDatabaseContext
    {
        /// <summary>
        /// Update parameter
        /// </summary>
        public SixnetUpdateDatabaseParameter UpdateParameter { get; set; }

        /// <summary>
        /// Version max record id
        /// </summary>
        public Dictionary<Version, long> VersionMaxRecordIds { get; set; }

        public Version GetMaxVersion()
        {
            if (VersionMaxRecordIds.IsNullOrEmpty())
            {
                return new Version(0, 0, 0, 0);
            }

            return VersionMaxRecordIds.Keys.Max();
        }

        /// <summary>
        /// Get version max record id
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public long GetVersionMaxRecordId(Version version)
        {
            if (version == null)
            {
                return 0;
            }
            if (VersionMaxRecordIds.TryGetValue(version, out var item))
            {
                return item;
            }
            return 0;
        }

        /// <summary>
        /// Add new record
        /// </summary>
        /// <param name="record"></param>
        internal void AddNewRecord(ISixnetDatabaseUpdateRecord record)
        {
            if (record == null)
            {
                return;
            }
            VersionMaxRecordIds ??= [];
            if (VersionMaxRecordIds.TryGetValue(record.Version, out var currentRecordId))
            {
                if (currentRecordId < record.Id)
                {
                    VersionMaxRecordIds[record.Version] = record.Id;
                }
            }
            else
            {
                VersionMaxRecordIds[record.Version] = record.Id;
            }
        }
    }
}
