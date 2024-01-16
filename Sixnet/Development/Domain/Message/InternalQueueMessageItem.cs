using System.Collections.Generic;
using Sixnet.Development.Work;
using Sixnet.Queue.Internal;

namespace Sixnet.Development.Message
{
    internal class InternalQueueMessageItem : IInternalQueueTask
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
