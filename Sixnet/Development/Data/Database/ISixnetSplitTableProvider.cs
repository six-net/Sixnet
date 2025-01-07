using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sixnet.Development.Data.Client;
using Sixnet.Development.Entity;

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
