using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sixnet.App.Module;
using Sixnet.DependencyInjection;
using Sixnet.Development.Entity;
using Sixnet.Logging;

namespace Sixnet.App
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
                LogManager.LogInformation<ApplicationInitializer>(FrameworkLogEvents.Application.Initialization, $"Start initialize the application");

                var allConventionTypes = ApplicationManager.GetAllConventionTypes();
                if (allConventionTypes.IsNullOrEmpty())
                {
                    LogManager.LogWarning<ApplicationInitializer>(FrameworkLogEvents.Application.Initialization, $"No type found that needs to be initialized");
                    return;
                }

                #region Configure entity

                foreach (var type in allConventionTypes)
                {
                    if ((!(type.IsInterface || type.IsAbstract)) && entityContractType.IsAssignableFrom(type))
                    {
                        EntityManager.ConfigureEntity(type);
                    }
                }

                #endregion

                #region Configure app

                foreach (var type in allConventionTypes)
                {
                    if (type.IsInterface)
                    {
                        var ignoreCaseComparison = StringComparison.OrdinalIgnoreCase;
                        var typeName = type.FullName;

                        #region Default service

                        var serviceRegex = new Regex($@"^*({string.Join("|", ConventionServicePatterns)})$", RegexOptions.IgnoreCase);
                        if (serviceRegex.IsMatch(typeName))
                        {
                            var implementType = allConventionTypes.FirstOrDefault(t => t.FullName != type.FullName && !t.IsInterface && !t.IsAbstract && type.IsAssignableFrom(t));
                            if (implementType != null)
                            {
                                ContainerManager.AddDefaultProjectService(type, implementType);
                            }
                        }
                        if (typeName.EndsWith("DataAccess", ignoreCaseComparison))
                        {
                            var relateTypes = allConventionTypes.Where(t => t.FullName != typeName && !t.IsInterface && !t.IsAbstract && type.IsAssignableFrom(t));
                            if (relateTypes.Any())
                            {
                                var providerType = relateTypes.FirstOrDefault(c => c.Name.EndsWith("CacheDataAccess", ignoreCaseComparison));
                                providerType ??= relateTypes.First();
                                ContainerManager.AddDefaultProjectService(type, providerType);
                            }
                        }

                        #endregion
                    }
                    else if (!type.IsAbstract)
                    {
                        var isEntity = entityContractType.IsAssignableFrom(type);

                        #region Module configuration

                        if (moduleConfigurationType.IsAssignableFrom(type))
                        {
                            var moduleConfiguration = Activator.CreateInstance(type) as IModuleConfiguration;
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
