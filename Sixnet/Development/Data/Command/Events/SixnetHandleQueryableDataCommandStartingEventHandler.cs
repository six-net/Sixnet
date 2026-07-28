// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Field;
using Sixnet.Development.Entity;
using Sixnet.Development.Queryable;
using Sixnet.Exceptions;
using Sixnet.Session;

namespace Sixnet.Development.Data.Command.Events
{
    /// <summary>
    /// Data command handling query starting event handler
    /// </summary>
    internal class SixnetHandleQueryableDataCommandStartingEventHandler : ISixnetDataCommandStartingEventHandler
    {
        public void Handle(SixnetDataCommandStartingEvent dataCommandStartingEvent)
        {
            var dataCommand = dataCommandStartingEvent.Command;
            var queryable = dataCommand.Queryable;
            var entityType = dataCommand.GetEntityType();
            var operationType = dataCommand.OperationType;
            var oldValues = dataCommand.FieldsAssignment?.OldValues;

            // Clone queryable object
            var newQueryable = queryable?.Clone();
            if (newQueryable == null)
            {
                newQueryable = SixnetQuerier.Create();
                newQueryable.SetModelType(entityType);
            }

            // Version condition for update
            if (operationType == SixnetDataOperationType.Update && !oldValues.IsNullOrEmpty())
            {
                var versionFieldName = SixnetEntityManager.GetFieldName(entityType, SixnetFieldRole.Revision);
                if (!string.IsNullOrWhiteSpace(versionFieldName) && oldValues.ContainsKey(versionFieldName))
                {
                    var versionValue = oldValues[versionFieldName];
                    var versionCriterion = SixnetCriterion.Create(SixnetCriterionOperator.Equal, SixnetDataField.Create(versionFieldName, entityType), SixnetConstantField.Create(versionValue));
                    newQueryable = newQueryable.Where(versionCriterion);
                }
            }

            // Filter data
            newQueryable = FilterData(entityType, dataCommand.Options, newQueryable, dataCommand.OperationType);
            dataCommand.Queryable = newQueryable;
        }

        #region Data filter

        /// <summary>
        /// Filter data
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="dataOperationOptions">Data operation options</param>
        /// <param name="originalQueryable">Origin queryable</param>
        /// <param name="operationType">Operation type</param>
        /// <returns></returns>
        internal static ISixnetQueryable FilterData(Type entityType, SixnetDataOperationOptions dataOperationOptions, ISixnetQueryable originalQueryable, SixnetDataOperationType operationType)
        {
            originalQueryable ??= SixnetQuerier.Create();
            originalQueryable.SetModelType(entityType);

            // Queryable filter context
            var queryableFilterContext = new SixnetQueryableFilterContext()
            {
                UsageSceneModelType = entityType,
                OperationType = operationType,
                Location = SixnetQueryableLocation.Top,
                ModelType = entityType,
                OriginalQueryable = originalQueryable,
                OperationOptions = dataOperationOptions
            };

            var dataOptions = SixnetDataManager.GetDataOptions();
            return FilterData(dataOptions, queryableFilterContext);
        }

        /// <summary>
        /// Filter data
        /// </summary>
        /// <param name="context">Context</param>
        /// <returns></returns>
        static ISixnetQueryable FilterData(SixnetDataOptions dataOptions, SixnetQueryableFilterContext context)
        {
            SixnetDirectThrower.ThrowArgNullIf(context?.OriginalQueryable is null, nameof(SixnetQueryableFilterContext.OriginalQueryable));
            var originalQueryable = context.OriginalQueryable;
            var modelType = originalQueryable.GetModelType();

            // group, not set model type
            if (originalQueryable.Info.CheckUseForGroup()
                || modelType == null
                || modelType == SixnetQueryableInfo.DefaultModelType)
            {
                return originalQueryable;
            }

            #region filter

            var globalFilter = GetDataFilter(dataOptions, context);

            #endregion

            #region condition

            if (!originalQueryable.Info.Conditions.IsNullOrEmpty())
            {
                foreach (var condition in originalQueryable.Info.Conditions)
                {
                    if (condition is ISixnetQueryable conditionQueryable
                        && !conditionQueryable.Info.CheckUseForGroup())
                    {
                        context.OriginalQueryable = conditionQueryable;
                        context.Location = SixnetQueryableLocation.Condition;
                        context.ModelType = conditionQueryable.GetModelType();
                        FilterData(dataOptions, context);
                    }
                    if (condition is SixnetCriterion criterion)
                    {
                        if (criterion.Left is SixnetQueryableField leftQueryableField
                            && leftQueryableField.Queryable != null)
                        {
                            context.OriginalQueryable = leftQueryableField.Queryable;
                            context.Location = SixnetQueryableLocation.Subquery;
                            context.ModelType = leftQueryableField.Queryable.GetModelType();
                            FilterData(dataOptions, context);
                        }
                        if (criterion.Right is SixnetQueryableField rightQueryableField
                            && rightQueryableField.Queryable != null)
                        {
                            context.OriginalQueryable = rightQueryableField.Queryable;
                            context.Location = SixnetQueryableLocation.Subquery;
                            context.ModelType = rightQueryableField.Queryable.GetModelType();
                            FilterData(dataOptions, context);
                        }
                    }
                }
            }

            #endregion

            #region join

            if (!originalQueryable.Info.Joins.IsNullOrEmpty())
            {
                foreach (var join in originalQueryable.Info.Joins)
                {
                    if (join == null)
                    {
                        continue;
                    }
                    var joinTargetQueryable = join.Target;
                    if (joinTargetQueryable != null)
                    {
                        context.OriginalQueryable = joinTargetQueryable;
                        context.Location = SixnetQueryableLocation.JoinTarget;
                        context.ModelType = joinTargetQueryable.GetModelType();
                        FilterData(dataOptions, context);
                        if (joinTargetQueryable.Info.FromType == SixnetQueryableFromType.Table
                            && !joinTargetQueryable.Info.Criteria.IsNullOrEmpty())
                        {
                            var newJoinTargetQueryable = SixnetQuerier.Create()
                                .SetModelType(joinTargetQueryable.GetModelType())
                                .From(joinTargetQueryable);
                            join.Target = newJoinTargetQueryable;
                        }
                    }
                    if (join.Connection != null)
                    {
                        if (join.Connection is SixnetCriterion connectionCriterion)
                        {
                            if (connectionCriterion.Left is SixnetQueryableField leftQueryableField && leftQueryableField.Queryable != null)
                            {
                                context.OriginalQueryable = leftQueryableField.Queryable;
                                context.Location = SixnetQueryableLocation.JoinConnection;
                                context.ModelType = leftQueryableField.Queryable.GetModelType();
                                FilterData(dataOptions, context);
                            }
                            if (connectionCriterion.Right is SixnetQueryableField rightQueryableField && rightQueryableField.Queryable != null)
                            {
                                context.OriginalQueryable = rightQueryableField.Queryable;
                                context.Location = SixnetQueryableLocation.JoinConnection;
                                context.ModelType = rightQueryableField.Queryable.GetModelType();
                                FilterData(dataOptions, context);
                            }
                        }
                        if (join.Connection is ISixnetQueryable connectionQueryable)
                        {
                            context.OriginalQueryable = connectionQueryable;
                            context.Location = SixnetQueryableLocation.JoinConnection;
                            context.ModelType = connectionQueryable.GetModelType();
                            FilterData(dataOptions, context);
                        }
                    }
                }
            }

            #endregion

            #region tree

            var treeInfo = originalQueryable.Info.TreeInfo;
            if (treeInfo != null)
            {
                if (treeInfo.DataField is ISixnetQueryable dataFieldQueryable)
                {
                    context.OriginalQueryable = dataFieldQueryable;
                    context.Location = SixnetQueryableLocation.TreeField;
                    context.ModelType = dataFieldQueryable.GetModelType();
                    FilterData(dataOptions, context);
                }
                if (treeInfo.ParentField is ISixnetQueryable parentFieldQueryable)
                {
                    context.OriginalQueryable = parentFieldQueryable;
                    context.Location = SixnetQueryableLocation.TreeField;
                    context.ModelType = parentFieldQueryable.GetModelType();
                    FilterData(dataOptions, context);
                }
            }

            #endregion

            #region combine

            if (!originalQueryable.Info.Combines.IsNullOrEmpty())
            {
                foreach (var combine in originalQueryable.Info.Combines)
                {
                    if (combine?.Target != null)
                    {
                        context.OriginalQueryable = combine.Target;
                        context.Location = SixnetQueryableLocation.Combine;
                        context.ModelType = combine.Target.GetModelType();
                        FilterData(dataOptions, context);
                    }
                }
            }

            #endregion

            #region from

            if (originalQueryable.Info.FromType == SixnetQueryableFromType.Queryable
                && originalQueryable.Info.TargetQueryable != null)
            {
                context.OriginalQueryable = originalQueryable.Info.TargetQueryable;
                context.Location = SixnetQueryableLocation.From;
                context.ModelType = originalQueryable.Info.TargetQueryable.GetModelType();
                FilterData(dataOptions, context);
            }

            #endregion

            #region root

            if (globalFilter != null)
            {
                originalQueryable.Where(globalFilter);
            }

            #endregion

            return originalQueryable;
        }

        /// <summary>
        /// Get global filter
        /// </summary>
        /// <param name="context">Filter context</param>
        /// <returns></returns>
        internal static ISixnetCondition GetDataFilter(SixnetDataOptions options, SixnetQueryableFilterContext context)
        {
            SixnetDirectThrower.ThrowArgNullIf(context == null, nameof(context));

            if (context.OriginalQueryable == null)
            {
                context.OriginalQueryable = SixnetQuerier.Create();
                context.OriginalQueryable.SetModelType(context.ModelType);
            }
            var originalQueryable = context.OriginalQueryable;
            if (originalQueryable.Info.CheckUseForGroup())
            {
                return null;
            }

            // Custom  filter
            var dataFilter = options?.GetCustomFilter()?.Invoke(context);

            // Archived
            var ignoreArchived = originalQueryable.HasIgnoredFilter(SixnetFieldRole.Archive)
                                 || options.HasIgnoredRoleFilter(SixnetFieldRole.Archive)
                                 || (context.OperationType == SixnetDataOperationType.Delete && !SixnetDataManager.AllowLogicalDelete(context.OperationOptions));
            if (!ignoreArchived)
            {
                var inactiveFieldName = SixnetEntityManager.GetFieldName(context.ModelType, SixnetFieldRole.Archive);
                if (!string.IsNullOrWhiteSpace(inactiveFieldName))
                {
                    dataFilter ??= SixnetQuerier.Create().SetModelType(context.ModelType);
                    dataFilter = dataFilter.Where(SixnetCriterion.Create(SixnetCriterionOperator.Equal, SixnetDataField.Create(inactiveFieldName, context.ModelType), SixnetConstantField.Create(false)));
                }
            }

            // Isolation
            var ignoreIsolation = originalQueryable.HasIgnoredFilter(SixnetFieldRole.Isolation) || options.HasIgnoredRoleFilter(SixnetFieldRole.Isolation);
            if (!ignoreIsolation)
            {
                var isolationFieldName = SixnetEntityManager.GetFieldName(context.ModelType, SixnetFieldRole.Isolation);
                if (!string.IsNullOrWhiteSpace(isolationFieldName))
                {
                    var isolationField = SixnetEntityManager.GetEntityConfig(context.ModelType).AllFields[isolationFieldName];
                    var isolationDataId = SixnetSessionContext.Current?.Isolation?.Id;

                    SixnetException.ThrowIf(string.IsNullOrWhiteSpace(isolationDataId), "Not set isolation value");

                    dataFilter ??= SixnetQuerier.Create().SetModelType(context.ModelType);
                    dataFilter = dataFilter.Where(SixnetCriterion.Create(SixnetCriterionOperator.Equal, SixnetDataField.Create(isolationFieldName, context.ModelType)
                        , SixnetConstantField.Create(isolationDataId.ConvertTo(isolationField.DataType))));
                }
            }

            // Type filter
            var typeFilters = options.GetTypeFilters();
            if (!typeFilters.IsNullOrEmpty())
            {
                dataFilter ??= SixnetQuerier.Create().SetModelType(context.ModelType);
                foreach (var filterItem in typeFilters)
                {
                    if (context.ModelType != null
                        && filterItem.Key.IsAssignableFrom(context.ModelType)
                        && !originalQueryable.HasIgnoredFilter(filterItem.Key)
                        && filterItem.Value != null)
                    {
                        dataFilter.Where(filterItem.Value);
                    }
                }
            }

            return dataFilter;
        }

        #endregion
    }
}
