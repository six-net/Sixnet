using Sixnet.Exceptions;
using System;
using System.Collections.Generic;

namespace Sixnet.Cache
{
    /// <summary>
    /// Cache server
    /// </summary>
    public class CacheServer
    {
        #region Properties

        /// <summary>
        /// Gets or sets the server type
        /// </summary>
        public CacheServerType Type { get; set; }

        /// <summary>
        /// Gets the server name
        /// Every cache ser should be set a unique name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the end points
        /// </summary>
        public List<CacheEndPoint> EndPoints { get; set; }

        /// <summary>
        /// Gets or sets the database
        /// </summary>
        public string Database { get; set; }

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
            if (obj is not CacheServer objServer)
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
            return Name?.GetHashCode() ?? 0;
        }

        #endregion
    }
}
