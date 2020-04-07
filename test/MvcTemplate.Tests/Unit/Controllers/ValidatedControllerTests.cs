using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MvcTemplate.Components.Mvc;
using MvcTemplate.Services;
using MvcTemplate.Validators;
using NSubstitute;
using System;
using Xunit;

namespace MvcTemplate.Controllers.Tests
{
    public class ValidatedControllerTests : ControllerTests
    {
        private ValidatedController<IValidator, IService> controller;
        private IValidator validator;
        private IService service;

        public ValidatedControllerTests()
        {
            service = Substitute.For<IService>();
            validator = Substitute.For<IValidator>();
            controller = Substitute.ForPartsOf<ValidatedController<IValidator, IService>>(validator, service);

            controller.ControllerContext.RouteData = new RouteData();
            controller.ControllerContext.HttpContext = Substitute.For<HttpContext>();
            controller.HttpContext.RequestServices.GetService(typeof(ILanguages)).Returns(Substitute.For<ILanguages>());
        }
        public override void Dispose()
        {
            controller.Dispose();
            validator.Dispose();
            service.Dispose();
        }

        [Fact]
        public void ValidatedController_SetsValidator()
        {
            Object actual = controller.Validator;
            Object expected = validator;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void OnActionExecuting_SetsServiceCurrentAccountId()
        {
            controller.CurrentAccountId.Returns(1);

            controller.OnActionExecuting(null);

            Int64 expected = controller.CurrentAccountId;
            Int64 actual = service.CurrentAccountId;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void OnActionExecuting_SetsValidatorCurrentAccountId()
        {
            controller.CurrentAccountId.Returns(1);

            controller.OnActionExecuting(null);

            Int64 expected = controller.CurrentAccountId;
            Int64 actual = validator.CurrentAccountId;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void OnActionExecuting_SetsValidatorAlerts()
        {
            controller.OnActionExecuting(null);

            Object expected = controller.Alerts;
            Object actual = validator.Alerts;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void OnActionExecuting_SetsModelState()
        {
            controller.OnActionExecuting(null);

            Object expected = controller.ModelState;
            Object actual = validator.ModelState;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Dispose_Service()
        {
            controller.Dispose();

            service.Received().Dispose();
        }

        [Fact]
        public void Dispose_Validator()
        {
            controller.Dispose();

            validator.Received().Dispose();
        }

        [Fact]
        public void Dispose_MultipleTimes()
        {
            controller.Dispose();
            controller.Dispose();
        }
    }
}
