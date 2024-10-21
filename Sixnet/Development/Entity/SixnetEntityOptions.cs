using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Development.Entity
{
    public class SixnetEntityOptions
    {
        /// <summary>
        /// Not auto generate id
        /// </summary>
        public bool NotAutoGenerageId { get; set; }

        /// <summary>
        /// Not auto move uploaded file
        /// </summary>
        public bool NotAutoStoreUploadedFile {  get; set; }
    }
}
