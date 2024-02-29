using System;
using System.Collections.Generic;
using System.Linq;
using Sixnet.App;
using Sixnet.Development.Entity;
using Sixnet.Mapper;
using Sixnet.Development.Domain;
using Sixnet.Model;

namespace AutoMapper
{
    public class ConventionMapperBuilder : ISixnetMapperBuilder
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
        /// <param name="configurationAction">Configuration action</param>
        public void ConfigureMap(Action<IMapperConfigurationExpression> configurationAction)
        {
            if (configurationAction != null)
            {
                mapperConfigurationAction += configurationAction;
            }
        }

        /// <summary>
        /// Create a mapper
        /// </summary>
        /// <returns>Return IMapper object</returns>
        public Sixnet.Mapper.ISixnetMapper CreateMapper()
        {
            return new ConventionMapper(mapperConfigurationAction);
        }

        /// <summary>
        /// Configure convention mapping
        /// </summary>
        /// <param name="mapperConfigurationExpression"></param>
        void ConfigureConventionMap(IMapperConfigurationExpression mapperConfigurationExpression)
        {
            var conventionTypes = SixnetApplication.GetAllConventionTypes();
            if (!conventionTypes.IsNullOrEmpty())
            {
                var mappableModelContract = typeof(ISixnetMappableModel);
                var entityContract = typeof(ISixnetEntity);
                var domainParameterContract = typeof(SixnetDomainParameter);
                var defaultMemberValidation = MemberList.None;
                var stringComparison = StringComparison.OrdinalIgnoreCase;
                //Mappable models
                var mappableModelTypes = conventionTypes.Where(c => mappableModelContract.IsAssignableFrom(c));
                foreach (var mappableModelType in mappableModelTypes)
                {
                    var mappableModelName = mappableModelType.Name;
                    if (entityContract.IsAssignableFrom(mappableModelType) && mappableModelName.EndsWith("Entity"))
                    {
                        mappableModelName = mappableModelName.LSplit("Entity")[0];
                    }
                    if (domainParameterContract.IsAssignableFrom(mappableModelType) && mappableModelName.EndsWith("Parameter"))
                    {
                        mappableModelName = mappableModelName.LSplit("Parameter")[0];
                    }

                    var dtoSourceType = mappableModelType;
                    // domain
                    var domainType = conventionTypes.FirstOrDefault(c => c != mappableModelType && c.Name.Equals(mappableModelName, stringComparison));
                    if (domainType != null && domainType != mappableModelType)
                    {
                        // entity <=> domain model
                        mapperConfigurationExpression.CreateMap(mappableModelType, domainType, defaultMemberValidation);
                        mapperConfigurationExpression.CreateMap(domainType, mappableModelType, defaultMemberValidation);
                        dtoSourceType = domainType;
                    }

                    // dto
                    var dtoType = conventionTypes.FirstOrDefault(c => c.Name.Equals($"{mappableModelName}Dto", stringComparison));
                    var appSourceType = dtoSourceType;
                    if (dtoType != null)
                    {
                        // dto source type <=> dto
                        mapperConfigurationExpression.CreateMap(dtoSourceType, dtoType, defaultMemberValidation);
                        mapperConfigurationExpression.CreateMap(dtoType, dtoSourceType, defaultMemberValidation);
                        appSourceType = dtoType;
                    }

                    // request => app source type
                    var requestType = conventionTypes.FirstOrDefault(c => c.Name.Equals($"{mappableModelName}Request", stringComparison));
                    if (requestType != null)
                    {
                        mapperConfigurationExpression.CreateMap(requestType, appSourceType, defaultMemberValidation);
                    }

                    // app source type => response
                    var responseType = conventionTypes.FirstOrDefault(c => c.Name.Equals($"{mappableModelName}Response", stringComparison));
                    if (responseType != null)
                    {
                        mapperConfigurationExpression.CreateMap(appSourceType, responseType, defaultMemberValidation);
                    }

                    // view model <=> app source type
                    var viewModelType = conventionTypes.FirstOrDefault(c => c.Name.Equals($"{mappableModelName}ViewModel", stringComparison));
                    if (viewModelType != null)
                    {
                        mapperConfigurationExpression.CreateMap(appSourceType, viewModelType, defaultMemberValidation);
                        mapperConfigurationExpression.CreateMap(viewModelType, appSourceType, defaultMemberValidation);
                    }
                }
            }
        }
    }
}
