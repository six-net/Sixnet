namespace Sixnet.Drawing.Imaging
{
    /// <summary>
    /// Defines a drawing range
    /// </summary>
    public struct DrawingRange
    {
        /// <summary>
        /// Get or sets the horizontal ordinate
        /// </summary>
        public int HorizontalOrdinate { get; set; }

        /// <summary>
        /// Gets or sets the veritcal ordinate
        /// </summary>
        public int VerticalOrdinate { get; set; }

        /// <summary>
        /// Gets or sets the image width
        /// </summary>
        public int ImageWidth { get; set; }

        /// <summary>
        /// Gets or sets the image height
        /// </summary>
        public int ImageHeight { get; set; }

        /// <summary>
        /// Gets or sets the drawing width
        /// </summary>
        public int DrawingWidth { get; set; }

        /// <summary>
        /// Gets or sets the drawing height
        /// </summary>
        public int DrawingHeight { get; set; }
    }
}
