using EZNEW.Develop.Command;
using EZNEW.Develop.Domain.Repository.Warehouse;
using EZNEW.Develop.Entity;
using EZNEW.Framework.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.UnitOfWork
{
    /// <summary>
    /// IUnitOfWork
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// commit success callback event
        /// </summary>
        event Action CommitSuccessCallbackEvent;

        /// <summary>
        /// Commit Command
        /// </summary>
        /// <returns></returns>
        CommitResult Commit();

        /// <summary>
        /// Commit Command
        /// </summary>
        /// <returns></returns>
        Task<CommitResult> CommitAsync();

        /// <summary>
        /// add command
        /// </summary>
        /// <param name="cmds">commands</param>
        void AddCommand(params ICommand[] cmds);

        /// <summary>
        /// add activation operation
        /// </summary>
        /// <param name="records"></param>
        void AddActivation(params IActivationRecord[] records);

        /// <summary>
        /// command count
        /// </summary>
        int CommandCount { get; }

        /// <summary>
        /// work id
        /// </summary>
        string WorkId { get; }

        /// <summary>
        /// get new record id
        /// </summary>
        /// <returns></returns>
        int GetRecordId();

        /// <summary>
        /// get command id
        /// </summary>
        /// <returns></returns>
        int GetCommandId();

        /// <summary>
        /// get data warehouse by entity type
        /// </summary>
        /// <returns></returns>
        DataWarehouse<ET> GetWarehouse<ET>() where ET : BaseEntity<ET>;

        /// <summary>
        /// save warehouse
        /// </summary>
        /// <param name="entityType">entity type</param>
        /// <param name="warehouse">warehouse</param>
        void SaveWarehouse<ET>(DataWarehouse<ET> warehouse) where ET : BaseEntity<ET>;
    }
}
