// "Company © 2025. All rights reserved."

using System.Diagnostics.CodeAnalysis;
using System.Reflection;

#pragma warning disable 659 // overrides AddToHashCodeCombiner instead

namespace Sixnet.Expressions.Linq
{
    // MemberExpression fingerprint class
    // Expression of form xxx.FieldOrProperty

    [SuppressMessage("Microsoft.Usage", "CA2218:OverrideGetHashCodeOnOverridingEquals", Justification = "Overrides AddToHashCodeCombiner() instead.")]
    public sealed class SixnetMemberExpressionFingerprint : SixnetExpressionFingerprint
    {
        public SixnetMemberExpressionFingerprint(ExpressionType nodeType, Type type, MemberInfo member)
            : base(nodeType, type)
        {
            Member = member;
        }

        // http://msdn.microsoft.com/en-us/library/system.linq.expressions.memberexpression.member.aspx
        public MemberInfo Member { get; private set; }

        public override bool Equals(object obj)
        {
            SixnetMemberExpressionFingerprint other = obj as SixnetMemberExpressionFingerprint;
            return other != null
                   && Equals(Member, other.Member)
                   && Equals(other);
        }

        internal override void AddToHashCodeCombiner(SixnetHashCodeCombiner combiner)
        {
            combiner.AddObject(Member);
            base.AddToHashCodeCombiner(combiner);
        }
    }
}
