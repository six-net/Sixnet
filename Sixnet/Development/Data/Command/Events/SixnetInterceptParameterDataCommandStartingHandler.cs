// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Intercept;

namespace Sixnet.Development.Data.Command.Events
{
    /// <summary>
    /// Intercept parameter starting handler
    /// </summary>
    internal class SixnetInterceptParameterDataCommandStartingHandler : ISixnetDataCommandStartingEventHandler
    {
        public void Handle(SixnetDataCommandStartingEvent dataCommandStartingEvent)
        {
            var dataCommand = dataCommandStartingEvent.Command;
            var entityType = dataCommand.GetEntityType();
            if (entityType != null)
            {
                var operationType = dataCommand.OperationType;
                if (operationType == SixnetDataOperationType.Insert || operationType == SixnetDataOperationType.Update)
                {
                    SixnetDataInterceptor.InterceptData(dataCommand);
                }
            }
        }
    }
}
