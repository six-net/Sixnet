using EZNEW.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EZNEW.Develop.Domain.Aggregation
{
    /// <summary>
    /// Aggregation root contract
    /// </summary>
    public interface IAggregationRoot
    {
        #region Properties

        /// <summary>
        /// Gets whether allow to save
        /// </summary>
        bool CanBeSave { get; }

        /// <summary>
        /// Gets whether allow to remove
        /// </summary>
        bool CanBeRemove { get; }

        /// <summary>
        /// Gets the identity value
        /// </summary>
        string IdentityValue { get; }

        /// <summary>
        /// Gets whether the object is new
        /// </summary>
        bool IsNew { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Mark data is new
        /// </summary>
        bool MarkNew();

        /// <summary>
        /// Mark data is stored
        /// </summary>
        /// <returns></returns>
        bool MarkStored();

        #endregion
    }

    /// <summary>
    /// Aggregation root contract
    /// </summary>
    public interface IAggregationRoot<in T> : IAggregationRoot where T : IAggregationRoot<T>
    {
        /// <summary>
        /// Compare two objects
        /// </summary>
        /// <param name="targetObj"></param>
        /// <returns></returns>
        bool Equals(T targetObj);

        /// <summary>
        /// Close lazy data load
        /// </summary>
        void CloseLazyMemberLoad();

        /// <summary>
        /// Open lazy data load
        /// </summary>
        void OpenLazyMemberLoad();

        /// <summary>
        /// Set load properties
        /// </summary>
        /// <param name="loadProperties">Properties</param>
        /// <returns></returns>
        void SetLoadProperties(IEnumerable<KeyValuePair<string, bool>> loadProperties);

        /// <summary>
        /// Check identity value is none
        /// </summary>
        /// <returns>Return identity value whether is none</returns>
        bool IdentityValueIsNone();
    }
}
