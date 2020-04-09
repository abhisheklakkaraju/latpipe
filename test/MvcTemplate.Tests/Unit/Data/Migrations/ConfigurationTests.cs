using MvcTemplate.Objects;
using MvcTemplate.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace MvcTemplate.Data.Migrations.Tests
{
    public class ConfigurationTests : IDisposable
    {
        private Configuration configuration;
        private TestingContext context;

        public ConfigurationTests()
        {
            context = new TestingContext();
            configuration = new Configuration(context);
        }
        public void Dispose()
        {
            configuration.Dispose();
            context.Dispose();
        }

        [Fact]
        public void Seed_Roles()
        {
            configuration.Seed();

            Assert.Single(context.Set<Role>(), role => role.Title == "Sys_Admin");
        }

        [Fact]
        public void Seed_Accounts()
        {
            configuration.Seed();

            Assert.Single(context.Set<Account>(), account => account.Username == "admin" && account.Role?.Title == "Sys_Admin");
        }

        [Theory]
        [InlineData("Administration", "Accounts", "Index")]
        [InlineData("Administration", "Accounts", "Create")]
        [InlineData("Administration", "Accounts", "Details")]
        [InlineData("Administration", "Accounts", "Edit")]
        [InlineData("Administration", "Roles", "Index")]
        [InlineData("Administration", "Roles", "Create")]
        [InlineData("Administration", "Roles", "Details")]
        [InlineData("Administration", "Roles", "Edit")]
        [InlineData("Administration", "Roles", "Delete")]
        public void Seed_Permissions(String area, String controller, String action)
        {
            configuration.Seed();

            Assert.Single(context.Set<Permission>(), permission =>
                permission.Controller == controller &&
                permission.Action == action &&
                permission.Area == area);
        }

        [Fact]
        public void Seed_RemovesUnusedPermissions()
        {
            Role role = ObjectsFactory.CreateRole();
            role.Permissions.Add(new RolePermission
            {
                Permission = new Permission { Area = "Test", Controller = "Test", Action = "Test" }
            });

            context.Add(role);
            context.SaveChanges();

            configuration.Seed();

            Assert.Empty(role.Permissions);
            Assert.Empty(context.Set<Permission>().Where(permission =>
                permission.Controller == "Test" &&
                permission.Action == "Test" &&
                permission.Area == "Test"));
        }

        [Fact]
        public void Seed_PermissionCount()
        {
            configuration.Seed();

            Int32 actual = context.Set<Permission>().Count();
            Int32 expected = GetType()
                .GetMethod(nameof(Seed_Permissions))!
                .GetCustomAttributes<InlineDataAttribute>()
                .Count();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Seed_RolePermissions()
        {
            configuration.Seed();

            IEnumerable<Int64> expected = context
                .Set<Permission>()
                .Select(permission => permission.Id)
                .OrderBy(permissionId => permissionId);

            IEnumerable<Int64> actual = context
                .Set<RolePermission>()
                .Where(permission => permission.Role.Title == "Sys_Admin")
                .Select(rolePermission => rolePermission.PermissionId)
                .OrderBy(permissionId => permissionId);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Seed_MultipleTimes()
        {
            configuration.Seed();
            configuration.Seed();
        }
    }
}
