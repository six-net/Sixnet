using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sixnet.Validation
{
    public class AsyncValidatorRuleOptions
    {
        /// <summary>
        /// Gets or sets the model type
        /// </summary>
        public Type ModelType { get; set; }

        /// <summary>
        /// Required
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Key prefixs
        /// </summary>
        public List<string> KeyPrefixs { get; set; }

        /// <summary>
        /// Allow null field names
        /// </summary>
        public List<string> AllowNullFieldNames { get; set; }

        internal string GetOptionsKey()
        {
            return $"{ModelType.FullName}{Required}{(KeyPrefixs.IsNullOrEmpty() ? string.Empty : string.Join("",KeyPrefixs?.OrderBy(c=>c)))}{(AllowNullFieldNames.IsNullOrEmpty() ? string.Empty : string.Join("", AllowNullFieldNames?.OrderBy(c => c)))}";
        }
    }
}
