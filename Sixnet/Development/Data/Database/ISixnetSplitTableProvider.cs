// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Defines split table provider
    /// </summary>
    public interface ISixnetSplitTableProvider
    {
        /// <summary>
        /// Get split table names
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns></returns>
        List<SixnetDatabaseObjectName> ResolveTableNames(SixnetResolveSplitTableNameParameter parameter);

        /// <summary>
        /// Get split table names
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns></returns>
        List<SixnetDatabaseObjectName> GetTableNames(SixnetGetSplitTableNameParameter parameter);

        /// <summary>
        /// Filter all table names
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns></returns>
        List<SixnetDatabaseObjectName> FilterAllTableNames(SixnetFilterAllSplitTableNameParameter parameter);

        /// <summary>
        /// Change root table names
        /// </summary>
        /// <param name="currentTableNames">Current table names</param>
        /// <param name="newRootTableName">New root table name</param>
        /// <returns>Key: old name, value: new name</returns>
        Dictionary<SixnetDatabaseObjectName, SixnetDatabaseObjectName> ChangeRootTableNames(IEnumerable<SixnetDatabaseObjectName> currentTableNames, SixnetDatabaseObjectName newRootTableName);
    }
}
