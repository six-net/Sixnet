using EZNEW.Framework.Extension;
using EZNEW.Framework.Internal.MQ;
using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.Command
{
    /// <summary>
    /// command callback internal message item
    /// </summary>
    public class CommandCallbackInternalMessageItem : IInternalMessageQueueItem
    {

        List<Tuple<CommandCallbackOperation, CommandCallbackParameter>> operations = null;

        internal CommandCallbackInternalMessageItem(List<Tuple<CommandCallbackOperation, CommandCallbackParameter>> operations)
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
