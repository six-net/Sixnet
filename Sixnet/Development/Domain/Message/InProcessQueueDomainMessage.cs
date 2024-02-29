using System.Collections.Generic;
using Sixnet.Development.Work;
using Sixnet.MQ;

namespace Sixnet.Development.Domain.Message
{
    internal class InProcessQueueDomainMessage : InProcessQueueMessage
    {
        public List<SixnetMessageInfo> Messages { get; set; }

        public ISixnetMessageProvider MessageProvider { get; set; }

        public ISixnetWork Work { get; set; }

        public InProcessQueueDomainMessage(ISixnetMessageProvider messageProvider, List<SixnetMessageInfo> messages, ISixnetWork work)
        {
            Messages = messages;
            MessageProvider = messageProvider;
            Work = work;
        }

        public override void Execute()
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
