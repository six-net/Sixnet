using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Development.Entity
{
    /// <summary>
    /// Defines entity contract
    /// </summary>
    public interface IEntity
    {

        /// <summary>
        /// Gets the property or field name
        /// </summary>
        /// <param name="name">Property or field name</param>
        /// <returns>Return the value</returns>
        dynamic GetValue(string name);

        /// <summary>
        /// Gets the property or field name
        /// </summary>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="name">Property or field name</param>
        /// <returns>Return the value</returns>
        TValue GetValue<TValue>(string name);

        /// <summary>
        /// Sets the property or field value
        /// </summary>
        /// <param name="name">Property or field name</param>
        /// <param name="value">Value</param>
        void SetValue(string name, dynamic value);

        /// <summary>
        /// Gets all property or field values
        /// </summary>
        /// <returns>Return all property values</returns>
        Dictionary<string, dynamic> GetAllValues();
    }
}
