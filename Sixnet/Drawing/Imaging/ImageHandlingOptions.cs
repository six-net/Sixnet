using System;
using System.Drawing;

namespace Sixnet.Drawing.Imaging
{
    /// <summary>
    /// Defines image handling options
    /// </summary>
    public class ImageHandlingOptions
    {
        /// <summary>
        /// Gets or sets the original image
        /// </summary>
        public Image OriginalImage { get; set; }

        /// <summary>
        /// Gets or sets the scaling width
        /// </summary>
        public int ScalingWidth { get; set; }

        /// <summary>
        /// Gets or sets the scaling height
        /// </summary>
        public int ScalingHeight { get; set; }

        /// <summary>
        /// Indecates whether scaling by fixed size
        /// </summary>
        public ScalingType ScalingType { get; set; } = ScalingType.Regular;

        /// <summary>
        /// Indecates whether sacling quality
        /// </summary>
        public ScalingQuality ScalingQuality { get; set; } = ScalingQuality.Default;

        /// <summary>
        /// Gets or sets the cut width
        /// </summary>
        public int CutWidth { get; set; }

        /// <summary>
        /// Gets or sets the cut height
        /// </summary>
        public int CutHeight { get; set; }

        /// <summary>
        /// Gets or sets the cut horizontal ordinate
        /// </summary>
        public int CutHorizontalOrdinate { get; set; }

        /// <summary>
        /// Gets or sets the cut vertical oridinate
        /// </summary>
        public int CutVerticalOrdinate { get; set; }

        /// <summary>
        /// Indecates whether use original image when no action
        /// </summary>
        public bool UseOriginalWhenNoAction { get; set; } = true;

        /// <summary>
        /// Gets or sets the graphics action
        /// </summary>
        public Action<Graphics> GraphicsAction { get; set; }

    }
}
