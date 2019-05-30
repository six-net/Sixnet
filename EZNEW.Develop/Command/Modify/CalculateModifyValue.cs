using EZNEW.Develop.CQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Command.Modify
{
    /// <summary>
    /// CalculateModify
    /// </summary>
    public class CalculateModifyValue : IModifyValue
    {
        bool isCalculated = false;
        dynamic calculatedValue;

        #region Propertys

        /// <summary>
        /// Calculate Operator
        /// </summary>
        public CalculateOperator Operator
        {
            get; private set;
        }

        /// <summary>
        /// Calculate Value
        /// </summary>
        public dynamic Value
        {
            get; private set;
        }

        #endregion

        public CalculateModifyValue(CalculateOperator calculateOperator, dynamic value)
        {
            Operator = calculateOperator;
            Value = value;
        }

        /// <summary>
        /// calculate value
        /// </summary>
        /// <param name="originalValue"></param>
        /// <returns></returns>
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
                case CalculateOperator.subtract:
                    calculatedValue = originalValue - Value;
                    break;
                case CalculateOperator.multiply:
                    calculatedValue = originalValue * Value;
                    break;
                case CalculateOperator.divide:
                    calculatedValue = originalValue / Value;
                    break;
                default:
                    calculatedValue = originalValue;
                    break;
            }
            isCalculated = true;
            return calculatedValue;
        }
    }
}
