using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Command
{
    /// <summary>
    /// command callback operation
    /// </summary>
    /// <param name="parameter">parameter</param>
    /// <returns>response</returns>
    public delegate CommandCallbackResult CommandCallbackOperation(CommandCallbackParameter parameter);

    /// <summary>
    /// command before execute operation
    /// </summary>
    /// <param name="parameter">parameter</param>
    public delegate CommandBeforeExecuteResult CommandBeforeExecuteOperation(CommandBeforeExecuteParameter parameter);
}
