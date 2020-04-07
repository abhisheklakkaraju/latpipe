using MvcTemplate.Data.Core;
using MvcTemplate.Objects;
using MvcTemplate.Tests;
using NSubstitute;
using System;
using System.Reflection;
using Xunit;

namespace MvcTemplate.Components.Security.Tests
{
    public class AuthorizationTests : IDisposable
    {
        private TestingContext context;
        private Authorization authorization;

        public AuthorizationTests()
        {
            context = new TestingContext();
            IServiceProvider services = Substitute.For<IServiceProvider>();
            services.GetService(typeof(IUnitOfWork)).Returns(info => new UnitOfWork(new TestingContext(context)));

            authorization = new Authorization(Assembly.GetExecutingAssembly(), services);
        }
        public void Dispose()
        {
            context.Dispose();
        }

        [Fact]
        public void IsGrantedFor_AuthorizesControllerByIgnoringCase()
        {
            Int64 accountId = CreateAccountWithPermissionFor("Area", "Authorized", "Action");

            Assert.True(authorization.IsGrantedFor(accountId, "Area", "AUTHORIZED", "Action"));
        }

        [Fact]
        public void IsGrantedFor_DoesNotAuthorizeControllerByIgnoringCase()
        {
            Int64 accountId = CreateAccountWithPermissionFor("Test", "Test", "Test");

            Assert.False(authorization.IsGrantedFor(accountId, "Area", "AUTHORIZED", "Action"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void IsGrantedFor_AuthorizesControllerWithoutArea(String area)
        {
            Int64 accountId = CreateAccountWithPermissionFor(null, "Authorized", "Action");

            Assert.True(authorization.IsGrantedFor(accountId, area, "Authorized", "Action"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void IsGrantedFor_DoesNotAuthorizeControllerWithoutArea(String area)
        {
            Int64 accountId = CreateAccountWithPermissionFor(null, "Test", "Test");

            Assert.False(authorization.IsGrantedFor(accountId, area, "Authorized", "Action"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesControllerWithArea()
        {
            Int64 accountId = CreateAccountWithPermissionFor("Area", "Authorized", "Action");

            Assert.True(authorization.IsGrantedFor(accountId, "Area", "Authorized", "Action"));
        }

        [Fact]
        public void IsGrantedFor_DoesNotAuthorizeControllerWithArea()
        {
            Int64 accountId = CreateAccountWithPermissionFor("Test", "Test", "Test");

            Assert.False(authorization.IsGrantedFor(accountId, "Area", "Authorized", "Action"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesGetAction()
        {
            Int64 accountId = CreateAccountWithPermissionFor("Area", "Authorized", "AuthorizedGetAction");

            Assert.True(authorization.IsGrantedFor(accountId, "Area", "Authorized", "AuthorizedGetAction"));
        }

        [Fact]
        public void IsGrantedFor_DoesNotAuthorizeGetAction()
        {
            Int64 accountId = CreateAccountWithPermissionFor("Test", "Test", "Test");

            Assert.False(authorization.IsGrantedFor(accountId, "Area", "Authorized", "AuthorizedGetAction"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesNamedGetAction()
        {
            Int64 accountId = CreateAccountWithPermissionFor("Area", "Authorized", "AuthorizedNamedGetAction");

            Assert.True(authorization.IsGrantedFor(accountId, "Area", "Authorized", "AuthorizedNamedGetAction"));
        }

        [Fact]
        public void IsGrantedFor_DoesNotAuthorizeNamedGetAction()
        {
            Int64 accountId = CreateAccountWithPermissionFor("Test", "Test", "Test");

            Assert.False(authorization.IsGrantedFor(accountId, "Area", "Authorized", "AuthorizedNamedGetAction"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesNotExistingAction()
        {
            Assert.True(authorization.IsGrantedFor(null, null, "Authorized", "Test"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesNonGetAction()
        {
            Int64 accountId = CreateAccountWithPermissionFor("Area", "Authorized", "AuthorizedPostAction");

            Assert.True(authorization.IsGrantedFor(accountId, "Area", "Authorized", "AuthorizedPostAction"));
        }

        [Fact]
        public void IsGrantedFor_DoesNotAuthorizeNonGetAction()
        {
            Int64 accountId = CreateAccountWithPermissionFor("Test", "Test", "Test");

            Assert.False(authorization.IsGrantedFor(accountId, "Area", "Authorized", "AuthorizedPostAction"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesNamedNonGetAction()
        {
            Int64 accountId = CreateAccountWithPermissionFor("Area", "Authorized", "AuthorizedNamedPostAction");

            Assert.True(authorization.IsGrantedFor(accountId, "Area", "Authorized", "AuthorizedNamedPostAction"));
        }

        [Fact]
        public void IsGrantedFor_DoesNotAuthorizeNamedNonGetAction()
        {
            Int64 accountId = CreateAccountWithPermissionFor("Area", "Test", "Test");

            Assert.False(authorization.IsGrantedFor(accountId, "Area", "Authorized", "AuthorizedNamedPostAction"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesActionAsAction()
        {
            Int64 accountId = CreateAccountWithPermissionFor("Area", "Authorized", "Action");

            Assert.True(authorization.IsGrantedFor(accountId, "Area", "Authorized", "AuthorizedAsAction"));
        }

        [Fact]
        public void IsGrantedFor_DoesNotAuthorizeActionAsAction()
        {
            Int64 accountId = CreateAccountWithPermissionFor(null, "Authorized", "Action");

            Assert.False(authorization.IsGrantedFor(accountId, "Area", "Authorized", "AuthorizedAsAction"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesActionAsSelf()
        {
            Int64 accountId = CreateAccountWithPermissionFor("Area", "Authorized", "AuthorizedAsSelf");

            Assert.True(authorization.IsGrantedFor(accountId, "Area", "Authorized", "AuthorizedAsSelf"));
        }

        [Fact]
        public void IsGrantedFor_DoesNotAuthorizeActionAsSelf()
        {
            Int64 accountId = CreateAccountWithPermissionFor("Area", "Test", "Test");

            Assert.False(authorization.IsGrantedFor(accountId, "Area", "Authorized", "AuthorizedAsSelf"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesActionAsOtherAction()
        {
            Int64 accountId = CreateAccountWithPermissionFor(null, "InheritedAuthorized", "InheritanceAction");

            Assert.True(authorization.IsGrantedFor(accountId, "Area", "Authorized", "AuthorizedAsOtherAction"));
        }

        [Fact]
        public void IsGrantedFor_DoesNotAuthorizeActionAsOtherAction()
        {
            Int64 accountId = CreateAccountWithPermissionFor("Area", "Authorized", "AuthorizedAsOtherAction");

            Assert.False(authorization.IsGrantedFor(accountId, "Area", "Authorized", "AuthorizedAsOtherAction"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesEmptyAreaAsNull()
        {
            Int64 accountId = CreateAccountWithPermissionFor(null, "Authorized", "Action");

            Assert.True(authorization.IsGrantedFor(accountId, "", "Authorized", "Action"));
        }

        [Fact]
        public void IsGrantedFor_DoesNotAuthorizeEmptyAreaAsNull()
        {
            Int64 accountId = CreateAccountWithPermissionFor(null, "Test", "Test");

            Assert.False(authorization.IsGrantedFor(accountId, "", "Authorized", "Action"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesAuthorizedAction()
        {
            Int64 accountId = CreateAccountWithPermissionFor(null, "AllowAnonymous", "AuthorizedAction");

            Assert.True(authorization.IsGrantedFor(accountId, null, "AllowAnonymous", "AuthorizedAction"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesAllowAnonymousAction()
        {
            Int64 accountId = CreateAccountWithPermissionFor(null, "Test", "Test");

            Assert.True(authorization.IsGrantedFor(accountId, null, "Authorized", "AllowAnonymousAction"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesAllowUnauthorizedAction()
        {
            Int64 accountId = CreateAccountWithPermissionFor(null, "Test", "Test");

            Assert.True(authorization.IsGrantedFor(accountId, null, "Authorized", "AllowUnauthorizedAction"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesAuthorizedController()
        {
            Int64 accountId = CreateAccountWithPermissionFor(null, "Authorized", "Action");

            Assert.True(authorization.IsGrantedFor(accountId, null, "Authorized", "Action"));
        }

        [Fact]
        public void IsGrantedFor_DoesNotAuthorizeAuthorizedController()
        {
            Int64 accountId = CreateAccountWithPermissionFor(null, "Test", "Test");

            Assert.False(authorization.IsGrantedFor(accountId, null, "Authorized", "Action"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesAllowAnonymousController()
        {
            Int64 accountId = CreateAccountWithPermissionFor(null, "Test", "Test");

            Assert.True(authorization.IsGrantedFor(accountId, null, "AllowAnonymous", "Action"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesAllowUnauthorizedController()
        {
            Int64 accountId = CreateAccountWithPermissionFor(null, "Test", "Test");

            Assert.True(authorization.IsGrantedFor(accountId, null, "AllowUnauthorized", "Action"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesInheritedAuthorizedController()
        {
            Int64 accountId = CreateAccountWithPermissionFor(null, "InheritedAuthorized", "InheritanceAction");

            Assert.True(authorization.IsGrantedFor(accountId, null, "InheritedAuthorized", "InheritanceAction"));
        }

        [Fact]
        public void IsGrantedFor_DoesNotAuthorizeInheritedAuthorizedController()
        {
            Int64 accountId = CreateAccountWithPermissionFor(null, "Test", "Test");

            Assert.False(authorization.IsGrantedFor(accountId, null, "InheritedAuthorized", "InheritanceAction"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesInheritedAllowAnonymousController()
        {
            Int64 accountId = CreateAccountWithPermissionFor(null, "Test", "Test");

            Assert.True(authorization.IsGrantedFor(accountId, null, "InheritedAllowAnonymous", "InheritanceAction"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesInheritedAllowUnauthorizedController()
        {
            Int64 accountId = CreateAccountWithPermissionFor(null, "Test", "Test");

            Assert.True(authorization.IsGrantedFor(accountId, null, "InheritedAllowUnauthorized", "InheritanceAction"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesNotAttributedController()
        {
            Int64 accountId = CreateAccountWithPermissionFor(null, "Test", "Test");

            Assert.True(authorization.IsGrantedFor(accountId, null, "NotAttributed", "Action"));
        }

        [Fact]
        public void IsGrantedFor_DoesNotAuthorizeNotExistingAccount()
        {
            CreateAccountWithPermissionFor("Area", "Authorized", "Action");

            Assert.False(authorization.IsGrantedFor(0, "Area", "Authorized", "Action"));
        }

        [Fact]
        public void IsGrantedFor_DoesNotAuthorizeLockedAccount()
        {
            Int64 accountId = CreateAccountWithPermissionFor("Area", "Authorized", "Action", isLocked: true);

            Assert.False(authorization.IsGrantedFor(accountId, "Area", "Authorized", "Action"));
        }

        [Fact]
        public void IsGrantedFor_DoesNotAuthorizeNullAccount()
        {
            CreateAccountWithPermissionFor(null, "Authorized", "Action");

            Assert.False(authorization.IsGrantedFor(null, null, "Authorized", "Action"));
        }

        [Fact]
        public void IsGrantedFor_AuthorizesByIgnoringCase()
        {
            Int64 accountId = CreateAccountWithPermissionFor("Area", "Authorized", "Action");

            Assert.True(authorization.IsGrantedFor(accountId, "area", "authorized", "action"));
        }

        [Fact]
        public void IsGrantedFor_DoesNotAuthorizeByIgnoringCase()
        {
            Int64 accountId = CreateAccountWithPermissionFor("Test", "Test", "Test");

            Assert.False(authorization.IsGrantedFor(accountId, "area", "authorized", "action"));
        }

        [Fact]
        public void IsGrantedFor_CachesAccountPermissions()
        {
            Int64 accountId = CreateAccountWithPermissionFor(null, "Authorized", "Action");

            context.Database.EnsureDeleted();

            Assert.True(authorization.IsGrantedFor(accountId, null, "Authorized", "Action"));
        }

        [Fact]
        public void Refresh_Permissions()
        {
            Int64 accountId = CreateAccountWithPermissionFor("Area", "Authorized", "Action");
            Assert.True(authorization.IsGrantedFor(accountId, "Area", "Authorized", "Action"));

            context.Database.EnsureDeleted();

            authorization.Refresh();

            Assert.False(authorization.IsGrantedFor(accountId, "Area", "Authorized", "Action"));
        }

        private Int64 CreateAccountWithPermissionFor(String? area, String controller, String action, Boolean isLocked = false)
        {
            RolePermission rolePermission = ObjectsFactory.CreateRolePermission();
            rolePermission.Permission.Controller = controller;
            rolePermission.Permission.Action = action;
            rolePermission.Permission.Area = area;

            Account account = ObjectsFactory.CreateAccount();
            account.Role = rolePermission.Role;
            account.IsLocked = isLocked;

            context.Add(rolePermission);
            context.Add(account);

            context.SaveChanges();

            authorization.Refresh();

            return account.Id;
        }
    }
}
