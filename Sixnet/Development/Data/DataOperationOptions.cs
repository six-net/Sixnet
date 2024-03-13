using System;
using System.Threading;
using Sixnet.Localization;

namespace Sixnet.Development.Data
{
    /// <summary>
    /// Data operation options
    /// </summary>
    [Serializable]
    public class DataOperationOptions
    {
        /// <summary>
        /// Gets or sets the cancellation token
        /// </summary>
        public CancellationToken? CancellationToken { get; set; } = null;

        /// <summary>
        /// Whether must affect data
        /// </summary>
        public bool MustAffectData { get; set; }

        /// <summary>
        /// Whether disable logical delete
        /// </summary>
        public bool DisableLogicalDelete { get; set; }

        /// <summary>
        /// Create data operation options
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        internal static DataOperationOptions Create(CancellationToken? cancellationToken = null, bool mustAffectData = false)
        {
            if (!cancellationToken.HasValue && !mustAffectData)
            {
                return null;
            }
            return new DataOperationOptions()
            {
                CancellationToken = cancellationToken,
                MustAffectData = mustAffectData
            };
        }
    }
}
