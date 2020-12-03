using System;

namespace EZNEW.Data
{
    /// <summary>
    /// Defines server type
    /// </summary>
    [Serializable]
    public enum DatabaseServerType
    {
        Others = 0,
        SQLServer = 110,
        MySQL = 120,
        Oracle = 130,
        //MongoDB = 140,
        SQLite = 150,
        PostgreSQL = 160,
        Access = 170
    }
}
