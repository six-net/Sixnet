// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Field;
using Sixnet.Model;

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Sort entry
    /// </summary>
    [Serializable]
    public class SixnetSortEntry : ISixnetCloneable<SixnetSortEntry>
    {
        /// <summary>
        /// Gets or sets the field
        /// </summary>
        public ISixnetField Field { get; set; }

        /// <summary>
        /// Indicates whether order by desc
        /// </summary>
        public bool Desc { get; set; }

        /// <summary>
        /// Gets or sets the sort options
        /// </summary>
        public SixnetSortOptions Options { get; set; }

        /// <summary>
        /// Create sort entry
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="desc"></param>
        /// <param name="modelType"></param>
        /// <param name="sortOptions"></param>
        /// <returns></returns>
        public static SixnetSortEntry Create(string propertyName, bool desc, Type modelType = null, SixnetSortOptions sortOptions = null)
        {
            return new SixnetSortEntry()
            {
                Field = SixnetDataField.Create(propertyName, modelType),
                Desc = desc,
                Options = sortOptions
            };
        }

        /// <summary>
        /// Clone a sort entry
        /// </summary>
        /// <returns></returns>
        public SixnetSortEntry Clone()
        {
            return new SixnetSortEntry()
            {
                Field = Field?.Clone(),
                Desc = Desc,
                Options = Options?.Clone()
            };
        }

        public override int GetHashCode()
        {
            return Field?.GetHashCode() ?? 0;
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
            {
                return Field?.Equals(obj) ?? false;
            }
            return true;
        }
    }
}
