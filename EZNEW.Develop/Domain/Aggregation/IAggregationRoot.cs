using EZNEW.Develop.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Domain.Aggregation
{

    public interface IAggregationRoot
    {
        #region Propertys

        /// <summary>
        /// allow to save
        /// </summary>
        bool CanBeSave { get; }

        /// <summary>
        /// allow to remove
        /// </summary>
        bool CanBeRemove { get; }

        /// <summary>
        /// identity value
        /// </summary>
        string IdentityValue { get; }

        /// <summary>
        /// is new object
        /// </summary>
        bool IsNew { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Mark Object Is New
        /// </summary>
        bool MarkNew();

        /// <summary>
        /// Mark Object Is Stored
        /// </summary>
        /// <returns></returns>
        bool MarkStored();

        /// <summary>
        /// Save Object
        /// </summary>
        void Save();

        /// <summary>
        /// Save Object
        /// </summary>
        /// <returns></returns>
        Task SaveAsync();

        /// <summary>
        /// Remove Object
        /// </summary>
        void Remove();

        /// <summary>
        /// Remove Object
        /// </summary>
        Task RemoveAsync();

        #endregion
    }

    /// <summary>
    /// AggregationRoot Interface
    /// </summary>
    public interface IAggregationRoot<in T> : IAggregationRoot where T : IAggregationRoot<T>
    {
        /// <summary>
        /// compare two objects
        /// </summary>
        /// <param name="targetObj"></param>
        /// <returns></returns>
        bool Equals(T targetObj);

        /// <summary>
        /// Close Lazy Data Load
        /// </summary>
        void CloseLazyMemberLoad();

        /// <summary>
        /// Open Lazy Data Load
        /// </summary>
        void OpenLazyMemberLoad();

        /// <summary>
        /// Set Load Propertys
        /// </summary>
        /// <param name="loadPropertys">propertys</param>
        /// <returns></returns>
        void SetLoadPropertys(Dictionary<string, bool> loadPropertys);
    }
}
