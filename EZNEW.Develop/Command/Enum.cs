using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Command
{
    /// <summary>
    /// Command Operate Type
    /// </summary>
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
    /// Command Execute Mode
    /// </summary>
    public enum CommandExecuteMode
    {
        CommandText,
        Transform
    }

    /// <summary>
    /// RdbCommand Text Type
    /// </summary>
    public enum RdbCommandTextType
    {
        Text = 210,
        Procedure = 220
    }

    /// <summary>
    /// Command Execute Result
    /// </summary>
    public enum ExecuteCommandResult
    {
        ExecuteRows = 310,
        ExecuteScalar = 320
    }

    /// <summary>
    /// Command Behavior
    /// </summary>
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
