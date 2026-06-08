// "Company © 2025. All rights reserved."

namespace Sixnet.Expressions.Linq
{
    // This is a visitor which rewrites constant expressions as parameter lookups. It's meant
    // to produce an expression which can be cached safely.

    public sealed class SixnetHoistingExpressionVisitor<TIn, TOut> : ExpressionVisitor
    {
        private static readonly ParameterExpression _hoistedConstantsParamExpr = Expression.Parameter(typeof(List<object>), "hoistedConstants");
        private int _numConstantsProcessed;

        // factory will create instance
        private SixnetHoistingExpressionVisitor()
        {
        }

        public static Expression<SixnetHoisted<TIn, TOut>> Hoist(Expression<Func<TIn, TOut>> expr)
        {
            // rewrite Expression<Func<TIn, TOut>> as Expression<Hoisted<TIn, TOut>>

            var visitor = new SixnetHoistingExpressionVisitor<TIn, TOut>();
            var rewrittenBodyExpr = visitor.Visit(expr.Body);
            var rewrittenLambdaExpr = Expression.Lambda<SixnetHoisted<TIn, TOut>>(rewrittenBodyExpr, expr.Parameters[0], _hoistedConstantsParamExpr);
            return rewrittenLambdaExpr;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            // rewrite the constant expression as (TConst)hoistedConstants[i];
            return Expression.Convert(Expression.Property(_hoistedConstantsParamExpr, "Item", Expression.Constant(_numConstantsProcessed++)), node.Type);
        }
    }
}
