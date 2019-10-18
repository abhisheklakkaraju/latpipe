using MvcTemplate.Data.Core;
using MvcTemplate.Objects;
using MvcTemplate.Resources;
using MvcTemplate.Tests;
using NSubstitute;
using System;
using System.Linq;
using Xunit;

namespace MvcTemplate.Validators.Tests
{
    public class BaseValidatorTests : IDisposable
    {
        private BaseValidatorProxy validator;
        private IUnitOfWork unitOfWork;

        public BaseValidatorTests()
        {
            unitOfWork = Substitute.For<IUnitOfWork>();
            validator = new BaseValidatorProxy(unitOfWork);
        }
        public void Dispose()
        {
            unitOfWork.Dispose();
            validator.Dispose();
        }

        [Fact]
        public void BaseValidator_CreatesEmptyModelState()
        {
            Assert.Empty(validator.ModelState);
        }

        [Fact]
        public void BaseValidator_CreatesEmptyAlerts()
        {
            Assert.Empty(validator.Alerts);
        }

        [Fact]
        public void IsSpecified_Null_ReturnsFalse()
        {
            RoleView view = new RoleView();

            Boolean isSpecified = validator.BaseIsSpecified(view, role => role.Title);
            String message = Validation.For("Required", Resource.ForProperty<RoleView, String>(role => role.Title));

            Assert.False(isSpecified);
            Assert.Empty(validator.Alerts);
            Assert.Single(validator.ModelState);
            Assert.Equal(message, validator.ModelState[nameof(RoleView.Title)].Errors.Single().ErrorMessage);
        }

        [Fact]
        public void IsSpecified_NullValue_ReturnsFalse()
        {
            AccountEditView view = new AccountEditView();

            Boolean isSpecified = validator.BaseIsSpecified(view, account => account.RoleId);
            String message = Validation.For("Required", Resource.ForProperty<AccountEditView, Int32?>(account => account.RoleId));

            Assert.False(isSpecified);
            Assert.Empty(validator.Alerts);
            Assert.Single(validator.ModelState);
            Assert.Equal(message, validator.ModelState[nameof(AccountEditView.RoleId)].Errors.Single().ErrorMessage);
        }

        [Fact]
        public void IsSpecified_Valid()
        {
            Assert.True(validator.BaseIsSpecified(ObjectsFactory.CreateRoleView(), role => role.Id));
            Assert.Empty(validator.ModelState);
            Assert.Empty(validator.Alerts);
        }

        [Fact]
        public void Dispose_UnitOfWork()
        {
            validator.Dispose();

            unitOfWork.Received().Dispose();
        }

        [Fact]
        public void Dispose_MultipleTimes()
        {
            validator.Dispose();
            validator.Dispose();
        }
    }
}
