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
        /// Gets or sets the database server name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the server identity value
        /// </summary>
        public string IdentityValue { get; private set; } = string.Empty;

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
                InitIdentityValue();
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
                InitIdentityValue();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Init identity value
        /// </summary>
        void InitIdentityValue()
        {
            IdentityValue = string.Format("{0}_{1}", (int)serverType, connectionString);
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
            return IdentityValue == otherServerInfo.IdentityValue;
        }

        public override int GetHashCode()
        {
            return IdentityValue.GetHashCode();
        }

        #endregion
    }
}
