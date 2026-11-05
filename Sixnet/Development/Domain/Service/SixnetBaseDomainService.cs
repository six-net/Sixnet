// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sixnet.DependencyInjection;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Entity;
using Sixnet.Development.Queryable;
using Sixnet.Development.Repository;
using Sixnet.Exceptions;

namespace Sixnet.Development.Domain.Service
{
    public abstract class SixnetBaseDomainService<T> where T : SixnetBaseEntity<T>, new()
    {
        protected readonly ISixnetRepository<T> repository = SixnetContainer.GetRepository<T>();

        protected readonly bool isSeparateVersioning = false;

        protected readonly bool isVersioning = false;

        protected readonly string versionFieldName = string.Empty;

        protected readonly string historyTagFieldName = string.Empty;

        protected readonly string defaultIdFieldName = string.Empty;

        public SixnetBaseDomainService()
        {
            var entityConfig = SixnetEntityManager.GetEntityConfig<T>();
            isSeparateVersioning = entityConfig.InStyle(SixnetEntityStyle.SeparateVersioning);
            isVersioning = entityConfig.InStyle(SixnetEntityStyle.Versioning) || isSeparateVersioning;

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

        protected ISixnetQueryable<T> GetIdQueryable(dynamic id)
        {
            var idCriterion = SixnetCriterion.Create(SixnetCriterionOperator.Equal, SixnetDataField.Create(defaultIdFieldName, typeof(T)), SixnetConstantField.Create(id));

            return repository
                .AsQueryable()
                .Where(idCriterion);
        }

        protected ISixnetQueryable<T> GetIdsQueryable(IEnumerable<dynamic> ids)
        {
            var idCriterion = SixnetCriterion.Create(SixnetCriterionOperator.In, SixnetDataField.Create(defaultIdFieldName, typeof(T)), SixnetConstantField.Create(ids));

            return repository
                .AsQueryable()
                .Where(idCriterion);
        }

        protected SixnetCriterion GetHistoryTagCriterion(bool isHistorical)
        {
            return SixnetCriterion.Create(SixnetCriterionOperator.Equal, SixnetDataField.Create(historyTagFieldName, typeof(T)), SixnetConstantField.Create(isHistorical));
        }
    }
}
