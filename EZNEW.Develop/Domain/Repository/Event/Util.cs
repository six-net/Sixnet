using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.Domain.Repository.Event
{
    /// <summary>
    /// event type
    /// </summary>
    public enum EventType
    {
        SaveObject = 2,
        RemoveObject = 4,
        RemoveByCondition = 8,
        QueryData = 16,
        ModifyExpression = 32
    }

    /// <summary>
    /// event callback
    /// </summary>
    /// <param name="result">result</param>
    public delegate void RepositoryEventCallback(IRepositoryEventHandleResult result);
}
