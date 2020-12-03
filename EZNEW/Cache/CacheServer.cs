using System.Collections.Generic;

namespace EZNEW.Cache
{
    /// <summary>
    /// Cache server
    /// </summary>
    public class CacheServer
    {
        private readonly SortedDictionary<string, dynamic> serverConfig = new SortedDictionary<string, dynamic>();
        CacheServerType serverType;
        bool initKey = false;
        string key = string.Empty;

        #region Properties

        /// <summary>
        /// Gets or sets the server type
        /// </summary>
        public CacheServerType ServerType
        {
            get
            {
                return serverType;
            }
            set
            {
                RemoveKey();
                serverType = value;
            }
        }

        /// <summary>
        /// Gets or sets the host
        /// </summary>
        public string Host
        {
            get
            {
                return GetConfigurationValue<string>("Host");
            }
            set
            {
                SetConfigurationValue("Host", value);
            }
        }

        /// <summary>
        /// Gets or sets the port
        /// </summary>
        public int Port
        {
            get
            {
                return GetConfigurationValue<int>("Port");
            }
            set
            {
                SetConfigurationValue("Port", value);
            }
        }

        /// <summary>
        /// Gets or sets the database
        /// </summary>
        public string Database { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user name
        /// </summary>
        public string UserName
        {
            get
            {
                return GetConfigurationValue<string>("UserName");
            }
            set
            {
                SetConfigurationValue("UserName", value);
            }
        }

        /// <summary>
        /// Gets or sets password
        /// </summary>
        public string Password
        {
            get
            {
                return GetConfigurationValue<string>("Pwd");
            }
            set
            {
                SetConfigurationValue("Pwd", value);
            }
        }

        /// <summary>
        /// Gets or sets whether allow admin
        /// </summary>
        public bool AllowAdmin
        {
            get
            {
                return GetConfigurationValue<bool>("AllowAdmin");
            }
            set
            {
                SetConfigurationValue("AllowAdmin", value);
            }
        }

        /// <summary>
        /// Gets or sets the connect timeout
        /// </summary>
        public int ConnectTimeout
        {
            get
            {
                return GetConfigurationValue<int>("ConnectTimeout");
            }
            set
            {
                SetConfigurationValue("ConnectTimeout", value);
            }
        }

        /// <summary>
        /// Gets or sets the client name
        /// </summary>
        public string ClientName
        {
            get
            {
                return GetConfigurationValue<string>("ClientName");
            }
            set
            {
                SetConfigurationValue("ClientName", value);
            }
        }

        /// <summary>
        /// Gets or sets whether resolve dns
        /// </summary>
        public bool ResolveDns
        {
            get
            {
                return GetConfigurationValue<bool>("ResolveDns");
            }
            set
            {
                SetConfigurationValue("ResolveDns", value);
            }
        }

        /// <summary>
        /// Gets or sets whether enable ssl
        /// </summary>
        public bool SSL
        {
            get
            {
                return GetConfigurationValue<bool>("SSL");
            }
            set
            {
                SetConfigurationValue("SSL", value);
            }
        }

        /// <summary>
        /// Gets or sets the ssl host
        /// </summary>
        public string SSLHost
        {
            get
            {
                return GetConfigurationValue<string>("SSLHost");
            }
            set
            {
                SetConfigurationValue("SSLHost", value);
            }
        }

        /// <summary>
        /// Gets or sets the sync timeout(ms)
        /// </summary>
        public int SyncTimeout
        {
            get
            {
                return GetConfigurationValue<int>("SyncTimeout");
            }
            set
            {
                SetConfigurationValue("SyncTimeout", value);
            }
        }

        /// <summary>
        /// Gets or sets the tiebreaker(master key)
        /// </summary>
        public string TieBreaker
        {
            get
            {
                return GetConfigurationValue<string>("TieBreaker");
            }
            set
            {
                SetConfigurationValue("TieBreaker", value);
            }
        }

        /// <summary>
        /// Gets or sets the write buffer
        /// </summary>
        public int WriteBuffer
        {
            get
            {
                return GetConfigurationValue<int>("WriteBuffer");
            }
            set
            {
                SetConfigurationValue("WriteBuffer", value);
            }
        }

        /// <summary>
        /// Gets or sets the identity key
        /// </summary>
        public string IdentityKey
        {
            get
            {
                return GetServerKey();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set server configuration value
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="value">Value</param>
        void SetConfigurationValue<T>(string name, T value)
        {
            RemoveKey();
            if (serverConfig.ContainsKey(name))
            {
                serverConfig[name] = value;
            }
            else
            {
                serverConfig.Add(name, value);
            }
        }

        /// <summary>
        /// Get configuration value
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Return the configuration value</returns>
        T GetConfigurationValue<T>(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || !serverConfig.ContainsKey(name))
            {
                return default(T);
            }
            return serverConfig[name];
        }

        /// <summary>
        /// Get server identity key
        /// </summary>
        /// <returns>Return identity key</returns>
        string GetServerKey()
        {
            if (initKey)
            {
                return key;
            }
            if (serverConfig == null)
            {
                return string.Empty;
            }
            List<string> valueItems = new List<string>();
            foreach (var configItem in serverConfig)
            {
                valueItems.Add(string.Format("{0}_{1}", configItem.Key, configItem.Value));
            }
            key = string.Join(",", valueItems);
            initKey = true;
            return key;
        }

        /// <summary>
        /// Clear identity key
        /// </summary>
        void RemoveKey()
        {
            initKey = false;
            key = string.Empty;
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="obj">Other object</param>
        /// <returns>Return whether equal</returns>
        public override bool Equals(object obj)
        {
            CacheServer objServer = obj as CacheServer;
            if (objServer == null)
            {
                return false;
            }
            return objServer.IdentityKey == IdentityKey;
        }

        /// <summary>
        /// Get hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return IdentityKey.GetHashCode();
        }

        #endregion
    }
}
