using Sixnet.MQ;
using System;
using System.Collections.Generic;

namespace Sixnet.Net.Email
{
    /// <summary>
    /// Send email in-process queue email message
    /// </summary>
    [Serializable]
    public class InProcessQueueEmailMessage : InProcessQueueMessage
    {
        /// <summary>
        /// Gets or sets the email infos
        /// </summary>
        public IEnumerable<EmailInfo> EmailInfos { get; set; }

        /// <summary>
        /// Gets or sets the email account
        /// </summary>
        public EmailAccount EmailAccount { get; set; }

        /// <summary>
        /// Execute
        /// </summary>
        public override void Execute()
        {
            if (EmailAccount == null)
            {
                SixnetEmailer.ExecuteSendAsync(EmailInfos).Wait();
            }
            else
            {
                SixnetEmailer.ExecuteSendAsync(EmailAccount, EmailInfos).Wait();
            }
        }
    }
}
