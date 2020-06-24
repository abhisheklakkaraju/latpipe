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
    public class AccountsTests : ControllerTests
    {
        private AccountCreateView accountCreate;
        private IAccountValidator validator;
        private AccountEditView accountEdit;
        private IAccountService service;
        private AccountView account;
        private Accounts controller;

        public AccountsTests()
        {
            service = Substitute.For<IAccountService>();
            account = ObjectsFactory.CreateAccountView(0);
            validator = Substitute.For<IAccountValidator>();
            accountEdit = ObjectsFactory.CreateAccountEditView(0);
            accountCreate = ObjectsFactory.CreateAccountCreateView(0);
            controller = Substitute.ForPartsOf<Accounts>(validator, service);

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
        public void Index_ReturnsAccountViews()
        {
            service.GetViews().Returns(Array.Empty<AccountView>().AsQueryable());

            Object actual = controller.Index().Model;
            Object expected = service.GetViews();

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Create_ReturnsEmptyView()
        {
            ViewResult actual = controller.Create();

            Assert.Null(actual.Model);
        }

        [Fact]
        public void Create_CanNotCreate_ReturnsSameView()
        {
            validator.CanCreate(accountCreate).Returns(false);

            Object actual = Assert.IsType<ViewResult>(controller.Create(accountCreate)).Model;
            Object expected = accountCreate;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Create_Account()
        {
            validator.CanCreate(accountCreate).Returns(true);

            controller.Create(accountCreate);

            service.Received().Create(accountCreate);
        }

        [Fact]
        public void Create_RefreshesAuthorization()
        {
            validator.CanCreate(accountCreate).Returns(true);

            controller.Create(accountCreate);

            controller.Authorization.Received().Refresh(controller.HttpContext.RequestServices);
        }

        [Fact]
        public void Create_RedirectsToIndex()
        {
            validator.CanCreate(accountCreate).Returns(true);

            Object expected = RedirectToAction(controller, nameof(Accounts.Index));
            Object actual = controller.Create(accountCreate);

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Details_ReturnsNotEmptyView()
        {
            service.Get<AccountView>(account.Id).Returns(account);

            Object expected = NotEmptyView(controller, account);
            Object actual = controller.Details(account.Id);

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Edit_ReturnsNotEmptyView()
        {
            service.Get<AccountEditView>(accountEdit.Id).Returns(accountEdit);

            Object expected = NotEmptyView(controller, accountEdit);
            Object actual = controller.Edit(accountEdit.Id);

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Edit_CanNotEdit_ReturnsSameView()
        {
            validator.CanEdit(accountEdit).Returns(false);

            Object actual = Assert.IsType<ViewResult>(controller.Edit(accountEdit)).Model;
            Object expected = accountEdit;

            Assert.Same(expected, actual);
        }

        [Fact]
        public void Edit_Account()
        {
            validator.CanEdit(accountEdit).Returns(true);

            controller.Edit(accountEdit);

            service.Received().Edit(accountEdit);
        }

        [Fact]
        public void Edit_RefreshesAuthorization()
        {
            validator.CanEdit(accountEdit).Returns(true);

            controller.Edit(accountEdit);

            controller.Authorization.Received().Refresh(controller.HttpContext.RequestServices);
        }

        [Fact]
        public void Edit_RedirectsToIndex()
        {
            validator.CanEdit(accountEdit).Returns(true);

            Object expected = RedirectToAction(controller, nameof(Accounts.Index));
            Object actual = controller.Edit(accountEdit);

            Assert.Same(expected, actual);
        }
    }
}
