using System;
using Sixnet.Development.Data.Field;

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Defines criterion
    /// </summary>
    [Serializable]
    public class Criterion : ISixnetCondition
    {
        #region Constructor

        /// <summary>
        /// Initialize a criterion instance
        /// </summary>
        /// <param name="criterionOperator">Criterion operator</param>
        /// <param name="leftField">Left field</param>
        /// <param name="rightField">Right field</param>
        private Criterion(CriterionOperator criterionOperator, ISixnetDataField leftField, ISixnetDataField rightField)
        {
            Left = leftField;
            Operator = criterionOperator;
            Right = rightField;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the left field
        /// </summary>
        public ISixnetDataField Left { get; internal set; }

        /// <summary>
        /// Gets or sets the criterion operator
        /// </summary>
        public CriterionOperator Operator { get; internal set; }

        /// <summary>
        /// Gets or sets the right field
        /// </summary>
        public ISixnetDataField Right { get; internal set; }

        /// <summary>
        /// Gets or sets the criterion options
        /// </summary>
        public CriterionOptions Options { get; set; }

        /// <summary>
        /// Gets or sets the connector
        /// </summary>
        public CriterionConnector Connector { get; set; } = CriterionConnector.And;

        /// <summary>
        /// Indicates whether is an equal criterion
        /// </summary>
        public bool IsEqual => Operator == CriterionOperator.Equal || Operator == CriterionOperator.In;

        public bool None => false;

        /// <summary>
        /// Whether negation
        /// </summary>
        public bool Negation { get; protected set; }

        #endregion

        #region Functions

        /// <summary>
        /// Clone a new criterion
        /// </summary>
        /// <returns></returns>
        public Criterion Clone()
        {
            return new Criterion(Operator, Left?.Clone(), Right?.Clone())
            {
                Options = Options?.Clone(),
                Connector = Connector,
                Negation = Negation
            };
        }

        /// <summary>
        /// Gets whether has field formatter
        /// </summary>
        /// <returns></returns>
        public bool HasFieldFormatter()
        {
            return (Left?.HasFormatter ?? false) || (Right?.HasFormatter ?? false);
        }

        /// <summary>
        /// Indicates whether is a boolean constant
        /// </summary>
        /// <returns>Return is a boolean constant</returns>
        public bool IsBooleanConstant()
        {
            return Operator == CriterionOperator.True || Operator == CriterionOperator.False;
        }

        /// <summary>
        /// Get boolean constant condition
        /// </summary>
        /// <returns></returns>
        public string GetBooleanConstantCondition()
        {
            return Operator == CriterionOperator.True ? "0 = 0" : "0 = 1";
        }

        #endregion

        #region Static method

        public static Criterion Create(CriterionOperator @operator, ISixnetDataField leftField, ISixnetDataField rightField
            , CriterionConnector connector = CriterionConnector.And, CriterionOptions criterionOptions = null, bool negation = false)
        {
            return new Criterion(@operator, leftField, rightField)
            {
                Connector = connector,
                Options = criterionOptions,
                Negation = negation
            };
        }

        #endregion
    }
}
