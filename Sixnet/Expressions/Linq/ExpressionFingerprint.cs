﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;

namespace Sixnet.Expressions.Linq
{
    // Serves as the base class for all expression fingerprints. Provides a default implementation
    // of GetHashCode().

    public abstract class ExpressionFingerprint
    {
        protected ExpressionFingerprint(ExpressionType nodeType, Type type)
        {
            NodeType = nodeType;
            Type = type;
        }

        // the type of expression node, e.g. OP_ADD, MEMBER_ACCESS, etc.
        public ExpressionType NodeType { get; private set; }

        // the CLR type resulting from this expression, e.g. int, string, etc.
        public Type Type { get; private set; }

        internal virtual void AddToHashCodeCombiner(HashCodeCombiner combiner)
        {
            combiner.AddInt32((int)NodeType);
            combiner.AddObject(Type);
        }

        protected bool Equals(ExpressionFingerprint other)
        {
            return other != null
                   && NodeType == other.NodeType
                   && Equals(Type, other.Type);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ExpressionFingerprint);
        }

        public override int GetHashCode()
        {
            HashCodeCombiner combiner = new HashCodeCombiner();
            AddToHashCodeCombiner(combiner);
            return combiner.CombinedHash;
        }
    }
}
