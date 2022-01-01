using System;

namespace EZNEW.Development.UnitOfWork
{
    /// <summary>
    /// Defines activation operation
    /// </summary>
    [Flags]
    [Serializable]
    public enum ActivationOperation
    {
        Package = 0,
        SaveObject = 2,
        ModifyByExpression = 4,
        RemoveObject = 8,
        RemoveByCondition = 16
    }
}
