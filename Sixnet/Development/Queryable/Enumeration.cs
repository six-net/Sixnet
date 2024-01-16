using System;

namespace Sixnet.Development.Queryable
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
    /// Defines queryable execution mode
    /// </summary>
    [Serializable]
    public enum QueryableExecutionMode
    {
        Regular,
        Script
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
    /// Defines queryable location
    /// </summary>
    [Serializable]
    public enum QueryableLocation
    {
        PreScript = 0,
        Top = 2,
        From = 4,
        Subquery = 8,
        Combine = 16,
        JoinTarget = 32,
        JoinConnection = 64,
        Condition = 128,
        Having = 256,
        UsingSource = 512
    }

    /// <summary>
    /// Defines queryable usage scene
    /// </summary>
    [Serializable]
    public enum QueryableUsageScene
    {
        Remove = 2001,
        Modify = 2005,
        Query = 2010,
        Exists = 2015,
        Count = 2020,
        Max = 2025,
        Min = 2030,
        Sum = 2035,
        Avg = 2040,
        Scalar = 2045
    }

    /// <summary>
    /// Tree matching direction
    /// </summary>
    [Serializable]
    public enum TreeMatchingDirection
    {
        Up = 210,
        Down = 220
    }

    /// <summary>
    /// Defines queryable from type
    /// </summary>
    public enum QueryableFromType
    {
        Table = 310,
        Queryable = 320
    }

    /// <summary>
    /// Defines queryable output type
    /// </summary>
    public enum QueryableOutputType
    {
        Data = 2,
        Count = 4,
        Predicate = 8
    }
}
