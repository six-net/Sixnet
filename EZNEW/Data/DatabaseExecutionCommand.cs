using System.Data;
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
        /// Indicates whether must arrected data
        /// </summary>
        public bool MustAffectedData { get; set; }

        /// <summary>
        /// Indicates whether has pre script
        /// </summary>
        public bool HasPreScript { get; set; }

        /// <summary>
        /// Indicates whether perform alone
        /// </summary>
        public bool PerformAlone
        {
            get
            {
                return HasPreScript || CommandType == CommandType.StoredProcedure || MustAffectedData;
            }
        }
    }
}
