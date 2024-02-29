using Microsoft.Extensions.Localization;

namespace System.Reflection
{
    /// <summary>
    /// Assembly extensions
    /// </summary>
    public static class AssemblyExtensions
    {
        #region Get root namespace

        /// <summary>
        /// Get root namespace
        /// </summary>
        /// <param name="assembly">Assembly</param>
        /// <returns></returns>
        public static string GetRootNamespace(this Assembly assembly)
        {
            var rootNamespaceAttribute = assembly.GetCustomAttribute<RootNamespaceAttribute>();
            return rootNamespaceAttribute?.RootNamespace ?? assembly.GetName().Name;
        } 

        #endregion
    }
}
