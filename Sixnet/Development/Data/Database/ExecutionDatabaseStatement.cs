using System.Data;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Database execution statement
    /// </summary>
    public class ExecutionDatabaseStatement : DatabaseStatement
    {
        /// <summary>
        /// Indicates whether must arrect data
        /// </summary>
        public bool MustAffectData { get; set; }

        /// <summary>
        /// Indicates whether has pre script
        /// </summary>
        public bool HasPreScript { get; set; }

        /// <summary>
        /// Incr script
        /// </summary>
        public string IncrScript { get; set; }

        /// <summary>
        /// Indicates whether perform alone
        /// </summary>
        public bool PerformAlone
        {
            get
            {
                return HasPreScript || ScriptType != CommandType.Text || MustAffectData || !string.IsNullOrWhiteSpace(IncrScript);
            }
        }
    }
}
