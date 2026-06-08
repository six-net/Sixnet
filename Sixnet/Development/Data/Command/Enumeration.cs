// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Data.Command
{
    /// <summary>
    /// Defines command execution mode
    /// </summary>
    [Serializable]
    public enum SixnetCommandExecutionMode
    {
        /// <summary>
        /// Script
        /// </summary>
        Script = 110,
        /// <summary>
        /// Transform
        /// </summary>
        Transform = 120
    }

    /// <summary>
    /// Defines command execution result type
    /// </summary>
    [Serializable]
    public enum SixnetCommandResultType
    {
        /// <summary>
        /// Affected rows
        /// </summary>
        AffectedRows = 310,
        /// <summary>
        /// Scalar value
        /// </summary>
        ScalarValue = 320
    }

    /// <summary>
    /// Defines command behavior
    /// </summary>
    [Serializable]
    public enum SixnetCommandBehavior
    {
        /// <summary>
        /// Add
        /// </summary>
        Add = 110,
        /// <summary>
        /// Update
        /// </summary>
        Update = 120,
        /// <summary>
        /// Update by query
        /// </summary>
        UpdateByQuery = 130,
        /// <summary>
        /// Update object type
        /// </summary>
        UpdateObjectType = 135,
        /// <summary>
        /// Remove by query
        /// </summary>
        RemoveByQuery = 140,
        /// <summary>
        /// Remove object type
        /// </summary>
        RemoveObjectType = 150,
        /// <summary>
        /// Remove data
        /// </summary>
        RemoveData = 160
    }
}
