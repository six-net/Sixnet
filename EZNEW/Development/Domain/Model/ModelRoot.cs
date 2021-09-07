using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EZNEW.Development.Domain.Repository;
using EZNEW.Development.Domain.Repository.Warehouse;
using EZNEW.Development.Domain.Event;
using EZNEW.Exceptions;
using EZNEW.DataValidation;
using EZNEW.Expressions;
using EZNEW.Model;

namespace EZNEW.Development.Domain.Model
{
    /// <summary>
    /// Defines model root
    /// </summary>
    [Serializable]
    public abstract class ModelRoot<T> : IModel<T> where T : ModelRoot<T>
    {
        #region Fields

        /// <summary>
        /// The _repository object
        /// </summary>
        private IRepository<T> _repository = RepositoryManager.GetRepository<T>();

        /// <summary>
        /// Indecates whether to load lazy member
        /// </summary>
        bool _allowLoadLazyMember = false;

        /// <summary>
        /// Lazy properties
        /// </summary>
        private Dictionary<string, bool> _loadProperties = new();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the repository
        /// </summary>
        protected IRepository<T> Repository => _repository;

        #endregion

        #region Methods

        #region Save

        /// <summary>
        /// Indecates whether allow to save
        /// </summary>
        /// <returns></returns>
        public bool AllowToSave()
        {
            return SaveValidation();
        }

        /// <summary>
        /// Save validation
        /// </summary>
        /// <returns>Return whether allow to save</returns>
        protected virtual bool SaveValidation()
        {
            return ModelDataManager<T>.SaveValidation(this as T);
        }

        /// <summary>
        /// Save
        /// </summary>
        public virtual Result<T> Save()
        {
            return ModelDataManager<T>.Save(_repository, this as T);
        }

        #endregion

        #region Remove

        /// <summary>
        /// Indecates whether allow to remove
        /// </summary>
        /// <returns></returns>
        public bool AllowToRemove()
        {
            return RemoveValidation();
        }

        /// <summary>
        /// Remove validation
        /// </summary>
        /// <returns>Return whether allow to remove</returns>
        protected virtual bool RemoveValidation()
        {
            return !IdentityValueIsNull();
        }

        /// <summary>
        /// Remove
        /// </summary>
        public virtual Result Remove()
        {
            return ModelDataManager<T>.Remove(_repository, this as T);
        }

        #endregion

        #region LifeSource

        /// <summary>
        /// Indecates whether is a new object
        /// </summary>
        /// <returns></returns>
        public bool IsNew()
        {
            return ModelDataManager<T>.IsNew(_repository, GetType(), this);
        }

        /// <summary>
        /// Mark object is new status
        /// </summary>
        /// <returns>Return whether is successful</returns>
        public virtual bool MarkNew()
        {
            return ModelDataManager<T>.MarkNew(_repository, this);
        }

        /// <summary>
        /// Mark object is stored status
        /// </summary>
        /// <returns>Return whether is successful</returns>
        public virtual bool MarkStored()
        {
            return ModelDataManager<T>.MarkStored(_repository, this);
        }

        #endregion

        #region Lazy member

        /// <summary>
        /// Set load properties
        /// </summary>
        /// <param name="loadProperties">Properties</param>
        /// <returns></returns>
        public virtual void SetLoadProperties(IEnumerable<KeyValuePair<string, bool>> loadProperties)
        {
            ModelDataManager<T>.SetLoadProperties(loadProperties, ref _loadProperties);
        }

        /// <summary>
        /// Set load property
        /// </summary>
        /// <param name="property">Property</param>
        /// <param name="allowLoad">Whether allow load</param>
        public virtual void SetLoadProperty(Expression<Func<T, dynamic>> property, bool allowLoad = true)
        {
            ModelDataManager<T>.SetLoadProperty(property, allowLoad, ref _loadProperties);
        }

        /// <summary>
        /// Close load lazy member
        /// </summary>
        public virtual void CloseLazyMember()
        {
            _allowLoadLazyMember = false;
        }

        /// <summary>
        /// Open load lazy member
        /// </summary>
        public virtual void OpenLazyMember()
        {
            _allowLoadLazyMember = true;
        }

        /// <summary>
        /// Check property whether allow to lazy load
        /// </summary>
        /// <param name="property">Property</param>
        /// <returns>Return wheather property allow load data</returns>
        protected virtual bool AllowLoad(string property)
        {
            if (!_allowLoadLazyMember || _loadProperties.IsNullOrEmpty() || !_loadProperties.ContainsKey(property))
            {
                return false;
            }
            return _loadProperties[property];
        }

        /// <summary>
        /// Check property whether allow to lazy load
        /// </summary>
        /// <param name="property">Property</param>
        /// <returns>Return wheather property allow load data</returns>
        protected virtual bool AllowLoad(Expression<Func<T, dynamic>> property)
        {
            if (property == null)
            {
                return false;
            }
            return AllowLoad(ExpressionHelper.GetExpressionPropertyName(property.Body));
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
            return AllowLoad(property) && !(lazyMember.CurrentValue?.IdentityValueIsNull() ?? true);
        }

        #endregion

        #region Identity value

        /// <summary>
        /// Init primary value
        /// </summary>
        public abstract void InitIdentityValue();

        /// <summary>
        /// Check identity value is null
        /// </summary>
        /// <returns>Return identity value whether has value</returns>
        public abstract bool IdentityValueIsNull();

        /// <summary>
        /// Get identity value
        /// </summary>
        /// <returns>Return model identity value</returns>
        public abstract string GetIdentityValue();

        #endregion

        #region Updating

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
        /// Update data
        /// </summary>
        /// <param name="newData">New data</param>
        /// <returns></returns>
        protected virtual T OnUpdating(T newData)
        {
            return newData;
        }

        #endregion

        #region Adding

        /// <summary>
        /// Add data
        /// </summary>
        /// <returns>Return data</returns>
        public IModel OnDataAdding()
        {
            if (IdentityValueIsNull())
            {
                InitIdentityValue();
            }
            return OnAdding();
        }

        /// <summary>
        /// Add data
        /// </summary>
        /// <returns>Return data</returns>
        protected virtual T OnAdding()
        {
            return this as T;
        }

        #endregion

        #region Equal

        /// <summary>
        /// Compare two objects
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns></returns>
        public virtual bool Equals(T data)
        {
            return data?.GetIdentityValue() == GetIdentityValue();
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
            return GetIdentityValue()?.GetHashCode() ?? 0;
        }

        #endregion

        #region Repository

        /// <summary>
        /// Set _repository
        /// </summary>
        /// <param name="repository">Repository</param>
        protected void SetRepository(IRepository<T> repository)
        {
            if (_repository != null)
            {
                _repository = repository;
            }
        }

        /// <summary>
        /// Get _repository
        /// </summary>
        /// <returns></returns>
        protected IRepository<T> GetRepository()
        {
            return _repository;
        }

        #endregion

        #endregion
    }
}
