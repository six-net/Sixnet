using System.Drawing;

namespace Sixnet.Drawing.Imaging
{
    /// <summary>
    /// Defines image handler contract
    /// </summary>
    public interface ISixnetImageHandler
    {
        /// <summary>
        /// Scale the image
        /// </summary>
        /// <param name="imageHandlingOptions">Image handling options</param>
        /// <returns>Return a new Image object </returns>
        Image Scale(HandleImageOptions imageHandlingOptions);

        /// <summary>
        /// Cut the image
        /// </summary>
        /// <param name="imageHandlingOptions">Image handling options</param>
        /// <returns>Return a new Image object </returns>
        Image Cut(HandleImageOptions imageHandlingOptions);
    }
}
