using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using EZNEW.Development.Domain.Model;
using EZNEW.Development.Domain.Repository;
using EZNEW.Expressions;
using EZNEW.Model;

namespace EZNEW.Development.Entity
{
    /// <summary>
    /// Defines model entity
    /// </summary>
    public class ModelEntity<T> : BaseEntity<T>, IModel<T> where T : BaseEntity<T>, IModel<T>, new()
    {
        static ModelEntity()
        {
            ModelManager.ConfigureModel<T>();
        }

        #region Fields

        /// <summary>
        /// The repository object
        /// </summary>
        [NonData]
        protected IRepository<T> repository = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether allow to save
        /// </summary>
        [NonData]
        public bool CanBeSave => SaveValidation();

        /// <summary>
        /// Gets whether allow to remove
        /// </summary>
        [NonData]
        public bool CanBeRemove => RemoveValidation();

        /// <summary>
        /// Gets whether allow lazy data load
        /// </summary>
        [NonData]
        protected bool LoadLazyMember { get; set; } = true;

        /// <summary>
        /// Gets all of the properties allow to load data
        /// </summary>
        [NonData]
        protected Dictionary<string, bool> LoadProperties = new Dictionary<string, bool>();

        /// <summary>
        /// Gets the identity value
        /// </summary>
        [NonData]
        public string IdentityValue => GetIdentityValue();

        /// <summary>
        /// Gets whether the object is new
        /// </summary>
        [NonData]
        public bool IsNew => ModelDataManager<T>.IsNew(repository, GetType(), this);

        #endregion

        #region Methods

        /// <summary>
        /// Save validation
        /// </summary>
        /// <returns>Return whether allow to save</returns>
        protected virtual bool SaveValidation()
        {
            return ModelDataManager<T>.SaveValidation(this as T);
        }

        /// <summary>
        /// Remove validation
        /// </summary>
        /// <returns>Return whether allow to remove</returns>
        protected virtual bool RemoveValidation()
        {
            return !IdentityValueIsNone();
        }

        /// <summary>
        /// Mark object is new status
        /// </summary>
        /// <returns>Return whether is successful</returns>
        public virtual bool MarkNew()
        {
            return ModelDataManager<T>.MarkNew(repository, this);
        }

        /// <summary>
        /// Mark object is stored status
        /// </summary>
        /// <returns>Return whether is successful</returns>
        public virtual bool MarkStored()
        {
            return ModelDataManager<T>.MarkStored(repository, this);
        }

        /// <summary>
        /// Set load properties
        /// </summary>
        /// <param name="loadProperties">Properties</param>
        /// <returns></returns>
        public virtual void SetLoadProperties(IEnumerable<KeyValuePair<string, bool>> loadProperties)
        {
            ModelDataManager<T>.SetLoadProperties(loadProperties, ref LoadProperties);
        }

        /// <summary>
        /// Set load property
        /// </summary>
        /// <param name="property">Property</param>
        /// <param name="allowLoad">Whether allow load</param>
        public virtual void SetLoadProperty(Expression<Func<T, dynamic>> property, bool allowLoad = true)
        {
            ModelDataManager<T>.SetLoadProperty(property, allowLoad, ref LoadProperties);
        }

        /// <summary>
        /// Close lazy data load
        /// </summary>
        public virtual void CloseLazyMemberLoad()
        {
            LoadLazyMember = false;
        }

        /// <summary>
        /// Open lazy data load
        /// </summary>
        public virtual void OpenLazyMemberLoad()
        {
            LoadLazyMember = true;
        }

        /// <summary>
        /// Check property whether allow to lazy load
        /// </summary>
        /// <param name="property">Property</param>
        /// <returns>Return wheather property allow load data</returns>
        protected virtual bool AllowLazyLoad(string property)
        {
            if (!LoadLazyMember || LoadProperties == null || !LoadProperties.ContainsKey(property))
            {
                return false;
            }
            return LoadProperties[property];
        }

        /// <summary>
        /// Check property whether allow to lazy load
        /// </summary>
        /// <param name="property">Property</param>
        /// <returns>Return wheather property allow load data</returns>
        protected virtual bool AllowLazyLoad(Expression<Func<T, dynamic>> property)
        {
            if (property == null)
            {
                return false;
            }
            return AllowLazyLoad(ExpressionHelper.GetExpressionPropertyName(property.Body));
        }

        /// <summary>
        /// Check whether allow load data
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="property">Property</param>
        /// <param name="lazyMember">Lazy member</param>
        /// <returns>Return whether allow load data</returns>
        protected virtual bool AllowLoad<TModel>(Expression<Func<T, dynamic>> property, LazyMember<TModel> lazyMember) where TModel : IModel<TModel>
        {
            return AllowLazyLoad(property) && !(lazyMember.CurrentValue?.IdentityValueIsNone() ?? true);
        }

        /// <summary>
        /// Save
        /// </summary>
        public virtual Result<T> Save()
        {
            return ModelDataManager<T>.Save(repository, this as T);
        }

        /// <summary>
        /// Remove
        /// </summary>
        public virtual Result Remove()
        {
            return ModelDataManager<T>.Remove(repository, this as T);
        }

        /// <summary>
        /// Init primary value
        /// </summary>
        public virtual void InitIdentityValue() { }

        /// <summary>
        /// Check identity value is none
        /// </summary>
        /// <returns>Return identity value whether has value</returns>
        public virtual bool IdentityValueIsNone()
        {
            return string.IsNullOrWhiteSpace(GetIdentityValue());
        }

        /// <summary>
        /// Compare two objects
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns></returns>
        public virtual bool Equals(T data)
        {
            return data?.IdentityValue == IdentityValue;
        }

        /// <summary>
        /// Compare two objects
        /// </summary>
        /// <param name="data">Target data</param>
        /// <returns></returns>
        public override bool Equals(object data)
        {
            return Equals(data as T);
        }

        /// <summary>
        /// Get hash code
        /// </summary>
        /// <returns>Return model hash code</returns>
        public override int GetHashCode()
        {
            return IdentityValue.GetHashCode();
        }

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="newData">New data</param>
        /// <returns></returns>
        internal protected virtual T OnUpdating(T newData)
        {
            return newData;
        }

        /// <summary>
        /// Add data
        /// </summary>
        /// <returns>Return data</returns>
        internal protected virtual T OnAdding()
        {
            if (IdentityValueIsNone())
            {
                InitIdentityValue();
            }
            return this as T;
        }

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="newData">New data</param>
        /// <returns></returns>
        public IModel OnDataUpdating(T newData)
        {
            return OnUpdating(newData);
        }

        /// <summary>
        /// Add data
        /// </summary>
        /// <returns>Return data</returns>
        public IModel OnDataAdding()
        {
            return OnAdding();
        }

        #endregion
    }
}
