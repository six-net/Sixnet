using EZNEW.Development.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Development.Domain.Model
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ModelMetadataAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the model type
        /// </summary>
        internal Type ModelType { get; set; }

        /// <summary>
        /// Gets or sets the feature
        /// </summary>
        public ModelFeature Feature { get; set; } = ModelFeature.Default;

        /// <summary>
        /// Gets or sets the relation type
        /// </summary>
        public Type EntityType { get; set; }
    }
}
