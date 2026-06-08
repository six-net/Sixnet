// "Company © 2025. All rights reserved."

namespace Sixnet.Expressions.Linq
{
    // Serves as the base class for all expression fingerprints. Provides a default implementation
    // of GetHashCode().

    public abstract class SixnetExpressionFingerprint
    {
        protected SixnetExpressionFingerprint(ExpressionType nodeType, Type type)
        {
            NodeType = nodeType;
            Type = type;
        }

        // the type of expression node, e.g. OP_ADD, MEMBER_ACCESS, etc.
        public ExpressionType NodeType { get; private set; }

        // the CLR type resulting from this expression, e.g. int, string, etc.
        public Type Type { get; private set; }

        internal virtual void AddToHashCodeCombiner(SixnetHashCodeCombiner combiner)
        {
            combiner.AddInt32((int)NodeType);
            combiner.AddObject(Type);
        }

        protected bool Equals(SixnetExpressionFingerprint other)
        {
            return other != null
                   && NodeType == other.NodeType
                   && Equals(Type, other.Type);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SixnetExpressionFingerprint);
        }

        public override int GetHashCode()
        {
            SixnetHashCodeCombiner combiner = new SixnetHashCodeCombiner();
            AddToHashCodeCombiner(combiner);
            return combiner.CombinedHash;
        }
    }
}
