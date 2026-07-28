// "Company © 2025. All rights reserved."

using System.Data;
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
    public abstract partial class SixnetBaseDataCommandResolver
    {
        #region Statements

        #region Query

        /// <summary>
        /// Generate a query statement
        /// </summary>
        /// <param name="command">Database single command</param>
        /// <returns></returns>
        public virtual async Task<SixnetQueryDatabaseStatement> GenerateDatabaseQueryStatementAsync(SixnetSingleDatabaseCommand command)
        {
            //create context
            var context = new SixnetDataCommandResolveContext(command.Connection, command.DataCommand);

            //translation query
            var queryableTranResult = await TranslateAsync(context).ConfigureAwait(false);

            //generate statement
            var statement = await GenerateQueryStatementCoreAsync(context, queryableTranResult, SixnetQueryableLocation.Top).ConfigureAwait(false);
            var queryable = queryableTranResult.GetOriginalQueryable();
            statement.ScriptType = GetCommandType(queryable.Info.ScriptType);
            return statement;
        }

        /// <summary>
        /// Generate a query statement
        /// </summary>
        /// <param name="command">Database multiple command</param>
        /// <returns></returns>
        public virtual async Task<SixnetQueryDatabaseStatement> GenerateDatabaseQueryStatementAsync(SixnetMultipleDatabaseCommand command)
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
                var queryableTranResult = await TranslateAsync(context).ConfigureAwait(false);

                //generate statement
                var cmdQueryableStatement = await GenerateQueryStatementCoreAsync(context, queryableTranResult, SixnetQueryableLocation.Top).ConfigureAwait(false);

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
        public virtual async Task<SixnetQueryDatabaseStatement> GenerateDatabaseQueryPagingStatementAsync(SixnetSingleDatabaseCommand command)
        {
            var queryable = command?.DataCommand?.Queryable;
            //create context
            var context = new SixnetDataCommandResolveContext(command.Connection, command.DataCommand);
            //translation query
            var translationResult = await TranslateAsync(context).ConfigureAwait(false);
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
                        var targetStatement = await GetFromTargetStatementAsync(context, queryable, SixnetQueryableLocation.Top, tablePetName).ConfigureAwait(false);
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
                    var outputFieldString = await FormatFieldsStringAsync(context, queryable, SixnetQueryableLocation.Top, SixnetFieldLocation.Output, outputFields).ConfigureAwait(false);

                    //sort
                    var sort = translationResult.GetSort();
                    if (string.IsNullOrWhiteSpace(sort))
                    {
                        sort = await GetDefaultSortAsync(context, translationResult, queryable, outputFields, tablePetName).ConfigureAwait(false);
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
        protected abstract Task<SixnetQueryDatabaseStatement> GenerateQueryStatementCoreAsync(SixnetDataCommandResolveContext context, SixnetQueryableTranslationResult translationResult, SixnetQueryableLocation queryableLocation);

        #endregion

        #region Execution

        /// <summary>
        /// Generate database execution statement
        /// </summary>
        /// <param name="command">Database single command</param>
        /// <returns></returns>
        public virtual async Task<List<SixnetExecutionDatabaseStatement>> GenerateDatabaseExecutionStatementsAsync(SixnetSingleDatabaseCommand command)
        {
            var commandResolveContext = new SixnetDataCommandResolveContext(command.Connection, command.DataCommand);
            return await GenerateDatabaseExecutionStatementsAsync(commandResolveContext).ConfigureAwait(false);
        }

        /// <summary>
        /// Generate database statement groups
        /// </summary>
        /// <param name="command">Database execution command</param>
        /// <returns></returns>
        public virtual async Task<List<SixnetExecutionDatabaseStatement>> GenerateDatabaseExecutionStatementsAsync(SixnetMultipleDatabaseCommand command)
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
        protected virtual async Task<List<SixnetExecutionDatabaseStatement>> GenerateDatabaseExecutionStatementsAsync(SixnetDataCommandResolveContext context)
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
                            statements.AddRange(await GenerateInsertStatementsAsync(context).ConfigureAwait(false));
                        }
                        break;
                    case SixnetDataOperationType.Update:
                        if (!(command?.FieldsAssignment?.NewValues?.IsNullOrEmpty() ?? true))
                        {
                            statements.AddRange(await GenerateUpdateStatementsAsync(context).ConfigureAwait(false));
                        }
                        break;
                    case SixnetDataOperationType.Delete:
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
        protected abstract Task<List<SixnetExecutionDatabaseStatement>> GenerateInsertStatementsAsync(SixnetDataCommandResolveContext context);

        /// <summary>
        /// Get update statement
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <returns></returns>
        protected abstract Task<List<SixnetExecutionDatabaseStatement>> GenerateUpdateStatementsAsync(SixnetDataCommandResolveContext context);

        /// <summary>
        /// Get delete statement
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <returns></returns>
        protected abstract Task<List<SixnetExecutionDatabaseStatement>> GenerateDeleteStatementsAsync(SixnetDataCommandResolveContext context);

        #endregion

        #region Migration

        #region Generate database migration statements

        /// <summary>
        /// Generate database migration statements
        /// </summary>
        /// <param name="command">Database migration command</param>
        /// <returns></returns>
        public virtual async Task<List<SixnetExecutionDatabaseStatement>> GenerateDatabaseMigrationStatementsAsync(SixnetMigrationDatabaseCommand command)
        {
            var statements = new List<SixnetExecutionDatabaseStatement>();

            #region Clear database

            if (command?.MigrationInfo?.ClearDatabase ?? false)
            {
                var clearForeignKeyStatements = await GetDeleteAllForeignKeyStatementsAsync(command).ConfigureAwait(false);
                if (!clearForeignKeyStatements.IsNullOrEmpty())
                {
                    statements.AddRange(clearForeignKeyStatements);
                }

                var clearViewStatements = await GetDeleteAllViewStatementsAsync(command).ConfigureAwait(false);
                if (!clearViewStatements.IsNullOrEmpty())
                {
                    statements.AddRange(clearViewStatements);
                }

                var clearTableStatements = await GetDeleteAllTableStatementsAsync(command).ConfigureAwait(false);
                if (!clearTableStatements.IsNullOrEmpty())
                {
                    statements.AddRange(clearTableStatements);
                }

                var clearProcedureStatements = await GetDeleteAllProcedureStatementsAsync(command).ConfigureAwait(false);
                if (!clearProcedureStatements.IsNullOrEmpty())
                {
                    statements.AddRange(clearProcedureStatements);
                }

                var clearFunctionStatements = await GetDeleteAllFunctionStatementsAsync(command).ConfigureAwait(false);
                if (!clearFunctionStatements.IsNullOrEmpty())
                {
                    statements.AddRange(clearFunctionStatements);
                }

                var clearCustomTypeStatements = await GetDeleteAllCustomTypeStatementsAsync(command).ConfigureAwait(false);
                if (!clearCustomTypeStatements.IsNullOrEmpty())
                {
                    statements.AddRange(clearCustomTypeStatements);
                }

                return statements;
            }
            #endregion

            #region Delete foreign key

            // Delete foreign key
            var deleteForeignKeyStatements = await GetDeleteForeignKeyStatementsAsync(command).ConfigureAwait(false);
            if (!deleteForeignKeyStatements.IsNullOrEmpty())
            {
                statements.AddRange(deleteForeignKeyStatements);
            }

            // Delete all foreign key
            if (command?.MigrationInfo?.DeleteAllForeignKey ?? false)
            {
                var deleteAllForeignKeyStatements = await GetDeleteAllForeignKeyStatementsAsync(command).ConfigureAwait(false);
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
                var deleteAllViewStatements = await GetDeleteAllViewStatementsAsync(command).ConfigureAwait(false);
                if (!deleteAllViewStatements.IsNullOrEmpty())
                {
                    statements.AddRange(deleteAllViewStatements);
                }
            }

            #endregion

            #region Tables

            // New tables
            var createTableStatements = await GetCreateTableStatementsAsync(command).ConfigureAwait(false);
            if (!createTableStatements.IsNullOrEmpty())
            {
                statements.AddRange(createTableStatements);
            }

            // Rename tables
            var renameTableStatements = await GetRenameTableStatementsAsync(command).ConfigureAwait(false);
            if (!renameTableStatements.IsNullOrEmpty())
            {
                statements.AddRange(renameTableStatements);
            }

            // Delete tables
            var deleteTableStatements = await GetDeleteTableStatementsAsync(command).ConfigureAwait(false);
            if (!deleteTableStatements.IsNullOrEmpty())
            {
                statements.AddRange(deleteTableStatements);
            }

            // Delete all tables
            if (command?.MigrationInfo?.DeleteAllTable ?? false)
            {
                var deleteAllTableStatements = await GetDeleteAllTableStatementsAsync(command).ConfigureAwait(false);
                if (!deleteAllTableStatements.IsNullOrEmpty())
                {
                    statements.AddRange(deleteAllTableStatements);
                }
            }

            #endregion

            #region Fields

            // New fields
            var newFieldStatements = await GetAddFieldStatementsAsync(command).ConfigureAwait(false);
            if (!newFieldStatements.IsNullOrEmpty())
            {
                statements.AddRange(newFieldStatements);
            }

            // Update fields
            var updateFieldStatements = await GetUpdateFieldStatementsAsync(command).ConfigureAwait(false);
            if (!updateFieldStatements.IsNullOrEmpty())
            {
                statements.AddRange(updateFieldStatements);
            }

            // Delete fields
            var deleteFieldStatements = await GetDeleteFieldStatementsAsync(command).ConfigureAwait(false);
            if (!deleteFieldStatements.IsNullOrEmpty())
            {
                statements.AddRange(deleteFieldStatements);
            }

            #endregion

            #region Index

            // Delete index
            var deleteIndexStatements = await GetDeleteIndexStatementsAsync(command).ConfigureAwait(false);
            if (!deleteIndexStatements.IsNullOrEmpty())
            {
                statements.AddRange(deleteIndexStatements);
            }

            // New index
            var addIndexStatements = await GetAddIndexStatementsAsync(command).ConfigureAwait(false);
            if (!addIndexStatements.IsNullOrEmpty())
            {
                statements.AddRange(addIndexStatements);
            }

            #endregion

            #region Procedure

            // Delete all procedure
            if (command?.MigrationInfo?.DeleteAllProcedure ?? false)
            {
                var deleteAllProcedureStatements = await GetDeleteAllProcedureStatementsAsync(command).ConfigureAwait(false);
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
                var deleteAllFunctionStatements = await GetDeleteAllFunctionStatementsAsync(command).ConfigureAwait(false);
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
                var deleteAllCustomTypeStatements = await GetDeleteAllCustomTypeStatementsAsync(command).ConfigureAwait(false);
                if (!deleteAllCustomTypeStatements.IsNullOrEmpty())
                {
                    statements.AddRange(deleteAllCustomTypeStatements);
                }
            }

            #endregion

            #region New foreign key

            // New foreign key
            var addForeignKeyStatements = await GetAddForeignKeyStatementsAsync(command).ConfigureAwait(false);
            if (!addForeignKeyStatements.IsNullOrEmpty())
            {
                statements.AddRange(addForeignKeyStatements);
            }

            #endregion

            return statements;
        }

        #endregion

        #region Tables

        #region Get create table statements

        /// <summary>
        /// Get create table statements
        /// </summary>
        /// <param name="migrationCommand">Migration command</param>
        /// <returns></returns>
        protected abstract Task<List<SixnetExecutionDatabaseStatement>> GetCreateTableStatementsAsync(SixnetMigrationDatabaseCommand migrationCommand);

        #endregion

        #region Get rename table statements

        protected abstract Task<List<SixnetExecutionDatabaseStatement>> GetRenameTableStatementsAsync(SixnetMigrationDatabaseCommand migrationCommand);

        #endregion

        #region Get delete table statements

        /// <summary>
        /// Get delete table statements
        /// </summary>
        /// <param name="migrationCommand"></param>
        /// <returns></returns>
        protected virtual Task<List<SixnetExecutionDatabaseStatement>> GetDeleteTableStatementsAsync(SixnetMigrationDatabaseCommand migrationCommand)
        {
            return Task.FromResult(GetDeleteTableStatements(migrationCommand));
        }

        #endregion

        #region Get delete all table statements

        /// <summary>
        /// Get delete all table statements
        /// </summary>
        /// <param name="migrationCommand"></param>
        /// <returns></returns>
        protected abstract Task<List<SixnetExecutionDatabaseStatement>> GetDeleteAllTableStatementsAsync(SixnetMigrationDatabaseCommand migrationCommand);

        #endregion

        #endregion

        #region Procedure

        /// <summary>
        /// Get delete all procedure statements
        /// </summary>
        /// <param name="migrationCommand"></param>
        /// <returns></returns>
        protected abstract Task<List<SixnetExecutionDatabaseStatement>> GetDeleteAllProcedureStatementsAsync(SixnetMigrationDatabaseCommand migrationCommand);

        #endregion

        #region Functions

        /// <summary>
        /// Get delete all function statements
        /// </summary>
        /// <param name="migrationCommand"></param>
        /// <returns></returns>
        protected abstract Task<List<SixnetExecutionDatabaseStatement>> GetDeleteAllFunctionStatementsAsync(SixnetMigrationDatabaseCommand migrationCommand);

        #endregion

        #region Custom type

        /// <summary>
        /// Get delete all custom type statements
        /// </summary>
        /// <param name="migrationCommand"></param>
        /// <returns></returns>
        protected abstract Task<List<SixnetExecutionDatabaseStatement>> GetDeleteAllCustomTypeStatementsAsync(SixnetMigrationDatabaseCommand migrationCommand);

        #endregion

        #region Fields

        #region Get add filed statements

        protected abstract Task<List<SixnetExecutionDatabaseStatement>> GetAddFieldStatementsAsync(SixnetMigrationDatabaseCommand migrationCommand);

        #endregion

        #region Get update field statements 

        protected abstract Task<List<SixnetExecutionDatabaseStatement>> GetUpdateFieldStatementsAsync(SixnetMigrationDatabaseCommand migrationCommand);

        #endregion

        #region Get delete filed statements

        protected abstract Task<List<SixnetExecutionDatabaseStatement>> GetDeleteFieldStatementsAsync(SixnetMigrationDatabaseCommand migrationCommand);

        #endregion

        #endregion

        #region Foreign key

        #region Add foreign key

        /// <summary>
        /// Get add foreign key statements
        /// </summary>
        /// <param name="migrationCommand"></param>
        /// <returns></returns>
        protected abstract Task<List<SixnetExecutionDatabaseStatement>> GetAddForeignKeyStatementsAsync(SixnetMigrationDatabaseCommand migrationCommand);


        #endregion

        #region Delete foreign key

        /// <summary>
        /// Get delete foreign key statements
        /// </summary>
        /// <param name="migrationCommand"></param>
        /// <returns></returns>
        protected abstract Task<List<SixnetExecutionDatabaseStatement>> GetDeleteForeignKeyStatementsAsync(SixnetMigrationDatabaseCommand migrationCommand);

        /// <summary>
        /// Get delete foreign key statements
        /// </summary>
        /// <param name="migrationCommand"></param>
        /// <returns></returns>
        protected abstract Task<List<SixnetExecutionDatabaseStatement>> GetDeleteAllForeignKeyStatementsAsync(SixnetMigrationDatabaseCommand migrationCommand);

        #endregion

        #endregion

        #region Index

        #region Add index

        /// <summary>
        /// Get add index status
        /// </summary>
        /// <param name="migrationCommand"></param>
        /// <returns></returns>
        protected abstract Task<List<SixnetExecutionDatabaseStatement>> GetAddIndexStatementsAsync(SixnetMigrationDatabaseCommand migrationCommand);

        #endregion

        #region Delete index

        /// <summary>
        /// Get delete index statements
        /// </summary>
        /// <param name="migrationCommand"></param>
        /// <returns></returns>
        protected abstract Task<List<SixnetExecutionDatabaseStatement>> GetDeleteIndexStatementsAsync(SixnetMigrationDatabaseCommand migrationCommand);

        #endregion 

        #endregion

        #region View

        /// <summary>
        /// Get delete all view statements
        /// </summary>
        /// <param name="migrationCommand"></param>
        /// <returns></returns>
        protected abstract Task<List<SixnetExecutionDatabaseStatement>> GetDeleteAllViewStatementsAsync(SixnetMigrationDatabaseCommand migrationCommand);

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
        protected virtual async Task<SixnetQueryDatabaseStatement> GetFromTargetStatementAsync(SixnetDataCommandResolveContext context, ISixnetQueryable originalQueryable
            , SixnetQueryableLocation location, string tablePetName, bool applyTablePetName = true)
        {
            switch (originalQueryable.Info.FromType)
            {
                case SixnetQueryableFromType.Queryable:
                    var targetTranslationResult = await ExecuteTranslationAsync(context, originalQueryable.Info.TargetQueryable, SixnetQueryableLocation.From, true).ConfigureAwait(false);
                    var databaseStatement = await GenerateQueryStatementCoreAsync(context, targetTranslationResult, SixnetQueryableLocation.From).ConfigureAwait(false);
                    databaseStatement.Script = $"({databaseStatement.Script}){(applyTablePetName ? $"{TablePetNameKeyword}{tablePetName}" : "")}";
                    databaseStatement.ComplexTarget = true;
                    if (!databaseStatement.OutputFields.IsNullOrEmpty())
                    {
                        databaseStatement.OutputFields = new List<ISixnetField>(1) { SixnetDataField.Create("*", originalQueryable.GetModelType(), 0, null, "*") };
                    }
                    return databaseStatement;
                default:
                    var tableNames = await context.GetTableNamesAsync(originalQueryable, location).ConfigureAwait(false);
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
        protected virtual async Task<SixnetQueryDatabaseStatement> GetJoinTargetStatementAsync(SixnetDataCommandResolveContext context, ISixnetQueryable topQueryable, SixnetJoinEntry joinEntry)
        {
            var joinTargetQueryable = joinEntry.Target;
            var joinTablePetName = context.GetTablePetName(topQueryable, joinTargetQueryable.GetModelType(), joinEntry.Index);
            return await GetFromTargetStatementAsync(context, joinTargetQueryable, SixnetQueryableLocation.JoinTarget, joinTablePetName).ConfigureAwait(false);
        }

        #endregion

        #endregion

        #region Condition

        /// <summary>
        /// Translate query object
        /// </summary>
        /// <param name="context">Command resolve context</param>
        /// <returns>Return a translation result</returns>
        protected virtual async Task<SixnetQueryableTranslationResult> TranslateAsync(SixnetDataCommandResolveContext context)
        {
            var queryable = context?.DataCommandExecutionContext?.Command?.Queryable;
            if (queryable != null)
            {
                return await ExecuteTranslationAsync(context, queryable, SixnetQueryableLocation.Top).ConfigureAwait(false);
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
        protected virtual async Task<SixnetQueryableTranslationResult> ExecuteTranslationAsync(SixnetDataCommandResolveContext context, ISixnetQueryable queryable, SixnetQueryableLocation location, bool useSort = true)
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
        protected virtual async Task<SixnetQueryableTranslationResult> AppendConditionAsync(SixnetDataCommandResolveContext context, SixnetQueryableTranslationResult parentTranslationResult, ISixnetQueryable queryable)
        {
            if (!queryable.Info.Conditions.IsNullOrEmpty())
            {
                foreach (var condition in queryable.Info.Conditions)
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
        protected virtual async Task<SixnetQueryableTranslationResult> TranslateConditionAsync(SixnetDataCommandResolveContext context, ISixnetQueryable topQueryable, ISixnetCondition condition)
        {
            SixnetQueryableTranslationResult translationResult = null;
            if (condition == null)
            {
                return translationResult;
            }
            if (condition is SixnetCriterion criterion)
            {
                translationResult = await TranslateCriterionAsync(context, topQueryable, criterion).ConfigureAwait(false);
            }
            if (condition is ISixnetQueryable groupQueryable && !groupQueryable.Info.Conditions.IsNullOrEmpty())
            {
                var conditionCount = groupQueryable.Info.Conditions.Count();
                if (conditionCount == 1)
                {
                    var firstCondition = groupQueryable.Info.Conditions.First();
                    if (firstCondition is SixnetCriterion firstCriterion)
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
                    translationResult = SixnetQueryableTranslationResult.Create(topQueryable);
                    var groupCondition = new StringBuilder($"(");
                    var index = 0;
                    foreach (var groupItem in groupQueryable.Info.Conditions)
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
        protected virtual async Task<SixnetQueryableTranslationResult> TranslateCriterionAsync(SixnetDataCommandResolveContext context, ISixnetQueryable topQueryable, SixnetCriterion criterion)
        {
            var criterionTranResult = SixnetQueryableTranslationResult.Create(topQueryable);
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
        protected virtual async Task<string> TranslateSubqueryAsync(SixnetDataCommandResolveContext context, ISixnetQueryable subqueryable)
        {
            SixnetException.ThrowIf(subqueryable.Info.SelectedFields.IsNullOrEmpty(), "Subqueryable must set query fields");

            var subqueryTranslationResult = await ExecuteTranslationAsync(context, subqueryable, SixnetQueryableLocation.Subquery, true).ConfigureAwait(false);
            var subqueryStatement = await GenerateQueryStatementCoreAsync(context, subqueryTranslationResult, SixnetQueryableLocation.Subquery).ConfigureAwait(false);
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
        protected virtual async Task<SixnetQueryableTranslationResult> AppandCombineAsync(SixnetDataCommandResolveContext context, ISixnetQueryable topQueryable, SixnetQueryableTranslationResult parentTranslationResult)
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
                var combineQueryResult = await ExecuteTranslationAsync(context, combineEntry.Target, SixnetQueryableLocation.Combine, true).ConfigureAwait(false);
                var combineStatement = await GenerateQueryStatementCoreAsync(context, combineQueryResult, SixnetQueryableLocation.Combine).ConfigureAwait(false);
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
        protected virtual async Task<SixnetQueryableTranslationResult> AppendSortAsync(SixnetDataCommandResolveContext context, ISixnetQueryable originalQueryable, SixnetQueryableTranslationResult parentTranslationResult, bool useSort)
        {
            if (!useSort || (originalQueryable?.Info.Sorts.IsNullOrEmpty() ?? true))
            {
                return parentTranslationResult;
            }
            var sortBuilder = new StringBuilder();
            var hasGroup = !originalQueryable.Info.GroupFields.IsNullOrEmpty();
            foreach (var sortEntry in originalQueryable.Info.Sorts)
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
        protected virtual async Task<SixnetQueryableTranslationResult> AppendJoinAsync(SixnetDataCommandResolveContext context, ISixnetQueryable topQueryable, SixnetQueryableTranslationResult parentTranslationResult)
        {
            if (topQueryable.Info.Joins.IsNullOrEmpty())
            {
                return parentTranslationResult;
            }
            var joinBuilder = new StringBuilder();
            foreach (var joinEntry in topQueryable.Info.Joins)
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
        protected virtual async Task<SixnetQueryableTranslationResult> GetJoinConnectionAsync(SixnetDataCommandResolveContext context, ISixnetQueryable topQueryable, SixnetJoinEntry joinEntry)
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
        protected virtual async Task<SixnetQueryableTranslationResult> AppendGroupAsync(SixnetDataCommandResolveContext context, ISixnetQueryable originalQueryable
            , SixnetQueryableTranslationResult translationResult, SixnetQueryableLocation location)
        {
            if (!originalQueryable.Info.GroupFields.IsNullOrEmpty())
            {
                var groupFormatedFields = new List<string>();
                foreach (var groupField in originalQueryable.Info.GroupFields)
                {
                    groupFormatedFields.Add(await FormatFieldAsync(context, originalQueryable, SixnetDataManager.GetField(DatabaseType, originalQueryable?.GetModelType(), groupField), location, SixnetFieldLocation.Criterion).ConfigureAwait(false));
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
        protected virtual async Task<SixnetQueryableTranslationResult> AppendHavingAsync(SixnetDataCommandResolveContext context, ISixnetQueryable originalQuery, SixnetQueryableTranslationResult translationResult, SixnetQueryableLocation location)
        {
            if (originalQuery.Info.HavingQueryable != null)
            {
                var havingResult = SixnetQueryableTranslationResult.Create(originalQuery);
                foreach (var condition in originalQuery.Info.HavingQueryable.Info.Conditions)
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
        protected virtual async Task<SixnetQueryableTranslationResult> AppendTreeAsync(SixnetDataCommandResolveContext context, ISixnetQueryable originalQueryable, SixnetQueryableTranslationResult translationResult, SixnetQueryableLocation location)
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
            var treeDataFieldString = await FormatFieldAsync(context, originalQueryable, dataField, SixnetQueryableLocation.PreScript
                , SixnetFieldLocation.Join, tablePetName: treeInfo.Direction == SixnetTreeMatchingDirection.Down ? preScriptTablePetName : "")
                .ConfigureAwait(false);
            var treeParentFieldString = await FormatFieldAsync(context, originalQueryable, parentField, SixnetQueryableLocation.PreScript
                , SixnetFieldLocation.Join, tablePetName: treeInfo.Direction == SixnetTreeMatchingDirection.Up ? preScriptTablePetName : "")
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
                var outputFieldString = await FormatFieldsStringAsync(context, originalQueryable, SixnetQueryableLocation.PreScript, SixnetFieldLocation.InnerOutput, outputFields).ConfigureAwait(false);
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
                var outputFieldString = await FormatFieldsStringAsync(context, originalQueryable, SixnetQueryableLocation.PreScript, SixnetFieldLocation.InnerOutput, outputFields).ConfigureAwait(false);
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

        #region Util

        #region Get default sort

        protected virtual async Task<string> GetDefaultSortAsync(SixnetDataCommandResolveContext context, SixnetQueryableTranslationResult translationResult, ISixnetQueryable originalQueryable, IEnumerable<ISixnetField> dataFields, string tablePetName)
        {
            var defaultSortField = dataFields?.Where(f => f is SixnetDataField)
                                              .OrderByDescending(f => f.InRole(SixnetFieldRole.Sequence))
                                              .ThenByDescending(f => f.InRole(SixnetFieldRole.PrimaryKey))
                                              .FirstOrDefault();
            if (defaultSortField != null)
            {
                var orderField = SixnetDataField.Create(defaultSortField.PropertyName, originalQueryable.GetModelType());
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
        protected virtual async Task<string> FormatFieldsStringAsync(SixnetDataCommandResolveContext context, ISixnetQueryable queryable, SixnetQueryableLocation queryLocation, SixnetFieldLocation fieldLocation, IEnumerable<ISixnetField> fields)
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
        protected virtual async Task<IEnumerable<string>> FormatFieldsAsync(SixnetDataCommandResolveContext context, ISixnetQueryable queryable, SixnetQueryableLocation queryLocation, SixnetFieldLocation fieldLocation, IEnumerable<ISixnetField> fields)
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
        protected virtual async Task<string> FormatSortFieldAsync(SixnetDataCommandResolveContext context, ISixnetQueryable queryable, SixnetSortEntry sortEntry)
        {
            var field = SixnetDataManager.GetField(context.DataCommandExecutionContext.Server.DatabaseType, queryable?.GetModelType(), sortEntry.Field);
            return await FormatFieldAsync(context, queryable, field, SixnetQueryableLocation.Top, SixnetFieldLocation.Sort).ConfigureAwait(false);
        }

        /// <summary>
        /// Format criterion field
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="tablePetName">Table pet name</param>
        /// <param name="queryable">Query object</param>
        /// <param name="field">Field</param>
        /// <returns></returns>
        protected virtual async Task<string> FormatCriterionFieldAsync(SixnetDataCommandResolveContext context, ISixnetQueryable queryable, ISixnetField field, SixnetCriterionOperator criterionOperator)
        {
            field = SixnetDataManager.GetField(context.DataCommandExecutionContext.Server.DatabaseType, queryable?.GetModelType(), field);
            return await FormatFieldAsync(context, queryable, field, SixnetQueryableLocation.Top, SixnetFieldLocation.Criterion, criterionOperator).ConfigureAwait(false);
        }

        /// <summary>
        /// Format update value field
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="command">Data command</param>
        /// <param name="value">Value</param>
        /// <returns></returns>
        protected virtual async Task<string> FormatUpdateValueFieldAsync(SixnetDataCommandResolveContext context, SixnetDataCommand command, dynamic value)
        {
            if (value == null)
            {
                return NullKeyword;
            }
            var valueField = value as ISixnetField;
            valueField ??= SixnetConstantField.Create(value);
            valueField = SixnetDataManager.GetField(context.DataCommandExecutionContext.Server.DatabaseType, command.GetEntityType(), valueField);
            return await FormatFieldAsync(context, command.Queryable, valueField, SixnetQueryableLocation.Top, SixnetFieldLocation.UpdateValue).ConfigureAwait(false);
        }

        /// <summary>
        /// Format insert value field
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="queryable">Queryable</param>
        /// <param name="valueField">Value field</param>
        /// <returns></returns>
        protected virtual async Task<string> FormatInsertValueFieldAsync(SixnetDataCommandResolveContext context, ISixnetQueryable queryable, dynamic value)
        {
            if (value == null)
            {
                return NullKeyword;
            }
            var valueField = value as ISixnetField;
            valueField ??= SixnetConstantField.Create(value);
            return await FormatFieldAsync(context, queryable, valueField, SixnetQueryableLocation.Top, SixnetFieldLocation.InsertValue).ConfigureAwait(false);
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
        protected virtual async Task<string> FormatFieldAsync(SixnetDataCommandResolveContext context, ISixnetQueryable queryable, ISixnetField field
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
                formatedFieldName = $"({await TranslateSubqueryAsync(context, queryableField.Queryable).ConfigureAwait(false)})";
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
                    formatedFieldName = constantField.Value == null ? $"{NullKeyword}" : $"{constantField.Value}";
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
                        formatSetting.Parameter = await FormatFieldAsync(context, queryable, parameterField, queryableLocation
                            , SixnetFieldLocation.FormatParameter, criterionOperator, tablePetName, formatSetting.Name)
                            .ConfigureAwait(false);
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

        #endregion

        #endregion
    }
}
