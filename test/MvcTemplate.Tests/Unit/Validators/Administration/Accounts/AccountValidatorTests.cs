using Microsoft.EntityFrameworkCore;
using MvcTemplate.Components.Notifications;
using MvcTemplate.Components.Security;
using MvcTemplate.Data;
using MvcTemplate.Objects;
using MvcTemplate.Resources;
using MvcTemplate.Tests;
using NSubstitute;
using System;
using System.Linq;
using Xunit;

namespace MvcTemplate.Validators.Tests
{
    public class AccountValidatorTests : IDisposable
    {
        private AccountValidator validator;
        private DbContext context;
        private Account account;
        private IHasher hasher;

        public AccountValidatorTests()
        {
            context = TestingContext.Create();
            hasher = Substitute.For<IHasher>();
            hasher.VerifyPassword(Arg.Any<String>(), Arg.Any<String>()).Returns(true);
            validator = new AccountValidator(new UnitOfWork(TestingContext.Create()), hasher);

            context.Drop().Add(account = ObjectsFactory.CreateAccount(0));
            context.SaveChanges();

            validator.CurrentAccountId = account.Id;
        }
        public void Dispose()
        {
            validator.Dispose();
            context.Dispose();
        }

        [Fact]
        public void CanRecover_InvalidState_ReturnsFalse()
        {
            validator.ModelState.AddModelError("Test", "Test");

            Assert.False(validator.CanRecover(ObjectsFactory.CreateAccountRecoveryView(0)));
            Assert.Single(validator.ModelState);
            Assert.Empty(validator.Alerts);
        }

        [Fact]
        public void CanRecover_ValidAccount()
        {
            Assert.True(validator.CanRecover(ObjectsFactory.CreateAccountRecoveryView(0)));
            Assert.Empty(validator.ModelState);
            Assert.Empty(validator.Alerts);
        }

        [Fact]
        public void CanReset_InvalidState_ReturnsFalse()
        {
            validator.ModelState.AddModelError("Test", "Test");

            Assert.False(validator.CanReset(ObjectsFactory.CreateAccountResetView(0)));
            Assert.Single(validator.ModelState);
            Assert.Empty(validator.Alerts);
        }

        [Fact]
        public void CanReset_ExpiredToken_ReturnsFalse()
        {
            account.RecoveryTokenExpiration = DateTime.Now.AddMinutes(-5);
            context.Update(account);
            context.SaveChanges();

            Boolean canReset = validator.CanReset(ObjectsFactory.CreateAccountResetView(account.Id + 1));
            Alert alert = validator.Alerts.Single();

            Assert.False(canReset);
            Assert.Equal(0, alert.Timeout);
            Assert.Empty(validator.ModelState);
            Assert.Equal(AlertType.Danger, alert.Type);
            Assert.Equal(Validation.For<AccountView>("ExpiredToken"), alert.Message);
        }

        [Fact]
        public void CanReset_ValidAccount()
        {
            Assert.True(validator.CanReset(ObjectsFactory.CreateAccountResetView(0)));
            Assert.Empty(validator.ModelState);
            Assert.Empty(validator.Alerts);
        }

        [Fact]
        public void CanLogin_InvalidState_ReturnsFalse()
        {
            validator.ModelState.AddModelError("Test", "Test");

            Assert.False(validator.CanLogin(ObjectsFactory.CreateAccountLoginView(0)));
            Assert.Single(validator.ModelState);
            Assert.Empty(validator.Alerts);
        }

        [Fact]
        public void CanLogin_NoAccount_ReturnsFalse()
        {
            AccountLoginView view = new();
            hasher.VerifyPassword(null, null).Returns(false);

            Boolean canLogin = validator.CanLogin(view);
            Alert alert = validator.Alerts.Single();

            Assert.False(canLogin);
            Assert.Equal(0, alert.Timeout);
            Assert.Empty(validator.ModelState);
            Assert.Equal(AlertType.Danger, alert.Type);
            Assert.Equal(Validation.For<AccountView>("IncorrectAuthentication"), alert.Message);
        }

        [Fact]
        public void CanLogin_IncorrectPassword_ReturnsFalse()
        {
            account = context.Set<Account>().Single();
            account.IsLocked = true;
            context.SaveChanges();

            AccountLoginView view = ObjectsFactory.CreateAccountLoginView(account.Id + 1);
            hasher.VerifyPassword(view.Password, Arg.Any<String>()).Returns(false);

            Boolean canLogin = validator.CanLogin(view);
            Alert alert = validator.Alerts.Single();

            Assert.False(canLogin);
            Assert.Equal(0, alert.Timeout);
            Assert.Empty(validator.ModelState);
            Assert.Equal(AlertType.Danger, alert.Type);
            Assert.Equal(Validation.For<AccountView>("IncorrectAuthentication"), alert.Message);
        }

        [Fact]
        public void CanLogin_LockedAccount_ReturnsFalse()
        {
            AccountLoginView view = ObjectsFactory.CreateAccountLoginView(account.Id + 1);
            account = context.Set<Account>().Single();
            account.IsLocked = true;
            context.SaveChanges();

            Boolean canLogin = validator.CanLogin(view);
            Alert alert = validator.Alerts.Single();

            Assert.False(canLogin);
            Assert.Equal(0, alert.Timeout);
            Assert.Empty(validator.ModelState);
            Assert.Equal(AlertType.Danger, alert.Type);
            Assert.Equal(Validation.For<AccountView>("LockedAccount"), alert.Message);
        }

        [Fact]
        public void CanLogin_IsCaseInsensitive()
        {
            AccountLoginView view = ObjectsFactory.CreateAccountLoginView(0);
            view.Username = view.Username.ToUpper();

            Assert.True(validator.CanLogin(view));
        }

        [Fact]
        public void CanLogin_ValidAccount()
        {
            Assert.True(validator.CanLogin(ObjectsFactory.CreateAccountLoginView(0)));
            Assert.Empty(validator.ModelState);
            Assert.Empty(validator.Alerts);
        }

        [Fact]
        public void CanCreate_InvalidState_ReturnsFalse()
        {
            validator.ModelState.AddModelError("Test", "Test");

            Assert.False(validator.CanCreate(ObjectsFactory.CreateAccountCreateView(account.Id + 1)));
            Assert.Single(validator.ModelState);
            Assert.Empty(validator.Alerts);
        }

        [Fact]
        public void CanCreate_UsedUsername_ReturnsFalse()
        {
            AccountCreateView view = ObjectsFactory.CreateAccountCreateView(account.Id + 1);
            view.Username = account.Username.ToLower();
            view.Id = account.Id;

            Boolean canCreate = validator.CanCreate(view);

            Assert.False(canCreate);
            Assert.Empty(validator.Alerts);
            Assert.Single(validator.ModelState);
            Assert.Equal(Validation.For<AccountView>("UniqueUsername"), validator.ModelState[nameof(AccountCreateView.Username)].Errors.Single().ErrorMessage);
        }

        [Fact]
        public void CanCreate_UsedEmail_ReturnsFalse()
        {
            AccountCreateView view = ObjectsFactory.CreateAccountCreateView(account.Id + 1);
            view.Email = account.Email.ToUpper();
            view.Id = account.Id;

            Boolean canCreate = validator.CanCreate(view);

            Assert.False(canCreate);
            Assert.Empty(validator.Alerts);
            Assert.Single(validator.ModelState);
            Assert.Equal(Validation.For<AccountView>("UniqueEmail"), validator.ModelState[nameof(AccountCreateView.Email)].Errors.Single().ErrorMessage);
        }

        [Fact]
        public void CanCreate_ValidAccount()
        {
            Assert.True(validator.CanCreate(ObjectsFactory.CreateAccountCreateView(account.Id + 1)));
            Assert.Empty(validator.ModelState);
            Assert.Empty(validator.Alerts);
        }

        [Fact]
        public void CanEdit_Account_InvalidState_ReturnsFalse()
        {
            validator.ModelState.AddModelError("Test", "Test");

            Assert.False(validator.CanEdit(ObjectsFactory.CreateAccountEditView(account.Id + 1)));
            Assert.Single(validator.ModelState);
            Assert.Empty(validator.Alerts);
        }

        [Fact]
        public void CanEdit_Account_UsedUsername_ReturnsFalse()
        {
            AccountEditView view = ObjectsFactory.CreateAccountEditView(account.Id + 1);
            view.Username = account.Username.ToLower();

            Boolean canEdit = validator.CanEdit(view);

            Assert.False(canEdit);
            Assert.Empty(validator.Alerts);
            Assert.Single(validator.ModelState);
            Assert.Equal(Validation.For<AccountView>("UniqueUsername"), validator.ModelState[nameof(AccountEditView.Username)].Errors.Single().ErrorMessage);
        }

        [Fact]
        public void CanEdit_Account_ToSameUsername()
        {
            AccountEditView view = ObjectsFactory.CreateAccountEditView(account.Id);
            view.Username = account.Username.ToUpper();

            Assert.True(validator.CanEdit(view));
        }

        [Fact]
        public void CanEdit_Account_UsedEmail_ReturnsFalse()
        {
            AccountEditView view = ObjectsFactory.CreateAccountEditView(account.Id + 1);
            view.Email = account.Email.ToUpper();

            Boolean canEdit = validator.CanEdit(view);

            Assert.False(canEdit);
            Assert.Empty(validator.Alerts);
            Assert.Single(validator.ModelState);
            Assert.Equal(Validation.For<AccountView>("UniqueEmail"), validator.ModelState[nameof(AccountEditView.Email)].Errors.Single().ErrorMessage);
        }

        [Fact]
        public void CanEdit_Account_ToSameEmail()
        {
            AccountEditView view = ObjectsFactory.CreateAccountEditView(account.Id);
            view.Email = account.Email.ToUpper();

            Assert.True(validator.CanEdit(view));
        }

        [Fact]
        public void CanEdit_ValidAccount()
        {
            Assert.True(validator.CanEdit(ObjectsFactory.CreateAccountEditView(account.Id)));
            Assert.Empty(validator.ModelState);
            Assert.Empty(validator.Alerts);
        }

        [Fact]
        public void CanEdit_Profile_InvalidState_ReturnsFalse()
        {
            validator.ModelState.AddModelError("Test", "Test");

            Assert.False(validator.CanEdit(ObjectsFactory.CreateProfileEditView(account.Id + 1)));
            Assert.Single(validator.ModelState);
            Assert.Empty(validator.Alerts);
        }

        [Fact]
        public void CanEdit_Profile_IncorrectPassword_ReturnsFalse()
        {
            ProfileEditView view = ObjectsFactory.CreateProfileEditView(account.Id + 1);
            hasher.VerifyPassword(view.Password, Arg.Any<String>()).Returns(false);

            Boolean canEdit = validator.CanEdit(view);

            Assert.False(canEdit);
            Assert.Empty(validator.Alerts);
            Assert.Single(validator.ModelState);
            Assert.Equal(Validation.For<AccountView>("IncorrectPassword"), validator.ModelState[nameof(ProfileEditView.Password)].Errors.Single().ErrorMessage);
        }

        [Fact]
        public void CanEdit_Profile_UsedUsername_ReturnsFalse()
        {
            Account usedAccount = ObjectsFactory.CreateAccount(account.Id + 1);
            context.Add(usedAccount);
            context.SaveChanges();

            ProfileEditView view = ObjectsFactory.CreateProfileEditView(usedAccount.Id + 1);
            view.Username = usedAccount.Username.ToLower();

            Boolean canEdit = validator.CanEdit(view);

            Assert.False(canEdit);
            Assert.Empty(validator.Alerts);
            Assert.Single(validator.ModelState);
            Assert.Equal(Validation.For<AccountView>("UniqueUsername"), validator.ModelState[nameof(ProfileEditView.Username)].Errors.Single().ErrorMessage);
        }

        [Fact]
        public void CanEdit_Profile_ToSameUsername()
        {
            ProfileEditView view = ObjectsFactory.CreateProfileEditView(account.Id + 1);
            view.Username = account.Username.ToUpper();

            Assert.True(validator.CanEdit(view));
        }

        [Fact]
        public void CanEdit_Profile_UsedEmail_ReturnsFalse()
        {
            Account usedAccount = ObjectsFactory.CreateAccount(account.Id + 1);
            context.Add(usedAccount);
            context.SaveChanges();

            ProfileEditView view = ObjectsFactory.CreateProfileEditView(0);
            view.Email = usedAccount.Email;

            Boolean canEdit = validator.CanEdit(view);

            Assert.False(canEdit);
            Assert.Empty(validator.Alerts);
            Assert.Single(validator.ModelState);
            Assert.Equal(Validation.For<AccountView>("UniqueEmail"), validator.ModelState[nameof(ProfileEditView.Email)].Errors.Single().ErrorMessage);
        }

        [Fact]
        public void CanEdit_Profile_ToSameEmail()
        {
            ProfileEditView view = ObjectsFactory.CreateProfileEditView(account.Id + 1);
            view.Email = account.Email.ToUpper();

            Assert.True(validator.CanEdit(view));
        }

        [Fact]
        public void CanEdit_ValidProfile()
        {
            Assert.True(validator.CanEdit(ObjectsFactory.CreateProfileEditView(account.Id + 1)));
            Assert.Empty(validator.ModelState);
            Assert.Empty(validator.Alerts);
        }

        [Fact]
        public void CanDelete_InvalidState_ReturnsFalse()
        {
            validator.ModelState.AddModelError("Test", "Test");

            Assert.False(validator.CanDelete(ObjectsFactory.CreateProfileDeleteView(account.Id + 1)));
            Assert.Single(validator.ModelState);
            Assert.Empty(validator.Alerts);
        }

        [Fact]
        public void CanDelete_IncorrectPassword_ReturnsFalse()
        {
            ProfileDeleteView view = ObjectsFactory.CreateProfileDeleteView(account.Id + 1);
            hasher.VerifyPassword(view.Password, Arg.Any<String>()).Returns(false);

            Boolean canDelete = validator.CanDelete(view);

            Assert.False(canDelete);
            Assert.Empty(validator.Alerts);
            Assert.Single(validator.ModelState);
            Assert.Equal(Validation.For<AccountView>("IncorrectPassword"), validator.ModelState["Password"].Errors.Single().ErrorMessage);
        }

        [Fact]
        public void CanDelete_ValidProfile()
        {
            Assert.True(validator.CanDelete(ObjectsFactory.CreateProfileDeleteView(account.Id + 1)));
            Assert.Empty(validator.ModelState);
            Assert.Empty(validator.Alerts);
        }
    }
}
