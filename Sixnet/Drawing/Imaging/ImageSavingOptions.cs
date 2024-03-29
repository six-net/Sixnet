﻿using System;
using System.Drawing.Imaging;

namespace Sixnet.Drawing.Imaging
{
    /// <summary>
    /// Image saving options
    /// </summary>
    public class ImageSavingOptions
    {
        /// <summary>
        /// Gets or sets the save path
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the file name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the encoder parameter action
        /// </summary>
        public Action<EncoderParameters> EncoderParametersAction { get; set; }
    }
}
