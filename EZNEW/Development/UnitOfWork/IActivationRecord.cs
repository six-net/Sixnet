using System.Collections.Generic;
using EZNEW.Development.Command;
using EZNEW.Data.Modification;
using EZNEW.Development.Query;

namespace EZNEW.Development.UnitOfWork
{
    /// <summary>
    /// Activation record contract
    /// </summary>
    public interface IActivationRecord
    {
        #region Properties

        /// <summary>
        /// Gets or sets the record id
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// Gets or the record identity value
        /// </summary>
        string RecordIdentity { get; }

        /// <summary>
        /// Gets or sets the parent record
        /// </summary>
        IActivationRecord ParentRecord { get; set; }

        /// <summary>
        /// Gets or sets the operation
        /// </summary>
        ActivationOperation Operation { get; set; }

        /// <summary>
        /// Gets or sets the identity value
        /// </summary>
        string IdentityValue { get; set; }

        /// <summary>
        /// Gets or sets the query object
        /// </summary>
        IQuery Query { get; set; }

        /// <summary>
        /// Gets or sets the modify expression
        /// </summary>
        IModification ModificationExpression { get; set; }

        /// <summary>
        /// Gets whether the record is obsolete
        /// </summary>
        bool IsObsolete { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Add follow record
        /// </summary>
        /// <param name="records">Activation records</param>
        void AddFollowRecord(params IActivationRecord[] records);

        /// <summary>
        /// Add follow record
        /// </summary>
        /// <param name="records">Activation records</param>
        void AddFollowRecord(IEnumerable<IActivationRecord> records);

        /// <summary>
        /// Get follow records
        /// </summary>
        /// <returns>Return follow records</returns>
        IEnumerable<IActivationRecord> GetFollowRecords();

        /// <summary>
        /// Get execution command
        /// </summary>
        /// <returns>Return execute command</returns>
        ICommand GetExecutionCommand();

        /// <summary>
        /// Obsolete
        /// </summary>
        void Obsolete();

        #endregion
    }
}
