using System;
using System.Collections.Generic;
using System.Linq;

namespace EZNEW.Model
{
    /// <summary>
    /// Birth date type
    /// </summary>
    [Serializable]
    public struct Birth
    {
        #region Fields

        /// <summary>
        /// age
        /// </summary>
        private readonly int age;

        /// <summary>
        /// constellation
        /// </summary>
        private readonly Constellation constellation;

        /// <summary>
        /// zero time span
        /// </summary>
        static readonly TimeSpan Zero = TimeSpan.FromMilliseconds(0);

        /// <summary>
        /// constellations
        /// </summary>
        private static readonly Dictionary<Constellation, Tuple<DateTimeOffset, DateTimeOffset>> constellationDictionary = new Dictionary<Constellation, Tuple<DateTimeOffset, DateTimeOffset>>()
        {
            { Constellation.Aquarius,new Tuple<DateTimeOffset, DateTimeOffset>(new DateTimeOffset(2000,1,20,0,0,0,Zero),new DateTimeOffset(2000,2,18,0,0,0,Zero))},
            { Constellation.Pisces,new Tuple<DateTimeOffset, DateTimeOffset>(new DateTimeOffset(2000,2,19,0,0,0,Zero),new DateTimeOffset(2000,3,20,0,0,0,Zero))},
            { Constellation.Aries,new Tuple<DateTimeOffset, DateTimeOffset>(new DateTimeOffset(2000,3,21,0,0,0,Zero),new DateTimeOffset(2000,4,19,0,0,0,Zero))},
            { Constellation.Taurus,new Tuple<DateTimeOffset, DateTimeOffset>(new DateTimeOffset(2000,4,20,0,0,0,Zero),new DateTimeOffset(2000,5,20,0,0,0,Zero))},
            { Constellation.Gemini,new Tuple<DateTimeOffset, DateTimeOffset>(new DateTimeOffset(2000,5,21,0,0,0,Zero),new DateTimeOffset(2000,6,21,0,0,0,Zero))},
            { Constellation.Cacer,new Tuple<DateTimeOffset, DateTimeOffset>(new DateTimeOffset(2000,6,22,0,0,0,Zero),new DateTimeOffset(2000,7,22,0,0,0,Zero))},
            { Constellation.Leo,new Tuple<DateTimeOffset, DateTimeOffset>(new DateTimeOffset(2000,7,23,0,0,0,Zero),new DateTimeOffset(2000,8,22,0,0,0,Zero))},
            { Constellation.Virgo,new Tuple<DateTimeOffset, DateTimeOffset>(new DateTimeOffset(2000,8,23,0,0,0,Zero),new DateTimeOffset(2000,9,22,0,0,0,Zero))},
            { Constellation.Libra,new Tuple<DateTimeOffset, DateTimeOffset>(new DateTimeOffset(2000,9,23,0,0,0,Zero),new DateTimeOffset(2000,10,23,0,0,0,Zero))},
            { Constellation.Scorpio,new Tuple<DateTimeOffset, DateTimeOffset>(new DateTimeOffset(2000,10,24,0,0,0,Zero),new DateTimeOffset(2000,11,22,0,0,0,Zero))},
            { Constellation.Sagittarius,new Tuple<DateTimeOffset, DateTimeOffset>(new DateTimeOffset(2000,11,23,0,0,0,Zero),new DateTimeOffset(2000,12,21,0,0,0,Zero))},
            { Constellation.Capricom,new Tuple<DateTimeOffset, DateTimeOffset>(new DateTimeOffset(2000,12,22,0,0,0,Zero),new DateTimeOffset(2000,1,19,0,0,0,Zero))},
        };

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the EZNEW.ValueType.Birth
        /// </summary>
        /// <param name="birthDate">birth date</param>
        public Birth(DateTimeOffset birthDate)
        {
            BirthDate = birthDate;
            constellation = GetConstellation(birthDate);
            age = GetAge(birthDate);
        }

        /// <summary>
        /// Gets age
        /// </summary>
        public int Age
        {
            get
            {
                return GetAge(BirthDate);
            }
        }

        /// <summary>
        /// Gets birth date
        /// </summary>
        public DateTimeOffset BirthDate { get; }

        /// <summary>
        /// Gets constellation
        /// </summary>
        public Constellation Constellation
        {
            get
            {
                return GetConstellation(BirthDate);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets constellation
        /// </summary>
        /// <param name="date">Date</param>
        /// <returns>Return the constellation</returns>
        public static Constellation GetConstellation(DateTimeOffset date)
        {
            var constell = Constellation.Gemini;
            var constellDate = new DateTimeOffset(2000, date.Month, date.Day, 0, 0, 0, Zero);
            var constellItem = constellationDictionary.FirstOrDefault(c => c.Value.Item1 <= constellDate && c.Value.Item2 >= constellDate);
            constell = constellItem.Key;
            return constell;
        }

        /// <summary>
        /// Gets age
        /// </summary>
        /// <param name="date">birth date</param>
        /// <returns>age</returns>
        public static int GetAge(DateTimeOffset date)
        {
            var nowDate = DateTimeOffset.Now.Date;
            var birthDate = date.Date;
            if (nowDate < birthDate.AddYears(1))
            {
                return 0;
            }
            return (nowDate - birthDate).Days / 365;
        }

        #endregion
    }

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
}
