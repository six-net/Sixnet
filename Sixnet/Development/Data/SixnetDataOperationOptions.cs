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
        /// Gets or sets the split on field name
        /// </summary>
        public string SpiltOnFieldName { get; set; }

        /// <summary>
        /// Gets or sets the split table behavior
        /// </summary>
        public SixnetSplitTableBehavior SplitTableBehavior { get; set; }

        /// <summary>
        /// timeout (in seconds)
        /// </summary>
        public int? Timeout { get; set; }

        #region Not overwrite

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

        #endregion

        #region Create

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

        #endregion

        #region Increment field

        /// <summary>
        /// Gets or sets the increment field behavior.
        /// Default is 'default'
        /// </summary>
        internal SixnetDataOperationBehavior InsertIncrementFieldBehavior { get; set; } = SixnetDataOperationBehavior.Default;

        /// <summary>
        /// Enable insert increment field
        /// </summary>
        public void EnableInsertIncrementField()
        {
            InsertIncrementFieldBehavior = SixnetDataOperationBehavior.Enabled;
        }

        /// <summary>
        /// Disable insert increment field
        /// </summary>
        public void DisableInsertIncrementField()
        {
            InsertIncrementFieldBehavior = SixnetDataOperationBehavior.Disabled;
        }

        /// <summary>
        /// Allow insert increment field
        /// </summary>
        /// <param name="globalAllowInsertIncrementField"></param>
        /// <returns></returns>
        internal bool AllowInsertIncrementField(bool globalAllowInsertIncrementField)
        {
            return InsertIncrementFieldBehavior != SixnetDataOperationBehavior.Disabled
                && (globalAllowInsertIncrementField || InsertIncrementFieldBehavior == SixnetDataOperationBehavior.Enabled);
        }

        #endregion

        #region Logical delete

        /// <summary>
        /// Gets or sets the data operation behavior.
        /// Default is 'default'
        /// </summary>
        internal SixnetDataOperationBehavior LogicalDeleteBehavior { get; set; } = SixnetDataOperationBehavior.Default;

        /// <summary>
        /// Enable logical delete
        /// </summary>
        public void EnableLogicalDelete()
        {
            LogicalDeleteBehavior = SixnetDataOperationBehavior.Enabled;
        }

        /// <summary>
        /// Disable logical delete
        /// </summary>
        public void DisableLogicalDelete()
        {
            LogicalDeleteBehavior = SixnetDataOperationBehavior.Disabled;
        }

        /// <summary>
        /// Allow logical delete
        /// </summary>
        /// <param name="globalAllowLogicalDelete"></param>
        /// <returns></returns>
        internal bool AllowLogicalDelete(bool globalAllowLogicalDelete)
        {
            return LogicalDeleteBehavior != SixnetDataOperationBehavior.Disabled
               && (globalAllowLogicalDelete || LogicalDeleteBehavior == SixnetDataOperationBehavior.Enabled);
        }

        #endregion

        #region Versioning table

        /// <summary>
        /// Gets or sets the versioning table type
        /// Default is 'All'
        /// </summary>
        internal SixnetVersioningTableType VersioningTableType { get; set; } = SixnetVersioningTableType.PrimaryTable;

        /// <summary>
        /// For all table
        /// </summary>
        public void ForAllTable()
        {
            VersioningTableType = SixnetVersioningTableType.All;
        }

        /// <summary>
        /// For primary table
        /// </summary>
        public void ForPrimaryTable()
        {
            VersioningTableType = SixnetVersioningTableType.PrimaryTable;
        }

        /// <summary>
        /// For version table
        /// </summary>
        public void ForVersionTable()
        {
            VersioningTableType = SixnetVersioningTableType.VersionTable;
        }

        /// <summary>
        /// Include primary table
        /// </summary>
        /// <returns></returns>
        internal bool IncludePrimaryTable()
        {
            return VersioningTableType == SixnetVersioningTableType.PrimaryTable || VersioningTableType == SixnetVersioningTableType.All;
        }

        /// <summary>
        /// Include version table
        /// </summary>
        /// <returns></returns>
        internal bool IncludeVersionTable()
        {
            return VersioningTableType == SixnetVersioningTableType.VersionTable || VersioningTableType == SixnetVersioningTableType.All;
        }

        #endregion
    }
}
