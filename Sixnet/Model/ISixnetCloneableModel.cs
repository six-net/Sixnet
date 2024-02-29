namespace Sixnet.Model
{
    /// <summary>
    /// Cloneable model
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISixnetCloneableModel<T>
    {
        /// <summary>
        /// Clone an object
        /// </summary>
        /// <returns></returns>
        T Clone();
    }
}
