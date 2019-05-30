using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.UnitOfWork
{
    /// <summary>
    /// commit result
    /// </summary>
    public class CommitResult
    {
        #region propertys

        /// <summary>
        /// commit command count
        /// </summary>
        public int CommitCommandCount
        {
            get; set;
        }

        /// <summary>
        /// executed data count
        /// </summary>
        public int ExecutedDataCount
        {
            get; set;
        }

        /// <summary>
        /// executed success or empty command
        /// </summary>
        public bool NoneCommandOrSuccess
        {
            get
            {
                return CommitCommandCount-AllowNoneResultCommandCount <= 0 || ExecutedSuccess;
            }
        }

        /// <summary>
        /// executed success
        /// </summary>
        public bool ExecutedSuccess
        {
            get
            {
                return ExecutedDataCount > 0;
            }
        }

        /// <summary>
        /// allow none result command count
        /// </summary>
        /// <returns></returns>
        public int AllowNoneResultCommandCount
        {
            get;set;
        }

        #endregion
    }
}
