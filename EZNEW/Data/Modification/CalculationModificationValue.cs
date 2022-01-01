using System;

namespace EZNEW.Data.Modification
{
    /// <summary>
    /// Defines calculation modification value
    /// </summary>
    [Serializable]
    public class CalculationModificationValue : IModificationValue
    {
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
        /// Initializes a new instance of the EZNEW.Data.Modification.CalculationModificationValue
        /// </summary>
        /// <param name="calculationOperator">Calculation operator</param>
        /// <param name="calculationValue">Calculation value</param>
        public CalculationModificationValue(CalculationOperator calculationOperator, dynamic calculationValue)
        {
            Operator = calculationOperator;
            Value = calculationValue;
        }

        /// <summary>
        /// Get the modified value
        /// </summary>
        /// <param name="originalValue">Original value</param>
        /// <returns>Return the modified value</returns>
        public dynamic GetModifiedValue(dynamic originalValue)
        {
            return Operator switch
            {
                CalculationOperator.Add => originalValue + Value,
                CalculationOperator.Subtract => originalValue - Value,
                CalculationOperator.Multiply => originalValue * Value,
                CalculationOperator.Divide => originalValue / Value,
                _ => originalValue,
            };
        }

        #endregion
    }
}
