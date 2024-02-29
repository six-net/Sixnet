using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Model
{
    /// <summary>
    /// Defines validatable model
    /// </summary>
    public interface ISixnetValidatableModel
    {
        /// <summary>
        /// Validate value
        /// </summary>
        void ValidateValue();
    }
}
