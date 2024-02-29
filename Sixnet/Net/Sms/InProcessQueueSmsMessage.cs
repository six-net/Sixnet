using System;
using Sixnet.MQ;

namespace Sixnet.Net.Sms
{
    [Serializable]
    public class InProcessQueueSmsMessage : InProcessQueueMessage
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
        public override void Execute()
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
                sendSmsResult = SixnetSms.Send(SmsOptions);
            }
            else
            {
                sendSmsResult = SixnetSms.Send(SmsAccount, SmsOptions);
            }
            SmsOptions?.OnAsyncCallback?.Invoke(sendSmsResult);
        }
    }
}
