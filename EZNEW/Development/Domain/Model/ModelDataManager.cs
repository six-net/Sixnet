using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using EZNEW.DataValidation;
using EZNEW.Development.Domain.Event;
using EZNEW.Development.Domain.Repository;
using EZNEW.Development.Domain.Repository.Warehouse;
using EZNEW.Development.UnitOfWork;
using EZNEW.Exceptions;
using EZNEW.Expressions;
using EZNEW.Model;
using EZNEW.Serialization;

namespace EZNEW.Development.Domain.Model
{
    /// <summary>
    /// Defines model data manager
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class ModelDataManager<T> where T : IModel<T>
    {
        internal static bool IsNew(IRepository<T> repository, Type dataType, IModel data)
        {
            if (data?.IdentityValueIsNull() ?? true)
            {
                return true;
            }
            CheckRepository(repository);
            var isVirtual = ModelManager.IsVirtualModel(dataType);
            return isVirtual || repository.GetDataSource(data) == DataSource.New;
        }

        internal static bool MarkNew(IRepository<T> repository, IModel data)
        {
            CheckRepository(repository);
            repository?.ModifyDataSource(data, DataSource.New);
            return true;
        }

        internal static bool MarkStored(IRepository<T> repository, IModel data)
        {
            CheckRepository(repository);
            repository.ModifyDataSource(data, DataSource.Storage);
            return true;
        }

        internal static void SetLoadProperties(IEnumerable<KeyValuePair<string, bool>> loadProperties, ref Dictionary<string, bool> sourceLoadProperties)
        {
            if (loadProperties == null)
            {
                return;
            }
            sourceLoadProperties ??= new Dictionary<string, bool>();
            foreach (var property in loadProperties)
            {
                sourceLoadProperties[property.Key] = property.Value;
            }
        }

        internal static void SetLoadProperty(Expression<Func<T, dynamic>> property, bool allowLoad, ref Dictionary<string, bool> sourceLoadProperties)
        {
            if (property == null)
            {
                return;
            }
            Dictionary<string, bool> propertyDict = new Dictionary<string, bool>()
            {
                { ExpressionHelper.GetExpressionPropertyName(property.Body),allowLoad}
            };
            SetLoadProperties(propertyDict, ref sourceLoadProperties);
        }

        internal static Result<T> Save(IRepository<T> repository, T data, ActivationOptions activationOptions)
        {
            CheckRepository(repository);
            var saveData = repository.Save(data, activationOptions);
            if (saveData == null)
            {
                return Result<T>.FailedResult("Data saved failed");
            }
            DomainEventBus.Publish(new DefaultSaveDomainEvent<T>()
            {
                Object = saveData
            });
            return Result<T>.SuccessResult(saveData, "Data saved successfully", "");
        }

        internal static Result Remove(IRepository<T> repository, T data, ActivationOptions activationOptions)
        {
            CheckRepository(repository);
            repository.Remove(data, activationOptions);
            DomainEventBus.Publish(new DefaultRemoveDomainEvent<T>()
            {
                Object = data
            });
            return Result.SuccessResult("Data removed successfully");
        }

        internal static bool SaveValidation(T data)
        {
            var verifyResults = ValidationManager.Validate(data, ValidationConstants.UseCaseNames.Domain);
            var errorMessages = verifyResults.GetErrorMessages();
            if (!errorMessages.IsNullOrEmpty())
            {
                throw new EZNEWException(JsonSerializer.Serialize(errorMessages));
            }
            return true;
        }

        internal static void CheckRepository(IRepository<T> repository)
        {
            if (repository == null)
            {
                throw new EZNEWException($"{typeof(T)}'s repository is null");
            }
        }
    }
}
