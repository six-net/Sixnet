using EZNEW.Develop.CQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Command.Modify
{
    /// <summary>
    /// calculate modify value
    /// </summary>
    public class CalculateModifyValue : IModifyValue
    {
        bool isCalculated = false;
        dynamic calculatedValue;

        #region propertys

        /// <summary>
        /// calculate operator
        /// </summary>
        public CalculateOperator Operator
        {
            get; private set;
        }

        /// <summary>
        /// calculate value
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
    }
}
