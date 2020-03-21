using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.DataAccess
{
    /// <summary>
    /// data isolation level
    /// </summary>
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
}
