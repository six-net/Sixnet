using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Serialization
{
    #region Json naming policy

    /// <summary>
    /// Defines json property naming policy
    /// </summary>
    [Serializable]
    public enum JsonPropertyNamingPolicy
    {
        Default = 0,
        Original = 2,
        CamelCase = 4,
        Uppercase = 8,
        Lowercase = 16
    }

    #endregion
}
