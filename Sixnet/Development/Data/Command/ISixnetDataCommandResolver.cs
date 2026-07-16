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
        SixnetQueryDatabaseStatement GenerateDatabaseQueryStatement(SixnetSingleDatabaseCommand command);

        /// <summary>
        /// Generate a query statement
        /// </summary>
        /// <param name="command">Database single command</param>
        /// <returns></returns>
        Task<SixnetQueryDatabaseStatement> GenerateDatabaseQueryStatementAsync(SixnetSingleDatabaseCommand command);

        /// <summary>
        /// Generate a query statement
        /// </summary>
        /// <param name="command">Database multiple command</param>
        /// <returns></returns>
        SixnetQueryDatabaseStatement GenerateDatabaseQueryStatement(SixnetMultipleDatabaseCommand command);

        /// <summary>
        /// Generate a query statement
        /// </summary>
        /// <param name="command">Database multiple command</param>
        /// <returns></returns>
        Task<SixnetQueryDatabaseStatement> GenerateDatabaseQueryStatementAsync(SixnetMultipleDatabaseCommand command);

        /// <summary>
        /// Generate a paging statement
        /// </summary>
        /// <param name="command">Database single command</param>
        /// <returns></returns>
        SixnetQueryDatabaseStatement GenerateDatabaseQueryPagingStatement(SixnetSingleDatabaseCommand command);

        /// <summary>
        /// Generate a paging statement
        /// </summary>
        /// <param name="command">Database single command</param>
        /// <returns></returns>
        Task<SixnetQueryDatabaseStatement> GenerateDatabaseQueryPagingStatementAsync(SixnetSingleDatabaseCommand command);

        /// <summary>
        /// Generate a execution statement
        /// </summary>
        /// <param name="command">Database single command</param>
        /// <returns></returns>
        List<SixnetExecutionDatabaseStatement> GenerateDatabaseExecutionStatements(SixnetSingleDatabaseCommand command);

        /// <summary>
        /// Generate a execution statement
        /// </summary>
        /// <param name="command">Database single command</param>
        /// <returns></returns>
        Task<List<SixnetExecutionDatabaseStatement>> GenerateDatabaseExecutionStatementsAsync(SixnetSingleDatabaseCommand command);

        /// <summary>
        /// Generate a execution statement
        /// </summary>
        /// <param name="command">Database multiple command</param>
        /// <returns></returns>
        List<SixnetExecutionDatabaseStatement> GenerateDatabaseExecutionStatements(SixnetMultipleDatabaseCommand command);

        /// <summary>
        /// Generate a execution statement
        /// </summary>
        /// <param name="command">Database multiple command</param>
        /// <returns></returns>
        Task<List<SixnetExecutionDatabaseStatement>> GenerateDatabaseExecutionStatementsAsync(SixnetMultipleDatabaseCommand command);

        /// <summary>
        /// Generate database migration statement
        /// </summary>
        /// <param name="command">Database migration command</param>
        /// <returns></returns>
        List<SixnetExecutionDatabaseStatement> GenerateDatabaseMigrationStatements(SixnetMigrationDatabaseCommand command);

        /// <summary>
        /// Generate database migration statement
        /// </summary>
        /// <param name="command">Database migration command</param>
        /// <returns></returns>
        Task<List<SixnetExecutionDatabaseStatement>> GenerateDatabaseMigrationStatementsAsync(SixnetMigrationDatabaseCommand command);
    }
}
