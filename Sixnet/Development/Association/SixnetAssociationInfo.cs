// "Company © 2025. All rights reserved."

using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sixnet.Exceptions;
using Sixnet.Model;

namespace Sixnet.Development.Association
{
    public interface ISixnetAssociationInfo
    {
        /// <summary>
        /// Gets or sets the source property name
        /// </summary>
        string SourcePropertyName { get; set; }

        /// <summary>
        /// Gets or sets the source data type
        /// </summary>
        Type SourceDataType { get; set; }

        /// <summary>
        /// Gets or sets the associated data type
        /// </summary>
        Type AssociatedDataType { get; set; }

        /// <summary>
        /// Gets or sets the associated entity type
        /// </summary>
        Type AssociatedEntityType { get; set; }

        /// <summary>
        /// Gets or sets the provider type
        /// </summary>
        Type AssociatedDataProviderType { get; set; }

        /// <summary>
        /// Gets or sets the default associated data provider type
        /// </summary>
        Type DefaultAssociatedDataProviderType {  get; set; }

        /// <summary>
        /// Whether include archived data
        /// </summary>
        bool IncludeArchived { get; set; }

        /// <summary>
        /// Whether include historical data
        /// </summary>
        bool IncludeHistorical {  get; set; }

        /// <summary>
        /// Gets or sets the identity infos
        /// </summary>
        SortedDictionary<string, ISixnetAssociatingIdentityInfo> IdentityInfos { get; set; }

        /// <summary>
        /// Set associated data
        /// </summary>
        /// <param name="sourceData"></param>
        /// <param name="value"></param>
        void SetAssociatedData(object sourceData, object associatedData);

        /// <summary>
        /// Get source data identity value
        /// </summary>
        /// <param name="sourceData"></param>
        /// <returns></returns>
        string GetSouceDataIdentityValue(object sourceData);

        /// <summary>
        /// Get association data identity value
        /// </summary>
        /// <param name="associatedData"></param>
        /// <returns></returns>
        string GetAssociatedDataIdentityValue(object associatedData);
    }

    public class SixnetAssociationInfo<T> : ISixnetAssociationInfo
    {
        /// <summary>
        /// Gets or sets the source property name
        /// </summary>
        public string SourcePropertyName { get; set; }

        /// <summary>
        /// Gets or sets the source data type
        /// </summary>
        public Type SourceDataType { get; set; }

        /// <summary>
        /// Gets or sets the associated data type
        /// </summary>
        public Type AssociatedDataType { get; set; }

        /// <summary>
        /// Gets or sets the associated entity type
        /// </summary>
        public Type AssociatedEntityType { get; set; }

        /// <summary>
        /// Gets or sets the provider type
        /// </summary>
        public Type AssociatedDataProviderType { get; set; }

        /// <summary>
        /// Gets or sets the default associated data provider type
        /// </summary>
        public Type DefaultAssociatedDataProviderType { get; set; }

        /// <summary>
        /// Whether include archived data
        /// </summary>
        public bool IncludeArchived { get; set; }

        /// <summary>
        /// Whether include historical data
        /// </summary>
        public bool IncludeHistorical { get; set; }

        /// <summary>
        /// Gets or sets the identity infos
        /// </summary>
        public SortedDictionary<string, ISixnetAssociatingIdentityInfo> IdentityInfos { get; set; }

        /// <summary>
        /// Set associated data action
        /// </summary>
        Action<T, object> SetAssociatedDataAction { get; set; }

        /// <summary>
        /// Set associated data
        /// </summary>
        /// <param name="sourceData"></param>
        /// <param name="associatedData"></param>
        public void SetAssociatedData(object sourceData, object associatedData)
        {
            if (sourceData is T sourceTypeData)
            {
                SetAssociatedDataAction?.Invoke(sourceTypeData, associatedData);
            }
        }

        internal void SetSettingAssociatedDataAction(Expression<Action<T, object>> expression)
        {
            if (expression != null)
            {
                SetAssociatedDataAction = expression.Compile();
            }
        }

        /// <summary>
        /// Get source data identity value
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string GetSouceDataIdentityValue(object data)
        {
            SixnetThrower.ThrowArgNullIf(data == null, "source data is null");

            var identityInfoKeys = IdentityInfos.Keys;
            var identityValueBuilder = new StringBuilder();
            foreach (var identityKey in identityInfoKeys)
            {
                var identityInfo = IdentityInfos[identityKey];
                identityValueBuilder.Append($"{identityInfo.GetSourceDataValue(data)}");
            }
            return identityValueBuilder.ToString();
        }

        /// <summary>
        /// Get associated data identity value
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string GetAssociatedDataIdentityValue(object data)
        {
            SixnetThrower.ThrowArgNullIf(data == null, "associated data is null");

            var identityInfoKeys = IdentityInfos.Keys;
            var identityValueBuilder = new StringBuilder();
            foreach (var identityKey in identityInfoKeys)
            {
                var identityInfo = IdentityInfos[identityKey];
                identityValueBuilder.Append($"{identityInfo.GetAssociatedDataValue(data)}");
            }
            return identityValueBuilder.ToString();
        }
    }
}
