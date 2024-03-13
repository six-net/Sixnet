using System;
using System.Collections.Generic;
using System.Data;
using System.Transactions;
using Sixnet.Threading.Locking;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Defines database connection
    /// </summary>
    public class DatabaseConnection
    {
        #region Properties

        /// <summary>
        /// Gets the database server
        /// </summary>
        public DatabaseServer DatabaseServer { get; private set; }

        /// <summary>
        /// Gets the db connection
        /// </summary>
        public IDbConnection DbConnection { get; private set; }

        /// <summary>
        /// Gets the db transaction
        /// </summary>
        public DatabaseTransaction Transaction { get; private set; }

        /// <summary>
        /// Whether use transaction
        /// </summary>
        internal bool UseTransaction = false;

        /// <summary>
        /// Get or set the data isolation level
        /// </summary>
        public DataIsolationLevel? DataIsolationLevel { get; set; }

        /// <summary>
        /// Gets the database provider
        /// </summary>
        public ISixnetDatabaseProvider DatabaseProvider { get; private set; }

        /// <summary>
        /// Gets or sets the connection lock
        /// </summary>
        private LockInstance? ConnectionLock { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Create a database connection
        /// </summary>
        /// <param name="databaseServer">Database server</param>
        /// <param name="useTransaction">Whether use transaction</param>
        /// <param name="isolationLevel">Isolation level</param>
        /// <exception cref="ArgumentNullException"></exception>
        private DatabaseConnection(DatabaseServer databaseServer, bool useTransaction, DataIsolationLevel? isolationLevel = null, LockInstance? connLock = null)
        {
            if (databaseServer == null)
            {
                throw new ArgumentNullException(nameof(databaseServer));
            }
            DatabaseProvider = SixnetDataManager.GetDatabaseProvider(databaseServer.DatabaseType);
            DbConnection = DatabaseProvider.GetDbConnection(databaseServer);
            DatabaseServer = databaseServer;
            DataIsolationLevel = isolationLevel;
            UseTransaction = useTransaction;
            ConnectionLock = connLock;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Open connection
        /// </summary>
        public void Open()
        {
            ChangeState(DatabaseConnectionOperationType.Open);
        }

        /// <summary>
        /// Close connection
        /// </summary>
        public void Close()
        {
            ChangeState(DatabaseConnectionOperationType.Close);
        }

        /// <summary>
        /// Commit transaction
        /// </summary>
        public void Commit()
        {
            ChangeState(DatabaseConnectionOperationType.Commit);
        }

        /// <summary>
        /// Rollback transaction
        /// </summary>
        public void Rollback()
        {
            ChangeState(DatabaseConnectionOperationType.Rollback);
        }

        /// <summary>
        /// Change state
        /// </summary>
        DatabaseTransaction ChangeState(DatabaseConnectionOperationType operationType)
        {
            DatabaseTransaction newTransaction = null;
            switch (operationType)
            {
                case DatabaseConnectionOperationType.Open:
                    if (DbConnection.State != ConnectionState.Open)
                    {
                        DbConnection.Open();
                    }
                    newTransaction = OpenTransaction();
                    break;
                case DatabaseConnectionOperationType.Close:
                    if (DbConnection.State != ConnectionState.Closed)
                    {
                        Transaction?.Dispose();
                        DbConnection.Close();
                    }
                    Transaction = null;
                    ReleaseConnectionLock();
                    break;
                case DatabaseConnectionOperationType.Commit:
                    Transaction?.Commit();
                    Transaction = null;
                    newTransaction = OpenTransaction();
                    break;
                case DatabaseConnectionOperationType.Rollback:
                    Transaction?.Rollback();
                    newTransaction = OpenTransaction();
                    break;
            }
            return newTransaction;
        }

        /// <summary>
        /// Get database transaction
        /// </summary>
        /// <returns></returns>
        IDbTransaction GetDbTransaction()
        {
            var defaultIsolationLevel = DataIsolationLevel.HasValue
                ? SixnetDataManager.GetSystemIsolationLevel(DataIsolationLevel.Value)
                : SixnetDataManager.GetSystemIsolationLevel(SixnetDataManager.GetDataIsolationLevel(DatabaseServer.DatabaseType));
            return defaultIsolationLevel.HasValue
                   ? DbConnection.BeginTransaction(defaultIsolationLevel.Value)
                   : DbConnection.BeginTransaction();
        }

        /// <summary>
        /// Open a transaction
        /// </summary>
        /// <returns></returns>
        DatabaseTransaction OpenTransaction()
        {
            if (UseTransaction && Transaction == null)
            {
                var newDbTransaction = GetDbTransaction();
                Transaction = new DatabaseTransaction(newDbTransaction);
                return Transaction;
            }
            return null;
        }

        /// <summary>
        /// Release connection lock
        /// </summary>
        void ReleaseConnectionLock()
        {
            ConnectionLock?.Release();
            ConnectionLock = null;
        }

        /// <summary>
        /// Create a new database connection
        /// </summary>
        /// <param name="databaseServer">Database server</param>
        /// <param name="useTransaction">Whether use transaction</param>
        /// <param name="isolationLevel">Isolation level</param>
        /// <returns></returns>
        public static DatabaseConnection Create(DatabaseServer server, bool useTransaction, DataIsolationLevel? isolationLevel = null)
        {
            if (!server.UseSingleConnection() || !useTransaction)
            {
                return new DatabaseConnection(server, useTransaction, isolationLevel);
            }
            var connLock = SixnetLocker.GetCreateDatabaseConnectionLock(server);
            if (connLock != null)
            {
                return new DatabaseConnection(server, useTransaction, isolationLevel, connLock);
            }
            throw new InvalidOperationException($"{server.Name} is locked");
        }

        #endregion
    }

    #region Connection operation type

    /// <summary>
    /// Defines database connection operation type
    /// </summary>
    enum DatabaseConnectionOperationType
    {
        /// <summary>
        /// Open
        /// </summary>
        Open = 1,
        /// <summary>
        /// Close
        /// </summary>
        Close = 2,
        /// <summary>
        /// Commit
        /// </summary>
        Commit = 4,
        /// <summary>
        /// Rollback
        /// </summary>
        Rollback = 5,
    }

    #endregion
}
