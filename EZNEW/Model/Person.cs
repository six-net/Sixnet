using System;

namespace EZNEW.Model
{
    /// <summary>
    /// Person data type
    /// </summary>
    [Serializable]
    public class Person
    {
        /// <summary>
        /// Gets or sets name
        /// </summary>
        public ChineseText Name { get; set; }

        /// <summary>
        /// Gets or sets birth
        /// </summary>
        public Birth Birth { get; set; }

        /// <summary>
        /// Gets or sets contact
        /// </summary>
        public Contact Contact { get; set; }

        /// <summary>
        /// Gets or sets gender
        /// </summary>
        public Gender Gender { get; set; }

        /// <summary>
        /// Gets or sets id card
        /// </summary>
        public string IdentityCard { get; set; }
    }

    /// <summary>
    /// Defines gender
    /// </summary>
    [Serializable]
    public enum Gender
    {
        Man = 2,
        Woman = 4,
        Secrecy = 8
    }
}
