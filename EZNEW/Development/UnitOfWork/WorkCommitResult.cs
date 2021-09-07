namespace EZNEW.Development.UnitOfWork
{
    /// <summary>
    /// Work commit result
    /// </summary>
    public class WorkCommitResult
    {
        /// <summary>
        /// Gets or sets the committed command count
        /// </summary>
        public int CommittedCommandCount
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the affected data count
        /// </summary>
        public int AffectedDataCount
        {
            get; set;
        }

        /// <summary>
        /// Indecates whether has command or commit success
        /// </summary>
        public bool EmptyOrSuccess
        {
            get
            {
                return Success || CommittedCommandCount - AllowEmptyCommandCount < 1;
            }
        }

        /// <summary>
        /// Indecates whether commit success
        /// </summary>
        public bool Success
        {
            get
            {
                return AffectedDataCount > 0;
            }
        }

        /// <summary>
        /// Gets or sets the allow none result command count
        /// </summary>
        /// <returns></returns>
        public int AllowEmptyCommandCount
        {
            get; set;
        }

        /// <summary>
        /// Get a empty commit result
        /// </summary>
        /// <returns>Return commit result</returns>
        public static WorkCommitResult Empty()
        {
            return new WorkCommitResult()
            {
                CommittedCommandCount = 0,
                AffectedDataCount = 0
            };
        }
    }
}
