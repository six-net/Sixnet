// "Company © 2025. All rights reserved."

using System.Reflection;

using Sixnet.Reflection;

namespace Sixnet.Validation
{
    /// <summary>
    /// Validation config
    /// </summary>
    public class SixnetValidationConfig
    {
        #region Fields

        /// <summary>
        /// Expression type
        /// </summary>
        static readonly Type _expressionType = typeof(Expression);

        /// <summary>
        /// Lambda method
        /// </summary>
        static readonly MethodInfo _lambdaMethod = null;

        /// <summary>
        /// Validation methods
        /// </summary>
        static readonly List<MethodInfo> _validationMethods = new();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the type validation rules
        /// </summary>
        public List<SixnetTypeValidationSetting> Types { get; set; }

        #endregion

        #region Constructor

        static SixnetValidationConfig()
        {
            var baseExpressMethods = _expressionType.GetMethods(BindingFlags.Public | BindingFlags.Static);
            _lambdaMethod = baseExpressMethods.FirstOrDefault(c => c.Name == "Lambda" && c.IsGenericMethod && c.GetParameters()[1].ParameterType.FullName == typeof(ParameterExpression[]).FullName);
            _validationMethods = typeof(SixnetValidations).GetMethods().ToList();
        }

        #endregion

        #region Build validation

        internal void BuildValidation()
        {
            if (Types.IsNullOrEmpty())
            {
                return;
            }
            foreach (var typeRule in Types)
            {
                if (typeRule == null || string.IsNullOrWhiteSpace(typeRule.TypeAssemblyQualifiedName) || typeRule.Properties == null)
                {
                    continue;
                }
                Type modelType = Type.GetType(typeRule.TypeAssemblyQualifiedName);
                if (modelType == null)
                {
                    continue;
                }
                //load properties and fields
                List<MemberInfo> memberInfoList = new List<MemberInfo>();
                memberInfoList.AddRange(modelType.GetFields(BindingFlags.Public | BindingFlags.Instance));
                memberInfoList.AddRange(modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance));
                //parameter expression
                ParameterExpression parameterExpression = Expression.Parameter(modelType);
                Array parameterArray = Array.CreateInstance(typeof(ParameterExpression), 1);
                parameterArray.SetValue(parameterExpression, 0);
                Type valFieldType = typeof(SixnetValidationField<>).MakeGenericType(modelType);
                foreach (var propertyRule in typeRule.Properties)
                {
                    if (propertyRule == null || propertyRule.Rules == null)
                    {
                        return;
                    }
                    string[] propertyNameArray = propertyRule.Name.LSplit(".");
                    Expression propertyExpress = null;
                    foreach (string pname in propertyNameArray)
                    {
                        if (propertyExpress == null)
                        {
                            propertyExpress = Expression.PropertyOrField(parameterExpression, pname);
                        }
                        else
                        {
                            propertyExpress = Expression.PropertyOrField(propertyExpress, pname);
                        }
                    }
                    Type funcType = typeof(Func<,>).MakeGenericType(modelType, typeof(object));//function type
                    var genericLambdaMethod = SixnetReflecter.Expression.LambdaMethod.MakeGenericMethod(funcType);
                    var lambdaExpression = genericLambdaMethod.Invoke(null, new object[]
                    {
                        Expression.Convert(propertyExpress,typeof(object)),parameterArray
                    });

                    if (lambdaExpression == null)
                    {
                        continue;
                    }
                    foreach (var rule in propertyRule.Rules)
                    {
                        var fieldInstance = Activator.CreateInstance(valFieldType);
                        valFieldType.GetProperty("Field").SetValue(fieldInstance, lambdaExpression);
                        valFieldType.GetProperty("ErrorMessage").SetValue(fieldInstance, rule.ErrorMessage);
                        valFieldType.GetProperty("TipMessage").SetValue(fieldInstance, rule.TipMessage);
                        valFieldType.GetProperty("IgnoreUseScenarios").SetValue(fieldInstance, rule.IgnoreUseScenarios);
                        Array valueFieldArray = Array.CreateInstance(valFieldType, 1);
                        valueFieldArray.SetValue(fieldInstance, 0);
                        switch (rule.Type)
                        {
                            case SixnetValidatorType.EnumType:
                                Type enumType = Type.GetType(rule.EnumType);
                                BuildEnumValidation(modelType, enumType, valueFieldArray);
                                break;
                            case SixnetValidatorType.MaxLength:
                                BuidMaxLengthValidation(modelType, System.Convert.ToInt32(rule.MaxValue), valueFieldArray);
                                break;
                            case SixnetValidatorType.MinLength:
                                BuildMinLengthValidation(modelType, System.Convert.ToInt32(rule.MinValue), valueFieldArray);
                                break;
                            case SixnetValidatorType.Range:
                                BuildRangeValidation(modelType, rule.MinValue, rule.MaxValue, rule.LowerBoundary, rule.UpperBoundary, valueFieldArray);
                                break;
                            case SixnetValidatorType.RegularExpression:
                                BuildRegularExpressionValidation(modelType, rule.Value, valueFieldArray);
                                break;
                            case SixnetValidatorType.StringLength:
                                BuildStringLengthValidation(modelType, System.Convert.ToInt32(rule.MinValue), System.Convert.ToInt32(rule.MaxValue), valueFieldArray);
                                break;
                            case SixnetValidatorType.Compare:
                                BuildCompareValidation(modelType, rule.Value, rule.CompareType, parameterExpression, funcType, parameterArray, rule.Operator, fieldInstance);
                                break;
                            case SixnetValidatorType.Email:
                            case SixnetValidatorType.CreditCard:
                            case SixnetValidatorType.Phone:
                            case SixnetValidatorType.Required:
                            case SixnetValidatorType.Url:
                            case SixnetValidatorType.Integer:
                            case SixnetValidatorType.PositiveInteger:
                            case SixnetValidatorType.PositiveIntegerOrZero:
                            case SixnetValidatorType.NegativeInteger:
                            case SixnetValidatorType.NegativeIntegerOrZero:
                            case SixnetValidatorType.Fraction:
                            case SixnetValidatorType.PositiveFraction:
                            case SixnetValidatorType.NegativeFraction:
                            case SixnetValidatorType.PositiveFractionOrZero:
                            case SixnetValidatorType.NegativeFractionOrZero:
                            case SixnetValidatorType.Number:
                            case SixnetValidatorType.Color:
                            case SixnetValidatorType.Chinese:
                            case SixnetValidatorType.PostCode:
                            case SixnetValidatorType.Mobile:
                            case SixnetValidatorType.IPV4:
                            case SixnetValidatorType.Date:
                            case SixnetValidatorType.DateTime:
                            case SixnetValidatorType.Letter:
                            case SixnetValidatorType.UpperLetter:
                            case SixnetValidatorType.LowerLetter:
                            case SixnetValidatorType.IdentityCard:
                                BuildValidatorValidation(rule.Type.ToString(), modelType, valueFieldArray);
                                break;
                        }
                    }
                }
            }
        }

        #endregion

        #region Enum

        /// <summary>
        /// Build enum validation
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <param name="enumType">Enum type</param>
        /// <param name="valueFieldArray">Value field type</param>
        void BuildEnumValidation(Type modelType, Type enumType, Array valueFieldArray)
        {
            MethodInfo enumMethod = _validationMethods.FirstOrDefault(c => c.Name == "EnumType");
            if (enumMethod == null || enumType == null)
            {
                return;
            }
            enumMethod.MakeGenericMethod(modelType).Invoke(null, new object[]
            {
                enumType,valueFieldArray
            });
        }

        #endregion

        #region MaxLength

        /// <summary>
        /// Build max length validation
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <param name="maxValue">Max value</param>
        /// <param name="valueFieldArray">Value field array type</param>
        void BuidMaxLengthValidation(Type modelType, int maxValue, Array valueFieldArray)
        {
            MethodInfo maxLengthMethod = _validationMethods.FirstOrDefault(c => c.Name == "MaxLength");
            if (maxLengthMethod == null)
            {
                return;
            }
            maxLengthMethod.MakeGenericMethod(modelType).Invoke(null, new object[]
            {
                maxValue,valueFieldArray
            });
        }

        #endregion

        #region MinLength

        /// <summary>
        /// Build max length validation
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <param name="minValue">Min value</param>
        /// <param name="valueFieldArray">Value field array type</param>
        void BuildMinLengthValidation(Type modelType, int minValue, Array valueFieldArray)
        {
            MethodInfo minLengthMethod = _validationMethods.FirstOrDefault(c => c.Name == "MinLength");
            if (minLengthMethod == null)
            {
                return;
            }
            minLengthMethod.MakeGenericMethod(modelType).Invoke(null, new object[]
            {
                minValue,valueFieldArray
            });
        }

        #endregion

        #region Range

        /// <summary>
        /// Build range validation
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <param name="minValue">Min value</param>
        /// <param name="maxValue">Max value</param>
        /// <param name="lowerBoundary">Lower boundary</param>
        /// <param name="upperBoundary">Upper boundary</param>
        /// <param name="valueFieldArray">Value field array</param>
        void BuildRangeValidation(Type modelType, dynamic minValue, dynamic maxValue, SixnetRangeBoundary lowerBoundary, SixnetRangeBoundary upperBoundary, Array valueFieldArray)
        {
            MethodInfo rangeMethod = _validationMethods.FirstOrDefault(c => c.Name == "Range");
            if (rangeMethod == null)
            {
                return;
            }
            rangeMethod.MakeGenericMethod(modelType).Invoke(null, new object[]
            {
                minValue,maxValue,lowerBoundary,upperBoundary,valueFieldArray
            });
        }

        #endregion

        #region RegularExpression

        /// <summary>
        /// Build regular expression validation
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <param name="value">Value</param>
        /// <param name="valueFieldArray">Value field array</param>
        void BuildRegularExpressionValidation(Type modelType, dynamic value, Array valueFieldArray)
        {
            MethodInfo regularExpressionMethod = _validationMethods.FirstOrDefault(c => c.Name == "RegularExpression");
            if (regularExpressionMethod == null)
            {
                return;
            }
            regularExpressionMethod.MakeGenericMethod(modelType).Invoke(null, new object[]
            {
                value,valueFieldArray
            });
        }

        #endregion

        #region StringLength

        /// <summary>
        /// Build string length validation
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <param name="minValue">Min value</param>
        /// <param name="maxValue">Max value</param>
        /// <param name="valueFieldArray">Value field array</param>
        void BuildStringLengthValidation(Type modelType, int minValue, int maxValue, Array valueFieldArray)
        {
            MethodInfo strLengthMethod = _validationMethods.FirstOrDefault(c => c.Name == "StringLength");
            if (strLengthMethod == null)
            {
                return;
            }
            strLengthMethod.MakeGenericMethod(modelType).Invoke(null, new object[]
            {
                maxValue,minValue,valueFieldArray
            });
        }

        #endregion

        #region Compare

        /// <summary>
        /// Build compare validation
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <param name="compareValue">Compare value</param>
        /// <param name="compareType">Compare type</param>
        /// <param name="parameterExpression">Parameter expression</param>
        /// <param name="funcType">Func type</param>
        /// <param name="parameterArray">Parameter array</param>
        /// <param name="compareOperator">Compare operator</param>
        /// <param name="fieldInstance">field instance</param>
        void BuildCompareValidation(Type modelType, object compareValue, CompareObject compareType, ParameterExpression parameterExpression, Type funcType, Array parameterArray, SixnetCompareOperator compareOperator, object fieldInstance)
        {
            MethodInfo compareMethod = _validationMethods.FirstOrDefault(c => c.Name == "SetCompareValidation");
            if (compareValue == null || compareMethod == null)
            {
                return;
            }
            switch (compareType)
            {
                case CompareObject.Field:
                    string[] comparePropertyNameArray = compareValue?.ToString().LSplit(".");
                    Expression comparePropertyExpress = null;
                    foreach (string pname in comparePropertyNameArray)
                    {
                        if (comparePropertyExpress == null)
                        {
                            comparePropertyExpress = Expression.PropertyOrField(parameterExpression, pname);
                        }
                        else
                        {
                            comparePropertyExpress = Expression.PropertyOrField(comparePropertyExpress, pname);
                        }
                    }
                    var compareLambdaExpression = _lambdaMethod.MakeGenericMethod(funcType).Invoke(null, new object[]
                    {
                        Expression.Convert(comparePropertyExpress,typeof(object)),parameterArray
                    });
                    if (compareLambdaExpression == null)
                    {
                        return;
                    }
                    compareValue = compareLambdaExpression;
                    break;
                default:
                    if (compareOperator == SixnetCompareOperator.In || compareOperator == SixnetCompareOperator.NotIn)
                    {
                        IEnumerable<dynamic> valueArray = compareValue.ToString().LSplit(",");
                        compareValue = valueArray;
                    }
                    break;
            }
            compareMethod.MakeGenericMethod(modelType).Invoke(null, new object[]
            {
                compareOperator,compareValue,fieldInstance
            });
        }

        #endregion

        #region Validator

        /// <summary>
        /// Build validator validation
        /// </summary>
        /// <param name="validatorName">Validator name</param>
        /// <param name="modelType">Model type</param>
        /// <param name="valueFieldArray">Value field array</param>
        void BuildValidatorValidation(string validatorName, Type modelType, Array valueFieldArray)
        {
            MethodInfo validatorMethod = _validationMethods.FirstOrDefault(c => c.Name == validatorName);
            if (validatorMethod == null)
            {
                return;
            }
            validatorMethod.MakeGenericMethod(modelType).Invoke(null, new object[]
            {
                valueFieldArray
            });
        }

        #endregion
    }
}
