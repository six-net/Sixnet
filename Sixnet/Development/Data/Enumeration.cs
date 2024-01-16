using System;

namespace Sixnet.Development.Data
{
    #region Data operation type

    /// <summary>
    /// Data operation type
    /// </summary>
    [Serializable]
    public enum DataOperationType
    {
        /// <summary>
        /// Insert
        /// </summary>
        Insert = 410,
        /// <summary>
        /// Update
        /// </summary>
        Update = 420,
        /// <summary>
        /// Delete
        /// </summary>
        Delete = 430,
        /// <summary>
        /// Query
        /// </summary>
        Query = 440,
        /// <summary>
        /// Exist
        /// </summary>
        Exist = 450,
        /// <summary>
        /// Scalar
        /// </summary>
        Scalar = 460,
        /// <summary>
        /// Bulk insert
        /// </summary>
        BulkInsert = 470
    }

    #endregion

    #region Database server type

    /// <summary>
    /// Database server type
    /// </summary>
    [Serializable]
    public enum DatabaseServerType
    {
        Others = 0,
        SQLServer = 110,
        MySQL = 120,
        Oracle = 130,
        SQLite = 150,
        PostgreSQL = 160,
    }

    /// <summary>
    /// Database server type
    /// </summary>
    [Serializable]
    public enum DatabaseServerRole
    {
        /// <summary>
        /// Default
        /// </summary>
        Default = 0,
        /// <summary>
        /// Read only
        /// </summary>
        ReadOnly = 2
    }

    #endregion

    #region Calculation operator

    /// <summary>
    /// Calculation operator
    /// </summary>
    [Serializable]
    public enum CalculationOperator
    {
        Add,
        Subtract,
        Multiply,
        Divide
    }

    #endregion

    #region Default data command database server source

    /// <summary>
    /// Default data command database server source
    /// </summary>
    public enum DefaultDataCommandDatabaseServerSource
    {
        None = 0,
        Default = 2,
        All = 4
    }

    #endregion

    #region Data isolation level

    /// <summary>
    /// Data isolation level
    /// </summary>
    [Serializable]
    public enum DataIsolationLevel
    {
        /// <summary>
        /// A different isolation level than the one specified is being used, but the level cannot be determined.
        /// </summary>
        Unspecified = -1,

        /// <summary>
        /// The pending changes from more highly isolated transactions cannot be overwritten.
        /// </summary>
        Chaos = 16,

        /// <summary>
        /// A dirty read is possible, meaning that no shared locks are issued and no exclusive locks are honored.
        /// </summary>
        ReadUncommitted = 256,

        /// <summary>
        /// Shared locks are held while the data is being read to avoid dirty reads, but
        /// the data can be changed before the end of the transaction, resulting in non-repeatable
        /// reads or phantom data.
        /// </summary>
        ReadCommitted = 4096,

        /// <summary>
        /// Locks are placed on all data that is used in a query, preventing other users
        /// from updating the data. Prevents non-repeatable reads but phantom rows are still
        /// possible.
        /// </summary>
        RepeatableRead = 65536,

        /// <summary>
        /// A range lock is placed on the System.Data.DataSet, preventing other users from
        /// updating or inserting rows into the dataset until the transaction is complete.
        /// </summary>
        Serializable = 1048576,

        /// <summary>
        /// Reduces blocking by storing a version of data that one application can read while
        /// another is modifying the same data. Indicates that from one transaction you cannot
        /// see changes made in other transactions, even if you requery.
        /// </summary>
        Snapshot = 16777216
    }

    #endregion

    #region Field location

    /// <summary>
    /// Field location
    /// </summary>
    public enum FieldLocation
    {
        Output = 310,
        InnerOutput = 315,
        Sort = 320,
        Criterion = 330,
        Join = 340,
        UpdateValue = 350,
        InsertValue = 360,
        FormatParameter = 370
    }

    #endregion

    #region Data intercept scene

    /// <summary>
    /// Data intercept scene
    /// </summary>
    public enum DataInterceptScene
    {
        Add = 1100,
        Modify = 1200,
        ModifyByExpression = 1300
    }

    #endregion

    #region Data script type

    /// <summary>
    /// Data script type
    /// </summary>
    [Serializable]
    public enum DataScriptType
    {
        /// <summary>
        /// SQL text
        /// </summary>
        Text = 210,
        /// <summary>
        /// The name of a stored procedure
        /// </summary>
        StoredProcedure = 220,
        /// <summary>
        /// The name of a table
        /// </summary>
        TableDirect = 230
    }

    #endregion

    #region Split table name selection pattern

    /// <summary>
    /// Split table name selection pattern
    /// </summary>
    public enum SplitTableNameSelectionPattern
    {
        /// <summary>
        /// Range
        /// </summary>
        Range = 110,
        /// <summary>
        /// Precision
        /// </summary>
        Precision = 120,
    }

    #endregion
}
