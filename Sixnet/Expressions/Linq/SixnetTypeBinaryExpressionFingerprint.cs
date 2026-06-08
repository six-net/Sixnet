// "Company © 2025. All rights reserved."

using System.Diagnostics.CodeAnalysis;

#pragma warning disable 659 // overrides AddToHashCodeCombiner instead

namespace Sixnet.Expressions.Linq
{
    // TypeBinary fingerprint class
    // Expression of form "obj is T"

    [SuppressMessage("Microsoft.Usage", "CA2218:OverrideGetHashCodeOnOverridingEquals", Justification = "Overrides AddToHashCodeCombiner() instead.")]
    public sealed class SixnetTypeBinaryExpressionFingerprint : SixnetExpressionFingerprint
    {
        public SixnetTypeBinaryExpressionFingerprint(ExpressionType nodeType, Type type, Type typeOperand)
            : base(nodeType, type)
        {
            TypeOperand = typeOperand;
        }

        // http://msdn.microsoft.com/en-us/library/system.linq.expressions.typebinaryexpression.typeoperand.aspx
        public Type TypeOperand { get; private set; }

        public override bool Equals(object obj)
        {
            SixnetTypeBinaryExpressionFingerprint other = obj as SixnetTypeBinaryExpressionFingerprint;
            return other != null
                   && Equals(TypeOperand, other.TypeOperand)
                   && Equals(other);
        }

        internal override void AddToHashCodeCombiner(SixnetHashCodeCombiner combiner)
        {
            combiner.AddObject(TypeOperand);
            base.AddToHashCodeCombiner(combiner);
        }
    }
}
