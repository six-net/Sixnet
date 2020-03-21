using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.CQuery.Translator
{
    /// <summary>
    /// query translator interace
    /// </summary>
    public interface IQueryTranslator
    {
        #region Propertys

        /// <summary>
        /// query object pet name
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

        #region functions

        /// <summary>
        /// translate query
        /// </summary>
        /// <param name="query">query object</param>
        /// <returns>return translate result</returns>
        TranslateResult Translate(IQuery query);

        #endregion
    }
}
