using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EZNEW.Application;
using EZNEW.Development.Domain;
using EZNEW.Development.Entity;
using EZNEW.Mapper;

namespace AutoMapper
{
    public class ConventionMapperBuilder : IMapperBuilder
    {
        private Action<IMapperConfigurationExpression> mapperConfigurationAction;

        public ConventionMapperBuilder(Action<IMapperConfigurationExpression> configureAction = null)
        {
            mapperConfigurationAction = new Action<IMapperConfigurationExpression>(ConfigureConventionMap);
            if (configureAction != null)
            {
                mapperConfigurationAction += configureAction;
            }
        }

        /// <summary>
        /// Configure map
        /// </summary>
        /// <param name="configureAction">Configure action</param>
        public void ConfigureMap(Action<IMapperConfigurationExpression> configureAction)
        {
            if (configureAction != null)
            {
                mapperConfigurationAction += configureAction;
            }
        }

        /// <summary>
        /// Create a mapper
        /// </summary>
        /// <returns>Return IMapper object</returns>
        public EZNEW.Mapper.IMapper CreateMapper()
        {
            return new ConventionMapper(mapperConfigurationAction);
        }

        /// <summary>
        /// Configure convention map
        /// </summary>
        /// <param name="mapperConfigurationExpression"></param>
        void ConfigureConventionMap(IMapperConfigurationExpression mapperConfigurationExpression)
        {
            var conventionTypes = ApplicationInitializer.GetAllConventionTypes();
            if (conventionTypes.IsNullOrEmpty())
            {
                return;
            }
            var allEntities = EntityManager.GetAllEntityConfigurations()?.Select(c => c.EntityType) ?? Array.Empty<Type>();
            var defaultMemberValidation = MemberList.None;
            StringComparison stringComparison = StringComparison.OrdinalIgnoreCase;
            foreach (var entity in allEntities)
            {
                var entityName = entity.Name;
                if (entityName.EndsWith("Entity"))
                {
                    entityName = entityName.Substring(0, entityName.Length - 6);
                }

                //domain
                var domainType = conventionTypes.FirstOrDefault(c => c != entity && c.Name.Equals(entityName, stringComparison));
                if (domainType != null)
                {
                    mapperConfigurationExpression.CreateMap(entity, domainType, defaultMemberValidation);
                    mapperConfigurationExpression.CreateMap(domainType, entity, defaultMemberValidation);
                }
                //dto
                var dtoType = conventionTypes.FirstOrDefault(c => c.Name.Equals($"{entityName}Dto", stringComparison));
                if (dtoType != null && domainType != null)
                {
                    mapperConfigurationExpression.CreateMap(domainType, dtoType, defaultMemberValidation);
                    mapperConfigurationExpression.CreateMap(dtoType, domainType, defaultMemberValidation);
                }
                //view model
                var viewModelType = conventionTypes.FirstOrDefault(c => c.Name.Equals($"{entityName}ViewModel", stringComparison));
                if (dtoType != null && viewModelType != null)
                {
                    mapperConfigurationExpression.CreateMap(viewModelType, dtoType, defaultMemberValidation);
                    mapperConfigurationExpression.CreateMap(dtoType, viewModelType, defaultMemberValidation);
                }
                //edit view model
                var editViewModelType = conventionTypes.FirstOrDefault(c => c.Name.Equals($"Edit{entityName}ViewModel", stringComparison));
                if (editViewModelType != null && dtoType != null)
                {
                    mapperConfigurationExpression.CreateMap(editViewModelType, dtoType, defaultMemberValidation);
                    mapperConfigurationExpression.CreateMap(dtoType, editViewModelType, defaultMemberValidation);
                }
                if (viewModelType != null && editViewModelType != null)
                {
                    mapperConfigurationExpression.CreateMap(editViewModelType, viewModelType, defaultMemberValidation);
                    mapperConfigurationExpression.CreateMap(viewModelType, editViewModelType, defaultMemberValidation);
                }
            }
            var domainParameterContract = typeof(IDomainParameter);
            //parmater
            var domainParameterTypes = conventionTypes.Where(c => domainParameterContract.IsAssignableFrom(c));
            foreach (var parameterType in domainParameterTypes)
            {
                var parameterName = parameterType.Name.LSplit("Parameter")[0];
                //parameter dto
                var parameterDto = conventionTypes.FirstOrDefault(c => c.Name.Equals($"{parameterName}Dto", stringComparison));
                if (parameterDto != null)
                {
                    mapperConfigurationExpression.CreateMap(parameterType, parameterDto, defaultMemberValidation);
                    mapperConfigurationExpression.CreateMap(parameterDto, parameterType, defaultMemberValidation);
                }
                // parameter viewmodel
                var parameterViewModel = conventionTypes.FirstOrDefault(c => c.Name.Equals($"{parameterName}ViewModel", stringComparison));
                if (parameterViewModel != null && parameterDto != null)
                {
                    mapperConfigurationExpression.CreateMap(parameterViewModel, parameterDto, defaultMemberValidation);
                    mapperConfigurationExpression.CreateMap(parameterDto, parameterViewModel, defaultMemberValidation);
                }
            }
        }
    }
}
