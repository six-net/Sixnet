namespace EZNEW.Develop.UnitOfWork
{
    /// <summary>
    /// Work commit result
    /// </summary>
    public class WorkCommitResult
    {
        /// <summary>
        /// Gets or sets the commit command count
        /// </summary>
        public int CommitCommandCount
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the executed data count
        /// </summary>
        public int ExecutedDataCount
        {
            get; set;
        }

        /// <summary>
        /// Gets whether was executed successful or empty command
        /// </summary>
        public bool EmptyResultOrSuccess
        {
            get
            {
                return CommitCommandCount - AllowEmptyResultCommandCount < 1 || ExecutedSuccess;
            }
        }

        /// <summary>
        /// Gets whether was executed successful
        /// </summary>
        public bool ExecutedSuccess
        {
            get
            {
                return ExecutedDataCount > 0;
            }
        }

        /// <summary>
        /// Gets or sets the allow none result command count
        /// </summary>
        /// <returns></returns>
        public int AllowEmptyResultCommandCount
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
                CommitCommandCount = 0,
                ExecutedDataCount = 0
            };
        }
    }
}
