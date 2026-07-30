// "Company © 2025. All rights reserved."

using System.Collections;
using System.Data;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

using Sixnet.Development.Data.Command;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Data.Field.Formatting;
using Sixnet.Development.Entity;
using Sixnet.Development.Queryable;
using Sixnet.Exceptions;
using Sixnet.Logging;
using Sixnet.Model;
using Sixnet.Reflection;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Base data command resolver
    /// </summary>
    public abstract partial class SixnetBaseDataCommandResolver : ISixnetDataCommandResolver
    {
        #region Properties

        public string KeywordPrefix { get; set; }
        public string KeywordSuffix { get; set; }
        public string ConditionStartKeyword { get; set; } = " WHERE ";
        public string AndConnector { get; set; } = "AND";
        public string OrConnector { get; set; } = "OR";
        public string EqualOperator { get; set; } = "=";
        public string GreaterThanOperator { get; set; } = ">";
        public string GreaterThanOrEqualOperator { get; set; } = ">=";
        public string NotEqualOperator { get; set; } = "<>";
        public string LessThanOperator { get; set; } = "<";
        public string LessThanOrEqualOperator { get; set; } = "<=";
        public string InOperator { get; set; } = " IN ";
        public string NotInOperator { get; set; } = " NOT IN ";
        public string LikeOperator { get; set; } = " LIKE ";
        public string NotLikeOperator { get; set; } = " NOT LIKE ";
        public string IsNullOperator { get; set; } = " IS NULL";
        public string NotNullOperator { get; set; } = " IS NOT NULL";
        public string TrueOperator { get; set; } = "1=1";
        public string FalseOperator { get; set; } = "1<>1";
        public string SortKeyword { get; set; } = " ORDER BY ";
        public string DescKeyword { get; set; } = " DESC";
        public string AscKeyword { get; set; } = " ASC";
        public string GroupByKeyword { get; set; } = " GROUP BY ";
        public string ParameterPrefix { get; set; } = "@";
        public string PagingTableName { get; set; } = "SIXNET_TEMTABLE_PAGING";
        public string PagingCountTableName { get; set; } = "SIXNET_TEMTABLE_PAGING_COUNT";
        public string TablePetNameKeyword { get; set; } = " AS ";
        public string ColumnPetNameKeyword { get; set; } = " AS ";
        public string WithTableKeyword { get; set; } = " AS ";
        public string NullKeyword { get; set; } = "NULL";
        public string DistinctKeyword { get; set; } = " DISTINCT";
        public string NegationKeyword { get; set; } = " NOT";
        public bool ParameterizationJsonFormatter { get; set; } = true;
        public Dictionary<string, bool> NotParameterizationFormatterNameDict { get; set; }
        public SixnetDatabaseType DatabaseType { get; set; }
        public Dictionary<SixnetJoinType, string> JoinOperatorDict { get; set; } = new Dictionary<SixnetJoinType, string>()
        {
            { SixnetJoinType.InnerJoin," INNER JOIN " },
            { SixnetJoinType.CrossJoin," CROSS JOIN " },
            { SixnetJoinType.LeftJoin," LEFT JOIN " },
            { SixnetJoinType.RightJoin," RIGHT JOIN " },
            { SixnetJoinType.FullJoin," FULL JOIN " }
        };
        public Dictionary<SixnetCalculationOperator, string> CalculationOperators = new Dictionary<SixnetCalculationOperator, string>(4)
        {
            [SixnetCalculationOperator.Add] = "+",
            [SixnetCalculationOperator.Subtract] = "-",
            [SixnetCalculationOperator.Multiply] = "*",
            [SixnetCalculationOperator.Divide] = "/",
        };
        public ISixnetFieldFormatter DefaultFieldFormatter { get; set; }
        public int DefaultCharLength { get; set; } = 50;
        public int DefaultDecimalLength { get; set; } = 20;
        public int DefaultDecimalPrecision { get; set; } = 4;
        public Dictionary<DbType, string> DbTypeDefaultValues { get; set; }
        public Func<SixnetDatabaseObjectName, SixnetDatabaseObjectName> FormatObjectNameFunc { get; set; }
        public Func<SixnetDatabaseObjectName, SixnetDatabaseObjectName> WrapObjectNameFunc { get; set; }
        public Func<SixnetDatabaseObjectName, string> GetObjectFullNameFunc { get; set; }
        public string RecursiveKeyword { get; set; }
        public bool UseFieldForRecursive { get; set; } = false;
        public bool SplitWrapParameter { get; set; } = false;

        #endregion

        #region Methods

        #region Statement

        #region Query

        /// <summary>
        /// Generate a query statement
        /// </summary>
        /// <param name="command">Database single command</param>
        /// <returns></returns>
        public virtual SixnetQueryDatabaseStatement GenerateDatabaseQueryStatement(SixnetSingleDatabaseCommand command)
        {
            //create context
            var context = new SixnetDataCommandResolveContext(command.Connection, command.DataCommand);

            //translation query
            var queryableTranResult = Translate(context);

            //generate statement
            var statement = GenerateQueryStatementCore(context, queryableTranResult, SixnetQueryableLocation.Top);
            var queryable = queryableTranResult.GetOriginalQueryable();
            statement.ScriptType = GetCommandType(queryable.Info.ScriptType);
            return statement;
        }

        /// <summary>
        /// Generate a query statement
        /// </summary>
        /// <param name="command">Database multiple command</param>
        /// <returns></returns>
        public virtual SixnetQueryDatabaseStatement GenerateDatabaseQueryStatement(SixnetMultipleDatabaseCommand command)
        {
            SixnetDirectThrower.ThrowArgNullIf(command?.DataCommands.IsNullOrEmpty() ?? true, "Not set any data command");

            //create context
            var context = new SixnetDataCommandResolveContext(command.Connection, null);
            var commandScriptBuilder = new StringBuilder();
            SixnetDataCommandParameters groupParameters = null;
            var commandType = CommandType.Text;
            foreach (var dataCommand in command.DataCommands)
            {
                context.SetCommand(dataCommand);

                //translation queryable
                var queryableTranResult = Translate(context);

                //generate statement
                var cmdQueryableStatement = GenerateQueryStatementCore(context, queryableTranResult, SixnetQueryableLocation.Top);

                commandScriptBuilder.AppendLine(cmdQueryableStatement.Script + ";");
                groupParameters = groupParameters == null
                    ? cmdQueryableStatement.Parameters
                    : groupParameters.Union(cmdQueryableStatement.Parameters);
                var queryable = queryableTranResult.GetOriginalQueryable();
                commandType = GetCommandType(queryable.Info.ScriptType);
            }
            var statement = SixnetQueryDatabaseStatement.Create(commandScriptBuilder.ToString(), groupParameters);
            statement.ScriptType = commandType;
            return statement;
        }

        /// <summary>
        /// Get a paging statement
        /// </summary>
        /// <param name="command">Database single command</param>
        /// <returns></returns>
        public virtual SixnetQueryDatabaseStatement GenerateDatabaseQueryPagingStatement(SixnetSingleDatabaseCommand command)
        {
            var queryable = command?.DataCommand?.Queryable;
            //create context
            var context = new SixnetDataCommandResolveContext(command.Connection, command.DataCommand);
            //translation query
            var translationResult = Translate(context);
            string sqlStatement;
            IEnumerable<ISixnetField> outputFields = null;
            switch (queryable.Info.ExecutionMode)
            {
                case SixnetQueryableExecutionMode.Script:
                    sqlStatement = translationResult.GetCondition();
                    break;
                case SixnetQueryableExecutionMode.Regular:
                default:
                    //table pet name
                    var tablePetName = context.GetDefaultTablePetName(queryable);
                    //combine
                    var combine = translationResult.GetCombine();
                    var hasCombine = !string.IsNullOrWhiteSpace(combine);
                    //group
                    var group = translationResult.GetGroup();
                    //having
                    var having = translationResult.GetHavingCondition();
                    //prescript output
                    var targetScript = translationResult.GetPreOutputStatement();

                    if (string.IsNullOrWhiteSpace(targetScript))
                    {
                        //target
                        var targetStatement = GetFromTargetStatement(context, queryable, SixnetQueryableLocation.Top, tablePetName);
                        outputFields = targetStatement.OutputFields;
                        //condition
                        var condition = translationResult.GetCondition(ConditionStartKeyword);
                        //join
                        var join = translationResult.GetJoin();
                        //target statement
                        targetScript = $"{targetStatement.Script}{join}{condition}{group}{having}";
                    }
                    else
                    {
                        targetScript = $"{targetScript}{group}{having}";
                    }

                    // output fields
                    if (outputFields.IsNullOrEmpty() || !queryable.Info.SelectedFields.IsNullOrEmpty())
                    {
                        outputFields = SixnetDataManager.GetQueryableFields(DatabaseType, queryable.GetModelType(), queryable, context.IsRootQueryable(queryable));
                    }
                    var outputFieldString = FormatFieldsString(context, queryable, SixnetQueryableLocation.Top, SixnetFieldLocation.Output, outputFields);

                    //sort
                    var sort = translationResult.GetSort();
                    if (string.IsNullOrWhiteSpace(sort))
                    {
                        sort = GetDefaultSort(context, translationResult, queryable, outputFields, tablePetName);
                    }
                    var hasSort = !string.IsNullOrWhiteSpace(sort);

                    //statement
                    switch (queryable.Info.OutputType)
                    {
                        case SixnetQueryableOutputType.Count:
                        case SixnetQueryableOutputType.Predicate:
                            throw new NotSupportedException("Not supported this output type for paging");
                        default:
                            sqlStatement = $"SELECT{GetDistinctString(queryable)} {outputFieldString} FROM {targetScript}";
                            break;
                    }
                    var preScript = FormatPreScript(context);

                    //limit
                    var pagingFilter = command.DataCommand.PagingFilter;
                    var limit = GetLimitString((pagingFilter.Page - 1) * pagingFilter.PageSize, pagingFilter.PageSize, hasSort);
                    var dataSqlStatement = hasCombine
                        ? $"{preScript}(SELECT {tablePetName}.* FROM ({sqlStatement}{sort}{limit}){TablePetNameKeyword}{tablePetName}){combine}"
                        : $"{preScript}{sqlStatement}{sort}{limit};";
                    var totalSqlStatement = $"{preScript}SELECT COUNT(1){ColumnPetNameKeyword}SixnetPagingTotalDataCount FROM ({sqlStatement}){TablePetNameKeyword}{tablePetName};";
                    sqlStatement = $"{dataSqlStatement}{Environment.NewLine}{totalSqlStatement}";
                    break;
            }

            //parameter
            var parameters = context.GetParameters();

            //log script
            LogScript(sqlStatement, parameters);
            return SixnetQueryDatabaseStatement.Create(sqlStatement, context.GetParameters());
        }

        /// <summary>
        /// Get query statement
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <param name="translationResult">Queryable translation result</param>
        /// <param name="queryableLocation">Queryable location</param>
        /// <returns></returns>
        protected abstract SixnetQueryDatabaseStatement GenerateQueryStatementCore(SixnetDataCommandResolveContext context, SixnetQueryableTranslationResult translationResult, SixnetQueryableLocation queryableLocation);

        #endregion

        #region Execution

        /// <summary>
        /// Generate database execution statement
        /// </summary>
        /// <param name="command">Database single command</param>
        /// <returns></returns>
        public virtual List<SixnetExecutionDatabaseStatement> GenerateDatabaseExecutionStatements(SixnetSingleDatabaseCommand command)
        {
            var commandResolveContext = new SixnetDataCommandResolveContext(command.Connection, command.DataCommand);
            return GenerateDatabaseExecutionStatements(commandResolveContext);
        }

        /// <summary>
        /// Generate database statement groups
        /// </summary>
        /// <param name="command">Database execution command</param>
        /// <returns></returns>
        public virtual List<SixnetExecutionDatabaseStatement> GenerateDatabaseExecutionStatements(SixnetMultipleDatabaseCommand command)
        {
            SixnetDirectThrower.ThrowArgNullIf(command?.DataCommands.IsNullOrEmpty() ?? true, "Data commands is null or empty");

            var statements = new List<SixnetExecutionDatabaseStatement>();
            var batchExecutionConfig = SixnetDataManager.GetBatchSetting(DatabaseType)
                ?? SixnetDatabaseBatchSetting.Default;
            var groupStatementsCount = batchExecutionConfig.GroupStatementsCount;
            groupStatementsCount = groupStatementsCount < 0 ? 1 : groupStatementsCount;
            var groupParameterCount = batchExecutionConfig.GroupParametersCount;
            groupParameterCount = groupParameterCount < 0 ? 1 : groupParameterCount;
            var commandScriptBuilder = new StringBuilder();
            var incrScriptBuilder = new StringBuilder();
            var appendedStatementCount = 0;
            var mustAffectData = false;
            SixnetDataCommandParameters groupParameters = null;
            var scriptType = CommandType.Text;
            var commandResolveContext = new SixnetDataCommandResolveContext(command.Connection, null);

            //create group execution statement
            SixnetExecutionDatabaseStatement GetGroupExecutionStatement()
            {
                if (incrScriptBuilder.Length > 0)
                {
                    commandScriptBuilder.Append($"SELECT {incrScriptBuilder.ToString().Trim(',')};");
                }
                var statement = new SixnetExecutionDatabaseStatement()
                {
                    Script = commandScriptBuilder.ToString(),
                    ScriptType = scriptType,
                    MustAffectData = mustAffectData,
                    Parameters = groupParameters
                };
                appendedStatementCount = 0;
                commandScriptBuilder.Clear();
                incrScriptBuilder.Clear();
                groupParameters = null;
                mustAffectData = false;
                scriptType = CommandType.Text;
                commandResolveContext.Reset();

                //Trace log
                LogExecutionStatement(statement);

                return statement;
            }

            //append statement
            void AppendExecutionStatement(SixnetExecutionDatabaseStatement statement)
            {
                commandScriptBuilder.AppendLine(statement.Script);
                groupParameters = groupParameters == null
                    ? statement.Parameters
                    : groupParameters.Union(statement.Parameters);
                mustAffectData |= statement.MustAffectData;
                scriptType = statement.ScriptType;
                if (!string.IsNullOrWhiteSpace(statement.IncrScript))
                {
                    incrScriptBuilder.Append($",{statement.IncrScript}");
                }
                appendedStatementCount++;
            }

            foreach (var cmd in command.DataCommands)
            {
                commandResolveContext.ClearParameters();
                commandResolveContext.SetCommand(cmd);
                var executionStatements = GenerateDatabaseExecutionStatements(commandResolveContext);
                if (!executionStatements.IsNullOrEmpty())
                {
                    foreach (var statement in executionStatements)
                    {
                        if (statement.PerformAlone)
                        {
                            if (appendedStatementCount > 0)
                            {
                                statements.Add(GetGroupExecutionStatement());
                            }
                            AppendExecutionStatement(statement);
                            statements.Add(GetGroupExecutionStatement());
                        }
                        else
                        {
                            AppendExecutionStatement(statement);
                            if (commandResolveContext.GetParameterSequence() >= groupParameterCount || appendedStatementCount >= groupStatementsCount)
                            {
                                statements.Add(GetGroupExecutionStatement());
                            }
                        }
                    }
                }
            }

            if (appendedStatementCount > 0)
            {
                statements.Add(GetGroupExecutionStatement());
            }
            return statements;
        }

        /// <summary>
        /// Generate database execution statement
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <returns></returns>
        protected virtual List<SixnetExecutionDatabaseStatement> GenerateDatabaseExecutionStatements(SixnetDataCommandResolveContext context)
        {
            var command = context.DataCommandExecutionContext.Command;
            var statements = new List<SixnetExecutionDatabaseStatement>();

            //Get script statement
            SixnetExecutionDatabaseStatement GetScriptStatement()
            {
                return new SixnetExecutionDatabaseStatement()
                {
                    Script = command.Script,
                    Parameters = ConvertParameter(command.ScriptParameters),
                    ScriptType = GetCommandType(command),
                    MustAffectData = command.Options?.MustAffectData ?? false,
                    HasPreScript = true
                };
            }

            if (command.ExecutionMode == SixnetCommandExecutionMode.Script)
            {
                statements.Add(GetScriptStatement());
            }
            else
            {
                switch (command.OperationType)
                {
                    case SixnetDataOperationType.Insert:
                        if (!(command?.FieldsAssignment?.NewValues?.IsNullOrEmpty() ?? true))
                        {
                            statements.AddRange(GenerateInsertStatements(context));
                        }
                        break;
                    case SixnetDataOperationType.Update:
                        if (!(command?.FieldsAssignment?.NewValues?.IsNullOrEmpty() ?? true))
                        {
                            statements.AddRange(GenerateUpdateStatements(context));
                        }
                        break;
                    case SixnetDataOperationType.Delete:
                        statements.AddRange(GenerateDeleteStatements(context));
                        break;
                    default:
                        statements.Add(GetScriptStatement());
                        break;
                }
            }
            return statements;
        }

        /// <summary>
        /// Get insert statement
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <returns></returns>
        protected abstract List<SixnetExecutionDatabaseStatement> GenerateInsertStatements(SixnetDataCommandResolveContext context);

        /// <summary>
        /// Get update statement
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <returns></returns>
        protected abstract List<SixnetExecutionDatabaseStatement> GenerateUpdateStatements(SixnetDataCommandResolveContext context);

        /// <summary>
        /// Get delete statement
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <returns></returns>
        protected abstract List<SixnetExecutionDatabaseStatement> GenerateDeleteStatements(SixnetDataCommandResolveContext context);

        #endregion

        #region Migration

        #region Generate database migration statements

        /// <summary>
        /// Generate database migration statements
        /// </summary>
        /// <param name="command">Database migration command</param>
        /// <returns></returns>
        public virtual List<SixnetExecutionDatabaseStatement> GenerateDatabaseMigrationStatements(SixnetMigrationDatabaseCommand command)
        {
            var statements = new List<SixnetExecutionDatabaseStatement>();

            #region Clear database

            if (command?.MigrationInfo?.ClearDatabase ?? false)
            {
                var clearForeignKeyStatements = GetDeleteAllForeignKeyStatements(command);
                if (!clearForeignKeyStatements.IsNullOrEmpty())
                {
                    statements.AddRange(clearForeignKeyStatements);
                }

                var clearViewStatements = GetDeleteAllViewStatements(command);
                if (!clearViewStatements.IsNullOrEmpty())
                {
                    statements.AddRange(clearViewStatements);
                }

                var clearTableStatements = GetDeleteAllTableStatements(command);
                if (!clearTableStatements.IsNullOrEmpty())
                {
                    statements.AddRange(clearTableStatements);
                }

                var clearProcedureStatements = GetDeleteAllProcedureStatements(command);
                if (!clearProcedureStatements.IsNullOrEmpty())
                {
                    statements.AddRange(clearProcedureStatements);
                }

                var clearFunctionStatements = GetDeleteAllFunctionStatements(command);
                if (!clearFunctionStatements.IsNullOrEmpty())
                {
                    statements.AddRange(clearFunctionStatements);
                }

                var clearCustomTypeStatements = GetDeleteAllCustomTypeStatements(command);
                if (!clearCustomTypeStatements.IsNullOrEmpty())
                {
                    statements.AddRange(clearCustomTypeStatements);
                }

                return statements;
            }
            #endregion

            #region Delete foreign key

            // Delete foreign key
            var deleteForeignKeyStatements = GetDeleteForeignKeyStatements(command);
            if (!deleteForeignKeyStatements.IsNullOrEmpty())
            {
                statements.AddRange(deleteForeignKeyStatements);
            }

            // Delete all foreign key
            if (command?.MigrationInfo?.DeleteAllForeignKey ?? false)
            {
                var deleteAllForeignKeyStatements = GetDeleteAllForeignKeyStatements(command);
                if (!deleteAllForeignKeyStatements.IsNullOrEmpty())
                {
                    statements.AddRange(deleteAllForeignKeyStatements);
                }
            }

            #endregion

            #region Views

            // Delete all view
            if (command?.MigrationInfo?.DeleteAllView ?? false)
            {
                var deleteAllViewStatements = GetDeleteAllViewStatements(command);
                if (!deleteAllViewStatements.IsNullOrEmpty())
                {
                    statements.AddRange(deleteAllViewStatements);
                }
            }

            #endregion

            #region Tables

            // New tables
            var createTableStatements = GetCreateTableStatements(command);
            if (!createTableStatements.IsNullOrEmpty())
            {
                statements.AddRange(createTableStatements);
            }

            // Rename tables
            var renameTableStatements = GetRenameTableStatements(command);
            if (!renameTableStatements.IsNullOrEmpty())
            {
                statements.AddRange(renameTableStatements);
            }

            // Delete tables
            var deleteTableStatements = GetDeleteTableStatements(command);
            if (!deleteTableStatements.IsNullOrEmpty())
            {
                statements.AddRange(deleteTableStatements);
            }

            // Delete all tables
            if (command?.MigrationInfo?.DeleteAllTable ?? false)
            {
                var deleteAllTableStatements = GetDeleteAllTableStatements(command);
                if (!deleteAllTableStatements.IsNullOrEmpty())
                {
                    statements.AddRange(deleteAllTableStatements);
                }
            }

            #endregion

            #region Fields

            // New fields
            var newFieldStatements = GetAddFieldStatements(command);
            if (!newFieldStatements.IsNullOrEmpty())
            {
                statements.AddRange(newFieldStatements);
            }

            // Update fields
            var updateFieldStatements = GetUpdateFieldStatements(command);
            if (!updateFieldStatements.IsNullOrEmpty())
            {
                statements.AddRange(updateFieldStatements);
            }

            // Delete fields
            var deleteFieldStatements = GetDeleteFieldStatements(command);
            if (!deleteFieldStatements.IsNullOrEmpty())
            {
                statements.AddRange(deleteFieldStatements);
            }

            #endregion

            #region Index

            // Delete index
            var deleteIndexStatements = GetDeleteIndexStatements(command);
            if (!deleteIndexStatements.IsNullOrEmpty())
            {
                statements.AddRange(deleteIndexStatements);
            }

            // New index
            var addIndexStatements = GetAddIndexStatements(command);
            if (!addIndexStatements.IsNullOrEmpty())
            {
                statements.AddRange(addIndexStatements);
            }

            #endregion

            #region Procedure

            // Delete all procedure
            if (command?.MigrationInfo?.DeleteAllProcedure ?? false)
            {
                var deleteAllProcedureStatements = GetDeleteAllProcedureStatements(command);
                if (!deleteAllProcedureStatements.IsNullOrEmpty())
                {
                    statements.AddRange(deleteAllProcedureStatements);
                }
            }

            #endregion

            #region Function

            // Delete all function
            if (command?.MigrationInfo?.DeleteAllFunction ?? false)
            {
                var deleteAllFunctionStatements = GetDeleteAllFunctionStatements(command);
                if (!deleteAllFunctionStatements.IsNullOrEmpty())
                {
                    statements.AddRange(deleteAllFunctionStatements);
                }
            }

            #endregion

            #region Custom type

            // Delete all custom type
            if (command?.MigrationInfo?.DeleteAllCustomType ?? false)
            {
                var deleteAllCustomTypeStatements = GetDeleteAllCustomTypeStatements(command);
                if (!deleteAllCustomTypeStatements.IsNullOrEmpty())
                {
                    statements.AddRange(deleteAllCustomTypeStatements);
                }
            }

            #endregion

            #region New foreign key

            // New foreign key
            var addForeignKeyStatements = GetAddForeignKeyStatements(command);
            if (!addForeignKeyStatements.IsNullOrEmpty())
            {
                statements.AddRange(addForeignKeyStatements);
            }

            #endregion

            return statements;
        }

        #endregion

        #region Tables

        /// <summary>
        /// Get create table statements
        /// </summary>
        /// <param name="migrationCommand">Migration command</param>
        /// <returns></returns>
        protected abstract List<SixnetExecutionDatabaseStatement> GetCreateTableStatements(SixnetMigrationDatabaseCommand migrationCommand);

        /// <summary>
        /// Get delete all table statements
        /// </summary>
        /// <param name="migrationCommand"></param>
        /// <returns></returns>
        protected abstract List<SixnetExecutionDatabaseStatement> GetDeleteAllTableStatements(SixnetMigrationDatabaseCommand migrationCommand);

        /// <summary>
        /// Get rename table statements
        /// </summary>
        /// <param name="migrationCommand"></param>
        /// <returns></returns>
        protected abstract List<SixnetExecutionDatabaseStatement> GetRenameTableStatements(SixnetMigrationDatabaseCommand migrationCommand);

        /// <summary>
        /// Get delete table statements
        /// </summary>
        /// <param name="migrationCommand"></param>
        /// <returns></returns>
        protected virtual List<SixnetExecutionDatabaseStatement> GetDeleteTableStatements(SixnetMigrationDatabaseCommand migrationCommand)
        {
            if (migrationCommand?.MigrationInfo?.DeletedTables.IsNullOrEmpty() ?? true)
            {
                return new List<SixnetExecutionDatabaseStatement>(0);
            }
            var statements = new List<SixnetExecutionDatabaseStatement>();
            foreach (var tableName in migrationCommand.MigrationInfo.DeletedTables)
            {
                var deleteStatement = new SixnetExecutionDatabaseStatement()
                {
                    Script = $"DROP TABLE IF EXISTS {FormatAndWrapObjectName(tableName)};"
                };
                statements.Add(deleteStatement);

                // Log script
                LogExecutionStatement(deleteStatement);
            }
            return statements;
        }

        #endregion

        #region Procedure

        /// <summary>
        /// Get delete all procedure statements
        /// </summary>
        /// <param name="migrationCommand"></param>
        /// <returns></returns>
        protected abstract List<SixnetExecutionDatabaseStatement> GetDeleteAllProcedureStatements(SixnetMigrationDatabaseCommand migrationCommand);

        #endregion

        #region Functions

        /// <summary>
        /// Get delete all function statements
        /// </summary>
        /// <param name="migrationCommand"></param>
        /// <returns></returns>
        protected abstract List<SixnetExecutionDatabaseStatement> GetDeleteAllFunctionStatements(SixnetMigrationDatabaseCommand migrationCommand);

        #endregion

        #region Custom type

        /// <summary>
        /// Get delete all custom type statements
        /// </summary>
        /// <param name="migrationCommand"></param>
        /// <returns></returns>
        protected abstract List<SixnetExecutionDatabaseStatement> GetDeleteAllCustomTypeStatements(SixnetMigrationDatabaseCommand migrationCommand);

        #endregion

        #region Fields

        #region Get add filed statements

        protected abstract List<SixnetExecutionDatabaseStatement> GetAddFieldStatements(SixnetMigrationDatabaseCommand migrationCommand);

        #endregion

        #region Get update field statements 
        protected abstract List<SixnetExecutionDatabaseStatement> GetUpdateFieldStatements(SixnetMigrationDatabaseCommand migrationCommand);

        #endregion

        #region Get delete filed statements

        protected abstract List<SixnetExecutionDatabaseStatement> GetDeleteFieldStatements(SixnetMigrationDatabaseCommand migrationCommand);

        #endregion

        #region Get field definition

        /// <summary>
        /// Get field definition
        /// </summary>
        /// <param name="field"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        protected virtual string GetFieldDefinition(SixnetDataField field, SixnetMigrationInfo options)
        {
            return $" {GetSqlDataType(field, options)}{GetFieldIdentity(field, options)}{GetFieldNullable(field, options)}{GetSqlDefaultValue(field, options)}";
        }

        #endregion

        #region Get field nullable

        /// <summary>
        /// Get field nullable
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        protected virtual string GetFieldNullable(SixnetDataField field, SixnetMigrationInfo options)
        {
            SixnetDirectThrower.ThrowArgNullIf(field == null, nameof(field));
            var dataType = field.DataType;
            var required = field.HasDbFeature(SixnetFieldDbFeature.NotNull);
            return required || !dataType.AllowNull() || field.InRole(SixnetFieldRole.PrimaryKey) ? " NOT NULL" : " NULL";
        }

        #endregion

        #region Get field sql data type

        /// <summary>
        /// Get sql data type
        /// </summary>
        /// <param name="field">Field</param>
        /// <returns></returns>
        protected abstract string GetSqlDataType(SixnetDataField field, SixnetMigrationInfo options);

        #endregion

        #region Get field default value

        /// <summary>
        /// Get sql default value
        /// </summary>
        /// <param name="field"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        protected virtual string GetSqlDefaultValue(SixnetDataField field, SixnetMigrationInfo options)
        {
            SixnetDirectThrower.ThrowArgNullIf(field == null, nameof(field));
            var defaultValue = field.DefaultValue;
            var useDefaultValue = field.HasDbFeature(SixnetFieldDbFeature.Default);
            if (string.IsNullOrWhiteSpace(defaultValue) && useDefaultValue)
            {
                var dbType = field.DataType.GetDbType();
                DbTypeDefaultValues.TryGetValue(dbType, out defaultValue);
            }
            if (!string.IsNullOrWhiteSpace(defaultValue))
            {
                defaultValue = $" DEFAULT ({defaultValue})";
            }
            return defaultValue;
        }

        #region Get field identity

        /// <summary>
        /// Get field identity
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        protected virtual string GetFieldIdentity(SixnetDataField field, SixnetMigrationInfo options)
        {
            return string.Empty;
        }

        #endregion

        #endregion

        #endregion

        #region Foreign key

        #region Add foreign key

        /// <summary>
        /// Get add foreign key statements
        /// </summary>
        /// <param name="migrationCommand"></param>
        /// <returns></returns>
        protected abstract List<SixnetExecutionDatabaseStatement> GetAddForeignKeyStatements(SixnetMigrationDatabaseCommand migrationCommand);


        #endregion

        #region Delete foreign key

        /// <summary>
        /// Get delete foreign key statements
        /// </summary>
        /// <param name="migrationCommand"></param>
        /// <returns></returns>
        protected abstract List<SixnetExecutionDatabaseStatement> GetDeleteForeignKeyStatements(SixnetMigrationDatabaseCommand migrationCommand);

        /// <summary>
        /// Get delete foreign key statements
        /// </summary>
        /// <param name="migrationCommand"></param>
        /// <returns></returns>
        protected abstract List<SixnetExecutionDatabaseStatement> GetDeleteAllForeignKeyStatements(SixnetMigrationDatabaseCommand migrationCommand);

        #endregion

        #endregion

        #region Index

        #region Add index

        /// <summary>
        /// Get add index status
        /// </summary>
        /// <param name="migrationCommand"></param>
        /// <returns></returns>
        protected abstract List<SixnetExecutionDatabaseStatement> GetAddIndexStatements(SixnetMigrationDatabaseCommand migrationCommand);

        #endregion

        #region Delete index

        /// <summary>
        /// Get delete index statements
        /// </summary>
        /// <param name="migrationCommand"></param>
        /// <returns></returns>
        protected abstract List<SixnetExecutionDatabaseStatement> GetDeleteIndexStatements(SixnetMigrationDatabaseCommand migrationCommand);

        #endregion 

        #endregion

        #region View

        /// <summary>
        /// Get delete all view statements
        /// </summary>
        /// <param name="migrationCommand"></param>
        /// <returns></returns>
        protected abstract List<SixnetExecutionDatabaseStatement> GetDeleteAllViewStatements(SixnetMigrationDatabaseCommand migrationCommand);

        #endregion

        #endregion

        #region From target

        /// <summary>
        /// Get from targetscript
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <param name="originalQueryable">Original query</param>
        /// <param name="location">Query object location</param>
        /// <param name="applyTablePetName">Whether apply table pet name</param>
        /// <returns></returns>
        protected virtual SixnetQueryDatabaseStatement GetFromTargetStatement(SixnetDataCommandResolveContext context, ISixnetQueryable originalQueryable
            , SixnetQueryableLocation location, string tablePetName, bool applyTablePetName = true)
        {
            switch (originalQueryable.Info.FromType)
            {
                case SixnetQueryableFromType.Queryable:
                    var targetTranslationResult = ExecuteTranslation(context, originalQueryable.Info.TargetQueryable, SixnetQueryableLocation.From, true);
                    var databaseStatement = GenerateQueryStatementCore(context, targetTranslationResult, SixnetQueryableLocation.From);
                    databaseStatement.Script = $"({databaseStatement.Script}){(applyTablePetName ? $"{TablePetNameKeyword}{tablePetName}" : "")}";
                    databaseStatement.ComplexTarget = true;
                    if (!databaseStatement.OutputFields.IsNullOrEmpty())
                    {
                        databaseStatement.OutputFields = new List<ISixnetField>(1) { SixnetDataField.Create("*", originalQueryable.GetModelType(), 0, null, "*") };
                    }
                    return databaseStatement;
                case SixnetQueryableFromType.ConstantValue:

                    var constantValues = SixnetReflecter.Collections.ResolveCollection(originalQueryable.Info.TargetConstantValue as IEnumerable);
                    SixnetDirectThrower.ThrowNotSupportIf(constantValues == null, "Target value is not supported");

                    var parameterNames = new List<string>();
                    foreach (var val in constantValues)
                    {

                        var valParameterName = FormatField(context, originalQueryable, SixnetConstantField.Create(val), location
                            , SixnetFieldLocation.Output);
                        parameterNames.Add($"({valParameterName})");
                    }
                    var outValueField = SixnetDataField.Create("*", originalQueryable.GetModelType(), 0, null, "*");
                    var outTablePetName = context.GetTablePetName(originalQueryable, outValueField.ModelType, outValueField.ModelTypeIndex);
                    var constantTargetScript = $"(VALUES {string.Join(",", parameterNames)}) {outTablePetName}(VALUE)";

                    return SixnetQueryDatabaseStatement.Create(constantTargetScript, null, new List<ISixnetField>(1) { outValueField });
                default:
                    var tableNames = context.GetTableNames(originalQueryable, location);
                    var complexTarget = false;
                    string targetScript;
                    if (tableNames.Count == 1)
                    {
                        targetScript = $"{FormatAndWrapObjectName(tableNames.FirstOrDefault())}{(applyTablePetName ? $"{TablePetNameKeyword}{tablePetName}" : "")}";
                    }
                    else
                    {
                        var targetScripts = new List<string>(tableNames.Count);
                        foreach (var tableName in tableNames)
                        {
                            targetScripts.Add($"SELECT * FROM {FormatAndWrapObjectName(tableName)}");
                        }
                        targetScript = $"({string.Join(" UNION ", targetScripts)}){(applyTablePetName ? $"{TablePetNameKeyword}{tablePetName}" : "")}";
                        complexTarget = true;
                    }
                    return SixnetQueryDatabaseStatement.Create(targetScript, null, complexTarget: complexTarget);
            }
        }

        #endregion

        #region Join target

        /// <summary>
        /// Get join target script
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <param name="topQueryable">Top queryable</param>
        /// <param name="joinEntry">Join entry</param>
        /// <returns></returns>
        protected virtual SixnetQueryDatabaseStatement GetJoinTargetStatement(SixnetDataCommandResolveContext context, ISixnetQueryable topQueryable, SixnetJoinEntry joinEntry)
        {
            var joinTargetQueryable = joinEntry.Target;
            var joinTablePetName = context.GetTablePetName(topQueryable, joinTargetQueryable.GetModelType(), joinEntry.Index);
            return GetFromTargetStatement(context, joinTargetQueryable, SixnetQueryableLocation.JoinTarget, joinTablePetName);
        }

        #endregion

        #endregion

        #region Condition

        /// <summary>
        /// Translate query object
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <returns>Return a translation result</returns>
        protected virtual SixnetQueryableTranslationResult Translate(SixnetDataCommandResolveContext context)
        {
            var queryable = context?.DataCommandExecutionContext?.Command?.Queryable;
            if (queryable != null)
            {
                return ExecuteTranslation(context, queryable, SixnetQueryableLocation.Top);
            }
            return null;
        }

        /// <summary>
        /// Execute translation
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="location">Queryable location</param>
        /// <param name="useSort">Indicates whether use sort</param>
        /// <returns>Return a translation result</returns>
        protected virtual SixnetQueryableTranslationResult ExecuteTranslation(SixnetDataCommandResolveContext context, ISixnetQueryable queryable, SixnetQueryableLocation location, bool useSort = true)
        {
            if (queryable == null)
            {
                return SixnetQueryableTranslationResult.Empty;
            }
            var translationResult = SixnetQueryableTranslationResult.Create(queryable);
            switch (queryable.Info.ExecutionMode)
            {
                case SixnetQueryableExecutionMode.Regular:

                    // Init table pet name
                    context.InitQueryableTablePetName(queryable);

                    // Condition
                    translationResult = AppendCondition(context, translationResult, queryable);

                    // Sort
                    translationResult = AppendSort(context, queryable, translationResult, useSort);

                    // Combine
                    translationResult = AppandCombine(context, queryable, translationResult);

                    // Join
                    translationResult = AppendJoin(context, queryable, translationResult);

                    // Group
                    translationResult = AppendGroup(context, queryable, translationResult, location);

                    // Having
                    translationResult = AppendHaving(context, queryable, translationResult, location);

                    // Recurve
                    translationResult = AppendTree(context, queryable, translationResult, location);

                    break;
                default:
                    translationResult.AddCondition(queryable.Info.Script, AndConnector);
                    context.SetParameters(ConvertParameter(queryable.Info.ScriptParameters));
                    break;
            }
            return translationResult;
        }

        /// <summary>
        /// Append condition
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <param name="parentTranslationResult">Parent translation result</param>
        /// <param name="queryable">Query object</param>
        /// <returns></returns>
        protected virtual SixnetQueryableTranslationResult AppendCondition(SixnetDataCommandResolveContext context, SixnetQueryableTranslationResult parentTranslationResult, ISixnetQueryable queryable)
        {
            if (!queryable.Info.Conditions.IsNullOrEmpty())
            {
                foreach (var condition in queryable.Info.Conditions)
                {
                    var conditionResult = TranslateCondition(context, queryable, condition);
                    if (conditionResult != null)
                    {
                        parentTranslationResult.AddCondition(conditionResult.GetCondition(), condition.Connector.ToString().ToUpper());
                    }
                }
            }
            return parentTranslationResult;
        }

        /// <summary>
        /// Translate condition
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <param name="topQueryable">Source query</param>
        /// <param name="condition">Condition</param>
        /// <param name="tablePetName">Table pet name</param>
        /// <returns></returns>
        protected virtual SixnetQueryableTranslationResult TranslateCondition(SixnetDataCommandResolveContext context, ISixnetQueryable topQueryable, ISixnetCondition condition)
        {
            SixnetQueryableTranslationResult translationResult = null;
            if (condition == null)
            {
                return translationResult;
            }
            if (condition is SixnetCriterion criterion)
            {
                translationResult = TranslateCriterion(context, topQueryable, criterion);
            }
            if (condition is ISixnetQueryable groupQueryable && !groupQueryable.Info.Conditions.IsNullOrEmpty())
            {
                var conditionCount = groupQueryable.Info.Conditions.Count();
                if (conditionCount == 1)
                {
                    var firstCondition = groupQueryable.Info.Conditions.First();
                    if (firstCondition is SixnetCriterion firstCriterion)
                    {
                        translationResult = TranslateCriterion(context, topQueryable, firstCriterion);
                    }
                    else
                    {
                        translationResult = TranslateCondition(context, topQueryable, firstCondition);
                    }
                }
                else
                {
                    translationResult = SixnetQueryableTranslationResult.Create(topQueryable);
                    var groupCondition = new StringBuilder($"(");
                    var index = 0;
                    foreach (var groupItem in groupQueryable.Info.Conditions)
                    {
                        var itemResult = TranslateCondition(context, topQueryable, groupItem);
                        var itemCondition = itemResult.GetCondition();
                        if (!string.IsNullOrWhiteSpace(itemCondition))
                        {
                            groupCondition.Append($"{(index > 0 ? $" {groupItem.Connector.ToString().ToUpper()} " : string.Empty)}{itemCondition}");
                            index++;
                        }
                    }
                    groupCondition.Append(")");
                    translationResult.AddCondition(groupCondition.ToString(), groupQueryable.Connector.ToString().ToUpper());
                }
            }

            if (translationResult != null && condition.Negation)
            {
                translationResult.Negate(NegateCondition);
            }

            return translationResult;
        }

        /// <summary>
        /// Translate criterion
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <param name="topQueryable">Top queryable</param>
        /// <param name="criterion">Criterion</param>
        /// <returns></returns>
        protected virtual SixnetQueryableTranslationResult TranslateCriterion(SixnetDataCommandResolveContext context, ISixnetQueryable topQueryable, SixnetCriterion criterion)
        {
            var criterionTranResult = SixnetQueryableTranslationResult.Create(topQueryable);
            if (criterion == null)
            {
                return criterionTranResult;
            }

            var sqlOperator = GetOperator(criterion.Operator);
            var leftFieldString = FormatCriterionField(context, topQueryable, criterion.Left, criterion.Operator);
            var rightFieldString = FormatCriterionField(context, topQueryable, criterion.Right, criterion.Operator);
            var needParameter = OperatorNeedParameter(criterion.Operator);
            var connector = criterion.Connector.ToString().ToUpper();
            if (!needParameter)
            {
                return criterionTranResult.AddCondition($"{(string.IsNullOrWhiteSpace(leftFieldString) ? rightFieldString : leftFieldString)}{sqlOperator}", connector);
            }
            var criterionCondition = $"{leftFieldString}{sqlOperator}{rightFieldString}";
            return criterionTranResult.AddCondition(criterionCondition, connector);
        }

        /// <summary>
        /// Translate subquery
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <param name="subqueryable">Subqueryable</param>
        /// <returns></returns>
        /// <exception cref="SixnetException"></exception>
        protected virtual string TranslateSubquery(SixnetDataCommandResolveContext context, ISixnetQueryable subqueryable)
        {
            SixnetException.ThrowIf(subqueryable.Info.SelectedFields.IsNullOrEmpty(), "Subqueryable must set query fields");

            var subqueryTranslationResult = ExecuteTranslation(context, subqueryable, SixnetQueryableLocation.Subquery, true);
            var subqueryStatement = GenerateQueryStatementCore(context, subqueryTranslationResult, SixnetQueryableLocation.Subquery);
            return subqueryStatement.Script;
        }

        #endregion

        #region Combine

        /// <summary>
        /// Append combine
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <param name="topQueryable">Top queryable</param>
        /// <param name="parentTranslationResult">Parent translation result</param>
        /// <returns></returns>
        protected virtual SixnetQueryableTranslationResult AppandCombine(SixnetDataCommandResolveContext context, ISixnetQueryable topQueryable, SixnetQueryableTranslationResult parentTranslationResult)
        {
            if (topQueryable?.Info.Combines.IsNullOrEmpty() ?? true)
            {
                return parentTranslationResult;
            }
            var combineBuilder = new StringBuilder();
            foreach (var combineEntry in topQueryable.Info.Combines)
            {
                if (combineEntry?.Target == null)
                {
                    continue;
                }
                var combineQueryResult = ExecuteTranslation(context, combineEntry.Target, SixnetQueryableLocation.Combine, true);
                var combineStatement = GenerateQueryStatementCore(context, combineQueryResult, SixnetQueryableLocation.Combine);
                combineBuilder.Append($"{GetCombineOperator(combineEntry.Type)}{combineStatement.Script}");
            }
            parentTranslationResult.SetCombine(combineBuilder.ToString());
            return parentTranslationResult;
        }

        #endregion

        #region Sort

        /// <summary>
        /// Append sort
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <param name="originalQueryable">Original queryable</param>
        /// <param name="parentTranslationResult">Parent translation result</param>
        /// <param name="useSort">Indecates whether use sort</param>
        /// <returns></returns>
        protected virtual SixnetQueryableTranslationResult AppendSort(SixnetDataCommandResolveContext context, ISixnetQueryable originalQueryable, SixnetQueryableTranslationResult parentTranslationResult, bool useSort)
        {
            if (!useSort || (originalQueryable?.Info.Sorts.IsNullOrEmpty() ?? true))
            {
                return parentTranslationResult;
            }
            var sortBuilder = new StringBuilder();
            var hasGroup = !originalQueryable.Info.GroupFields.IsNullOrEmpty();
            foreach (var sortEntry in originalQueryable.Info.Sorts)
            {
                sortBuilder.Append($"{FormatSortField(context, originalQueryable, sortEntry)}{(sortEntry.Desc ? DescKeyword : AscKeyword)},");
            }
            parentTranslationResult.SetSort(sortBuilder.ToString().Trim(','), SortKeyword);
            return parentTranslationResult;
        }

        #endregion

        #region Join

        /// <summary>
        /// Append join
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <param name="topQueryable">Original query</param>
        /// <param name="parentTranslationResult">Parent translation result</param>
        /// <param name="tablePetName">Table pet name</param>
        /// <returns></returns>
        protected virtual SixnetQueryableTranslationResult AppendJoin(SixnetDataCommandResolveContext context, ISixnetQueryable topQueryable, SixnetQueryableTranslationResult parentTranslationResult)
        {
            if (topQueryable.Info.Joins.IsNullOrEmpty())
            {
                return parentTranslationResult;
            }
            var joinBuilder = new StringBuilder();
            foreach (var joinEntry in topQueryable.Info.Joins)
            {
                var joinTargetSegment = GetJoinTargetStatement(context, topQueryable, joinEntry);

                //join connection
                var joinResult = GetJoinConnection(context, topQueryable, joinEntry);

                var joinConnection = joinResult.GetJoinConnection();
                joinBuilder.Append($"{GetJoinOperator(joinEntry.Type)}{joinTargetSegment.Script}{joinConnection}");
            }

            parentTranslationResult.SetJoin(joinBuilder.ToString());
            return parentTranslationResult;
        }

        /// <summary>
        /// Get join condition
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <param name="topQueryable">Top queryable</param>
        /// <param name="joinEntry">Join entry</param>
        /// <returns>Return join condition</returns>
        protected virtual SixnetQueryableTranslationResult GetJoinConnection(SixnetDataCommandResolveContext context, ISixnetQueryable topQueryable, SixnetJoinEntry joinEntry)
        {
            if (joinEntry.Type == SixnetJoinType.CrossJoin)
            {
                return SixnetQueryableTranslationResult.Empty;
            }

            var joinConnection = joinEntry.Connection;
            var sourceEntityType = topQueryable.GetModelType();
            var targetEntityType = joinEntry.Target.GetModelType();

            SixnetException.ThrowIf(joinConnection?.None ?? true, $"Not set join connection between {sourceEntityType?.FullName} and {targetEntityType?.FullName}");

            var joinConnectionResult = SixnetQueryableTranslationResult.Create(topQueryable);
            foreach (var condition in joinConnection.Info.Conditions)
            {
                var conditionResult = TranslateCondition(context, topQueryable, condition);
                joinConnectionResult.AddCondition(conditionResult.GetCondition(), condition.Connector.ToString().ToUpper());
            }

            var joinConnectionCondition = joinConnectionResult?.GetCondition();
            if (!string.IsNullOrWhiteSpace(joinConnectionCondition))
            {
                joinConnectionResult.SetJoinConnection($" ON {joinConnectionCondition}");
            }

            return joinConnectionResult;
        }

        #endregion

        #region Tree

        /// <summary>
        /// Append tree
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <param name="originalQueryable">Original query</param>
        /// <param name="translationResult">Translation result</param>
        /// <param name="location">Query object location</param>
        /// <returns></returns>
        protected virtual SixnetQueryableTranslationResult AppendTree(SixnetDataCommandResolveContext context, ISixnetQueryable originalQueryable, SixnetQueryableTranslationResult translationResult, SixnetQueryableLocation location)
        {
            var treeInfo = originalQueryable.Info.TreeInfo;
            if (treeInfo == null)
            {
                return translationResult;
            }

            (var preScriptTableName, var preScriptTablePetName) = context.GetPreTableName();

            //field
            var dataField = SixnetDataManager.GetField(DatabaseType, originalQueryable?.GetModelType(), treeInfo.DataField);
            var parentField = SixnetDataManager.GetField(DatabaseType, originalQueryable?.GetModelType(), treeInfo.ParentField);
            var treeDataFieldString = FormatField(context, originalQueryable, dataField, SixnetQueryableLocation.PreScript
                , SixnetFieldLocation.Join, tablePetName: treeInfo.Direction == SixnetTreeMatchingDirection.Down ? preScriptTablePetName : "");
            var treeParentFieldString = FormatField(context, originalQueryable, parentField, SixnetQueryableLocation.PreScript
                , SixnetFieldLocation.Join, tablePetName: treeInfo.Direction == SixnetTreeMatchingDirection.Up ? preScriptTablePetName : "");

            // entity table name
            context.SetActivityQueryable(originalQueryable, location);
            var tablePetName = context.GetDefaultTablePetName(originalQueryable);

            //fields
            string preScript;
            IEnumerable<ISixnetField> outputFields;
            var join = translationResult.GetJoin();
            var condition = translationResult.GetCondition(ConditionStartKeyword);
            var targetStatement = GetFromTargetStatement(context, originalQueryable, location, tablePetName, false);
            if (targetStatement.ComplexTarget)
            {
                //output fields
                outputFields = targetStatement.OutputFields;
                if (outputFields.IsNullOrEmpty())
                {
                    outputFields = SixnetDataManager.GetAllQueryableFields(DatabaseType, originalQueryable.GetModelType());
                }
                var outputFieldString = FormatFieldsString(context, originalQueryable, SixnetQueryableLocation.PreScript, SixnetFieldLocation.InnerOutput, outputFields);
                var withFields = UseFieldForRecursive ? $"({FormatColumnFieldsString(context, originalQueryable, outputFields)})" : "";

                //target statement
                (var targetPreScriptTableName, var targetPreScriptTablePetName) = context.GetPreTableName();
                context.AddPreScript($"{targetPreScriptTableName}{withFields}{WithTableKeyword}{targetStatement.Script}", string.Empty, string.Empty);

                preScript =
                $"{preScriptTableName}{withFields}{WithTableKeyword}(SELECT {outputFieldString} FROM {targetPreScriptTableName}{TablePetNameKeyword}{tablePetName}{join}{condition} " +
                $"UNION ALL SELECT {outputFieldString} FROM {targetPreScriptTableName}{TablePetNameKeyword}{tablePetName} INNER JOIN {preScriptTableName}{TablePetNameKeyword}{preScriptTablePetName} " +
                $"ON {(treeInfo.Direction == SixnetTreeMatchingDirection.Up ? $"{treeDataFieldString}={treeParentFieldString}" : $"{treeParentFieldString}={treeDataFieldString}")})";
            }
            else
            {
                var fromScript = $"{targetStatement.Script}{TablePetNameKeyword}{tablePetName}";
                outputFields = SixnetDataManager.GetAllQueryableFields(DatabaseType, originalQueryable.GetModelType());
                var outputFieldString = FormatFieldsString(context, originalQueryable, SixnetQueryableLocation.PreScript, SixnetFieldLocation.InnerOutput, outputFields);
                var withFields = UseFieldForRecursive ? $"({FormatColumnFieldsString(context, originalQueryable, outputFields)})" : "";

                preScript =
                    $"{preScriptTableName}{withFields}{WithTableKeyword}(SELECT {outputFieldString} FROM {fromScript}{join}{condition} " +
                    $"UNION ALL SELECT {outputFieldString} FROM {fromScript} INNER JOIN {preScriptTableName}{TablePetNameKeyword}{preScriptTablePetName} " +
                    $"ON {(treeInfo.Direction == SixnetTreeMatchingDirection.Up ? $"{treeDataFieldString}={treeParentFieldString}" : $"{treeParentFieldString}={treeDataFieldString}")})";
            }

            translationResult.SetPreOutput($"{preScriptTableName}{TablePetNameKeyword}{tablePetName}", outputFields);
            context.AddPreScript(preScript, preScriptTableName, preScriptTablePetName, location != SixnetQueryableLocation.PreScript);
            translationResult.ClearCondition();
            translationResult.SetJoin(string.Empty);
            return translationResult;
        }

        #endregion

        #region Group

        /// <summary>
        /// Append group
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <param name="originalQueryable">Original queryable</param>
        /// <param name="translationResult">Translation result</param>
        /// <param name="location">Query object location</param>
        /// <returns></returns>
        protected virtual SixnetQueryableTranslationResult AppendGroup(SixnetDataCommandResolveContext context, ISixnetQueryable originalQueryable
            , SixnetQueryableTranslationResult translationResult, SixnetQueryableLocation location)
        {
            if (!originalQueryable.Info.GroupFields.IsNullOrEmpty())
            {
                translationResult.SetGroup($"{GroupByKeyword}{string.Join(",", originalQueryable.Info.GroupFields.Select(gf => FormatField(context, originalQueryable, SixnetDataManager.GetField(DatabaseType, originalQueryable?.GetModelType(), gf), location, SixnetFieldLocation.Criterion)))}");
            }
            return translationResult;
        }

        #endregion

        #region Having

        /// <summary>
        /// Append Having
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <param name="originalQuery">Original query</param>
        /// <param name="translationResult">Translation result</param>
        /// <param name="location">Query object location</param>
        /// <returns></returns>
        protected virtual SixnetQueryableTranslationResult AppendHaving(SixnetDataCommandResolveContext context, ISixnetQueryable originalQuery, SixnetQueryableTranslationResult translationResult, SixnetQueryableLocation location)
        {
            if (originalQuery.Info.HavingQueryable != null)
            {
                var havingResult = SixnetQueryableTranslationResult.Create(originalQuery);
                foreach (var condition in originalQuery.Info.HavingQueryable.Info.Conditions)
                {
                    var conditionResult = TranslateCondition(context, originalQuery, condition);
                    havingResult.AddCondition(conditionResult.GetCondition(), condition.Connector.ToString().ToUpper());
                }

                var havingCondition = havingResult?.GetCondition();
                if (!string.IsNullOrWhiteSpace(havingCondition))
                {
                    translationResult.SetHavingCondition($" HAVING {havingCondition}");
                }
            }
            return translationResult;
        }

        #endregion

        #region Util

        #region Operator

        /// <summary>
        /// Get sql operator by criterion operator
        /// </summary>
        /// <param name="criterionOperator">Criterion operator</param>
        /// <returns></returns>
        protected virtual string GetOperator(SixnetCriterionOperator criterionOperator)
        {
            var sqlOperator = string.Empty;
            switch (criterionOperator)
            {
                case SixnetCriterionOperator.Equal:
                    sqlOperator = EqualOperator;
                    break;
                case SixnetCriterionOperator.GreaterThan:
                    sqlOperator = GreaterThanOperator;
                    break;
                case SixnetCriterionOperator.GreaterThanOrEqual:
                    sqlOperator = GreaterThanOrEqualOperator;
                    break;
                case SixnetCriterionOperator.NotEqual:
                    sqlOperator = NotEqualOperator;
                    break;
                case SixnetCriterionOperator.LessThan:
                    sqlOperator = LessThanOperator;
                    break;
                case SixnetCriterionOperator.LessThanOrEqual:
                    sqlOperator = LessThanOrEqualOperator;
                    break;
                case SixnetCriterionOperator.In:
                    sqlOperator = InOperator;
                    break;
                case SixnetCriterionOperator.NotIn:
                    sqlOperator = NotInOperator;
                    break;
                case SixnetCriterionOperator.Like:
                case SixnetCriterionOperator.BeginLike:
                case SixnetCriterionOperator.EndLike:
                    sqlOperator = LikeOperator;
                    break;
                case SixnetCriterionOperator.NotLike:
                case SixnetCriterionOperator.NotBeginLike:
                case SixnetCriterionOperator.NotEndLike:
                    sqlOperator = NotLikeOperator;
                    break;
                case SixnetCriterionOperator.IsNull:
                    sqlOperator = IsNullOperator;
                    break;
                case SixnetCriterionOperator.NotNull:
                    sqlOperator = NotNullOperator;
                    break;
                case SixnetCriterionOperator.True:
                    sqlOperator = TrueOperator;
                    break;
                case SixnetCriterionOperator.False:
                    sqlOperator = FalseOperator;
                    break;
            }
            return sqlOperator;
        }

        /// <summary>
        /// Get join operator
        /// </summary>
        /// <param name="joinType">Join type</param>
        /// <returns></returns>
        protected virtual string GetJoinOperator(SixnetJoinType joinType)
        {
            return JoinOperatorDict[joinType];
        }

        /// <summary>
        /// Get combine operator
        /// </summary>
        /// <param name="combineType">Combine type</param>
        /// <returns>Return combine operator</returns>
        protected virtual string GetCombineOperator(SixnetCombineType combineType)
        {
            return combineType switch
            {
                SixnetCombineType.UnionAll => " UNION ALL ",
                SixnetCombineType.Union => " UNION ",
                SixnetCombineType.Except => " EXCEPT ",
                SixnetCombineType.Intersect => " INTERSECT ",
                _ => throw new InvalidOperationException($"{DatabaseType} not support {combineType}"),
            };
        }

        /// <summary>
        /// Indicates operator whether need parameter
        /// </summary>
        /// <param name="criterionOperator">Criterion operator</param>
        /// <returns></returns>
        protected virtual bool OperatorNeedParameter(SixnetCriterionOperator criterionOperator)
        {
            var needParameter = true;
            switch (criterionOperator)
            {
                case SixnetCriterionOperator.NotNull:
                case SixnetCriterionOperator.IsNull:
                    needParameter = false;
                    break;
            }
            return needParameter;
        }

        /// <summary>
        /// Get system calculation operator
        /// </summary>
        /// <param name="calculationOperator">Calculation operator</param>
        /// <returns>Return system calculation operator</returns>
        protected virtual string GetSystemCalculationOperator(SixnetCalculationOperator calculationOperator)
        {
            CalculationOperators.TryGetValue(calculationOperator, out var systemCalculationOperator);
            return systemCalculationOperator;
        }

        #endregion

        #region Get limit string

        /// <summary>
        /// Get limit string
        /// </summary>
        /// <param name="offsetNum">Offset num</param>
        /// <param name="takeNum">Take num</param>
        /// <param name="hasSort">Has sort</param>
        /// <returns></returns>
        protected abstract string GetLimitString(int offsetNum, int takeNum, bool hasSort);

        #endregion

        #region Get distinct string

        /// <summary>
        /// Get distinct string
        /// </summary>
        /// <param name="queryable"></param>
        /// <returns></returns>
        protected virtual string GetDistinctString(ISixnetQueryable queryable)
        {
            if (queryable?.Info.IsDistincted ?? false)
            {
                return DistinctKeyword;
            }
            return string.Empty;
        }

        #endregion

        #region Get default sort

        protected virtual string GetDefaultSort(SixnetDataCommandResolveContext context, SixnetQueryableTranslationResult translationResult, ISixnetQueryable originalQueryable, IEnumerable<ISixnetField> dataFields, string tablePetName)
        {
            var defaultSortField = dataFields?.Where(f => f is SixnetDataField)
                                              .OrderByDescending(f => f.InRole(SixnetFieldRole.Sequence))
                                              .ThenByDescending(f => f.InRole(SixnetFieldRole.PrimaryKey))
                                              .FirstOrDefault();
            if (defaultSortField != null)
            {
                var orderField = SixnetDataField.Create(defaultSortField.PropertyName, originalQueryable.GetModelType());
                originalQueryable.OrderBy(orderField);
                AppendSort(context, originalQueryable, translationResult, true);
            }
            return translationResult.GetSort();
        }

        #endregion

        #region Format field

        /// <summary>
        /// Format fields output string
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="tablePetName">Table pet name</param>
        /// <param name="queryable">Query</param>
        /// <param name="fields">Fields</param>
        /// <param name="ignoreFormatter">Whether ignore formatter</param>
        /// <returns></returns>
        protected virtual string FormatFieldsString(SixnetDataCommandResolveContext context, ISixnetQueryable queryable, SixnetQueryableLocation queryLocation, SixnetFieldLocation fieldLocation, IEnumerable<ISixnetField> fields)
        {
            return string.Join(",", FormatFields(context, queryable, queryLocation, fieldLocation, fields));
        }

        /// <summary>
        /// Format fields
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <param name="tablePetName">Table pet name</param>
        /// <param name="queryable">Query object</param>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        protected virtual IEnumerable<string> FormatFields(SixnetDataCommandResolveContext context, ISixnetQueryable queryable, SixnetQueryableLocation queryLocation, SixnetFieldLocation fieldLocation, IEnumerable<ISixnetField> fields)
        {
            return fields?.Select(field => FormatField(context, queryable, field, queryLocation, fieldLocation, null)) ?? Array.Empty<string>();
        }

        /// <summary>
        /// Format criterion field
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="tablePetName">Table pet name</param>
        /// <param name="queryable">Query object</param>
        /// <param name="field">Field</param>
        /// <returns></returns>
        protected virtual string FormatCriterionField(SixnetDataCommandResolveContext context, ISixnetQueryable queryable, ISixnetField field, SixnetCriterionOperator criterionOperator)
        {
            field = SixnetDataManager.GetField(context.DataCommandExecutionContext.Server.DatabaseType, queryable?.GetModelType(), field);
            return FormatField(context, queryable, field, SixnetQueryableLocation.Top, SixnetFieldLocation.Criterion, criterionOperator);
        }

        /// <summary>
        /// Format sort field name
        /// </summary>
        /// <param name="queryable">Query object</param>
        /// <param name="tablePetName">Table pet name</param>
        /// <param name="sortEntry">Sort entry</param>
        /// <returns></returns>
        protected virtual string FormatSortField(SixnetDataCommandResolveContext context, ISixnetQueryable queryable, SixnetSortEntry sortEntry)
        {
            var field = SixnetDataManager.GetField(context.DataCommandExecutionContext.Server.DatabaseType, queryable?.GetModelType(), sortEntry.Field);
            return FormatField(context, queryable, field, SixnetQueryableLocation.Top, SixnetFieldLocation.Sort);
        }

        /// <summary>
        /// Format update value field
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="command">Data command</param>
        /// <param name="value">Value</param>
        /// <returns></returns>
        protected virtual string FormatUpdateValueField(SixnetDataCommandResolveContext context, SixnetDataCommand command, dynamic value)
        {
            if (value == null)
            {
                return NullKeyword;
            }
            var valueField = value as ISixnetField;
            valueField ??= SixnetConstantField.Create(value);
            valueField = SixnetDataManager.GetField(context.DataCommandExecutionContext.Server.DatabaseType, command.GetEntityType(), valueField);
            return FormatField(context, command.Queryable, valueField, SixnetQueryableLocation.Top, SixnetFieldLocation.UpdateValue);
        }

        /// <summary>
        /// Format insert value field
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="valueField">Value field</param>
        /// <returns></returns>
        protected virtual string FormatInsertValueField(SixnetDataCommandResolveContext context, ISixnetQueryable queryable, dynamic value)
        {
            if (value == null)
            {
                return NullKeyword;
            }
            var valueField = value as ISixnetField;
            valueField ??= SixnetConstantField.Create(value);
            return FormatField(context, queryable, valueField, SixnetQueryableLocation.Top, SixnetFieldLocation.InsertValue);
        }

        /// <summary>
        /// Format field
        /// </summary>
        /// <param name="server">Database server</param>
        /// <param name="tablePetName">Table pet name</param>
        /// <param name="field">Field</param>
        /// <param name="fieldLocation">Field location</param>
        /// <param name="formatOptions">Field format options</param>
        /// <param name="ignoreFormatter">Whether ignore formatter</param>
        /// <returns>Return field conversion result</returns>
        protected virtual string FormatField(SixnetDataCommandResolveContext context, ISixnetQueryable queryable, ISixnetField field
            , SixnetQueryableLocation queryableLocation, SixnetFieldLocation fieldLocation, SixnetCriterionOperator? criterionOperator = null
            , string tablePetName = "", string formatterName = "")
        {
            if (field == null)
            {
                return string.Empty;
            }
            var formatedFieldName = "";
            var fieldName = "";
            var propertyName = field.PropertyName;
            var formatSetting = field.FormatSetting;
            // regular field
            if (field is SixnetDataField regularField)
            {
                var fieldModelType = regularField.GetModelType();
                if (fieldModelType == null
                    || fieldModelType == SixnetQueryableInfo.DefaultModelType
                    || (context.DataOptions?.IsFilterType(fieldModelType) ?? false))
                {
                    fieldModelType = queryable.GetModelType();
                }

                if (string.IsNullOrWhiteSpace(tablePetName))
                {
                    tablePetName = context.GetTablePetName(queryable, fieldModelType, regularField.ModelTypeIndex);
                }
                if (regularField.FieldName == "*")
                {
                    formatedFieldName = fieldName = regularField.FieldName;
                }
                else
                {
                    var formatObjectName = FormatObjectName(SixnetDatabaseObjectName.Create(regularField.FieldName, SixnetDatabaseObjectType.Column));
                    fieldName = formatObjectName.Name;
                    formatedFieldName = GetObjectFullName(WrapObjectName(formatObjectName));
                }
                if (!string.IsNullOrWhiteSpace(tablePetName) && fieldLocation != SixnetFieldLocation.InsertValue)
                {
                    formatedFieldName = $"{tablePetName}.{formatedFieldName}";
                }
            }
            // queryable field
            else if (field is SixnetQueryableField queryableField)
            {
                formatedFieldName = $"({TranslateSubquery(context, queryableField.Queryable)})";
            }
            // constant field
            else if (field is SixnetConstantField constantField)
            {
                if (ParameterizationField(context, queryable, fieldLocation, formatterName))
                {
                    var constantValue = constantField.Value;
                    if (criterionOperator.HasValue && NeedWrapParameter(criterionOperator.Value) && SplitWrapParameter)
                    {
                        var parameterNames = new List<string>();
                        foreach (var val in constantValue)
                        {
                            var valParameterName = context.AddParameterByKeyword(string.Empty, val);
                            parameterNames.Add(FormatParameterName(valParameterName));
                        }
                        formatedFieldName = $"({string.Join(",", parameterNames)})";
                    }
                    else
                    {
                        var parameterName = context.AddParameterByKeyword(string.Empty, FormatCriterionValue(criterionOperator, constantValue));
                        formatedFieldName = FormatParameterName(parameterName);
                    }
                }
                else
                {
                    formatedFieldName = constantField.Value == null ? $"{NullKeyword}" : $"{ToSqlLiteral(constantField.Value)}";
                }
            }
            else if (field is SixnetConditionalDataField conditionalDataField)
            {
                formatedFieldName = FormatConditionalField(context, queryable, queryableLocation, conditionalDataField, criterionOperator, tablePetName, formatterName);
            }
            SixnetDirectThrower.ThrowInvalidOperationIf(string.IsNullOrWhiteSpace(formatedFieldName), $"Invalid for {field.GetType()}");

            var hasFormat = formatSetting != null;
            if (hasFormat)
            {
                var realFormat = false;
                var formatContext = new SixnetFormatFieldContext()
                {
                    PropertyName = propertyName,
                    TablePetName = tablePetName,
                    Server = context.DataCommandExecutionContext.Server,
                    FieldLocation = fieldLocation,
                    QueryLocation = queryableLocation,
                    ResolveContext = context
                };
                do
                {
                    if (formatSetting.Parameter is ISixnetField parameterField)
                    {
                        formatSetting.Parameter = FormatField(context, queryable, parameterField, queryableLocation, SixnetFieldLocation.FormatParameter, criterionOperator, tablePetName, formatSetting.Name);
                    }
                    formatContext.FieldName = formatedFieldName;
                    formatContext.FormatSetting = formatSetting;
                    var fieldFormatter = SixnetDataManager.GetFieldFormatter(formatSetting.Name) ?? DefaultFieldFormatter;
                    var newFormatedFieldName = fieldFormatter.Format(formatContext);
                    if (!string.IsNullOrWhiteSpace(newFormatedFieldName))
                    {
                        formatedFieldName = newFormatedFieldName;
                        realFormat = true;
                    }
                    formatSetting = formatSetting.Child;
                } while (formatSetting != null);

                hasFormat = realFormat;
            }

            var fieldPetName = (queryableLocation == SixnetQueryableLocation.Top || queryableLocation == SixnetQueryableLocation.From)
                && fieldLocation == SixnetFieldLocation.Output && !string.IsNullOrWhiteSpace(propertyName)
                    ? GetObjectFullName(WrapObjectName(SixnetDatabaseObjectName.Create(propertyName, SixnetDatabaseObjectType.Column)))
                    : !string.IsNullOrWhiteSpace(fieldName)
                      ? GetObjectFullName(WrapObjectName(SixnetDatabaseObjectName.Create(fieldName, SixnetDatabaseObjectType.Column)))
                      : string.Empty;
            formatedFieldName = !string.IsNullOrWhiteSpace(fieldPetName)
                && (fieldLocation == SixnetFieldLocation.Output || fieldLocation == SixnetFieldLocation.InnerOutput)
                && (hasFormat || ((queryableLocation == SixnetQueryableLocation.Top || queryableLocation == SixnetQueryableLocation.From)
                && fieldName != propertyName))
                    ? $"{formatedFieldName}{ColumnPetNameKeyword}{fieldPetName}"
                    : formatedFieldName;

            return formatedFieldName;
        }

        protected virtual bool ParameterizationField(SixnetDataCommandResolveContext context, ISixnetQueryable currentQueryable, SixnetFieldLocation fieldLocation, string formatterName = "")
        {
            return fieldLocation == SixnetFieldLocation.Criterion
                || fieldLocation == SixnetFieldLocation.UpdateValue
                || fieldLocation == SixnetFieldLocation.InsertValue
                || fieldLocation == SixnetFieldLocation.Conditional
                || (fieldLocation == SixnetFieldLocation.FormatParameter && !string.IsNullOrWhiteSpace(formatterName))
                 && (NotParameterizationFormatterNameDict.IsNullOrEmpty()
                    || !NotParameterizationFormatterNameDict.TryGetValue(formatterName, out var notParam)
                    || !notParam);
        }

        protected virtual bool NeedWrapParameter(SixnetCriterionOperator criterionOperator)
        {
            switch (criterionOperator)
            {
                case SixnetCriterionOperator.In:
                case SixnetCriterionOperator.NotIn:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Format column fields string
        /// </summary>
        /// <param name="context"></param>
        /// <param name="queryable"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        protected virtual string FormatColumnFieldsString(SixnetDataCommandResolveContext context, ISixnetQueryable queryable, IEnumerable<ISixnetField> fields)
        {
            return fields.IsNullOrEmpty()
                ? string.Empty
                : string.Join(",", fields.Select(f => FormatAndWrapObjectName(f.GetFieldName(context.DataCommandExecutionContext.Server.DatabaseType), SixnetDatabaseObjectType.Column)));
        }

        /// <summary>
        /// Format conditional field
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        protected string FormatConditionalField(SixnetDataCommandResolveContext context, ISixnetQueryable queryable, SixnetQueryableLocation queryableLocation, SixnetConditionalDataField field
            , SixnetCriterionOperator? criterionOperator = null, string tablePetName = "", string formatterName = "")
        {
            var caseString = new StringBuilder("(CASE");
            foreach (var conditionItem in field.Conditions)
            {
                var conditionResult = TranslateCondition(context, queryable, conditionItem.Condition);
                var trueValue = FormatField(context, queryable, conditionItem.Value, queryableLocation, SixnetFieldLocation.Conditional, criterionOperator, tablePetName, formatterName);
                caseString.Append($" WHEN {conditionResult.GetCondition()} THEN {trueValue}");
            }
            caseString.Append($" ELSE {FormatField(context, queryable, field.FalseValue, queryableLocation, SixnetFieldLocation.Conditional, criterionOperator, tablePetName, formatterName)} END)");
            return caseString.ToString();
        }

        #endregion

        #region Criterion value

        /// <summary>
        /// Format criterion value
        /// </summary>
        /// <param name="criterionOperator">Criterion operator</param>
        /// <param name="value">Value</param>
        /// <returns>Return formated criterion value</returns>
        protected virtual dynamic FormatCriterionValue(SixnetCriterionOperator? criterionOperator, dynamic value)
        {
            dynamic realValue = value;
            if (criterionOperator.HasValue)
            {
                switch (criterionOperator)
                {
                    case SixnetCriterionOperator.Like:
                    case SixnetCriterionOperator.NotLike:
                        realValue = $"%{value}%";
                        break;
                    case SixnetCriterionOperator.BeginLike:
                    case SixnetCriterionOperator.NotBeginLike:
                        realValue = $"{value}%";
                        break;
                    case SixnetCriterionOperator.EndLike:
                    case SixnetCriterionOperator.NotEndLike:
                        realValue = $"%{value}";
                        break;
                }
            }
            return realValue;
        }

        #endregion

        #region Pre script

        /// <summary>
        /// Get pre script
        /// </summary>
        /// <param name="context"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        protected virtual string GetPreScript(SixnetDataCommandResolveContext context, SixnetQueryableLocation location)
        {
            if (location == SixnetQueryableLocation.Top || location == SixnetQueryableLocation.UsingSource)
            {
                return FormatPreScript(context);
            }
            return string.Empty;
        }

        /// <summary>
        /// Get pre script
        /// </summary>
        /// <returns>Return pre script</returns>
        protected virtual string FormatPreScript(SixnetDataCommandResolveContext context)
        {
            var preScripts = context.GetPreScripts();
            if (preScripts.IsNullOrEmpty())
            {
                return string.Empty;
            }
            return $"{RecursiveKeyword} {string.Join(",", preScripts)}";
        }

        #endregion

        #region Parameter

        /// <summary>
        /// Format parameter name
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <returns></returns>
        public virtual string FormatParameterName(string parameterName)
        {
            return $"{ParameterPrefix}{parameterName}";
        }

        #endregion

        #region Data source

        ///// <summary>
        ///// Check whether is independent data source
        ///// </summary>
        ///// <param name="targetQueryable">target queryable</param>
        ///// <returns></returns>
        //protected virtual bool IsIndependentDataSource(ISixnetQueryable targetQueryable)
        //{
        //    var isIndependentSource = targetQueryable.FromType != QueryableFromType.Table || IgnoreJoinConditionInRoot(targetQueryable);
        //    if (!isIndependentSource)
        //    {
        //        var entityConfig = EntityManager.GetEntityConfiguration(targetQueryable?.GetModelType());
        //        isIndependentSource = entityConfig?.IsSplitTable ?? false;
        //    }
        //    return isIndependentSource;
        //}

        ///// <summary>
        ///// Check whether Ignore join condition in root condition string
        ///// </summary>
        ///// <param name="joinTargetQueryable">Join target queryable</param>
        ///// <returns></returns>
        //protected virtual bool IgnoreJoinConditionInRoot(ISixnetQueryable joinTargetQueryable)
        //{
        //    return joinTargetQueryable.TreeInfo != null || !joinTargetQueryable.GroupFields.IsNullOrEmpty() || joinTargetQueryable.TakeCount > 0 || !joinTargetQueryable.Combines.IsNullOrEmpty();
        //}

        #endregion

        #region Framework log

        /// <summary>
        /// Log execution command
        /// </summary>
        /// <param name="statement">Exection command</param>
        protected virtual void LogExecutionStatement(SixnetExecutionDatabaseStatement statement)
        {
            SixnetFrameworkLogManager.LogDatabaseExecutionStatement(GetType(), DatabaseType, statement);
        }

        /// <summary>
        /// Log script
        /// </summary>
        /// <param name="script">Script</param>
        /// <param name="parameter">Parameter</param>
        protected virtual void LogScript(string script, object parameter)
        {
            SixnetFrameworkLogManager.LogDatabaseScript(GetType(), DatabaseType, script, parameter);
        }

        #endregion

        #region Get command type

        /// <summary>
        /// Get command type
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Return command type</returns>
        protected virtual CommandType GetCommandType(SixnetDataCommand command)
        {
            return GetCommandType(command.ScriptType);
        }

        /// <summary>
        /// Get command type
        /// </summary>
        /// <param name="scriptType">Script type</param>
        /// <returns></returns>
        public virtual CommandType GetCommandType(SixnetDataScriptType scriptType)
        {
            return scriptType switch
            {
                SixnetDataScriptType.Text => CommandType.Text,
                SixnetDataScriptType.StoredProcedure => CommandType.StoredProcedure,
                SixnetDataScriptType.TableDirect => CommandType.TableDirect,
                _ => throw new NotSupportedException(scriptType.ToString()),
            };
        }

        #endregion

        #region Convert parameter

        /// <summary>
        /// Convert parameter
        /// </summary>
        /// <param name="originalParameter">Original parameter</param>
        /// <returns>Return command parameters</returns>
        protected virtual SixnetDataCommandParameters ConvertParameter(object originalParameter)
        {
            return SixnetDataCommandParameters.Parse(originalParameter);
        }

        #endregion

        #region Format wrap join fields

        protected virtual IEnumerable<string> FormatWrapJoinPrimaryKeys(SixnetDataCommandResolveContext context, ISixnetQueryable queryable, Type entityType, string topTablePetName, string sourceTablePetName, string targetTablePetName)
        {
            var primaryKeyFields = SixnetDataManager.GetFields(DatabaseType, entityType, SixnetEntityManager.GetPrimaryKeyFields(entityType));
            SixnetException.ThrowIf(primaryKeyFields.IsNullOrEmpty(), $"{entityType?.FullName} not set primary key");
            return FormatWrapJoinFields(context, queryable, primaryKeyFields, topTablePetName, sourceTablePetName, targetTablePetName);
        }

        protected virtual IEnumerable<string> FormatWrapJoinFields(SixnetDataCommandResolveContext context, ISixnetQueryable queryable, IEnumerable<ISixnetField> fields, string topTablePetName, string sourceTablePetName, string targetTablePetName)
        {
            var joinItems = fields.Select(field =>
            {
                return $"{FormatField(context, queryable, field, SixnetQueryableLocation.Top, SixnetFieldLocation.Join, tablePetName: sourceTablePetName)} = {FormatField(context, queryable, field, SixnetQueryableLocation.Top, SixnetFieldLocation.Join, tablePetName: targetTablePetName)}";
            });
            return joinItems;
        }

        #endregion

        #region Negate condition

        /// <summary>
        /// Negate condition
        /// </summary>
        /// <param name="conditionString">Condition string</param>
        /// <returns></returns>
        protected string NegateCondition(string conditionString)
        {
            if (string.IsNullOrWhiteSpace(conditionString))
            {
                return string.Empty;
            }
            return $"{NegationKeyword} {conditionString}";
        }

        #endregion

        #region Object name

        /// <summary>
        /// Format object name
        /// </summary>
        /// <param name="objectName">Object name</param>
        /// <returns></returns>
        public SixnetDatabaseObjectName FormatObjectName(SixnetDatabaseObjectName objectName)
        {
            if (FormatObjectNameFunc == null)
            {
                return DefaultFormatObjectName(objectName);
            }
            else
            {
                return FormatObjectNameFunc(objectName);
            }
        }

        internal protected SixnetDatabaseObjectName DefaultFormatObjectName(SixnetDatabaseObjectName objectName)
        {
            return SixnetDataManager.FormatDatabaseObjectName(DatabaseType, objectName);
        }

        /// <summary>
        /// Wrap object name
        /// </summary>
        /// <param name="objectName">Object name</param>
        /// <returns></returns>
        public SixnetDatabaseObjectName WrapObjectName(SixnetDatabaseObjectName objectName)
        {
            if (WrapObjectNameFunc == null)
            {
                return DefaultWrapObjectName(objectName);
            }
            else
            {
                return WrapObjectNameFunc(objectName);
            }
        }

        internal protected SixnetDatabaseObjectName DefaultWrapObjectName(SixnetDatabaseObjectName objectName)
        {
            if (!string.IsNullOrWhiteSpace(objectName.Name))
            {
                var newObjectName = objectName.Clone();
                newObjectName.Name = $"{KeywordPrefix}{objectName.Name}{KeywordSuffix}";
                return newObjectName;
            }
            return objectName;
        }

        /// <summary>
        /// Get object full name
        /// </summary>
        /// <param name="objectName">Object name</param>
        /// <returns></returns>
        public string GetObjectFullName(SixnetDatabaseObjectName objectName)
        {
            if (GetObjectFullNameFunc == null)
            {
                return DefaultGetObjectFullName(objectName);
            }
            else
            {
                return GetObjectFullNameFunc(objectName);
            }
        }

        internal protected string DefaultGetObjectFullName(SixnetDatabaseObjectName objectName)
        {
            if (!string.IsNullOrWhiteSpace(objectName.SchemaName))
            {
                return $"{objectName.SchemaName}.{objectName.Name}";
            }
            return objectName.Name;
        }

        /// <summary>
        /// Format and wrap keyword object name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string FormatAndWrapObjectName(string name, SixnetDatabaseObjectType nameType)
        {
            return FormatAndWrapObjectName(SixnetDatabaseObjectName.Create(name, nameType));
        }

        /// <summary>
        /// Format and wrap keyword object name
        /// </summary>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public string FormatAndWrapObjectName(SixnetDatabaseObjectName objectName)
        {
            return GetObjectFullName(WrapObjectName(FormatObjectName(objectName)));
        }

        #endregion

        #region To sql literal

        protected virtual string ToSqlLiteral(object value)
        {
            if (value == null || value == DBNull.Value)
                return "NULL";

            switch (value)
            {
                case bool b:
                    return b ? "1" : "0";

                case string s:
                    return QuoteString(s);

                case char c:
                    return QuoteString(c.ToString());

                case Guid guid:
                    return $"'{guid}'";

                case DateTime dt:
                    return $"'{dt:yyyy-MM-dd HH:mm:ss.fff}'";

                case DateTimeOffset dto:
                    return $"'{dto:yyyy-MM-dd HH:mm:ss.fff zzz}'";

                case TimeSpan ts:
                    return $"'{ts}'";

                case byte[] bytes:
                    return "0x" + System.Convert.ToHexString(bytes);

                case Enum e:
                    return System.Convert.ToInt64(e)
                        .ToString(CultureInfo.InvariantCulture);

                case sbyte or
                     byte or
                     short or
                     ushort or
                     int or
                     uint or
                     long or
                     ulong or
                     float or
                     double or
                     decimal:
                    return System.Convert.ToString(value, CultureInfo.InvariantCulture)!;

                case IEnumerable enumerable:
                    return ToCollectionLiteral(enumerable);

                default:
                    throw new NotSupportedException(
                        $"Unsupported type: {value.GetType()}");
            }
        }

        protected virtual string ToCollectionLiteral(IEnumerable values)
        {
            var list = new List<string>();

            foreach (var item in values)
            {
                list.Add(ToSqlLiteral(item));
            }

            return $"({string.Join(",", list)})";
        }

        protected virtual string QuoteString(string value)
        {
            return $"N'{value.Replace("'", "''")}'";
        }

        #endregion

        #endregion

        #endregion
    }
}
