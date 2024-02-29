using Sixnet.Model;
using System;
using System.Collections.Generic;

namespace Sixnet.Constants
{
    /// <summary>
    /// Time constants
    /// </summary>
    public static class SixnetTimes
    {
        /// <summary>
        /// zero time span
        /// </summary>
        public static readonly TimeSpan Zero = TimeSpan.FromMilliseconds(0);

        /// <summary>
        /// constellations
        /// </summary>
        public static readonly Dictionary<Constellation, Tuple<DateTimeOffset, DateTimeOffset>> Constellations = new()
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
    }
}
