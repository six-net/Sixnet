using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sixnet.Development.Data.Command;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Data.Field.Formatting;
using Sixnet.Development.Entity;
using Sixnet.Development.Queryable;
using Sixnet.Exceptions;

namespace Sixnet.Development.Data.Database
{
    public abstract partial class BaseDataCommandResolver
    {
        #region Statements

        #region Query

        /// <summary>
        /// Generate a query statement
        /// </summary>
        /// <param name="command">Database single command</param>
        /// <returns></returns>
        public virtual async Task<QueryDatabaseStatement> GenerateDatabaseQueryStatementAsync(SingleDatabaseCommand command)
        {
            //create context
            var context = new DataCommandResolveContext(command.Connection, command.DataCommand);

            //translation query
            var queryableTranResult = await TranslateAsync(context).ConfigureAwait(false);

            //generate statement
            var statement = await GenerateQueryStatementCoreAsync(context, queryableTranResult, QueryableLocation.Top).ConfigureAwait(false);
            var queryable = queryableTranResult.GetOriginalQueryable();
            statement.ScriptType = GetCommandType(queryable.ScriptType);
            return statement;
        }

        /// <summary>
        /// Generate a query statement
        /// </summary>
        /// <param name="command">Database multiple command</param>
        /// <returns></returns>
        public virtual async Task<QueryDatabaseStatement> GenerateDatabaseQueryStatementAsync(MultipleDatabaseCommand command)
        {
            SixnetDirectThrower.ThrowArgNullIf(command?.DataCommands.IsNullOrEmpty() ?? true, "Not set any data command");

            //create context
            var context = new DataCommandResolveContext(command.Connection, null);
            var commandScriptBuilder = new StringBuilder();
            DataCommandParameters groupParameters = null;
            var commandType = CommandType.Text;
            foreach (var dataCommand in command.DataCommands)
            {
                context.SetCommand(dataCommand);

                //translation queryable
                var queryableTranResult = await TranslateAsync(context).ConfigureAwait(false);

                //generate statement
                var cmdQueryableStatement = await GenerateQueryStatementCoreAsync(context, queryableTranResult, QueryableLocation.Top).ConfigureAwait(false);

                commandScriptBuilder.AppendLine(cmdQueryableStatement.Script + ";");
                groupParameters = groupParameters == null
                    ? cmdQueryableStatement.Parameters
                    : groupParameters.Union(cmdQueryableStatement.Parameters);
                var queryable = queryableTranResult.GetOriginalQueryable();
                commandType = GetCommandType(queryable.ScriptType);
            }
            var statement = QueryDatabaseStatement.Create(commandScriptBuilder.ToString(), groupParameters);
            statement.ScriptType = commandType;
            return statement;
        }

        /// <summary>
        /// Get a paging statement
        /// </summary>
        /// <param name="command">Database single command</param>
        /// <returns></returns>
        public virtual async Task<QueryDatabaseStatement> GenerateDatabaseQueryPagingStatementAsync(SingleDatabaseCommand command)
        {
            var queryable = command?.DataCommand?.Queryable;
            //create context
            var context = new DataCommandResolveContext(command.Connection, command.DataCommand);
            //translation query
            var translationResult = await TranslateAsync(context).ConfigureAwait(false);
            string sqlStatement;
            IEnumerable<ISixnetField> outputFields = null;
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
                        var targetStatement = await GetFromTargetStatementAsync(context, queryable, QueryableLocation.Top, tablePetName).ConfigureAwait(false);
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
                    if (outputFields.IsNullOrEmpty() || !queryable.SelectedFields.IsNullOrEmpty())
                    {
                        outputFields = SixnetDataManager.GetQueryableFields(DatabaseType, queryable.GetModelType(), queryable, context.IsRootQueryable(queryable));
                    }
                    var outputFieldString = await FormatFieldsStringAsync(context, queryable, QueryableLocation.Top, FieldLocation.Output, outputFields).ConfigureAwait(false);

                    //sort
                    var sort = translationResult.GetSort();
                    if (string.IsNullOrWhiteSpace(sort))
                    {
                        sort = await GetDefaultSortAsync(context, translationResult, queryable, outputFields, tablePetName).ConfigureAwait(false);
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
            return QueryDatabaseStatement.Create(sqlStatement, context.GetParameters());
        }

        /// <summary>
        /// Get query statement
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <param name="translationResult">Queryable translation result</param>
        /// <param name="queryableLocation">Queryable location</param>
        /// <returns></returns>
        protected abstract Task<QueryDatabaseStatement> GenerateQueryStatementCoreAsync(DataCommandResolveContext context, QueryableTranslationResult translationResult, QueryableLocation queryableLocation);

        #endregion

        #region Execution

        /// <summary>
        /// Generate database execution statement
        /// </summary>
        /// <param name="command">Database single command</param>
        /// <returns></returns>
        public virtual async Task<List<ExecutionDatabaseStatement>> GenerateDatabaseExecutionStatementsAsync(SingleDatabaseCommand command)
        {
            var commandResolveContext = new DataCommandResolveContext(command.Connection, command.DataCommand);
            return await GenerateDatabaseExecutionStatementsAsync(commandResolveContext).ConfigureAwait(false);
        }

        /// <summary>
        /// Generate database statement groups
        /// </summary>
        /// <param name="command">Database execution command</param>
        /// <returns></returns>
        public virtual async Task<List<ExecutionDatabaseStatement>> GenerateDatabaseExecutionStatementsAsync(MultipleDatabaseCommand command)
        {
            SixnetDirectThrower.ThrowArgNullIf(command?.DataCommands.IsNullOrEmpty() ?? true, "Data commands is null or empty");

            var statements = new List<ExecutionDatabaseStatement>();
            var batchExecutionConfig = SixnetDataManager.GetBatchSetting(DatabaseType)
                ?? DatabaseBatchSetting.Default;
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
            ExecutionDatabaseStatement GetGroupExecutionStatement()
            {
                if (incrScriptBuilder.Length > 0)
                {
                    commandScriptBuilder.Append($"SELECT {incrScriptBuilder.ToString().Trim(',')};");
                }
                var statement = new ExecutionDatabaseStatement()
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
            void AppendExecutionStatement(ExecutionDatabaseStatement statement)
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
                var executionStatements = await GenerateDatabaseExecutionStatementsAsync(commandResolveContext).ConfigureAwait(false);
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
        protected virtual async Task<List<ExecutionDatabaseStatement>> GenerateDatabaseExecutionStatementsAsync(DataCommandResolveContext context)
        {
            var command = context.DataCommandExecutionContext.Command;
            var statements = new List<ExecutionDatabaseStatement>();

            //Get script statement
            ExecutionDatabaseStatement GetScriptStatement()
            {
                return new ExecutionDatabaseStatement()
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
                        if (!(command?.FieldsAssignment?.NewValues?.IsNullOrEmpty() ?? true))
                        {
                            statements.AddRange(await GenerateInsertStatementsAsync(context).ConfigureAwait(false));
                        }
                        break;
                    case DataOperationType.Update:
                        if (!(command?.FieldsAssignment?.NewValues?.IsNullOrEmpty() ?? true))
                        {
                            statements.AddRange(await GenerateUpdateStatementsAsync(context).ConfigureAwait(false));
                        }
                        break;
                    case DataOperationType.Delete:
                        statements.AddRange(await GenerateDeleteStatementsAsync(context).ConfigureAwait(false));
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
        protected abstract Task<List<ExecutionDatabaseStatement>> GenerateInsertStatementsAsync(DataCommandResolveContext context);

        /// <summary>
        /// Get update statement
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <returns></returns>
        protected abstract Task<List<ExecutionDatabaseStatement>> GenerateUpdateStatementsAsync(DataCommandResolveContext context);

        /// <summary>
        /// Get delete statement
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <returns></returns>
        protected abstract Task<List<ExecutionDatabaseStatement>> GenerateDeleteStatementsAsync(DataCommandResolveContext context);

        #endregion

        #region Migration

        #region Generate database migration statements

        /// <summary>
        /// Generate database migration statements
        /// </summary>
        /// <param name="command">Database migration command</param>
        /// <returns></returns>
        public virtual async Task<List<ExecutionDatabaseStatement>> GenerateDatabaseMigrationStatementsAsync(MigrationDatabaseCommand command)
        {
            var statements = new List<ExecutionDatabaseStatement>();

            // Create table
            var createTableStatements = await GetCreateTableStatementsAsync(command).ConfigureAwait(false);
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
        protected abstract Task<List<ExecutionDatabaseStatement>> GetCreateTableStatementsAsync(MigrationDatabaseCommand migrationCommand);

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
        protected virtual async Task<QueryDatabaseStatement> GetFromTargetStatementAsync(DataCommandResolveContext context, ISixnetQueryable originalQueryable
            , QueryableLocation location, string tablePetName, bool applyTablePetName = true)
        {
            switch (originalQueryable.FromType)
            {
                case QueryableFromType.Queryable:
                    var targetTranslationResult = await ExecuteTranslationAsync(context, originalQueryable.TargetQueryable, QueryableLocation.From, true).ConfigureAwait(false);
                    var databaseStatement = await GenerateQueryStatementCoreAsync(context, targetTranslationResult, QueryableLocation.From).ConfigureAwait(false);
                    databaseStatement.Script = $"({databaseStatement.Script}){(applyTablePetName ? $"{TablePetNameKeyword}{tablePetName}" : "")}";
                    databaseStatement.ComplexTarget = true;
                    return databaseStatement;
                default:
                    var tableNames = await context.GetTableNamesAsync(originalQueryable, location).ConfigureAwait(false);
                    var targetScript = "";
                    var complexTarget = false;
                    if (tableNames.Count == 1)
                    {
                        targetScript = $"{FormatAndWrapKeywordFunc(tableNames.FirstOrDefault())}{(applyTablePetName ? $"{TablePetNameKeyword}{tablePetName}" : "")}";
                    }
                    else
                    {
                        var targetScripts = new List<string>(tableNames.Count);
                        foreach (var tableName in tableNames)
                        {
                            targetScripts.Add($"SELECT * FROM {FormatAndWrapKeywordFunc(tableName)}");
                        }
                        targetScript = $"({string.Join(" UNION ", targetScripts)}){(applyTablePetName ? $"{TablePetNameKeyword}{tablePetName}" : "")}";
                        complexTarget = true;
                    }
                    return QueryDatabaseStatement.Create(targetScript, null, complexTarget: complexTarget);
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
        protected virtual async Task<QueryDatabaseStatement> GetJoinTargetStatementAsync(DataCommandResolveContext context, ISixnetQueryable topQueryable, JoinEntry joinEntry)
        {
            var joinTargetQueryable = joinEntry.Target;
            var joinTablePetName = context.GetTablePetName(topQueryable, joinTargetQueryable.GetModelType(), joinEntry.Index);
            return await GetFromTargetStatementAsync(context, joinTargetQueryable, QueryableLocation.JoinTarget, joinTablePetName).ConfigureAwait(false);
        }

        #endregion

        #endregion

        #region Condition

        /// <summary>
        /// Translate query object
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <returns>Return a translation result</returns>
        protected virtual async Task<QueryableTranslationResult> TranslateAsync(DataCommandResolveContext context)
        {
            var queryable = context?.DataCommandExecutionContext?.Command?.Queryable;
            if (queryable != null)
            {
                return await ExecuteTranslationAsync(context, queryable, QueryableLocation.Top).ConfigureAwait(false);
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
        protected virtual async Task<QueryableTranslationResult> ExecuteTranslationAsync(DataCommandResolveContext context, ISixnetQueryable queryable, QueryableLocation location, bool useSort = true)
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
                    translationResult = await AppendConditionAsync(context, translationResult, queryable).ConfigureAwait(false);

                    // Sort
                    translationResult = await AppendSortAsync(context, queryable, translationResult, useSort).ConfigureAwait(false);

                    // Combine
                    translationResult = await AppandCombineAsync(context, queryable, translationResult).ConfigureAwait(false);

                    // Join
                    translationResult = await AppendJoinAsync(context, queryable, translationResult).ConfigureAwait(false);

                    // Group
                    translationResult = await AppendGroupAsync(context, queryable, translationResult, location).ConfigureAwait(false);

                    // Having
                    translationResult = await AppendHavingAsync(context, queryable, translationResult, location).ConfigureAwait(false);

                    // Recurve
                    translationResult = await AppendTreeAsync(context, queryable, translationResult, location).ConfigureAwait(false);

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
        protected virtual async Task<QueryableTranslationResult> AppendConditionAsync(DataCommandResolveContext context, QueryableTranslationResult parentTranslationResult, ISixnetQueryable queryable)
        {
            if (!queryable.Conditions.IsNullOrEmpty())
            {
                foreach (var condition in queryable.Conditions)
                {
                    var conditionResult = await TranslateConditionAsync(context, queryable, condition).ConfigureAwait(false);
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
        /// <returns></returns>
        protected virtual async Task<QueryableTranslationResult> TranslateConditionAsync(DataCommandResolveContext context, ISixnetQueryable topQueryable, ISixnetCondition condition)
        {
            QueryableTranslationResult translationResult = null;
            if (condition == null)
            {
                return translationResult;
            }
            if (condition is Criterion criterion)
            {
                translationResult = await TranslateCriterionAsync(context, topQueryable, criterion).ConfigureAwait(false);
            }
            if (condition is ISixnetQueryable groupQueryable && !groupQueryable.Conditions.IsNullOrEmpty())
            {
                var conditionCount = groupQueryable.Conditions.Count();
                if (conditionCount == 1)
                {
                    var firstCondition = groupQueryable.Conditions.First();
                    if (firstCondition is Criterion firstCriterion)
                    {
                        translationResult = await TranslateCriterionAsync(context, topQueryable, firstCriterion).ConfigureAwait(false);
                    }
                    else
                    {
                        translationResult = await TranslateConditionAsync(context, topQueryable, firstCondition).ConfigureAwait(false);
                    }
                }
                else
                {
                    translationResult = QueryableTranslationResult.Create(topQueryable);
                    var groupCondition = new StringBuilder($"(");
                    var index = 0;
                    foreach (var groupItem in groupQueryable.Conditions)
                    {
                        var itemResult = await TranslateConditionAsync(context, topQueryable, groupItem).ConfigureAwait(false);
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
        protected virtual async Task<QueryableTranslationResult> TranslateCriterionAsync(DataCommandResolveContext context, ISixnetQueryable topQueryable, Criterion criterion)
        {
            var criterionTranResult = QueryableTranslationResult.Create(topQueryable);
            if (criterion == null)
            {
                return criterionTranResult;
            }

            var sqlOperator = GetOperator(criterion.Operator);
            var leftFieldString = await FormatCriterionFieldAsync(context, topQueryable, criterion.Left, criterion.Operator).ConfigureAwait(false);
            var rightFieldString = await FormatCriterionFieldAsync(context, topQueryable, criterion.Right, criterion.Operator).ConfigureAwait(false);
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
        protected virtual async Task<string> TranslateSubqueryAsync(DataCommandResolveContext context, ISixnetQueryable subqueryable)
        {
            SixnetException.ThrowIf(subqueryable.SelectedFields.IsNullOrEmpty(), "Subqueryable must set query fields");

            var subqueryTranslationResult = await ExecuteTranslationAsync(context, subqueryable, QueryableLocation.Subquery, true).ConfigureAwait(false);
            var subqueryStatement = await GenerateQueryStatementCoreAsync(context, subqueryTranslationResult, QueryableLocation.Subquery).ConfigureAwait(false);
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
        protected virtual async Task<QueryableTranslationResult> AppandCombineAsync(DataCommandResolveContext context, ISixnetQueryable topQueryable, QueryableTranslationResult parentTranslationResult)
        {
            if (topQueryable?.Combines.IsNullOrEmpty() ?? true)
            {
                return parentTranslationResult;
            }
            var combineBuilder = new StringBuilder();
            foreach (var combineEntry in topQueryable.Combines)
            {
                if (combineEntry?.Target == null)
                {
                    continue;
                }
                var combineQueryResult = await ExecuteTranslationAsync(context, combineEntry.Target, QueryableLocation.Combine, true).ConfigureAwait(false);
                var combineStatement = await GenerateQueryStatementCoreAsync(context, combineQueryResult, QueryableLocation.Combine).ConfigureAwait(false);
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
        protected virtual async Task<QueryableTranslationResult> AppendSortAsync(DataCommandResolveContext context, ISixnetQueryable originalQueryable, QueryableTranslationResult parentTranslationResult, bool useSort)
        {
            if (!useSort || (originalQueryable?.Sorts.IsNullOrEmpty() ?? true))
            {
                return parentTranslationResult;
            }
            var sortBuilder = new StringBuilder();
            var hasGroup = !originalQueryable.GroupFields.IsNullOrEmpty();
            foreach (var sortEntry in originalQueryable.Sorts)
            {
                sortBuilder.Append($"{await FormatSortFieldAsync(context, originalQueryable, sortEntry).ConfigureAwait(false)}{(sortEntry.Desc ? DescKeyword : AscKeyword)},");
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
        protected virtual async Task<QueryableTranslationResult> AppendJoinAsync(DataCommandResolveContext context, ISixnetQueryable topQueryable, QueryableTranslationResult parentTranslationResult)
        {
            if (topQueryable.Joins.IsNullOrEmpty())
            {
                return parentTranslationResult;
            }
            var joinBuilder = new StringBuilder();
            foreach (var joinEntry in topQueryable.Joins)
            {
                var joinTargetSegment = await GetJoinTargetStatementAsync(context, topQueryable, joinEntry).ConfigureAwait(false);

                //join connection
                var joinResult = await GetJoinConnectionAsync(context, topQueryable, joinEntry).ConfigureAwait(false);

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
        protected virtual async Task<QueryableTranslationResult> GetJoinConnectionAsync(DataCommandResolveContext context, ISixnetQueryable topQueryable, JoinEntry joinEntry)
        {
            if (joinEntry.Type == JoinType.CrossJoin)
            {
                return QueryableTranslationResult.Empty;
            }

            var joinConnection = joinEntry.Connection;
            var sourceEntityType = topQueryable.GetModelType();
            var targetEntityType = joinEntry.Target.GetModelType();

            SixnetException.ThrowIf(joinConnection?.None ?? true, $"Not set join connection between {sourceEntityType?.FullName} and {targetEntityType?.FullName}");

            var joinConnectionResult = QueryableTranslationResult.Create(topQueryable);
            foreach (var condition in joinConnection.Conditions)
            {
                var conditionResult = await TranslateConditionAsync(context, topQueryable, condition).ConfigureAwait(false);
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

        #region Group

        /// <summary>
        /// Append group
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <param name="originalQueryable">Original queryable</param>
        /// <param name="translationResult">Translation result</param>
        /// <param name="location">Query object location</param>
        /// <returns></returns>
        protected virtual async Task<QueryableTranslationResult> AppendGroupAsync(DataCommandResolveContext context, ISixnetQueryable originalQueryable
            , QueryableTranslationResult translationResult, QueryableLocation location)
        {
            if (!originalQueryable.GroupFields.IsNullOrEmpty())
            {
                var groupFormatedFields = new List<string>();
                foreach (var groupField in originalQueryable.GroupFields)
                {
                    groupFormatedFields.Add(await FormatFieldAsync(context, originalQueryable, SixnetDataManager.GetField(DatabaseType, originalQueryable?.GetModelType(), groupField), location, FieldLocation.Criterion).ConfigureAwait(false));
                }
                translationResult.SetGroup($"{GroupByKeyword}{string.Join(",", groupFormatedFields)}");
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
        protected virtual async Task<QueryableTranslationResult> AppendHavingAsync(DataCommandResolveContext context, ISixnetQueryable originalQuery, QueryableTranslationResult translationResult, QueryableLocation location)
        {
            if (originalQuery.HavingQueryable != null)
            {
                var havingResult = QueryableTranslationResult.Create(originalQuery);
                foreach (var condition in originalQuery.HavingQueryable.Conditions)
                {
                    var conditionResult = await TranslateConditionAsync(context, originalQuery, condition).ConfigureAwait(false);
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

        #region Tree

        /// <summary>
        /// Append tree
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <param name="originalQueryable">Original query</param>
        /// <param name="translationResult">Translation result</param>
        /// <param name="location">Query object location</param>
        /// <returns></returns>
        protected virtual async Task<QueryableTranslationResult> AppendTreeAsync(DataCommandResolveContext context, ISixnetQueryable originalQueryable, QueryableTranslationResult translationResult, QueryableLocation location)
        {
            var treeInfo = originalQueryable.TreeInfo;
            if (treeInfo == null)
            {
                return translationResult;
            }

            (var preScriptTableName, var preScriptTablePetName) = context.GetPreTableName();

            //field
            var dataField = SixnetDataManager.GetField(DatabaseType, originalQueryable?.GetModelType(), treeInfo.DataField);
            var parentField = SixnetDataManager.GetField(DatabaseType, originalQueryable?.GetModelType(), treeInfo.ParentField);
            var treeDataFieldString = await FormatFieldAsync(context, originalQueryable, dataField, QueryableLocation.PreScript
                , FieldLocation.Join, tablePetName: treeInfo.Direction == TreeMatchingDirection.Down ? preScriptTablePetName : "")
                .ConfigureAwait(false);
            var treeParentFieldString = await FormatFieldAsync(context, originalQueryable, parentField, QueryableLocation.PreScript
                , FieldLocation.Join, tablePetName: treeInfo.Direction == TreeMatchingDirection.Up ? preScriptTablePetName : "")
                .ConfigureAwait(false);

            // entity table name
            context.SetActivityQueryable(originalQueryable, location);
            var tablePetName = context.GetDefaultTablePetName(originalQueryable);

            //fields
            string preScript;
            IEnumerable<ISixnetField> outputFields;
            var join = translationResult.GetJoin();
            var condition = translationResult.GetCondition(ConditionStartKeyword);
            var targetStatement = await GetFromTargetStatementAsync(context, originalQueryable, location, tablePetName, false).ConfigureAwait(false);
            if (targetStatement.ComplexTarget)
            {
                //output fields
                outputFields = targetStatement.OutputFields;
                if (outputFields.IsNullOrEmpty())
                {
                    outputFields = SixnetDataManager.GetAllQueryableFields(DatabaseType, originalQueryable.GetModelType());
                }
                var outputFieldString = await FormatFieldsStringAsync(context, originalQueryable, QueryableLocation.PreScript, FieldLocation.InnerOutput, outputFields).ConfigureAwait(false);
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
                outputFields = SixnetDataManager.GetAllQueryableFields(DatabaseType, originalQueryable.GetModelType());
                var outputFieldString = await FormatFieldsStringAsync(context, originalQueryable, QueryableLocation.PreScript, FieldLocation.InnerOutput, outputFields).ConfigureAwait(false);
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

        #region Util

        #region Get default sort

        protected virtual async Task<string> GetDefaultSortAsync(DataCommandResolveContext context, QueryableTranslationResult translationResult, ISixnetQueryable originalQueryable, IEnumerable<ISixnetField> dataFields, string tablePetName)
        {
            var defaultSortField = dataFields?.Where(f => f is DataField)
                                              .OrderByDescending(f => f.InRole(FieldRole.Sequence))
                                              .ThenByDescending(f => f.InRole(FieldRole.PrimaryKey))
                                              .FirstOrDefault();
            if (defaultSortField != null)
            {
                var orderField = DataField.Create(defaultSortField.PropertyName, originalQueryable.GetModelType());
                originalQueryable.OrderBy(orderField);
                await AppendSortAsync(context, originalQueryable, translationResult, true).ConfigureAwait(false);
            }
            return translationResult.GetSort();
        }

        #endregion

        #region Format field

        /// <summary>
        /// Format fields output string
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="queryable">Query</param>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        protected virtual async Task<string> FormatFieldsStringAsync(DataCommandResolveContext context, ISixnetQueryable queryable, QueryableLocation queryLocation, FieldLocation fieldLocation, IEnumerable<ISixnetField> fields)
        {
            return string.Join(",", await FormatFieldsAsync(context, queryable, queryLocation, fieldLocation, fields));
        }

        /// <summary>
        /// Format fields
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <param name="queryable">Query object</param>
        /// <param name="fields">Fields</param>
        /// <returns></returns>
        protected virtual async Task<IEnumerable<string>> FormatFieldsAsync(DataCommandResolveContext context, ISixnetQueryable queryable, QueryableLocation queryLocation, FieldLocation fieldLocation, IEnumerable<ISixnetField> fields)
        {
            var formatedFields = new List<string>();
            if (!fields.IsNullOrEmpty())
            {
                foreach (var field in fields)
                {
                    formatedFields.Add(await FormatFieldAsync(context, queryable, field, queryLocation, fieldLocation, null).ConfigureAwait(false));
                }
            }
            return formatedFields;
        }

        /// <summary>
        /// Format sort field name
        /// </summary>
        /// <param name="queryable">Query object</param>
        /// <param name="sortEntry">Sort entry</param>
        /// <returns></returns>
        protected virtual async Task<string> FormatSortFieldAsync(DataCommandResolveContext context, ISixnetQueryable queryable, SortEntry sortEntry)
        {
            var field = SixnetDataManager.GetField(context.DataCommandExecutionContext.Server.DatabaseType, queryable?.GetModelType(), sortEntry.Field);
            return await FormatFieldAsync(context, queryable, field, QueryableLocation.Top, FieldLocation.Sort).ConfigureAwait(false);
        }

        /// <summary>
        /// Format criterion field
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="tablePetName">Table pet name</param>
        /// <param name="queryable">Query object</param>
        /// <param name="field">Field</param>
        /// <returns></returns>
        protected virtual async Task<string> FormatCriterionFieldAsync(DataCommandResolveContext context, ISixnetQueryable queryable, ISixnetField field, CriterionOperator criterionOperator)
        {
            field = SixnetDataManager.GetField(context.DataCommandExecutionContext.Server.DatabaseType, queryable?.GetModelType(), field);
            return await FormatFieldAsync(context, queryable, field, QueryableLocation.Top, FieldLocation.Criterion, criterionOperator).ConfigureAwait(false);
        }

        /// <summary>
        /// Format update value field
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="command">Data command</param>
        /// <param name="value">Value</param>
        /// <returns></returns>
        protected virtual async Task<string> FormatUpdateValueFieldAsync(DataCommandResolveContext context, SixnetDataCommand command, dynamic value)
        {
            if (value == null)
            {
                return NullKeyword;
            }
            var valueField = value as ISixnetField;
            valueField ??= ConstantField.Create(value);
            valueField = SixnetDataManager.GetField(context.DataCommandExecutionContext.Server.DatabaseType, command.GetEntityType(), valueField);
            return await FormatFieldAsync(context, command.Queryable, valueField, QueryableLocation.Top, FieldLocation.UpdateValue).ConfigureAwait(false);
        }

        /// <summary>
        /// Format insert value field
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="valueField">Value field</param>
        /// <returns></returns>
        protected virtual async Task<string> FormatInsertValueFieldAsync(DataCommandResolveContext context, ISixnetQueryable queryable, dynamic value)
        {
            if (value == null)
            {
                return NullKeyword;
            }
            var valueField = value as ISixnetField;
            valueField ??= ConstantField.Create(value);
            return await FormatFieldAsync(context, queryable, valueField, QueryableLocation.Top, FieldLocation.InsertValue).ConfigureAwait(false);
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
        protected virtual async Task<string> FormatFieldAsync(DataCommandResolveContext context, ISixnetQueryable queryable, ISixnetField field
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
            var formatSetting = field.FormatSetting;
            // regular field
            if (field is DataField regularField)
            {
                var fieldModelType = regularField.GetModelType();
                if (fieldModelType == null
                    || fieldModelType == QueryableContext.DefaultModelType
                    || (context.DataOptions?.IsFilterType(fieldModelType) ?? false))
                {
                    fieldModelType = queryable.GetModelType();
                }

                if (string.IsNullOrWhiteSpace(tablePetName))
                {
                    tablePetName = context.GetTablePetName(queryable, fieldModelType, regularField.ModelTypeIndex);
                }
                fieldName = FormatKeywordFunc(regularField.FieldName);
                formatedFieldName = WrapKeywordFunc(fieldName);
                if (!string.IsNullOrWhiteSpace(tablePetName) && fieldLocation != FieldLocation.InsertValue)
                {
                    formatedFieldName = $"{tablePetName}.{formatedFieldName}";
                }
            }
            // queryable field
            else if (field is QueryableField queryableField)
            {
                formatedFieldName = $"({await TranslateSubqueryAsync(context, queryableField.Queryable).ConfigureAwait(false)})";
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
            SixnetDirectThrower.ThrowInvalidOperationIf(string.IsNullOrWhiteSpace(formatedFieldName), $"Invalid for {field.GetType()}");

            var hasFormat = formatSetting != null;
            if (hasFormat)
            {
                var formatContext = new FormatFieldContext()
                {
                    PropertyName = propertyName,
                    TablePetName = tablePetName,
                    Server = context.DataCommandExecutionContext.Server,
                    FieldLocation = fieldLocation,
                    QueryLocation = queryableLocation
                };
                do
                {
                    if (formatSetting.Parameter is ISixnetField parameterField)
                    {
                        formatSetting.Parameter = await FormatFieldAsync(context, queryable, parameterField, queryableLocation, FieldLocation.FormatParameter, criterionOperator, tablePetName, formatSetting.Name).ConfigureAwait(false);
                    }
                    formatContext.FieldName = formatedFieldName;
                    formatContext.FormatSetting = formatSetting;
                    var fieldFormatter = SixnetDataManager.GetFieldFormatter(formatSetting.Name) ?? DefaultFieldFormatter;
                    formatedFieldName = fieldFormatter.Format(formatContext);
                    formatSetting = formatSetting.Child;

                } while (formatSetting != null);
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

        #endregion

        #endregion
    }
}
