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

        void RecordLatestVersionData(IEnumerable<T> datas, Action<SixnetDataOperationOptions> configure = null)
        {
            if (datas.IsNullOrEmpty())
            {
                return;
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
                    options.ForVersionTable();
                    configure?.Invoke(options);
                });

                repository.Add(datas, options =>
                {
                    options.ForVersionTable();
                    options.EnableInsertIncrementField();
                    configure?.Invoke(options);
                });
            }
            else
            {
                repository.Update(fieldsAssignment, condition, configure);
                repository.Add(datas, configure);
            }
        }

        async Task RecordLatestVersionDataAsync(IEnumerable<T> datas, Action<SixnetDataOperationOptions> configure = null)
        {
            if (datas.IsNullOrEmpty())
            {
                return;
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
                    options.ForVersionTable();
                    configure?.Invoke(options);
                }).ConfigureAwait(false);

                await repository.AddAsync(datas, options =>
                {
                    options.ForVersionTable();
                    options.EnableInsertIncrementField();
                    configure?.Invoke(options);
                }).ConfigureAwait(false);
            }
            else
            {
                await repository.UpdateAsync(fieldsAssignment, condition, configure).ConfigureAwait(false);
                await repository.AddAsync(datas, configure).ConfigureAwait(false);
            }
        }

        #endregion

        #region Save data

        /// <summary>
        /// Save data
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected T SaveData(SaveDataParameter<T> parameter, Action<SixnetDataOperationOptions> configure = null)
        {
            var newData = parameter?.NewData;
            SixnetThrower.ThrowArgNullIf(newData == null, nameof(parameter));

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
                    options.ForPrimaryTable();
                    configure?.Invoke(options);
                });

                if (isSeparateVersioning)
                {
                    repository.Add(newData, options =>
                    {
                        options.ForVersionTable();
                        options.EnableInsertIncrementField();
                        configure?.Invoke(options);
                    });
                }

                return newData;
            }
            else
            {
                var currentData = GetCurrentDatas(GetIdentityQueryable([newData]))?.FirstOrDefault();
                SixnetThrower.ThrowGetDataFailedIf<T>(currentData == null);

                HandleUpdating(currentData, parameter);

                if (isVersioning)
                {
                    currentData.SetValue(versionFieldName, currentData.GetValue(versionFieldName) + 1);
                    currentData.SetValue(historyTagFieldName, false);

                    if (isSeparateVersioning)
                    {
                        repository.Update(currentData, options =>
                        {
                            options.ForPrimaryTable();
                            configure?.Invoke(options);
                        });
                    }

                    RecordLatestVersionData([currentData], configure);
                }
                else
                {
                    repository.Update(currentData, configure);
                }

                return currentData;
            }
        }

        /// <summary>
        /// Save data
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected async Task<T> SaveDataAsync(SaveDataParameter<T> parameter, Action<SixnetDataOperationOptions> configure = null)
        {
            var newData = parameter?.NewData;
            SixnetThrower.ThrowArgNullIf(newData == null, nameof(parameter));

            if (newData.IdentityValueIsNull())
            {
                HandleAdding(parameter);

                if (isVersioning)
                {
                    newData.SetValue(versionFieldName, 1);
                    newData.SetValue(historyTagFieldName, false);
                }

                await repository.AddAsync(newData, options =>
                {
                    options.ForPrimaryTable();
                    configure?.Invoke(options);
                }).ConfigureAwait(false);

                if (isSeparateVersioning)
                {
                    await repository.AddAsync(newData, options =>
                    {
                        options.ForVersionTable();
                        options.EnableInsertIncrementField();
                        configure?.Invoke(options);
                    }).ConfigureAwait(false);
                }

                return newData;
            }
            else
            {
                var currentData = (await GetCurrentDatasAsync(GetIdentityQueryable([newData])).ConfigureAwait(false))?.FirstOrDefault();
                SixnetThrower.ThrowGetDataFailedIf<T>(currentData == null);

                HandleUpdating(currentData, parameter);

                if (isVersioning)
                {
                    currentData.SetValue(versionFieldName, currentData.GetValue(versionFieldName) + 1);
                    currentData.SetValue(historyTagFieldName, false);

                    if (isSeparateVersioning)
                    {
                        await repository.UpdateAsync(currentData, options =>
                        {
                            options.ForPrimaryTable();
                            configure?.Invoke(options);
                        }).ConfigureAwait(false);
                    }

                    await RecordLatestVersionDataAsync([currentData], configure).ConfigureAwait(false);
                }
                else
                {
                    await repository.UpdateAsync(currentData, configure).ConfigureAwait(false);
                }

                return currentData;
            }
        }

        #endregion

        #region Update

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="fieldsAssignment"></param>
        /// <param name="queryable"></param>
        /// <param name="configure"></param>
        protected void UpdateData(SixnetFieldsAssignment fieldsAssignment, ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null)
        {
            SixnetThrower.ThrowArgNullIf(fieldsAssignment == null);

            if (isVersioning)
            {
                fieldsAssignment.RemoveNewValue(versionFieldName);
                fieldsAssignment.RemoveNewValue(historyTagFieldName);

                if (isSeparateVersioning)
                {
                    var addVersionValue = SixnetDataField.Create(versionFieldName, typeof(T), fieldFormatSetting: SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.ADD, SixnetConstantField.Create(1)));
                    fieldsAssignment.SetNewValue(versionFieldName, addVersionValue);
                    fieldsAssignment.SetNewValue(historyTagFieldName, SixnetConstantField.Create(false));

                    repository.Update(fieldsAssignment, queryable, options =>
                    {
                        options.ForPrimaryTable();
                        configure?.Invoke(options);
                    });
                }

                var currentDatas = GetCurrentDatas(queryable);
                if (currentDatas.IsNullOrEmpty())
                {
                    return;
                }

                if (!isSeparateVersioning)
                {
                    foreach (var currentData in currentDatas)
                    {
                        currentData.SetValue(versionFieldName, currentData.GetValue(versionFieldName) + 1);
                        currentData.SetValue(historyTagFieldName, false);
                    }
                }

                RecordLatestVersionData(currentDatas, configure);

                if (!isSeparateVersioning)
                {
                    var identityQuery = SixnetConditionExtensions.IncludeEntities(null, currentDatas);
                    repository.Update(fieldsAssignment, identityQuery, configure);
                }
            }
            else
            {
                repository.Update(fieldsAssignment, queryable, configure);
            }
        }

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="fieldsAssignment"></param>
        /// <param name="ids"></param>
        /// <param name="configure"></param>
        protected void UpdateData(SixnetFieldsAssignment fieldsAssignment, IEnumerable<dynamic> ids, Action<SixnetDataOperationOptions> configure = null)
        {
            UpdateData(fieldsAssignment, GetIdsQueryable(ids), configure);
        }

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="fieldsAssignmentExpression"></param>
        /// <param name="ids"></param>
        /// <param name="configure"></param>
        protected void UpdateData(Expression<Func<T, bool>> fieldsAssignmentExpression, IEnumerable<dynamic> ids, Action<SixnetDataOperationOptions> configure = null)
        {
            UpdateData(SixnetExpressionHelper.GetFieldsAssignment(fieldsAssignmentExpression), ids, configure);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression"></param>
        /// <param name="id"></param>
        /// <param name="configure"></param>
        protected void UpdateData(Expression<Func<T, bool>> fieldsAssignmentExpression, dynamic id, Action<SixnetDataOperationOptions> configure = null)
        {
            UpdateData(fieldsAssignmentExpression, [id], configure);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression"></param>
        /// <param name="condition"></param>
        /// <param name="configure"></param>
        protected void UpdateData(Expression<Func<T, bool>> fieldsAssignmentExpression, Expression<Func<T, bool>> condition, Action<SixnetDataOperationOptions> configure = null)
        {
            UpdateData(SixnetExpressionHelper.GetFieldsAssignment(fieldsAssignmentExpression), SixnetQuerier.Create(condition), configure);
        }

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="fieldsAssignment"></param>
        /// <param name="queryable"></param>
        /// <param name="configure"></param>
        protected async Task UpdateDataAsync(SixnetFieldsAssignment fieldsAssignment, ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null)
        {
            SixnetThrower.ThrowArgNullIf(fieldsAssignment == null);

            if (isVersioning)
            {
                fieldsAssignment.RemoveNewValue(versionFieldName);
                fieldsAssignment.RemoveNewValue(historyTagFieldName);

                if (isSeparateVersioning)
                {
                    var addVersionValue = SixnetDataField.Create(versionFieldName, typeof(T), fieldFormatSetting: SixnetFieldFormatSetting.Create(SixnetFieldFormatterNames.ADD, SixnetConstantField.Create(1)));
                    fieldsAssignment.SetNewValue(versionFieldName, addVersionValue);
                    fieldsAssignment.SetNewValue(historyTagFieldName, SixnetConstantField.Create(false));

                    await repository.UpdateAsync(fieldsAssignment, queryable, options =>
                    {
                        options.ForPrimaryTable();
                        configure?.Invoke(options);
                    }).ConfigureAwait(false);
                }

                var currentDatas = await GetCurrentDatasAsync(queryable).ConfigureAwait(false);
                if (currentDatas.IsNullOrEmpty())
                {
                    return;
                }

                if (!isSeparateVersioning)
                {
                    foreach (var currentData in currentDatas)
                    {
                        currentData.SetValue(versionFieldName, currentData.GetValue(versionFieldName) + 1);
                        currentData.SetValue(historyTagFieldName, false);
                    }
                }

                await RecordLatestVersionDataAsync(currentDatas, configure).ConfigureAwait(false);

                if (!isSeparateVersioning)
                {
                    var identityQuery = SixnetConditionExtensions.IncludeEntities(null, currentDatas);
                    await repository.UpdateAsync(fieldsAssignment, identityQuery, configure).ConfigureAwait(false);
                }
            }
            else
            {
                await repository.UpdateAsync(fieldsAssignment, queryable, configure).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="fieldsAssignment"></param>
        /// <param name="ids"></param>
        /// <param name="configure"></param>
        protected Task UpdateDataAsync(SixnetFieldsAssignment fieldsAssignment, IEnumerable<dynamic> ids, Action<SixnetDataOperationOptions> configure = null)
        {
            return UpdateDataAsync(fieldsAssignment, GetIdsQueryable(ids), configure);
        }

        /// <summary>
        /// Update data
        /// </summary>
        /// <param name="fieldsAssignmentExpression"></param>
        /// <param name="ids"></param>
        /// <param name="configure"></param>
        protected Task UpdateDataAsync(Expression<Func<T, bool>> fieldsAssignmentExpression, IEnumerable<dynamic> ids, Action<SixnetDataOperationOptions> configure = null)
        {
            return UpdateDataAsync(SixnetExpressionHelper.GetFieldsAssignment(fieldsAssignmentExpression), ids, configure);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression"></param>
        /// <param name="id"></param>
        /// <param name="configure"></param>
        protected Task UpdateDataAsync(Expression<Func<T, bool>> fieldsAssignmentExpression, dynamic id, Action<SixnetDataOperationOptions> configure = null)
        {
            return UpdateDataAsync(fieldsAssignmentExpression, [id], configure);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="fieldsAssignmentExpression"></param>
        /// <param name="condition"></param>
        /// <param name="configure"></param>
        protected Task UpdateDataAsync(Expression<Func<T, bool>> fieldsAssignmentExpression, Expression<Func<T, bool>> condition, Action<SixnetDataOperationOptions> configure = null)
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
        protected void DeleteData(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null)
        {
            var archiveField = SixnetEntityManager.GetFields<T>(SixnetFieldRole.Archive)?.FirstOrDefault();
            if (archiveField != null)
            {
                var fieldsAssignment = new SixnetFieldsAssignment();
                fieldsAssignment.SetNewValue(archiveField.PropertyName, true);

                UpdateData(fieldsAssignment, queryable, configure);
            }
            else
            {
                repository.Delete(queryable, options =>
                {
                    options.ForPrimaryTable();
                    configure?.Invoke(options);
                });
            }
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="configure"></param>
        protected void DeleteData(IEnumerable<dynamic> ids, Action<SixnetDataOperationOptions> configure = null)
        {
            DeleteData(GetIdsQueryable(ids), configure);
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="id"></param>
        /// <param name="configure"></param>
        protected void DeleteData(dynamic id, Action<SixnetDataOperationOptions> configure = null)
        {
            DeleteData([id], configure);
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="configure"></param>
        protected void DeleteData(Expression<Func<T, bool>> condition, Action<SixnetDataOperationOptions> configure = null)
        {
            DeleteData(SixnetQuerier.Create(condition), configure);
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="queryable"></param>
        /// <param name="configure"></param>
        protected async Task DeleteDataAsync(ISixnetQueryable queryable, Action<SixnetDataOperationOptions> configure = null)
        {
            var archiveField = SixnetEntityManager.GetFields<T>(SixnetFieldRole.Archive)?.FirstOrDefault();
            if (archiveField != null)
            {
                var fieldsAssignment = new SixnetFieldsAssignment();
                fieldsAssignment.SetNewValue(archiveField.PropertyName, true);

                await UpdateDataAsync(fieldsAssignment, queryable, configure).ConfigureAwait(false);
            }
            else
            {
                await repository.DeleteAsync(queryable, options =>
                {
                    options.ForPrimaryTable();
                    configure?.Invoke(options);
                }).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="configure"></param>
        protected Task DeleteDataAsync(IEnumerable<dynamic> ids, Action<SixnetDataOperationOptions> configure = null)
        {
            return DeleteDataAsync(GetIdsQueryable(ids), configure);
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="id"></param>
        /// <param name="configure"></param>
        protected Task DeleteDataAsync(dynamic id, Action<SixnetDataOperationOptions> configure = null)
        {
            return DeleteDataAsync([id], configure);
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="configure"></param>
        protected Task DeleteDataAsync(Expression<Func<T, bool>> condition, Action<SixnetDataOperationOptions> configure = null)
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

        #region Handle data

        /// <summary>
        /// Handle adding
        /// </summary>
        /// <param name="newData"></param>
        protected virtual void HandleAdding(SaveDataParameter<T> parameter)
        {
        }

        /// <summary>
        /// Handle updating
        /// </summary>
        /// <param name="currentData"></param>
        /// <param name="newData"></param>
        protected virtual void HandleUpdating(T currentData, SaveDataParameter<T> parameter)
        {
            currentData.ModifyFrom(parameter.NewData);
        }

        #endregion
    }
}
