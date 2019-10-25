using MvcTemplate.Objects;
using MvcTemplate.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace MvcTemplate.Data.Migrations.Tests
{
    public class InitialDataTests : IDisposable
    {
        private Configuration configuration;
        private TestingContext context;

        public InitialDataTests()
        {
            context = new TestingContext();
            configuration = new Configuration(context, null);
            configuration.SeedData();
        }
        public void Dispose()
        {
            configuration.Dispose();
            context.Dispose();
        }

        [Fact]
        public void RolesTable_HasSysAdmin()
        {
            Assert.Single(context.Set<Role>(), role => role.Title == "Sys_Admin");
        }

        [Fact]
        public void AccountsTable_HasAdmin()
        {
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
        public void PermissionsTable_HasPermission(String area, String controller, String action)
        {
            Assert.Single(context.Set<Permission>(), permission =>
                permission.Controller == controller &&
                permission.Action == action &&
                permission.Area == area);
        }

        [Fact]
        public void PermissionsTable_HasExactNumberOfPermissions()
        {
            Int32 actual = context.Set<Permission>().Count();
            Int32 expected = GetType()
                .GetMethod(nameof(PermissionsTable_HasPermission))!
                .GetCustomAttributes<InlineDataAttribute>()
                .Count();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RolesPermissionsTable_HasAllSysAdminPermissions()
        {
            IEnumerable<Int32> expected = context
                .Set<Permission>()
                .Select(permission => permission.Id)
                .OrderBy(permissionId => permissionId);

            IEnumerable<Int32> actual = context
                .Set<RolePermission>()
                .Where(permission => permission.Role.Title == "Sys_Admin")
                .Select(rolePermission => rolePermission.PermissionId)
                .OrderBy(permissionId => permissionId);

            Assert.Equal(expected, actual);
        }
    }
}
