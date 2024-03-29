﻿using System.ComponentModel.DataAnnotations;
using Sixnet.Model;

namespace Sixnet.Development.Domain
{
    /// <summary>
    /// Domain parameter
    /// </summary>
    public abstract class SixnetDomainParameter : SixnetDomainModel, IValidatableModel
    {
        /// <summary>
        /// Validate
        /// </summary>
        public virtual void Validate()
        {
        }
    }
}
