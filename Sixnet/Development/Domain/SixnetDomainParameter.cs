using System.ComponentModel.DataAnnotations;
using Sixnet.Model;

namespace Sixnet.Development.Domain
{
    /// <summary>
    /// Domain parameter
    /// </summary>
    public abstract class SixnetDomainParameter : SixnetDomainModel, ISixnetCheckable
    {
        /// <summary>
        /// Check parameter
        /// </summary>
        public virtual void Check()
        {
        }
    }
}
