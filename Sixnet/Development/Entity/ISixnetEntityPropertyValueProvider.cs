// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Entity
{
    internal interface ISixnetEntityPropertyValueProvider
    {
        object Get(object data);

        void Set(object data, object value);
    }
}
