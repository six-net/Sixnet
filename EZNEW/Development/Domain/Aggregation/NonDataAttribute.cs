using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Development.Domain.Aggregation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class NonDataAttribute : Attribute
    {
    }
}
