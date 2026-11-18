// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sixnet.DependencyInjection;
using Sixnet.Development.Data;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Data.Field.Formatting;
using Sixnet.Development.Domain.Model;
using Sixnet.Development.Entity;
using Sixnet.Development.Events.Data;
using Sixnet.Development.Queryable;
using Sixnet.Development.Repository;
using Sixnet.Exceptions;
using Sixnet.Expressions.Linq;

namespace Sixnet.Development.Domain.Service
{
    public abstract class SixnetBaseCommandService<T> : SixnetBaseQueryService<T> where T : SixnetBaseEntity<T>, new()
    {
        #region Identity queryable

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

        #endregion

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
        protected T SaveData(SixnetSaveDataInfo<T> parameter, Action<SixnetDataOperationOptions> configure = null)
        {
            var newData = parameter?.Data?.Clone();
            SixnetThrower.ThrowArgNullIf(newData == null, nameof(SixnetSaveDataInfo<T>.Data));

            var isAdd = newData.IdentityValueIsNull();
            var saveContext = new SixnetSaveDataContext<T>()
            {
                NewData = newData,
                OldData = null,
                IncludeArchived = parameter.IncludeArchived,
                IsUpdate = !isAdd,
                SaveInfo = parameter
            };

            if (isAdd)
            {
                SaveDataBefore(saveContext, configure);
                newData = saveContext.NewData;

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
                var identityQueryable = HandleQueryable(GetIdentityQueryable([newData]), false, null, parameter.IncludeArchived);
                var currentData = GetList(identityQueryable, configure)?.FirstOrDefault();
                SixnetThrower.ThrowGetDataFailedIf<T>(currentData == null);

                saveContext.OldData = currentData.Clone();
                SaveDataBefore(saveContext, configure);
                newData = saveContext.NewData;

                if (isVersioning)
                {
                    newData.SetValue(versionFieldName, currentData.GetValue(versionFieldName) + 1);
                    newData.SetValue(historyTagFieldName, false);

                    if (isSeparateVersioning)
                    {
                        var updateAssignment = currentData.UpdateFrom(newData, saveContext.UpdateConfigure);
                        repository.Update(updateAssignment, options =>
                        {
                            options.FromPrimaryTable();
                            configure?.Invoke(options);
                        });
                    }

                    RecordLatestVersionData([currentData], configure);
                }
                else
                {
                    var updateAssignment = currentData.UpdateFrom(newData, saveContext.UpdateConfigure);
                    repository.Update(updateAssignment, configure);
                }

                saveContext.NewData = newData = currentData;
            }

            // save data after
            SaveDataAfter(saveContext, configure);

            return newData;
        }

        /// <summary>
        /// Save data
        /// </summary>
        /// <param name="newData"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        protected T SaveData(T newData, Action<SixnetDataOperationOptions> configure = null)
        {
            return SaveData(new SixnetSaveDataInfo<T>() { Data = newData }, configure);
        }

        /// <summary>
        /// Save datas
        /// </summary>
        /// <param name="newDatas"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        protected void SaveDatas(IEnumerable<T> newDatas, Action<SixnetDataOperationOptions> configure = null)
        {
            if (newDatas.IsNullOrEmpty())
            {
                return;
            }
            foreach (var newData in newDatas)
            {
                SaveData(new SixnetSaveDataInfo<T>() { Data = newData }, configure);
            }
        }

        /// <summary>
        /// Save data
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        protected async Task<T> SaveDataAsync(SixnetSaveDataInfo<T> parameter, Action<SixnetDataOperationOptions> configure = null)
        {
            var newData = parameter?.Data?.Clone();
            SixnetThrower.ThrowArgNullIf(newData == null, nameof(SixnetSaveDataInfo<T>.Data));

            // save before
            var isAdd = newData.IdentityValueIsNull();
            var saveContext = new SixnetSaveDataContext<T>()
            {
                NewData = newData,
                OldData = null,
                IncludeArchived = parameter.IncludeArchived,
                IsUpdate = !isAdd,
                SaveInfo = parameter
            };

            if (isAdd)
            {
                await SaveDataBeforeAsync(saveContext, configure).ConfigureAwait(false);
                newData = saveContext.NewData;

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
                var identityQueryable = HandleQueryable(GetIdentityQueryable([newData]), false, null, parameter.IncludeArchived);
                var currentData = (await GetListAsync(identityQueryable, configure).ConfigureAwait(false))?.FirstOrDefault();
                SixnetThrower.ThrowGetDataFailedIf<T>(currentData == null);

                saveContext.OldData = currentData.Clone();
                await SaveDataBeforeAsync(saveContext, configure).ConfigureAwait(false);
                newData = saveContext.NewData;

                if (isVersioning)
                {
                    newData.SetValue(versionFieldName, currentData.GetValue(versionFieldName) + 1);
                    newData.SetValue(historyTagFieldName, false);

                    if (isSeparateVersioning)
                    {
                        var updateAssignment = await currentData.UpdateFromAsync(newData, saveContext.UpdateConfigure).ConfigureAwait(false);
                        await repository.UpdateAsync(updateAssignment, options =>
                        {
                            options.FromPrimaryTable();
                            configure?.Invoke(options);
                        }).ConfigureAwait(false);
                    }

                    await RecordLatestVersionDataAsync([currentData], configure).ConfigureAwait(false);
                }
                else
                {
                    var updateAssignment = await currentData.UpdateFromAsync(newData, saveContext.UpdateConfigure).ConfigureAwait(false);
                    await repository.UpdateAsync(updateAssignment, configure).ConfigureAwait(false);
                }

                saveContext.NewData = newData = currentData;
            }

            // save data after
            await SaveDataAfterAsync(saveContext, configure).ConfigureAwait(false);

            return newData;
        }

        /// <summary>
        /// Save data
        /// </summary>
        /// <param name="newData"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        protected Task<T> SaveDataAsync(T newData, Action<SixnetDataOperationOptions> configure = null)
        {
            return SaveDataAsync(new SixnetSaveDataInfo<T>() { Data = newData }, configure);
        }

        /// <summary>
        /// Save datas
        /// </summary>
        /// <param name="newDatas"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        protected async Task SaveDatasAsync(IEnumerable<T> newDatas, Action<SixnetDataOperationOptions> configure = null)
        {
            if (newDatas.IsNullOrEmpty())
            {
                return;
            }
            foreach (var newData in newDatas)
            {
                await SaveDataAsync(new SixnetSaveDataInfo<T>() { Data = newData }, configure).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Save data before
        /// </summary>
        /// <param name="context"></param>
        /// <param name="configure"></param>
        protected virtual void SaveDataBefore(SixnetSaveDataContext<T> context, Action<SixnetDataOperationOptions> configure = null)
        {
        }

        /// <summary>
        /// Save data before
        /// </summary>
        /// <param name="context"></param>
        /// <param name="configure"></param>
        protected virtual Task SaveDataBeforeAsync(SixnetSaveDataContext<T> context, Action<SixnetDataOperationOptions> configure = null)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Save data after
        /// </summary>
        /// <param name="context"></param>
        /// <param name="configure"></param>
        protected virtual void SaveDataAfter(SixnetSaveDataContext<T> context, Action<SixnetDataOperationOptions> configure = null)
        {
        }

        /// <summary>
        /// Save data after
        /// </summary>
        /// <param name="context"></param>
        /// <param name="configure"></param>
        protected virtual Task SaveDataAfterAsync(SixnetSaveDataContext<T> context, Action<SixnetDataOperationOptions> configure = null)
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

                var currentDatas = GetList(HandleQueryable(queryable, false, null, false));
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
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment"></param>
        /// <param name="condition"></param>
        /// <param name="configure"></param>
        protected int UpdateData(SixnetFieldsAssignment fieldsAssignment, Expression<Func<T, bool>> condition, Action<SixnetDataOperationOptions> configure = null)
        {
            return UpdateData(fieldsAssignment, SixnetQuerier.Create(condition), configure);
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

                var currentDatas = await GetListAsync(HandleQueryable(queryable, false, null, false)).ConfigureAwait(false);
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
        /// Update
        /// </summary>
        /// <param name="fieldsAssignment"></param>
        /// <param name="condition"></param>
        /// <param name="configure"></param>
        protected Task<int> UpdateDataAsync(SixnetFieldsAssignment fieldsAssignment, Expression<Func<T, bool>> condition, Action<SixnetDataOperationOptions> configure = null)
        {
            return UpdateDataAsync(fieldsAssignment, SixnetQuerier.Create(condition), configure);
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
    }
}
