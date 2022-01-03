using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Development.Entity
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class NonDataFieldAttribute : Attribute
    {
    }
}
