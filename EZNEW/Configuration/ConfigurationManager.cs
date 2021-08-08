using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using EZNEW.Application;
using EZNEW.Develop.CQuery;
using EZNEW.Develop.Entity;
using EZNEW.Logging;
using EZNEW.Mapper;

namespace EZNEW.Configuration
{
    /// <summary>
    /// Configuration core
    /// </summary>
    public partial class ConfigurationManager
    {
        /// <summary>
        /// Configuration setting
        /// </summary>
        public static readonly ConfigurationSetting Setting = new ConfigurationSetting();

        /// <summary>
        /// Convention file name patterns
        /// </summary>
        private static readonly List<string> ConventionFileNamePatterns = new List<string>()
        {
            @"\.Entity\.",
            @"\.QueryModel\.",
            @"\.ModuleConfig\.",
            @"\.Domain\.",
            @"\.DataAccess",
            @"\.CacheDataAccess\.",
            @"\.Business",
            @"\.Repository\.",
            @"\.Service\.",
            @"\.AppService",
            @"\.Domain\.",
            @"\.DTO\.",
            @"\.ViewModel\.",
            @"\.Module\.",
            @"\.ApiModel\."
        };

        /// <summary>
        /// Convention file match regex
        /// </summary>
        public static Regex ConventionFileMatchRegex => new Regex($@"^{ApplicationManager.ApplicationExecutableDirectory.Replace(@"\", @"\\")}.*({string.Join("|", ConventionFileNamePatterns.Union(Setting.AdditionalConventionFilePatterns))}).*$", RegexOptions.IgnoreCase);

        /// <summary>
        /// Configure application
        /// </summary>
        /// <param name="action"></param>
        public static void Configure(Action<ConfigurationSetting> action = null)
        {
            action?.Invoke(Setting);
            InternalConfigurationManager.Confirure();
        }

        /// <summary>
        /// Get the matched files
        /// </summary>
        /// <returns></returns>
        internal static IEnumerable<FileInfo> GetMatchedFiles()
        {
            return FilterFiles(new DirectoryInfo(ApplicationManager.ApplicationExecutableDirectory).GetFiles("*.dll", Setting.FileSearchOption));
        }

        /// <summary>
        /// Filter files
        /// </summary>
        /// <param name="originalFiles">Original files</param>
        /// <param name="setting">Configuration setting</param>
        /// <returns></returns>
        internal static IEnumerable<FileInfo> FilterFiles(IEnumerable<FileInfo> originalFiles, ConfigurationSetting setting = null)
        {
            if (originalFiles.IsNullOrEmpty())
            {
                return Array.Empty<FileInfo>();
            }
            setting ??= Setting;
            return originalFiles.Where(c =>
            {
                var matched = setting.FileMatchPattern switch
                {
                    FileMatchPattern.None => true,
                    FileMatchPattern.FileNamePrefix => setting.FileNameKeywords?.Any(rn => c.Name.StartsWith(rn, StringComparison.OrdinalIgnoreCase)) ?? false,
                    FileMatchPattern.FileNameSuffix => setting.FileNameKeywords?.Any(rn => c.Name.EndsWith(rn, StringComparison.OrdinalIgnoreCase)) ?? false,
                    FileMatchPattern.IncludeFileName => setting.FileNameKeywords?.Any(kw => c.Name.Contains(kw)) ?? false,
                    FileMatchPattern.ExcludeFileName => !(setting.FileNameKeywords?.Any(kw => c.Name.Contains(kw)) ?? false),
                    FileMatchPattern.IncludeByRegex => new Regex(setting.RegexExpression, RegexOptions.IgnoreCase).IsMatch(c.FullName),
                    FileMatchPattern.ExcludeByRegex => !new Regex(setting.RegexExpression, RegexOptions.IgnoreCase).IsMatch(c.FullName),
                    FileMatchPattern.Convention => ConventionFileMatchRegex.IsMatch(c.FullName),
                    _ => true
                };
                return matched;
            });
        }
    }

    internal class InternalConfigurationManager
    {
        /// <summary>
        /// Path directory separator
        /// </summary>
        internal static readonly string RegexPathDirectorySeparator = $"{Path.DirectorySeparatorChar}".Replace(@"\", @"\\");

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

        static InternalConfigurationManager()
        {
            var files = ConfigurationManager.GetMatchedFiles();
            IEnumerable<Type> allTypes = Array.Empty<Type>();
            foreach (var file in files)
            {
                try
                {
                    var types = Assembly.LoadFrom(file.FullName).GetTypes();
                    allTypes = allTypes.Union(types);
                    foreach (var type in types)
                    {
                        if (type.IsInterface)
                        {
                            continue;
                        }
                        bool isEntity = (type.GetCustomAttributes(typeof(EntityAttribute), false)?.FirstOrDefault()) is EntityAttribute entityAttribute;
                        if (isEntity)
                        {
                            ConfigurationManager.Entity.ConfigureEntity(type);
                        }
                        //query model
                        var queryModelInterface = type.GetInterface(queryModelGenericType.Name);
                        if (queryModelInterface != null && !isEntity)
                        {
                            ConfigurationManager.QueryModel.ConfigureQueryModelRelationEntity(type);
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

            //Object map
            if (ConfigurationManager.Setting.MapperBuilder != null)
            {
                ObjectMapper.Current = ConfigurationManager.Setting.MapperBuilder.CreateMapper(allTypes);
            }
        }

        /// <summary>
        /// Trigger configure
        /// </summary>
        internal static void Confirure() { }
    }
}
