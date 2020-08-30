using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using EZNEW.Application;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.Entity;
using EZNEW.Logging;

namespace EZNEW.Configuration
{
    /// <summary>
    /// Configuration core
    /// </summary>
    public partial class ConfigurationManager
    {
        static readonly Type booleanType = typeof(bool);

        /// <summary>
        /// Convention file names
        /// </summary>
        static readonly List<string> conventionFileNames = new List<string>()
        {
            ".Entity.",
            ".QueryModel.",
            ".ModuleConfig."
        };

        /// <summary>
        /// Entity generic type
        /// </summary>
        static readonly Type entityGenericType = typeof(BaseEntity<>);

        /// <summary>
        /// Query model generic type
        /// </summary>
        static readonly Type queryModelGenericType = typeof(IQueryModel<>);

        /// <summary>
        /// Module configuration contract
        /// </summary>
        static readonly Type moduleConfigurationType = typeof(IModuleConfiguration);

        static ConfigurationManager()
        {
            var files = new DirectoryInfo(ApplicationManager.ApplicationExecutableDirectory).GetFiles("*.dll", SearchOption.AllDirectories).Where(c =>
             {
                 var matched = ConfigurationOptions.DefaultConfigureFileMatchPattern switch
                 {
                     DefaultConfigureFileMatchPattern.None => true,
                     DefaultConfigureFileMatchPattern.Default => !ConfigurationOptions.ConfigurationExcludeFileRegex.IsMatch(c.FullName),
                     DefaultConfigureFileMatchPattern.Convention => conventionFileNames.Any(kw => c.Name.Contains(kw)),
                     DefaultConfigureFileMatchPattern.IncludeFileName => ConfigurationOptions.DefaultConfigureFileNameMatchKeyWords?.Any(kw => c.Name.Contains(kw)) ?? false,
                     DefaultConfigureFileMatchPattern.ExcludeFileName => !(ConfigurationOptions.DefaultConfigureFileNameMatchKeyWords?.Any(kw => c.Name.Contains(kw)) ?? false),
                     _ => true
                 };
                 return matched;
             });
            List<Type> allTypes = new List<Type>();
            foreach (var file in files)
            {
                try
                {
                    var types = Assembly.LoadFrom(file.FullName).GetTypes();
                    foreach (var type in types)
                    {
                        if (type.IsInterface)
                        {
                            continue;
                        }
                        var baseType = type.BaseType;
                        bool isEntity = false;
                        if (baseType != null && baseType.IsGenericType)
                        {
                            //Entity
                            var genericType = baseType.GetGenericTypeDefinition();
                            if (genericType == entityGenericType)
                            {
                                isEntity = true;
                                Entity.ConfigureEntity(type);
                            }
                        }
                        //query model
                        var queryModelInterface = type.GetInterface(queryModelGenericType.Name);
                        if (queryModelInterface != null && !isEntity)
                        {
                            QueryModel.ConfigureQueryModelRelationEntity(type);
                        }
                        //module configuration
                        if (moduleConfigurationType.IsAssignableFrom(type))
                        {
                            IModuleConfiguration moduleConfiguration = Activator.CreateInstance(type) as IModuleConfiguration;
                            moduleConfiguration?.Init();
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManager.LogError(ex, ex.Message);
                }
            }
        }

        /// <summary>
        /// Just trigger configure
        /// </summary>
        public static void Init() { }
    }
}
