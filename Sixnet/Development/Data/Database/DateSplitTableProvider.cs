using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Sixnet.Development.Entity;
using Sixnet.Exceptions;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Date split table provider
    /// </summary>
    internal class DateSplitTableProvider : ISplitTableProvider
    {
        /// <summary>
        /// Get split table names
        /// </summary>
        /// <param name="options">Get split table name options</param>
        /// <returns></returns>
        public List<string> GetSplitTableNames(GetSplitTableNameOptions options)
        {
            ThrowHelper.ThrowArgNullIf(options == null, nameof(options));
            ThrowHelper.ThrowArgNullIf(options.EntityConfiguration == null, nameof(GetSplitTableNameOptions.EntityConfiguration));
            ThrowHelper.ThrowNullOrEmptyIf(options.SplitBehavior == null, nameof(GetSplitTableNameOptions.SplitBehavior));
            ThrowHelper.ThrowNullOrEmptyIf(string.IsNullOrWhiteSpace(options.RootTableName), nameof(GetSplitTableNameOptions.RootTableName));

            var splitBehavior = options.SplitBehavior;
            if (splitBehavior.SplitValues.IsNullOrEmpty())
            {
                return new List<string>(0);
            }

            var tableNames = new List<string>();
            foreach (var splitValue in splitBehavior.SplitValues)
            {
                var splitDate = GetSplitTableDate((splitValue is DateTimeOffset splitOffsetValue) ? splitOffsetValue.DateTime : splitValue, options.EntityConfiguration.SplitTableType);
                tableNames.Add(GetSplitTable(options.EntityConfiguration, splitDate));
            }
            return tableNames;
        }

        /// <summary>
        /// Get finally split table names
        /// </summary>
        /// <param name="splitBehavior">Split table behavior</param>
        /// <param name="allTableNames">All table names</param>
        /// <param name="parsedTableNames">Parsed table names</param>
        /// <returns></returns>
        public List<string> GetFinallySplitTableNames(SplitTableBehavior splitBehavior, List<string> allTableNames, List<string> parsedTableNames)
        {
            if (allTableNames.IsNullOrEmpty() || parsedTableNames.IsNullOrEmpty())
            {
                return new List<string>(0);
            }
            switch (splitBehavior.SelectionPattern)
            {
                case SplitTableNameSelectionPattern.Range:
                    var sortedAllTableNames = allTableNames.OrderBy(t => t).ToList();
                    var sortedParsedTableNames = parsedTableNames.OrderBy(t => t);
                    var minTableName = parsedTableNames.First();
                    var maxTableName = parsedTableNames.Last();
                    var minTableNameIndex = sortedAllTableNames.FindIndex(t => string.Equals(t, minTableName, StringComparison.OrdinalIgnoreCase));
                    var maxTableNameIndex = sortedAllTableNames.FindIndex(t => string.Equals(t, maxTableName, StringComparison.OrdinalIgnoreCase));
                    parsedTableNames = sortedAllTableNames.GetRange(minTableNameIndex, (maxTableNameIndex - minTableNameIndex) + 1);
                    break;
            }
            return parsedTableNames;
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
                SplitTableType.Day => Convert.ToDateTime(splitDateTime.ToString("yyyy-MM-dd")),
                SplitTableType.Week => GetMondayDateTime(splitDateTime),
                SplitTableType.Month => Convert.ToDateTime(splitDateTime.ToString("yyyy-MM-01")),
                SplitTableType.Season => GetSeasonDateTime(splitDateTime),
                SplitTableType.Year => Convert.ToDateTime(splitDateTime.ToString("yyyy-01-01")),
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
