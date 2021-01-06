using System.Collections.Generic;
using EZNEW.Develop.Command.Modify;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.UnitOfWork;

namespace EZNEW.Develop.Domain.Aggregation
{
    /// <summary>
    /// Data operation
    /// </summary>
    /// <typeparam name="T">Data type</typeparam>
    /// <param name="datas">Datas</param>
    /// <param name="options">Activation options</param>
    public delegate void DataOperation<T>(IEnumerable<T> datas, ActivationOptions options = null);

    /// <summary>
    /// Operation by condition
    /// </summary>
    /// <param name="query">Query object</param>
    /// <param name="options">Activation options</param>
    public delegate void ConditionOperation(IQuery query, ActivationOptions options = null);

    /// <summary>
    /// Modify data operation
    /// </summary>
    /// <param name="modify">Modify expression</param>
    /// <param name="query">Query object</param>
    /// <param name="options">Activation options</param>
    public delegate void ModifyOperation(IModify modify, IQuery query, ActivationOptions options = null);

    /// <summary>
    /// Query data operation
    /// </summary>
    /// <param name="datas">Datas</param>
    /// <returns></returns>
    public delegate List<T> QueryData<T>(IEnumerable<T> datas);
}
