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

namespace Sixnet.Development.Data.Database
{
    public abstract class SixnetBaseDatabaseUpdateRecord<T> : ISixnetDatabaseUpdateRecord where T : ISixnetDatabaseUpdateRecord
    {
        public abstract Version Version { get; set; }
        public abstract string Note { get; set; }
        protected ISixnetDataClient DataClient { get; set; }
        protected SixnetUpdateDatabaseContext Context { get; set; }
        public abstract long Id { get; set; }

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
                await recordRepository.AddAsync(new SixnetAppUpdateRecordEntity()
                {
                    Id = Id,
                    AppVersion = context.CurrentVersion.ToString(),
                    AppVersionId = context.CurrentVersion.VersionToLong(),
                    Note = Note
                }).ConfigureAwait(false);
                await workContext.CommitAsync().ConfigureAwait(false);
            }).ConfigureAwait(false);
        }

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

        public virtual int CompareTo(object other)
        {
            var otherRecord = other as ISixnetDatabaseUpdateRecord;
            return Id.CompareTo(otherRecord?.Id);
        }

        protected void ReportMessage(string message)
        {
            Context?.UpdateParameter?.ReportProcess?.Invoke(UpdateDatabaseProcess.Create(UpdateDatabaseProcessState.Message, Context.UpdateParameter
                , this, Context.CurrentVersion, Context.CurrentRecordId, null, message));
        }

        protected abstract Task ExecuteUpdateAsync();
        protected abstract Task ExecuteRollbackAsync();
    }
}
