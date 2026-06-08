// "Company © 2025. All rights reserved."

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Sixnet.Drawing.Imaging
{
    /// <summary>
    /// Defines the default implements for ISixnetImageHandler
    /// </summary>
    public class SixnetDefaultImageHandler : ISixnetImageHandler
    {
        #region Fields

        static readonly Dictionary<string, ImageCodecInfo> ImageEncoderDict = null;

        static readonly PixelFormat[] IndexedPixelFormats = { PixelFormat.Undefined, PixelFormat.DontCare, PixelFormat.Format16bppArgb1555, PixelFormat.Format1bppIndexed, PixelFormat.Format4bppIndexed, PixelFormat.Format8bppIndexed };

        static readonly Color DEFAULT_BACKGROUND_COLOR = Color.Transparent;

        static readonly Dictionary<SixnetScalingQuality, Action<Graphics>> ScalingQualityGraphicsActions = new Dictionary<SixnetScalingQuality, Action<Graphics>>()
        {
            {
                SixnetScalingQuality.Default,
                gh =>
                {
                    gh.InterpolationMode = InterpolationMode.Default;
                    gh.CompositingQuality = CompositingQuality.Default;
                    gh.SmoothingMode = SmoothingMode.Default;
                    gh.Clear(DEFAULT_BACKGROUND_COLOR);

                }
            },
            {
                SixnetScalingQuality.High,
                gh =>
                {
                    gh.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    gh.CompositingQuality = CompositingQuality.HighQuality;
                    gh.SmoothingMode = SmoothingMode.HighQuality;
                    gh.Clear(DEFAULT_BACKGROUND_COLOR);
                }
            },
            {
                SixnetScalingQuality.Low,
                gh =>
                {
                    gh.InterpolationMode = InterpolationMode.Low;
                    gh.CompositingQuality = CompositingQuality.HighSpeed;
                    gh.SmoothingMode = SmoothingMode.HighSpeed;
                    gh.Clear(DEFAULT_BACKGROUND_COLOR);
                }
            }
        };

        #endregion

        #region Constructor

        static SixnetDefaultImageHandler()
        {
            ImageEncoderDict = ImageCodecInfo.GetImageEncoders().ToDictionary(c => c.MimeType, c => c);
        }

        #endregion

        #region Scale

        /// <summary>
        /// Scale the image
        /// </summary>
        /// <param name="imageHandlingOptions">Image handling options</param>
        /// <returns>Return a new Image object </returns>
        public Image Scale(SixnetHandleImageOptions imageHandlingOptions)
        {
            #region Check arguments

            if (imageHandlingOptions == null)
            {
                throw new ArgumentNullException(nameof(imageHandlingOptions));
            }
            if (imageHandlingOptions.OriginalImage == null)
            {
                throw new ArgumentException("Original image is null");
            }
            if (imageHandlingOptions.ScalingWidth < 1 || imageHandlingOptions.ScalingHeight < 1)
            {
                throw new ArgumentException("ScalingWidth and ScalingHeight must greater thran 0");
            }

            #endregion

            var drawingRange = GetDrawingRange(SixnetImageHandlingType.Scale, imageHandlingOptions);
            return GenerateImage(SixnetImageHandlingType.Scale, drawingRange, imageHandlingOptions);
        }

        #endregion

        #region Cut

        /// <summary>
        /// Cut the image
        /// </summary>
        /// <param name="imageHandlingOptions">Image handling options</param>
        /// <returns>Return a new Image object </returns>
        public Image Cut(SixnetHandleImageOptions imageHandlingOptions)
        {
            #region Check parameters

            if (imageHandlingOptions == null)
            {
                throw new ArgumentNullException(nameof(imageHandlingOptions));
            }
            if (imageHandlingOptions.OriginalImage == null)
            {
                throw new ArgumentException("Original image is null");
            }
            if (imageHandlingOptions.CutWidth < 1 || imageHandlingOptions.CutHeight < 1 || imageHandlingOptions.CutHorizontalOrdinate < 1 || imageHandlingOptions.CutVerticalOrdinate < 1)
            {
                throw new ArgumentException("CutWidth, CutHeight,CutHorizontalOrdinate,CutVerticalOrdinate must greater thran 0");
            }

            #endregion

            var image = imageHandlingOptions.OriginalImage;
            var cutRange = GetDrawingRange(SixnetImageHandlingType.Cut, imageHandlingOptions);
            if (cutRange.ImageWidth == image.Width && cutRange.ImageHeight == image.Height
                && imageHandlingOptions.CutHorizontalOrdinate <= 0 && imageHandlingOptions.CutVerticalOrdinate <= 0
                && imageHandlingOptions.UseOriginalWhenNoAction)
            {
                return imageHandlingOptions.OriginalImage;
            }

            PixelFormat newFormat = IsPixelFormatIndexed(image.PixelFormat) ? PixelFormat.Format24bppRgb : image.PixelFormat;
            Bitmap newImage = new Bitmap(cutRange.ImageWidth, cutRange.ImageHeight, newFormat);
            newImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            Graphics newImgGrap = Graphics.FromImage(newImage);

            newImgGrap.InterpolationMode = InterpolationMode.HighQualityBicubic;
            newImgGrap.CompositingQuality = CompositingQuality.HighQuality;
            newImgGrap.SmoothingMode = SmoothingMode.HighQuality;
            newImgGrap.DrawImage(image, new RectangleF(0, 0, cutRange.DrawingWidth, cutRange.DrawingHeight), new Rectangle(cutRange.HorizontalOrdinate, cutRange.VerticalOrdinate, cutRange.DrawingWidth, cutRange.DrawingHeight), GraphicsUnit.Pixel);
            newImgGrap.Dispose();
            return newImage;
        }

        #endregion

        #region Util

        /// <summary>
        /// Get drawing range
        /// </summary>
        /// <param name="imageHandlingOptions">Image handling options</param>
        /// <returns></returns>
        SixnetDrawingRange GetDrawingRange(SixnetImageHandlingType imageHandlingType, SixnetHandleImageOptions imageHandlingOptions)
        {
            switch (imageHandlingType)
            {
                case SixnetImageHandlingType.Scale:
                    return GetScaleRange(imageHandlingOptions);
                case SixnetImageHandlingType.Cut:
                    return GetCutRange(imageHandlingOptions);
                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Get scale range
        /// </summary>
        /// <param name="imageHandlingOptions">Image handling options</param>
        /// <returns></returns>
        SixnetDrawingRange GetScaleRange(SixnetHandleImageOptions imageHandlingOptions)
        {
            var originalImage = imageHandlingOptions.OriginalImage;
            SixnetDrawingRange drawingRange = default;
            switch (imageHandlingOptions.ScalingType)
            {
                case SixnetScalingType.FixedSize:
                    drawingRange = GetScaleRangeByFixedSize(imageHandlingOptions);
                    break;
                case SixnetScalingType.Regular:
                    drawingRange = GetScaleRangeByRegular(imageHandlingOptions);
                    break;
                case SixnetScalingType.WidthFirst:
                    drawingRange = GetScaleRangeByWidthFirst(imageHandlingOptions);
                    break;
                case SixnetScalingType.HeightFirst:
                    drawingRange = GetScaleRangeByHeightFirst(imageHandlingOptions);
                    break;
            }
            return drawingRange;
        }

        /// <summary>
        /// Get sacle range by fixed size
        /// </summary>
        /// <param name="imageHandlingOptions">Image handling options</param>
        /// <returns></returns>
        SixnetDrawingRange GetScaleRangeByFixedSize(SixnetHandleImageOptions imageHandlingOptions)
        {
            var originalImage = imageHandlingOptions.OriginalImage;

            int originalImageWidth = originalImage.Width;
            int originalImageHeight = originalImage.Height;

            bool originalWidthLessThanScalingWidth = originalImageWidth <= imageHandlingOptions.ScalingWidth;
            bool originalHeightLessThanScalingHeight = originalImageHeight <= imageHandlingOptions.ScalingHeight;

            int beginX = 0;
            int beginY = 0;

            int drawingWidth = imageHandlingOptions.ScalingWidth;
            int drawingHeight = imageHandlingOptions.ScalingHeight;

            if (originalWidthLessThanScalingWidth && originalHeightLessThanScalingHeight)
            {
                beginX = (imageHandlingOptions.ScalingWidth - originalImageWidth) / 2;
                beginY = (imageHandlingOptions.ScalingHeight - originalImageHeight) / 2;
            }
            else if (originalWidthLessThanScalingWidth)
            {
                drawingWidth = imageHandlingOptions.ScalingHeight.ToDouble().ComputeWdith(originalImageWidth, originalImageHeight).ToInt32();
                beginX = (imageHandlingOptions.ScalingWidth - drawingWidth) / 2;
            }
            else if (originalHeightLessThanScalingHeight)
            {
                drawingHeight = imageHandlingOptions.ScalingWidth.ToDouble().ComputeHeight(originalImageWidth, originalImageHeight).ToInt32();
                beginY = (imageHandlingOptions.ScalingHeight - drawingHeight) / 2;
            }
            return new SixnetDrawingRange()
            {
                HorizontalOrdinate = beginX,
                VerticalOrdinate = beginY,
                DrawingWidth = drawingWidth,
                DrawingHeight = drawingHeight,
                ImageHeight = imageHandlingOptions.ScalingHeight,
                ImageWidth = imageHandlingOptions.ScalingWidth
            };
        }

        /// <summary>
        /// Get scale range by regular
        /// </summary>
        /// <param name="imageHandlingOptions"></param>
        /// <returns></returns>
        SixnetDrawingRange GetScaleRangeByRegular(SixnetHandleImageOptions imageHandlingOptions)
        {
            var image = imageHandlingOptions.OriginalImage;

            int originalImageWidth = image.Width;
            int originalImageHeight = image.Height;
            int drawingWidth = imageHandlingOptions.ScalingWidth;
            int drawingHeight = imageHandlingOptions.ScalingHeight;
            bool originalWidthLessThanScalingWidth = originalImageWidth <= drawingWidth;
            bool originalHeightLessThanScalingHeight = originalImageHeight <= drawingHeight;
            if (originalWidthLessThanScalingWidth && originalWidthLessThanScalingWidth)
            {
            }
            else if (originalWidthLessThanScalingWidth)
            {
                drawingHeight = imageHandlingOptions.ScalingHeight;
                drawingWidth = drawingHeight.ToDouble().ComputeWdith(originalImageWidth, originalImageHeight).ToInt32();
            }
            else if (originalHeightLessThanScalingHeight)
            {
                drawingWidth = imageHandlingOptions.ScalingWidth;
                drawingHeight = drawingWidth.ToDouble().ComputeHeight(originalImageWidth, originalImageHeight).ToInt32();
            }
            return new SixnetDrawingRange()
            {
                DrawingHeight = drawingHeight,
                DrawingWidth = drawingWidth,
                HorizontalOrdinate = 0,
                VerticalOrdinate = 0,
                ImageWidth = drawingWidth,
                ImageHeight = drawingHeight
            };
        }

        /// <summary>
        /// Get scale range by width first
        /// </summary>
        /// <param name="imageHandlingOptions"></param>
        /// <returns></returns>
        SixnetDrawingRange GetScaleRangeByWidthFirst(SixnetHandleImageOptions imageHandlingOptions)
        {
            var image = imageHandlingOptions.OriginalImage;

            int originalImageWidth = image.Width;
            int originalImageHeight = image.Height;
            int drawingWidth = imageHandlingOptions.ScalingWidth;
            int drawingHeight = originalImageHeight;
            if (originalImageWidth > drawingWidth)
            {
                drawingHeight = drawingWidth.ToDouble().ComputeHeight(originalImageWidth, originalImageHeight).ToInt32();
            }
            return new SixnetDrawingRange()
            {
                DrawingHeight = drawingHeight,
                DrawingWidth = drawingWidth,
                HorizontalOrdinate = 0,
                VerticalOrdinate = 0,
                ImageWidth = drawingWidth,
                ImageHeight = drawingHeight
            };
        }

        /// <summary>
        /// Get scale range by height first
        /// </summary>
        /// <param name="imageHandlingOptions"></param>
        /// <returns></returns>
        SixnetDrawingRange GetScaleRangeByHeightFirst(SixnetHandleImageOptions imageHandlingOptions)
        {
            var image = imageHandlingOptions.OriginalImage;

            int originalImageWidth = image.Width;
            int originalImageHeight = image.Height;
            int drawingWidth = originalImageWidth;
            int drawingHeight = imageHandlingOptions.ScalingHeight;
            if (originalImageHeight > drawingHeight)
            {
                drawingWidth = drawingHeight.ToDouble().ComputeWdith(originalImageWidth, originalImageHeight).ToInt32();
            }
            return new SixnetDrawingRange()
            {
                DrawingHeight = drawingHeight,
                DrawingWidth = drawingWidth,
                HorizontalOrdinate = 0,
                VerticalOrdinate = 0,
                ImageWidth = drawingWidth,
                ImageHeight = drawingHeight
            };
        }

        /// <summary>
        /// Get cut range
        /// </summary>
        /// <param name="imageHandlingOptions"></param>
        /// <returns></returns>
        SixnetDrawingRange GetCutRange(SixnetHandleImageOptions imageHandlingOptions)
        {
            var image = imageHandlingOptions.OriginalImage;
            int originalImageWidth = image.Width;
            int originalImageHeight = image.Height;
            int drawingWidth = imageHandlingOptions.CutWidth;
            int drawingHeight = imageHandlingOptions.CutHeight;
            if (originalImageWidth > drawingWidth || originalImageHeight > drawingHeight)
            {
                if (originalImageWidth < imageHandlingOptions.CutHorizontalOrdinate + drawingWidth)
                {
                    drawingWidth = originalImageWidth - imageHandlingOptions.CutHorizontalOrdinate;
                }
                if (originalImageHeight < imageHandlingOptions.CutVerticalOrdinate + drawingHeight)
                {
                    drawingHeight = originalImageHeight - imageHandlingOptions.CutVerticalOrdinate;
                }
            }
            return new SixnetDrawingRange()
            {
                ImageWidth = drawingWidth,
                ImageHeight = drawingHeight,
                DrawingWidth = drawingWidth,
                DrawingHeight = drawingHeight,
                HorizontalOrdinate = imageHandlingOptions.CutHorizontalOrdinate,
                VerticalOrdinate = imageHandlingOptions.CutVerticalOrdinate,
            };
        }

        /// <summary>
        /// Generate image
        /// </summary>
        /// <param name="drawingRange">Drawing range</param>
        /// <param name="imageHandlingOptions">Image handling options</param>
        /// <returns></returns>
        Image GenerateImage(SixnetImageHandlingType imageHandlingType, SixnetDrawingRange drawingRange, SixnetHandleImageOptions imageHandlingOptions)
        {
            var image = imageHandlingOptions.OriginalImage;
            PixelFormat newFormat = IsPixelFormatIndexed(image.PixelFormat) ? PixelFormat.Format24bppRgb : image.PixelFormat;
            Bitmap newImage = new Bitmap(drawingRange.ImageWidth, drawingRange.ImageHeight, newFormat);
            newImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            Graphics graphics = Graphics.FromImage(newImage);
            if (ScalingQualityGraphicsActions.TryGetValue(imageHandlingOptions.ScalingQuality, out var defaultGrapAction))
            {
                defaultGrapAction?.Invoke(graphics);
            }
            imageHandlingOptions?.GraphicsAction?.Invoke(graphics);
            if (imageHandlingType == SixnetImageHandlingType.Scale)
            {
                graphics.DrawImage(image, new RectangleF(drawingRange.HorizontalOrdinate, drawingRange.VerticalOrdinate, drawingRange.DrawingWidth, drawingRange.DrawingHeight), new RectangleF(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
            }
            else
            {
                graphics.DrawImage(image, new RectangleF(0, 0, drawingRange.DrawingWidth, drawingRange.DrawingHeight), new RectangleF(drawingRange.HorizontalOrdinate, drawingRange.VerticalOrdinate, image.Width, image.Height), GraphicsUnit.Pixel);
            }
            graphics.Flush();
            graphics.Dispose();
            return newImage;
        }

        /// <summary>
        /// Determine if the image's PixelFormat is in the PixelFormat that raised the exception and cannot create a graphics object from an image with an indexed PixelFormat
        /// </summary>
        /// <param name="imagePixelFormat">Image's pixel format</param>
        /// <returns>Whether has pixel format</returns>
        private static bool IsPixelFormatIndexed(PixelFormat imagePixelFormat)
        {
            return IndexedPixelFormats.Contains(imagePixelFormat);
        }

        #endregion
    }
}
