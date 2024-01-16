namespace Sixnet.Drawing.Imaging
{
    /// <summary>
    /// Defines image handling type
    /// </summary>
    public enum ImageHandlingType
    {
        Scale = 1101,
        Cut = 1105
    }

    /// <summary>
    /// Defines image scale type
    /// </summary>
    public enum ScalingType
    {
        Regular = 210,
        WidthFirst = 215,
        HeightFirst = 220,
        FixedSize = 225
    }

    /// <summary>
    /// Defines scaling quality
    /// </summary>
    public enum ScalingQuality
    {
        High = 310,
        Default = 320,
        Low = 330
    }

    /// <summary>
    /// Defines image split direction
    /// </summary>
    public enum ImageSplitDirection
    {
        Horizontal = 410,
        Vertical = 420
    }
}
