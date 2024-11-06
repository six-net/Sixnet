namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Defines bulk insertion options
    /// </summary>
    public interface ISixnetBulkInsertionOptions 
    {
        /// <summary>
        /// Gets or sets the data operation options
        /// </summary>
        SixnetDataOperationOptions DataOperationOptions { get; set; }
    }
}
