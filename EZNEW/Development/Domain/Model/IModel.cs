using System.Collections.Generic;
using System.Threading.Tasks;
using EZNEW.Development.Query;

namespace EZNEW.Development.Domain.Model
{
    /// <summary>
    /// Defines model contract
    /// </summary>
    public interface IModel
    {
        /// <summary>
        /// Indicates whether allow to save
        /// </summary>
        /// <returns></returns>
        bool AllowToSave();

        /// <summary>
        /// Indicates whether allow to remove
        /// </summary>
        /// <returns></returns>
        bool AllowToRemove();

        /// <summary>
        /// Gets the identity value
        /// </summary>
        /// <returns></returns>
        string GetIdentityValue();

        /// <summary>
        /// Indicates whether is a new object
        /// </summary>
        /// <returns></returns>
        bool IsNew();

        /// <summary>
        /// Mark data is new
        /// </summary>
        bool MarkNew();

        /// <summary>
        /// Mark data is stored
        /// </summary>
        /// <returns></returns>
        bool MarkStored();

        /// <summary>
        /// Check identity value is null
        /// </summary>
        /// <returns>Return identity value whether is null</returns>
        bool IdentityValueIsNull();
    }

    /// <summary>
    /// Defines model generic contract
    /// </summary>
    public interface IModel<in T> : IModel, IQueryModel<T> where T : IModel<T>
    {
        /// <summary>
        /// Compare two objects
        /// </summary>
        /// <param name="targetObj"></param>
        /// <returns></returns>
        bool Equals(T targetObj);

        /// <summary>
        /// Close load lazy member
        /// </summary>
        void CloseLazyMember();

        /// <summary>
        /// Open load lazy member
        /// </summary>
        void OpenLazyMember();

        /// <summary>
        /// Set load properties
        /// Key=> Property name
        /// Value=> Whether allow to load
        /// </summary>
        /// <param name="loadProperties">Properties</param>
        /// <returns></returns>
        void SetLoadProperties(IEnumerable<KeyValuePair<string, bool>> loadProperties);

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="newData">New data</param>
        /// <returns></returns>
        IModel OnDataUpdating(T newData);

        /// <summary>
        /// Add data
        /// </summary>
        /// <returns>Return data</returns>
        IModel OnDataAdding();
    }
}
