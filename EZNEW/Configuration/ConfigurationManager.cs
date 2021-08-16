using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using EZNEW.DependencyInjection;

namespace EZNEW.Configuration
{
    /// <summary>
    /// Configuration core
    /// </summary>
    public partial class ConfigurationManager
    {
        #region Get configuration

        /// <summary>
        /// Get configuration
        /// </summary>
        /// <returns>Return a IConfiguration object</returns>
        public static IConfiguration GetConfiguration()
        {
            return ContainerManager.Resolve<IConfiguration>();
        } 

        #endregion

        #region Get configuration section

        /// <summary>
        /// Get configuration section
        /// </summary>
        /// <param name="key">Section key</param>
        /// <returns>Return a IConfiguration section</returns>
        public static IConfigurationSection GetSection(string key)
        {
            return GetConfiguration()?.GetSection(key);
        }

        #endregion

        #region Get connection string

        /// <summary>
        /// Get connection string
        /// </summary>
        /// <param name="name">Connection name</param>
        /// <returns>Return a connection string</returns>
        public static string GetConnectionString(string name)
        {
            return GetConfiguration()?.GetConnectionString(name);
        }

        #endregion
    }
}
