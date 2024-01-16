using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Sixnet.Constants;
using Sixnet.Exceptions;

namespace Sixnet.Model
{
    public static class SixnetExtensions
    {
        /// <summary>
        /// Validate
        /// </summary>
        /// <param name="validatableObject">Validatable object</param>
        /// <param name="nullMessage">Message when validatableObject is null</param>
        public static void Validate(this IValidatableModel validatableObject, string nullMessage = "")
        {
            ThrowHelper.ThrowArgNullIf(validatableObject == null, string.IsNullOrWhiteSpace(nullMessage) ? ExceptionMessages.ArgumentIsNull : nullMessage);

            validatableObject.Validate();
        }
    }
}
