// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Field;

namespace Sixnet.Development.Queryable
{
    /// <summary>
    /// Defines criterion
    /// </summary>
    [Serializable]
    public class SixnetCriterion : ISixnetCondition
    {
        #region Constructor

        /// <summary>
        /// Initialize a criterion instance
        /// </summary>
        /// <param name="criterionOperator">Criterion operator</param>
        /// <param name="leftField">Left field</param>
        /// <param name="rightField">Right field</param>
        private SixnetCriterion(SixnetCriterionOperator criterionOperator, ISixnetField leftField, ISixnetField rightField)
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
        public ISixnetField Left { get; internal set; }

        /// <summary>
        /// Gets or sets the criterion operator
        /// </summary>
        public SixnetCriterionOperator Operator { get; internal set; } = SixnetCriterionOperator.None;

        /// <summary>
        /// Gets or sets the right field
        /// </summary>
        public ISixnetField Right { get; internal set; }

        /// <summary>
        /// Gets or sets the criterion options
        /// </summary>
        public SixnetCriterionOptions Options { get; set; }

        /// <summary>
        /// Gets or sets the connector
        /// </summary>
        public SixnetCriterionConnector Connector { get; set; } = SixnetCriterionConnector.And;

        /// <summary>
        /// Indicates whether is an equal criterion
        /// </summary>
        public bool IsEqual => Operator == SixnetCriterionOperator.Equal || Operator == SixnetCriterionOperator.In;

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
        public SixnetCriterion Clone()
        {
            return new SixnetCriterion(Operator, Left?.Clone(), Right?.Clone())
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
            return Operator == SixnetCriterionOperator.True || Operator == SixnetCriterionOperator.False;
        }

        /// <summary>
        /// Get boolean constant condition
        /// </summary>
        /// <returns></returns>
        public string GetBooleanConstantCondition()
        {
            return Operator == SixnetCriterionOperator.True ? "0 = 0" : "0 = 1";
        }

        #endregion

        #region Static method

        public static SixnetCriterion Create(SixnetCriterionOperator @operator, ISixnetField leftField, ISixnetField rightField
            , SixnetCriterionConnector connector = SixnetCriterionConnector.And, SixnetCriterionOptions criterionOptions = null, bool negation = false)
        {
            return new SixnetCriterion(@operator, leftField, rightField)
            {
                Connector = connector,
                Options = criterionOptions,
                Negation = negation
            };
        }

        #endregion
    }
}
