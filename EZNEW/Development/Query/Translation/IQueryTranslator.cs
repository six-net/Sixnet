using EZNEW.Data;

namespace EZNEW.Development.Query.Translation
{
    /// <summary>
    /// Query translator contract
    /// </summary>
    public interface IQueryTranslator
    {
        #region Properties

        /// <summary>
        /// Gets the object pet name
        /// </summary>
        string ObjectPetName { get; }

        /// <summary>
        /// Gets or sets the parameter sequence
        /// </summary>
        int ParameterSequence { get; set; }

        /// <summary>
        /// Gets or sets the data access context
        /// </summary>
        DataAccessContext DataAccessContext { get; set; }

        #endregion

        #region Functions

        /// <summary>
        /// Translate query object
        /// </summary>
        /// <param name="query">Query object</param>
        /// <returns>Return translate result</returns>
        QueryTranslationResult Translate(IQuery query);

        #endregion
    }
}
