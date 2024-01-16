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
    public interface ISplitTableProvider
    {
        /// <summary>
        /// Get split table names
        /// </summary>
        /// <param name="options">Get split table name options</param>
        /// <returns></returns>
        List<string> GetSplitTableNames(GetSplitTableNameOptions options);

        /// <summary>
        /// Get finally split table names
        /// </summary>
        /// <param name="splitBehavior">Split table behavior</param>
        /// <param name="allTableNames">All table names</param>
        /// <param name="parsedTableNames">Parsed table names</param>
        /// <returns></returns>
        List<string> GetFinallySplitTableNames(SplitTableBehavior splitBehavior, List<string> allTableNames, List<string> parsedTableNames);
    }
}
