// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Update database process
    /// </summary>
    public class UpdateDatabaseProcess
    {
        /// <summary>
        /// Record
        /// </summary>
        public ISixnetDatabaseUpdateRecord Record { get; set; }

        /// <summary>
        /// State
        /// </summary>
        public UpdateDatabaseProcessState State { get; set; }

        /// <summary>
        /// Current version
        /// </summary>
        public Version CurrentVersion { get; set; }

        /// <summary>
        /// Current record id
        /// </summary>
        public long? CurrentRecordId { get; set; }

        /// <summary>
        /// Update parameter
        /// </summary>
        public SixnetUpdateDatabaseParameter Parameter { get; set; }

        /// <summary>
        /// Exception
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message {  get; set; }

        public static UpdateDatabaseProcess Create(UpdateDatabaseProcessState state, SixnetUpdateDatabaseParameter parameter, ISixnetDatabaseUpdateRecord record = null
            , Version currentVersion = null, long? currentRecordId = null, Exception ex = null, string message = null)
        {
            return new UpdateDatabaseProcess()
            {
                Record = record,
                State = state,
                CurrentVersion = currentVersion,
                CurrentRecordId = currentRecordId,
                Parameter = parameter,
                Exception = ex,
                Message = message
            };
        }
    }

    /// <summary>
    /// Update database process state
    /// </summary>
    public enum UpdateDatabaseProcessState
    {
        Begin = 1,
        End = 5,
        Update = 10,
        UpdateFinished = 15,
        Rollback = 20,
        RollbackFinished = 25,
        BeginGettingCurrentInfo = 30,
        EndGettingCurrentInfo = 35,
        NoneRecords = 40,
        Message = 100,
        ErrorRecord = 1000,
        Error = 5000
    }
}
