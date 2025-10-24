using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Database update
    /// </summary>
    public interface ISixnetDatabaseUpdate
    {
        /// <summary>
        /// Update
        /// </summary>
        /// <returns></returns>
        Task UpdateAsync();

        /// <summary>
        /// Rollback
        /// </summary>
        /// <returns></returns>
        Task RollbackAsync();
    }
}
