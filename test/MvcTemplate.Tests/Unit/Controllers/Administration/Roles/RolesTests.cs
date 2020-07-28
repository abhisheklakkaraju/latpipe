using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MvcTemplate.Components.Security;
using MvcTemplate.Controllers.Tests;
using MvcTemplate.Objects;
using MvcTemplate.Services;
using MvcTemplate.Tests;
using MvcTemplate.Validators;
using NSubstitute;
using System;
using System.Linq;
using Xunit;

namespace MvcTemplate.Controllers.Administration.Tests
{
    public class RolesTests : ControllerTests
    {
        private IRoleValidator validator;
        private IRoleService service;
        private Roles controller;
        private RoleView role;

        public RolesTests()
        {
            role = ObjectsFactory.CreateRoleView(0);
            service = Substitute.For<IRoleService>();
            validator = Substitute.For<IRoleValidator>();
            controller = Substitute.ForPartsOf<Roles>(validator, service);

            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.Authorization.Returns(Substitute.For<IAuthorization>());
            controller.ControllerContext.RouteData = new RouteData();
        }
        public override void Dispose()
        {
            controller.Dispose();
            validator.Dispose();
            service.Dispose();
        }

        [Fact]
        public void Index_ReturnsRoleViews()
        {
            service.GetViews().Returns(Array.Empty<RoleView>().AsQueryable());

            Object actual = controller.Index().Model;
            Object expected = service.GetViews();

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Create_ReturnsNewRoleView()
        {
            RoleView actual = Assert.IsType<RoleView>(controller.Create().Model);

            Assert.NotNull(actual.Permissions);
            Assert.Null(actual.Title);
        }

        [Fact]
        public void Create_SeedsPermissions()
        {
            RoleView view = Assert.IsType<RoleView>(controller.Create().Model);

            service.Received().Seed(view.Permissions);
        }

        [Fact]
        public void Create_CanNotCreate_SeedsPermissions()
        {
            validator.CanCreate(role).Returns(false);

            controller.Create(role);

            service.Received().Seed(role.Permissions);
        }

        [Fact]
        public void Create_CanNotCreate_ReturnsSameView()
        {
            validator.CanCreate(role).Returns(false);

            Object actual = Assert.IsType<ViewResult>(controller.Create(role)).Model;
            Object expected = role;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Create_Role()
        {
            validator.CanCreate(role).Returns(true);

            controller.Create(role);

            service.Received().Create(role);
        }

        [Fact]
        public void Create_RedirectsToIndex()
        {
            validator.CanCreate(role).Returns(true);

            Object expected = RedirectToAction(controller, nameof(Roles.Index));
            Object actual = controller.Create(role);

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Details_ReturnsNotEmptyView()
        {
            service.GetView(role.Id).Returns(role);

            Object expected = NotEmptyView(controller, role);
            Object actual = controller.Details(role.Id);

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Edit_ReturnsNotEmptyView()
        {
            service.GetView(role.Id).Returns(role);

            Object expected = NotEmptyView(controller, role);
            Object actual = controller.Edit(role.Id);

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Edit_CanNotEdit_SeedsPermissions()
        {
            validator.CanEdit(role).Returns(false);

            controller.Edit(role);

            service.Received().Seed(role.Permissions);
        }

        [Fact]
        public void Edit_CanNotEdit_ReturnsSameView()
        {
            validator.CanEdit(role).Returns(false);

            Object actual = Assert.IsType<ViewResult>(controller.Edit(role)).Model;
            Object expected = role;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Edit_Role()
        {
            validator.CanEdit(role).Returns(true);

            controller.Edit(role);

            service.Received().Edit(role);
        }

        [Fact]
        public void Edit_RefreshesAuthorization()
        {
            controller.Authorization.Returns(Substitute.For<IAuthorization>());
            validator.CanEdit(role).Returns(true);

            controller.Edit(role);

            controller.Authorization.Received().Refresh(controller.HttpContext.RequestServices);
        }

        [Fact]
        public void Edit_RedirectsToIndex()
        {
            validator.CanEdit(role).Returns(true);

            Object expected = RedirectToAction(controller, nameof(Roles.Index));
            Object actual = controller.Edit(role);

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Delete_ReturnsNotEmptyView()
        {
            service.GetView(role.Id).Returns(role);

            Object expected = NotEmptyView(controller, role);
            Object actual = controller.Delete(role.Id);

            Assert.Same(expected, actual);
        }

        [Fact]
        public void DeleteConfirmed_DeletesRole()
        {
            controller.DeleteConfirmed(role.Id);

            service.Received().Delete(role.Id);
        }

        [Fact]
        public void DeleteConfirmed_RefreshesAuthorization()
        {
            controller.DeleteConfirmed(role.Id);

            controller.Authorization.Received().Refresh(controller.HttpContext.RequestServices);
        }

        [Fact]
        public void DeleteConfirmed_RedirectsToIndex()
        {
            Object expected = RedirectToAction(controller, nameof(Roles.Index));
            Object actual = controller.DeleteConfirmed(role.Id);

            Assert.Same(expected, actual);
        }
    }
}
