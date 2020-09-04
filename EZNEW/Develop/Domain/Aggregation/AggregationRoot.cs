using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EZNEW.Develop.Domain.Repository;
using EZNEW.Develop.Domain.Repository.Warehouse;
using EZNEW.Develop.Domain.Event;
using EZNEW.Fault;
using EZNEW.DataValidation;
using EZNEW.ExpressionUtil;
using EZNEW.Response;
using System.DirectoryServices.Protocols;
using EZNEW.ValueType;

namespace EZNEW.Develop.Domain.Aggregation
{
    /// <summary>
    /// Aggregation root
    /// </summary>
    [Serializable]
    public abstract class AggregationRoot<T> : IAggregationRoot<T> where T : AggregationRoot<T>
    {
        static AggregationRoot()
        {
            AggregationManager.ConfigureAggregationModel<T>();
        }

        #region Fields

        /// <summary>
        /// The repository object
        /// </summary>
        protected IAggregationRepository<T> repository = null;

        /// <summary>
        /// The default identity
        /// </summary>
        private Guid defaultIdentity = Guid.NewGuid();

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether allow to save
        /// </summary>
        public bool CanBeSave
        {
            get
            {
                return SaveValidation();
            }
        }

        /// <summary>
        /// Gets whether allow to remove
        /// </summary>
        public bool CanBeRemove
        {
            get
            {
                return RemoveValidation();
            }
        }

        /// <summary>
        /// Gets whether allow lazy data load
        /// </summary>
        protected bool LoadLazyMember { get; set; } = true;

        /// <summary>
        /// Gets all of the properties allow to load data
        /// </summary>
        protected Dictionary<string, bool> LoadProperties { get; set; } = new Dictionary<string, bool>();

        /// <summary>
        /// Gets the identity value
        /// </summary>
        public string IdentityValue
        {
            get
            {
                return GetIdentityValue();
            }
        }

        /// <summary>
        /// Gets whether the object is new
        /// </summary>
        public bool IsNew
        {
            get
            {
                var isVirtual = AggregationManager.IsVirtualAggregation(GetType());
                return isVirtual || repository.GetLifeSource(this) == DataLifeSource.New;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Save validation
        /// </summary>
        /// <returns>Return whether allow to save</returns>
        protected virtual bool SaveValidation()
        {
            var verifyResults = ValidationManager.Validate(this);
            var errorMessages = verifyResults.GetErrorMessages();
            if (!errorMessages.IsNullOrEmpty())
            {
                throw new EZNEWException(string.Join("\n", errorMessages));
            }
            return true;
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
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }
            repository?.ModifyLifeSource(this, DataLifeSource.New);
            return true;
        }

        /// <summary>
        /// Mark object is stored status
        /// </summary>
        /// <returns>Return whether is successful</returns>
        public virtual bool MarkStored()
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }
            repository.ModifyLifeSource(this, DataLifeSource.DataSource);
            return true;
        }

        /// <summary>
        /// Set load properties
        /// </summary>
        /// <param name="loadProperties">Properties</param>
        /// <returns></returns>
        public virtual void SetLoadProperties(IEnumerable<KeyValuePair<string, bool>> loadProperties)
        {
            if (loadProperties == null)
            {
                return;
            }
            LoadProperties ??= new Dictionary<string, bool>();
            foreach (var property in loadProperties)
            {
                LoadProperties[property.Key] = property.Value;
            }
        }

        /// <summary>
        /// Set load property
        /// </summary>
        /// <param name="property">Property</param>
        /// <param name="allowLoad">Whether allow load</param>
        public virtual void SetLoadProperty(Expression<Func<T, dynamic>> property, bool allowLoad = true)
        {
            if (property == null)
            {
                return;
            }
            Dictionary<string, bool> propertyDic = new Dictionary<string, bool>()
            {
                { ExpressionHelper.GetExpressionPropertyName(property.Body),allowLoad}
            };
            SetLoadProperties(propertyDic);
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
        protected virtual bool AllowLoad<TModel>(Expression<Func<T, dynamic>> property, LazyMember<TModel> lazyMember) where TModel : IAggregationRoot<TModel>
        {
            return AllowLazyLoad(property) && !(lazyMember.CurrentValue?.IdentityValueIsNone() ?? true);
        }

        /// <summary>
        /// Save
        /// </summary>
        public virtual Result<T> Save()
        {
            var saveData = repository.Save(this as T);
            if (saveData == null)
            {
                return Result<T>.FailedResult("Data saved failed");
            }
            DomainEventBus.Publish(new DefaultAggregationSaveDomainEvent<T>()
            {
                Object = saveData
            });
            return Result<T>.SuccessResult("Data saved successfully", "", saveData);
        }

        /// <summary>
        /// Remove
        /// </summary>
        public virtual Result Remove()
        {
            if (IdentityValueIsNone())
            {
                throw new EZNEWException("The object does not have an identity value set and cannot perform a remove");
            }
            T removeData = this as T;
            repository.Remove(removeData);
            DomainEventBus.Publish(new DefaultAggregationRemoveDomainEvent<T>()
            {
                Object = removeData
            });
            return Result.SuccessResult("Data removed successfully");
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
            return true;
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
        public sealed override bool Equals(object data)
        {
            return Equals(data as T);
        }

        /// <summary>
        /// Get hash code
        /// </summary>
        /// <returns>Return model hash code</returns>
        public sealed override int GetHashCode()
        {
            return IdentityValue.GetHashCode();
        }

        /// <summary>
        /// Get identity value
        /// </summary>
        /// <returns>Return model identity value</returns>
        protected virtual string GetIdentityValue()
        {
            return defaultIdentity.ToString();
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

        #endregion
    }
}
