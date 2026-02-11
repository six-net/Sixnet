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
    /// <summary>
    /// Localization split table provider
    /// </summary>
    internal class DefaultLocalizationSplitTableProvider : ISixnetSplitTableProvider
    {
        public const string Name = "SIXNET_DEFAULT_LOCALIZATION_SPLIT_TABLE_PROVIDER";

        public List<DatabaseObjectName> FilterAllTableNames(FilterAllSplitTableNameParameter parameter)
        {
            if (parameter?.AllTableNames.IsNullOrEmpty() ?? true)
            {
                return new List<DatabaseObjectName>(0);
            }
            return parameter.AllTableNames?.Where(tn => !string.IsNullOrWhiteSpace(tn.Name)
            && tn.Name.Contains("sixnet_localization", StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public List<DatabaseObjectName> GetTableNames(GetSplitTableNameParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameter == null, nameof(parameter));

            var splitBehavior = parameter.Behavior;
            var allTableNames = parameter.AllTableNames;
            var resolvedTableNames = parameter.ResolvedTableNames;
            if (splitBehavior.SplitTableNameFilter != null)
            {
                resolvedTableNames = splitBehavior.SplitTableNameFilter(allTableNames, resolvedTableNames)?.ToList();
            }
            return resolvedTableNames;
        }

        public List<DatabaseObjectName> ResolveTableNames(ResolveSplitTableNameParameter parameter)
        {
            SixnetDirectThrower.ThrowArgNullIf(parameter == null, nameof(parameter));
            SixnetDirectThrower.ThrowArgNullIf(parameter.EntityConfiguration == null, nameof(ResolveSplitTableNameParameter.EntityConfiguration));
            SixnetDirectThrower.ThrowArgNullIf(parameter.SplitBehavior == null, nameof(ResolveSplitTableNameParameter.SplitBehavior));
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(parameter.RootTableName.Name), nameof(ResolveSplitTableNameParameter.RootTableName.Name));

            var splitBehavior = parameter.SplitBehavior;
            if (splitBehavior.SplitValues.IsNullOrEmpty())
            {
                return new List<DatabaseObjectName>(0);
            }

            var tableNames = new HashSet<string>();
            foreach (var splitValue in splitBehavior.SplitValues)
            {
                string splitStringValue = splitValue?.ToString();
                if (!string.IsNullOrWhiteSpace(splitStringValue))
                {
                    tableNames.Add($"{parameter.RootTableName.Name}_{splitStringValue.ToUpper().ReplaceByRegex("[.-]", "_")}");
                }
            }
            return tableNames.Select(t => DatabaseObjectName.Create(t, DatabaseObjectType.Table, parameter.RootTableName.SchemaName)).ToList();
        }
    }
}
