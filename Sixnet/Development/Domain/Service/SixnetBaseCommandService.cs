// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sixnet.DependencyInjection;
using Sixnet.Development.Data;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Data.Field.Formatting;
using Sixnet.Development.Domain.Parameter;
using Sixnet.Development.Entity;
using Sixnet.Development.Events.Data;
using Sixnet.Development.Queryable;
using Sixnet.Development.Repository;
using Sixnet.Exceptions;
using Sixnet.Expressions.Linq;

namespace Sixnet.Development.Domain.Service
{
    public abstract class SixnetBaseCommandService<T> : SixnetBaseDomainService<T> where T : SixnetBaseEntity<T>, new()
    {
        /// <summary>
        /// Get identity queryable 
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        ISixnetQueryable GetIdentityQueryable(IEnumerable<T> datas, bool withoutVersion = true)
        {
            if (datas.IsNullOrEmpty())
            {
                return SixnetQuerier.Create<T>(d => 0 == 1);
            }
            else
            {
                var ignorePks = new List<string>();
                if (isVersioning && withoutVersion && !string.IsNullOrWhiteSpace(versionFieldName))
                {
                    ignorePks.Add(versionFieldName);
                }
                return SixnetConditionExtensions.IncludeEntities(null, datas, ignorePks);
            }
        }

        #region Record lastest version data

        int RecordLatestVersionData(IEnumerable<T> datas, Action<SixnetDataOperationOptions> configure = null)
        {
            if (datas.IsNullOrEmpty())
            {
                return 0;
            }

            var fieldsAssignment = new SixnetFieldsAssignment();
            fieldsAssignment.SetNewValue(historyTagFieldName, true);

            var condition = GetIdentityQueryable(datas);
            var historicalCriterion = GetHistoryTagCriterion(false);
            condition.Where(historicalCriterion);

            if (isSeparateVersioning)
            {
                repository.Update(fieldsAssignment, condition, options =>
                {
                    options.FromVersionTable();
                    configure?.Invoke(options);
                });

                return repository.Add(datas, options =>
                {
                    options.FromVersionTable();
                    options.EnableInsertIncrementField();
                    configure?.Invoke(options);
                });
            }
            else
            {
                repository.Update(fieldsAssignment, condition, configure);
                return repository.Add(datas, configure);
            }
        }

        async Task<int> RecordLatestVersionDataAsync(IEnumerable<T> datas, Action<SixnetDataOperationOptions> configure = null)
        {
            if (datas.IsNullOrEmpty())
            {
                return 0;
            }

            var fieldsAssignment = new SixnetFieldsAssignment();
            fieldsAssignment.SetNewValue(historyTagFieldName, true);

            var condition = GetIdentityQueryable(datas);
            var historicalCriterion = GetHistoryTagCriterion(false);
            condition.Where(historicalCriterion);

            if (isSeparateVersioning)
            {
                await repository.UpdateAsync(fieldsAssignment, condition, options =>
                {
                    options.FromVersionTable();
                    configure?.Invoke(options);
                }).ConfigureAwait(false);

                return await repository.AddAsync(datas, options =>
                {
                    options.FromVersionTable();
                    options.EnableInsertIncrementField();
                    configure?.Invoke(options);
                }).ConfigureAwait(false);
            }
            else
            {
                await repository.UpdateAsync(fieldsAssignment, condition, configure).ConfigureAwait(false);
                return await repository.AddAsync(datas, configure).ConfigureAwait(false);
            }
        }

        #endregion

        #region Save data

        /// <summary>
        /// Save data
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        protected T SaveData(SaveDataParameter<T> parameter, Action<SixnetDataOperationOptions> configure = null)
        {
            var newData = parameter?.NewData;
            SixnetThrower.ThrowArgNullIf(newData == null, nameof(parameter));

            // save before
            SaveDataBefore(parameter, configure);

            T originalCurrentData = default;
            if (newData.IdentityValueIsNull())
            {
                HandleAdding(parameter);

                if (isVersioning)
                {
                    newData.SetValue(versionFieldName, 1);
                    newData.SetValue(historyTagFieldName, false);
                }

                repository.Add(newData, options =>
                {
                    options.FromPrimaryTable();
                    configure?.Invoke(options);
                });

                if (isSeparateVersioning)
                {
                    repository.Add(newData, options =>
                    {
                        options.FromVersionTable();
                        options.EnableInsertIncrementField();
                        configure?.Invoke(options);
                    });
                }
            }
            else
            {
                var currentData = GetCurrentDatas(GetIdentityQueryable([newData]))?.FirstOrDefault();
                SixnetThrower.ThrowGetDataFailedIf<T>(currentData == null);

                originalCurrentData = currentData.Clone();

                // update date
                HandleUpdating(currentData, parameter);

                if (isVersioning)
                {
                    currentData.SetValue(versionFieldName, currentData.GetValue(versionFieldName) + 1);
                    currentData.SetValue(historyTagFieldName, false);

                    if (isSeparateVersioning)
                    {
                        repository.Update(currentData, options =>
                        {
                            options.FromPrimaryTable();
                            configure?.Invoke(options);
                        });
                    }

                    RecordLatestVersionData([currentData], configure);
                }
                else
                {
                    repository.Update(currentData, configure);
                }

                newData = currentData;
            }

            // save data after
            SaveDataAfter(new SaveDataAfterContext<T>()
            {
                SaveParameter = parameter,
                OldData = newData,
            }, configure);

            return newData;
        }

        /// <summary>
        /// Save data
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        protected async Task<T> SaveDataAsync(SaveDataParameter<T> parameter, Action<SixnetDataOperationOptions> configure = null)
        {
            var newData = parameter?.NewData;
            SixnetThrower.ThrowArgNullIf(newData == null, typeof(T).Name);

            // save before
            await SaveDataBeforeAsync(parameter, configure).ConfigureAwait(false);

            T originalCurrentData = default;
            if (newData.IdentityValueIsNull())
            {
                await HandleAddingAsync(parameter).ConfigureAwait(false);

                if (isVersioning)
                {
                    newData.SetValue(versionFieldName, 1);
                    newData.SetValue(historyTagFieldName, false);
                }

                await repository.AddAsync(newData, options =>
                {
                    options.FromPrimaryTable();
                    configure?.Invoke(options);
                }).ConfigureAwait(false);

                if (isSeparateVersioning)
                {
                    await repository.AddAsync(newData, options =>
                    {
                        options.FromVersionTable();
                        options.EnableInsertIncrementField();
                        configure?.Invoke(options);
                    }).ConfigureAwait(false);
                }
            }
            else
            {
                var currentData = (await GetCurrentDatasAsync(GetIdentityQueryable([newData])).ConfigureAwait(false))?.FirstOrDefault();
                SixnetThrower.ThrowGetDataFailedIf<T>(currentData == null);

                originalCurrentData = currentData.Clone();

                await HandleUpdatingAsync(currentData, parameter).ConfigureAwait(false);

                if (isVersioning)
                {
                    currentData.SetValue(versionFieldName, currentData.GetValue(versionFieldName) + 1);
                    currentData.SetValue(historyTagFieldName, false);

                    if (isSeparateVersioning)
                    {
                        await repository.UpdateAsync(currentData, options =>
                        {
                            options.FromPrimaryTable();
                            configure?.Invoke(options);
                        }).ConfigureAwait(false);
                    }

                    await RecordLatestVersionDataAsync([currentData], configure).ConfigureAwait(false);
                }
                else
                {
                    await repository.UpdateAsync(currentData, configure).ConfigureAwait(false);
                }

                newData = currentData;
            }

            // save data after
            await SaveDataAfterAsync(new SaveDataAfterContext<T>()
            {
                SaveParameter = parameter,
                OldData = originalCurrentData
            }, configure).ConfigureAwait(false);

            return newData;
        }

        /// <summary>
        /// Handle adding
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="configure"></param>
        protected virtual void HandleAdding(SaveDataParameter<T> parameter, Action<SixnetDataOperationOptions> configure = null)
        {
        }

        /// <summary>
        /// Handle adding
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="configure"></param>
        protected virtual Task HandleAddingAsync(SaveDataParameter<T> parameter, Action<SixnetDataOperationOptions> configure = null)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Handle updating
        /// </summary>
        /// <param name="currentData"></param>
        /// <param name="parameter"></param>
        /// <param name="configure"></param>
        protected virtual void HandleUpdating(T currentData, SaveDataParameter<T> parameter, Action<SixnetDataOperationOptions> configure = null)
        {
            currentData.ModifyFrom(parameter.NewData);
        }

        /// <summary>
        /// Handle updating
        /// </summary>
        /// <param name="currentData"></param>
        /// <param name="parameter"></param>
        /// <param name="configure"></param>
        protected virtual Task HandleUpdatingAsync(T currentData, SaveDataParameter<T> parameter, Action<SixnetDataOperationOptions> configure = null)
        {
            currentData.ModifyFrom(parameter.NewData);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Save data before
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="configure"></param>
        protected virtual void SaveDataBefore(SaveDataParameter<T> parameter, Action<SixnetDataOperationOptions> configure = null)
        {
        }

        /// <summary>
        /// Save data before
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="configure"></param>
        protected virtual Task SaveDataBeforeAsync(SaveDataParameter<T> parameter, Action<SixnetDataOperationOptions> configure = null)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Save data after
        /// </summary>
        /// <param name="context"></param>
        /// <param name="configure"></param>
        protected virtual void SaveDataAfter(SaveDataAfterContext<T> context, Action<SixnetDataOperationOptions> configure = null)
        {
        }

        /// <summary>
        /// Save data after
        /// </summary>
        /// <param name="context"></param>
        /// <param name="configure"></param>
        protected virtual Task SaveDataAfterAsync(SaveDataAfterContext<T> context, Action<SixnetDataOperationOptions> configure = null)
        {
            return Task.CompletedTask;
        }

        #endregion

        #region Update

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="fieldsAssignment"></param>
        /// <param name="queryable"></param>
        /// <param name="configure"></param>
        protected int UpdateData(SixnetFieldsAssignment fieldsAssignment, ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null)
        {
            SixnetThrower.ThrowArgNullIf(fieldsAssignment == null);

            if (isVersioning)
            {
                var affectRows = 0;
                fieldsAssignment.RemoveNewValue(versionFieldName);
                fieldsAssignment.RemoveNewValue(historyTagFieldName);

                if (isSeparateVersioning)
                {
                    var addVersionValue = SixnetDataField.Create(versionFieldName, typeof(T), fieldFormatSetting: SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.ADD, SixnetConstantField.Create(1)));
                    fieldsAssignment.SetNewValue(versionFieldName, addVersionValue);
                    fieldsAssignment.SetNewValue(historyTagFieldName, SixnetConstantField.Create(false));

                    affectRows = repository.Update(fieldsAssignment, queryable, options =>
                    {
                        options.FromPrimaryTable();
                        configure?.Invoke(options);
                    });
                }

                var currentDatas = GetCurrentDatas(queryable);
                if (currentDatas.IsNullOrEmpty())
                {
                    return affectRows;
                }

                if (!isSeparateVersioning)
                {
                    foreach (var currentData in currentDatas)
                    {
                        currentData.SetValue(versionFieldName, currentData.GetValue(versionFieldName) + 1);
                        currentData.SetValue(historyTagFieldName, false);
                    }
                }

                affectRows = RecordLatestVersionData(currentDatas, configure);

                if (!isSeparateVersioning)
                {
                    var identityQuery = SixnetConditionExtensions.IncludeEntities(null, currentDatas);
                    affectRows = repository.Update(fieldsAssignment, identityQuery, configure);
                }
                return affectRows;
            }
            else
            {
                return repository.Update(fieldsAssignment, queryable, configure);
            }
        }

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="fieldsAssignment"></param>
        /// <param name="ids"></param>
        /// <param name="configure"></param>
        protected int UpdateData(SixnetFieldsAssignment fieldsAssignment, IEnumerable<dynamic> ids, Action<SixnetDataOperationOptions> configure = null)
        {
            return UpdateData(fieldsAssignment, GetIdsQueryable(ids), configure);
        }

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="fieldsAssignmentExpression"></param>
        /// <param name="ids"></param>
        /// <param name="configure"></param>
        protected int UpdateData(Expression<Func<T, bool>> fieldsAssignmentExpression, IEnumerable<dynamic> ids, Action<SixnetDataOperationOptions> configure = null)
        {
            return UpdateData(SixnetExpressionHelper.GetFieldsAssignment(fieldsAssignmentExpression), ids, configure);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression"></param>
        /// <param name="id"></param>
        /// <param name="configure"></param>
        protected int UpdateData(Expression<Func<T, bool>> fieldsAssignmentExpression, dynamic id, Action<SixnetDataOperationOptions> configure = null)
        {
            return UpdateData(fieldsAssignmentExpression, [id], configure);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression"></param>
        /// <param name="condition"></param>
        /// <param name="configure"></param>
        protected int UpdateData(Expression<Func<T, bool>> fieldsAssignmentExpression, Expression<Func<T, bool>> condition, Action<SixnetDataOperationOptions> configure = null)
        {
            return UpdateData(SixnetExpressionHelper.GetFieldsAssignment(fieldsAssignmentExpression), SixnetQuerier.Create(condition), configure);
        }

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="fieldsAssignment"></param>
        /// <param name="queryable"></param>
        /// <param name="configure"></param>
        protected async Task<int> UpdateDataAsync(SixnetFieldsAssignment fieldsAssignment, ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null)
        {
            SixnetThrower.ThrowArgNullIf(fieldsAssignment == null);

            if (isVersioning)
            {
                var affectRows = 0;
                fieldsAssignment.RemoveNewValue(versionFieldName);
                fieldsAssignment.RemoveNewValue(historyTagFieldName);

                if (isSeparateVersioning)
                {
                    var addVersionValue = SixnetDataField.Create(versionFieldName, typeof(T), fieldFormatSetting: SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.ADD, SixnetConstantField.Create(1)));
                    fieldsAssignment.SetNewValue(versionFieldName, addVersionValue);
                    fieldsAssignment.SetNewValue(historyTagFieldName, SixnetConstantField.Create(false));

                    affectRows = await repository.UpdateAsync(fieldsAssignment, queryable, options =>
                    {
                        options.FromPrimaryTable();
                        configure?.Invoke(options);
                    }).ConfigureAwait(false);
                }

                var currentDatas = await GetCurrentDatasAsync(queryable).ConfigureAwait(false);
                if (currentDatas.IsNullOrEmpty())
                {
                    return affectRows;
                }

                if (!isSeparateVersioning)
                {
                    foreach (var currentData in currentDatas)
                    {
                        currentData.SetValue(versionFieldName, currentData.GetValue(versionFieldName) + 1);
                        currentData.SetValue(historyTagFieldName, false);
                    }
                }

                affectRows = await RecordLatestVersionDataAsync(currentDatas, configure).ConfigureAwait(false);

                if (!isSeparateVersioning)
                {
                    var identityQuery = SixnetConditionExtensions.IncludeEntities(null, currentDatas);
                    affectRows = await repository.UpdateAsync(fieldsAssignment, identityQuery, configure).ConfigureAwait(false);
                }
                return affectRows;
            }
            else
            {
                return await repository.UpdateAsync(fieldsAssignment, queryable, configure).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="fieldsAssignment"></param>
        /// <param name="ids"></param>
        /// <param name="configure"></param>
        protected Task<int> UpdateDataAsync(SixnetFieldsAssignment fieldsAssignment, IEnumerable<dynamic> ids, Action<SixnetDataOperationOptions> configure = null)
        {
            return UpdateDataAsync(fieldsAssignment, GetIdsQueryable(ids), configure);
        }

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="fieldsAssignmentExpression"></param>
        /// <param name="ids"></param>
        /// <param name="configure"></param>
        protected Task<int> UpdateDataAsync(Expression<Func<T, bool>> fieldsAssignmentExpression, IEnumerable<dynamic> ids, Action<SixnetDataOperationOptions> configure = null)
        {
            return UpdateDataAsync(SixnetExpressionHelper.GetFieldsAssignment(fieldsAssignmentExpression), ids, configure);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression"></param>
        /// <param name="id"></param>
        /// <param name="configure"></param>
        protected Task<int> UpdateDataAsync(Expression<Func<T, bool>> fieldsAssignmentExpression, dynamic id, Action<SixnetDataOperationOptions> configure = null)
        {
            return UpdateDataAsync(fieldsAssignmentExpression, [id], configure);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression"></param>
        /// <param name="condition"></param>
        /// <param name="configure"></param>
        protected Task<int> UpdateDataAsync(Expression<Func<T, bool>> fieldsAssignmentExpression, Expression<Func<T, bool>> condition, Action<SixnetDataOperationOptions> configure = null)
        {
            return UpdateDataAsync(SixnetExpressionHelper.GetFieldsAssignment(fieldsAssignmentExpression), SixnetQuerier.Create(condition), configure);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="queryable"></param>
        /// <param name="configure"></param>
        protected int DeleteData(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null)
        {
            var archiveField = SixnetEntityManager.GetFields<T>(SixnetFieldRole.Archive)?.FirstOrDefault();
            if (archiveField != null)
            {
                var fieldsAssignment = new SixnetFieldsAssignment();
                fieldsAssignment.SetNewValue(archiveField.PropertyName, true);

                return UpdateData(fieldsAssignment, queryable, configure);
            }
            else
            {
                return repository.Delete(queryable, options =>
                {
                    options.FromPrimaryTable();
                    configure?.Invoke(options);
                });
            }
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="configure"></param>
        protected int DeleteData(IEnumerable<dynamic> ids, Action<SixnetDataOperationOptions> configure = null)
        {
            return DeleteData(GetIdsQueryable(ids), configure);
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="id"></param>
        /// <param name="configure"></param>
        protected int DeleteData(dynamic id, Action<SixnetDataOperationOptions> configure = null)
        {
            return DeleteData([id], configure);
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="configure"></param>
        protected int DeleteData(Expression<Func<T, bool>> condition, Action<SixnetDataOperationOptions> configure = null)
        {
            return DeleteData(SixnetQuerier.Create(condition), configure);
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="queryable"></param>
        /// <param name="configure"></param>
        protected async Task<int> DeleteDataAsync(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null)
        {
            var archiveField = SixnetEntityManager.GetFields<T>(SixnetFieldRole.Archive)?.FirstOrDefault();
            if (archiveField != null)
            {
                var fieldsAssignment = new SixnetFieldsAssignment();
                fieldsAssignment.SetNewValue(archiveField.PropertyName, true);

                return await UpdateDataAsync(fieldsAssignment, queryable, configure).ConfigureAwait(false);
            }
            else
            {
                return await repository.DeleteAsync(queryable, options =>
                {
                    options.FromPrimaryTable();
                    configure?.Invoke(options);
                }).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="configure"></param>
        protected Task<int> DeleteDataAsync(IEnumerable<dynamic> ids, Action<SixnetDataOperationOptions> configure = null)
        {
            return DeleteDataAsync(GetIdsQueryable(ids), configure);
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="id"></param>
        /// <param name="configure"></param>
        protected Task<int> DeleteDataAsync(dynamic id, Action<SixnetDataOperationOptions> configure = null)
        {
            return DeleteDataAsync([id], configure);
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="configure"></param>
        protected Task<int> DeleteDataAsync(Expression<Func<T, bool>> condition, Action<SixnetDataOperationOptions> configure = null)
        {
            return DeleteDataAsync(SixnetQuerier.Create(condition), configure);
        }

        #endregion

        #region Get current data

        /// <summary>
        /// Get current data
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        protected virtual List<T> GetCurrentDatas(ISixnetQueryable queryable)
        {
            queryable ??= SixnetQuerier.Create<T>();
            if (isVersioning)
            {
                var historicalCriterion = GetHistoryTagCriterion(false);
                queryable.Where(historicalCriterion);
            }
            return repository.AsQueryable()
                .Where(queryable)
                .IncludeArchived()
                .ToList();
        }

        /// <summary>
        /// Get current data
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        protected List<T> GetCurrentDatas(IEnumerable<dynamic> ids)
        {
            return GetCurrentDatas(GetIdsQueryable(ids));
        }

        /// <summary>
        /// Get current data
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        protected virtual Task<List<T>> GetCurrentDatasAsync(ISixnetQueryable queryable)
        {
            queryable ??= SixnetQuerier.Create<T>();
            if (isVersioning)
            {
                var historicalCriterion = GetHistoryTagCriterion(false);
                queryable.Where(historicalCriterion);
            }
            return repository.AsQueryable()
                .Where(queryable)
                .IncludeArchived()
                .ToListAsync();
        }

        /// <summary>
        /// Get current data
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        protected Task<List<T>> GetCurrentDatasAsync(IEnumerable<dynamic> ids)
        {
            return GetCurrentDatasAsync(GetIdsQueryable(ids));
        }

        #endregion
    }
}
