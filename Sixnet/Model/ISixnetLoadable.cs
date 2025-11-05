// "Company © 2025. All rights reserved."

namespace Sixnet.Model
{
    /// <summary>
    /// Loadable model
    /// </summary>
    public interface ISixnetLoadable
    {
        /// <summary>
        /// Whether allow load data
        /// </summary>
        /// <param name="dataName">Data name</param>
        /// <returns></returns>
        bool Allow(string dataName);

        /// <summary>
        /// Set need to load datas
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
