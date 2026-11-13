// "Company © 2025. All rights reserved."

using System.Collections;
using System.Text;

using Sixnet.Development.Queryable;

namespace Sixnet.Development.Association
{
    /// <summary>
    /// Load data context
    /// </summary>
    public class SixnetLoadAssociatedDataContext
    {
        private readonly IEnumerable _sourceDatas;

        private readonly ISixnetQueryable _queryable;

        private Dictionary<string, object> _associationDatas;

        private readonly ISixnetAssociationInfo _sixnetAssociationInfo;

        public SixnetLoadAssociatedDataContext(ISixnetQueryable associatedDataQueryable
            , IEnumerable sourceDatas
            , ISixnetAssociationInfo associationInfo
            , ISixnetAssociationLoadable loadable = null)
        {
            Loadable = loadable;
            _queryable = associatedDataQueryable?.FromAllVersions().IncludeArchivedIf(associationInfo.IncludeArchived);
            _sourceDatas = sourceDatas;
            _sixnetAssociationInfo = associationInfo;
        }

        /// <summary>
        /// Gets or sets the loadable
        /// </summary>
        public ISixnetAssociationLoadable Loadable { get; set; }

        /// <summary>
        /// Get source datas
        /// </summary>
        /// <returns></returns>
        public IEnumerable GetSourceDatas()
        {
            return _sourceDatas;
        }

        /// <summary>
        /// Get associated data queryable
        /// </summary>
        /// <returns></returns>
        public ISixnetQueryable GetAssociatedDataQueryable()
        {
            return _queryable;
        }

        /// <summary>
        /// Set associated datas
        /// </summary>
        /// <typeparam name="TAD"></typeparam>
        /// <param name="associatedDatas"></param>
        public void SetAssociatedDatas<TAD>(IEnumerable<TAD> associatedDatas)
        {
            if (associatedDatas.IsNullOrEmpty())
            {
                return;
            }
            _associationDatas = associatedDatas.ToDictionary(d => _sixnetAssociationInfo.GetAssociatedDataIdentityValue(d), d => (object)d);
        }

        /// <summary>
        /// Set source associated data
        /// </summary>
        /// <param name="sourceData"></param>
        internal void SetSourceAssociatedData(object sourceData)
        {
            if (sourceData == null)
            {
                return;
            }
            var sourceKey = _sixnetAssociationInfo.GetSouceDataIdentityValue(sourceData);
            if (_associationDatas?.TryGetValue(sourceKey, out var associationData) ?? false)
            {
                _sixnetAssociationInfo.SetAssociatedData(sourceData, associationData);
            }
        }
    }
}
