using System;
using System.Collections.Generic;
using System.Text;
using Sixnet.Development.Command;
using Sixnet.Exceptions;
using Sixnet.Model;

namespace Sixnet.Development.Data.Field
{
    /// <summary>
    /// Fields assignment
    /// </summary>
    public class FieldsAssignment : ISixnetCloneableModel<FieldsAssignment>
    {
        #region Fields

        Dictionary<string, dynamic> newValues = new();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the new entries
        /// </summary>
        public Dictionary<string, dynamic> NewValues => newValues;

        /// <summary>
        /// Gets or sets the old values
        /// </summary>
        public Dictionary<string, dynamic> OldValues { get; set; }

        #endregion

        #region Functions

        /// <summary>
        /// Set new value
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="newValue">New value</param>
        public void SetNewValue(string propertyName, dynamic newValue)
        {
            if (!string.IsNullOrWhiteSpace(propertyName))
            {
                newValues[propertyName] = newValue;
            }
        }

        /// <summary>
        /// Whether has new value
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <returns></returns>
        public bool HasNewValue(string propertyName)
        {
            if (!string.IsNullOrWhiteSpace(propertyName))
            {
                return newValues.ContainsKey(propertyName);
            }
            return false;
        }

        /// <summary>
        /// Whether has old value
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <returns></returns>
        public bool HasOldValue(string propertyName)
        {
            if (!string.IsNullOrWhiteSpace(propertyName))
            {
                return OldValues?.ContainsKey(propertyName) ?? false;
            }
            return false;
        }

        /// <summary>
        /// Get new value
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <returns></returns>
        public dynamic GetNewValue(string propertyName)
        {
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(propertyName), nameof(propertyName));
            newValues.TryGetValue(propertyName, out var newValue);
            return newValue;
        }

        /// <summary>
        /// Get old value
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <returns></returns>
        public dynamic GetOldValue(string propertyName)
        {
            SixnetDirectThrower.ThrowArgNullIf(string.IsNullOrWhiteSpace(propertyName), nameof(propertyName));
            dynamic oldValue = null;
            OldValues?.TryGetValue(propertyName, out oldValue);
            return oldValue;
        }

        public FieldsAssignment Clone()
        {
            return new FieldsAssignment()
            {
                newValues = new Dictionary<string, dynamic>(NewValues),
                OldValues = OldValues == null ? null : new Dictionary<string, dynamic>(OldValues)
            };
        }

        /// <summary>
        /// Create a fields assignment
        /// </summary>
        /// <returns></returns>
        public static FieldsAssignment Create()
        {
            return new FieldsAssignment();
        }

        #endregion
    }
}
