using EZNEW.Develop.Command;
using EZNEW.Develop.Command.Modify;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.DataAccess;
using EZNEW.Develop.Entity;
using EZNEW.Framework.Extension;
using EZNEW.Framework.Fault;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EZNEW.Develop.UnitOfWork
{
    /// <summary>
    /// activation record
    /// </summary>
    public interface IActivationRecord
    {
        /// <summary>
        /// id
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// identity value
        /// </summary>
        string RecordIdentity { get; }

        /// <summary>
        /// parent record
        /// </summary>
        IActivationRecord ParentRecord { get; set; }

        /// <summary>
        /// operation
        /// </summary>
        ActivationOperation Operation { get; set; }

        /// <summary>
        /// identity value
        /// </summary>
        string IdentityValue { get; set; }

        /// <summary>
        /// query
        /// </summary>
        IQuery Query { get; set; }
        /// <summary>
        /// modify expression
        /// </summary>
        IModify ModifyExpression
        {
            get; set;
        }

        /// <summary>
        /// is obsolete
        /// </summary>
        bool IsObsolete { get; }

        /// <summary>
        /// add follow records
        /// </summary>
        /// <param name="records">records</param>
        void AddFollowRecords(params IActivationRecord[] records);

        /// <summary>
        /// get follow records
        /// </summary>
        /// <returns></returns>
        List<IActivationRecord> GetFollowRecords();

        /// <summary>
        /// get execute commands
        /// </summary>
        /// <returns></returns>
        ICommand GetExecuteCommand();

        /// <summary>
        /// obsolete
        /// </summary>
        void Obsolete();
    }
}
