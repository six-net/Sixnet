using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Localization
{
    public struct ResourcePrefix
    {
        /// <summary>
        /// Root namespace
        /// </summary>
        public string RootNamespace { get; set; }

        /// <summary>
        /// Resource path
        /// </summary>
        public string ResourcePath { get; set; }

        /// <summary>
        /// Resx resource base name
        /// </summary>
        public string ResxBaseName { get; set; }

        /// <summary>
        /// Json resource base name
        /// </summary>
        public string JsonBaseName { get; set; }

        /// <summary>
        /// Json resource path
        /// </summary>

        public string JsonResourcePath { get; set; }
    }
}
