using System;

namespace EZNEW.Data
{
    /// <summary>
    /// Defines database server type
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
        //Access = 170
    }

    /// <summary>
    /// Defines calculation operator
    /// </summary>
    [Serializable]
    public enum CalculationOperator
    {
        Add,
        Subtract,
        Multiply,
        Divide
    }
}
