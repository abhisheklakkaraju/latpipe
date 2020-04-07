using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using MvcTemplate.Components.Mail;
using MvcTemplate.Components.Notifications;
using MvcTemplate.Components.Security;
using MvcTemplate.Objects;
using MvcTemplate.Resources;
using MvcTemplate.Services;
using MvcTemplate.Tests;
using MvcTemplate.Validators;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MvcTemplate.Controllers.Tests
{
    public class AuthTests : ControllerTests
    {
        private AccountRecoveryView accountRecovery;
        private AccountResetView accountReset;
        private AccountLoginView accountLogin;
        private IAccountValidator validator;
        private IAccountService service;
        private IMailClient mail;
        private Auth controller;

        public AuthTests()
        {
            mail = Substitute.For<IMailClient>();
            service = Substitute.For<IAccountService>();
            validator = Substitute.For<IAccountValidator>();
            controller = Substitute.ForPartsOf<Auth>(validator, service, mail);
            controller.ControllerContext.HttpContext = Substitute.For<HttpContext>();
            controller.Authorization.Returns(Substitute.For<IAuthorization>());
            controller.TempData = Substitute.For<ITempDataDictionary>();
            controller.ControllerContext.RouteData = new RouteData();
            controller.Url = Substitute.For<IUrlHelper>();

            accountRecovery = ObjectsFactory.CreateAccountRecoveryView();
            accountReset = ObjectsFactory.CreateAccountResetView();
            accountLogin = ObjectsFactory.CreateAccountLoginView();
        }
        public override void Dispose()
        {
            controller.Dispose();
            validator.Dispose();
            service.Dispose();
        }

        [Fact]
        public void Recover_IsLoggedIn_RedirectsToDefault()
        {
            service.IsLoggedIn(controller.User).Returns(true);

            Object expected = RedirectToDefault(controller);
            Object actual = controller.Recover();

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Recover_ReturnsEmptyView()
        {
            service.IsLoggedIn(controller.User).Returns(false);

            ViewResult actual = Assert.IsType<ViewResult>(controller.Recover());

            Assert.Null(actual.Model);
        }

        [Fact]
        public async Task Recover_Post_IsLoggedIn_RedirectsToDefault()
        {
            service.IsLoggedIn(controller.User).Returns(true);
            validator.CanRecover(accountRecovery).Returns(true);

            Object expected = RedirectToDefault(controller);
            Object actual = await controller.Recover(new AccountRecoveryView());

            Assert.Same(expected, actual);
        }

        [Fact]
        public async Task Recover_CanNotRecover_ReturnsSameView()
        {
            service.IsLoggedIn(controller.User).Returns(false);
            validator.CanRecover(accountRecovery).Returns(false);

            Object actual = Assert.IsType<ViewResult>(await controller.Recover(accountRecovery)).Model;
            Object expected = accountRecovery;

            Assert.Same(expected, actual);
        }

        [Fact]
        public async Task Recover_Account()
        {
            service.IsLoggedIn(controller.User).Returns(false);
            validator.CanRecover(accountRecovery).Returns(true);

            await controller.Recover(accountRecovery);

            service.Received().Recover(accountRecovery);
        }

        [Fact]
        public async Task Recover_SendsRecoveryInformation()
        {
            service.IsLoggedIn(controller.User).Returns(false);
            validator.CanRecover(accountRecovery).Returns(true);
            service.Recover(accountRecovery).Returns("TestToken");

            await controller.Recover(accountRecovery);

            String url = controller.Url.Action(nameof(Auth.Reset), nameof(Auth), new { token = "TestToken" }, controller.Request.Scheme);
            String subject = Message.For<AccountView>("RecoveryEmailSubject");
            String body = Message.For<AccountView>("RecoveryEmailBody", url);
            String email = accountRecovery.Email!;

            await mail.Received().SendAsync(email, subject, body);
        }

        [Fact]
        public async Task Recover_NullToken_DoesNotSendRecoveryInformation()
        {
            service.Recover(accountRecovery).ReturnsNull();
            service.IsLoggedIn(controller.User).Returns(false);
            validator.CanRecover(accountRecovery).Returns(true);

            await controller.Recover(accountRecovery);

            await mail.DidNotReceive().SendAsync(Arg.Any<String>(), Arg.Any<String>(), Arg.Any<String>());
        }

        [Fact]
        public async Task Recover_AddsRecoveryMessage()
        {
            service.IsLoggedIn(controller.User).Returns(false);
            validator.CanRecover(accountRecovery).Returns(true);
            service.Recover(accountRecovery).Returns("UmVjb3Zlcnk=");

            await controller.Recover(accountRecovery);

            Alert actual = controller.Alerts.Single();

            Assert.Equal(Message.For<AccountView>("RecoveryInformation"), actual.Message);
            Assert.Equal(AlertType.Info, actual.Type);
            Assert.Equal(0, actual.Timeout);
        }

        [Fact]
        public async Task Recover_RedirectsToLogin()
        {
            service.IsLoggedIn(controller.User).Returns(false);
            validator.CanRecover(accountRecovery).Returns(true);
            service.Recover(accountRecovery).Returns("UmVjb3Zlcnk=");

            Object expected = RedirectToAction(controller, nameof(Auth.Login));
            Object actual = await controller.Recover(accountRecovery);

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Reset_IsLoggedIn_RedirectsToDefault()
        {
            service.IsLoggedIn(controller.User).Returns(true);

            Object expected = RedirectToDefault(controller);
            Object actual = controller.Reset("");

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Reset_CanNotReset_RedirectsToRecover()
        {
            service.IsLoggedIn(controller.User).Returns(false);
            validator.CanReset(Arg.Any<AccountResetView>()).Returns(false);

            Object expected = RedirectToAction(controller, nameof(Auth.Recover));
            Object actual = controller.Reset("Token");

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Reset_ReturnsEmptyView()
        {
            service.IsLoggedIn(controller.User).Returns(false);
            validator.CanReset(Arg.Any<AccountResetView>()).Returns(true);

            ViewResult actual = Assert.IsType<ViewResult>(controller.Reset(""));

            Assert.Null(actual.Model);
        }

        [Fact]
        public void Reset_Post_IsLoggedIn_RedirectsToDefault()
        {
            service.IsLoggedIn(controller.User).Returns(true);

            Object expected = RedirectToDefault(controller);
            Object actual = controller.Reset(accountReset);

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Reset_Post_CanNotReset_RedirectsToRecover()
        {
            service.IsLoggedIn(controller.User).Returns(false);
            validator.CanReset(accountReset).Returns(false);

            Object expected = RedirectToAction(controller, nameof(Auth.Recover));
            Object actual = controller.Reset(accountReset);

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Reset_Account()
        {
            service.IsLoggedIn(controller.User).Returns(false);
            validator.CanReset(accountReset).Returns(true);

            controller.Reset(accountReset);

            service.Received().Reset(accountReset);
        }

        [Fact]
        public void Reset_AddsResetMessage()
        {
            service.IsLoggedIn(controller.User).Returns(false);
            validator.CanReset(accountReset).Returns(true);

            controller.Reset(accountReset);

            Alert actual = controller.Alerts.Single();

            Assert.Equal(Message.For<AccountView>("SuccessfulReset"), actual.Message);
            Assert.Equal(AlertType.Success, actual.Type);
            Assert.Equal(4000, actual.Timeout);
        }

        [Fact]
        public void Reset_RedirectsToLogin()
        {
            service.IsLoggedIn(controller.User).Returns(false);
            validator.CanReset(accountReset).Returns(true);

            Object expected = RedirectToAction(controller, nameof(Auth.Login));
            Object actual = controller.Reset(accountReset);

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Login_IsLoggedIn_RedirectsToUrl()
        {
            controller.RedirectToLocal(Arg.Is("/")).Returns(new RedirectResult("/"));
            service.IsLoggedIn(controller.User).Returns(true);

            Object expected = controller.RedirectToLocal("/");
            Object actual = controller.Login("/");

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Login_NotLoggedIn_ReturnsEmptyView()
        {
            service.IsLoggedIn(controller.User).Returns(false);

            ViewResult actual = Assert.IsType<ViewResult>(controller.Login("/"));

            Assert.Null(actual.Model);
        }

        [Fact]
        public async Task Login_Post_IsLoggedIn_RedirectsToUrl()
        {
            controller.RedirectToLocal(Arg.Is("/")).Returns(new RedirectResult("/"));
            service.IsLoggedIn(controller.User).Returns(true);

            Object actual = await controller.Login(new AccountLoginView(), "/");
            Object expected = controller.RedirectToLocal("/");

            Assert.Same(expected, actual);
        }

        [Fact]
        public async Task Login_CanNotLogin_ReturnsSameView()
        {
            validator.CanLogin(accountLogin).Returns(false);

            ActionResult result = await controller.Login(accountLogin, null);

            Object actual = Assert.IsType<ViewResult>(result).Model;
            Object expected = accountLogin;

            Assert.Same(expected, actual);
        }

        [Fact]
        public async Task Login_Account()
        {
            validator.CanLogin(accountLogin).Returns(true);
            controller.RedirectToLocal(Arg.Is("/")).Returns(new RedirectResult("/"));

            await controller.Login(accountLogin, null);

            await service.Received().Login(controller.HttpContext, accountLogin.Username!);
        }

        [Fact]
        public async Task Login_RedirectsToUrl()
        {
            validator.CanLogin(accountLogin).Returns(true);
            controller.RedirectToLocal(Arg.Is("/")).Returns(new RedirectResult("/"));

            Object actual = await controller.Login(accountLogin, "/");
            Object expected = controller.RedirectToLocal("/");

            Assert.Same(expected, actual);
        }

        [Fact]
        public async Task Logout_Account()
        {
            await controller.Logout();

            await service.Received().Logout(controller.HttpContext);
        }

        [Fact]
        public async Task Logout_RedirectsToLogin()
        {
            Object expected = RedirectToAction(controller, nameof(Auth.Login));
            Object actual = await controller.Logout();

            Assert.Same(expected, actual);
        }
    }
}
