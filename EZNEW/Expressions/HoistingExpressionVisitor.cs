// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EZNEW.Expressions
{
    // This is a visitor which rewrites constant expressions as parameter lookups. It's meant
    // to produce an expression which can be cached safely.

    public sealed class HoistingExpressionVisitor<TIn, TOut> : ExpressionVisitor
    {
        private static readonly ParameterExpression _hoistedConstantsParamExpr = System.Linq.Expressions.Expression.Parameter(typeof(List<object>), "hoistedConstants");
        private int _numConstantsProcessed;

        // factory will create instance
        private HoistingExpressionVisitor()
        {
        }

        public static Expression<Hoisted<TIn, TOut>> Hoist(Expression<Func<TIn, TOut>> expr)
        {
            // rewrite Expression<Func<TIn, TOut>> as Expression<Hoisted<TIn, TOut>>

            var visitor = new HoistingExpressionVisitor<TIn, TOut>();
            var rewrittenBodyExpr = visitor.Visit(expr.Body);
            var rewrittenLambdaExpr = System.Linq.Expressions.Expression.Lambda<Hoisted<TIn, TOut>>(rewrittenBodyExpr, expr.Parameters[0], _hoistedConstantsParamExpr);
            return rewrittenLambdaExpr;
        }

        protected override System.Linq.Expressions.Expression VisitConstant(ConstantExpression node)
        {
            // rewrite the constant expression as (TConst)hoistedConstants[i];
            return System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Property(_hoistedConstantsParamExpr, "Item", System.Linq.Expressions.Expression.Constant(_numConstantsProcessed++)), node.Type);
        }
    }
}
