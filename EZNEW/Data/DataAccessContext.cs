using System;
using System.Collections.Generic;
using System.Text;
using EZNEW.Development.Command;
using EZNEW.Development.Query;
using EZNEW.Exceptions;

namespace EZNEW.Data
{
    /// <summary>
    /// Defines data access context
    /// </summary>
    public class DataAccessContext
    {
        /// <summary>
        /// Gets or sets the server
        /// </summary>
        public DatabaseServer Server { get; set; }

        /// <summary>
        /// Gets or sets the execution command
        /// </summary>
        public ICommand Command { get; private set; }

        /// <summary>
        /// Gets or sets the activity query
        /// </summary>
        public IQuery ActivityQuery { get; set; }

        /// <summary>
        /// Gets or sets the activity query location
        /// </summary>
        public QueryLocation ActivityQueryLocation { get; set; }

        /// <summary>
        /// Get subquery entity object name
        /// </summary>
        /// <param name="subquery">Subquery</param>
        /// <param name="defaultObjectName">Default object name</param>
        /// <returns>Return entity object name</returns>
        public string GetSubqueryEntityObjectName(IQuery subquery, string defaultObjectName = "")
        {
            return GetEntityObjectName(subquery, QueryLocation.Subuery, defaultObjectName);
        }

        /// <summary>
        /// Get join entity object name
        /// </summary>
        /// <param name="joinQuery">Join query</param>
        /// <param name="defaultObjectName">Default object name</param>
        /// <returns>Return entity object name</returns>
        public string GetJoinEntityObjectName(IQuery joinQuery, string defaultObjectName = "")
        {
            return GetEntityObjectName(joinQuery, QueryLocation.Join, defaultObjectName);
        }

        /// <summary>
        /// Get combine entity object name
        /// </summary>
        /// <param name="combineQuery">Combine query</param>
        /// <param name="defaultObjectName">Default object name</param>
        /// <returns>Return entity object name</returns>
        public string GetCombineEntityObjectName(IQuery combineQuery, string defaultObjectName = "")
        {
            return GetEntityObjectName(combineQuery, QueryLocation.Combine, defaultObjectName);
        }

        /// <summary>
        /// Set activity query
        /// </summary>
        /// <param name="query">Query object</param>
        /// <param name="location">Location</param>
        public void SetActivityQuery(IQuery query, QueryLocation location)
        {
            ActivityQuery = query;
            ActivityQueryLocation = location;
        }

        /// <summary>
        /// Create data access context
        /// </summary>
        /// <param name="server">Database server</param>
        /// <param name="command">Command</param>
        /// <returns></returns>
        public static DataAccessContext Create(DatabaseServer server, ICommand command = null)
        {
            return new DataAccessContext()
            {
                Server = server,
                Command = command,
                ActivityQuery = command?.Query,
                ActivityQueryLocation = QueryLocation.Top
            };
        }

        /// <summary>
        /// Get entity object name
        /// </summary>
        /// <param name="activityQuery">Activity query</param>
        /// <param name="activityQuerylocation">Activity query location</param>
        /// <param name="defaultObjectName">Default object name</param>
        /// <returns>Return entity object name</returns>
        public string GetEntityObjectName(IQuery activityQuery, QueryLocation activityQuerylocation, string defaultObjectName = "")
        {
            if (Command is null)
            {
                throw new EZNEWException($"Not set command");
            }
            SetActivityQuery(activityQuery, activityQuerylocation);
            return DataManager.GetEntityObjectName(this, defaultObjectName);
        }

        /// <summary>
        /// Get command entity object name
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Return entity object name</returns>
        public string GetCommandEntityObjectName(ICommand command)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            SetCommand(command);
            return GetEntityObjectName(command.Query, QueryLocation.Top, command.EntityObjectName);
        }

        /// <summary>
        /// Set command
        /// </summary>
        /// <param name="command">Command</param>
        public void SetCommand(ICommand command)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            Command = command;
            SetActivityQuery(command.Query, QueryLocation.Top);
        }
    }
}
