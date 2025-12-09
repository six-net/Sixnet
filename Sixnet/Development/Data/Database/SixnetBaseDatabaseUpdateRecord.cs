// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sixnet.DependencyInjection;
using Sixnet.Development.Data.Client;
using Sixnet.Development.Entity;
using Sixnet.Development.Repository;
using Sixnet.Development.Work;
using Sixnet.Model.Paging;

namespace Sixnet.Development.Data.Database
{
    public abstract class SixnetBaseDatabaseUpdateRecord<T> : ISixnetDatabaseUpdateRecord where T : ISixnetDatabaseUpdateRecord
    {
        /// <summary>
        /// Application version
        /// </summary>
        public abstract Version Version { get; set; }

        /// <summary>
        /// Note
        /// </summary>
        public abstract string Note { get; set; }

        /// <summary>
        /// Data client
        /// </summary>
        protected ISixnetDataClient DataClient { get; set; }

        /// <summary>
        /// Update context
        /// </summary>
        protected SixnetUpdateDatabaseContext Context { get; set; }

        /// <summary>
        /// Record id
        /// </summary>
        public abstract long Id { get; set; }

        /// <summary>
        /// System user id
        /// </summary>
        public static long SystemUserId { get; set; }

        /// <summary>
        /// System user name
        /// </summary>
        public static string SystemUserName { get; set; }

        /// <summary>
        /// System user display name
        /// </summary>
        public static string SystemUserDisplayName { get; set; }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task UpdateAsync(SixnetUpdateDatabaseContext context)
        {
            Context = context;
            await UnitOfWork.ExecuteAsync(new List<DatabaseServer>() { context.UpdateParameter.DatabaseServer }, async workContext =>
            {
                DataClient = UnitOfWork.Current.DataClient;
                await ExecuteUpdateAsync().ConfigureAwait(false);
                if (Version > context.CurrentVersion)
                {
                    context.CurrentVersion = Version;
                }
                var recordRepository = SixnetContainer.GetService<ISixnetRepository<SixnetAppUpdateRecordEntity>>();
                var recordVersion = context.CurrentVersion >= Version ? context.CurrentVersion : Version;
                await recordRepository.AddAsync(new SixnetAppUpdateRecordEntity()
                {
                    Id = Id,
                    AppVersion = recordVersion.ToString(),
                    AppVersionId = recordVersion.VersionToLong(),
                    Note = Note
                }).ConfigureAwait(false);
                await workContext.CommitAsync().ConfigureAwait(false);
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Rollback
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task RollbackAsync(SixnetUpdateDatabaseContext context)
        {
            Context = context;
            await UnitOfWork.ExecuteAsync(new List<DatabaseServer>() { context.UpdateParameter.DatabaseServer }, async workContext =>
            {
                DataClient = UnitOfWork.Current.DataClient;
                await ExecuteRollbackAsync().ConfigureAwait(false);
                var recordRepository = SixnetContainer.GetService<ISixnetRepository<SixnetAppUpdateRecordEntity>>();
                await recordRepository.DeleteAsync(r => r.Id == Id).ConfigureAwait(false);
                await workContext.CommitAsync().ConfigureAwait(false);
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Compare to
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual int CompareTo(object other)
        {
            var otherRecord = other as ISixnetDatabaseUpdateRecord;
            return Id.CompareTo(otherRecord?.Id);
        }

        /// <summary>
        /// Report message
        /// </summary>
        /// <param name="message"></param>
        protected void ReportMessage(string message)
        {
            Context?.UpdateParameter?.ReportProcess?.Invoke(UpdateDatabaseProcess.Create(UpdateDatabaseProcessState.Message, Context.UpdateParameter
                , this, Context.CurrentVersion, Context.CurrentRecordId, null, message));
        }

        /// <summary>
        /// Get entity instance
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="configure"></param>
        /// <returns></returns>
        protected virtual TEntity GetEntityInstance<TEntity>(Action<TEntity> configure = null) where TEntity : class, ISixnetEntity<TEntity>, new()
        {
            var instance = new TEntity();
            if (instance is CreateDateEntity<TEntity> createDateEntity)
            {
                createDateEntity.CreateUserId = SystemUserId;
                createDateEntity.CreateUserName = SystemUserName;
                createDateEntity.CreateUserDisplayName = SystemUserDisplayName;
                createDateEntity.CreateDate = DateTimeOffset.Now;
            }
            if (instance is CreateUpdateDateEntity<TEntity> updateDateEntity)
            {
                updateDateEntity.UpdateUserId = SystemUserId;
                updateDateEntity.UpdateUserName = SystemUserName;
                updateDateEntity.UpdateUserDisplayName = SystemUserDisplayName;
                updateDateEntity.UpdateDate = DateTimeOffset.Now;
            }
            configure?.Invoke(instance);
            return instance;
        }

        /// <summary>
        /// Insert entity data
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="configure"></param>
        /// <returns></returns>
        protected virtual async Task<TEntity> InsertAsync<TEntity>(Action<TEntity> configure = null) where TEntity : class, ISixnetEntity<TEntity>, new()
        {
            var instance = GetEntityInstance<TEntity>();
            var repository = SixnetContainer.GetRepository<TEntity>();
            await repository.AddAsync(instance).ConfigureAwait(false);
            return instance;
        }

        /// <summary>
        /// Delete data
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="conditionExpression"></param>
        /// <param name="isIncludeArchived"></param>
        /// <returns></returns>
        protected virtual async Task DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> conditionExpression, bool isIncludeArchived = true)
        {
            var repository = SixnetContainer.GetRepository<TEntity>();
            var deleteQueryable = repository.AsQueryable(conditionExpression);
            if (!isIncludeArchived)
            {
                deleteQueryable = deleteQueryable.IncludeArchived();
            }
            await deleteQueryable.DeleteAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Update data
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="fieldsAssignmentExpression"></param>
        /// <param name="conditionExpression"></param>
        /// <returns></returns>
        protected virtual async Task UpdateAsync<TEntity>(Expression<Func<TEntity, bool>> fieldsAssignmentExpression, Expression<Func<TEntity, bool>> conditionExpression, bool isIncludeArchived = true)
        {
            var repository = SixnetContainer.GetRepository<TEntity>();
            var updateQueryable = repository.AsQueryable(conditionExpression);
            if (isIncludeArchived)
            {
                updateQueryable = updateQueryable.IncludeArchived();
            }
            await updateQueryable.UpdateAsync(fieldsAssignmentExpression).ConfigureAwait(false);
        }

        /// <summary>
        /// Get data list
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="conditionExpression"></param>
        /// <param name="isIncludeArchived"></param>
        /// <returns></returns>
        protected virtual async Task<List<TEntity>> GetListAsync<TEntity>(Expression<Func<TEntity, bool>> conditionExpression, bool isIncludeArchived = true)
        {
            var repository = SixnetContainer.GetRepository<TEntity>();
            var queryable = repository.AsQueryable(conditionExpression);
            if (isIncludeArchived)
            {
                queryable = queryable.IncludeArchived();
            }
            return await queryable.ToListAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Get paging
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="conditionExpression"></param>
        /// <param name="isIncludeArchived"></param>
        /// <returns></returns>
        protected virtual async Task<PagingInfo<TEntity>> GetPagingAsync<TEntity>(int page, int pageSize, Expression<Func<TEntity, bool>> conditionExpression, bool isIncludeArchived = true)
        {
            var repository = SixnetContainer.GetRepository<TEntity>();
            var queryable = repository.AsQueryable(conditionExpression);
            if (isIncludeArchived)
            {
                queryable = queryable.IncludeArchived();
            }
            return await queryable.ToPagingAsync(page, pageSize).ConfigureAwait(false);
        }

        /// <summary>
        /// Execute update
        /// </summary>
        /// <returns></returns>
        protected abstract Task ExecuteUpdateAsync();

        /// <summary>
        /// Execute rollback
        /// </summary>
        /// <returns></returns>
        protected abstract Task ExecuteRollbackAsync();
    }
}
