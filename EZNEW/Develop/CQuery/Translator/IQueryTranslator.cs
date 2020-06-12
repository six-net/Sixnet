namespace EZNEW.Develop.CQuery.Translator
{
    /// <summary>
    /// query translator interace
    /// </summary>
    public interface IQueryTranslator
    {
        #region Properties

        /// <summary>
        /// Gets the object pet name
        /// </summary>
        string ObjectPetName
        {
            get;
        }

        /// <summary>
        /// Gets or sets the parameter sequence
        /// </summary>
        int ParameterSequence
        {
            get; set;
        }

        #endregion

        #region Functions

        /// <summary>
        /// Translate query object
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return translate result</returns>
        TranslateResult Translate(IQuery query);

        #endregion
    }
}
