// "Company © 2025. All rights reserved."

namespace Sixnet.Expressions.Linq
{
    public delegate TValue Hoisted<TModel, TValue>(TModel model, List<object> capturedConstants);
}
