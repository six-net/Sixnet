// "Company © 2025. All rights reserved."

using Sixnet.Component.Retry;
using Sixnet.Exceptions;

namespace Sixnet.Development.Work
{
    /// <summary>
    /// Unit of work options
    /// </summary>
    public class SixnetUnitOfWorkOptions
    {
        /// <summary>
        /// Gets or sets the retry times
        /// Default is 1
        /// </summary>
        public int RetryTimes { get; set; } = 1;

        /// <summary>
        /// Determines whether to retry
        /// </summary>
        public Func<RetryContext, bool> ToRetry { get; set; }

        /// <summary>
        /// Whether disable default sql retry
        /// </summary>
        public bool DisableDefaultSqlRetry { get; set; }

        /// <summary>
        /// Whether not retry
        /// </summary>
        public bool NotRetry { get; set; }

        bool DefaultSqlToRetry(RetryContext context)
        {
            var exp = context.Exception;
            return (exp is SixnetSqlAlreadExistsException)
                || (exp is AggregateException aggExp && (aggExp.InnerExceptions?.Any(c => c is SixnetSqlAlreadExistsException) ?? false));
        }

        internal bool AllowRetry(RetryContext context)
        {
            return (!DisableDefaultSqlRetry && DefaultSqlToRetry(context))
                || (ToRetry != null && ToRetry(context));
        }
    }
}
