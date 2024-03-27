using System;

namespace Sixnet.Serialization.Json
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
