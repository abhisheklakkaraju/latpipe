using AutoMapper;
using MvcTemplate.Objects;
using MvcTemplate.Tests;
using Xunit;

namespace MvcTemplate.Data.Mapping.Tests
{
    public class ObjectMapperTests
    {
        static ObjectMapperTests()
        {
            ObjectMapper.MapObjects();
        }

        [Fact]
        public void MapRoles_Role_RoleView()
        {
            Role expected = ObjectsFactory.CreateRole();
            RoleView actual = Mapper.Map<RoleView>(expected);

            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.Empty(actual.Permissions.SelectedIds);
            Assert.Equal(expected.Title, actual.Title);
            Assert.Empty(actual.Permissions.Nodes);
            Assert.Equal(expected.Id, actual.Id);
        }

        [Fact]
        public void MapRoles_RoleView_Role()
        {
            RoleView expected = ObjectsFactory.CreateRoleView();
            Role actual = Mapper.Map<Role>(expected);

            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.Equal(expected.Title, actual.Title);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Empty(actual.Permissions);
        }
    }
}
