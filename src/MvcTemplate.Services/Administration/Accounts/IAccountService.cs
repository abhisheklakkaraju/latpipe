using Microsoft.AspNetCore.Http;
using MvcTemplate.Objects;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace MvcTemplate.Services
{
    public interface IAccountService : IService
    {
        TView? Get<TView>(Int64 id) where TView : AView;
        IQueryable<AccountView> GetViews();

        Boolean IsLoggedIn(IPrincipal user);
        Boolean IsActive(Int64 id);

        String? Recover(AccountRecoveryView view);
        void Reset(AccountResetView view);

        void Create(AccountCreateView view);
        void Edit(AccountEditView view);

        void Edit(ClaimsPrincipal user, ProfileEditView view);
        void Delete(Int64 id);

        Task Login(HttpContext context, String username);
        Task Logout(HttpContext context);
    }
}
