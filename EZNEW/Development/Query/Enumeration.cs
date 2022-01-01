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
    /// Defines condition connection operator
    /// </summary>
    [Serializable]
    public enum ConditionConnectionOperator
    {
        AND,
        OR
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

    /// <summary>
    /// Defines join operator
    /// </summary>
    [Serializable]
    public enum JoinOperator
    {
        Equal,              //=  
        NotEqual,      //<>  
        LessThanOrEqual,    //<=  
        LessThan,           //<  
        GreaterThan,        //>  
        GreaterThanOrEqual //>=
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
        Combine = 16
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
