using System;

namespace EZNEW.Development.Query
{
    /// <summary>
    /// Query model
    /// </summary>
    public interface IQueryModel<T> where T : IQueryModel<T>
    {
    }
}
