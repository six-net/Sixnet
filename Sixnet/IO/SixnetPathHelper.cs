using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sixnet.IO
{
    public static class SixnetPathHelper
    {
        /// <summary>
        /// Combine path
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static string Combine(params string[] paths)
        {
            if (paths.IsNullOrEmpty())
            {
                return string.Empty;
            }
            var realPaths = new List<string>();
            foreach (var path in paths)
            {
                if (!string.IsNullOrWhiteSpace(path))
                {
                    realPaths.Add(path);
                }
            }
            if (realPaths.IsNullOrEmpty())
            {
                return string.Empty;
            }
            return Path.Combine(realPaths.ToArray());
        }
    }
}
