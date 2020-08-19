using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;
using EZNEW.Develop.CQuery;
using EZNEW.Reflection;

namespace EZNEW.DataValidation.Configuration
{
    /// <summary>
    /// Type validation rule collection
    /// </summary>
    public class ValidationRuleCollection
    {
        /// <summary>
        /// Expression type
        /// </summary>
        static readonly Type ExpressionType = typeof(Expression);

        /// <summary>
        /// Lambda method
        /// </summary>
        static readonly MethodInfo LambdaMethod = null;

        /// <summary>
        /// Validation methods
        /// </summary>
        static List<MethodInfo> ValidationMethods = new List<MethodInfo>();

        /// <summary>
        /// Gets or sets the type validation rules
        /// </summary>
        [JsonProperty(PropertyName = "types")]
        public List<TypeValidationRule> Types
        {
            get; set;
        }

        static ValidationRuleCollection()
        {
            var baseExpressMethods = ExpressionType.GetMethods(BindingFlags.Public | BindingFlags.Static);
            LambdaMethod = baseExpressMethods.FirstOrDefault(c => c.Name == "Lambda" && c.IsGenericMethod && c.GetParameters()[1].ParameterType.FullName == typeof(ParameterExpression[]).FullName);
            ValidationMethods = typeof(ValidationManager).GetMethods().ToList();
        }

        internal void BuildValidation()
        {
            if (Types.IsNullOrEmpty())
            {
                return;
            }
            foreach (var typeRule in Types)
            {
                if (typeRule == null || string.IsNullOrWhiteSpace(typeRule.TypeFullName) || typeRule.Properties == null)
                {
                    continue;
                }
                Type modelType = Type.GetType(typeRule.TypeFullName);
                if (modelType == null)
                {
                    continue;
                }
                //load propertys and fields
                List<MemberInfo> memberInfoList = new List<MemberInfo>();
                memberInfoList.AddRange(modelType.GetFields(BindingFlags.Public | BindingFlags.Instance));
                memberInfoList.AddRange(modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance));
                //parameter expression
                ParameterExpression parameterExpression = Expression.Parameter(modelType);
                Array parameterArray = Array.CreateInstance(typeof(ParameterExpression), 1);
                parameterArray.SetValue(parameterExpression, 0);
                Type valFieldType = typeof(ValidationField<>).MakeGenericType(modelType);
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
                    var genericLambdaMethod = ReflectionManager.Expression.LambdaMethod.MakeGenericMethod(funcType);
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
                        Array valueFieldArray = Array.CreateInstance(valFieldType, 1);
                        valueFieldArray.SetValue(fieldInstance, 0);
                        switch (rule.ValidatorType)
                        {
                            case ValidatorType.EnumType:
                                Type enumType = Type.GetType(rule.EnumType);
                                BuildEnumValidation(modelType, enumType, valueFieldArray);
                                break;
                            case ValidatorType.MaxLength:
                                BuidMaxLengthValidation(modelType, Convert.ToInt32(rule.MaxValue), valueFieldArray);
                                break;
                            case ValidatorType.MinLength:
                                BuildMinLengthValidation(modelType, Convert.ToInt32(rule.MinValue), valueFieldArray);
                                break;
                            case ValidatorType.Range:
                                BuildRangeValidation(modelType, rule.MinValue, rule.MaxValue, rule.LowerBoundary, rule.UpperBoundary, valueFieldArray);
                                break;
                            case ValidatorType.RegularExpression:
                                BuildRegularExpressionValidation(modelType, rule.Value, valueFieldArray);
                                break;
                            case ValidatorType.StringLength:
                                BuildStringLengthValidation(modelType, Convert.ToInt32(rule.MinValue), Convert.ToInt32(rule.MaxValue), valueFieldArray);
                                break;
                            case ValidatorType.Compare:
                                BuildCompareValidation(modelType, rule.Value, rule.CompareType, parameterExpression, funcType, parameterArray, rule.Operator, fieldInstance);
                                break;
                            case ValidatorType.Email:
                            case ValidatorType.CreditCard:
                            case ValidatorType.Phone:
                            case ValidatorType.Required:
                            case ValidatorType.Url:
                            case ValidatorType.Integer:
                            case ValidatorType.PositiveInteger:
                            case ValidatorType.PositiveIntegerOrZero:
                            case ValidatorType.NegativeInteger:
                            case ValidatorType.NegativeIntegerOrZero:
                            case ValidatorType.Fraction:
                            case ValidatorType.PositiveFraction:
                            case ValidatorType.NegativeFraction:
                            case ValidatorType.PositiveFractionOrZero:
                            case ValidatorType.NegativeFractionOrZero:
                            case ValidatorType.Number:
                            case ValidatorType.Color:
                            case ValidatorType.Chinese:
                            case ValidatorType.PostCode:
                            case ValidatorType.Mobile:
                            case ValidatorType.IPV4:
                            case ValidatorType.Date:
                            case ValidatorType.DateTime:
                            case ValidatorType.Letter:
                            case ValidatorType.UpperLetter:
                            case ValidatorType.LowerLetter:
                            case ValidatorType.IdentityCard:
                                BuildValidatorValidation(rule.ValidatorType.ToString(), modelType, valueFieldArray);
                                break;
                        }
                    }
                }
            }
        }

        #region Enum

        /// <summary>
        /// Build enum validation
        /// </summary>
        /// <param name="modelType">Model type</param>
        /// <param name="enumType">Enum type</param>
        /// <param name="valueFieldArray">Value field type</param>
        void BuildEnumValidation(Type modelType, Type enumType, Array valueFieldArray)
        {
            MethodInfo enumMethod = ValidationMethods.FirstOrDefault(c => c.Name == "EnumType");
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
            MethodInfo maxLengthMethod = ValidationMethods.FirstOrDefault(c => c.Name == "MaxLength");
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
            MethodInfo minLengthMethod = ValidationMethods.FirstOrDefault(c => c.Name == "MinLength");
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
        void BuildRangeValidation(Type modelType, dynamic minValue, dynamic maxValue, RangeBoundary lowerBoundary, RangeBoundary upperBoundary, Array valueFieldArray)
        {
            MethodInfo rangeMethod = ValidationMethods.FirstOrDefault(c => c.Name == "Range");
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
            MethodInfo regularExpressionMethod = ValidationMethods.FirstOrDefault(c => c.Name == "RegularExpression");
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
            MethodInfo strLengthMethod = ValidationMethods.FirstOrDefault(c => c.Name == "StringLength");
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
        void BuildCompareValidation(Type modelType, object compareValue, CompareObject compareType, ParameterExpression parameterExpression, Type funcType, Array parameterArray, CompareOperator compareOperator, object fieldInstance)
        {
            MethodInfo compareMethod = ValidationMethods.FirstOrDefault(c => c.Name == "SetCompareValidation");
            if (compareMethod == null)
            {
                return;
            }
            //object compareValue = rule.Value;
            switch (compareType)
            {
                case CompareObject.Field:
                    string[] comparePropertyNameArray = compareValue.ToString().LSplit(".");
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
                    var compareLambdaExpression = LambdaMethod.MakeGenericMethod(funcType).Invoke(null, new object[]
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
                    if (compareOperator == CompareOperator.In || compareOperator == CompareOperator.NotIn)
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
            MethodInfo validatorMethod = ValidationMethods.FirstOrDefault(c => c.Name == validatorName);
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
