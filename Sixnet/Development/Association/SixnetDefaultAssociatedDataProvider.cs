// "Company © 2025. All rights reserved."

using System.Threading.Tasks;
using Sixnet.DependencyInjection;
using Sixnet.Development.Domain.Service;
using Sixnet.Development.Queryable;

namespace Sixnet.Development.Association
{
    internal class SixnetDefaultAssociatedDataProvider<TAssociatedDto, TAssociatedEntity> : ISixnetDefaultAssociatedDataProvider<TAssociatedDto, TAssociatedEntity>
    {
        readonly ISixnetQueryService<TAssociatedEntity> entityQueryService = SixnetContainer.GetInstance<ISixnetQueryService<TAssociatedEntity>>();

        public async Task LoadAssociatedDatasAsync(SixnetLoadAssociatedDataContext context)
        {
            var queryable = context.GetAssociatedDataQueryable();
            if (queryable != null)
            {
                queryable = SixnetQuerier.Create<TAssociatedEntity>(queryable);
                var dataList = (await entityQueryService.GetListAsync(queryable).ConfigureAwait(false))
                    .MapTo<List<TAssociatedDto>>();
                context.SetAssociatedDatas(dataList);
            }
        }
    }
}
