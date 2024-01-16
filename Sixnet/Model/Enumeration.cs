using System;

namespace Sixnet.Model
{
    #region Constellation

    /// <summary>
    /// Constellation
    /// </summary>
    [Serializable]
    public enum Constellation
    {
        Aquarius = 120218,
        Pisces = 219320,
        Aries = 321419,
        Taurus = 420520,
        Gemini = 521621,
        Cacer = 622722,
        Leo = 723822,
        Virgo = 823922,
        Libra = 9231023,
        Scorpio = 10241122,
        Sagittarius = 11231221,
        Capricom = 12220119
    }

    #endregion

    #region ObjectCloneMethod

    /// <summary>
    /// Defines object clone method
    /// </summary>
    [Serializable]
    public enum ObjectCloneMethod
    {
        Binary = 2,
        Json = 4
    }

    #endregion
}
