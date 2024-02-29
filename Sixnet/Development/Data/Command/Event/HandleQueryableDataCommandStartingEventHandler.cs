using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Sixnet.Development.Data;
using Sixnet.Development.Data.Field;
using Sixnet.Development.Entity;
using Sixnet.Development.Queryable;

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

            // clone queryable object
            var newQueryable = queryable?.Clone();
            if (newQueryable == null)
            {
                newQueryable = SixnetQueryable.Create();
                newQueryable.SetModelType(entityType);
            }

            // version condition for update
            if (operationType == DataOperationType.Update && !oldValues.IsNullOrEmpty())
            {
                var versionFieldName = SixnetEntityManager.GetRoleFieldName(entityType, FieldRole.Revision);
                if (!string.IsNullOrWhiteSpace(versionFieldName) && oldValues.ContainsKey(versionFieldName))
                {
                    var versionValue = oldValues[versionFieldName];
                    var versionCriterion = Criterion.Create(CriterionOperator.Equal, PropertyField.Create(versionFieldName, entityType), ConstantField.Create(versionValue));
                    newQueryable = newQueryable.Where(versionCriterion);
                }
            }

            // set global condition
            newQueryable = SixnetQueryable.SetGlobalFilter(entityType, newQueryable, dataCommand.OperationType);
            dataCommand.Queryable = newQueryable;
        }
    }
}
