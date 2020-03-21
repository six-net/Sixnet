using EZNEW.Develop.Command.Modify;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Domain.Aggregation
{
    #region normal

    /// <summary>
    /// data operation
    /// </summary>
    /// <typeparam name="T">DataType</typeparam>
    /// <param name="datas">datas</param>
    /// <param name="option">activation option</param>
    public delegate void DataOperation<T>(IEnumerable<T> datas, ActivationOption option = null);

    /// <summary>
    /// operation by condition
    /// </summary>
    /// <param name="query">query object</param>
    /// <param name="option">activation option</param>
    public delegate void ConditionOperation(IQuery query, ActivationOption option = null);

    /// <summary>
    /// modify data operation
    /// </summary>
    /// <param name="modify">modify expression</param>
    /// <param name="query">query object</param>
    /// <param name="option">activation option</param>
    public delegate void ModifyOperation(IModify modify, IQuery query, ActivationOption option = null);

    /// <summary>
    /// query data delegate
    /// </summary>
    /// <param name="datas">datas</param>
    /// <returns></returns>
    public delegate List<T> QueryData<T>(IEnumerable<T> datas);

    #endregion
}
