using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sixnet.Development.Data.Client;

namespace Sixnet.Development.Work
{
    /// <summary>
    /// Defines work contract
    /// </summary>
    public interface IWork : IDisposable
    {
        #region Properties

        /// <summary>
        /// Get the work id
        /// </summary>
        string WorkId { get; }

        /// <summary>
        /// Get the data client
        /// </summary>
        IDataClient DataClient { get; }

        #endregion

        #region Methods

        #region Commit

        /// <summary>
        /// Commit work
        /// </summary>
        /// <returns></returns>
        bool Commit();

        /// <summary>
        /// Commit work
        /// </summary>
        /// <returns></returns>
        Task<bool> CommitAsync(CancellationToken cancellationToken = default);

        #endregion

        #region Rollback

        /// <summary>
        /// Rollback work
        /// </summary>
        void Rollback();

        #endregion

        #region Work event

        /// <summary>
        /// Subscribe commit success event
        /// </summary>
        /// <param name="eventHandlers">Event handlers</param>
        void SubscribeCommitSuccessEvent(IEnumerable<Action<IWork>> eventHandlers);

        /// <summary>
        /// Subscribe commit success event
        /// </summary>
        /// <param name="eventHandlers">Event handlers</param>
        void SubscribeCommitSuccessEvent(params Action<IWork>[] eventHandlers);

        #endregion

        #endregion
    }
}
