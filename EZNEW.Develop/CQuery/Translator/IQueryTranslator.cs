using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.CQuery.Translator
{
    /// <summary>
    /// Query Translator Interace
    /// </summary>
    public interface IQueryTranslator
    {
        #region Propertys

        /// <summary>
        /// Query Object Pet Name
        /// </summary>
        string ObjectPetName
        {
            get;
        }

        /// <summary>
        /// parameter sequence
        /// </summary>
        int ParameterSequence
        {
            get;set;
        }

        #endregion

        #region Functions

        /// <summary>
        /// Translate Query
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns>return translate result</returns>
        TranslateResult Translate(IQuery query);

        #endregion
    }
}
