// "Company © 2025. All rights reserved."

using System.Data;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Defines database transaction
    /// </summary>
    public class DatabaseTransaction : IDisposable
    {
        /// <summary>
        /// Get the db transaction
        /// </summary>
        public IDbTransaction DbTransaction { get; private set; }

        internal DatabaseTransaction(IDbTransaction dbTransaction)
        {
            if (dbTransaction == null)
            {
                throw new ArgumentNullException(nameof(dbTransaction));
            }
            DbTransaction = dbTransaction;
        }

        internal void Commit()
        {
            DbTransaction?.Commit();
        }

        internal void Rollback()
        {
            DbTransaction?.Rollback();
        }

        public void Dispose()
        {
            Rollback();
            DbTransaction?.Dispose();
        }
    }
}
