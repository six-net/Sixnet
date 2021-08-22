using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using EZNEW.DataValidation;
using EZNEW.Development.Domain.Event;
using EZNEW.Development.Domain.Repository;
using EZNEW.Development.Domain.Repository.Warehouse;
using EZNEW.Exceptions;
using EZNEW.Expressions;
using EZNEW.Model;

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
            var isVirtual = ModelManager.IsVirtualModel(dataType);
            return isVirtual || repository.GetLifeSource(data) == DataLifeSource.New;
        }

        internal static bool MarkNew(IRepository<T> repository, IModel data)
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }
            repository?.ModifyLifeSource(data, DataLifeSource.New);
            return true;
        }

        internal static bool MarkStored(IRepository<T> repository, IModel data)
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }
            repository.ModifyLifeSource(data, DataLifeSource.DataSource);
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

        internal static Result<T> Save(IRepository<T> repository, T data)
        {
            var saveData = repository.Save(data);
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

        internal static Result Remove(IRepository<T> repository, T data)
        {
            repository.Remove(data);
            DomainEventBus.Publish(new DefaultRemoveDomainEvent<T>()
            {
                Object = data
            });
            return Result.SuccessResult("Data removed successfully");
        }

        internal static bool SaveValidation(T data)
        {
            var verifyResults = ValidationManager.Validate(data);
            var errorMessages = verifyResults.GetErrorMessages();
            if (!errorMessages.IsNullOrEmpty())
            {
                throw new EZNEWException(string.Join("\n", errorMessages));
            }
            return true;
        }
    }
}
