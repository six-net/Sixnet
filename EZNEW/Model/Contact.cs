using System;

namespace EZNEW.Model
{
    /// <summary>
    /// Contact data type
    /// </summary>
    [Serializable]
    public struct Contact
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the EZNEW.ValueType.Contact
        /// </summary>
        /// <param name="phone">phone</param>
        /// <param name="mobile">mobile</param>
        /// <param name="email">email</param>
        /// <param name="qq">qq</param>
        /// <param name="msn">msn</param>
        /// <param name="weChat">weChat</param>
        /// <param name="weiBo">weiBo</param>
        public Contact(string phone = "", string mobile = "", string email = "", string qq = "", string msn = "", string weChat = "", string weiBo = "")
        {
            Phone = phone;
            Mobile = mobile;
            Email = email;
            QQ = qq;
            MSN = msn;
            WeChat = weChat;
            WeiBo = weiBo;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets phone
        /// </summary>
        public string Phone { get; }

        /// <summary>
        /// Gets mobile
        /// </summary>
        public string Mobile { get; }

        /// <summary>
        /// Gets email
        /// </summary>
        public string Email { get; }

        /// <summary>
        /// Gets QQ
        /// </summary>
        public string QQ { get; }

        /// <summary>
        /// Gets MSN
        /// </summary>
        public string MSN { get; }

        /// <summary>
        /// Gets WeChat
        /// </summary>
        public string WeChat { get; }

        /// <summary>
        /// Gets WeiBo
        /// </summary>
        public string WeiBo { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Compare two contact objects whether is equal
        /// </summary>
        /// <param name="contactOne">First contact</param>
        /// <param name="contactTwo">Second contact</param>
        /// <param name="comparison">String comparison</param>
        /// <returns>Whether is equal</returns>
        public static bool Equals(Contact contactOne, Contact contactTwo, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            return contactOne.Email.Equals(contactTwo.Email, comparison)
                && contactOne.Mobile.Equals(contactTwo.Mobile, comparison)
                && contactOne.MSN.Equals(contactTwo.MSN, comparison)
                && contactOne.Phone.Equals(contactTwo.Phone, comparison)
                && contactOne.QQ.Equals(contactTwo.QQ, comparison)
                && contactOne.WeChat.Equals(contactTwo.WeChat, comparison)
                && contactOne.WeiBo.Equals(contactTwo.WeiBo, comparison);
        }

        /// <summary>
        /// Override equals method
        /// </summary>
        /// <param name="data">Compare data</param>
        /// <returns>Return whether is equal</returns>
        public override bool Equals(object data)
        {
            return Equals(this, (Contact)data);
        }

        /// <summary>
        /// Gets hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return 0;
        }

        #endregion
    }
}
