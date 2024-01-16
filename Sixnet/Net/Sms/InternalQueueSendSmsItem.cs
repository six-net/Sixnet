using System;
using Sixnet.Queue.Internal;

namespace Sixnet.Net.Sms
{
    [Serializable]
    public class InternalQueueSendSmsItem : IInternalQueueTask
    {
        /// <summary>
        /// Gets or sets the sms options
        /// </summary>
        public SendSmsOptions SmsOptions { get; set; }

        /// <summary>
        /// Gets or sets the sms account
        /// </summary>
        public SmsAccount SmsAccount { get; set; }

        /// <summary>
        /// Execute
        /// </summary>
        public void Execute()
        {
            if (SmsOptions == null)
            {
                return;
            }
            SmsOptions.Asynchronously = false;
            if (SmsOptions.OnAsyncBeforeSend != null && !SmsOptions.OnAsyncBeforeSend(SmsOptions))
            {
                return;
            }
            SendSmsResult sendSmsResult;
            if (SmsAccount == null)
            {
                sendSmsResult = SmsManager.Send(SmsOptions);
            }
            else
            {
                sendSmsResult = SmsManager.Send(SmsAccount, SmsOptions);
            }
            SmsOptions?.OnAsyncCallback?.Invoke(sendSmsResult);
        }
    }
}
