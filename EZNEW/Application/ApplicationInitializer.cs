using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using EZNEW.DependencyInjection;
using EZNEW.Development.DataAccess;
using EZNEW.Development.Domain.Model;
using EZNEW.Development.Domain.Repository;
using EZNEW.Development.Entity;
using EZNEW.Development.Query;
using EZNEW.Logging;
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
        /// Model contract type
        /// </summary>
        static readonly Type modelContractType = typeof(IModel);

        /// <summary>
        /// Entity contract type
        /// </summary>
        static readonly Type entityContractType = typeof(IEntity);

        /// <summary>
        /// Module configuration contract
        /// </summary>
        static readonly Type moduleConfigurationType = typeof(IModuleConfiguration);

        /// <summary>
        /// Convention service patterns
        /// </summary>
        internal static readonly List<string> ConventionServicePatterns = new()
        {
            @"Service",
            @"Business",
            @"DbAccess",
            @"Repository"
        };

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
                LogManager.LogDebug<ApplicationInitializer>(FrameworkLogEvents.Application.Initialization, $"Initialize the application");

                var allTypeDict = ApplicationManager.GetAllConventionTypes()?.ToDictionary(c => c.FullName, c => c);
                if (allTypeDict.IsNullOrEmpty())
                {
                    return;
                }

                #region Configure entity

                foreach (var type in allTypeDict.Values)
                {
                    if (type.IsInterface || type.IsAbstract)
                    {
                        continue;
                    }
                    bool isEntity = entityContractType.IsAssignableFrom(type);
                    if (isEntity)
                    {
                        EntityManager.ConfigureEntity(type);
                    }
                }

                #endregion

                #region Configure app

                foreach (var type in allTypeDict.Values)
                {
                    if (type.IsInterface)
                    {
                        StringComparison ignoreCaseComparison = StringComparison.OrdinalIgnoreCase;
                        string typeName = type.FullName;

                        #region Default service

                        var serviceRegex = new Regex($@"^*({string.Join("|", ConventionServicePatterns)})$", RegexOptions.IgnoreCase);
                        if (serviceRegex.IsMatch(typeName))
                        {
                            Type implementType = allTypeDict.Values.FirstOrDefault(t => t.FullName != type.FullName && !t.IsInterface && !t.IsAbstract && type.IsAssignableFrom(t));
                            if (implementType != null)
                            {
                                ContainerManager.AddDefaultProjectService(type, implementType);
                            }
                        }
                        if (typeName.EndsWith("DataAccess", ignoreCaseComparison))
                        {
                            var relateTypes = allTypeDict.Values.Where(t => t.FullName != typeName && !t.IsInterface && !t.IsAbstract && type.IsAssignableFrom(t));
                            if (relateTypes.Any())
                            {
                                Type providerType = relateTypes.FirstOrDefault(c => c.Name.EndsWith("CacheDataAccess", ignoreCaseComparison));
                                providerType ??= relateTypes.First();
                                ContainerManager.AddDefaultProjectService(type, providerType);
                            }

                            //Default warehouse
                            var interfaces = type.GetInterfaces();
                            var baseDataAccessInterface = interfaces.FirstOrDefault(c => c.IsGenericType && c.GetGenericTypeDefinition() == typeof(IDataAccess<>));
                            if (baseDataAccessInterface != null)
                            {
                                var entityType = baseDataAccessInterface.GenericTypeArguments[0];
                                ContainerManager.AddWarehouseService(entityType, type);
                            }
                        }

                        #endregion
                    }
                    else if (!type.IsAbstract)
                    {
                        bool isEntity = entityContractType.IsAssignableFrom(type);
                        var isModel = modelContractType.IsAssignableFrom(type);

                        #region Model

                        if (isModel)
                        {
                            //Configure model
                            ModelManager.ConfigureModel(type, allTypeDict);
                            //Default repository
                            RepositoryManager.RegisterDefaultRepository(type);
                        }

                        #endregion

                        #region Query model

                        var queryModelInterface = type.GetInterface(queryModelGenericType.Name);
                        if (queryModelInterface != null && !isEntity && !isModel)
                        {
                            QueryManager.ConfigureQueryModelRelationEntity(type);
                        }

                        #endregion

                        #region Module configuration

                        if (moduleConfigurationType.IsAssignableFrom(type))
                        {
                            IModuleConfiguration moduleConfiguration = Activator.CreateInstance(type) as IModuleConfiguration;
                            ModuleManager.RegisterModuleConfiguration(moduleConfiguration);
                        }

                        #endregion 
                    }
                }

                #endregion

                GC.Collect();
            }
            catch (Exception ex)
            {
                LogManager.LogError<ApplicationInitializer>(FrameworkLogEvents.Application.InitializationFailure, ex, ex.Message);
            }
        }

        #endregion
    }
}
