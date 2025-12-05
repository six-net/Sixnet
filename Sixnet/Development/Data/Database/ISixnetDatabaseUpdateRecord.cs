// "Company © 2025. All rights reserved."

using System;
using System.Threading.Tasks;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Database update record
    /// </summary>
    public interface ISixnetDatabaseUpdateRecord : IComparable
    {
        /// <summary>
        /// Record id
        /// </summary>
        long Id { get; set; }

        /// <summary>
        /// App version
        /// </summary>
        Version Version { get; set; }

        /// <summary>
        /// Note
        /// </summary>
        string Note { get; set; }

        /// <summary>
        /// Update
        /// </summary>
        /// <returns></returns>
        Task UpdateAsync(SixnetUpdateDatabaseContext context);

        /// <summary>
        /// Rollback
        /// </summary>
        /// <returns></returns>
        Task RollbackAsync(SixnetUpdateDatabaseContext context);
    }
}
