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
        #region propertys

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
        /// mark object is new
        /// </summary>
        bool MarkNew();

        /// <summary>
        /// mark object is stored
        /// </summary>
        /// <returns></returns>
        bool MarkStored();

        /// <summary>
        /// save object
        /// </summary>
        void Save();

        /// <summary>
        /// save object
        /// </summary>
        /// <returns></returns>
        Task SaveAsync();

        /// <summary>
        /// remove object
        /// </summary>
        void Remove();

        /// <summary>
        /// remove object
        /// </summary>
        Task RemoveAsync();

        #endregion
    }

    /// <summary>
    /// aggregationRoot interface
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
        /// close lazy data load
        /// </summary>
        void CloseLazyMemberLoad();

        /// <summary>
        /// open lazy data load
        /// </summary>
        void OpenLazyMemberLoad();

        /// <summary>
        /// set load propertys
        /// </summary>
        /// <param name="loadPropertys">propertys</param>
        /// <returns></returns>
        void SetLoadPropertys(Dictionary<string, bool> loadPropertys);
    }
}
