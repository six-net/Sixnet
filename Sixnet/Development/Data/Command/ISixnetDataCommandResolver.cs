// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Development.Data.Database;

namespace Sixnet.Development.Data.Command
{
    /// <summary>
    /// Defines data command resolver
    /// </summary>
    public interface ISixnetDataCommandResolver
    {
        /// <summary>
        /// Generate a query statement
        /// </summary>
        /// <param name="command">Database single command</param>
        /// <returns></returns>
        QueryDatabaseStatement GenerateDatabaseQueryStatement(SingleDatabaseCommand command);

        /// <summary>
        /// Generate a query statement
        /// </summary>
        /// <param name="command">Database single command</param>
        /// <returns></returns>
        Task<QueryDatabaseStatement> GenerateDatabaseQueryStatementAsync(SingleDatabaseCommand command);

        /// <summary>
        /// Generate a query statement
        /// </summary>
        /// <param name="command">Database multiple command</param>
        /// <returns></returns>
        QueryDatabaseStatement GenerateDatabaseQueryStatement(MultipleDatabaseCommand command);

        /// <summary>
        /// Generate a query statement
        /// </summary>
        /// <param name="command">Database multiple command</param>
        /// <returns></returns>
        Task<QueryDatabaseStatement> GenerateDatabaseQueryStatementAsync(MultipleDatabaseCommand command);

        /// <summary>
        /// Generate a paging statement
        /// </summary>
        /// <param name="command">Database single command</param>
        /// <returns></returns>
        QueryDatabaseStatement GenerateDatabaseQueryPagingStatement(SingleDatabaseCommand command);

        /// <summary>
        /// Generate a paging statement
        /// </summary>
        /// <param name="command">Database single command</param>
        /// <returns></returns>
        Task<QueryDatabaseStatement> GenerateDatabaseQueryPagingStatementAsync(SingleDatabaseCommand command);

        /// <summary>
        /// Generate a execution statement
        /// </summary>
        /// <param name="command">Database single command</param>
        /// <returns></returns>
        List<ExecutionDatabaseStatement> GenerateDatabaseExecutionStatements(SingleDatabaseCommand command);

        /// <summary>
        /// Generate a execution statement
        /// </summary>
        /// <param name="command">Database single command</param>
        /// <returns></returns>
        Task<List<ExecutionDatabaseStatement>> GenerateDatabaseExecutionStatementsAsync(SingleDatabaseCommand command);

        /// <summary>
        /// Generate a execution statement
        /// </summary>
        /// <param name="command">Database multiple command</param>
        /// <returns></returns>
        List<ExecutionDatabaseStatement> GenerateDatabaseExecutionStatements(MultipleDatabaseCommand command);

        /// <summary>
        /// Generate a execution statement
        /// </summary>
        /// <param name="command">Database multiple command</param>
        /// <returns></returns>
        Task<List<ExecutionDatabaseStatement>> GenerateDatabaseExecutionStatementsAsync(MultipleDatabaseCommand command);

        /// <summary>
        /// Generate database migration statement
        /// </summary>
        /// <param name="command">Database migration command</param>
        /// <returns></returns>
        List<ExecutionDatabaseStatement> GenerateDatabaseMigrationStatements(MigrationDatabaseCommand command);
    }
}
