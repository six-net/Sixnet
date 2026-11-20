// "Company © 2025. All rights reserved."

using System.Threading.Tasks;
using Sixnet.DependencyInjection;
using Sixnet.Development.Queryable;
using Sixnet.Development.Repository;

namespace Sixnet.Development.Association
{
    internal class SixnetDefaultAssociatedDataProvider<TAssociatedDto, TAssociatedEntity> : ISixnetDefaultAssociatedDataProvider<TAssociatedDto, TAssociatedEntity>
    {
        readonly ISixnetRepository<TAssociatedEntity> entityRepository = SixnetContainer.GetRepository<TAssociatedEntity>();

        public async Task LoadAssociatedDatasAsync(SixnetLoadAssociatedDataContext context)
        {
            var queryable = context.GetAssociatedDataQueryable();
            if (queryable != null)
            {
                queryable = SixnetQuerier.Create<TAssociatedEntity>(queryable);
                var dataList = (await entityRepository.GetListAsync(queryable).ConfigureAwait(false))
                    .MapTo<List<TAssociatedDto>>();
                context.SetAssociatedDatas(dataList);
            }
        }
    }
}
