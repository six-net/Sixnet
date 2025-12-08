// "Company © 2025. All rights reserved."

using System.Text.RegularExpressions;

using Sixnet.Development.Entity;
using Sixnet.Exceptions;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Date split table provider
    /// </summary>
    internal class DefaultDateSplitTableProvider : ISixnetSplitTableProvider
    {
        /// <summary>
        /// Resolve split table names
        /// </summary>
        /// <param name="parameter">Get split table name options</param>
        /// <returns></returns>
        public List<string> ResolveTableNames(ResolveSplitTableNameParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameter == null, nameof(parameter));
            SixnetDirectThrower.ThrowArgNullIf(parameter.EntityConfiguration == null, nameof(ResolveSplitTableNameParameter.EntityConfiguration));
            SixnetDirectThrower.ThrowArgNullIf(parameter.SplitBehavior == null, nameof(ResolveSplitTableNameParameter.SplitBehavior));
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(parameter.RootTableName), nameof(ResolveSplitTableNameParameter.RootTableName));

            var splitBehavior = parameter.SplitBehavior;
            if (splitBehavior.SplitValues.IsNullOrEmpty())
            {
                return new List<string>(0);
            }

            var tableNames = new List<string>();
            foreach (var splitValue in splitBehavior.SplitValues)
            {
                var splitDate = GetSplitTableDate((splitValue is DateTimeOffset splitOffsetValue)
                    ? splitOffsetValue.DateTime
                    : splitValue, parameter.EntityConfiguration.SplitTableType);
                tableNames.Add(GetSplitTable(parameter.EntityConfiguration, splitDate));
                if (parameter.ExpansionNum > 0)
                {
                    for (var i = 1; i <= parameter.ExpansionNum; i++)
                    {
                        var expansionSplitDate = GetExpansionSplitTableDate(splitDate, parameter.EntityConfiguration.SplitTableType, i);
                        tableNames.Add(GetSplitTable(parameter.EntityConfiguration, expansionSplitDate));
                    }
                }
            }
            return tableNames.Distinct().ToList();
        }

        /// <summary>
        /// Get table names
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns></returns>
        public List<string> GetTableNames(GetSplitTableNameParameter parameter)
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
                    case SplitTableNameSelectionPattern.Range:
                        var sortedAllTableNames = allTableNames.OrderBy(t => t).ToList();
                        var sortedResolvedTableNames = resolvedTableNames.OrderBy(t => t);
                        var minTableName = sortedResolvedTableNames.First();
                        var maxTableName = sortedResolvedTableNames.Last();
                        var minTableNameIndex = sortedAllTableNames.FindIndex(t => string.Equals(t, minTableName, StringComparison.OrdinalIgnoreCase));
                        var maxTableNameIndex = sortedAllTableNames.FindIndex(t => string.Equals(t, maxTableName, StringComparison.OrdinalIgnoreCase));
                        resolvedTableNames = sortedAllTableNames.GetRange(minTableNameIndex, (maxTableNameIndex - minTableNameIndex) + 1);
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
        public List<string> FilterAllTableNames(FilterAllSplitTableNameParameter parameter)
        {
            if (parameter?.AllTableNames.IsNullOrEmpty() ?? true)
            {
                return new List<string>(0);
            }
            var tableNameRegex = new Regex(@$"^{parameter.RootTableName}_\d+$");
            return parameter.AllTableNames?.Where(tn => !string.IsNullOrWhiteSpace(tn) && tableNameRegex.IsMatch(tn)).ToList();
        }

        /// <summary>
        /// Get a split table info
        /// </summary>
        /// <param name="entityConfig"></param>
        /// <param name="splitTableDate"></param>
        /// <returns></returns>
        string GetSplitTable(EntityConfiguration entityConfig, DateTime splitTableDate)
        {
            var rootTableName = entityConfig.TableName;
            return $"{rootTableName}_{splitTableDate.ToString("yyyyMMdd")}";
        }

        /// <summary>
        /// Get split table date
        /// </summary>
        /// <param name="splitDateTime">Split date time</param>
        /// <param name="splitTableType">Split table type</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        DateTime GetSplitTableDate(DateTime splitDateTime, SplitTableType splitTableType)
        {
            return splitTableType switch
            {
                SplitTableType.Day => System.Convert.ToDateTime(splitDateTime.ToString("yyyy-MM-dd")),
                SplitTableType.Week => GetMondayDateTime(splitDateTime),
                SplitTableType.Month => System.Convert.ToDateTime(splitDateTime.ToString("yyyy-MM-01")),
                SplitTableType.Season => GetSeasonDateTime(splitDateTime),
                SplitTableType.Year => System.Convert.ToDateTime(splitDateTime.ToString("yyyy-01-01")),
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
        DateTime GetExpansionSplitTableDate(DateTime splitDateTime, SplitTableType splitTableType, int expansionValue)
        {
            return splitTableType switch
            {
                SplitTableType.Day => splitDateTime.AddDays(expansionValue),
                SplitTableType.Week => splitDateTime.AddDays(expansionValue * 7),
                SplitTableType.Month => splitDateTime.AddMonths(expansionValue),
                SplitTableType.Season => splitDateTime.AddMonths(expansionValue * 3),
                SplitTableType.Year => splitDateTime.AddYears(expansionValue),
                _ => throw new Exception($"Not support {splitTableType}"),
            };
        }

        /// <summary>
        /// Get monday date time
        /// </summary>
        /// <param name="splitDateTime">Split date time</param>
        /// <returns></returns>
        DateTime GetMondayDateTime(DateTime splitDateTime)
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
        DateTime GetSeasonDateTime(DateTime splitDateTime)
        {
            var month = splitDateTime.Month;
            month = ((month / 4) * 3) + 1;
            return new DateTime(splitDateTime.Year, month, 1, 0, 0, 0);
        }
    }
}
