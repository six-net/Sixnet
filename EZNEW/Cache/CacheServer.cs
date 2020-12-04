using System;
using System.Collections.Generic;

namespace EZNEW.Cache
{
    /// <summary>
    /// Cache server
    /// </summary>
    public class CacheServer
    {
        public CacheServer(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("Cache server name is null or empty");
            }
            Name = name;
        }

        public CacheServer(string name, CacheServerType serverType) : this(name)
        {
            ServerType = serverType;
        }

        #region Properties

        /// <summary>
        /// Gets or sets the server type
        /// </summary>
        public CacheServerType ServerType { get; set; }

        /// <summary>
        /// Gets the server name
        /// Every cache ser should be set a unique name
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets or sets the end points
        /// </summary>
        public List<CacheEndPoint> EndPoints { get; set; }

        /// <summary>
        /// Gets or sets the databases
        /// </summary>
        public List<string> Databases { get; set; }

        /// <summary>
        /// Gets or sets the user name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets whether allow admin
        /// </summary>
        public bool AllowAdmin { get; set; }

        /// <summary>
        /// Gets or sets the connect timeout
        /// </summary>
        public int ConnectTimeout { get; set; }

        /// <summary>
        /// Gets or sets the client name
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// Gets or sets whether resolve dns
        /// </summary>
        public bool ResolveDns { get; set; }

        /// <summary>
        /// Gets or sets whether enable ssl
        /// </summary>
        public bool SSL { get; set; }

        /// <summary>
        /// Gets or sets the ssl host
        /// </summary>
        public string SSLHost { get; set; }

        /// <summary>
        /// Gets or sets the sync timeout(ms)
        /// </summary>
        public int SyncTimeout { get; set; }

        /// <summary>
        /// Gets or sets the tiebreaker(master key)
        /// </summary>
        public string TieBreaker { get; set; }

        /// <summary>
        /// Gets or sets the write buffer
        /// </summary>
        public int WriteBuffer { get; set; }

        #endregion

        #region Methods

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
            return string.Equals(objServer.Name, Name, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Get hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        #endregion
    }
}
