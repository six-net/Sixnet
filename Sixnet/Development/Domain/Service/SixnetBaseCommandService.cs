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
    public class SixnetBaseCommandService<TEntity> : SixnetBaseDomainService<TEntity>, ISixnetCommandService<TEntity> where TEntity : SixnetBaseEntity<TEntity>, new()
    {
        #region Identity queryable

        /// <summary>
        /// Get identity queryable 
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        ISixnetQueryable GetIdentityQueryable(IEnumerable<TEntity> datas, bool withoutVersion = true)
        {
            if (datas.IsNullOrEmpty())
            {
                return SixnetQuerier.Create<TEntity>(d => 0 == 1);
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

        int RecordLatestVersionData(IEnumerable<TEntity> datas, Action<SixnetDataOperationOptions> configure = null)
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

        async Task<int> RecordLatestVersionDataAsync(IEnumerable<TEntity> datas, Action<SixnetDataOperationOptions> configure = null)
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
        /// <param name="saveInfo"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public TEntity Save(SixnetSaveDataInfo<TEntity> saveInfo, Action<SixnetDataOperationOptions> configure = null)
        {
            var newData = saveInfo?.Data?.Clone();
            SixnetThrower.ThrowArgNullIf(newData == null, nameof(SixnetSaveDataInfo<TEntity>.Data));

            var isAdd = newData.IdentityValueIsNull();
            var saveContext = new SixnetSaveDataContext<TEntity>()
            {
                NewData = newData,
                OldData = null,
                IncludeArchived = saveInfo.IncludeArchived,
                IsUpdate = !isAdd,
                SaveInfo = saveInfo
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
                var identityQueryable = HandleQueryable(GetIdentityQueryable([newData]), false, null, saveInfo.IncludeArchived);
                var currentData = repository.Get(identityQueryable, configure);
                SixnetThrower.ThrowGetDataFailedIf<TEntity>(currentData == null);

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
        public TEntity Save(TEntity newData, Action<SixnetDataOperationOptions> configure = null)
        {
            return Save(new SixnetSaveDataInfo<TEntity>() { Data = newData }, configure);
        }

        /// <summary>
        /// Save datas
        /// </summary>
        /// <param name="newDatas"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public List<TEntity> Save(IEnumerable<TEntity> newDatas, Action<SixnetDataOperationOptions> configure = null)
        {
            if (newDatas.IsNullOrEmpty())
            {
                return new List<TEntity>(0);
            }
            var savedDatas = new List<TEntity>();
            foreach (var newData in newDatas)
            {
                savedDatas.Add(Save(new SixnetSaveDataInfo<TEntity>() { Data = newData }, configure));
            }
            return savedDatas;
        }

        /// <summary>
        /// Save data
        /// </summary>
        /// <param name="saveInfo"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public async Task<TEntity> SaveAsync(SixnetSaveDataInfo<TEntity> saveInfo, Action<SixnetDataOperationOptions> configure = null)
        {
            var newData = saveInfo?.Data?.Clone();
            SixnetThrower.ThrowArgNullIf(newData == null, nameof(SixnetSaveDataInfo<TEntity>.Data));

            // save before
            var isAdd = newData.IdentityValueIsNull();
            var saveContext = new SixnetSaveDataContext<TEntity>()
            {
                NewData = newData,
                OldData = null,
                IncludeArchived = saveInfo.IncludeArchived,
                IsUpdate = !isAdd,
                SaveInfo = saveInfo
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
                var identityQueryable = HandleQueryable(GetIdentityQueryable([newData]), false, null, saveInfo.IncludeArchived);
                var currentData = await repository.GetAsync(identityQueryable, configure).ConfigureAwait(false);
                SixnetThrower.ThrowGetDataFailedIf<TEntity>(currentData == null);

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
        public Task<TEntity> SaveAsync(TEntity newData, Action<SixnetDataOperationOptions> configure = null)
        {
            return SaveAsync(new SixnetSaveDataInfo<TEntity>() { Data = newData }, configure);
        }

        /// <summary>
        /// Save datas
        /// </summary>
        /// <param name="newDatas"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public async Task<List<TEntity>> SaveAsync(IEnumerable<TEntity> newDatas, Action<SixnetDataOperationOptions> configure = null)
        {
            if (newDatas.IsNullOrEmpty())
            {
                return new List<TEntity>(0);
            }
            var savedDatas = new List<TEntity>();
            foreach (var newData in newDatas)
            {
                savedDatas.Add(await SaveAsync(new SixnetSaveDataInfo<TEntity>() { Data = newData }, configure).ConfigureAwait(false));
            }
            return savedDatas;
        }

        /// <summary>
        /// Save data before
        /// </summary>
        /// <param name="context"></param>
        /// <param name="configure"></param>
        protected virtual void SaveDataBefore(SixnetSaveDataContext<TEntity> context, Action<SixnetDataOperationOptions> configure = null)
        {
        }

        /// <summary>
        /// Save data before
        /// </summary>
        /// <param name="context"></param>
        /// <param name="configure"></param>
        protected virtual Task SaveDataBeforeAsync(SixnetSaveDataContext<TEntity> context, Action<SixnetDataOperationOptions> configure = null)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Save data after
        /// </summary>
        /// <param name="context"></param>
        /// <param name="configure"></param>
        protected virtual void SaveDataAfter(SixnetSaveDataContext<TEntity> context, Action<SixnetDataOperationOptions> configure = null)
        {
        }

        /// <summary>
        /// Save data after
        /// </summary>
        /// <param name="context"></param>
        /// <param name="configure"></param>
        protected virtual Task SaveDataAfterAsync(SixnetSaveDataContext<TEntity> context, Action<SixnetDataOperationOptions> configure = null)
        {
            return Task.CompletedTask;
        }

        #endregion

        #region Delete data

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="configure"></param>
        public virtual int Delete(IEnumerable<dynamic> ids, Action<SixnetDataOperationOptions> configure)
        {
            if (ids.IsNullOrEmpty())
            {
                return 0;
            }
            return repository.Delete(GetIdsQueryable(ids), configure);
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="configure"></param>
        public virtual int Delete(IEnumerable<dynamic> ids)
        {
            if (ids.IsNullOrEmpty())
            {
                return 0;
            }
            return repository.Delete(GetIdsQueryable(ids));
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <param name="configure"></param>
        public virtual int Delete(dynamic id, Action<SixnetDataOperationOptions> configure)
        {
            return Delete([id], configure);
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <param name="configure"></param>
        public virtual int Delete(dynamic id)
        {
            return Delete([id]);
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="configure"></param>
        public virtual async Task<int> DeleteAsync(IEnumerable<dynamic> ids, Action<SixnetDataOperationOptions> configure)
        {
            if (ids.IsNullOrEmpty())
            {
                return 0;
            }
            return await repository.DeleteAsync(GetIdsQueryable(ids), configure).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="ids"></param>
        public virtual async Task<int> DeleteAsync(IEnumerable<dynamic> ids)
        {
            if (ids.IsNullOrEmpty())
            {
                return 0;
            }
            return await repository.DeleteAsync(GetIdsQueryable(ids)).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <param name="configure"></param>
        public virtual Task<int> DeleteAsync(dynamic id, Action<SixnetDataOperationOptions> configure)
        {
            return DeleteAsync([id], configure);
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        public virtual Task<int> DeleteAsync(dynamic id)
        {
            return DeleteAsync([id]);
        }

        #endregion
    }
}
