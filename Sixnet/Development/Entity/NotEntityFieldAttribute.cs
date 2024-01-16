using System;

namespace Sixnet.Development.Entity
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class NotEntityFieldAttribute : Attribute
    {
    }
}
