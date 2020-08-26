using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MvcTemplate.Components.Security;
using MvcTemplate.Data;
using MvcTemplate.Objects;
using MvcTemplate.Tests;
using NSubstitute;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Xunit;

namespace MvcTemplate.Services.Tests
{
    public class AccountServiceTests : IDisposable
    {
        private HttpContext httpContext;
        private AccountService service;
        private DbContext context;
        private Account account;
        private IHasher hasher;

        public AccountServiceTests()
        {
            context = TestingContext.Create();
            hasher = Substitute.For<IHasher>();
            httpContext = new DefaultHttpContext();
            service = new AccountService(new UnitOfWork(TestingContext.Create()), hasher);
            hasher.HashPassword(Arg.Any<String>()).Returns(info => $"{info.Arg<String>()}Hashed");

            context.Drop().Add(account = ObjectsFactory.CreateAccount(0));
            context.SaveChanges();

            service.CurrentAccountId = account.Id;
        }
        public void Dispose()
        {
            service.Dispose();
            context.Dispose();
        }

        [Fact]
        public void Get_ReturnsViewById()
        {
            AccountView actual = service.Get<AccountView>(account.Id)!;
            AccountView expected = Mapper.Map<AccountView>(account);

            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.Equal(expected.RoleTitle, actual.RoleTitle);
            Assert.Equal(expected.IsLocked, actual.IsLocked);
            Assert.Equal(expected.Username, actual.Username);
            Assert.Equal(expected.Email, actual.Email);
            Assert.Equal(expected.Id, actual.Id);
        }

        [Fact]
        public void GetViews_ReturnsAccountViews()
        {
            AccountView[] actual = service.GetViews().ToArray();
            AccountView[] expected = context
                .Set<Account>()
                .ProjectTo<AccountView>()
                .OrderByDescending(view => view.Id)
                .ToArray();

            for (Int32 i = 0; i < expected.Length || i < actual.Length; i++)
            {
                Assert.Equal(expected[i].CreationDate, actual[i].CreationDate);
                Assert.Equal(expected[i].RoleTitle, actual[i].RoleTitle);
                Assert.Equal(expected[i].IsLocked, actual[i].IsLocked);
                Assert.Equal(expected[i].Username, actual[i].Username);
                Assert.Equal(expected[i].Email, actual[i].Email);
                Assert.Equal(expected[i].Id, actual[i].Id);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsLoggedIn_ReturnsIsAuthenticated(Boolean isAuthenticated)
        {
            IPrincipal user = Substitute.For<IPrincipal>();
            user.Identity?.IsAuthenticated.Returns(isAuthenticated);

            Boolean actual = service.IsLoggedIn(user);
            Boolean expected = isAuthenticated;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsActive_ReturnsAccountState(Boolean isLocked)
        {
            account.IsLocked = isLocked;
            context.Update(account);
            context.SaveChanges();

            Boolean actual = service.IsActive(account.Id);
            Boolean expected = !isLocked;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsActive_NoAccount_ReturnsFalse()
        {
            Assert.False(service.IsActive(0));
        }

        [Fact]
        public void Recover_NoEmail_ReturnsNull()
        {
            AccountRecoveryView view = ObjectsFactory.CreateAccountRecoveryView(account.Id + 1);
            view.Email = "not@existing.email";

            Assert.Null(service.Recover(view));
        }

        [Fact]
        public void Recover_Information()
        {
            AccountRecoveryView view = ObjectsFactory.CreateAccountRecoveryView(0);
            account.RecoveryTokenExpirationDate = DateTime.Now.AddMinutes(30);
            String? oldToken = account.RecoveryToken;
            view.Email = view.Email?.ToUpper();

            account.RecoveryToken = service.Recover(view);

            Account actual = Assert.Single(context.Set<Account>().AsNoTracking());
            Account expected = account;

            Assert.InRange(actual.RecoveryTokenExpirationDate!.Value.Ticks,
                expected.RecoveryTokenExpirationDate.Value.Ticks - TimeSpan.TicksPerSecond,
                expected.RecoveryTokenExpirationDate.Value.Ticks + TimeSpan.TicksPerSecond);
            Assert.Equal(expected.RecoveryToken, actual.RecoveryToken);
            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.Equal(expected.IsLocked, actual.IsLocked);
            Assert.Equal(expected.Passhash, actual.Passhash);
            Assert.Equal(expected.Username, actual.Username);
            Assert.NotEqual(oldToken, actual.RecoveryToken);
            Assert.Equal(expected.RoleId, actual.RoleId);
            Assert.Equal(expected.Email, actual.Email);
            Assert.Equal(expected.Id, actual.Id);
            Assert.NotNull(actual.RecoveryToken);
        }

        [Fact]
        public void Reset_Account()
        {
            AccountResetView view = ObjectsFactory.CreateAccountResetView(0);

            service.Reset(view);

            Account actual = Assert.Single(context.Set<Account>().AsNoTracking());
            Account expected = account;

            Assert.Equal(hasher.HashPassword(view.NewPassword!), actual.Passhash);
            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.Equal(expected.IsLocked, actual.IsLocked);
            Assert.Equal(expected.Username, actual.Username);
            Assert.Null(actual.RecoveryTokenExpirationDate);
            Assert.Equal(expected.RoleId, actual.RoleId);
            Assert.Equal(expected.Email, actual.Email);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Null(actual.RecoveryToken);
        }

        [Fact]
        public void Create_Account()
        {
            AccountCreateView view = ObjectsFactory.CreateAccountCreateView(account.Id + 1);
            view.Email = view.Email?.ToUpper();
            view.RoleId = account.RoleId;

            service.Create(view);

            Account actual = Assert.Single(context.Set<Account>(), model => model.Id != account.Id);
            AccountCreateView expected = view;

            Assert.Equal(hasher.HashPassword(expected.Password!), actual.Passhash);
            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.Equal(expected.Email?.ToLower(), actual.Email);
            Assert.Equal(expected.Username, actual.Username);
            Assert.Null(actual.RecoveryTokenExpirationDate);
            Assert.Equal(expected.RoleId, actual.RoleId);
            Assert.Null(actual.RecoveryToken);
            Assert.False(actual.IsLocked);
        }

        [Fact]
        public void Edit_Account()
        {
            AccountEditView view = ObjectsFactory.CreateAccountEditView(account.Id);
            view.IsLocked = account.IsLocked = !account.IsLocked;
            view.Username = $"{account.Username}Test";
            view.RoleId = account.RoleId = null;
            view.Email = $"{account.Email}s";

            service.Edit(view);

            Account actual = Assert.Single(context.Set<Account>().AsNoTracking());
            Account expected = account;

            Assert.Equal(expected.RecoveryTokenExpirationDate, actual.RecoveryTokenExpirationDate);
            Assert.Equal(expected.RecoveryToken, actual.RecoveryToken);
            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.Equal(expected.IsLocked, actual.IsLocked);
            Assert.Equal(expected.Username, actual.Username);
            Assert.Equal(expected.Passhash, actual.Passhash);
            Assert.Equal(expected.RoleId, actual.RoleId);
            Assert.Equal(expected.Email, actual.Email);
            Assert.Equal(expected.Id, actual.Id);
        }

        [Fact]
        public void Edit_Profile()
        {
            ProfileEditView view = ObjectsFactory.CreateProfileEditView(account.Id + 1);
            account.Passhash = hasher.HashPassword(view.NewPassword!);
            view.Username = account.Username += "Test";
            view.Email = account.Email += "Test";

            service.Edit(httpContext.User, view);

            Account actual = Assert.Single(context.Set<Account>().AsNoTracking());
            Account expected = account;

            Assert.Equal(expected.RecoveryTokenExpirationDate, actual.RecoveryTokenExpirationDate);
            Assert.Equal(expected.RecoveryToken, actual.RecoveryToken);
            Assert.Equal(expected.CreationDate, actual.CreationDate);
            Assert.Equal(expected.Email.ToLower(), actual.Email);
            Assert.Equal(expected.IsLocked, actual.IsLocked);
            Assert.Equal(expected.Username, actual.Username);
            Assert.Equal(expected.Passhash, actual.Passhash);
            Assert.Equal(expected.RoleId, actual.RoleId);
            Assert.Equal(expected.Id, actual.Id);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void Edit_NullOrEmptyNewPassword_DoesNotEditPassword(String newPassword)
        {
            ProfileEditView view = ObjectsFactory.CreateProfileEditView(account.Id + 1);
            view.NewPassword = newPassword;

            service.Edit(httpContext.User, view);

            String actual = Assert.Single(context.Set<Account>().AsNoTracking()).Passhash;
            String expected = account.Passhash;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Edit_UpdatesClaims()
        {
            httpContext.User.AddIdentity(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, "test@email.com"),
                new Claim(ClaimTypes.Name, "TestName")
            }));

            ProfileEditView view = ObjectsFactory.CreateProfileEditView(account.Id + 1);
            view.Username = account.Username += "Test";
            view.Email = account.Email += "Test";

            service.Edit(httpContext.User, view);

            Account expected = account;
            ClaimsPrincipal actual = httpContext.User;

            Assert.Equal(expected.Username, actual.FindFirstValue(ClaimTypes.Name));
            Assert.Equal(expected.Email.ToLower(), actual.FindFirstValue(ClaimTypes.Email));
        }

        [Fact]
        public void Delete_Account()
        {
            service.Delete(account.Id);

            Assert.Empty(context.Set<Account>());
        }

        [Fact]
        public async Task Login_Account()
        {
            httpContext = Substitute.For<HttpContext>();
            IAuthenticationService authentication = Substitute.For<IAuthenticationService>();
            httpContext.RequestServices.GetService(typeof(IAuthenticationService)).Returns(authentication);

            await service.Login(httpContext, account.Username.ToUpper());

            await authentication.Received().SignInAsync(httpContext, "Cookies", Arg.Is<ClaimsPrincipal>(principal =>
                principal.FindFirstValue(ClaimTypes.NameIdentifier) == account.Id.ToString() &&
                principal.FindFirstValue(ClaimTypes.Name) == account.Username &&
                principal.FindFirstValue(ClaimTypes.Email) == account.Email &&
                principal.Identity!.AuthenticationType == "Password"), null);
        }

        [Fact]
        public async Task Logout_Account()
        {
            httpContext = Substitute.For<HttpContext>();
            IAuthenticationService authentication = Substitute.For<IAuthenticationService>();
            httpContext.RequestServices.GetService(typeof(IAuthenticationService)).Returns(authentication);

            await service.Logout(httpContext);

            await authentication.Received().SignOutAsync(httpContext, "Cookies", null);
        }
    }
}

