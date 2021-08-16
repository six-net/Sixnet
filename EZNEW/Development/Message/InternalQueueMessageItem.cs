using System;
using System.Collections.Generic;
using System.Text;
using EZNEW.Development.UnitOfWork;
using EZNEW.Queue;

namespace EZNEW.Development.Message
{
    internal class InternalQueueMessageItem : IInternalQueueItem
    {
        public List<MessageInfo> Messages { get; set; }

        public IMessageProvider MessageProvider { get; set; }

        public IWork Work { get; set; }

        public InternalQueueMessageItem(IMessageProvider messageProvider, List<MessageInfo> messages, IWork work)
        {
            Messages = messages;
            MessageProvider = messageProvider;
            Work = work;
        }

        public void Execute()
        {
            if (Messages.IsNullOrEmpty() || MessageProvider == null)
            {
                return;
            }
            MessageProvider.Send(new SendMessageOptions()
            {
                Messages = Messages
            });
        }
    }
}
