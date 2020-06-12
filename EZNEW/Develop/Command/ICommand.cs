using System;
using System.Collections.Generic;
using EZNEW.Develop.CQuery;

namespace EZNEW.Develop.Command
{
    /// <summary>
    /// Execute command contract
    /// </summary>
    public interface ICommand
    {
        #region Properties

        /// <summary>
        /// Gets the command Id
        /// </summary>
        long Id { get; }

        /// <summary>
        /// Gets or sets the command text
        /// </summary>
        string CommandText { get; set; }

        /// <summary>
        /// Gets or sets the parameters
        /// </summary>
        dynamic Parameters { get; set; }

        /// <summary>
        /// Gets or sets the object name
        /// </summary>
        string ObjectName { get; set; }

        /// <summary>
        /// Gets or sets the object keys
        /// </summary>
        List<string> ObjectKeys { get; set; }

        /// <summary>
        /// Gets or sets the object key values
        /// </summary>
        Dictionary<string, dynamic> ObjectKeyValues { get; set; }

        /// <summary>
        /// Gets or sets server keys
        /// </summary>
        List<string> ServerKeys { get; set; }

        /// <summary>
        /// Gets or sets the server key values
        /// </summary>
        Dictionary<string, dynamic> ServerKeyValues { get; set; }

        /// <summary>
        /// Gets or sets the command execute mode
        /// </summary>
        CommandExecuteMode ExecuteMode { get; set; }

        /// <summary>
        /// Gets or sets the query object
        /// </summary>
        IQuery Query { get; set; }

        /// <summary>
        /// Gets or sets the operate type
        /// </summary>
        OperateType OperateType { get; set; }

        /// <summary>
        /// Gets or sets the fields
        /// </summary>
        List<string> Fields { get; set; }

        /// <summary>
        /// Whether the command is obsolete
        /// </summary>
        bool IsObsolete { get; }

        /// <summary>
        /// Gets or sets the entity type
        /// </summary>
        Type EntityType { get; set; }

        /// <summary>
        /// Gets or sets whether must return value on successful
        /// </summary>
        bool MustReturnValueOnSuccess { get; set; }

        #endregion

        #region Methods

        #region Command starting event

        #region Listen command starting

        /// <summary>
        /// Listen command starting event
        /// </summary>
        /// <param name="eventHandler">Event handler</param>
        /// <param name="eventParameter">Event parameter</param>
        /// <param name="async">Whether execute event handler by async</param>
        void ListenStarting(CommandStartingEventHandler eventHandler, CommandStartingEventParameter eventParameter, bool async = false);

        #endregion

        #region Trigger command starting event

        /// <summary>
        /// Trigger command executing event
        /// </summary>
        /// <returns>Return command starting event result</returns>
        CommandStartingEventExecuteResult TriggerStartingEvent();

        #endregion

        #endregion

        #region Command callback event

        #region Listen command callback event

        /// <summary>
        /// Listen command callback event
        /// </summary>
        /// <param name="eventHandler">Event handler</param>
        /// <param name="eventParameter">Event parameter</param>
        void ListenCallback(CommandCallbackEventHandler eventHandler, CommandCallbackEventParameter eventParameter);

        #endregion

        #region Trigger command callback event

        /// <summary>
        /// Trigger command callback event
        /// </summary>
        /// <param name="success">Whether command has executed successful</param>
        /// <returns>Return callback event result</returns>
        void TriggerCallbackEvent(bool success);

        #endregion

        #endregion

        #endregion
    }
}
