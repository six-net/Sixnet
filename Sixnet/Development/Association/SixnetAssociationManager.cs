// "Company © 2025. All rights reserved."

using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

using AutoMapper.Internal;

using Sixnet.DependencyInjection;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Entity;
using Sixnet.Development.Queryable;
using Sixnet.Exceptions;
using Sixnet.Model;
using Sixnet.Reflection;

namespace Sixnet.Development.Association
{
    internal class SixnetAssociationConfigInfo
    {
        public Type SourceModelType { get; set; }

        public MemberInfo Member { get; set; }

        public SixnetAssociatingAttribute AssociationAttribute { get; set; }
    }

    public static class SixnetAssociationManager
    {
        #region Config

        /// <summary>
        /// Key: source type
        /// Value key1: associated data type, key2: associated property or field name
        /// </summary>
        internal static Dictionary<Type, Dictionary<string, ISixnetAssociationInfo>> AssociationConfiguration = new Dictionary<Type, Dictionary<string, ISixnetAssociationInfo>>();

        static List<SixnetAssociationConfigInfo> AssociationConfigInfos = new List<SixnetAssociationConfigInfo>();

        /// <summary>
        /// Set association info
        /// </summary>
        /// <param name="type"></param>
        internal static void SetAssociationConfigurable(Type type)
        {
            if (type == null || type.IsInterface)
            {
                return;
            }
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            if (!properties.IsNullOrEmpty())
            {
                foreach (var property in properties)
                {
                    var associationAttr = property.GetCustomAttribute<SixnetAssociatingAttribute>();
                    if (associationAttr != null)
                    {
                        AssociationConfigInfos.Add(new SixnetAssociationConfigInfo()
                        {
                            SourceModelType = type,
                            Member = property,
                            AssociationAttribute = associationAttr
                        });
                    }
                }
            }

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            if (!fields.IsNullOrEmpty())
            {
                foreach (var field in fields)
                {
                    var associationAttr = field.GetCustomAttribute<SixnetAssociatingAttribute>();
                    if (associationAttr != null)
                    {
                        AssociationConfigInfos.Add(new SixnetAssociationConfigInfo()
                        {
                            SourceModelType = type,
                            Member = field,
                            AssociationAttribute = associationAttr
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Execute association configurable
        /// </summary>
        internal static void ExecuteAssociationConfigurable()
        {
            foreach (var configItem in AssociationConfigInfos)
            {
                SetAssociationInfo(configItem.SourceModelType, configItem.Member, configItem.AssociationAttribute);
            }
            AssociationConfigInfos = null;
        }

        /// <summary>
        /// Set association info
        /// </summary>
        /// <param name="sourceModelType"></param>
        /// <param name="member"></param>
        /// <param name="sixnetAssociatingDataAttribute"></param>
        internal static void SetAssociationInfo(Type sourceModelType, MemberInfo member, SixnetAssociatingAttribute sixnetAssociatingDataAttribute)
        {
            if (!AssociationConfiguration.TryGetValue(sourceModelType, out var sourceTypeAssociationTypes))
            {
                sourceTypeAssociationTypes = new Dictionary<string, ISixnetAssociationInfo>();
                AssociationConfiguration[sourceModelType] = sourceTypeAssociationTypes;
            }

            if (sourceTypeAssociationTypes.ContainsKey(member.Name))
            {
                return;
            }

            var memberType = member.GetMemberType();
            var associatedDataType = sixnetAssociatingDataAttribute.AssociatedType ?? memberType;

            var identityValueNames = sixnetAssociatingDataAttribute?.IdentityPropertyNames;
            SixnetThrower.ThrowSixnetExceptionIf(identityValueNames.IsNullOrEmpty(), $"Not set associated identity property for {associatedDataType}");

            var associatedEntityType = sixnetAssociatingDataAttribute.AssociatedEntityType;
            if (associatedEntityType == null)
            {
                var associatedEntityAttr = associatedDataType.GetCustomAttribute<SixnetAssociatingEntityAttribute>();
                associatedEntityType = associatedEntityAttr?.EntityType;

                if (associatedEntityType == null)
                {
                    var associatedDataBaseType = associatedDataType.BaseType;
                    while (associatedDataBaseType != null)
                    {
                        var entityConfig = SixnetEntityManager.GetEntityConfig(associatedDataBaseType);
                        if (entityConfig != null)
                        {
                            associatedEntityType = associatedDataBaseType;
                            break;
                        }
                        associatedDataBaseType = associatedDataBaseType.BaseType;
                    }
                }
            }

            var identityInfoType = typeof(SixnetAssociatingIdentityInfo<,>).MakeGenericType(sourceModelType, associatedDataType);
            var identityInfos = new SortedDictionary<string, ISixnetAssociatingIdentityInfo>();
            foreach (var identityPropertyName in identityValueNames)
            {
                var identityProperty = SixnetReflecter.GetPropertyOrField(sourceModelType, identityPropertyName);
                SixnetThrower.ThrowSixnetExceptionIf(identityProperty == null, $"Not found any property or field named as {identityPropertyName} in {sourceModelType}");
                var identityPropertyType = identityProperty.GetType();

                var associationPropertyName = identityPropertyName;
                var associationFieldName = string.Empty;
                var identityAttribute = identityProperty.GetCustomAttribute<SixnetAssociatingIdentityAttribute>() ?? new SixnetAssociatingIdentityAttribute();
                if (!string.IsNullOrWhiteSpace(identityAttribute.AssociatedPropertyName))
                {
                    associationPropertyName = identityAttribute.AssociatedPropertyName;
                }
                if (!string.IsNullOrWhiteSpace(identityAttribute.AssociatedFieldName))
                {
                    associationFieldName = identityAttribute.AssociatedFieldName;
                }
                if (string.IsNullOrWhiteSpace(associationFieldName))
                {
                    associationFieldName = associationPropertyName;
                }

                var identityInfo = Activator.CreateInstance(identityInfoType);
                SixnetReflecter.SetValue(identityInfo, nameof(SixnetAssociatingIdentityInfo<object, object>.SourcePropertyName), identityPropertyName);
                SixnetReflecter.SetValue(identityInfo, nameof(SixnetAssociatingIdentityInfo<object, object>.AssociatedPropertyName), associationPropertyName);
                SixnetReflecter.SetValue(identityInfo, nameof(SixnetAssociatingIdentityInfo<object, object>.AssociatedFieldName), associationFieldName);

                // Get souce value func
                var getSourceValueExpression = SixnetReflecter.Expressions.GenerateGettingValueLambdaExpression(sourceModelType, identityProperty);
                SixnetReflecter.InvokeMethod(identityInfo, nameof(SixnetAssociatingIdentityInfo<object, object>.SetGettingSourceDataValueFunc), getSourceValueExpression);

                // Get association value func
                var associationIdentityProperty = SixnetReflecter.GetPropertyOrField(associatedDataType, associationPropertyName);
                SixnetThrower.ThrowSixnetExceptionIf(associationIdentityProperty == null, $"Not found any property or field named as {associationPropertyName} in {associatedDataType}");

                var getAssociationValueExpression = SixnetReflecter.Expressions.GenerateGettingValueLambdaExpression(associatedDataType, associationIdentityProperty);
                SixnetReflecter.InvokeMethod(identityInfo, nameof(SixnetAssociatingIdentityInfo<object, object>.SetGettingAssociatedDataValueFunc), getAssociationValueExpression);

                identityInfos[identityPropertyName] = (ISixnetAssociatingIdentityInfo)identityInfo;
            }

            var associationInfoType = typeof(SixnetAssociationInfo<>).MakeGenericType(sourceModelType);
            var associationInfo = Activator.CreateInstance(associationInfoType);
            var associationDataProviderType = typeof(ISixnetAssociatedDataProvider<>).MakeGenericType(associatedDataType);
            SixnetReflecter.SetValue(associationInfo, nameof(SixnetAssociationInfo<object>.SourcePropertyName), member.Name);
            SixnetReflecter.SetValue(associationInfo, nameof(SixnetAssociationInfo<object>.SourceDataType), sourceModelType);
            SixnetReflecter.SetValue(associationInfo, nameof(SixnetAssociationInfo<object>.AssociatedDataType), associatedDataType);
            SixnetReflecter.SetValue(associationInfo, nameof(SixnetAssociationInfo<object>.AssociatedEntityType), associatedEntityType);
            SixnetReflecter.SetValue(associationInfo, nameof(SixnetAssociationInfo<object>.AssociatedDataProviderType), associationDataProviderType);
            SixnetReflecter.SetValue(associationInfo, nameof(SixnetAssociationInfo<object>.IdentityInfos), identityInfos);
            SixnetReflecter.SetValue(associationInfo, nameof(SixnetAssociationInfo<object>.IncludeArchived), sixnetAssociatingDataAttribute.IncludeArchived);

            if (associatedEntityType != null)
            {
                SixnetReflecter.SetValue(associationInfo, nameof(ISixnetAssociationInfo.DefaultAssociatedDataProviderType), typeof(ISixnetDefaultAssociatedDataProvider<,>).MakeGenericType(associatedDataType, associatedEntityType));
            }

            // Set association data action
            var setDataExpression = SixnetReflecter.Expressions.GenerateSettingValueLambdaExpression(sourceModelType, member);
            if (setDataExpression != null)
            {
                SixnetReflecter.InvokeMethod(associationInfo, nameof(SixnetAssociationInfo<object>.SetSettingAssociatedDataAction), setDataExpression);
            }

            sourceTypeAssociationTypes[member.Name] = (ISixnetAssociationInfo)associationInfo;
        }

        #endregion

        #region Load datas

        static Tuple<Dictionary<string, HashSet<object>>, Dictionary<string, ISixnetQueryable>> GetIdentityValues(SixnetLoadAssociatedDataInfo loadInfo)
        {
            var sourceDataType = loadInfo?.SourceDataType;
            var allowLoadNames = loadInfo?.Loadable?.GetDataNames();
            var sourceDatas = loadInfo?.SourceDatas;
            if (sourceDataType == null || allowLoadNames.IsNullOrEmpty() || sourceDatas.IsNullOrEmpty())
            {
                return null;
            }
            if (!AssociationConfiguration.TryGetValue(sourceDataType, out var sourceTypeAssociationTypes) || sourceTypeAssociationTypes.IsNullOrEmpty())
            {
                return null;
            }

            var associationIdentityValues = new Dictionary<string, HashSet<object>>();
            var associationQueryables = new Dictionary<string, ISixnetQueryable>();
            foreach (var data in loadInfo.SourceDatas)
            {
                if (data == null)
                {
                    continue;
                }
                foreach (var loadDataName in allowLoadNames)
                {
                    if (string.IsNullOrWhiteSpace(loadDataName) || !sourceTypeAssociationTypes.TryGetValue(loadDataName, out var associItem))
                    {
                        continue;
                    }

                    if (associItem.IdentityInfos.IsNullOrEmpty())
                    {
                        continue;
                    }

                    if (associItem.IdentityInfos.Count > 1)
                    {
                        if (!associationQueryables.TryGetValue(loadDataName, out var associationIdentityQueryable))
                        {
                            associationIdentityQueryable ??= SixnetQuerier.Create();
                            associationQueryables[loadDataName] = associationIdentityQueryable;
                        }

                        var identityGroupQueryable = SixnetQuerier.Create();
                        identityGroupQueryable.Connector = SixnetCriterionConnector.Or;
                        foreach (var identityItem in associItem.IdentityInfos)
                        {
                            var identityInfo = identityItem.Value;
                            var identityValue = identityInfo.GetSourceDataValue(data);
                            var identityCriterion = SixnetCriterion.Create(SixnetCriterionOperator.Equal, SixnetDataField.Create(identityInfo.AssociatedFieldName), SixnetConstantField.Create(identityValue));
                            identityGroupQueryable.Where(identityCriterion);
                        }

                        associationIdentityQueryable.Where(identityGroupQueryable);

                    }
                    else
                    {
                        if (!associationIdentityValues.TryGetValue(loadDataName, out var identityValues))
                        {
                            identityValues ??= new HashSet<object>();
                            associationIdentityValues[loadDataName] = identityValues;
                        }
                        var firstIdentityInfo = associItem.IdentityInfos.First();
                        var identityValue = firstIdentityInfo.Value.GetSourceDataValue(data);
                        if (identityValue != null)
                        {
                            identityValues.Add(identityValue);
                        }
                    }
                }
            }

            return new Tuple<Dictionary<string, HashSet<object>>, Dictionary<string, ISixnetQueryable>>(associationIdentityValues, associationQueryables);
        }

        static ISixnetAssociatedDataProvider GetAssociationDataProvider(ISixnetAssociationInfo associationInfo)
        {
            var provider = SixnetContainer.GetInstance(associationInfo.AssociatedDataProviderType);
            if (provider == null && associationInfo.DefaultAssociatedDataProviderType != null)
            {
                SixnetThrower.ThrowSixnetExceptionIf(associationInfo.AssociatedEntityType == null, $"Must set {nameof(ISixnetAssociationInfo.AssociatedEntityType)} for default associated data provider");
                provider = SixnetContainer.GetInstance(associationInfo.DefaultAssociatedDataProviderType);
            }
            SixnetThrower.ThrowSixnetExceptionIf(provider == null, $"Not found any data provider for {associationInfo.AssociatedDataType}");
            return provider as ISixnetAssociatedDataProvider;
        }

        static void SetAssociationDatas(SixnetLoadAssociatedDataInfo loadInfo, Dictionary<string, SixnetLoadAssociatedDataContext> contexts)
        {
            foreach (var data in loadInfo.SourceDatas)
            {
                foreach (var contextItem in contexts)
                {
                    contextItem.Value.SetSourceAssociatedData(data);
                }
            }
        }

        /// <summary>
        /// Load associated datas
        /// </summary>
        /// <param name="loadInfo"></param>
        internal static void LoadDatas(SixnetLoadAssociatedDataInfo loadInfo)
        {
            // get identity
            var allIdentityInfos = GetIdentityValues(loadInfo);
            if (allIdentityInfos == null)
            {
                return;
            }

            var associationIdentityValues = allIdentityInfos.Item1;
            var associationQueryables = allIdentityInfos.Item2;


            // load data
            var sourceTypeAssociationTypes = AssociationConfiguration[loadInfo.SourceDataType];
            var associationContexts = new Dictionary<string, SixnetLoadAssociatedDataContext>();
            foreach (var associationIdentityItem in associationIdentityValues)
            {
                var dataName = associationIdentityItem.Key;
                if (!sourceTypeAssociationTypes.TryGetValue(dataName, out var associInfo))
                {
                    continue;
                }

                var provider = GetAssociationDataProvider(associInfo);
                var associationQueryable = SixnetQuerier.Create();
                var firstIdentityInfo = associInfo.IdentityInfos.First();
                associationQueryable.Where(SixnetCriterion.Create(SixnetCriterionOperator.In, SixnetDataField.Create(firstIdentityInfo.Value.AssociatedFieldName), SixnetConstantField.Create(associationIdentityItem.Value)));

                var loadContext = new SixnetLoadAssociatedDataContext(associationQueryable, loadInfo.SourceDatas, associInfo, loadInfo.Loadable);
                associationContexts[dataName] = loadContext;
                provider.LoadAssociatedDatasAsync(loadContext).Wait();
            }
            foreach (var associationQueryableItem in associationQueryables)
            {
                var dataName = associationQueryableItem.Key;
                if (!sourceTypeAssociationTypes.TryGetValue(dataName, out var associInfo))
                {
                    continue;
                }

                var provider = GetAssociationDataProvider(associInfo);
                var loadContext = new SixnetLoadAssociatedDataContext(SixnetQuerier.Create().Where(associationQueryableItem.Value), loadInfo.SourceDatas, associInfo, loadInfo.Loadable);
                associationContexts[dataName] = loadContext;
                provider.LoadAssociatedDatasAsync(loadContext).Wait();
            }

            // set data
            SetAssociationDatas(loadInfo, associationContexts);
        }

        /// <summary>
        /// Load associated datas
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="loadable"></param>
        public static void LoadDatas<T>(T data, ISixnetAssociationLoadable loadable) where T : class
        {
            LoadDatas(new SixnetLoadAssociatedDataInfo()
            {
                SourceDatas = new List<T>() { data },
                SourceDataType = typeof(T),
                Loadable = loadable
            });
        }

        /// <summary>
        /// Load associated datas
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datas"></param>
        /// <param name="loadable"></param>
        public static void LoadDatas<T>(IEnumerable<T> datas, ISixnetAssociationLoadable loadable) where T : class
        {
            LoadDatas(new SixnetLoadAssociatedDataInfo()
            {
                SourceDatas = datas,
                SourceDataType = typeof(T),
                Loadable = loadable
            });
        }

        /// <summary>
        /// Load associated datas
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="datas"></param>
        /// <param name="loadedPropertyNames"></param>
        public static void LoadDatas<T>(IEnumerable<T> datas, params string[] loadedPropertyNames) where T : class
        {
            if (loadedPropertyNames.IsNullOrEmpty())
            {
                return;
            }
            var loadable = new SixnetAssociationLoadable();
            loadable.Need(loadedPropertyNames);
            LoadDatas(datas, loadable);
        }

        /// <summary>
        /// Load associated datas
        /// </summary>
        /// <param name="loadInfo"></param>
        /// <returns></returns>
        internal static async Task LoadDatasAsync(SixnetLoadAssociatedDataInfo loadInfo)
        {
            // get identity
            var allIdentityInfos = GetIdentityValues(loadInfo);
            if (allIdentityInfos == null)
            {
                return;
            }

            var associationIdentityValues = allIdentityInfos.Item1;
            var associationQueryables = allIdentityInfos.Item2;

            // load data
            var sourceTypeAssociationTypes = AssociationConfiguration[loadInfo.SourceDataType];
            var associationContexts = new Dictionary<string, SixnetLoadAssociatedDataContext>();
            var loadTasks = new List<Task>();
            foreach (var associationIdentityItem in associationIdentityValues)
            {
                var dataName = associationIdentityItem.Key;
                if (!sourceTypeAssociationTypes.TryGetValue(dataName, out var associInfo))
                {
                    continue;
                }

                var provider = GetAssociationDataProvider(associInfo);
                var associationQueryable = SixnetQuerier.Create();
                var firstIdentityInfo = associInfo.IdentityInfos.First();
                associationQueryable.Where(SixnetCriterion.Create(SixnetCriterionOperator.In, SixnetDataField.Create(firstIdentityInfo.Value.AssociatedFieldName), SixnetConstantField.Create(associationIdentityItem.Value)));

                var loadContext = new SixnetLoadAssociatedDataContext(associationQueryable, loadInfo.SourceDatas, associInfo, loadInfo.Loadable);
                associationContexts[dataName] = loadContext;
                loadTasks.Add(provider.LoadAssociatedDatasAsync(loadContext));
            }
            foreach (var associationQueryableItem in associationQueryables)
            {
                var dataName = associationQueryableItem.Key;
                if (!sourceTypeAssociationTypes.TryGetValue(dataName, out var associInfo))
                {
                    continue;
                }

                var provider = GetAssociationDataProvider(associInfo);
                var loadContext = new SixnetLoadAssociatedDataContext(SixnetQuerier.Create().Where(associationQueryableItem.Value), loadInfo.SourceDatas, associInfo, loadInfo.Loadable);
                associationContexts[dataName] = loadContext;
                loadTasks.Add(provider.LoadAssociatedDatasAsync(loadContext));
            }

            await Task.WhenAll(loadTasks).ConfigureAwait(false);

            // set data
            SetAssociationDatas(loadInfo, associationContexts);
        }

        /// <summary>
        /// Load associated datas
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceDatas">Source datas</param>
        /// <param name="loadable">Loadable</param>
        public static Task LoadDatasAsync<T>(IEnumerable<T> sourceDatas, ISixnetAssociationLoadable loadable) where T : class
        {
            return LoadDatasAsync(new SixnetLoadAssociatedDataInfo()
            {
                SourceDatas = sourceDatas,
                SourceDataType = typeof(T),
                Loadable = loadable
            });
        }

        /// <summary>
        /// Load associated datas
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceDatas">Source datas</param>
        /// <param name="loadedPropertyNames">Loaded property names</param>
        public static async Task LoadDatasAsync<T>(IEnumerable<T> sourceDatas, params string[] loadedPropertyNames) where T : class
        {
            if (loadedPropertyNames.IsNullOrEmpty())
            {
                return;
            }
            var loadable = new SixnetAssociationLoadable();
            loadable.Need(loadedPropertyNames);
            await LoadDatasAsync(sourceDatas, loadable).ConfigureAwait(false);
        }

        #endregion
    }
}
