using System;
using System.Collections.Generic;
using System.Text;
using EZNEW.Queue;

namespace EZNEW.Sms
{
    [Serializable]
    public class InternalQueueSendSmsItem : IInternalQueueItem
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
        /// Run command
        /// </summary>
        public void Execute()
        {
            if (SmsOptions == null)
            {
                return;
            }
            SmsOptions.Asynchronously = false;
            if (SmsAccount == null)
            {
                SmsManager.Send(SmsOptions);
            }
            else
            {
                SmsManager.Send(SmsAccount, SmsOptions);
            }
        }
    }
}
