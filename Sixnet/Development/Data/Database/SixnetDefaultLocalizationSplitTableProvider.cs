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
    internal class SixnetDefaultLocalizationSplitTableProvider : SixnetBaseSplitTableProvider
    {
        public const string Name = "SIXNET_DEFAULT_LOCALIZATION_SPLIT_TABLE_PROVIDER";

        public override List<SixnetDatabaseObjectName> FilterAllTableNames(SixnetFilterAllSplitTableNameParameter parameter)
        {
            if (parameter?.AllTableNames.IsNullOrEmpty() ?? true)
            {
                return new List<SixnetDatabaseObjectName>(0);
            }
            return parameter.AllTableNames?.Where(tn => !string.IsNullOrWhiteSpace(tn.Name)
            && tn.Name.Contains("sixnet_localization", StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public override List<SixnetDatabaseObjectName> GetTableNames(SixnetGetSplitTableNameParameter parameter)
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

        protected override List<SixnetDatabaseObjectName> FigureOutTableNames(SixnetResolveSplitTableNameParameter parameter)
        {
            var splitBehavior = parameter.SplitBehavior;
            var tableNames = new HashSet<string>();
            foreach (var splitValue in splitBehavior.SplitValues)
            {
                string splitStringValue = splitValue?.ToString();
                if (!string.IsNullOrWhiteSpace(splitStringValue))
                {
                    tableNames.Add($"{parameter.RootTableName.Name}_{splitStringValue.ToUpper().ReplaceByRegex("[.-]", "_")}");
                }
            }
            return [.. tableNames.Select(t => SixnetDatabaseObjectName.Create(t, SixnetDatabaseObjectType.Table, parameter.RootTableName.SchemaName))];
        }
    }
}
