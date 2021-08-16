﻿using System.Data;
using EZNEW.Development.Command;

namespace EZNEW.Data
{
    /// <summary>
    /// Database execution command
    /// </summary>
    public class DatabaseExecutionCommand
    {
        /// <summary>
        /// Gets or sets the command text
        /// </summary>
        public string CommandText { get; set; }

        /// <summary>
        /// Gets or sets the commandType
        /// </summary>
        public CommandType CommandType { get; set; }

        /// <summary>
        /// Gets or sets the parameters
        /// </summary>
        public CommandParameters Parameters { get; set; }

        /// <summary>
        /// Gets or sets whether force return value
        /// </summary>
        public bool ForceReturnValue { get; set; }

        /// <summary>
        /// Gets or sets whether has pre script
        /// </summary>
        public bool HasPreScript { get; set; }

        /// <summary>
        /// Gets or sets whether perform alone
        /// </summary>
        public bool PerformAlone
        {
            get
            {
                return HasPreScript || CommandType == CommandType.StoredProcedure || ForceReturnValue;
            }
        }
    }
}
