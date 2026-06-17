// "Company © 2025. All rights reserved."

using System.Reflection.Metadata;
using System.Text.RegularExpressions;

using Sixnet.Development.Entity;
using Sixnet.Exceptions;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Date split table provider
    /// </summary>
    internal class SixnetDefaultDateSplitTableProvider : ISixnetSplitTableProvider
    {
        /// <summary>
        /// Resolve split table names
        /// </summary>
        /// <param name="parameter">Get split table name options</param>
        /// <returns></returns>
        public List<SixnetDatabaseObjectName> ResolveTableNames(SixnetResolveSplitTableNameParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameter == null, nameof(parameter));
            SixnetDirectThrower.ThrowArgNullIf(parameter.EntityConfiguration == null, nameof(SixnetResolveSplitTableNameParameter.EntityConfiguration));
            SixnetDirectThrower.ThrowArgNullIf(parameter.SplitBehavior == null, nameof(SixnetResolveSplitTableNameParameter.SplitBehavior));
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(parameter.RootTableName.Name), nameof(SixnetResolveSplitTableNameParameter.RootTableName.Name));

            var splitBehavior = parameter.SplitBehavior;
            if (splitBehavior.SplitValues.IsNullOrEmpty())
            {
                return new List<SixnetDatabaseObjectName>(0);
            }

            var tableNames = new HashSet<string>();
            foreach (var splitValue in splitBehavior.SplitValues)
            {
                var splitDate = GetSplitTableDate((splitValue is DateTimeOffset splitOffsetValue)
                    ? splitOffsetValue.DateTime
                    : splitValue, parameter.EntityConfiguration.SplitTableType);
                tableNames.Add(GetSplitTable(parameter, splitDate));
                if (parameter.ExpansionNum > 0)
                {
                    for (var i = 1; i <= parameter.ExpansionNum; i++)
                    {
                        var expansionSplitDate = GetExpansionSplitTableDate(splitDate, parameter.EntityConfiguration.SplitTableType, i);
                        tableNames.Add(GetSplitTable(parameter, expansionSplitDate));
                    }
                }
            }
            return tableNames.Select(t => SixnetDatabaseObjectName.Create(t, SixnetDatabaseObjectType.Table, parameter.RootTableName.SchemaName)).ToList();
        }

        /// <summary>
        /// Get table names
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns></returns>
        public List<SixnetDatabaseObjectName> GetTableNames(SixnetGetSplitTableNameParameter parameter)
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
        public List<SixnetDatabaseObjectName> FilterAllTableNames(SixnetFilterAllSplitTableNameParameter parameter)
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
        public Dictionary<SixnetDatabaseObjectName, SixnetDatabaseObjectName> ChangeRootTableNames(IEnumerable<SixnetDatabaseObjectName> currentTableNames, SixnetDatabaseObjectName newRootTableName)
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

        /// <summary>
        /// Get a split table info
        /// </summary>
        /// <param name="entityConfig"></param>
        /// <param name="splitTableDate"></param>
        /// <returns></returns>
        static string GetSplitTable(SixnetResolveSplitTableNameParameter parameter, DateTime splitTableDate)
        {
            var rootTableName = parameter.RootTableName.Name;
            return $"{rootTableName}_{splitTableDate:yyyyMMdd}";
        }

        /// <summary>
        /// Get split table date
        /// </summary>
        /// <param name="splitDateTime">Split date time</param>
        /// <param name="splitTableType">Split table type</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        static DateTime GetSplitTableDate(DateTime splitDateTime, SixnetSplitTableType splitTableType)
        {
            return splitTableType switch
            {
                SixnetSplitTableType.Day => System.Convert.ToDateTime(splitDateTime.ToString("yyyy-MM-dd")),
                SixnetSplitTableType.Week => SixnetDefaultDateSplitTableProvider.GetMondayDateTime(splitDateTime),
                SixnetSplitTableType.Month => System.Convert.ToDateTime(splitDateTime.ToString("yyyy-MM-01")),
                SixnetSplitTableType.Season => SixnetDefaultDateSplitTableProvider.GetSeasonDateTime(splitDateTime),
                SixnetSplitTableType.Year => System.Convert.ToDateTime(splitDateTime.ToString("yyyy-01-01")),
                _ => throw new Exception($"Not support {splitTableType}"),
            };
        }

        /// <summary>
        /// Get expansion split table date
        /// </summary>
        /// <param name="splitDateTime"></param>
        /// <param name="splitTableType"></param>
        /// <param name="expansionValue"></param>
        /// <returns></returns>
        static DateTime GetExpansionSplitTableDate(DateTime splitDateTime, SixnetSplitTableType splitTableType, int expansionValue)
        {
            return splitTableType switch
            {
                SixnetSplitTableType.Day => splitDateTime.AddDays(expansionValue),
                SixnetSplitTableType.Week => splitDateTime.AddDays(expansionValue * 7),
                SixnetSplitTableType.Month => splitDateTime.AddMonths(expansionValue),
                SixnetSplitTableType.Season => splitDateTime.AddMonths(expansionValue * 3),
                SixnetSplitTableType.Year => splitDateTime.AddYears(expansionValue),
                _ => throw new Exception($"Not support {splitTableType}"),
            };
        }

        /// <summary>
        /// Get monday date time
        /// </summary>
        /// <param name="splitDateTime">Split date time</param>
        /// <returns></returns>
        static DateTime GetMondayDateTime(DateTime splitDateTime)
        {
            var day = (int)splitDateTime.DayOfWeek - 1;
            day = day == -1 ? 6 : day;
            var value = new TimeSpan(day, 0, 0, 0);
            return splitDateTime.Subtract(value);
        }

        /// <summary>
        /// Get season date time
        /// </summary>
        /// <param name="splitDateTime">Split date time</param>
        /// <returns></returns>
        static DateTime GetSeasonDateTime(DateTime splitDateTime)
        {
            var month = splitDateTime.Month;
            month = ((month / 4) * 3) + 1;
            return new DateTime(splitDateTime.Year, month, 1, 0, 0, 0);
        }
    }
}
