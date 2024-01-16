using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Sixnet.Code;
using Sixnet.DependencyInjection;

namespace Sixnet.Drawing.Imaging
{
    /// <summary>
    /// Image helper
    /// </summary>
    public static class ImageHelper
    {
        #region Scale

        /// <summary>
        /// Scale an image
        /// </summary>
        /// <param name="originalImage">Original image</param>
        /// <param name="imageHandlingOptionsAction">Image handling options action</param>
        /// <returns>Return a new image object</returns>
        public static Image Scale(Action<ImageHandlingOptions> imageHandlingOptionsAction)
        {
            var imageHandlingOptions = new ImageHandlingOptions()
            {
                ScalingType = ScalingType.Regular,
            };
            imageHandlingOptionsAction?.Invoke(imageHandlingOptions);
            return GetImageHandler().Scale(imageHandlingOptions);
        }

        /// <summary>
        /// Scale an image
        /// </summary>
        /// <param name="image">Original image</param>
        /// <param name="scalingWidth">Scaling width</param>
        /// <param name="scalingHeight">Scaling height</param>
        /// <returns>Return a new image object</returns>
        public static Image Scale(Image image, int scalingWidth, int scalingHeight)
        {
            return Scale(option =>
            {
                option.ScalingWidth = scalingWidth;
                option.ScalingHeight = scalingHeight;
                option.ScalingType = ScalingType.Regular;
                option.OriginalImage = image;
            });
        }

        /// <summary>
        /// Scale an image by fixed size
        /// </summary>
        /// <param name="image">Original image</param>
        /// <param name="scalingWidth">Scaling width</param>
        /// <param name="scalingHeight">Scaling height</param>
        /// <returns>Return a new image object</returns>
        public static Image ScaleByFixedSize(Image image, int scalingWidth, int scalingHeight)
        {
            return Scale(options =>
            {
                options.ScalingWidth = scalingWidth;
                options.ScalingHeight = scalingHeight;
                options.ScalingType = ScalingType.FixedSize;
                options.OriginalImage = image;
            });
        }

        /// <summary>
        /// Scale by width
        /// </summary>
        /// <param name="image">Original image</param>
        /// <param name="scalingWidth">Scaling width</param>
        /// <returns>Return a new image object</returns>
        public static Image ScaleByWidth(Image image, int scalingWidth)
        {
            return Scale(option =>
            {
                option.ScalingType = ScalingType.WidthFirst;
                option.ScalingWidth = scalingWidth;
                option.OriginalImage = image;

            });
        }

        /// <summary>
        /// Scale by height
        /// </summary>
        /// <param name="image">Original image</param>
        /// <param name="scalingWidth">Scaling height</param>
        /// <returns>Return a new image object</returns>
        public static Image ScaleByHeight(Image image, int scalingHeight)
        {
            return Scale(option =>
            {
                option.ScalingType = ScalingType.HeightFirst;
                option.ScalingHeight = scalingHeight;
                option.OriginalImage = image;

            });
        }

        /// <summary>
        /// Scale a image file
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="imageHandlingOptionsAction">Image handling options action</param>
        /// <returns>Return an new image object</returns>
        public static Image Scale(string filePath, Action<ImageHandlingOptions> imageHandlingOptionsAction)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }
            using (Image originalImage = GetImageFromFile(filePath))
            {
                return Scale(options =>
                {
                    options.OriginalImage = originalImage;
                    imageHandlingOptionsAction?.Invoke(options);
                });
            }
        }

        /// <summary>
        /// Scale and save image
        /// </summary>
        /// <param name="imageHandlingOptionsAction">Image handling options action</param>
        /// <param name="imageSavingOptionsAction">Image saving options action</param>
        /// <returns>Return the new file path </returns>
        public static string ScaleThenSave(Action<ImageHandlingOptions> imageHandlingOptionsAction, Action<ImageSavingOptions> imageSavingOptionsAction)
        {
            var scaleImage = Scale(imageHandlingOptionsAction);
            if (scaleImage != null)
            {
                return SaveImage(scaleImage, imageSavingOptionsAction);
            }
            throw new Exception("Scale image failed");
        }

        /// <summary>
        /// Scale a image file and save the new image
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="imageHandlingOptionsAction">Image handling options action</param>
        /// <param name="imageSavingOptionsAction">Image saving options action</param>
        /// <returns>Return the new file path </returns>
        public static string ScaleThenSave(string filePath, Action<ImageHandlingOptions> imageHandlingOptionsAction, Action<ImageSavingOptions> imageSavingOptionsAction)
        {
            var scaleImage = Scale(filePath, imageHandlingOptionsAction);
            if (scaleImage != null)
            {
                return SaveImage(scaleImage, imageSavingOptionsAction);
            }
            throw new Exception("Scale image failed");
        }

        #endregion

        #region Cut

        /// <summary>
        /// Cut an image
        /// </summary>
        /// <param name="imageHandlingOptionsAction"></param>
        /// <returns></returns>
        public static Image Cut(Action<ImageHandlingOptions> imageHandlingOptionsAction)
        {
            var imageHandlingOptions = new ImageHandlingOptions();
            imageHandlingOptionsAction?.Invoke(imageHandlingOptions);
            return GetImageHandler().Cut(imageHandlingOptions);
        }

        /// <summary>
        /// Cut an image
        /// </summary>
        /// <param name="image">Original image</param>
        /// <param name="cutWidth">Cut width</param>
        /// <param name="cutHeight">Cut height</param>
        /// <param name="cutHorizontalOrdinate">Cut horizontal ordinate</param>
        /// <param name="cutVerticalOrdinate"Cut vertical ordinate</param>
        /// <returns>Return a new image object</returns>
        public static Image Cut(Image image, int cutWidth, int cutHeight, int cutHorizontalOrdinate, int cutVerticalOrdinate)
        {
            return Cut(option =>
            {
                option.OriginalImage = image;
                option.CutWidth = cutWidth;
                option.CutHeight = cutHeight;
                option.CutHorizontalOrdinate = cutHorizontalOrdinate;
                option.CutVerticalOrdinate = cutVerticalOrdinate;
            });
        }

        /// <summary>
        /// Cut an image
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="imageHandlingOptionsAction">Image handling options action</param>
        /// <returns>Return the new image object</returns>
        public static Image Cut(string filePath, Action<ImageHandlingOptions> imageHandlingOptionsAction)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }
            using (var originalImage = GetImageFromFile(filePath))
            {
                return Cut(options =>
                {
                    options.OriginalImage = originalImage;
                    imageHandlingOptionsAction?.Invoke(options);
                });
            }
        }

        /// <summary>
        /// Cut center part
        /// </summary>
        /// <param name="image">Original image</param>
        /// <param name="cutWidth">Cut width</param>
        /// <param name="cutHeight">Cut height</param>
        /// <returns></returns>
        public static Image CutCenterPart(Image image, int cutWidth, int cutHeight)
        {
            var centerPartLocation = CalculateCenterPartOrdinate(image, cutWidth, cutHeight);
            return Cut(image, cutWidth, cutHeight, centerPartLocation.Item1, centerPartLocation.Item2);
        }

        /// <summary>
        /// Scale and cut an image
        /// </summary>
        /// <param name="imageHandlingOptionsAction">Image handling options action</param>
        /// <returns>Return a new image object</returns>
        public static Image ScaleAndCut(Action<ImageHandlingOptions> imageHandlingOptionsAction)
        {
            var scalImage = Scale(imageHandlingOptionsAction);
            if (scalImage != null)
            {
                return Cut(options =>
                {
                    options.OriginalImage = scalImage;
                    imageHandlingOptionsAction?.Invoke(options);
                });
            }
            throw new Exception("Scale image failed");
        }

        /// <summary>
        /// Scale and cut an image center part
        /// </summary>
        /// <param name="imageHandlingOptionsAction">Image handling options action</param>
        /// <returns>Return a new image object</returns>
        public static Image ScaleAndCutCenterPart(Action<ImageHandlingOptions> imageHandlingOptionsAction)
        {
            var scalImage = Scale(imageHandlingOptionsAction);
            if (scalImage != null)
            {
                return Cut(options =>
                {
                    options.OriginalImage = scalImage;
                    imageHandlingOptionsAction?.Invoke(options);
                    var centerPartLocation = CalculateCenterPartOrdinate(scalImage, options.CutWidth, options.CutHeight);
                    options.CutHorizontalOrdinate = centerPartLocation.Item1;
                    options.CutVerticalOrdinate = centerPartLocation.Item2;
                });
            }
            throw new Exception("Scale image failed");
        }

        /// <summary>
        /// Scale and cut an image
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="imageHandlingOptionsAction">Image handling options action</param>
        /// <returns>Return a new image object</returns>
        public static Image ScaleAndCut(string filePath, Action<ImageHandlingOptions> imageHandlingOptionsAction)
        {
            var scalImage = Scale(filePath, imageHandlingOptionsAction);
            if (scalImage != null)
            {
                return Cut(imageHandlingOptionsAction);
            }
            throw new Exception("Scale image failed");
        }

        /// <summary>
        /// Scale and cut an image
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="imageHandlingOptionsAction">Image handling options action</param>
        /// <returns>Return a new image object</returns>
        public static Image ScaleAndCutCenterPart(string filePath, Action<ImageHandlingOptions> imageHandlingOptionsAction)
        {
            var scalImage = Scale(filePath, imageHandlingOptionsAction);
            if (scalImage != null)
            {
                return Cut(options =>
                {
                    options.OriginalImage = scalImage;
                    imageHandlingOptionsAction?.Invoke(options);
                    var centerPartLocation = CalculateCenterPartOrdinate(scalImage, options.CutWidth, options.CutHeight);
                    options.CutHorizontalOrdinate = centerPartLocation.Item1;
                    options.CutVerticalOrdinate = centerPartLocation.Item2;

                });
            }
            throw new Exception("Scale image failed");
        }

        /// <summary>
        /// Cut an image and save
        /// </summary>
        /// <param name="imageHandlingOptionsAction">Image handling options action</param>
        /// <param name="imageSavingOptionsAction">Image saving options action</param>
        /// <returns>Return the new image path</returns>
        public static string CutThenSave(Action<ImageHandlingOptions> imageHandlingOptionsAction, Action<ImageSavingOptions> imageSavingOptionsAction)
        {
            var cutImage = Cut(imageHandlingOptionsAction);
            if (cutImage != null)
            {
                return SaveImage(cutImage, imageSavingOptionsAction);
            }
            throw new Exception("Cut image failed");
        }

        /// <summary>
        /// Cut an image center part and save
        /// </summary>
        /// <param name="imageHandlingOptionsAction">Image handling options action</param>
        /// <param name="imageSavingOptionsAction">Image saving options action</param>
        /// <returns>Return the new image path</returns>
        public static string CutCenterPartThenSave(Action<ImageHandlingOptions> imageHandlingOptionsAction, Action<ImageSavingOptions> imageSavingOptionsAction)
        {
            var cutImage = Cut(options =>
            {
                imageHandlingOptionsAction?.Invoke(options);
                var centerPartLocation = CalculateCenterPartOrdinate(options.OriginalImage, options.CutWidth, options.CutHeight);
                options.CutHorizontalOrdinate = centerPartLocation.Item1;
                options.CutVerticalOrdinate = centerPartLocation.Item2;
            });
            if (cutImage != null)
            {
                return SaveImage(cutImage, imageSavingOptionsAction);
            }
            throw new Exception("Cut image failed");
        }

        /// <summary>
        /// Cut an image and save
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="imageHandlingOptionsAction">Image handling options action</param>
        /// <param name="imageSavingOptionsAction">Image saving options action</param>
        /// <returns>Return the new image path</returns>
        public static string CutThenSave(string filePath, Action<ImageHandlingOptions> imageHandlingOptionsAction, Action<ImageSavingOptions> imageSavingOptionsAction)
        {
            var cutImage = Cut(filePath, imageHandlingOptionsAction);
            if (cutImage != null)
            {
                return SaveImage(cutImage, imageSavingOptionsAction);
            }
            throw new Exception("Cut image failed");
        }

        /// <summary>
        /// Cut an image center part and save
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="imageHandlingOptionsAction">Image handling options action</param>
        /// <param name="imageSavingOptionsAction">Image saving options action</param>
        /// <returns>Return the new image path</returns>
        public static string CutCenterPartThenSave(string filePath, Action<ImageHandlingOptions> imageHandlingOptionsAction, Action<ImageSavingOptions> imageSavingOptionsAction)
        {
            var cutImage = Cut(filePath, options =>
            {
                imageHandlingOptionsAction?.Invoke(options);
                var centerPartLocation = CalculateCenterPartOrdinate(options.OriginalImage, options.CutWidth, options.CutHeight);
                options.CutHorizontalOrdinate = centerPartLocation.Item1;
                options.CutVerticalOrdinate = centerPartLocation.Item2;
            });
            if (cutImage != null)
            {
                return SaveImage(cutImage, imageSavingOptionsAction);
            }
            throw new Exception("Cut image failed");
        }

        /// <summary>
        /// Scale and cut an image ,then save it
        /// </summary>
        /// <param name="imageHandlingOptionsAction">Image handling options action</param>
        /// <param name="imageSavingOptionsAction">Image saving options action</param>
        /// <returns>Return the new image path</returns>
        public static string ScaleAndCutThenSave(Action<ImageHandlingOptions> imageHandlingOptionsAction, Action<ImageSavingOptions> imageSavingOptionsAction)
        {
            var cutImage = ScaleAndCut(imageHandlingOptionsAction);
            if (cutImage != null)
            {
                return SaveImage(cutImage, imageSavingOptionsAction);
            }
            throw new Exception("Cut image failed");
        }

        /// <summary>
        /// Scale and cut an image center part ,then save it
        /// </summary>
        /// <param name="imageHandlingOptionsAction">Image handling options action</param>
        /// <param name="imageSavingOptionsAction">Image saving options action</param>
        /// <returns>Return the new image path</returns>
        public static string ScaleAndCutCenterPartThenSave(Action<ImageHandlingOptions> imageHandlingOptionsAction, Action<ImageSavingOptions> imageSavingOptionsAction)
        {
            var cutImage = ScaleAndCutCenterPart(imageHandlingOptionsAction);
            if (cutImage != null)
            {
                return SaveImage(cutImage, imageSavingOptionsAction);
            }
            throw new Exception("Cut image failed");
        }

        /// <summary>
        /// Scale and cut an image ,then save it
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="imageHandlingOptionsAction">Image handling options action</param>
        /// <param name="imageSavingOptionsAction">Image saving options action</param>
        /// <returns>Return the new image path</returns>
        public static string ScaleAndCutThenSave(string filePath, Action<ImageHandlingOptions> imageHandlingOptionsAction, Action<ImageSavingOptions> imageSavingOptionsAction)
        {
            var cutImage = ScaleAndCut(filePath, imageHandlingOptionsAction);
            if (cutImage != null)
            {
                return SaveImage(cutImage, imageSavingOptionsAction);
            }
            throw new Exception("Cut image failed");
        }

        /// <summary>
        /// Scale and cut an image center part ,then save it
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="imageHandlingOptionsAction">Image handling options action</param>
        /// <param name="imageSavingOptionsAction">Image saving options action</param>
        /// <returns>Return the new image path</returns>
        public static string ScaleAndCutCenterPartThenSave(string filePath, Action<ImageHandlingOptions> imageHandlingOptionsAction, Action<ImageSavingOptions> imageSavingOptionsAction)
        {
            var cutImage = ScaleAndCutCenterPart(filePath, imageHandlingOptionsAction);
            if (cutImage != null)
            {
                return SaveImage(cutImage, imageSavingOptionsAction);
            }
            throw new Exception("Cut image failed");
        }

        #endregion

        #region Split

        /// <summary>
        /// Split an image
        /// </summary>
        /// <param name="imageSplitOptionsAction">Image split options action</param>
        /// <returns>Return an image list</returns>
        public static List<Image> Split(Action<ImageSpitOptions> imageSplitOptionsAction)
        {
            ImageSpitOptions imageSpitOptions = new ImageSpitOptions();
            imageSplitOptionsAction?.Invoke(imageSpitOptions);

            #region Horizontal

            List<Image> HandleSplitByHorizontal(ImageSpitOptions imageSpitOptions)
            {
                if (imageSpitOptions?.OriginalImage == null)
                {
                    throw new ArgumentNullException(nameof(ImageSpitOptions.OriginalImage));
                }

                if (imageSpitOptions.SplitBoundary > 0 && imageSpitOptions.OriginalImage.Width <= imageSpitOptions.SplitBoundary)
                {
                    return new List<Image>(1)
                    {
                        imageSpitOptions.OriginalImage
                    };
                }
                int count = imageSpitOptions.OriginalImage.Width / imageSpitOptions.Size;
                count = imageSpitOptions.OriginalImage.Width % imageSpitOptions.Size == 0 ? count : count + 1;
                List<Image> images = new List<Image>(count);
                for (int i = 0; i < count; i++)
                {
                    int cindex = i;
                    Action<ImageHandlingOptions> cutOptionsAction = options =>
                    {
                        options.OriginalImage = imageSpitOptions.OriginalImage;
                        options.CutWidth = imageSpitOptions.Size;
                        options.CutHeight = imageSpitOptions.OriginalImage.Height;
                        options.CutHorizontalOrdinate = cindex * imageSpitOptions.Size + cindex;
                        options.CutVerticalOrdinate = 0;
                    };
                    var imageItem = Cut(cutOptionsAction);
                    if (imageItem != null)
                    {
                        images.Add(imageItem);
                    }
                }
                return images;
            }

            #endregion

            #region Vertical

            List<Image> HandleSplitByVertical(ImageSpitOptions imageSpitOptions)
            {
                if (imageSpitOptions?.OriginalImage == null)
                {
                    throw new ArgumentNullException(nameof(ImageSpitOptions.OriginalImage));
                }
                if (imageSpitOptions.SplitBoundary > 0 && imageSpitOptions.OriginalImage.Height <= imageSpitOptions.SplitBoundary)
                {
                    return new List<Image>(1)
                    {
                        imageSpitOptions.OriginalImage
                    };
                }
                int count = imageSpitOptions.OriginalImage.Height / imageSpitOptions.Size;
                count = imageSpitOptions.OriginalImage.Height % imageSpitOptions.Size == 0 ? count : count + 1;
                List<Image> images = new List<Image>(count);
                for (int i = 0; i < count; i++)
                {
                    int cindex = i;
                    Action<ImageHandlingOptions> cutOptionsAction = options =>
                    {
                        options.OriginalImage = imageSpitOptions.OriginalImage;
                        options.CutWidth = imageSpitOptions.OriginalImage.Width;
                        options.CutHeight = imageSpitOptions.Size;
                        options.CutHorizontalOrdinate = 0;
                        options.CutVerticalOrdinate = cindex * imageSpitOptions.Size + cindex;
                    };
                    var imageItem = Cut(cutOptionsAction);
                    if (imageItem != null)
                    {
                        images.Add(imageItem);
                    }
                }
                return images;
            }

            #endregion

            switch (imageSpitOptions.Direction)
            {
                case ImageSplitDirection.Horizontal:
                    return HandleSplitByHorizontal(imageSpitOptions);
                case ImageSplitDirection.Vertical:
                    return HandleSplitByVertical(imageSpitOptions);
            }
            throw new InvalidOperationException("Invalid split operation");
        }

        /// <summary>
        /// Split an image
        /// </summary>
        /// <param name="filePath">Image file</param>
        /// <param name="imageSplitOptionsAction">Image split options action</param>
        /// <returns>Return an image list</returns>
        public static List<Image> Split(string filePath, Action<ImageSpitOptions> imageSplitOptionsAction)
        {
            using (var image = GetImageFromFile(filePath))
            {
                return Split(options =>
                {
                    options.OriginalImage = image;
                    imageSplitOptionsAction?.Invoke(options);
                });
            }
        }

        /// <summary>
        /// Split an image and save
        /// </summary>
        /// <param name="imageSplitOptionsAction">Image split options action</param>
        /// <param name="imageSavingOptionsAction">Image saving options action</param>
        /// <returns>Return images paths</returns>
        public static List<string> SplitThenSave(Action<ImageSpitOptions> imageSplitOptionsAction, Action<ImageSavingOptions> imageSavingOptionsAction)
        {
            var images = Split(imageSplitOptionsAction);
            try
            {
                if (!images.IsNullOrEmpty())
                {
                    return SaveImage(images, imageSavingOptionsAction);
                }
                throw new Exception("Split image failed");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                images?.ForEach(img =>
                {
                    img?.Dispose();
                });
            }
        }

        /// <summary>
        /// Split an image and save
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="imageSplitOptionsAction">Image split options action</param>
        /// <param name="imageSavingOptionsAction">Image saving options action</param>
        /// <returns>Return image paths</returns>
        public static List<string> SplitThenSave(string filePath, Action<ImageSpitOptions> imageSplitOptionsAction, Action<ImageSavingOptions> imageSavingOptionsAction)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }
            using (var image = GetImageFromFile(filePath))
            {
                return SplitThenSave(options =>
                {
                    options.OriginalImage = image;
                    imageSplitOptionsAction?.Invoke(options);
                }, imageSavingOptionsAction);
            }
        }

        #endregion

        #region Util

        /// <summary>
        /// Default image handler
        /// </summary>
        static readonly IImageHandler DefaultImageHandler = new DefaultImageHandler();

        /// <summary>
        /// Default encoder parameters action
        /// </summary>
        static readonly Action<EncoderParameters> DefaultEncoderParametersAction = (parameters) =>
        {
            parameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
        };

        /// <summary>
        /// Get image handler
        /// </summary>
        /// <returns></returns>
        static IImageHandler GetImageHandler()
        {
            var imageHandler = ContainerManager.Resolve<IImageHandler>();
            return imageHandler ?? DefaultImageHandler;
        }

        /// <summary>
        /// Get image codec info
        /// </summary>
        /// <param name="extension">File extension</param>
        /// <returns>Return an image codec info</returns>
        public static ImageCodecInfo GetImageEncoderByExtension(string extension)
        {
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            if (extension.IsNullOrEmpty())
            {
                return encoders.FirstOrDefault(e => e.MimeType == "image/jpeg");
            }
            extension = extension.ToLower().Trim('.');
            switch (extension)
            {
                case "jpg":
                case "jpeg":
                default:
                    return encoders.FirstOrDefault(e => e.MimeType == "image/jpeg");
                case "gif":
                    return encoders.FirstOrDefault(e => e.MimeType == "image/gif");
                case "bmp":
                    return encoders.FirstOrDefault(e => e.MimeType == "image/bmp");
                case "tiff":
                    return encoders.FirstOrDefault(e => e.MimeType == "image/tiff");
                case "png":
                    return encoders.FirstOrDefault(e => e.MimeType == "image/png");
            }
        }

        /// <summary>
        /// Save image
        /// </summary>
        /// <param name="image">Image object</param>
        /// <param name="imageSavingOptionsAction">Image saving options action</param>
        /// <returns>Return file path</returns>
        static string SaveImage(Image image, Action<ImageSavingOptions> imageSavingOptionsAction)
        {
            var savingOptions = new ImageSavingOptions();
            imageSavingOptionsAction?.Invoke(savingOptions);
            string path = savingOptions.Path;
            if (string.IsNullOrWhiteSpace(path))
            {
                path = Directory.GetCurrentDirectory();
            }
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), path);
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string fileName = savingOptions.FileName;
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = $"{GuidHelper.GetGuidUniqueCode()}.png";
            }
            string fileFullPath = Path.Combine(path, fileName);
            string extension = Path.HasExtension(fileName) ? Path.GetExtension(fileName) : "png";
            ImageCodecInfo encodeInfo = GetImageEncoderByExtension(extension);
            EncoderParameters parameters = new EncoderParameters();
            DefaultEncoderParametersAction(parameters);
            savingOptions?.EncoderParametersAction?.Invoke(parameters);
            image.Save(fileFullPath, encodeInfo, parameters);
            return Path.Combine(savingOptions.Path ?? string.Empty, fileName);
        }

        /// <summary>
        /// Save image
        /// </summary>
        /// <param name="images">Image list</param>
        /// <param name="imageSavingOptionsAction">Image saving options action</param>
        /// <returns>Return file path</returns>
        static List<string> SaveImage(List<Image> images, Action<ImageSavingOptions> imageSavingOptionsAction)
        {
            if (images.IsNullOrEmpty())
            {
                return new List<string>(0);
            }
            var savingOptions = new ImageSavingOptions();
            imageSavingOptionsAction?.Invoke(savingOptions);
            string path = savingOptions.Path;
            if (string.IsNullOrWhiteSpace(path))
            {
                path = Directory.GetCurrentDirectory();
            }
            if (!Path.IsPathRooted(path))
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), path);
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string extension = !string.IsNullOrWhiteSpace(savingOptions.FileName) && Path.HasExtension(savingOptions.FileName) ? Path.GetExtension(savingOptions.FileName) : "png";
            ImageCodecInfo encodeInfo = GetImageEncoderByExtension(extension);
            EncoderParameters parameters = new EncoderParameters();
            DefaultEncoderParametersAction(parameters);
            savingOptions?.EncoderParametersAction?.Invoke(parameters);

            List<string> newFilePaths = new List<string>(images.Count);
            foreach (var image in images)
            {
                string fileName = $"{newFilePaths.Count + 1}.{extension}";
                string fileFullPath = Path.Combine(path, fileName);
                image.Save(fileFullPath, encodeInfo, parameters);
                newFilePaths.Add(Path.Combine(savingOptions.Path ?? string.Empty, fileName));
            }
            return newFilePaths;
        }

        /// <summary>
        /// Get image from file
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <returns>Return an image object</returns>
        static Image GetImageFromFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }
            if (!Path.IsPathRooted(filePath))
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), filePath);
            }
            return Image.FromFile(filePath);
        }

        /// <summary>
        /// Calculate center part ordinate
        /// </summary>
        /// <param name="originalImage">Original image</param>
        /// <param name="cutWidth">Cut width</param>
        /// <param name="cutHeight">Cut height</param>
        /// <returns></returns>
        static Tuple<int, int> CalculateCenterPartOrdinate(Image originalImage, int cutWidth, int cutHeight)
        {
            int cutHorizontalOrdinate = 0;
            int cutVerticalOrdinate = 0;
            if (originalImage.Width > cutWidth)
            {
                cutHorizontalOrdinate = (originalImage.Width - cutWidth) / 2;
            }
            if (originalImage.Height > cutHeight)
            {
                cutVerticalOrdinate = (originalImage.Height - cutHeight) / 2;
            }
            return Tuple.Create(cutHorizontalOrdinate, cutVerticalOrdinate);
        }

        #endregion
    }
}
