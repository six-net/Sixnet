// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Sixnet.Exceptions;

namespace Sixnet.Development.Data.Database
{
    public abstract class SixnetBaseSplitTableProvider : ISixnetSplitTableProvider
    {
        /// <summary>
        /// Resolve split table names
        /// </summary>
        /// <param name="parameter">Get split table name options</param>
        /// <returns></returns>
        public virtual List<SixnetDatabaseObjectName> ResolveTableNames(SixnetResolveSplitTableNameParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameter == null, nameof(parameter));
            SixnetDirectThrower.ThrowArgNullIf(parameter.EntityConfiguration == null, nameof(SixnetResolveSplitTableNameParameter.EntityConfiguration));
            SixnetDirectThrower.ThrowArgNullIf(parameter.SplitBehavior == null, nameof(SixnetResolveSplitTableNameParameter.SplitBehavior));
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(parameter.RootTableName.Name), nameof(SixnetResolveSplitTableNameParameter.RootTableName.Name));

            var splitBehavior = parameter.SplitBehavior;

            List<SixnetDatabaseObjectName> splitTableNames = null;
            if (splitBehavior.SplitValues.IsNullOrEmpty())
            {
                return splitTableNames ?? [];
            }

            var figureOutTableNames = FigureOutTableNames(parameter);
            if (figureOutTableNames.IsNullOrEmpty())
            {
                return splitTableNames ?? [];
            }
            if (splitTableNames.IsNullOrEmpty())
            {
                return figureOutTableNames ?? [];
            }
            splitTableNames.AddRange(figureOutTableNames);
            splitTableNames = [.. splitTableNames.Distinct().OrderBy(c => c.Name)];
            return splitTableNames;
        }

        /// <summary>
        /// Figure out table names
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected abstract List<SixnetDatabaseObjectName> FigureOutTableNames(SixnetResolveSplitTableNameParameter parameter);

        /// <summary>
        /// Get table names
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns></returns>
        public virtual List<SixnetDatabaseObjectName> GetTableNames(SixnetGetSplitTableNameParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameter == null, nameof(parameter));

            var splitBehavior = parameter.Behavior;
            var allTableNames = parameter.AllTableNames;
            var resolvedTableNames = parameter.ResolvedTableNames;
            if (splitBehavior.SplitTableNameFilter != null)
            {
                resolvedTableNames = splitBehavior.SplitTableNameFilter(allTableNames, resolvedTableNames)?.ToList();
            }
            else if (!allTableNames.IsNullOrEmpty() && !resolvedTableNames.IsNullOrEmpty())
            {
                switch (splitBehavior.SelectionPattern)
                {
                    case SixnetSplitTableNameSelectionPattern.Range:
                        var sortedAllTableNames = allTableNames.OrderBy(t => t).ToList();
                        var sortedResolvedTableNames = resolvedTableNames.OrderBy(t => t).ToList();

                        var resolvedMinTableName = sortedResolvedTableNames.First();
                        var actualMinTableName = sortedAllTableNames.First();

                        var resolvedMaxTableName = sortedResolvedTableNames.Last();
                        var actualMaxTableName = sortedAllTableNames.Last();

                        actualMinTableName = resolvedMinTableName.CompareTo(actualMinTableName) >= 0 ? resolvedMinTableName : actualMinTableName;
                        actualMaxTableName = resolvedMaxTableName.CompareTo(actualMaxTableName) >= 0 ? actualMaxTableName : resolvedMaxTableName;

                        var actualMinTableNameIndex = sortedAllTableNames.FindIndex(t => t.Equals(actualMinTableName));
                        var actualMaxTableNameIndex = sortedAllTableNames.FindIndex(t => t.Equals(actualMaxTableName));

                        resolvedTableNames = sortedAllTableNames.GetRange(actualMinTableNameIndex, (actualMaxTableNameIndex - actualMinTableNameIndex) + 1);
                        break;
                }
            }
            return resolvedTableNames;
        }

        /// <summary>
        /// Filter all table names
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns></returns>
        public virtual List<SixnetDatabaseObjectName> FilterAllTableNames(SixnetFilterAllSplitTableNameParameter parameter)
        {
            if (parameter?.AllTableNames.IsNullOrEmpty() ?? true)
            {
                return new List<SixnetDatabaseObjectName>(0);
            }
            var tableNameRegex = new Regex(@$"^{parameter.RootTableName.FullName}_\d+$", RegexOptions.IgnoreCase);
            return parameter.AllTableNames?.Where(tn => !string.IsNullOrWhiteSpace(tn.Name) && tableNameRegex.IsMatch($"{tn.FullName}")).ToList();
        }

        /// <summary>
        /// Change root table names
        /// </summary>
        /// <param name="currentTableNames">Current table names</param>
        /// <param name="newRootTableName">New root table name</param>
        /// <returns></returns>
        public virtual Dictionary<SixnetDatabaseObjectName, SixnetDatabaseObjectName> ChangeRootTableNames(IEnumerable<SixnetDatabaseObjectName> currentTableNames, SixnetDatabaseObjectName newRootTableName)
        {
            if (currentTableNames.IsNullOrEmpty())
            {
                return new Dictionary<SixnetDatabaseObjectName, SixnetDatabaseObjectName>(0);
            }

            var newNamesDict = new Dictionary<SixnetDatabaseObjectName, SixnetDatabaseObjectName>();
            foreach (var currentTableName in currentTableNames)
            {
                var newTableName = currentTableName.Clone();
                var idx = currentTableName.Name.LastIndexOf('_');
                newTableName.Name = $"{newRootTableName.Name}{currentTableName.Name.Substring(idx)}";
                newNamesDict[currentTableName] = newTableName;
            }
            return newNamesDict;
        }

    }
}
