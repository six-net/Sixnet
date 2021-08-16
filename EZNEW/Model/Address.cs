using System;
using System.Collections.Generic;

namespace EZNEW.Model
{
    /// <summary>
    /// Address info
    /// </summary>
    [Serializable]
    public struct Address
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the EZNEW.ValueType.Address
        /// </summary>
        /// <param name="streetAddress">street address</param>
        /// <param name="regionList">region list</param>
        /// <param name="zipCode">zip code</param>
        public Address(string streetAddress, List<Region> regionList = null, string zipCode = "")
        {
            StreetAddress = streetAddress;
            Regions = regionList;
            ZipCode = zipCode;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets regions
        /// </summary>
        public List<Region> Regions { get; }

        /// <summary>
        /// Gets the street address
        /// </summary>
        public string StreetAddress { get; }

        /// <summary>
        /// Gets zip code
        /// </summary>
        public string ZipCode { get; }

        #endregion
    }

    /// <summary>
    /// region
    /// </summary>
    [Serializable]
    public struct Region
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the EZNEW.ValueType.Region
        /// </summary>
        /// <param name="name">region name</param>
        /// <param name="code">region code</param>
        public Region(string name, string code = "", int level = 0)
        {
            Name = name;
            Code = code;
            Level = level;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets region name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets region code
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Gets region level
        /// </summary>
        public int Level { get; }

        #endregion
    }
}
