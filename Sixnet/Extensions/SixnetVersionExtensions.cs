// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class SixnetVersionExtensions
    {
        /// <summary>
        /// Version to long
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static long VersionToLong(this Version version)
        {
            var versionString = version.ToString();
            var parts = versionString.Split('.');
            int[] nums = new int[4];

            for (int i = 0; i < parts.Length && i < 4; i++)
            {
                nums[i] = int.Parse(parts[i]);
            }
            string longStr = $"{nums[0]:D3}{nums[1]:D4}{nums[2]:D5}{nums[3]:D6}";
            return long.Parse(longStr);
        }
    }
}
