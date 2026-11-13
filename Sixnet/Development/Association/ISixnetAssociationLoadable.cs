// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Association
{
    /// <summary>
    /// Association loadable info
    /// </summary>
    public interface ISixnetAssociationLoadable
    {
        /// <summary>
        /// Whether allow load specific data
        /// </summary>
        /// <param name="dataName">Data name</param>
        /// <returns></returns>
        bool Allow(string dataName);

        /// <summary>
        /// Set need to load data names
        /// </summary>
        /// <param name="dataNames">Data names</param>
        /// <returns></returns>
        void Need(params string[] dataNames);

        /// <summary>
        /// Get data names
        /// </summary>
        /// <returns></returns>
        List<string> GetDataNames();
    }
}
