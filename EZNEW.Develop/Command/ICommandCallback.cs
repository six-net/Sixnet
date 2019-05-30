using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Command
{
    /// <summary>
    /// Command Callback
    /// </summary>
    /// <param name="request">request</param>
    /// <returns>response</returns>
    public delegate Task<CommandCallbackResponse> ExecuteCommandCallback(CommandCallbackRequest request);

    /// <summary>
    /// Command Before Execute
    /// </summary>
    /// <param name="request">request</param>
    public delegate Task<bool> BeforeExecute(BeforeExecuteRequest request);
}
