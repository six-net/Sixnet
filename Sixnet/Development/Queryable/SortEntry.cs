using System;
using Sixnet.Development.Data.Field;
using Sixnet.Model;

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Sort entry
    /// </summary>
    [Serializable]
    public class SortEntry : ISixnetCloneable<SortEntry>
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
        public SortOptions Options { get; set; }

        /// <summary>
        /// Create sort entry
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="desc"></param>
        /// <param name="modelType"></param>
        /// <param name="sortOptions"></param>
        /// <returns></returns>
        public static SortEntry Create(string propertyName, bool desc, Type modelType = null, SortOptions sortOptions = null)
        {
            return new SortEntry()
            {
                Field = DataField.Create(propertyName, modelType),
                Desc = desc,
                Options = sortOptions
            };
        }

        /// <summary>
        /// Clone a sort entry
        /// </summary>
        /// <returns></returns>
        public SortEntry Clone()
        {
            return new SortEntry()
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
