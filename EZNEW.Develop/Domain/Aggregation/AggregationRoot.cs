using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using System.Runtime.Serialization.Formatters.Binary;
using EZNEW.Framework.Extension;
using EZNEW.Framework;
using EZNEW.Develop.DataValidation;
using EZNEW.Develop.Domain.Repository;
using EZNEW.Framework.ExpressionUtil;
using EZNEW.Framework.Serialize;
using EZNEW.Develop.Entity;
using EZNEW.Develop.Domain.Repository.Warehouse;
using EZNEW.Develop.Domain.Event;

namespace EZNEW.Develop.Domain.Aggregation
{
    /// <summary>
    /// aggregation root
    /// </summary>
    [Serializable]
    public abstract class AggregationRoot<T> : IAggregationRoot<T> where T : AggregationRoot<T>
    {
        static AggregationRoot()
        {
            AggregationManager.ConfigAggregationConfig<T>();
        }

        #region fields

        /// <summary>
        /// enable lazy load
        /// </summary>
        protected bool loadLazyMember = true;

        //allow load propertys
        protected Dictionary<string, bool> allowLoadPropertys = new Dictionary<string, bool>();

        /// <summary>
        /// repository
        /// </summary>
        protected IAggregationRepository<T> repository = null;

        /// <summary>
        /// default identity
        /// </summary>
        private Guid defaultIdentity = Guid.NewGuid();

        #endregion

        #region propertys

        /// <summary>
        /// allow to save
        /// </summary>
        public bool CanBeSave
        {
            get
            {
                return SaveValidation();
            }
        }

        /// <summary>
        /// allow to remove
        /// </summary>
        public bool CanBeRemove
        {
            get
            {
                return RemoveValidation();
            }
        }

        /// <summary>
        /// allow lazy data load
        /// </summary>
        protected bool LoadLazyMember
        {
            get
            {
                return loadLazyMember;
            }
            set
            {
                loadLazyMember = value;
            }
        }

        /// <summary>
        /// allow load data propertys
        /// </summary>
        protected Dictionary<string, bool> LoadPropertys
        {
            get
            {
                return allowLoadPropertys;
            }
            set
            {
                allowLoadPropertys = value;
            }
        }

        /// <summary>
        /// get identity value
        /// </summary>
        public string IdentityValue
        {
            get
            {
                return GetIdentityValue();
            }
        }

        /// <summary>
        /// is new object
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

        #region methods

        /// <summary>
        /// save validation
        /// </summary>
        /// <returns></returns>
        protected virtual bool SaveValidation()
        {
            //validate object primary value
            if (IdentityValueIsNone())
            {
                if (IsNew)
                {
                    InitIdentityValue();
                }
                else
                {
                    throw new Exception("the identity value for the object to be saved is not specified");
                }
            }
            var verifyResults = ValidationManager.Validate(this);
            string[] errorMessages = verifyResults.GetErrorMessage();
            if (errorMessages != null && errorMessages.Length > 0)
            {
                throw new Exception(string.Join("\n", errorMessages));
            }
            return true;
        }

        /// <summary>
        /// remove validation
        /// </summary>
        /// <returns></returns>
        protected virtual bool RemoveValidation()
        {
            return !IdentityValueIsNone();
        }

        /// <summary>
        /// mark object is new
        /// </summary>
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
        /// mark object is stored
        /// </summary>
        /// <returns></returns>
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
        /// set load propertys
        /// </summary>
        /// <param name="loadPropertys">propertys</param>
        /// <returns></returns>
        public virtual void SetLoadPropertys(Dictionary<string, bool> loadPropertys)
        {
            if (loadPropertys == null)
            {
                return;
            }
            allowLoadPropertys = allowLoadPropertys ?? new Dictionary<string, bool>();
            foreach (var property in loadPropertys)
            {
                allowLoadPropertys[property.Key] = property.Value;
            }
        }

        /// <summary>
        /// set load propertys
        /// </summary>
        /// <param name="property">property</param>
        /// <param name="allowLoad">allow load</param>
        public virtual void SetLoadPropertys(Expression<Func<T, dynamic>> property, bool allowLoad = true)
        {
            if (property == null)
            {
                return;
            }
            Dictionary<string, bool> propertyDic = new Dictionary<string, bool>()
            {
                { ExpressionHelper.GetExpressionPropertyName(property.Body),allowLoad}
            };
            SetLoadPropertys(propertyDic);
        }

        /// <summary>
        /// close lazy data load
        /// </summary>
        public virtual void CloseLazyMemberLoad()
        {
            loadLazyMember = false;
        }

        /// <summary>
        /// open lazy data load
        /// </summary>
        public virtual void OpenLazyMemberLoad()
        {
            loadLazyMember = true;
        }

        /// <summary>
        /// allow lazy load
        /// </summary>
        /// <param name="property">property</param>
        /// <returns>wheather allow load property</returns>
        protected virtual bool AllowLazyLoad(string property)
        {
            if (!loadLazyMember || allowLoadPropertys == null || !allowLoadPropertys.ContainsKey(property))
            {
                return false;
            }
            return allowLoadPropertys[property];
        }

        /// <summary>
        /// allow lazy load
        /// </summary>
        /// <param name="property">property</param>
        /// <returns>wheather allow load property</returns>
        protected virtual bool AllowLazyLoad(Expression<Func<T, dynamic>> property)
        {
            if (property == null)
            {
                return false;
            }
            return AllowLazyLoad(ExpressionHelper.GetExpressionPropertyName(property.Body));
        }

        /// <summary>
        /// save
        /// </summary>
        public void Save()
        {
            SaveAsync().Wait();
        }

        /// <summary>
        /// save
        /// </summary>
        public virtual async Task SaveAsync()
        {
            await repository.SaveAsync((T)this).ConfigureAwait(false);
            DomainEventBus.Publish(new DefaultAggregationSaveDomainEvent<T>()
            {
                Object = this as T
            });
        }

        /// <summary>
        /// remove
        /// </summary>
        public void Remove()
        {
            RemoveAsync().Wait();
        }

        /// <summary>
        /// remove
        /// </summary>
        public virtual async Task RemoveAsync()
        {
            await repository.RemoveAsync((T)this).ConfigureAwait(false);
            DomainEventBus.Publish(new DefaultAggregationRemoveDomainEvent<T>()
            {
                Object = this as T
            });
        }

        /// <summary>
        /// init from similar object
        /// </summary>
        /// <typeparam name="DT">data type</typeparam>
        /// <param name="similarObject">similar object</param>
        /// <returns></returns>
        public virtual void InitFromSimilarObject<DT>(DT similarObject) where DT : AggregationRoot<T>, T
        {
            if (similarObject == null)
            {
                return;
            }
            CopyDataFromSimilarObject(similarObject);//copy data
        }

        /// <summary>
        /// copy data from similar object
        /// </summary>
        /// <typeparam name="DT">data type</typeparam>
        /// <param name="similarObject">similar object</param>
        /// <param name="excludePropertys">not copy propertys</param>
        protected virtual void CopyDataFromSimilarObject<DT>(DT similarObject, IEnumerable<string> excludePropertys = null) where DT : T
        {
        }

        /// <summary>
        /// init primary value
        /// </summary>
        public virtual void InitIdentityValue()
        { }

        /// <summary>
        /// primary value is none
        /// </summary>
        /// <returns></returns>
        public virtual bool IdentityValueIsNone()
        {
            return true;
        }

        /// <summary>
        /// compare two objects
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool Equals(T obj)
        {
            if (obj == null)
            {
                return false;
            }
            return obj.IdentityValue == IdentityValue;
        }

        /// <summary>
        /// compare two objects
        /// </summary>
        /// <param name="obj">target object</param>
        /// <returns></returns>
        public sealed override bool Equals(object obj)
        {
            return Equals(obj as T);
        }

        /// <summary>
        /// get hash code
        /// </summary>
        /// <returns></returns>
        public sealed override int GetHashCode()
        {
            return 0;
        }

        /// <summary>
        /// get identity value
        /// </summary>
        /// <returns></returns>
        protected virtual string GetIdentityValue()
        {
            return defaultIdentity.ToString();
        }

        #endregion
    }
}
