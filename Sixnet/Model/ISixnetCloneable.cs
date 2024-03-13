namespace Sixnet.Model
{
    /// <summary>
    /// Cloneable model
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISixnetCloneable<T>
    {
        /// <summary>
        /// Clone an object
        /// </summary>
        /// <returns></returns>
        T Clone();
    }
}
