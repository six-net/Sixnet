using EZNEW.Develop.CQuery;
using System;

namespace EZNEW.Develop.Command.Modify
{
    /// <summary>
    /// Calculate modify value
    /// </summary>
    [Serializable]
    public class CalculateModifyValue : IModifyValue
    {
        bool isCalculated = false;
        dynamic calculatedValue;

        #region Properties

        /// <summary>
        /// Gets the calculate operator
        /// </summary>
        public CalculateOperator Operator
        {
            get; private set;
        }

        /// <summary>
        /// Gets the calculate value
        /// </summary>
        public dynamic Value
        {
            get; private set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the EZNEW.Develop.Command.Modify.CalculateModifyValue
        /// </summary>
        /// <param name="calculateOperator">Calculate operator</param>
        /// <param name="value">Calculate value</param>
        public CalculateModifyValue(CalculateOperator calculateOperator, dynamic value)
        {
            Operator = calculateOperator;
            Value = value;
        }

        /// <summary>
        /// Calculate value
        /// </summary>
        /// <param name="originalValue">Original value</param>
        /// <returns>Calculated value</returns>
        public dynamic GetModifyValue(dynamic originalValue)
        {
            if (isCalculated)
            {
                return calculatedValue;
            }
            switch (Operator)
            {
                case CalculateOperator.Add:
                    calculatedValue = originalValue + Value;
                    break;
                case CalculateOperator.Subtract:
                    calculatedValue = originalValue - Value;
                    break;
                case CalculateOperator.Multiply:
                    calculatedValue = originalValue * Value;
                    break;
                case CalculateOperator.Divide:
                    calculatedValue = originalValue / Value;
                    break;
                default:
                    calculatedValue = originalValue;
                    break;
            }
            isCalculated = true;
            return calculatedValue;
        }

        #endregion
    }
}
