namespace Sixnet.MQ
{
    /// <summary>
    /// Defines message queue server
    /// </summary>
    public class MessageQueueServer
    {
        #region Properties

        /// <summary>
        /// Gets or sets the server name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the server type
        /// </summary>
        public MessageQueueType Type { get; set; }

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

        #endregion

        #region Methods

        public override int GetHashCode()
        {
            return Name?.GetHashCode() ?? 0;
        }

        public override bool Equals(object obj)
        {
            if (obj is MessageQueueServer objServer)
            {
                return objServer.Name == Name;
            }
            return false;
        }

        #endregion
    }
}
