using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.Queue.Message
{
    /// <summary>
    /// message group
    /// </summary>
    public struct MessageGroup
    {
        #region field

        string _groupCode;

        #endregion

        #region propertys

        /// <summary>
        /// group code
        /// </summary>
        public string GroupCode
        {
            get
            {
                return _groupCode;
            }
        }

        /// <summary>
        /// group identity
        /// </summary>
        public string GroupIdentity
        {
            get
            {
                return _groupCode;
            }
        }

        #endregion

        public MessageGroup(string groupCode)
        {
            _groupCode = groupCode;
        }

        #region method

        /// <summary>
        /// compare two MsgGroup object whether is equal
        /// </summary>
        /// <param name="one">first group</param>
        /// <param name="two">second group</param>
        /// <returns>whether is equal</returns>
        public static bool operator ==(MessageGroup one, MessageGroup two)
        {
            return Equals(one, two);
        }

        /// <summary>
        /// compare two MsgGroup object whether is not equal
        /// </summary>
        /// <param name="one">first group</param>
        /// <param name="two">second group</param>
        /// <returns>whether is not equal</returns>
        public static bool operator !=(MessageGroup one, MessageGroup two)
        {
            return !Equals(one, two);
        }

        /// <summary>
        /// compare two MsgGroup object whether is equal
        /// </summary>
        /// <param name="obj">other MsgGroup object</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return Equals(this, (MessageGroup)obj);
        }

        /// <summary>
        /// compare two MsgGroup object whether is equal
        /// </summary>
        /// <param name="groupOne">first group</param>
        /// <param name="groupTwo">second group</param>
        /// <returns>whether is equal</returns>
        public static bool Equals(MessageGroup groupOne, MessageGroup groupTwo)
        {
            return groupOne.GroupCode == groupTwo.GroupCode;
        }

        public override int GetHashCode()
        {
            return _groupCode.GetHashCode();
        }

        #endregion
    }
}
