using System;
using EZNEW.Development.Query;

namespace EZNEW.Development.Command.Modification
{
    /// <summary>
    /// Defines calculation modification value
    /// </summary>
    [Serializable]
    public class CalculationModificationValue : IModificationValue
    {
        bool isCalculated = false;
        dynamic calculatedValue;

        #region Properties

        /// <summary>
        /// Gets the calculate operator
        /// </summary>
        public CalculationOperator Operator { get; private set; }

        /// <summary>
        /// Gets the calculate value
        /// </summary>
        public dynamic Value { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the EZNEW.Develop.Command.Modification.CalculationModificationValue
        /// </summary>
        /// <param name="calculationOperator">Calculation operator</param>
        /// <param name="value">Calculation value</param>
        public CalculationModificationValue(CalculationOperator calculationOperator, dynamic value)
        {
            Operator = calculationOperator;
            Value = value;
        }

        /// <summary>
        /// Get the modified value
        /// </summary>
        /// <param name="originalValue">Original value</param>
        /// <returns>Return the modified value</returns>
        public dynamic GetModifiedValue(dynamic originalValue)
        {
            if (isCalculated)
            {
                return calculatedValue;
            }
            calculatedValue = Operator switch
            {
                CalculationOperator.Add => originalValue + Value,
                CalculationOperator.Subtract => originalValue - Value,
                CalculationOperator.Multiply => originalValue * Value,
                CalculationOperator.Divide => originalValue / Value,
                _ => originalValue,
            };
            isCalculated = true;
            return calculatedValue;
        }

        #endregion
    }
}
