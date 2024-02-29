using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;

namespace Sixnet.Drawing.Imaging
{
    /// <summary>
    /// Defines the default implements for ISixnetImageHandler
    /// </summary>
    public class DefaultImageHandler : ISixnetImageHandler
    {
        #region Fields

        static readonly Dictionary<string, ImageCodecInfo> ImageEncoderDict = null;

        static readonly PixelFormat[] IndexedPixelFormats = { PixelFormat.Undefined, PixelFormat.DontCare, PixelFormat.Format16bppArgb1555, PixelFormat.Format1bppIndexed, PixelFormat.Format4bppIndexed, PixelFormat.Format8bppIndexed };

        static readonly Color DEFAULT_BACKGROUND_COLOR = Color.Transparent;

        static readonly Dictionary<ScalingQuality, Action<Graphics>> ScalingQualityGraphicsActions = new Dictionary<ScalingQuality, Action<Graphics>>()
        {
            {
                ScalingQuality.Default,
                gh =>
                {
                    gh.InterpolationMode = InterpolationMode.Default;
                    gh.CompositingQuality = CompositingQuality.Default;
                    gh.SmoothingMode = SmoothingMode.Default;
                    gh.Clear(DEFAULT_BACKGROUND_COLOR);

                }
            },
            {
                ScalingQuality.High,
                gh =>
                {
                    gh.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    gh.CompositingQuality = CompositingQuality.HighQuality;
                    gh.SmoothingMode = SmoothingMode.HighQuality;
                    gh.Clear(DEFAULT_BACKGROUND_COLOR);
                }
            },
            {
                ScalingQuality.Low,
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

        static DefaultImageHandler()
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
        public Image Scale(HandleImageOptions imageHandlingOptions)
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

            var drawingRange = GetDrawingRange(ImageHandlingType.Scale, imageHandlingOptions);
            return GenerateImage(ImageHandlingType.Scale, drawingRange, imageHandlingOptions);
        }

        #endregion

        #region Cut

        /// <summary>
        /// Cut the image
        /// </summary>
        /// <param name="imageHandlingOptions">Image handling options</param>
        /// <returns>Return a new Image object </returns>
        public Image Cut(HandleImageOptions imageHandlingOptions)
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
            var cutRange = GetDrawingRange(ImageHandlingType.Cut, imageHandlingOptions);
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
        DrawingRange GetDrawingRange(ImageHandlingType imageHandlingType, HandleImageOptions imageHandlingOptions)
        {
            switch (imageHandlingType)
            {
                case ImageHandlingType.Scale:
                    return GetScaleRange(imageHandlingOptions);
                case ImageHandlingType.Cut:
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
        DrawingRange GetScaleRange(HandleImageOptions imageHandlingOptions)
        {
            var originalImage = imageHandlingOptions.OriginalImage;
            DrawingRange drawingRange = default;
            switch (imageHandlingOptions.ScalingType)
            {
                case ScalingType.FixedSize:
                    drawingRange = GetScaleRangeByFixedSize(imageHandlingOptions);
                    break;
                case ScalingType.Regular:
                    drawingRange = GetScaleRangeByRegular(imageHandlingOptions);
                    break;
                case ScalingType.WidthFirst:
                    drawingRange = GetScaleRangeByWidthFirst(imageHandlingOptions);
                    break;
                case ScalingType.HeightFirst:
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
        DrawingRange GetScaleRangeByFixedSize(HandleImageOptions imageHandlingOptions)
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
            return new DrawingRange()
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
        DrawingRange GetScaleRangeByRegular(HandleImageOptions imageHandlingOptions)
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
            return new DrawingRange()
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
        DrawingRange GetScaleRangeByWidthFirst(HandleImageOptions imageHandlingOptions)
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
            return new DrawingRange()
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
        DrawingRange GetScaleRangeByHeightFirst(HandleImageOptions imageHandlingOptions)
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
            return new DrawingRange()
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
        DrawingRange GetCutRange(HandleImageOptions imageHandlingOptions)
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
            return new DrawingRange()
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
        Image GenerateImage(ImageHandlingType imageHandlingType, DrawingRange drawingRange, HandleImageOptions imageHandlingOptions)
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
            if (imageHandlingType == ImageHandlingType.Scale)
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
