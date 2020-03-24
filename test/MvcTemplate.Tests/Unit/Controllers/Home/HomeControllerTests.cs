using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using MvcTemplate.Services;
using NSubstitute;
using System;
using Xunit;

namespace MvcTemplate.Controllers.Tests
{
    public class HomeControllerTests : ControllerTests
    {
        private HomeController controller;
        private IAccountService service;

        public HomeControllerTests()
        {
            service = Substitute.For<IAccountService>();
            controller = Substitute.ForPartsOf<HomeController>(service);

            ActionContext context = new ActionContext(new DefaultHttpContext(), new RouteData(), new ControllerActionDescriptor());
            controller.ControllerContext = new ControllerContext(context);

            ReturnCurrentAccountId(controller, 1);
        }
        public override void Dispose()
        {
            controller.Dispose();
            service.Dispose();
        }

        [Fact]
        public void Index_NotActive_RedirectsToLogout()
        {
            service.IsActive(controller.CurrentAccountId).Returns(false);

            Object expected = RedirectToAction(controller, "Logout", "Auth");
            Object actual = controller.Index();

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Index_ReturnsEmptyView()
        {
            service.IsActive(controller.CurrentAccountId).Returns(true);

            ViewResult actual = Assert.IsType<ViewResult>(controller.Index());

            Assert.Null(actual.Model);
        }

        [Fact]
        public void Error_ReturnsJsonAlerts()
        {
            controller.Request.Headers["X-Requested-With"] = "XMLHttpRequest";

            JsonResult actual = Assert.IsType<JsonResult>(controller.Error());

            Assert.Equal(StatusCodes.Status500InternalServerError, controller.Response.StatusCode);
            Assert.Same(controller.Alerts, actual.Value.GetType().GetProperty("alerts")?.GetValue(actual.Value));
        }

        [Fact]
        public void Error_ReturnsEmptyView()
        {
            ViewResult actual = Assert.IsType<ViewResult>(controller.Error());

            Assert.Equal(StatusCodes.Status500InternalServerError, controller.Response.StatusCode);
            Assert.Null(actual.Model);
        }

        [Fact]
        public void NotFound_NotActive_RedirectsToLogout()
        {
            service.IsLoggedIn(controller.User).Returns(true);
            service.IsActive(controller.CurrentAccountId).Returns(false);

            Object expected = RedirectToAction(controller, "Logout", "Auth");
            Object actual = controller.NotFound();

            Assert.Same(expected, actual);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void NotFound_ReturnsEmptyView(Boolean isLoggedIn, Boolean isActive)
        {
            service.IsActive(controller.CurrentAccountId).Returns(isActive);
            service.IsLoggedIn(controller.User).Returns(isLoggedIn);

            ViewResult actual = Assert.IsType<ViewResult>(controller.NotFound());

            Assert.Equal(StatusCodes.Status404NotFound, controller.Response.StatusCode);
            Assert.Null(actual.Model);
        }
    }
}
