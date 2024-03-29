﻿using System;
using System.Collections.Generic;
using Sixnet.Queue.Internal;

namespace Sixnet.Net.Email
{
    /// <summary>
    /// Send email internal queue item
    /// </summary>
    [Serializable]
    public class InternalQueueSendEmailItem : IInternalQueueTask
    {
        /// <summary>
        /// Gets or sets the email send infos
        /// </summary>
        public IEnumerable<SendEmailOptions> Datas { get; set; }

        /// <summary>
        /// Gets or sets the email account
        /// </summary>
        public EmailAccount EmailAccount { get; set; }

        /// <summary>
        /// Execute
        /// </summary>
        public void Execute()
        {
            if (EmailAccount == null)
            {
                EmailManager.ExecuteSendAsync(Datas).Wait();
            }
            else
            {
                EmailManager.ExecuteSendAsync(EmailAccount, Datas).Wait();
            }
        }
    }
}
