using System;
using System.Collections.Generic;
using EZNEW.Development.Query;

namespace EZNEW.Development.Command
{
    /// <summary>
    /// Defines command contract
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
        string Text { get; set; }

        /// <summary>
        /// Gets or sets the parameters
        /// </summary>
        CommandParameters Parameters { get; set; }

        /// <summary>
        /// Gets or sets the entity object name
        /// </summary>
        string EntityObjectName { get; set; }

        /// <summary>
        /// Gets or sets the entity identity values
        /// </summary>
        Dictionary<string, dynamic> EntityIdentityValues { get; set; }

        /// <summary>
        /// Gets or sets the command properties
        /// </summary>
        Dictionary<string, dynamic> Properties { get; set; }

        /// <summary>
        /// Gets or sets the command execution mode
        /// </summary>
        CommandExecutionMode ExecutionMode { get; set; }

        /// <summary>
        /// Gets or sets the query object
        /// </summary>
        IQuery Query { get; set; }

        /// <summary>
        /// Gets or sets the operate type
        /// </summary>
        CommandOperationType OperationType { get; set; }

        /// <summary>
        /// Gets or sets the fields
        /// </summary>
        IEnumerable<string> Fields { get; set; }

        /// <summary>
        /// Whether the command is obsolete
        /// </summary>
        bool IsObsolete { get; }

        /// <summary>
        /// Gets or sets the entity type
        /// </summary>
        Type EntityType { get; set; }

        /// <summary>
        /// Indicates whether must affect data
        /// </summary>
        bool MustAffectedData { get; set; }

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
        CommandStartingEventExecutionResult TriggerStartingEvent();

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

        #region Clone

        /// <summary>
        /// Clone a ICommand object
        /// </summary>
        /// <returns></returns>
        ICommand Clone();

        #endregion

        #endregion
    }
}
