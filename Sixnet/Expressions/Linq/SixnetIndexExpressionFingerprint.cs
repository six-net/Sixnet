// "Company © 2025. All rights reserved."

using System.Diagnostics.CodeAnalysis;
using System.Reflection;

#pragma warning disable 659 // overrides AddToHashCodeCombiner instead

namespace Sixnet.Expressions.Linq
{
    // IndexExpression fingerprint class
    // Represents certain forms of array access or indexer property access

    [SuppressMessage("Microsoft.Usage", "CA2218:OverrideGetHashCodeOnOverridingEquals", Justification = "Overrides AddToHashCodeCombiner() instead.")]
    public sealed class SixnetIndexExpressionFingerprint : SixnetExpressionFingerprint
    {
        public SixnetIndexExpressionFingerprint(ExpressionType nodeType, Type type, PropertyInfo indexer)
            : base(nodeType, type)
        {
            // Other properties on IndexExpression (like the argument count) are simply derived
            // from Type and Indexer, so they're not necessary for inclusion in the fingerprint.

            Indexer = indexer;
        }

        // http://msdn.microsoft.com/en-us/library/system.linq.expressions.indexexpression.indexer.aspx
        public PropertyInfo Indexer { get; private set; }

        public override bool Equals(object obj)
        {
            SixnetIndexExpressionFingerprint other = obj as SixnetIndexExpressionFingerprint;
            return other != null
                   && Equals(Indexer, other.Indexer)
                   && Equals(other);
        }

        internal override void AddToHashCodeCombiner(SixnetHashCodeCombiner combiner)
        {
            combiner.AddObject(Indexer);
            base.AddToHashCodeCombiner(combiner);
        }
    }
}
