using Sixnet.Development.Data.Intercept;

namespace Sixnet.Development.Data.Command.Event
{
    /// <summary>
    /// Intercept parameter starting handler
    /// </summary>
    internal class InterceptParameterDataCommandStartingHandler : ISixnetDataCommandStartingEventHandler
    {
        public void Handle(SixnetDataCommandStartingEvent dataCommandStartingEvent)
        {
            var dataCommand = dataCommandStartingEvent.Command;
            var query = dataCommand.Queryable;
            var entityType = dataCommand.GetEntityType();
            if (entityType != null)
            {
                var operationType = dataCommand.OperationType;
                if (operationType == DataOperationType.Insert || operationType == DataOperationType.Update)
                {
                    SixnetDataInterceptor.InterceptData(dataCommand);
                }
            }
        }
    }
}
