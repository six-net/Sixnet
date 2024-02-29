using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sixnet.Development.Data.Database;
using Sixnet.Development.Queryable;
using Sixnet.Exceptions;

namespace Sixnet.Development.Data.Command
{
    /// <summary>
    /// Defines data command resolve context
    /// </summary>
    public class DataCommandResolveContext
    {
        #region Fields

        const string PreTableNamePrefix = "PreTable";
        const string PreTablePetNamePrefix = "PT";
        int preTableNameSequence = 0;
        int subqueryTableNameSequence = 0;
        int parameterSequence = 0;
        LinkedList<string> preScripts;
        string latestPreTableName;
        string latestPreTablePetName;
        DataCommandParameters parameters;

        #endregion

        #region Properties

        /// <summary>
        /// Data access context
        /// </summary>
        public DataCommandExecutionContext DataCommandExecutionContext { get; private set; }

        /// <summary>
        /// Indecates whether use for execution
        /// </summary>
        public bool UseForExecution { get; set; } = false;

        /// <summary>
        /// Table pet name dict
        /// </summary>
        Dictionary<Guid, Dictionary<string, string>> tablePetNameDict;

        const string DefaultTablePetName = "TB";

        const string DefaultParameterName = "Param";

        #endregion

        #region Constructor

        public DataCommandResolveContext(SixnetDatabaseConnection connection, SixnetDataCommand command)
        {
            DataCommandExecutionContext = DataCommandExecutionContext.Create(connection, command);
        }

        #endregion

        #region Methods

        #region Pre script

        /// <summary>
        /// Get pre scripts
        /// </summary>
        /// <returns></returns>
        public LinkedList<string> GetPreScripts()
        {
            return preScripts;
        }

        /// <summary>
        /// Add pre script
        /// </summary>
        /// <param name="preScript">Pre script</param>
        /// <param name="preTableName">Pre table</param>
        /// <param name="preTablePetName">Pre pet name</param>
        public void AddPreScript(string preScript, string preTableName = "", string preTablePetName = "", bool addLast = true)
        {
            if (string.IsNullOrWhiteSpace(preScript))
            {
                return;
            }
            preScripts ??= new LinkedList<string>();
            if (!addLast)
            {
                preScripts.AddFirst(preScript);
                if (!string.IsNullOrWhiteSpace(preTableName))
                {
                    latestPreTableName = preTableName;
                }
                if (!string.IsNullOrWhiteSpace(preTablePetName))
                {
                    latestPreTablePetName = preTablePetName;
                }
            }
            else
            {
                preScripts.AddLast(preScript);
            }
        }

        /// <summary>
        /// Get new pre table name and pet name
        /// </summary>
        /// <returns>Return table name and pet name</returns>
        public (string preScriptTableName, string preScriptPetName) GetPreTableName()
        {
            var preTableIndex = preTableNameSequence++.ToString();
            return ($"{PreTableNamePrefix}{preTableIndex}", $"{PreTablePetNamePrefix}{preTableIndex}");
        }

        #endregion

        #region Pet name

        /// <summary>
        /// Get a new subquery table pet name
        /// </summary>
        /// <returns></returns>
        public string GetNewTablePetName()
        {
            return $"TB{subqueryTableNameSequence++}";
        }

        /// <summary>
        /// Get table pet name
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="entityType">Entity type</param>
        /// <returns></returns>
        public string GetTablePetName(ISixnetQueryable queryable, Type entityType, int index = 0)
        {
            entityType ??= queryable.GetModelType();
            if (entityType == null)
            {
                throw new ArgumentNullException(nameof(entityType));
            }
            tablePetNameDict ??= new Dictionary<Guid, Dictionary<string, string>>();
            var queryableId = queryable.Id;
            var entityTypeId = $"{entityType.GUID}_{index}";
            if (!tablePetNameDict.TryGetValue(queryableId, out var queryablePetNameDict))
            {
                queryablePetNameDict ??= new Dictionary<string, string>();
                tablePetNameDict[queryableId] = queryablePetNameDict;
            }
            if (!queryablePetNameDict.TryGetValue(entityTypeId, out var petName))
            {
                petName = GetNewTablePetName();
                queryablePetNameDict[entityTypeId] = petName;
            }
            return petName;
        }

        /// <summary>
        /// Get default table pet name
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <returns></returns>
        public string GetDefaultTablePetName(ISixnetQueryable queryable)
        {
            return GetTablePetName(queryable, queryable.GetModelType(), 0);
        }

        public void InitQueryableTablePetName(ISixnetQueryable queryable)
        {
            if (queryable != null)
            {
                var queryableType = queryable.GetType();
                var topEntityType = queryable.GetModelType();
                SixnetDirectThrower.ThrowSixnetExceptionIf(topEntityType == null, "Queryable model type is null");

                tablePetNameDict ??= new Dictionary<Guid, Dictionary<string, string>>();
                var entityTablePetNameDict = new Dictionary<string, string>();
                entityTablePetNameDict[$"{topEntityType.GUID}_0"] = GetNewTablePetName();

                if (!queryable.Joins.IsNullOrEmpty())
                {
                    foreach (var joinEntry in queryable.Joins)
                    {
                        var joinTargetQueryable = joinEntry.TargetQueryable;
                        var joinModelType = joinTargetQueryable.GetModelType();
                        SixnetDirectThrower.ThrowSixnetExceptionIf(joinModelType == null, "Join queryable model type is null");
                        entityTablePetNameDict[$"{topEntityType.GUID}_{joinEntry.Index}"] = GetNewTablePetName();
                    }
                }
                tablePetNameDict[queryable.Id] = entityTablePetNameDict;
            }
        }

        #endregion

        #region Parameter

        /// <summary>
        /// Get current parameter sequence
        /// </summary>
        /// <returns>Return current parameter sequence</returns>
        public int GetParameterSequence()
        {
            return parameterSequence;
        }

        /// <summary>
        /// Increase parameter sequence
        /// </summary>
        /// <param name="increaseValue">Increase value</param>
        /// <returns>Return the latest parameter sequence</returns>
        public int IncreaseParameterSequence(int increaseValue)
        {
            parameterSequence += increaseValue;
            return parameterSequence;
        }

        /// <summary>
        /// Gets a new parameter name
        /// </summary>
        /// <returns></returns>
        public string GetParameterName(string parameterNameKeyword)
        {
            if (string.IsNullOrWhiteSpace(parameterNameKeyword))
            {
                parameterNameKeyword = DefaultParameterName;
            }
            return $"{parameterNameKeyword}{parameterSequence++}";
        }

        /// <summary>
        /// Get command parameters
        /// </summary>
        /// <returns></returns>
        public DataCommandParameters GetParameters()
        {
            return parameters;
        }

        /// <summary>
        /// Add parameter
        /// </summary>
        /// <param name="parameterNameKeyword">Parameter name keyword</param>
        /// <param name="value">Parameter value</param>
        /// <returns>Return the real parameter name</returns>
        public string AddParameterByKeyword(string parameterNameKeyword, dynamic value)
        {
            var parameterName = GetParameterName(parameterNameKeyword);
            parameters ??= new DataCommandParameters();
            parameters.Add(parameterName, value);
            return parameterName;
        }

        /// <summary>
        /// Add output parameter
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="dbType"></param>
        public void AddOutputParameter(string parameterName, DbType? dbType = null)
        {
            parameters ??= new DataCommandParameters();
            parameters.Add(parameterName, null, dbType, ParameterDirection.Output);
        }

        /// <summary>
        /// Set parameters
        /// </summary>
        /// <param name="newParameters">New parameters</param>
        public void SetParameters(DataCommandParameters newParameters)
        {
            if (newParameters == null)
            {
                parameters.Clear();
            }
            else
            {
                parameters = newParameters;
            }
        }

        /// <summary>
        /// Clear parameters
        /// </summary>
        public void ClearParameters()
        {
            parameters = null;
        }

        /// <summary>
        /// Clear sequence
        /// </summary>
        public void ClearSequence()
        {
            preTableNameSequence = subqueryTableNameSequence = parameterSequence = 0;
        }

        #endregion

        #region Command

        /// <summary>
        /// Set command
        /// </summary>
        /// <param name="command">Command</param>
        public void SetCommand(SixnetDataCommand command)
        {
            DataCommandExecutionContext?.SetCommand(command);
        }

        #endregion

        #region Activity queryable

        /// <summary>
        /// Set activity queryable
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="location">Location</param>
        public void SetActivityQueryable(ISixnetQueryable queryable, QueryableLocation location)
        {
            DataCommandExecutionContext.SetActivityQueryable(queryable, location);
        }

        #endregion

        #region Table name

        /// <summary>
        /// Get join table name
        /// </summary>
        /// <param name="joinQueryable">Join queryable</param>
        /// <returns>Return table name</returns>
        public string GetJoinTableName(ISixnetQueryable joinQueryable)
        {
            return GetTableNames(joinQueryable, QueryableLocation.JoinTarget)?.FirstOrDefault();
        }

        /// <summary>
        /// Get table name
        /// </summary>
        /// <param name="activityQueryable">Activity queryable</param>
        /// <param name="queryableLocation">Activity queryable location</param>
        /// <returns>Return table name</returns>
        public List<string> GetTableNames(ISixnetQueryable activityQueryable, QueryableLocation queryableLocation)
        {
            return DataCommandExecutionContext.GetTableNames(activityQueryable, queryableLocation);
        }

        #endregion

        #region Reset

        /// <summary>
        /// Reset context
        /// </summary>
        public void Reset()
        {
            preScripts?.Clear();
            latestPreTableName = latestPreTablePetName = string.Empty;
            ClearParameters();
            ClearSequence();
        }

        #endregion

        #region Root query

        /// <summary>
        /// Check whether is root queryable
        /// </summary>
        /// <param name="queryable"></param>
        /// <returns></returns>
        public bool IsRootQueryable(ISixnetQueryable queryable)
        {
            return DataCommandExecutionContext?.Command?.Queryable == queryable;
        }

        #endregion

        #endregion
    }
}
