using System;
using System.Drawing;

namespace Sixnet.Drawing.Imaging
{
    /// <summary>
    /// Split image options
    /// </summary>
    public class SplitImageOptions
    {
        /// <summary>
        /// Gets or sets the image handling options action
        /// </summary>
        public Action<HandleImageOptions> ImageHandlingOptionsAction { get; set; }

        /// <summary>
        /// Gets or sets the original image
        /// </summary>
        public Image OriginalImage { get; set; }

        /// <summary>
        /// Gets or sets the split direction
        /// </summary>
        public ImageSplitDirection Direction { get; set; }

        /// <summary>
        /// Gets or sets the split size
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Gets or sets the split boundary
        /// </summary>
        public int SplitBoundary { get; set; }
    }
}
