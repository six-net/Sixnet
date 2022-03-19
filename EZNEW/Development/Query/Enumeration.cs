using System;

namespace EZNEW.Development.Query
{
    /// <summary>
    /// Defines criterion operator
    /// </summary>
    [Serializable]
    public enum CriterionOperator
    {
        Equal,              //=  
        NotEqual,      //<>  
        LessThanOrEqual,    //<=  
        LessThan,           //<  
        GreaterThan,        //>  
        GreaterThanOrEqual, //>=  
        In,                 //IN()  
        NotIn,              //NOT IN ()  
        Like,
        NotLike,
        BeginLike,
        NotBeginLike,
        EndLike,
        NotEndLike,
        IsNull,
        NotNull,
        True,
        False
    }

    /// <summary>
    /// Defines criterion connector
    /// </summary>
    [Serializable]
    public enum CriterionConnector
    {
        And,
        Or
    }

    /// <summary>
    /// Defines query execution mode
    /// </summary>
    [Serializable]
    public enum QueryExecutionMode
    {
        QueryObject,
        Text
    }

    /// <summary>
    /// Defines join type
    /// </summary>
    [Serializable]
    public enum JoinType
    {
        InnerJoin = 2,
        LeftJoin = 4,
        RightJoin = 8,
        FullJoin = 16,
        CrossJoin = 32
    }

    ///// <summary>
    ///// Defines join operator
    ///// </summary>
    //[Serializable]
    //public enum JoinOperator
    //{
    //    Equal = 2,              //=  
    //    NotEqual = 4,      //<>  
    //    LessThanOrEqual = 8,    //<=  
    //    LessThan = 16,           //<  
    //    GreaterThan = 32,        //>  
    //    GreaterThanOrEqual = 64 //>=
    //}

    /// <summary>
    /// Defines join field type
    /// </summary>
    [Serializable]
    public enum JoinFieldType
    {
        Field = 2,
        Constant = 4,
        Condition = 8
    }

    /// <summary>
    /// Defines combine type
    /// </summary>
    [Serializable]
    public enum CombineType
    {
        UnionAll,
        Union,
        Except,
        Intersect
    }

    /// <summary>
    /// Defines query location
    /// </summary>
    [Serializable]
    public enum QueryLocation
    {
        Top = 2,
        Subuery = 4,
        Join = 8,
        Combine = 16,
        JoinConnection = 32
    }

    /// <summary>
    /// Defines query usage scene
    /// </summary>
    [Serializable]
    public enum QueryUsageScene
    {
        Remove = 2001,
        Modify = 2005,
        Query = 2010,
        Exists = 2015,
        Count = 2020,
        Max = 2025,
        Min = 2030,
        Sum = 2035,
        Avg = 2040
    }

    /// <summary>
    /// Defines recurve direction
    /// </summary>
    [Serializable]
    public enum RecurveDirection
    {
        Up = 210,
        Down = 220
    }
}
