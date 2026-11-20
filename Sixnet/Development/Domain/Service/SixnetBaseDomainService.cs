// "Company © 2025. All rights reserved."

using Sixnet.DependencyInjection;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Entity;
using Sixnet.Development.Queryable;
using Sixnet.Development.Repository;

namespace Sixnet.Development.Domain.Service
{
    public abstract class SixnetBaseDomainService<TEntity> where TEntity : SixnetBaseEntity<TEntity>, new()
    {
        protected readonly ISixnetRepository<TEntity> repository = SixnetContainer.GetRepository<TEntity>();

        protected readonly bool isSeparateVersioning = false;

        protected readonly bool isVersioning = false;

        protected readonly string versionFieldName = string.Empty;

        protected readonly string historyTagFieldName = string.Empty;

        protected readonly string defaultIdFieldName = string.Empty;

        public SixnetBaseDomainService()
        {
            var entityConfig = SixnetEntityManager.GetEntityConfig<TEntity>();
            isSeparateVersioning = entityConfig.IsSeparateVersioning;
            isVersioning = entityConfig.IsVersioning;

            // version
            if (entityConfig.RoleFields.TryGetValue(SixnetFieldRole.Version, out var versionFields))
            {
                versionFieldName = versionFields?.First()?.PropertyName;
            }

            // history tag
            if (entityConfig.RoleFields.TryGetValue(SixnetFieldRole.HistoryTag, out var historyTagFields))
            {
                historyTagFieldName = historyTagFields?.First()?.PropertyName;
            }

            // default id
            if (entityConfig.RoleFields.TryGetValue(SixnetFieldRole.DefaultId, out var defaultIdFields))
            {
                defaultIdFieldName = defaultIdFields?.First()?.PropertyName;
            }
            if (string.IsNullOrWhiteSpace(defaultIdFieldName) && entityConfig.RoleFields.TryGetValue(SixnetFieldRole.PrimaryKey, out var primaryKeyFields))
            {
                defaultIdFieldName = primaryKeyFields?.First()?.PropertyName;
            }
        }

        protected SixnetCriterion GetVersionCriterion(long version)
        {
            return SixnetCriterion.Create(SixnetCriterionOperator.Equal, SixnetDataField.Create(versionFieldName, typeof(TEntity)), SixnetConstantField.Create(version));
        }

        protected SixnetCriterion GetHistoryTagCriterion(bool isHistorical)
        {
            return SixnetCriterion.Create(SixnetCriterionOperator.Equal, SixnetDataField.Create(historyTagFieldName, typeof(TEntity)), SixnetConstantField.Create(isHistorical));
        }

        protected ISixnetQueryable<TEntity> GetIdQueryable(dynamic id)
        {
            var idCriterion = SixnetCriterion.Create(SixnetCriterionOperator.Equal, SixnetDataField.Create(defaultIdFieldName, typeof(TEntity)), SixnetConstantField.Create(id));

            return repository
                .AsQueryable()
                .Where(idCriterion);
        }

        protected ISixnetQueryable<TEntity> GetIdsQueryable(IEnumerable<dynamic> ids)
        {
            var idCriterion = SixnetCriterion.Create(SixnetCriterionOperator.In, SixnetDataField.Create(defaultIdFieldName, typeof(TEntity)), SixnetConstantField.Create(ids));

            return repository
                .AsQueryable()
                .Where(idCriterion);
        }

        /// <summary>
        /// Handle queryable
        /// </summary>
        /// <param name="queryable"></param>
        /// <param name="version"></param>
        /// <param name="includeArchived"></param>
        /// <returns></returns>
        internal protected ISixnetQueryable HandleQueryable(ISixnetQueryable queryable, bool includeHistorical = false, long? version = null, bool includeArchived = false)
        {
            queryable ??= SixnetQuerier.Create<TEntity>();

            if (queryable.GetModelType() != typeof(TEntity))
            {
                queryable = SixnetQuerier.Create<TEntity>(queryable);
            }

            if (isVersioning)
            {
                var hasVersion = version.HasValue && version.Value > 0;
                if (hasVersion)
                {
                    var versionCriterion = GetVersionCriterion(version.Value);
                    queryable.Where(versionCriterion);
                }
                includeHistorical |= hasVersion;
                includeHistorical |= queryable.Info.IsFromAllVersions();
            }

            queryable = queryable
                .IncludeHistoricalIf(includeHistorical)
                .IncludeArchivedIf(includeArchived);

            return queryable;
        }
    }
}
