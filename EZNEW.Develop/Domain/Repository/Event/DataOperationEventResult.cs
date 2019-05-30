using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.Domain.Repository.Event
{
    public class DataOperationEventResult : IRepositoryEventHandleResult
    {
        public static readonly DataOperationEventResult Empty = new DataOperationEventResult();
    }
}
