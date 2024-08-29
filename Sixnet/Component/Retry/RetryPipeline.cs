using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.Component.Retry
{
    /// <summary>
    /// Retry pipeline
    /// </summary>
    internal class RetryPipeline
    {
        /// <summary>
        /// Gets or sets the action
        /// </summary>
        public Action Action { get; set; }

        /// <summary>
        /// Gets or sets the action
        /// </summary>
        public Func<Task> ActionAsync { get; set; }

        /// <summary>
        /// Gets or sets the retry times
        /// </summary>
        public int Times { get; set; }

        /// <summary>
        /// Gets or sets when execute retry
        /// </summary>
        public Func<RetryContext, bool> When { get; set; }

        /// <summary>
        /// Before retry
        /// </summary>
        public Action<RetryContext> OnBeforeRetry { get; set; }

        /// <summary>
        /// Execute
        /// </summary>
        public void Execute()
        {
            var allowTimes = Times < 0 ? 0 : Times;
            var retriedTimes = 0;
            var ctx = new RetryContext();
            do
            {
                try
                {
                    Action?.Invoke();
                    break;
                }
                catch (Exception ex)
                {
                    ctx.Exception = ex;
                    var retry = When?.Invoke(ctx) ?? false;
                    if (!retry || retriedTimes >= allowTimes)
                    {
                        throw ex;
                    }
                    else
                    {
                        OnBeforeRetry?.Invoke(ctx);
                    }
                }
                retriedTimes++;
            } while (retriedTimes <= allowTimes);
        }

        /// <summary>
        /// Execute
        /// </summary>
        public async Task ExecuteAsync()
        {
            var allowTimes = Times < 0 ? 0 : Times;
            var retriedTimes = 0;
            var ctx = new RetryContext();
            do
            {
                try
                {
                    if (ActionAsync != null)
                    {
                        await ActionAsync().ConfigureAwait(false);
                    }
                    break;
                }
                catch (Exception ex)
                {
                    ctx.Exception = ex;
                    var retry = When?.Invoke(ctx) ?? false;
                    if (!retry || retriedTimes >= allowTimes)
                    {
                        throw ex;
                    }
                    else
                    {
                        OnBeforeRetry?.Invoke(ctx);
                    }
                }
                retriedTimes++;
            } while (retriedTimes <= allowTimes);
        }
    }

    /// <summary>
    /// Retry pipeline
    /// </summary>
    internal class RetryPipeline<T>
    {
        /// <summary>
        /// Gets or sets the func
        /// </summary>
        public Func<T> Func { get; set; }

        /// <summary>
        /// Gets or sets the func
        /// </summary>
        public Func<Task<T>> FuncAsync { get; set; }

        /// <summary>
        /// Gets or sets the retry times
        /// </summary>
        public int Times { get; set; }

        /// <summary>
        /// Gets or sets when execute retry
        /// </summary>
        public Func<RetryContext, bool> When { get; set; }

        /// <summary>
        /// Before retry
        /// </summary>
        public Action<RetryContext> OnBeforeRetry { get; set; }

        /// <summary>
        /// Execute
        /// </summary>
        public T Execute()
        {
            var allowTimes = Times < 0 ? 0 : Times;
            var retriedTimes = 0;
            var ctx = new RetryContext();
            T res = default;
            do
            {
                try
                {
                    if (Func != null)
                    {
                        res = Func();
                    }
                    break;
                }
                catch (Exception ex)
                {
                    ctx.Exception = ex;
                    var retry = When?.Invoke(ctx) ?? false;
                    if (!retry || retriedTimes >= allowTimes)
                    {
                        throw ex;
                    }
                    else
                    {
                        OnBeforeRetry?.Invoke(ctx);
                    }
                }
                retriedTimes++;
            } while (retriedTimes <= allowTimes);
            return res;
        }

        /// <summary>
        /// Execute
        /// </summary>
        public async Task<T> ExecuteAsync()
        {
            var allowTimes = Times < 0 ? 0 : Times;
            var retriedTimes = 0;
            var ctx = new RetryContext();
            T res = default;
            do
            {
                try
                {
                    if (FuncAsync != null)
                    {
                        res = await FuncAsync().ConfigureAwait(false);
                    }
                    break;
                }
                catch (Exception ex)
                {
                    ctx.Exception = ex;
                    var retry = When?.Invoke(ctx) ?? false;
                    if (!retry || retriedTimes >= allowTimes)
                    {
                        throw ex;
                    }
                    else
                    {
                        OnBeforeRetry?.Invoke(ctx);
                    }
                }
                retriedTimes++;
            } while (retriedTimes <= allowTimes);
            return res;
        }
    }
}
