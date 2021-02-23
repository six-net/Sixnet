using System;
using System.Linq.Expressions;

namespace EZNEW.Develop.Entity
{
    internal class DefaultEntityPropertyValueProvider<T> : IEntityPropertyValueProvider
    {
        internal Func<T, object> Getter { get; set; }

        internal Action<T, object> Setter { get; set; }

        public void SetGetter(Expression<Func<T, object>> expression)
        {
            if (expression != null)
            {
                Getter = expression.Compile();
            }
        }

        public void SetSetter(Expression<Action<T, object>> expression)
        {
            if (expression != null)
            {
                Setter = expression.Compile();
            }
        }

        public object Get(object data)
        {
            if (data is T instance && Getter != null)
            {
                return Getter(instance);
            }
            return null;
        }

        public void Set(object data, object value)
        {
            if (data is T instance && Setter != null)
            {
                Setter(instance, value);
            }
        }
    }
}
