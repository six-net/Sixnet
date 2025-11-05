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
        List<string> ResolveTableNames(ResolveSplitTableNameParameter parameter);

        /// <summary>
        /// Get split table names
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns></returns>
        List<string> GetTableNames(GetSplitTableNameParameter parameter);

        /// <summary>
        /// Filter all table names
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns></returns>
        List<string> FilterAllTableNames(FilterAllSplitTableNameParameter parameter);
    }
}
