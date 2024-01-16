using System.Drawing;

namespace Sixnet.Drawing.Imaging
{
    /// <summary>
    /// Defines image handler contract
    /// </summary>
    public interface IImageHandler
    {
        /// <summary>
        /// Scale the image
        /// </summary>
        /// <param name="imageHandlingOptions">Image handling options</param>
        /// <returns>Return a new Image object </returns>
        Image Scale(ImageHandlingOptions imageHandlingOptions);

        /// <summary>
        /// Cut the image
        /// </summary>
        /// <param name="imageHandlingOptions">Image handling options</param>
        /// <returns>Return a new Image object </returns>
        Image Cut(ImageHandlingOptions imageHandlingOptions);
    }
}
