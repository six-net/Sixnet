using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Sixnet.Development.Data;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Entity;
using Sixnet.Development.Queryable;
using Sixnet.Exceptions;
using Sixnet.Session;

namespace Sixnet.Development.Data.Command.Event
{
    /// <summary>
    /// Data command handling query starting event handler
    /// </summary>
    internal class HandleQueryableDataCommandStartingEventHandler : ISixnetDataCommandStartingEventHandler
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
            if (operationType == DataOperationType.Update && !oldValues.IsNullOrEmpty())
            {
                var versionFieldName = SixnetEntityManager.GetFieldName(entityType, FieldRole.Revision);
                if (!string.IsNullOrWhiteSpace(versionFieldName) && oldValues.ContainsKey(versionFieldName))
                {
                    var versionValue = oldValues[versionFieldName];
                    var versionCriterion = Criterion.Create(CriterionOperator.Equal, DataField.Create(versionFieldName, entityType), ConstantField.Create(versionValue));
                    newQueryable = newQueryable.Where(versionCriterion);
                }
            }

            // Filter data
            newQueryable = FilterData(entityType, newQueryable, dataCommand.OperationType);
            dataCommand.Queryable = newQueryable;
        }

        #region Data filter

        /// <summary>
        /// Filter data
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <param name="originalQueryable">Origin queryable</param>
        /// <param name="operationType">Operation type</param>
        /// <returns></returns>
        internal static ISixnetQueryable FilterData(Type entityType, ISixnetQueryable originalQueryable, DataOperationType operationType)
        {
            originalQueryable ??= SixnetQuerier.Create();
            originalQueryable.SetModelType(entityType);

            // Queryable filter context
            var queryableFilterContext = new QueryableFilterContext()
            {
                UsageSceneModelType = entityType,
                OperationType = operationType,
                Location = QueryableLocation.Top,
                ModelType = entityType,
                OriginalQueryable = originalQueryable
            };

            var dataOptions = SixnetDataManager.GetDataOptions();
            return FilterData(dataOptions, queryableFilterContext);
        }

        /// <summary>
        /// Filter data
        /// </summary>
        /// <param name="context">Context</param>
        /// <returns></returns>
        static ISixnetQueryable FilterData(DataOptions dataOptions, QueryableFilterContext context)
        {
            SixnetDirectThrower.ThrowArgNullIf(context?.OriginalQueryable is null, nameof(QueryableFilterContext.OriginalQueryable));
            var originalQueryable = context.OriginalQueryable;
            var modelType = originalQueryable.GetModelType();

            // group, not set model type
            if (originalQueryable.IsGroupQueryable
                || modelType == null
                || modelType == QueryableContext.DefaultModelType)
            {
                return originalQueryable;
            }

            #region filter

            var globalFilter = GetDataFilter(dataOptions, context);

            #endregion

            #region condition

            if (!originalQueryable.Conditions.IsNullOrEmpty())
            {
                foreach (var condition in originalQueryable.Conditions)
                {
                    if (condition is ISixnetQueryable conditionQueryable
                        && !conditionQueryable.IsGroupQueryable)
                    {
                        context.OriginalQueryable = conditionQueryable;
                        context.Location = QueryableLocation.Condition;
                        context.ModelType = conditionQueryable.GetModelType();
                        FilterData(dataOptions, context);
                    }
                    if (condition is Criterion criterion)
                    {
                        if (criterion.Left is QueryableField leftQueryableField
                            && leftQueryableField.Queryable != null)
                        {
                            context.OriginalQueryable = leftQueryableField.Queryable;
                            context.Location = QueryableLocation.Subquery;
                            context.ModelType = leftQueryableField.Queryable.GetModelType();
                            FilterData(dataOptions, context);
                        }
                        if (criterion.Right is QueryableField rightQueryableField
                            && rightQueryableField.Queryable != null)
                        {
                            context.OriginalQueryable = rightQueryableField.Queryable;
                            context.Location = QueryableLocation.Subquery;
                            context.ModelType = rightQueryableField.Queryable.GetModelType();
                            FilterData(dataOptions, context);
                        }
                    }
                }
            }

            #endregion

            #region join

            if (!originalQueryable.Joins.IsNullOrEmpty())
            {
                foreach (var join in originalQueryable.Joins)
                {
                    if (join == null)
                    {
                        continue;
                    }
                    var joinTargetQueryable = join.Target;
                    if (joinTargetQueryable != null)
                    {
                        context.OriginalQueryable = joinTargetQueryable;
                        context.Location = QueryableLocation.JoinTarget;
                        context.ModelType = joinTargetQueryable.GetModelType();
                        FilterData(dataOptions, context);
                        if (joinTargetQueryable.FromType == QueryableFromType.Table
                            && !joinTargetQueryable.Criteria.IsNullOrEmpty())
                        {
                            var newJoinTargetQueryable = SixnetQuerier.Create()
                                .SetModelType(joinTargetQueryable.GetModelType())
                                .From(joinTargetQueryable);
                            join.Target = newJoinTargetQueryable;
                        }
                    }
                    if (join.Connection != null)
                    {
                        if (join.Connection is Criterion connectionCriterion)
                        {
                            if (connectionCriterion.Left is QueryableField leftQueryableField && leftQueryableField.Queryable != null)
                            {
                                context.OriginalQueryable = leftQueryableField.Queryable;
                                context.Location = QueryableLocation.JoinConnection;
                                context.ModelType = leftQueryableField.Queryable.GetModelType();
                                FilterData(dataOptions, context);
                            }
                            if (connectionCriterion.Right is QueryableField rightQueryableField && rightQueryableField.Queryable != null)
                            {
                                context.OriginalQueryable = rightQueryableField.Queryable;
                                context.Location = QueryableLocation.JoinConnection;
                                context.ModelType = rightQueryableField.Queryable.GetModelType();
                                FilterData(dataOptions, context);
                            }
                        }
                        if (join.Connection is ISixnetQueryable connectionQueryable)
                        {
                            context.OriginalQueryable = connectionQueryable;
                            context.Location = QueryableLocation.JoinConnection;
                            context.ModelType = connectionQueryable.GetModelType();
                            FilterData(dataOptions, context);
                        }
                    }
                }
            }

            #endregion

            #region tree

            var treeInfo = originalQueryable.TreeInfo;
            if (treeInfo != null)
            {
                if (treeInfo.DataField is ISixnetQueryable dataFieldQueryable)
                {
                    context.OriginalQueryable = dataFieldQueryable;
                    context.Location = QueryableLocation.TreeField;
                    context.ModelType = dataFieldQueryable.GetModelType();
                    FilterData(dataOptions, context);
                }
                if (treeInfo.ParentField is ISixnetQueryable parentFieldQueryable)
                {
                    context.OriginalQueryable = parentFieldQueryable;
                    context.Location = QueryableLocation.TreeField;
                    context.ModelType = parentFieldQueryable.GetModelType();
                    FilterData(dataOptions, context);
                }
            }

            #endregion

            #region combine

            if (!originalQueryable.Combines.IsNullOrEmpty())
            {
                foreach (var combine in originalQueryable.Combines)
                {
                    if (combine?.Target != null)
                    {
                        context.OriginalQueryable = combine.Target;
                        context.Location = QueryableLocation.Combine;
                        context.ModelType = combine.Target.GetModelType();
                        FilterData(dataOptions, context);
                    }
                }
            }

            #endregion

            #region from

            if (originalQueryable.FromType == QueryableFromType.Queryable
                && originalQueryable.TargetQueryable != null)
            {
                context.OriginalQueryable = originalQueryable.TargetQueryable;
                context.Location = QueryableLocation.From;
                context.ModelType = originalQueryable.TargetQueryable.GetModelType();
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
        internal static ISixnetCondition GetDataFilter(DataOptions options, QueryableFilterContext context)
        {
            SixnetDirectThrower.ThrowArgNullIf(context == null, nameof(context));

            if (context.OriginalQueryable == null)
            {
                context.OriginalQueryable = SixnetQuerier.Create();
                context.OriginalQueryable.SetModelType(context.ModelType);
            }
            var originalQueryable = context.OriginalQueryable;
            if (originalQueryable.IsGroupQueryable)
            {
                return null;
            }

            // Custom  filter
            var dataFilter = options?.GetCustomDataFilter?.Invoke(context);

            // Archived
            var ignoreArchived = originalQueryable.HasIgnoredFilter(FieldRole.Archive) || options.HasIgnoredRoleFilter(FieldRole.Archive);
            if (!ignoreArchived)
            {
                var inactiveFieldName = SixnetEntityManager.GetFieldName(context.ModelType, FieldRole.Archive);
                if (!string.IsNullOrWhiteSpace(inactiveFieldName))
                {
                    dataFilter ??= SixnetQuerier.Create().SetModelType(context.ModelType);
                    dataFilter = dataFilter.Where(Criterion.Create(CriterionOperator.Equal, DataField.Create(inactiveFieldName, context.ModelType), ConstantField.Create(false)));
                }
            }

            // Isolation
            var ignoreIsolation = originalQueryable.HasIgnoredFilter(FieldRole.Isolation) || options.HasIgnoredRoleFilter(FieldRole.Isolation);
            if (!ignoreIsolation)
            {
                var isolationFieldName = SixnetEntityManager.GetFieldName(context.ModelType, FieldRole.Isolation);
                if (!string.IsNullOrWhiteSpace(isolationFieldName))
                {
                    var isolationField = SixnetEntityManager.GetEntityConfig(context.ModelType).AllFields[isolationFieldName];
                    var isolationDataId = SessionContext.Current?.Isolation?.Id;

                    SixnetException.ThrowIf(string.IsNullOrWhiteSpace(isolationDataId), "Not set isolation value");

                    dataFilter ??= SixnetQuerier.Create().SetModelType(context.ModelType);
                    dataFilter = dataFilter.Where(Criterion.Create(CriterionOperator.Equal, DataField.Create(isolationFieldName, context.ModelType)
                        , ConstantField.Create(isolationDataId.ConvertTo(isolationField.DataType))));
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
