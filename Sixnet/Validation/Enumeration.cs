// "Company © 2025. All rights reserved."

namespace Sixnet.Validation
{
    /// <summary>
    /// Defines compare operator
    /// </summary>
    [Serializable]
    public enum SixnetCompareOperator
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
    public enum SixnetRangeBoundary
    {
        Include,
        NotInclude
    }

    /// <summary>
    /// Defines validator type
    /// </summary>
    [Serializable]
    public enum SixnetValidatorType
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
