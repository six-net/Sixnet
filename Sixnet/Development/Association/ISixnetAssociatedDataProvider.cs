// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixnet.Development.Association
{
    public interface ISixnetAssociatedDataProvider
    {
        /// <summary>
        /// Get associated datas
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task LoadAssociatedDatasAsync(SixnetLoadAssociatedDataContext context);
    }

    public interface ISixnetAssociatedDataProvider<T> : ISixnetAssociatedDataProvider
    {
    }

    internal interface ISixnetDefaultAssociatedDataProvider<TAssociatedDto, TAssociatedEntity> : ISixnetAssociatedDataProvider
    { }
}
