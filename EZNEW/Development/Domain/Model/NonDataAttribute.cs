using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Development.Domain.Model
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class NonDataAttribute : Attribute
    {
    }
}
