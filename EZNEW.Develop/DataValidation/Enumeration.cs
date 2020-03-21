using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.DataValidation
{
    /// <summary>
    /// compare operator
    /// </summary>
    public enum CompareOperator
    {
        Equal,
        NotEqual,
        LessThanOrEqual,
        LessThan,
        GreaterThan,
        GreaterThanOrEqual,
        In,
        NotIn,
    }

    /// <summary>
    /// range boundary
    /// </summary>
    public enum RangeBoundary
    {
        Include,
        NotInclude
    }

    /// <summary>
    /// validator type
    /// </summary>
    public enum ValidatorType
    {
        Compare,
        CreditCard,
        Email,
        EnumType,
        MaxLength,
        MinLength,
        Phone,
        Range,
        RegularExpression,
        Required,
        StringLength,
        Url
    }
}
