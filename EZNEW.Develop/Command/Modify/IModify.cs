using EZNEW.Develop.CQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Command.Modify
{
    /// <summary>
    /// data modify
    /// </summary>
    public interface IModify
    {
        #region methods

        /// <summary>
        /// set modify expression
        /// </summary>
        /// <typeparam name="T">Model Type</typeparam>
        /// <param name="fields">modify fields</param>
        /// <returns>IModify object</returns>
        IModify Set<T>(params Expression<Func<T, dynamic>>[] fields);

        /// <summary>
        /// set modify expression
        /// </summary>
        /// <typeparam name="T">Model Type</typeparam>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <returns>IModify object</returns>
        IModify Set<T>(Expression<Func<T, dynamic>> field, dynamic value);

        /// <summary>
        /// set modify value
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="value">value</param>
        /// <returns>IModify object</returns>
        IModify Set(string name, dynamic value);

        /// <summary>
        /// calculate with current value then modify
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="calculateOperator">calculate operator</param>
        /// <param name="value">value</param>
        /// <returns>IModify object</returns>
        IModify Calculate(string name, CalculateOperator calculateOperator, dynamic value);

        /// <summary>
        /// calculate with current value then modify
        /// </summary>
        /// <typeparam name="T">Model Type</typeparam>
        /// <param name="field">field</param>
        /// <param name="calculateOperator">calculate operator</param>
        /// <param name="value">value</param>
        /// <returns>IModify object</returns>
        IModify Calculate<T>(Expression<Func<T, dynamic>> field, CalculateOperator calculateOperator, dynamic value);

        /// <summary>
        /// add value
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="value">value</param>
        /// <returns>IModify object</returns>
        IModify Add(string name, dynamic value);

        /// <summary>
        /// add value
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <returns>IModify object</returns>
        IModify Add<T>(Expression<Func<T, dynamic>> field, dynamic value);

        /// <summary>
        /// subtract value
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="value">value</param>
        /// <returns>IModify object</returns>
        IModify Subtract(string name, dynamic value);

        /// <summary>
        /// subtract value
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <returns>IModify object</returns>
        IModify Subtract<T>(Expression<Func<T, dynamic>> field, dynamic value);

        /// <summary>
        /// multiply value
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="value">value</param>
        /// <returns>IModify object</returns>
        IModify Multiply(string name, dynamic value);

        /// <summary>
        /// multiply value
        /// </summary>
        /// <param name="field">name</param>
        /// <param name="value">value</param>
        /// <returns>IModify object</returns>
        IModify Multiply<T>(Expression<Func<T, dynamic>> field, dynamic value);

        /// <summary>
        /// divide value
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="value">value</param>
        /// <returns>IModify object</returns>
        IModify Divide(string name, dynamic value);

        /// <summary>
        /// divide value
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <returns>IModify object</returns>
        IModify Divide<T>(Expression<Func<T, dynamic>> field, dynamic value);

        /// <summary>
        /// get modify values
        /// </summary>
        /// <returns>modify values</returns>
        Dictionary<string, IModifyValue> GetModifyValues();

        #endregion
    }
}
