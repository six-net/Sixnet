using System;
using System.Collections.Generic;
using EZNEW.Internal.MessageQueue;

namespace EZNEW.Email
{
    /// <summary>
    /// Send email internal message command
    /// </summary>
    [Serializable]
    public class SendEmailInternalMessageCommand : IInternalMessageQueueCommand
    {
        /// <summary>
        /// Gets or sets the email send infos
        /// </summary>
        public IEnumerable<EmailSendInfo> SendInfos { get; set; }

        /// <summary>
        /// Gets or sets the email account
        /// </summary>
        public EmailAccount EmailAccount { get; set; }

        /// <summary>
        /// Run command
        /// </summary>
        public void Run()
        {
            if (EmailAccount == null)
            {
                EmailManager.ExecuteSendAsync(SendInfos).Wait();
            }
            else
            {
                EmailManager.ExecuteSendAsync(EmailAccount, SendInfos).Wait();
            }
        }
    }
}
