//using Sixnet.Development.Work;
//using Sixnet.MQ;
//using System.Collections.Generic;

//namespace Sixnet.Development.Message
//{
//    internal class InProcessQueueTemplateMessage : InProcessQueueMessage
//    {
//        public List<MessageInfo> Messages { get; set; }

//        public ISixnetWork Work { get; set; }

//        public InProcessQueueTemplateMessage(List<MessageInfo> messages, ISixnetWork work)
//        {
//            Messages = messages;
//            Work = work;
//        }

//        public override void Execute()
//        {
//            if (!Messages.IsNullOrEmpty())
//            {
//                var privider = SixnetMessager.GetMessageProvider();
//                privider.Send(new SendMessageParameter()
//                {
//                    Messages = Messages
//                });
//            }
//        }
//    }
//}
