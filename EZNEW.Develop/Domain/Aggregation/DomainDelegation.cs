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
    public delegate void DataOperation<T>(IEnumerable<T> datas);

    /// <summary>
    /// operation by condition
    /// </summary>
    /// <param name="query"></param>
    /// <param name="activationRecord"></param>
    public delegate void ConditionOperation(IQuery query);

    /// <summary>
    /// modify data operation
    /// </summary>
    /// <param name="modify"></param>
    /// <param name="query"></param>
    /// <param name="activationRecord"></param>
    public delegate void ModifyOperation(IModify modify, IQuery query);

    /// <summary>
    /// query data delegate
    /// </summary>
    /// <param name="datas">datas</param>
    /// <returns></returns>
    public delegate List<T> QueryData<T>(IEnumerable<T> datas);

    #endregion
}
