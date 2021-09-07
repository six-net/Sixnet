using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using EZNEW.Code;
using EZNEW.Development.Domain.Model;
using EZNEW.Development.Domain.Repository;
using EZNEW.Expressions;
using EZNEW.Model;

namespace EZNEW.Development.Entity
{
    /// <summary>
    /// Defines model entity
    /// </summary>
    public abstract class ModelEntity<T> : BaseEntity<T>, IModel<T> where T : BaseEntity<T>, IModel<T>, new()
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
        public virtual void InitIdentityValue()
        {
            var primaryKeys = EntityManager.GetPrimaryKeys<T>();
            if (primaryKeys.IsNullOrEmpty())
            {
                return;
            }
            foreach (var pk in primaryKeys)
            {
                var field = EntityManager.GetEntityField(typeof(T), pk);
                if (field != null)
                {
                    var valueType = field.DataType?.GetRealValueType();
                    var typeCode = Type.GetTypeCode(valueType);
                    switch (typeCode)
                    {
                        case TypeCode.Byte:
                            SetValue(pk, RandomNumberHelper.GetRandomNumber(byte.MaxValue, 1));
                            break;
                        case TypeCode.SByte:
                            SetValue(pk, RandomNumberHelper.GetRandomNumber(sbyte.MaxValue, 1));
                            break;
                        case TypeCode.Int16:
                            SetValue(pk, RandomNumberHelper.GetRandomNumber(short.MaxValue, 1));
                            break;
                        case TypeCode.UInt16:
                            SetValue(pk, RandomNumberHelper.GetRandomNumber(ushort.MaxValue, 1));
                            break;
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                            SetValue(pk, RandomNumberHelper.GetRandomNumber(int.MaxValue, 1));
                            break;
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Double:
                        case TypeCode.Single:
                        case TypeCode.Decimal:
                            SetValue(pk, SerialNumber.GenerateSerialNumber<T>());
                            break;
                        case TypeCode.String:
                            SetValue(pk, SerialNumber.GenerateSerialNumber<T>().ToString());
                            break;
                        case TypeCode.DateTime:
                            SetValue(pk, DateTime.Now);
                            break;
                        default:
                            if (valueType == typeof(Guid))
                            {
                                SetValue(pk, Guid.NewGuid());
                            }
                            else if (valueType == typeof(DateTimeOffset))
                            {
                                SetValue(pk, DateTimeOffset.Now);
                            }
                            else
                            {
                                throw new InvalidOperationException($"Initialization values are not supported for {field.DataType}.");
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Check identity value is null
        /// </summary>
        /// <returns>Return identity value whether has value</returns>
        public virtual bool IdentityValueIsNull()
        {
            var primaryKeys = EntityManager.GetPrimaryKeys<T>();
            if (primaryKeys.IsNullOrEmpty())
            {
                return true;
            }
            bool identityValueIsNull = false;
            foreach (var pk in primaryKeys)
            {
                var field = EntityManager.GetEntityField(typeof(T), pk);
                identityValueIsNull = field == null;
                if (!identityValueIsNull)
                {
                    var valueType = field.DataType?.GetRealValueType();
                    var typeCode = Type.GetTypeCode(valueType);
                    var fieldValue = field.ValueProvider.Get(this);
                    switch (typeCode)
                    {
                        case TypeCode.Byte:
                            identityValueIsNull = !byte.TryParse(fieldValue?.ToString(), out var byteValue) || byteValue < 1;
                            break;
                        case TypeCode.SByte:
                            identityValueIsNull = !sbyte.TryParse(fieldValue?.ToString(), out var sbyteValue) || sbyteValue < 1;
                            break;
                        case TypeCode.Int16:
                            identityValueIsNull = !short.TryParse(fieldValue?.ToString(), out var shortValue) || shortValue < 1;
                            break;
                        case TypeCode.UInt16:
                            identityValueIsNull = !ushort.TryParse(fieldValue?.ToString(), out var ushortValue) || ushortValue < 1;
                            break;
                        case TypeCode.Int32:
                            identityValueIsNull = !int.TryParse(fieldValue?.ToString(), out var intValue) || intValue < 1;
                            break;
                        case TypeCode.UInt32:
                            identityValueIsNull = !uint.TryParse(fieldValue?.ToString(), out var uintValue) || uintValue < 1;
                            break;
                        case TypeCode.Int64:
                            identityValueIsNull = !long.TryParse(fieldValue?.ToString(), out var longValue) || longValue < 1;
                            break;
                        case TypeCode.UInt64:
                            identityValueIsNull = !ulong.TryParse(fieldValue?.ToString(), out var ulongValue) || ulongValue < 1;
                            break;
                        case TypeCode.Double:
                        case TypeCode.Single:
                            identityValueIsNull = !double.TryParse(fieldValue?.ToString(), out var doubleValue) || doubleValue < 1;
                            break;
                        case TypeCode.Decimal:
                            identityValueIsNull = !decimal.TryParse(fieldValue?.ToString(), out var decimalValue) || decimalValue < 1;
                            break;
                        case TypeCode.String:
                            identityValueIsNull = string.IsNullOrWhiteSpace(fieldValue?.ToString());
                            break;
                        case TypeCode.DateTime:
                            identityValueIsNull = !DateTime.TryParse(fieldValue?.ToString(), out var dateTimeValue) || dateTimeValue <= DateTime.MinValue;
                            break;
                        default:
                            if (valueType == typeof(Guid))
                            {
                                identityValueIsNull = !Guid.TryParse(fieldValue?.ToString(), out var guidValue) || guidValue.Equals(Guid.Empty);
                            }
                            else if (valueType == typeof(DateTimeOffset))
                            {
                                identityValueIsNull = !DateTimeOffset.TryParse(fieldValue?.ToString(), out var dateTimeOffsetValue) && dateTimeOffsetValue <= DateTimeOffset.MinValue;
                            }
                            else
                            {
                                throw new InvalidOperationException($"Not supported check value for {field.DataType}.");
                            }
                            break;
                    }
                }
                if (identityValueIsNull)
                {
                    break;
                }
            }
            return identityValueIsNull;
        }

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
        internal protected virtual T OnUpdating(T newData)
        {
            var entityConfig = EntityManager.GetEntityConfiguration<T>();
            if (!(entityConfig?.AllFields.IsNullOrEmpty() ?? true))
            {
                foreach (var fieldItem in entityConfig.AllFields)
                {
                    if (fieldItem.Value.InRole(FieldRole.CreationTime)
                        || fieldItem.Value.InRole(FieldRole.UpdateTime)
                        || fieldItem.Value.InRole(FieldRole.Version)
                        || fieldItem.Value.InRole(FieldRole.PrimaryKey))
                    {
                        continue;
                    }
                    SetValue(fieldItem.Key, newData.GetValue(fieldItem.Key));
                }
            }
            return this as T;
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
        internal protected virtual T OnAdding()
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
        /// <param name="_repository">Repository</param>
        protected void SetRepository(IRepository<T> repository)
        {
            if (repository != null)
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
