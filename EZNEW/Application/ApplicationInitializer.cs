using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using EZNEW.Development.Entity;
using EZNEW.Development.Query;
using EZNEW.Diagnostics;
using EZNEW.Logging;
using EZNEW.Mapper;
using EZNEW.Module;

namespace EZNEW.Application
{
    /// <summary>
    /// Defines application initializer
    /// </summary>
    internal class ApplicationInitializer
    {
        static ApplicationInitializer()
        {
            InitApplication();
        }

        #region Fields

        /// <summary>
        /// Query model generic type
        /// </summary>
        static readonly Type queryModelGenericType = typeof(IQueryModel<>);

        /// <summary>
        /// Module configuration contract
        /// </summary>
        static readonly Type moduleConfigurationType = typeof(IModuleConfiguration);

        /// <summary>
        /// Convention file name patterns
        /// </summary>
        internal static readonly List<string> ConventionFileNamePatterns = new List<string>()
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
        internal static Regex ConventionFileMatchRegex = null;

        #endregion

        #region Methods

        /// <summary>
        /// Init application
        /// </summary>
        internal static void Init() { }

        /// <summary>
        /// Init application
        /// </summary>
        static void InitApplication()
        {
            try
            {
                IEnumerable<Type> allTypes = GetAllConventionTypes();
                if (allTypes.IsNullOrEmpty())
                {
                    return;
                }
                foreach (var type in allTypes)
                {
                    if (type.IsInterface)
                    {
                        continue;
                    }
                    //entity
                    bool isEntity = (type.GetCustomAttributes(typeof(EntityAttribute), false)?.FirstOrDefault()) is EntityAttribute entityAttribute;
                    if (isEntity)
                    {
                        EntityManager.ConfigureEntity(type);
                    }
                    //query model
                    var queryModelInterface = type.GetInterface(queryModelGenericType.Name);
                    if (queryModelInterface != null && !isEntity)
                    {
                        QueryManager.ConfigureQueryModelRelationEntity(type);
                    }
                    //module configuration
                    if (moduleConfigurationType.IsAssignableFrom(type))
                    {
                        IModuleConfiguration moduleConfiguration = Activator.CreateInstance(type) as IModuleConfiguration;
                        moduleConfiguration?.Init();
                    }
                }

                //object mapper
                ObjectMapper.BuildMapper();
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex, ex.Message);
            }
        }

        /// <summary>
        /// Get the matched files
        /// </summary>
        /// <returns></returns>
        internal static IEnumerable<FileInfo> GetMatchedFiles()
        {
            return FilterFiles(new DirectoryInfo(ApplicationManager.RootPath).GetFiles("*.dll", ApplicationManager.Options.FileMatchOptions.FileSearchOption));
        }

        /// <summary>
        /// Get all convention types
        /// </summary>
        /// <returns></returns>
        internal static IEnumerable<Type> GetAllConventionTypes()
        {
            var files = GetMatchedFiles();
            if (files.IsNullOrEmpty())
            {
                return Array.Empty<Type>();
            }
            IEnumerable<Type> allTypes = Array.Empty<Type>();
            foreach (var file in files)
            {
                try
                {
                    var types = Assembly.LoadFrom(file.FullName).GetTypes();
                    allTypes = allTypes.Union(types);
                }
                catch (Exception ex)
                {
                    LogManager.LogError(ex, ex.Message);
                }
            }
            return allTypes;
        }

        /// <summary>
        /// Filter files
        /// </summary>
        /// <param name="originalFiles">Original files</param>
        /// <param name="options">Configuration setting</param>
        /// <returns></returns>
        internal static IEnumerable<FileInfo> FilterFiles(IEnumerable<FileInfo> originalFiles, ApplicationOptions options = null)
        {
            if (originalFiles.IsNullOrEmpty())
            {
                return Array.Empty<FileInfo>();
            }
            options ??= ApplicationManager.Options;
            var fileOptions = options.FileMatchOptions;
            return originalFiles.Where(c =>
            {
                var matched = fileOptions.FileMatchPattern switch
                {
                    FileMatchPattern.None => true,
                    FileMatchPattern.FileNamePrefix => fileOptions.FileNameKeywords?.Any(rn => c.Name.StartsWith(rn, StringComparison.OrdinalIgnoreCase)) ?? false,
                    FileMatchPattern.FileNameSuffix => fileOptions.FileNameKeywords?.Any(rn => c.Name.EndsWith(rn, StringComparison.OrdinalIgnoreCase)) ?? false,
                    FileMatchPattern.IncludeFileName => fileOptions.FileNameKeywords?.Any(kw => c.Name.Contains(kw)) ?? false,
                    FileMatchPattern.ExcludeFileName => !(fileOptions.FileNameKeywords?.Any(kw => c.Name.Contains(kw)) ?? false),
                    FileMatchPattern.IncludeByRegex => new Regex(fileOptions.RegexExpression, RegexOptions.IgnoreCase).IsMatch(c.FullName),
                    FileMatchPattern.ExcludeByRegex => !new Regex(fileOptions.RegexExpression, RegexOptions.IgnoreCase).IsMatch(c.FullName),
                    FileMatchPattern.Convention => new Regex($@"^{ApplicationManager.RootPath.Replace(@"\", @"\\")}.*({string.Join("|", ConventionFileNamePatterns.Union(ApplicationManager.Options.FileMatchOptions.AdditionalConventionFilePatterns ?? new List<string>(0)))}).*$", RegexOptions.IgnoreCase).IsMatch(c.FullName),
                    _ => true
                };
                return matched;
            });
        }

        #endregion
    }
}
