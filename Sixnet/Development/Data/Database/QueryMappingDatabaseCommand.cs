using System;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Defines base database query mapping command
    /// </summary>
    public abstract class BaseDatabaseQueryMappingCommand : SingleDatabaseCommand
    {
        /// <summary>
        /// Gets or sets the split on field name
        /// </summary>
        public string SpiltOnFieldName { get; set; } = "Id";
    }

    /// <summary>
    /// Defines database query mapping command
    /// </summary>
    public class QueryMappingDatabaseCommand<TFirst, TSecond, TReturn> : BaseDatabaseQueryMappingCommand
    {
        /// <summary>
        /// Gets or sets the data mapping func
        /// </summary>
        public Func<TFirst, TSecond, TReturn> DataMappingFunc { get; set; }
    }

    /// <summary>
    /// Defines database query mapping command
    /// </summary>
    public class DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TReturn> : BaseDatabaseQueryMappingCommand
    {
        /// <summary>
        /// Gets or sets the data mapping func
        /// </summary>
        public Func<TFirst, TSecond, TThird, TReturn> DataMappingFunc { get; set; }
    }

    /// <summary>
    /// Defines database query mapping command
    /// </summary>
    public class DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TFourth, TReturn> : BaseDatabaseQueryMappingCommand
    {
        /// <summary>
        /// Gets or sets the data mapping func
        /// </summary>
        public Func<TFirst, TSecond, TThird, TFourth, TReturn> DataMappingFunc { get; set; }
    }

    /// <summary>
    /// Defines database query mapping command
    /// </summary>
    public class DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> : BaseDatabaseQueryMappingCommand
    {
        /// <summary>
        /// Gets or sets the data mapping func
        /// </summary>
        public Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> DataMappingFunc { get; set; }
    }

    /// <summary>
    /// Defines database query mapping command
    /// </summary>
    public class DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> : BaseDatabaseQueryMappingCommand
    {
        /// <summary>
        /// Gets or sets the data mapping func
        /// </summary>
        public Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> DataMappingFunc { get; set; }
    }

    /// <summary>
    /// Defines database query mapping command
    /// </summary>
    public class DatabaseQueryMappingCommand<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> : BaseDatabaseQueryMappingCommand
    {
        /// <summary>
        /// Gets or sets the data mapping func
        /// </summary>
        public Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> DataMappingFunc { get; set; }
    }
}
