using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using MvcTemplate.Components.Security;
using MvcTemplate.Services;
using NSubstitute;
using System;
using System.Collections.Generic;
using Xunit;

namespace MvcTemplate.Controllers.Tests
{
    public class ServicedControllerTests : ControllerTests
    {
        private ServicedController<IService> controller;
        private ActionExecutingContext context;
        private IService service;

        public ServicedControllerTests()
        {
            service = Substitute.For<IService>();
            controller = Substitute.ForPartsOf<ServicedController<IService>>(service);
            ActionContext action = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
            context = new ActionExecutingContext(action, new List<IFilterMetadata>(), new Dictionary<String, Object>(), controller);

            controller.ControllerContext.RouteData = new RouteData();
            controller.ControllerContext.HttpContext = Substitute.For<HttpContext>();
            controller.HttpContext.RequestServices.GetService(typeof(IAuthorization)).Returns(Substitute.For<IAuthorization>());
        }
        public override void Dispose()
        {
            controller.Dispose();
            service.Dispose();
        }

        [Fact]
        public void ServicedController_SetsService()
        {
            Object actual = controller.Service;
            Object expected = service;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void OnActionExecuting_SetsServiceCurrentAccountId()
        {
            controller.CurrentAccountId.Returns(1);

            controller.OnActionExecuting(context);

            Int64 expected = controller.CurrentAccountId;
            Int64 actual = service.CurrentAccountId;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Dispose_Service()
        {
            controller.Dispose();

            service.Received().Dispose();
        }

        [Fact]
        public void Dispose_MultipleTimes()
        {
            controller.Dispose();
            controller.Dispose();
        }
    }
}
