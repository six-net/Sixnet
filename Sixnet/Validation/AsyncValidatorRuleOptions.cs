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

        /// <summary>
        /// Trigger
        /// </summary>
        public AsyncValidatorRuleTriggerType Trigger { get; set; } = AsyncValidatorRuleTriggerType.Change;

        /// <summary>
        /// Field trigger
        /// </summary>
        public Dictionary<string, AsyncValidatorRuleTriggerType> FieldTriggers { get; set; }

        internal string GetOptionsKey()
        {
            var keyPrefixKey = KeyPrefixs.IsNullOrEmpty() ? string.Empty : string.Join("", KeyPrefixs.OrderBy(c => c));
            var nullFieldNameKey = AllowNullFieldNames.IsNullOrEmpty() ? string.Empty : string.Join("", AllowNullFieldNames.OrderBy(c => c));
            var fieldTriggerKey = FieldTriggers.IsNullOrEmpty() ? string.Empty : string.Join("", FieldTriggers.OrderBy(c => c.Key).Select(ck => $"{ck.Key}{ck.Value}"));
            return $"{ModelType.FullName}{Required}{Trigger}{keyPrefixKey}{nullFieldNameKey}{fieldTriggerKey}";
        }
    }

    public enum AsyncValidatorRuleTriggerType
    {
        Change = 1,
        Blur = 2
    }
}
