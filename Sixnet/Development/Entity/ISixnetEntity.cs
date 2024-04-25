using System;
using System.Collections.Generic;
using Sixnet.Development.Data.Field;
using Sixnet.Model;
using Sixnet.Serialization;

namespace Sixnet.Development.Entity
{
    /// <summary>
    /// Defines entity contract
    /// </summary>
    public interface ISixnetEntity : ISixnetMappable
    {
        /// <summary>
        /// Whether allow to save
        /// </summary>
        /// <returns></returns>
        bool AllowToSave();

        /// <summary>
        /// Whether allow to delete
        /// </summary>
        /// <returns></returns>
        bool AllowToDelete();

        /// <summary>
        /// Check identity value is null
        /// </summary>
        /// <returns>Return identity value whether is null</returns>
        bool IdentityValueIsNull();

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

        /// <summary>
        /// Gets the identity value
        /// </summary>
        /// <returns>Return identity value</returns>
        string GetIdentityValue();
    }

    /// <summary>
    /// Defines entity contract
    /// </summary>
    public interface ISixnetEntity<T> : ISixnetEntity where T : class, ISixnetEntity<T>
    {
        /// <summary>
        /// Compare two objects
        /// </summary>
        /// <param name="targetEntity">Target object</param>
        /// <returns></returns>
        bool Equals(T targetEntity);

        /// <summary>
        /// Update data
        /// </summary>
        /// <returns></returns>
        void OnDataUpdating();

        /// <summary>
        /// Add data
        /// </summary>
        /// <returns></returns>
        void OnDataAdding();

        /// <summary>
        /// Modify from new data
        /// </summary>
        /// <param name="newData">New data</param>
        /// <param name="configure">Configure</param>
        void ModifyFrom(T newData, Action<ModifyEntityOptions> configure = null);

        /// <summary>
        /// Get modification assignment
        /// </summary>
        /// <param name="newData">New data</param>
        /// <param name="configure">Configure</param>
        FieldsAssignment GetModificationAssignment(T newData, Action<ModifyEntityOptions> configure = null);
    }
}
