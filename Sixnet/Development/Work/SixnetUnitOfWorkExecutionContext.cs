// "Company © 2025. All rights reserved."

using System.Threading;
using System.Threading.Tasks;

namespace Sixnet.Development.Work
{
    public class SixnetUnitOfWorkExecutionContext
    {
        /// <summary>
        /// Gets or sets the work
        /// </summary>
        public ISixnetWork Work { get; set; }

        #region Commit

        /// <summary>
        /// Commit work
        /// </summary>
        /// <returns></returns>
        public bool Commit()
        {
            return Work?.Commit() ?? false;
        }

        /// <summary>
        /// Commit work
        /// </summary>
        /// <returns></returns>
        public Task<bool> CommitAsync(CancellationToken cancellationToken = default)
        {
            return Work?.CommitAsync(cancellationToken);
        }

        #endregion

        #region Rollback

        /// <summary>
        /// Rollback work
        /// </summary>
        public void Rollback()
        {
            Work?.Rollback();
        }

        #endregion
    }
}
