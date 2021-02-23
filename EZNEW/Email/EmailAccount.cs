using System;

namespace EZNEW.Email
{
    /// <summary>
    /// Email account
    /// </summary>
    [Serializable]
    public class EmailAccount
    {
        #region Properties

        /// <summary>
        /// Gets or sets the send person name
        /// </summary>
        public string SendPersonName { get; set; }

        /// <summary>
        /// Gets or sets the send email address
        /// </summary>
        public string SendEmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the user name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the smtp address
        /// </summary>
        public string SmtpAddress { get; set; }

        /// <summary>
        /// Gets or sets the password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or set the port
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// Gets or sets whether enable ssl
        /// </summary>
        public bool EnableSsl { get; set; }

        /// <summary>
        /// Gets the email account identity key
        /// </summary>
        public string IdentityKey
        {
            get
            {
                return GetIdentityKey();
            }
        }

        #endregion

        public EmailAccount Clone()
        {
            return new EmailAccount()
            {
                SendPersonName = SendPersonName,
                SendEmailAddress = SendEmailAddress,
                UserName = UserName,
                SmtpAddress = SmtpAddress,
                Password = Password,
                Port = Port,
                EnableSsl = EnableSsl
            };
        }

        /// <summary>
        /// Gets the email account identity key
        /// </summary>
        string GetIdentityKey()
        {
            return string.Format("{0}_{1}", SmtpAddress, SendEmailAddress);
        }
    }
}
