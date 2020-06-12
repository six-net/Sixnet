using System;

namespace EZNEW.Develop.Command
{
    /// <summary>
    /// Defines command operate type
    /// </summary>
    [Serializable]
    public enum OperateType
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
    /// Defines command execute mode
    /// </summary>
    [Serializable]
    public enum CommandExecuteMode
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
    /// Defines command execute result
    /// </summary>
    [Serializable]
    public enum ExecuteCommandResult
    {
        ExecuteRows = 310,
        ExecuteScalar = 320
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
