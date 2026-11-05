// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.Development.Entity
{
    public interface ISixnetIdEntity<TId> : ISixnetEntity
    {
        /// <summary>
        /// Gets or sets Id
        /// </summary>
        TId Id { get; set; }
    }
}
