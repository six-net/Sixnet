﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.DataValidation
{
    /// <summary>
    /// Defines validation rule contract
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IValidationRule<T>
    {
        /// <summary>
        /// Gets or sets the field
        /// </summary>
        ValidationField<T> Field { get; set; }
    }
}
