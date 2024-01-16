using System;
using System.Collections.Generic;
using System.Text;

namespace Sixnet.Model.Paging
{
    /// <summary>
    /// Paging data model
    /// </summary>
    public class PagingTotalCountModel
    {
        /// <summary>
        /// Total data count
        /// </summary>
        public int PagingTotalDataCount { get; set; }
    }

    public class PagingTotalCountMappingModel<T>
    {
        /// <summary>
        /// Total data count
        /// </summary>
        public int PagingTotalDataCount { get; set; }

        /// <summary>
        /// T
        /// </summary>
        public T RealReturnData { get; set; }

        /// <summary>
        /// Paging total count mapping func
        /// </summary>
        /// <param name="totalCountModel"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static PagingTotalCountMappingModel<T> PagingTotalCountMappingFunc(PagingTotalCountModel totalCountModel, T data)
        {
            return new PagingTotalCountMappingModel<T>()
            {
                PagingTotalDataCount = totalCountModel.PagingTotalDataCount,
                RealReturnData = data
            };
        }
    }
}
