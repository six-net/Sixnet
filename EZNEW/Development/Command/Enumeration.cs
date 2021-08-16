using System;

namespace EZNEW.Development.Command
{
    /// <summary>
    /// Defines command operate type
    /// </summary>
    [Serializable]
    public enum CommandOperationType
    {
        Insert = 410,
        Update = 420,
        Delete = 430,
        Query = 440,
        Exist = 450,
        Max = 460,
        Min = 470,
        Sum = 480,
        Avg = 490,
        Count = 500
    }

    /// <summary>
    /// Defines command execution mode
    /// </summary>
    [Serializable]
    public enum CommandExecutionMode
    {
        CommandText,
        Transform
    }

    /// <summary>
    /// Defines command text type
    /// </summary>
    [Serializable]
    public enum CommandTextType
    {
        Text = 210,
        Procedure = 220
    }

    /// <summary>
    /// Defines command execution result type
    /// </summary>
    [Serializable]
    public enum CommandResultType
    {
        AffectedRows = 310,
        ScalarValue = 320
    }

    /// <summary>
    /// Defines command behavior
    /// </summary>
    [Serializable]
    public enum CommandBehavior
    {
        Add = 110,
        Update = 120,
        UpdateByQuery = 130,
        UpdateObjectType = 135,
        RemoveByQuery = 140,
        RemoveObjectType = 150,
        RemoveData = 160
    }
}
