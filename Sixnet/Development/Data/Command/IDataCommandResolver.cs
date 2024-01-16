using System.Collections.Generic;
using Sixnet.Development.Data.Database;

namespace Sixnet.Development.Data.Command
{
    /// <summary>
    /// Defines data command resolver
    /// </summary>
    public interface IDataCommandResolver
    {
        /// <summary>
        /// Generate a query statement
        /// </summary>
        /// <param name="command">Database single command</param>
        /// <returns></returns>
        DatabaseQueryStatement GenerateDatabaseQueryStatement(DatabaseSingleCommand command);

        /// <summary>
        /// Generate a query statement
        /// </summary>
        /// <param name="command">Database multiple command</param>
        /// <returns></returns>
        DatabaseQueryStatement GenerateDatabaseQueryStatement(DatabaseMultipleCommand command);

        /// <summary>
        /// Generate a paging statement
        /// </summary>
        /// <param name="command">Database single command</param>
        /// <returns></returns>
        DatabaseQueryStatement GenerateDatabaseQueryPagingStatement(DatabaseSingleCommand command);

        /// <summary>
        /// Generate a execution statement
        /// </summary>
        /// <param name="command">Database single command</param>
        /// <returns></returns>
        List<DatabaseExecutionStatement> GenerateDatabaseExecutionStatements(DatabaseSingleCommand command);

        /// <summary>
        /// Generate a execution statement
        /// </summary>
        /// <param name="command">Database multiple command</param>
        /// <returns></returns>
        List<DatabaseExecutionStatement> GenerateDatabaseExecutionStatements(DatabaseMultipleCommand command);

        /// <summary>
        /// Generate database migration statement
        /// </summary>
        /// <param name="command">Database migration command</param>
        /// <returns></returns>
        List<DatabaseExecutionStatement> GenerateDatabaseMigrationStatements(DatabaseMigrationCommand command);
    }
}
