using System;
using System.Collections.Generic;
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
        /// Not overwrite fields
        /// </summary>
        internal HashSet<string> NotOverwriteFieldNames { get; set; }

        /// <summary>
        /// Whether not overwrite all field
        /// Priority greater  than NotOverwriteFieldNames
        /// </summary>
        public bool NotOverwrite { get; set; }

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

        /// <summary>
        /// Not overwrite fields
        /// </summary>
        /// <param name="fieldNames"></param>
        public void NotOverwriteFields(params string[] fieldNames)
        {
            if (!fieldNames.IsNullOrEmpty())
            {
                NotOverwriteFieldNames ??= new HashSet<string>();
                foreach (var name in fieldNames)
                {
                    NotOverwriteFieldNames.Add(name);
                }
            }
        }

        internal bool IsNotNotOverwriteField(string fieldName)
        {
            return !string.IsNullOrWhiteSpace(fieldName) && (NotOverwriteFieldNames?.Contains(fieldName) ?? false);
        }
    }
}
