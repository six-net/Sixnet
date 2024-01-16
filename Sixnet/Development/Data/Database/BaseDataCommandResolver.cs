using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Sixnet.Development.Data.Command;
using Sixnet.Development.Data.Configuration;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Data.Field.Formatting;
using Sixnet.Development.Entity;
using Sixnet.Development.Queryable;
using Sixnet.Exceptions;
using Sixnet.Logging;
using Sixnet.Model.Paging;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Base data command resolver
    /// </summary>
    public abstract class BaseDataCommandResolver : IDataCommandResolver
    {
        #region Properties

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
        public DatabaseServerType DatabaseServerType { get; set; }
        public Dictionary<JoinType, string> JoinOperatorDict { get; set; } = new Dictionary<JoinType, string>()
        {
            { JoinType.InnerJoin," INNER JOIN " },
            { JoinType.CrossJoin," CROSS JOIN " },
            { JoinType.LeftJoin," LEFT JOIN " },
            { JoinType.RightJoin," RIGHT JOIN " },
            { JoinType.FullJoin," FULL JOIN " }
        };
        public Dictionary<CalculationOperator, string> CalculationOperators = new Dictionary<CalculationOperator, string>(4)
        {
            [CalculationOperator.Add] = "+",
            [CalculationOperator.Subtract] = "-",
            [CalculationOperator.Multiply] = "*",
            [CalculationOperator.Divide] = "/",
        };
        public IFieldFormatter DefaultFieldFormatter { get; set; }
        public int DefaultCharLength { get; set; } = 50;
        public int DefaultDecimalLength { get; set; } = 20;
        public int DefaultDecimalPrecision { get; set; } = 4;
        public Dictionary<DbType, string> DbTypeDefaultValues { get; set; }
        public Func<string, string> WrapKeywordFunc { get; set; }
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
        public virtual DatabaseQueryStatement GenerateDatabaseQueryStatement(DatabaseSingleCommand command)
        {
            //create context
            var context = new DataCommandResolveContext(command.Connection, command.DataCommand);

            //translation query
            var queryableTranResult = Translate(context);

            //generate statement
            var statement = GenerateQueryStatementCore(context, queryableTranResult, QueryableLocation.Top);
            var queryable = queryableTranResult.GetOriginalQueryable();
            statement.ScriptType = GetCommandType(queryable.ScriptType);
            return statement;
        }

        /// <summary>
        /// Generate a query statement
        /// </summary>
        /// <param name="command">Database multiple command</param>
        /// <returns></returns>
        public virtual DatabaseQueryStatement GenerateDatabaseQueryStatement(DatabaseMultipleCommand command)
        {
            ThrowHelper.ThrowArgNullIf(command?.DataCommands.IsNullOrEmpty() ?? true, "Not set any data command");

            //create context
            var context = new DataCommandResolveContext(command.Connection, null);
            var commandScriptBuilder = new StringBuilder();
            DataCommandParameters groupParameters = null;
            var commandType = CommandType.Text;
            foreach (var dataCommand in command.DataCommands)
            {
                context.SetCommand(dataCommand);

                //translation queryable
                var queryableTranResult = Translate(context);

                //generate statement
                var cmdQueryableStatement = GenerateQueryStatementCore(context, queryableTranResult, QueryableLocation.Top);

                commandScriptBuilder.AppendLine(cmdQueryableStatement.Script + ";");
                groupParameters = groupParameters == null
                    ? cmdQueryableStatement.Parameters
                    : groupParameters.Union(cmdQueryableStatement.Parameters);
                var queryable = queryableTranResult.GetOriginalQueryable();
                commandType = GetCommandType(queryable.ScriptType);
            }
            var statement = DatabaseQueryStatement.Create(commandScriptBuilder.ToString(), groupParameters);
            statement.ScriptType = commandType;
            return statement;
        }

        /// <summary>
        /// Get a paging statement
        /// </summary>
        /// <param name="command">Database single command</param>
        /// <returns></returns>
        public virtual DatabaseQueryStatement GenerateDatabaseQueryPagingStatement(DatabaseSingleCommand command)
        {
            var queryable = command?.DataCommand?.Queryable;
            //create context
            var context = new DataCommandResolveContext(command.Connection, command.DataCommand);
            //translation query
            var translationResult = Translate(context);
            string sqlStatement;
            IEnumerable<IDataField> outputFields = null;
            IEnumerable<IDataField> originalOutputFields = null;
            switch (queryable.ExecutionMode)
            {
                case QueryableExecutionMode.Script:
                    sqlStatement = translationResult.GetCondition();
                    break;
                case QueryableExecutionMode.Regular:
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
                        var targetStatement = GetFromTargetStatement(context, queryable, QueryableLocation.Top, tablePetName);
                        originalOutputFields = outputFields = targetStatement.OutputFields;
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
                    if (outputFields.IsNullOrEmpty() || !queryable.SelectedFields.IsNullOrEmpty())
                    {
                        originalOutputFields = outputFields = DataManager.GetQueryableFields(DatabaseServerType, queryable.GetModelType(), queryable, context.IsRootQueryable(queryable));
                    }
                    if (!queryable.Sorts.IsNullOrEmpty())
                    {
                        var sortFields = queryable.Sorts.Select(se => se.Field);
                        outputFields = outputFields.Union(sortFields);
                    }
                    var outputFieldString = FormatFieldsString(context, queryable, QueryableLocation.PreScript, FieldLocation.InnerOutput, outputFields);

                    //sort
                    var sort = translationResult.GetSort();
                    if (string.IsNullOrWhiteSpace(sort))
                    {
                        sort = GetDefaultSort(context, translationResult, queryable, outputFields, tablePetName);
                    }
                    var hasSort = !string.IsNullOrWhiteSpace(sort);

                    //statement
                    switch (queryable.OutputType)
                    {
                        case QueryableOutputType.Count:
                        case QueryableOutputType.Predicate:
                            throw new NotSupportedException("Not supported this output type for paging");
                        default:
                            sqlStatement = $"SELECT{GetDistinctString(queryable)} {outputFieldString} FROM {targetScript}";
                            if (hasCombine)
                            {
                                sqlStatement = $"({sqlStatement}){combine}";
                            }
                            break;
                    }
                    var pagingTotalCountFieldName = DataManager.PagingTotalCountFieldName;
                    context.AddPreScript($"{PagingTableName}{WithTableKeyword}({sqlStatement})", PagingTableName, tablePetName);
                    context.AddPreScript($"{PagingCountTableName}{WithTableKeyword}(SELECT COUNT(1){ColumnPetNameKeyword}{pagingTotalCountFieldName} FROM {PagingTableName})", PagingCountTableName, tablePetName);
                    var preScript = FormatPreScript(context);

                    //limit
                    var pagingFilter = command.DataCommand.PagingFilter;
                    var limit = GetLimitString((pagingFilter.Page - 1) * pagingFilter.PageSize, pagingFilter.PageSize, hasSort);
                    outputFieldString = FormatFieldsString(context, queryable, QueryableLocation.Top, FieldLocation.Output, originalOutputFields);
                    sqlStatement = $"{preScript}SELECT (SELECT {pagingTotalCountFieldName} FROM {PagingCountTableName}){ColumnPetNameKeyword}{pagingTotalCountFieldName},''{ColumnPetNameKeyword}{DataManager.PagingTotalCountSplitFieldName},{outputFieldString} FROM {PagingTableName}{TablePetNameKeyword}{tablePetName}{sort}{limit}";
                    break;
            }

            //parameter
            var parameters = context.GetParameters();

            //log script
            LogScript(sqlStatement, parameters);
            return DatabaseQueryStatement.Create(sqlStatement, context.GetParameters());
        }

        /// <summary>
        /// Get query statement
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <param name="translationResult">Queryable translation result</param>
        /// <param name="queryableLocation">Queryable location</param>
        /// <returns></returns>
        protected abstract DatabaseQueryStatement GenerateQueryStatementCore(DataCommandResolveContext context, QueryableTranslationResult translationResult, QueryableLocation queryableLocation);

        #endregion

        #region Execution

        /// <summary>
        /// Generate database execution statement
        /// </summary>
        /// <param name="command">Database single command</param>
        /// <returns></returns>
        public virtual List<DatabaseExecutionStatement> GenerateDatabaseExecutionStatements(DatabaseSingleCommand command)
        {
            var commandResolveContext = new DataCommandResolveContext(command.Connection, command.DataCommand);
            return GenerateDatabaseExecutionStatements(commandResolveContext);
        }

        /// <summary>
        /// Generate database statement groups
        /// </summary>
        /// <param name="command">Database execution command</param>
        /// <returns></returns>
        public virtual List<DatabaseExecutionStatement> GenerateDatabaseExecutionStatements(DatabaseMultipleCommand command)
        {
            ThrowHelper.ThrowNullOrEmptyIf(command?.DataCommands.IsNullOrEmpty() ?? true, "Data commands is null or empty");

            var statements = new List<DatabaseExecutionStatement>();
            var batchExecutionConfig = DataManager.GetBatchExecutionConfiguration(DatabaseServerType) ?? BatchExecutionConfiguration.Default;
            var groupStatementsCount = batchExecutionConfig.GroupStatementsCount;
            groupStatementsCount = groupStatementsCount < 0 ? 1 : groupStatementsCount;
            var groupParameterCount = batchExecutionConfig.GroupParametersCount;
            groupParameterCount = groupParameterCount < 0 ? 1 : groupParameterCount;
            var commandScriptBuilder = new StringBuilder();
            var incrScriptBuilder = new StringBuilder();
            var appendedStatementCount = 0;
            var mustAffectData = false;
            DataCommandParameters groupParameters = null;
            var scriptType = CommandType.Text;
            var commandResolveContext = new DataCommandResolveContext(command.Connection, null);

            //create group execution statement
            DatabaseExecutionStatement GetGroupExecutionStatement()
            {
                if (incrScriptBuilder.Length > 0)
                {
                    commandScriptBuilder.Append($"SELECT {incrScriptBuilder.ToString().Trim(',')};");
                }
                var statement = new DatabaseExecutionStatement()
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
            void AppendExecutionStatement(DatabaseExecutionStatement statement)
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
        protected virtual List<DatabaseExecutionStatement> GenerateDatabaseExecutionStatements(DataCommandResolveContext context)
        {
            var command = context.DataCommandExecutionContext.Command;
            var statements = new List<DatabaseExecutionStatement>();

            //Get script statement
            DatabaseExecutionStatement GetScriptStatement()
            {
                return new DatabaseExecutionStatement()
                {
                    Script = command.Script,
                    Parameters = ConvertParameter(command.ScriptParameters),
                    ScriptType = GetCommandType(command),
                    MustAffectData = command.Options?.MustAffectData ?? false,
                    HasPreScript = true
                };
            }

            if (command.ExecutionMode == CommandExecutionMode.Script)
            {
                statements.Add(GetScriptStatement());
            }
            else
            {
                switch (command.OperationType)
                {
                    case DataOperationType.Insert:
                        statements.AddRange(GenerateInsertStatements(context));
                        break;
                    case DataOperationType.Update:
                        statements.AddRange(GenerateUpdateStatements(context));
                        break;
                    case DataOperationType.Delete:
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
        protected abstract List<DatabaseExecutionStatement> GenerateInsertStatements(DataCommandResolveContext context);

        /// <summary>
        /// Get update statement
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <returns></returns>
        protected abstract List<DatabaseExecutionStatement> GenerateUpdateStatements(DataCommandResolveContext context);

        /// <summary>
        /// Get delete statement
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <returns></returns>
        protected abstract List<DatabaseExecutionStatement> GenerateDeleteStatements(DataCommandResolveContext context);

        #endregion

        #region Migration

        #region Generate database migration statements

        /// <summary>
        /// Generate database migration statements
        /// </summary>
        /// <param name="command">Database migration command</param>
        /// <returns></returns>
        public virtual List<DatabaseExecutionStatement> GenerateDatabaseMigrationStatements(DatabaseMigrationCommand command)
        {
            var statements = new List<DatabaseExecutionStatement>();

            // Create table
            var createTableStatements = GetCreateTableStatements(command);
            if (!createTableStatements.IsNullOrEmpty())
            {
                statements.AddRange(createTableStatements);
            }

            return statements;
        }

        #endregion

        #region Get create table statements

        /// <summary>
        /// Get create table statements
        /// </summary>
        /// <param name="migrationCommand">Migration command</param>
        /// <returns></returns>
        protected abstract List<DatabaseExecutionStatement> GetCreateTableStatements(DatabaseMigrationCommand migrationCommand);

        #endregion

        #region Get field nullable

        /// <summary>
        /// Get field nullable
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        protected virtual string GetFieldNullable(EntityField field, MigrationInfo options)
        {
            ThrowHelper.ThrowArgNullIf(field == null, nameof(field));
            var dataType = field.DataType;
            var required = field.HasDbFeature(FieldDbFeature.NotNull);
            return required || !dataType.AllowNull() ? " NOT NULL" : " NULL";
        }

        #endregion

        #region Get field sql data type

        /// <summary>
        /// Get sql data type
        /// </summary>
        /// <param name="field">Field</param>
        /// <returns></returns>
        protected abstract string GetSqlDataType(EntityField field, MigrationInfo options);

        #endregion

        #region Get field default value

        /// <summary>
        /// Get sql default value
        /// </summary>
        /// <param name="field"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        protected virtual string GetSqlDefaultValue(EntityField field, MigrationInfo options)
        {
            ThrowHelper.ThrowArgNullIf(field == null, nameof(field));
            var defaultValue = field.DefaultValue;
            var useDefaultValue = field.HasDbFeature(FieldDbFeature.Default);
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
        protected virtual DatabaseQueryStatement GetFromTargetStatement(DataCommandResolveContext context, ISixnetQueryable originalQueryable, QueryableLocation location, string tablePetName, bool applyTablePetName = true)
        {
            switch (originalQueryable.FromType)
            {
                case QueryableFromType.Queryable:
                    var targetTranslationResult = ExecuteTranslation(context, originalQueryable.TargetQueryable, QueryableLocation.From, true);
                    var databaseStatement = GenerateQueryStatementCore(context, targetTranslationResult, QueryableLocation.From);
                    databaseStatement.Script = $"({databaseStatement.Script}){(applyTablePetName ? $"{TablePetNameKeyword}{tablePetName}" : "")}";
                    databaseStatement.ComplexTarget = true;
                    return databaseStatement;
                default:
                    var tableNames = context.GetTableNames(originalQueryable, location);
                    var targetScript = "";
                    var complexTarget = false;
                    if (tableNames.Count == 1)
                    {
                        targetScript = $"{WrapKeywordFunc(tableNames.FirstOrDefault())}{(applyTablePetName ? $"{TablePetNameKeyword}{tablePetName}" : "")}";
                    }
                    else
                    {
                        var targetScripts = new List<string>(tableNames.Count);
                        foreach (var tableName in tableNames)
                        {
                            targetScripts.Add($"SELECT * FROM {WrapKeywordFunc(tableName)}");
                        }
                        targetScript = $"({string.Join(" UNION ", targetScripts)}){(applyTablePetName ? $"{TablePetNameKeyword}{tablePetName}" : "")}";
                        complexTarget = true;
                    }
                    return DatabaseQueryStatement.Create(targetScript, null, complexTarget: complexTarget);
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
        protected virtual DatabaseQueryStatement GetJoinTargetStatement(DataCommandResolveContext context, ISixnetQueryable topQueryable, JoinEntry joinEntry)
        {
            var joinTargetQueryable = joinEntry.TargetQueryable;
            var joinTablePetName = context.GetTablePetName(topQueryable, joinTargetQueryable.GetModelType(), joinEntry.Index);
            return GetFromTargetStatement(context, joinTargetQueryable, QueryableLocation.JoinTarget, joinTablePetName);
        }

        #endregion

        #endregion

        #region Condition

        /// <summary>
        /// Translate query object
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <returns>Return a translation result</returns>
        protected virtual QueryableTranslationResult Translate(DataCommandResolveContext context)
        {
            var queryable = context?.DataCommandExecutionContext?.Command?.Queryable;
            if (queryable != null)
            {
                return ExecuteTranslation(context, queryable, QueryableLocation.Top);
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
        protected virtual QueryableTranslationResult ExecuteTranslation(DataCommandResolveContext context, ISixnetQueryable queryable, QueryableLocation location, bool useSort = true)
        {
            if (queryable == null)
            {
                return QueryableTranslationResult.Empty;
            }
            var translationResult = QueryableTranslationResult.Create(queryable);
            switch (queryable.ExecutionMode)
            {
                case QueryableExecutionMode.Regular:

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
                    translationResult.AddCondition(queryable.Script, AndConnector);
                    context.SetParameters(ConvertParameter(queryable.ScriptParameters));
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
        protected virtual QueryableTranslationResult AppendCondition(DataCommandResolveContext context, QueryableTranslationResult parentTranslationResult, ISixnetQueryable queryable)
        {
            if (!queryable.Conditions.IsNullOrEmpty())
            {
                foreach (var condition in queryable.Conditions)
                {
                    var conditionResult = TranslateCondition(context, queryable, condition);
                    parentTranslationResult.AddCondition(conditionResult.GetCondition(), condition.Connector.ToString().ToUpper());
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
        protected virtual QueryableTranslationResult TranslateCondition(DataCommandResolveContext context, ISixnetQueryable topQueryable, ICondition condition)
        {
            QueryableTranslationResult translationResult = null;
            if (condition == null)
            {
                return translationResult;
            }
            if (condition is Criterion criterion)
            {
                translationResult = TranslateCriterion(context, topQueryable, criterion);
            }
            if (condition is ISixnetQueryable groupQueryable && !groupQueryable.Conditions.IsNullOrEmpty())
            {
                var conditionCount = groupQueryable.Conditions.Count();
                if (conditionCount == 1)
                {
                    var firstCondition = groupQueryable.Conditions.First();
                    if (firstCondition is Criterion firstCriterion)
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
                    translationResult = QueryableTranslationResult.Create(topQueryable);
                    var groupCondition = new StringBuilder($"(");
                    var index = 0;
                    foreach (var groupItem in groupQueryable.Conditions)
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
        protected virtual QueryableTranslationResult TranslateCriterion(DataCommandResolveContext context, ISixnetQueryable topQueryable, Criterion criterion)
        {
            var criterionTranResult = QueryableTranslationResult.Create(topQueryable);
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
        protected virtual string TranslateSubquery(DataCommandResolveContext context, ISixnetQueryable subqueryable)
        {
            SixnetException.ThrowIf(subqueryable.SelectedFields.IsNullOrEmpty(), "Subqueryable must set query fields");

            var subqueryTranslationResult = ExecuteTranslation(context, subqueryable, QueryableLocation.Subquery, true);
            var subqueryStatement = GenerateQueryStatementCore(context, subqueryTranslationResult, QueryableLocation.Subquery);
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
        protected virtual QueryableTranslationResult AppandCombine(DataCommandResolveContext context, ISixnetQueryable topQueryable, QueryableTranslationResult parentTranslationResult)
        {
            if (topQueryable?.Combines.IsNullOrEmpty() ?? true)
            {
                return parentTranslationResult;
            }
            var combineBuilder = new StringBuilder();
            foreach (var combineEntry in topQueryable.Combines)
            {
                if (combineEntry?.TargetQueryable == null)
                {
                    continue;
                }
                var combineQueryResult = ExecuteTranslation(context, combineEntry.TargetQueryable, QueryableLocation.Combine, true);
                var combineStatement = GenerateQueryStatementCore(context, combineQueryResult, QueryableLocation.Combine);
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
        protected virtual QueryableTranslationResult AppendSort(DataCommandResolveContext context, ISixnetQueryable originalQueryable, QueryableTranslationResult parentTranslationResult, bool useSort)
        {
            if (!useSort || (originalQueryable?.Sorts.IsNullOrEmpty() ?? true))
            {
                return parentTranslationResult;
            }
            var sortBuilder = new StringBuilder();
            var hasGroup = !originalQueryable.GroupFields.IsNullOrEmpty();
            foreach (var sortEntry in originalQueryable.Sorts)
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
        protected virtual QueryableTranslationResult AppendJoin(DataCommandResolveContext context, ISixnetQueryable topQueryable, QueryableTranslationResult parentTranslationResult)
        {
            if (topQueryable.Joins.IsNullOrEmpty())
            {
                return parentTranslationResult;
            }
            var joinBuilder = new StringBuilder();
            foreach (var joinEntry in topQueryable.Joins)
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
        protected virtual QueryableTranslationResult GetJoinConnection(DataCommandResolveContext context, ISixnetQueryable topQueryable, JoinEntry joinEntry)
        {
            if (joinEntry.Type == JoinType.CrossJoin)
            {
                return QueryableTranslationResult.Empty;
            }

            var joinConnection = joinEntry.Connection;
            var sourceEntityType = topQueryable.GetModelType();
            var targetEntityType = joinEntry.TargetQueryable.GetModelType();

            SixnetException.ThrowIf(joinConnection?.None ?? true, $"Not set join connection between {sourceEntityType?.FullName} and {targetEntityType?.FullName}");

            var joinConnectionResult = QueryableTranslationResult.Create(topQueryable);
            foreach (var condition in joinConnection.Conditions)
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
        protected virtual QueryableTranslationResult AppendTree(DataCommandResolveContext context, ISixnetQueryable originalQueryable, QueryableTranslationResult translationResult, QueryableLocation location)
        {
            var treeInfo = originalQueryable.TreeInfo;
            if (treeInfo == null)
            {
                return translationResult;
            }

            (var preScriptTableName, var preScriptTablePetName) = context.GetPreTableName();

            //field
            var dataField = DataManager.GetField(DatabaseServerType, originalQueryable, treeInfo.DataField);
            var parentField = DataManager.GetField(DatabaseServerType, originalQueryable, treeInfo.ParentField);
            var treeDataFieldString = FormatField(context, originalQueryable, dataField, QueryableLocation.PreScript, FieldLocation.Join, tablePetName: treeInfo.Direction == TreeMatchingDirection.Down ? preScriptTablePetName : "");
            var treeParentFieldString = FormatField(context, originalQueryable, parentField, QueryableLocation.PreScript, FieldLocation.Join, tablePetName: treeInfo.Direction == TreeMatchingDirection.Up ? preScriptTablePetName : "");

            // entity table name
            context.SetActivityQueryable(originalQueryable, location);
            var tablePetName = context.GetDefaultTablePetName(originalQueryable);

            //fields
            string preScript;
            IEnumerable<IDataField> outputFields;
            var join = translationResult.GetJoin();
            var condition = translationResult.GetCondition(ConditionStartKeyword);
            var targetStatement = GetFromTargetStatement(context, originalQueryable, location, tablePetName, false);
            if (targetStatement.ComplexTarget)
            {
                //output fields
                outputFields = targetStatement.OutputFields;
                if (!outputFields.IsNullOrEmpty())
                {
                    outputFields = DataManager.GetAllQueryableFields(DatabaseServerType, originalQueryable.GetModelType());
                }
                var outputFieldString = FormatFieldsString(context, originalQueryable, QueryableLocation.PreScript, FieldLocation.InnerOutput, outputFields);
                var withFields = UseFieldForRecursive ? $"({FormatColumnFieldsString(context, originalQueryable, outputFields)})" : "";

                //target statement
                (var targetPreScriptTableName, var targetPreScriptTablePetName) = context.GetPreTableName();
                context.AddPreScript($"{targetPreScriptTableName}{withFields}{WithTableKeyword}{targetStatement.Script}", string.Empty, string.Empty);

                preScript =
                $"{preScriptTableName}{withFields}{WithTableKeyword}(SELECT {outputFieldString} FROM {targetPreScriptTableName}{TablePetNameKeyword}{tablePetName}{join}{condition} " +
                $"UNION ALL SELECT {outputFieldString} FROM {targetPreScriptTableName}{TablePetNameKeyword}{tablePetName} INNER JOIN {preScriptTableName}{TablePetNameKeyword}{preScriptTablePetName} " +
                $"ON {(treeInfo.Direction == TreeMatchingDirection.Up ? $"{treeDataFieldString}={treeParentFieldString}" : $"{treeParentFieldString}={treeDataFieldString}")})";
            }
            else
            {
                var fromScript = $"{targetStatement.Script}{TablePetNameKeyword}{tablePetName}";
                outputFields = DataManager.GetAllQueryableFields(DatabaseServerType, originalQueryable.GetModelType());
                var outputFieldString = FormatFieldsString(context, originalQueryable, QueryableLocation.PreScript, FieldLocation.InnerOutput, outputFields);
                var withFields = UseFieldForRecursive ? $"({FormatColumnFieldsString(context, originalQueryable, outputFields)})" : "";

                preScript =
                    $"{preScriptTableName}{withFields}{WithTableKeyword}(SELECT {outputFieldString} FROM {fromScript}{join}{condition} " +
                    $"UNION ALL SELECT {outputFieldString} FROM {fromScript} INNER JOIN {preScriptTableName}{TablePetNameKeyword}{preScriptTablePetName} " +
                    $"ON {(treeInfo.Direction == TreeMatchingDirection.Up ? $"{treeDataFieldString}={treeParentFieldString}" : $"{treeParentFieldString}={treeDataFieldString}")})";
            }

            translationResult.SetPreOutput($"{preScriptTableName}{TablePetNameKeyword}{tablePetName}", outputFields);
            context.AddPreScript(preScript, preScriptTableName, preScriptTablePetName, location != QueryableLocation.PreScript);
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
        protected virtual QueryableTranslationResult AppendGroup(DataCommandResolveContext context, ISixnetQueryable originalQueryable, QueryableTranslationResult translationResult, QueryableLocation location)
        {
            if (!originalQueryable.GroupFields.IsNullOrEmpty())
            {
                translationResult.SetGroup($"{GroupByKeyword}{string.Join(",", originalQueryable.GroupFields.Select(gf => FormatField(context, originalQueryable, DataManager.GetField(DatabaseServerType, originalQueryable, gf), location, FieldLocation.Criterion)))}");
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
        protected virtual QueryableTranslationResult AppendHaving(DataCommandResolveContext context, ISixnetQueryable originalQuery, QueryableTranslationResult translationResult, QueryableLocation location)
        {
            if (originalQuery.HavingQueryable != null)
            {
                var havingResult = QueryableTranslationResult.Create(originalQuery);
                foreach (var condition in originalQuery.HavingQueryable.Conditions)
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
        protected virtual string GetOperator(CriterionOperator criterionOperator)
        {
            var sqlOperator = string.Empty;
            switch (criterionOperator)
            {
                case CriterionOperator.Equal:
                    sqlOperator = EqualOperator;
                    break;
                case CriterionOperator.GreaterThan:
                    sqlOperator = GreaterThanOperator;
                    break;
                case CriterionOperator.GreaterThanOrEqual:
                    sqlOperator = GreaterThanOrEqualOperator;
                    break;
                case CriterionOperator.NotEqual:
                    sqlOperator = NotEqualOperator;
                    break;
                case CriterionOperator.LessThan:
                    sqlOperator = LessThanOperator;
                    break;
                case CriterionOperator.LessThanOrEqual:
                    sqlOperator = LessThanOrEqualOperator;
                    break;
                case CriterionOperator.In:
                    sqlOperator = InOperator;
                    break;
                case CriterionOperator.NotIn:
                    sqlOperator = NotInOperator;
                    break;
                case CriterionOperator.Like:
                case CriterionOperator.BeginLike:
                case CriterionOperator.EndLike:
                    sqlOperator = LikeOperator;
                    break;
                case CriterionOperator.NotLike:
                case CriterionOperator.NotBeginLike:
                case CriterionOperator.NotEndLike:
                    sqlOperator = NotLikeOperator;
                    break;
                case CriterionOperator.IsNull:
                    sqlOperator = IsNullOperator;
                    break;
                case CriterionOperator.NotNull:
                    sqlOperator = NotNullOperator;
                    break;
                case CriterionOperator.True:
                    sqlOperator = TrueOperator;
                    break;
                case CriterionOperator.False:
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
        protected virtual string GetJoinOperator(JoinType joinType)
        {
            return JoinOperatorDict[joinType];
        }

        /// <summary>
        /// Get combine operator
        /// </summary>
        /// <param name="combineType">Combine type</param>
        /// <returns>Return combine operator</returns>
        protected virtual string GetCombineOperator(CombineType combineType)
        {
            return combineType switch
            {
                CombineType.UnionAll => " UNION ALL ",
                CombineType.Union => " UNION ",
                CombineType.Except => " EXCEPT ",
                CombineType.Intersect => " INTERSECT ",
                _ => throw new InvalidOperationException($"{DatabaseServerType} not support {combineType}"),
            };
        }

        /// <summary>
        /// Indicates operator whether need parameter
        /// </summary>
        /// <param name="criterionOperator">Criterion operator</param>
        /// <returns></returns>
        protected virtual bool OperatorNeedParameter(CriterionOperator criterionOperator)
        {
            var needParameter = true;
            switch (criterionOperator)
            {
                case CriterionOperator.NotNull:
                case CriterionOperator.IsNull:
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
        protected virtual string GetSystemCalculationOperator(CalculationOperator calculationOperator)
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
            if (queryable?.IsDistincted ?? false)
            {
                return DistinctKeyword;
            }
            return string.Empty;
        }

        #endregion

        #region Get default sort

        protected virtual string GetDefaultSort(DataCommandResolveContext context, QueryableTranslationResult translationResult, ISixnetQueryable originalQueryable, IEnumerable<IDataField> dataFields, string tablePetName)
        {
            var defaultSortField = dataFields?.Where(f => f is PropertyField)
                                              .OrderByDescending(f => f.InRole(FieldRole.Sequence))
                                              .ThenByDescending(f => f.InRole(FieldRole.PrimaryKey))
                                              .FirstOrDefault();
            if (defaultSortField != null)
            {
                originalQueryable.OrderBy(defaultSortField.PropertyName, true);
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
        protected virtual string FormatFieldsString(DataCommandResolveContext context, ISixnetQueryable queryable, QueryableLocation queryLocation, FieldLocation fieldLocation, IEnumerable<IDataField> fields)
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
        protected virtual IEnumerable<string> FormatFields(DataCommandResolveContext context, ISixnetQueryable queryable, QueryableLocation queryLocation, FieldLocation fieldLocation, IEnumerable<IDataField> fields)
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
        protected virtual string FormatCriterionField(DataCommandResolveContext context, ISixnetQueryable queryable, IDataField field, CriterionOperator criterionOperator)
        {
            field = DataManager.GetField(context.DataCommandExecutionContext.Server.ServerType, queryable, field);
            return FormatField(context, queryable, field, QueryableLocation.Top, FieldLocation.Criterion, criterionOperator);
        }

        /// <summary>
        /// Format sort field name
        /// </summary>
        /// <param name="queryable">Query object</param>
        /// <param name="tablePetName">Table pet name</param>
        /// <param name="sortEntry">Sort entry</param>
        /// <returns></returns>
        protected virtual string FormatSortField(DataCommandResolveContext context, ISixnetQueryable queryable, SortEntry sortEntry)
        {
            var field = DataManager.GetField(context.DataCommandExecutionContext.Server.ServerType, queryable, sortEntry.Field);
            return FormatField(context, queryable, field, QueryableLocation.Top, FieldLocation.Sort);
        }

        /// <summary>
        /// Format update value field
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="command">Data command</param>
        /// <param name="value">Value</param>
        /// <returns></returns>
        protected virtual string FormatUpdateValueField(DataCommandResolveContext context, DataCommand command, dynamic value)
        {
            if (value == null)
            {
                return NullKeyword;
            }
            var valueField = value as IDataField;
            valueField ??= ConstantField.Create(value);
            valueField = DataManager.GetField(context.DataCommandExecutionContext.Server.ServerType, command.GetEntityType(), valueField);
            return FormatField(context, command.Queryable, valueField, QueryableLocation.Top, FieldLocation.UpdateValue);
        }

        /// <summary>
        /// Format insert value field
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="valueField">Value field</param>
        /// <returns></returns>
        protected virtual string FormatInsertValueField(DataCommandResolveContext context, ISixnetQueryable queryable, dynamic value)
        {
            if (value == null)
            {
                return NullKeyword;
            }
            var valueField = value as IDataField;
            valueField ??= ConstantField.Create(value);
            return FormatField(context, queryable, valueField, QueryableLocation.Top, FieldLocation.InsertValue);
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
        protected virtual string FormatField(DataCommandResolveContext context, ISixnetQueryable queryable, IDataField field
            , QueryableLocation queryableLocation, FieldLocation fieldLocation, CriterionOperator? criterionOperator = null
            , string tablePetName = "", string formatterName = "")
        {
            if (field == null)
            {
                return string.Empty;
            }
            var formatedFieldName = "";
            var fieldName = "";
            var propertyName = field.PropertyName;
            var formatOption = field.FormatOption;
            // regular field
            if (field is PropertyField regularField)
            {
                if (string.IsNullOrWhiteSpace(tablePetName))
                {
                    tablePetName = context.GetTablePetName(queryable, regularField.ModelType, regularField.ModelTypeIndex);
                }
                fieldName = regularField.FieldName;
                formatedFieldName = $"{WrapKeywordFunc(regularField.FieldName)}";
                if (!string.IsNullOrWhiteSpace(tablePetName) && fieldLocation != FieldLocation.InsertValue)
                {
                    formatedFieldName = $"{tablePetName}.{formatedFieldName}";
                }
            }
            // queryable field
            else if (field is QueryableField queryableField)
            {
                formatedFieldName = $"({TranslateSubquery(context, queryableField.Queryable)})";
            }
            // constant field
            else if (field is ConstantField constantField)
            {
                if (ParameterizationField(fieldLocation))
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
                    formatedFieldName = constantField.Value == null ? "''" : $"'{constantField.Value.ToString()}'";
                }
            }
            ThrowHelper.ThrowInvalidOperationIf(string.IsNullOrWhiteSpace(formatedFieldName), $"Invalid for {field.GetType()}");

            var hasFormat = formatOption != null;
            if (hasFormat)
            {
                var formatContext = new FieldFormatContext()
                {
                    PropertyName = propertyName,
                    TablePetName = tablePetName,
                    Server = context.DataCommandExecutionContext.Server,
                    FieldLocation = fieldLocation,
                    QueryLocation = queryableLocation
                };
                do
                {
                    if (formatOption.Parameter is IDataField parameterField)
                    {
                        formatOption.Parameter = FormatField(context, queryable, parameterField, queryableLocation, FieldLocation.FormatParameter, criterionOperator, tablePetName, formatOption.Name);
                    }
                    formatContext.FieldName = formatedFieldName;
                    formatContext.FormatOption = formatOption;
                    var fieldFormatter = DataManager.GetFieldFormatter(formatOption.Name) ?? DefaultFieldFormatter;
                    formatedFieldName = fieldFormatter.Format(formatContext);
                    formatOption = formatOption.ChildFormatOption;

                } while (formatOption != null);
            }

            var fieldPetName = queryableLocation == QueryableLocation.Top && fieldLocation == FieldLocation.Output && !string.IsNullOrWhiteSpace(propertyName)
                    ? WrapKeywordFunc(propertyName)
                    : !string.IsNullOrWhiteSpace(fieldName)
                      ? WrapKeywordFunc(fieldName)
                      : string.Empty;
            formatedFieldName = !string.IsNullOrWhiteSpace(fieldPetName)
                && (fieldLocation == FieldLocation.Output || fieldLocation == FieldLocation.InnerOutput)
                && (hasFormat || (queryableLocation == QueryableLocation.Top && fieldName != propertyName))
                    ? $"{formatedFieldName}{ColumnPetNameKeyword}{fieldPetName}"
                    : formatedFieldName;

            return formatedFieldName;
        }

        protected virtual bool ParameterizationField(FieldLocation fieldLocation, string formatterName = "")
        {
            return fieldLocation == FieldLocation.Criterion
                || fieldLocation == FieldLocation.UpdateValue
                || fieldLocation == FieldLocation.InsertValue
                || (fieldLocation == FieldLocation.FormatParameter && !string.IsNullOrWhiteSpace(formatterName))
                 && (NotParameterizationFormatterNameDict.IsNullOrEmpty() || !NotParameterizationFormatterNameDict.TryGetValue(formatterName, out var notParam) || !notParam);
        }

        protected virtual bool NeedWrapParameter(CriterionOperator criterionOperator)
        {
            switch (criterionOperator)
            {
                case CriterionOperator.In:
                case CriterionOperator.NotIn:
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
        protected virtual string FormatColumnFieldsString(DataCommandResolveContext context, ISixnetQueryable queryable, IEnumerable<IDataField> fields)
        {
            if (fields.IsNullOrEmpty())
            {
                return string.Empty;
            }
            return string.Join(",", fields.Where(f => f is PropertyField).Select(f => WrapKeywordFunc(f.GetFieldName())));
        }

        #endregion

        #region Criterion value

        /// <summary>
        /// Format criterion value
        /// </summary>
        /// <param name="criterionOperator">Criterion operator</param>
        /// <param name="value">Value</param>
        /// <returns>Return formated criterion value</returns>
        protected virtual dynamic FormatCriterionValue(CriterionOperator? criterionOperator, dynamic value)
        {
            dynamic realValue = value;
            if (criterionOperator.HasValue)
            {
                switch (criterionOperator)
                {
                    case CriterionOperator.Like:
                    case CriterionOperator.NotLike:
                        realValue = $"%{value}%";
                        break;
                    case CriterionOperator.BeginLike:
                    case CriterionOperator.NotBeginLike:
                        realValue = $"{value}%";
                        break;
                    case CriterionOperator.EndLike:
                    case CriterionOperator.NotEndLike:
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
        protected virtual string GetPreScript(DataCommandResolveContext context, QueryableLocation location)
        {
            if (location == QueryableLocation.Top || location == QueryableLocation.UsingSource)
            {
                return FormatPreScript(context);
            }
            return string.Empty;
        }

        /// <summary>
        /// Get pre script
        /// </summary>
        /// <returns>Return pre script</returns>
        protected virtual string FormatPreScript(DataCommandResolveContext context)
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
        protected virtual void LogExecutionStatement(DatabaseExecutionStatement statement)
        {
            FrameworkLogManager.LogDatabaseExecutionStatement(GetType(), DatabaseServerType, statement);
        }

        /// <summary>
        /// Log script
        /// </summary>
        /// <param name="script">Script</param>
        /// <param name="parameter">Parameter</param>
        protected virtual void LogScript(string script, object parameter)
        {
            FrameworkLogManager.LogDatabaseScript(GetType(), DatabaseServerType, script, parameter);
        }

        #endregion

        #region Get command type

        /// <summary>
        /// Get command type
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Return command type</returns>
        protected virtual CommandType GetCommandType(DataCommand command)
        {
            return GetCommandType(command.ScriptType);
        }

        /// <summary>
        /// Get command type
        /// </summary>
        /// <param name="scriptType">Script type</param>
        /// <returns></returns>
        public virtual CommandType GetCommandType(DataScriptType scriptType)
        {
            return scriptType switch
            {
                DataScriptType.Text => CommandType.Text,
                DataScriptType.StoredProcedure => CommandType.StoredProcedure,
                DataScriptType.TableDirect => CommandType.TableDirect,
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
        protected virtual DataCommandParameters ConvertParameter(object originalParameter)
        {
            return DataCommandParameters.Parse(originalParameter);
        }

        #endregion

        #region Format wrap join fields

        protected virtual IEnumerable<string> FormatWrapJoinPrimaryKeys(DataCommandResolveContext context, ISixnetQueryable queryable, Type entityType, string topTablePetName, string sourceTablePetName, string targetTablePetName)
        {
            var primaryKeyFields = DataManager.GetFields(DatabaseServerType, entityType, EntityManager.GetPrimaryKeyFields(entityType));
            SixnetException.ThrowIf(primaryKeyFields.IsNullOrEmpty(), $"{entityType?.FullName} not set primary key");
            return FormatWrapJoinFields(context, queryable, primaryKeyFields, topTablePetName, sourceTablePetName, targetTablePetName);
        }

        protected virtual IEnumerable<string> FormatWrapJoinFields(DataCommandResolveContext context, ISixnetQueryable queryable, IEnumerable<IDataField> fields, string topTablePetName, string sourceTablePetName, string targetTablePetName)
        {
            var joinItems = fields.Select(field =>
            {
                return $"{FormatField(context, queryable, field, QueryableLocation.Top, FieldLocation.Join, tablePetName: sourceTablePetName)} = {FormatField(context, queryable, field, QueryableLocation.Top, FieldLocation.Join, tablePetName: targetTablePetName)}";
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

        #endregion

        #endregion
    }
}
