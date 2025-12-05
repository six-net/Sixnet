// "Company © 2025. All rights reserved."

using System.Runtime;
using System.Text.RegularExpressions;

using Sixnet.DependencyInjection;
using Sixnet.Development.Data;
using Sixnet.Development.Data.Database;
using Sixnet.Development.Entity;
using Sixnet.Logging;
using Sixnet.Model;

namespace Sixnet.App
{
    /// <summary>
    /// Application initializer
    /// </summary>
    internal class ApplicationInitializer
    {
        #region Constructor

        static ApplicationInitializer()
        {
            InitApplication();
        }

        #endregion

        #region Fields

        /// <summary>
        /// Entity contract type
        /// </summary>
        static readonly Type _entityContractType = typeof(ISixnetEntity);

        /// <summary>
        /// Initializable contract
        /// </summary>
        static readonly Type _initializableContractType = typeof(ISixnetInitializable);

        /// <summary>
        /// Configurable contract 
        /// </summary>
        static readonly Type _configurableContractType = typeof(ISixnetConfigurable);

        /// <summary>
        /// Database update contract type
        /// </summary>
        static readonly Type _databaseUpdateRecordContractType = typeof(ISixnetDatabaseUpdateRecord);

        /// <summary>
        /// Convention service patterns
        /// </summary>
        static readonly List<string> _conventionServicePatterns = new()
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
                SixnetLogger.LogInformation<ApplicationInitializer>(SixnetLogEvents.Application.Initialization, "Start init application.");

                var allConventionTypes = SixnetApplication.GetAllConventionTypes();
                if (allConventionTypes.IsNullOrEmpty())
                {
                    SixnetLogger.LogWarning<ApplicationInitializer>(SixnetLogEvents.Application.Initialization, "No convention type was found that needs to be initialized");
                    return;
                }

                #region Configure entity

                foreach (var type in allConventionTypes)
                {
                    if ((!(type.IsInterface || type.IsAbstract)) && _entityContractType.IsAssignableFrom(type))
                    {
                        SixnetEntityManager.ConfigureEntity(type);
                    }
                }

                #endregion

                #region Configure app

                foreach (var type in allConventionTypes)
                {
                    if (type.IsInterface)
                    {
                        var typeName = type.FullName;

                        #region Default service

                        var serviceRegex = new Regex($@"^*({string.Join("|", _conventionServicePatterns)})$", RegexOptions.IgnoreCase);
                        if (serviceRegex.IsMatch(typeName))
                        {
                            var implementType = allConventionTypes.FirstOrDefault(t => t.FullName != type.FullName && !t.IsInterface && !t.IsAbstract && type.IsAssignableFrom(t));
                            if (implementType != null)
                            {
                                SixnetContainer.AddProjectService(type, implementType);
                            }
                        }

                        #endregion
                    }
                    else if (!type.IsAbstract)
                    {
                        if (IsDirectFromInterface(_initializableContractType, type)) // init model
                        {
                            var initializableModel = Activator.CreateInstance(type) as ISixnetInitializable;
                            SixnetApplication.AddInitializable(initializableModel);
                        }
                        if (IsDirectFromInterface(_configurableContractType, type))
                        {
                            var configModel = Activator.CreateInstance(type) as ISixnetConfigurable;
                            SixnetApplication.AddConfigurable(configModel);
                        }
                        if (_databaseUpdateRecordContractType.IsAssignableFrom(type))
                        {
                            var updateRecord = Activator.CreateInstance(type) as ISixnetDatabaseUpdateRecord;
                            SixnetDataManager.AddDatabaseUpdateRecord(updateRecord);
                        }
                    }
                }

                #endregion

                GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                GC.Collect();
            }
            catch (Exception ex)
            {
                SixnetLogger.LogError<ApplicationInitializer>(SixnetLogEvents.Application.InitializationFailure, ex, ex.Message);
            }
        }

        static bool IsDirectFromInterface(Type interfaceType, Type classType)
        {
            return classType != null
                && interfaceType != null
                && interfaceType.IsAssignableFrom(classType)
                && (classType.BaseType == null
                    || classType.BaseType == typeof(object)
                    || !interfaceType.IsAssignableFrom(classType.BaseType));
        }

        #endregion
    }
}
