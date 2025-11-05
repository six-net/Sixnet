// "Company © 2025. All rights reserved."

using System.Threading;
using System.Threading.Tasks;

using Sixnet.Development.Data.Database;

namespace Sixnet.Development.Data.Client
{
    /// <summary>
    /// Defines data access client
    /// </summary>
    public interface ISixnetDataClient : ISixnetDataAccessClient
    {
        #region Open

        /// <summary>
        /// Open data client
        /// </summary>
        void Open();

        #endregion

        #region Close

        void Close();

        #endregion

        #region Transaction

        /// <summary>
        /// Commit
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        Task CommitAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Commit transaction
        /// </summary>
        void Commit();

        /// <summary>
        /// Rollback transaction
        /// </summary>
        void Rollback();

        #endregion

        #region Database

        /// <summary>
        /// Get connections
        /// </summary>
        /// <returns></returns>
        Dictionary<string, DatabaseConnection> GetConnections();

        /// <summary>
        /// Get database servers
        /// </summary>
        /// <returns></returns>
        List<DatabaseServer> GetDatabaseServers();

        #endregion
    }
}
