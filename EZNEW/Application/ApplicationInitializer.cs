using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using EZNEW.Development.DataAccess;
using EZNEW.Development.Domain.Aggregation;
using EZNEW.Development.Domain.Repository;
using EZNEW.Development.Entity;
using EZNEW.Development.Query;
using EZNEW.Diagnostics;
using EZNEW.Logging;
using EZNEW.Mapper;
using EZNEW.Module;
using EZNEW.Reflection;

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
        /// Aggregation contract type
        /// </summary>
        static readonly Type aggreagtionContractType = typeof(IAggregationRoot);

        /// <summary>
        /// Entity contract type
        /// </summary>
        static readonly Type entityContractType = typeof(IEntity);

        /// <summary>
        /// Module configuration contract
        /// </summary>
        static readonly Type moduleConfigurationType = typeof(IModuleConfiguration);

        /// <summary>
        /// Convention file name patterns
        /// </summary>
        internal static readonly List<string> ConventionFileNamePatterns = new()
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
                var allTypeDict = GetAllConventionTypes()?.ToDictionary(c => c.FullName, c => c);
                if (allTypeDict.IsNullOrEmpty())
                {
                    return;
                }
                foreach (var type in allTypeDict.Values)
                {
                    if (type.IsInterface || type.IsAbstract)
                    {
                        continue;
                    }

                    bool isEntity = entityContractType.IsAssignableFrom(type);
                    var isAggreagtion = aggreagtionContractType.IsAssignableFrom(type);

                    #region Entity

                    if (isEntity)
                    {
                        EntityManager.ConfigureEntity(type);
                        if (isAggreagtion)
                        {
                            //default data access
                            DataAccessManager.RegisterEntityDefaultDataAccess(type);
                            //default repository
                            RepositoryManager.RegisterDefaultRepository(type);
                        }
                    }

                    #endregion

                    #region Aggregation model

                    if (isAggreagtion && !isEntity)
                    {
                        var entityType = AggregationManager.GetAggregationModelRelationEntityType(type);
                        if (entityType == null)
                        {
                            var namespaceArray = type.Assembly.FullName.LSplit(",")[0].LSplit(".");
                            string conventionEntityName = $"{namespaceArray[0]}.Entity.{namespaceArray[namespaceArray.Length - 1]}.{type.Name}Entity";
                            allTypeDict.TryGetValue(conventionEntityName, out entityType);
                        }
                        if (entityType != null)
                        {
                            QueryManager.ConfigureQueryModelRelationEntity(type, entityType);
                        }
                    }

                    #endregion

                    #region Query model

                    var queryModelInterface = type.GetInterface(queryModelGenericType.Name);
                    if (queryModelInterface != null && !isEntity && !isAggreagtion)
                    {
                        QueryManager.ConfigureQueryModelRelationEntity(type);
                    }

                    #endregion

                    #region Module configuration

                    if (moduleConfigurationType.IsAssignableFrom(type))
                    {
                        IModuleConfiguration moduleConfiguration = Activator.CreateInstance(type) as IModuleConfiguration;
                        moduleConfiguration?.Init();
                    }

                    #endregion
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
                    FileMatchPattern.Convention => new Regex($@"^{ApplicationManager.RootPath.Replace(@"\", @"\\")}.*({string.Join("|", ConventionFileNamePatterns.Union(ApplicationManager.Options.FileMatchOptions.FileRegexPatterns ?? new List<string>(0)))}).*$", RegexOptions.IgnoreCase).IsMatch(c.FullName),
                    _ => true
                };
                return matched;
            });
        }

        #endregion
    }
}
