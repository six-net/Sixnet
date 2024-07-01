using System;
using System.Collections.Generic;
using System.Text;
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
        [LocalString]
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
        /// Gets or sets the children
        /// </summary>
        public List<SixnetCascadingValue<TValue>> Children { get; set; }
    }
}
