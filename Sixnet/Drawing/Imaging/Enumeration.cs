// "Company © 2025. All rights reserved."

namespace Sixnet.Drawing.Imaging
{
    /// <summary>
    /// Defines image handling type
    /// </summary>
    public enum SixnetImageHandlingType
    {
        Scale = 1101,
        Cut = 1105
    }

    /// <summary>
    /// Defines image scale type
    /// </summary>
    public enum SixnetScalingType
    {
        Regular = 210,
        WidthFirst = 215,
        HeightFirst = 220,
        FixedSize = 225
    }

    /// <summary>
    /// Defines scaling quality
    /// </summary>
    public enum SixnetScalingQuality
    {
        High = 310,
        Default = 320,
        Low = 330
    }

    /// <summary>
    /// Defines image split direction
    /// </summary>
    public enum SixnetImageSplitDirection
    {
        Horizontal = 410,
        Vertical = 420
    }
}
