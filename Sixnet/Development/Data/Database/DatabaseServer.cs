using Sixnet.Exceptions;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Database server
    /// </summary>
    public class DatabaseServer
    {
        #region Properties

        /// <summary>
        /// Gets or sets the database server name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the database server role
        /// </summary>
        public DatabaseServerRole Role { get; set; } = DatabaseServerRole.Default;

        /// <summary>
        /// Gets or sets connection string
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets database type
        /// </summary>
        public DatabaseType DatabaseType { get; set; }

        #endregion

        #region Methods

        public string GetServerIdentityValue()
        {
            return $"{DatabaseType}_{ConnectionString}";
        }

        public override bool Equals(object otherServer)
        {
            if (otherServer == null)
            {
                return false;
            }
            if (otherServer is not DatabaseServer otherServerInfo)
            {
                return false;
            }
            return GetServerIdentityValue() == otherServerInfo.GetServerIdentityValue();
        }

        public override int GetHashCode()
        {
            return GetServerIdentityValue().GetHashCode();
        }

        /// <summary>
        /// Whether use single connection
        /// </summary>
        /// <returns></returns>
        public virtual bool UseSingleConnection()
        {
            return DatabaseType == DatabaseType.SQLite;
        }

        #endregion
    }
}
