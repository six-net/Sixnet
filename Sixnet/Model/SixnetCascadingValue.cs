// "Company © 2025. All rights reserved."

using Sixnet.Serialization.Json;

namespace Sixnet.Model
{
    public class SixnetCascadingValue<TValue>
    {
        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public TValue Value { get; set; }

        /// <summary>
        /// Gets or sets the label
        /// </summary>
        [SixnetLocalString]
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the level
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Gets or sets the sequence
        /// </summary>
        public double Sequence { get; set; }

        /// <summary>
        /// Gets or sets the source data
        /// </summary>
        public object SourceData {  get; set; }

        /// <summary>
        /// Wheter is leaf node
        /// </summary>
        public bool IsLeaf {  get; set; }

        /// <summary>
        /// Gets or sets the children
        /// </summary>
        public List<SixnetCascadingValue<TValue>> Children { get; set; }
    }
}
