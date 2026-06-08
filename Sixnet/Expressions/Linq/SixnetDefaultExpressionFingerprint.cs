// "Company © 2025. All rights reserved."

using System.Diagnostics.CodeAnalysis;

#pragma warning disable 659 // overrides AddToHashCodeCombiner instead

namespace Sixnet.Expressions.Linq
{
    // DefaultExpression fingerprint class
    // Expression of form default(T)

    [SuppressMessage("Microsoft.Usage", "CA2218:OverrideGetHashCodeOnOverridingEquals", Justification = "Overrides AddToHashCodeCombiner() instead.")]
    public sealed class SixnetDefaultExpressionFingerprint : SixnetExpressionFingerprint
    {
        public SixnetDefaultExpressionFingerprint(ExpressionType nodeType, Type type)
            : base(nodeType, type)
        {
            // There are no properties on DefaultExpression that are worth including in
            // the fingerprint.
        }

        public override bool Equals(object obj)
        {
            SixnetDefaultExpressionFingerprint other = obj as SixnetDefaultExpressionFingerprint;
            return other != null
                   && Equals(other);
        }
    }
}
