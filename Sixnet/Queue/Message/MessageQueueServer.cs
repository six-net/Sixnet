using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.Queue.Message
{
    /// <summary>
    /// Defines message queue server
    /// </summary>
    public class MessageQueueServer
    {
        #region Properties

        /// <summary>
        /// Gets or sets the server type
        /// </summary>
        public MessageQueueServerType ServerType { get; set; }

        /// <summary>
        /// Gets or sets the user name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the host
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the exchange name
        /// </summary>
        public string ExchangeName { get; set; }

        /// <summary>
        /// Gets or sets the exchange type
        /// </summary>
        public MessageQueueExchangeType ExchangeType { get; set; }

        /// <summary>
        /// Gets or sets the routing key
        /// </summary>
        public string RoutingKey { get; set; }

        /// <summary>
        /// Gets the identity key
        /// </summary>
        public string IdentityKey
        {
            get
            {
                return GetIdentityKey();
            }
        }

        #endregion

        #region Gets server identity key

        /// <summary>
        /// Gets server identity key
        /// </summary>
        /// <returns></returns>
        string GetIdentityKey()
        {
            return string.Format("{0}_{1}_{2}_{3}", Host, Port, UserName, Password);
        }

        #endregion
    }
}
