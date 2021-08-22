using System;

namespace EZNEW.Development.Query
{
    /// <summary>
    /// Query model
    /// </summary>
    public interface IQueryModel<in T> where T : IQueryModel<T>
    {
    }
}
