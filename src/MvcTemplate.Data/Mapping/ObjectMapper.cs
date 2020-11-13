using AutoMapper;
using AutoMapper.Configuration;
using MvcTemplate.Objects;
using System.Collections.Generic;

namespace MvcTemplate.Data.Mapping
{
    public sealed class ObjectMapper
    {
        public static void MapObjects()
        {
            Mapper.Reset();
            Mapper.Initialize(Map());
        }

        private MapperConfigurationExpression Configuration { get; }

        private ObjectMapper(MapperConfigurationExpression configuration)
        {
            Configuration = configuration;
            Configuration.ValidateInlineMaps = false;
            Configuration.AddConditionalObjectMapper().Conventions.Add(pair => pair.SourceType.Namespace != "Castle.Proxies");
        }

        private static MapperConfigurationExpression Map()
        {
            ObjectMapper mapper = new(new MapperConfigurationExpression());

            mapper.MapRoles();

            return mapper.Configuration;
        }

        private void MapRoles()
        {
            Configuration.CreateMap<Role, RoleView>()
                .ForMember(role => role.Permissions, member => member.Ignore());
            Configuration.CreateMap<RoleView, Role>()
                .ForMember(role => role.Permissions, member => member.MapFrom(_ => new List<RolePermission>()));
        }
    }
}
