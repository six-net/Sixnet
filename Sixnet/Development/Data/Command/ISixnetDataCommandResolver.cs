// "Company © 2025. All rights reserved."

using System.Data;
using System.Threading.Tasks;

using Sixnet.Development.Data.Database;
using Sixnet.Development.Data.Field.Formatting;
using Sixnet.Development.Queryable;

namespace Sixnet.Development.Data.Command
{
    /// <summary>
    /// Defines data command resolver
    /// </summary>
    public interface ISixnetDataCommandResolver
    {
        #region Properties

        string KeywordPrefix { get; set; }

        string KeywordSuffix { get; set; }

        string ConditionStartKeyword { get; set; }

        string AndConnector { get; set; }

        string OrConnector { get; set; }

        string EqualOperator { get; set; }

        string GreaterThanOperator { get; set; }

        string GreaterThanOrEqualOperator { get; set; }

        string NotEqualOperator { get; set; }

        string LessThanOperator { get; set; }

        string LessThanOrEqualOperator { get; set; }

        string InOperator { get; set; }

        string NotInOperator { get; set; }

        string LikeOperator { get; set; }

        string NotLikeOperator { get; set; }

        string IsNullOperator { get; set; }

        string NotNullOperator { get; set; }

        string TrueOperator { get; set; }

        string FalseOperator { get; set; }

        string SortKeyword { get; set; }

        string DescKeyword { get; set; }

        string AscKeyword { get; set; }

        string GroupByKeyword { get; set; }

        string ParameterPrefix { get; set; }

        string PagingTableName { get; set; }

        string PagingCountTableName { get; set; }

        string TablePetNameKeyword { get; set; }

        string ColumnPetNameKeyword { get; set; }

        string WithTableKeyword { get; set; }

        string NullKeyword { get; set; }

        string DistinctKeyword { get; set; }

        string NegationKeyword { get; set; }

        bool ParameterizationJsonFormatter { get; set; }

        Dictionary<string, bool> NotParameterizationFormatterNameDict { get; set; }

        SixnetDatabaseType DatabaseType { get; set; }

        Dictionary<SixnetJoinType, string> JoinOperatorDict { get; set; }

        Dictionary<SixnetCalculationOperator, string> CalculationOperators { get; set; }

        ISixnetFieldFormatter DefaultFieldFormatter { get; set; }

        int DefaultCharLength { get; set; }

        int DefaultDecimalLength { get; set; }

        int DefaultDecimalPrecision { get; set; }

        Dictionary<DbType, string> DbTypeDefaultValues { get; set; }

        Func<SixnetDatabaseObjectName, SixnetDatabaseObjectName> FormatObjectNameFunc { get; set; }

        Func<SixnetDatabaseObjectName, SixnetDatabaseObjectName> WrapObjectNameFunc { get; set; }

        Func<SixnetDatabaseObjectName, string> GetObjectFullNameFunc { get; set; }

        string RecursiveKeyword { get; set; }

        bool UseFieldForRecursive { get; set; }

        bool SplitWrapParameter { get; set; }

        bool OnlyIncrementPrimaryKey { get; set; }

        bool DisableDefaultPrimaryKeyAsc { get; set; }

        int MaxIdentifierLength { get; set; }

        #endregion

        #region Methods

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

        /// <summary>
        /// Format object name
        /// </summary>
        /// <param name="objectName">Object name</param>
        /// <returns></returns>
        SixnetDatabaseObjectName FormatObjectName(SixnetDatabaseObjectName objectName);

        /// <summary>
        /// Wrap object name
        /// </summary>
        /// <param name="objectName">Object name</param>
        /// <returns></returns>
        SixnetDatabaseObjectName WrapObjectName(SixnetDatabaseObjectName objectName);

        /// <summary>
        /// Get object full name
        /// </summary>
        /// <param name="objectName">Object name</param>
        /// <returns></returns>
        string GetObjectFullName(SixnetDatabaseObjectName objectName);

        /// <summary>
        /// Format and wrap keyword object name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string FormatAndWrapObjectName(string name, SixnetDatabaseObjectType nameType);

        /// <summary>
        /// Format and wrap keyword object name
        /// </summary>
        /// <param name="objectName"></param>
        /// <returns></returns>
        string FormatAndWrapObjectName(SixnetDatabaseObjectName objectName);

        #endregion
    }
}
