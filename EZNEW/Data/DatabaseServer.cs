namespace EZNEW.Data
{
    /// <summary>
    /// Database server info
    /// </summary>
    public class DatabaseServer
    {
        #region Fields

        /// <summary>
        /// Connection string
        /// </summary>
        string connectionString = string.Empty;

        /// <summary>
        /// Server type
        /// </summary>
        DatabaseServerType serverType;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the server key
        /// </summary>
        public string Key { get; private set; } = string.Empty;

        /// <summary>
        /// Gets or sets connection string
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return connectionString;
            }
            set
            {
                connectionString = value;
                InitKey();
            }
        }

        /// <summary>
        /// Gets or sets server type
        /// </summary>
        public DatabaseServerType ServerType
        {
            get
            {
                return serverType;
            }
            set
            {
                serverType = value;
                InitKey();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Init server key
        /// </summary>
        void InitKey()
        {
            Key = string.Format("{0}_{1}", (int)serverType, connectionString);
        }

        public override bool Equals(object otherServer)
        {
            if (otherServer == null)
            {
                return false;
            }
            DatabaseServer otherServerInfo = otherServer as DatabaseServer;
            if (otherServerInfo == null)
            {
                return false;
            }
            return Key == otherServerInfo.Key;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        #endregion
    }
}
