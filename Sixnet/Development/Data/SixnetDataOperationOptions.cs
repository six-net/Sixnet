// "Company © 2025. All rights reserved."

using System.Threading;

using Sixnet.Development.Data.Database;

namespace Sixnet.Development.Data
{
    /// <summary>
    /// Sixnet Data operation options
    /// </summary>
    [Serializable]
    public class SixnetDataOperationOptions
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
        /// Gets or sets the data operation behavior.
        /// Default is 'default'
        /// </summary>
        public DataOperationBehavior LogicalDeleteBehavior { get; set; } = DataOperationBehavior.Default;

        /// <summary>
        /// Not overwrite fields
        /// </summary>
        HashSet<string> _notOverwriteFieldNames;

        /// <summary>
        /// Whether not overwrite all field
        /// Priority greater than NotOverwriteFieldNames
        /// </summary>
        public bool NotOverwrite { get; set; }

        /// <summary>
        /// Gets or sets the split on field name
        /// </summary>
        public string SpiltOnFieldName { get; set; }

        /// <summary>
        /// Gets or sets the increment field behavior.
        /// Default is 'default'
        /// </summary>
        public DataOperationBehavior InsertIncrementFieldBehavior { get; set; } = DataOperationBehavior.Default;

        /// <summary>
        /// Create data operation options
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        internal static SixnetDataOperationOptions Create(CancellationToken? cancellationToken = null, bool mustAffectData = false)
        {
            return new SixnetDataOperationOptions()
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
                _notOverwriteFieldNames ??= new HashSet<string>();
                foreach (var name in fieldNames)
                {
                    _notOverwriteFieldNames.Add(name);
                }
            }
        }

        /// <summary>
        /// Is not overwrite field
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        internal bool IsNotOverwriteField(string fieldName)
        {
            return !string.IsNullOrWhiteSpace(fieldName) && (_notOverwriteFieldNames?.Contains(fieldName) ?? false);
        }

        /// <summary>
        /// Allow logical delete
        /// </summary>
        /// <param name="globalAllowLogicalDelete"></param>
        /// <returns></returns>
        public bool AllowLogicalDelete(bool globalAllowLogicalDelete)
        {
            return LogicalDeleteBehavior != DataOperationBehavior.Disable
               && (globalAllowLogicalDelete || LogicalDeleteBehavior == DataOperationBehavior.Enable);
        }

        /// <summary>
        /// Allow insert increment field
        /// </summary>
        /// <param name="globalAllowInsertIncrementField"></param>
        /// <returns></returns>
        public bool AllowInsertIncrementField(bool globalAllowInsertIncrementField)
        {
            return InsertIncrementFieldBehavior != DataOperationBehavior.Disable
                && (globalAllowInsertIncrementField || InsertIncrementFieldBehavior == DataOperationBehavior.Enable);
        }

        /// <summary>
        /// Gets or sets the split table behavior
        /// </summary>
        public SplitTableBehavior SplitTableBehavior { get; set; }
    }
}
