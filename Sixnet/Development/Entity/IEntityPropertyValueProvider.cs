namespace Sixnet.Development.Entity
{
    internal interface IEntityPropertyValueProvider
    {
        object Get(object data);

        void Set(object data, object value);
    }
}
