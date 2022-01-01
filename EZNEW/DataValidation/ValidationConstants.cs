using System.ComponentModel.DataAnnotations;

namespace EZNEW.DataValidation
{
    /// <summary>
    /// Validation constants
    /// </summary>
    public static class ValidationConstants
    {
        /// <summary>
        /// File extensions
        /// </summary>
        public static class FileExtensions
        {
            /// <summary>
            /// Gets the image file extensions
            /// </summary>
            public const string ImageFile = "jpg,bmp,gif,ico,pcx,jpeg,tif,png,raw,tga";

            /// <summary>
            /// Gets the compress file extensions
            /// </summary>
            public const string CompressFile = "rar,zip,7zip,tgz";
        }

        /// <summary>
        /// Default attributes
        /// </summary>
        public static class DefaultAttributes
        {
            /// <summary>
            /// Email address attribute
            /// </summary>
            public static readonly EmailAddressAttribute Email = new EmailAddressAttribute();

            /// <summary>
            /// Url attribute
            /// </summary>
            public static readonly UrlAttribute Url = new UrlAttribute();

            /// <summary>
            /// Credit card
            /// </summary>
            public static readonly CreditCardAttribute CreditCard = new CreditCardAttribute();

            /// <summary>
            /// Phone
            /// </summary>
            public static readonly PhoneAttribute Phone = new PhoneAttribute();

            /// <summary>
            /// Image file
            /// </summary>
            public static readonly FileExtensionsAttribute ImageFile = new FileExtensionsAttribute()
            {
                Extensions = FileExtensions.ImageFile
            };

            /// <summary>
            /// Compress file
            /// </summary>
            public static readonly FileExtensionsAttribute CompressFile = new FileExtensionsAttribute()
            {
                Extensions = FileExtensions.CompressFile
            };
        }

        /// <summary>
        /// Defines default use case names
        /// </summary>
        public static class UseCaseNames
        {
            public const string Domain = nameof(Domain);

            public const string Mvc = nameof(Mvc);
        }
    }
}
