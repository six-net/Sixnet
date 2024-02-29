using System;

namespace Sixnet.Validation
{
    /// <summary>
    /// Defines compare operator
    /// </summary>
    [Serializable]
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
    /// Defines range boundary
    /// </summary>
    [Serializable]
    public enum RangeBoundary
    {
        Include,
        NotInclude
    }

    /// <summary>
    /// Defines validator type
    /// </summary>
    [Serializable]
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
        Url,
        Integer,
        PositiveInteger,
        PositiveIntegerOrZero,
        NegativeInteger,
        NegativeIntegerOrZero,
        Fraction,
        PositiveFraction,
        NegativeFraction,
        PositiveFractionOrZero,
        NegativeFractionOrZero,
        Number,
        Color,
        Chinese,
        PostCode,
        Mobile,
        IPV4,
        Date,
        DateTime,
        Letter,
        UpperLetter,
        LowerLetter,
        IdentityCard,
        ImageFile,
        CompressFile
    }
}
