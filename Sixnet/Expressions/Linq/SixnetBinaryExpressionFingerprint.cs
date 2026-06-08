// "Company © 2025. All rights reserved."

using System.Diagnostics.CodeAnalysis;
using System.Reflection;

#pragma warning disable 659 // overrides AddToHashCodeCombiner instead

namespace Sixnet.Expressions.Linq
{
    // BinaryExpression fingerprint class
    // Useful for things like array[index]

    [SuppressMessage("Microsoft.Usage", "CA2218:OverrideGetHashCodeOnOverridingEquals", Justification = "Overrides AddToHashCodeCombiner() instead.")]
    public sealed class SixnetBinaryExpressionFingerprint : SixnetExpressionFingerprint
    {
        public SixnetBinaryExpressionFingerprint(ExpressionType nodeType, Type type, MethodInfo method)
            : base(nodeType, type)
        {
            // Other properties on BinaryExpression (like IsLifted / IsLiftedToNull) are simply derived
            // from Type and NodeType, so they're not necessary for inclusion in the fingerprint.

            Method = method;
        }

        // http://msdn.microsoft.com/en-us/library/system.linq.expressions.binaryexpression.method.aspx
        public MethodInfo Method { get; private set; }

        public override bool Equals(object obj)
        {
            SixnetBinaryExpressionFingerprint other = obj as SixnetBinaryExpressionFingerprint;
            return other != null
                   && Equals(Method, other.Method)
                   && Equals(other);
        }

        internal override void AddToHashCodeCombiner(SixnetHashCodeCombiner combiner)
        {
            combiner.AddObject(Method);
            base.AddToHashCodeCombiner(combiner);
        }
    }
}
