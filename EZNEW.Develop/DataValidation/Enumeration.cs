using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.DataValidation
{
    /// <summary>
    /// Compare Operator
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
    /// Range Boundary
    /// </summary>
    public enum RangeBoundary
    {
        Include,
        NotInclude
    }

    /// <summary>
    /// Validator Type
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
