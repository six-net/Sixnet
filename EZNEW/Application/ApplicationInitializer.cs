using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using EZNEW.Development.DataAccess;
using EZNEW.Development.Domain.Model;
using EZNEW.Development.Domain.Repository;
using EZNEW.Development.Entity;
using EZNEW.Development.Query;
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
        /// Aggregation contract type
        /// </summary>
        static readonly Type aggreagtionContractType = typeof(IModel);

        /// <summary>
        /// Entity contract type
        /// </summary>
        static readonly Type entityContractType = typeof(IEntity);

        /// <summary>
        /// Module configuration contract
        /// </summary>
        static readonly Type moduleConfigurationType = typeof(IModuleConfiguration);

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
                var allTypeDict = ApplicationManager.GetAllConventionTypes()?.ToDictionary(c => c.FullName, c => c);
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

                    #region Model

                    if (isAggreagtion && !isEntity)
                    {
                        var entityType = ModelManager.GetModelRelationEntityType(type);
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

        #endregion
    }
}
