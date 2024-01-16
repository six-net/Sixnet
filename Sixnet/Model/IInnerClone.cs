namespace Sixnet.Model
{
    /// <summary>
    /// Defines inner clone contract
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IInnerClone<T>
    {
        /// <summary>
        /// Clone an object
        /// </summary>
        /// <returns></returns>
        T Clone();
    }
}
