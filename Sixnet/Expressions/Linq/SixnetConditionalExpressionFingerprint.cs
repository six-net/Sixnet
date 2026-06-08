// "Company © 2025. All rights reserved."

using System.Diagnostics.CodeAnalysis;

#pragma warning disable 659 // overrides AddToHashCodeCombiner instead

namespace Sixnet.Expressions.Linq
{
    // ConditionalExpression fingerprint class
    // Expression of form (test) ? ifTrue : ifFalse

    [SuppressMessage("Microsoft.Usage", "CA2218:OverrideGetHashCodeOnOverridingEquals", Justification = "Overrides AddToHashCodeCombiner() instead.")]
    public sealed class SixnetConditionalExpressionFingerprint : SixnetExpressionFingerprint
    {
        public SixnetConditionalExpressionFingerprint(ExpressionType nodeType, Type type)
            : base(nodeType, type)
        {
            // There are no properties on ConditionalExpression that are worth including in
            // the fingerprint.
        }

        public override bool Equals(object obj)
        {
            SixnetConditionalExpressionFingerprint other = obj as SixnetConditionalExpressionFingerprint;
            return other != null
                   && Equals(other);
        }
    }
}
