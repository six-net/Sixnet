using EZNEW.Framework.Extension;
using EZNEW.Framework.Internal.MQ;
using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.Command
{
    /// <summary>
    /// command before operation internal message item
    /// </summary>
    public class CommandBeforeOperationInternalMessageItem : IInternalMessageQueueItem
    {
        List<Tuple<CommandBeforeExecuteOperation, CommandBeforeExecuteParameter>> operations = null;

        internal CommandBeforeOperationInternalMessageItem(List<Tuple<CommandBeforeExecuteOperation, CommandBeforeExecuteParameter>> operations)
        {
            this.operations = operations;
        }

        public void Run()
        {
            if (operations.IsNullOrEmpty())
            {
                return;
            }
            operations.ForEach(op =>
            {
                op.Item1(op.Item2);
            });
        }
    }
}
