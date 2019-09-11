using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.CQuery
{
    /// <summary>
    /// Condition Operator
    /// </summary>
    public enum CriteriaOperator
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
        NotEndLike
    }

    /// <summary>
    /// Connect Operator
    /// </summary>
    public enum QueryOperator
    {
        AND,
        OR
    }

    /// <summary>
    /// Query Command Type
    /// </summary>
    public enum QueryCommandType
    {
        QueryObject,
        Text
    }


    /// <summary>
    /// Calculate Operator
    /// </summary>
    public enum CalculateOperator
    {
        Add,
        subtract,
        multiply,
        divide
    }

    /// <summary>
    /// join type
    /// </summary>
    public enum JoinType
    {
        InnerJoin = 2,
        LeftJoin = 4,
        RightJoin = 8,
        FullJoin = 16,
        CrossJoin = 32
    }

    /// <summary>
    /// Join Operator
    /// </summary>
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
    /// condition source type
    /// </summary>
    public enum QuerySourceType
    {
        Repository = 2,
        Subuery = 4,
        JoinQuery = 8
    }

    /// <summary>
    /// query usage scene
    /// </summary>
    public enum QueryUsageScene
    {
        Remove = 2001,
        Modify = 2005,
        Query = 2010,
        Exist = 2015,
        Count = 2020,
        Max = 2025,
        Min = 2030,
        Sum = 2035,
        Avg = 2040
    }
}
