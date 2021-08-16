namespace EZNEW.Development.Command
{
    /// <summary>
    /// Command executed event handler
    /// </summary>
    /// <param name="parameter">Command callback event parameter</param>
    /// <returns>Return command callback event result</returns>
    public delegate CommandCallbackEventExecutionResult CommandCallbackEventHandler(CommandCallbackEventParameter parameter);

    /// <summary>
    /// Command starting event handler
    /// </summary>
    /// <param name="parameter">Command starting event parameter</param>
    /// <returns>Return command starting event result</returns>
    public delegate CommandStartingEventExecutionResult CommandStartingEventHandler(CommandStartingEventParameter parameter);
}
