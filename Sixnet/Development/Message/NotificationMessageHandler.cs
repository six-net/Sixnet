using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.Development.Message
{
    internal class NotificationMessageHandler : ISixnetMessageHandler
    {
        public void Send(SendMessageContext context)
        {
            throw new NotImplementedException();
        }

        public Task SendAsync(SendMessageContext context)
        {
            throw new NotImplementedException();
        }
    }
}
