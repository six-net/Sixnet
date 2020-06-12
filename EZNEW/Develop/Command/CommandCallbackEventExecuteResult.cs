using System;

namespace EZNEW.Develop.Command
{
    /// <summary>
    /// Command executed event result
    /// </summary>
    [Serializable]
    public class CommandCallbackEventExecuteResult
    {
        public static readonly CommandCallbackEventExecuteResult Default = new CommandCallbackEventExecuteResult();
    }
}
