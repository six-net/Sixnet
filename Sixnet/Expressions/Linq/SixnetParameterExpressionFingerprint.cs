// "Company © 2025. All rights reserved."

using System.Diagnostics.CodeAnalysis;

#pragma warning disable 659 // overrides AddToHashCodeCombiner instead

namespace Sixnet.Expressions.Linq
{
    // ParameterExpression fingerprint class
    // Can represent the model parameter or an inner parameter in an open lambda expression

    [SuppressMessage("Microsoft.Usage", "CA2218:OverrideGetHashCodeOnOverridingEquals", Justification = "Overrides AddToHashCodeCombiner() instead.")]
    public sealed class SixnetParameterExpressionFingerprint : SixnetExpressionFingerprint
    {
        public SixnetParameterExpressionFingerprint(ExpressionType nodeType, Type type, int parameterIndex)
            : base(nodeType, type)
        {
            ParameterIndex = parameterIndex;
        }

        // Parameter position within the overall expression, used to maintain alpha equivalence.
        public int ParameterIndex { get; private set; }

        public override bool Equals(object obj)
        {
            SixnetParameterExpressionFingerprint other = obj as SixnetParameterExpressionFingerprint;
            return other != null
                   && ParameterIndex == other.ParameterIndex
                   && Equals(other);
        }

        internal override void AddToHashCodeCombiner(SixnetHashCodeCombiner combiner)
        {
            combiner.AddInt32(ParameterIndex);
            base.AddToHashCodeCombiner(combiner);
        }
    }
}
