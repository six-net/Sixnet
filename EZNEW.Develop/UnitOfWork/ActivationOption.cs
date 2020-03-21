using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.UnitOfWork
{
    /// <summary>
    /// activation option
    /// </summary>
    public class ActivationOption
    {
        public static readonly ActivationOption Default = new ActivationOption();

        /// <summary>
        /// force execute
        /// </summary>
        public bool ForceExecute
        {
            get; set;
        }
    }
}
