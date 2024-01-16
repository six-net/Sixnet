using System.Threading.Tasks;
using Sixnet.Development.Data;
using Sixnet.Development.Data.Interceptor;

namespace Sixnet.Development.Data.Command.Event
{
    /// <summary>
    /// Intercept parameter starting handler
    /// </summary>
    internal class InterceptDataCommandParameterStartingHandler : IDataCommandStartingEventHandler
    {
        public void Handle(DataCommandStartingEvent dataCommandStartingEvent)
        {
            var dataCommand = dataCommandStartingEvent.Command;
            var query = dataCommand.Queryable;
            var entityType = dataCommand.GetEntityType();
            if (entityType != null)
            {
                var operationType = dataCommand.OperationType;
                if (operationType == DataOperationType.Insert || operationType == DataOperationType.Update)
                {
                    InterceptDataManager.InterceptData(dataCommand);
                }
            }
        }
    }
}
