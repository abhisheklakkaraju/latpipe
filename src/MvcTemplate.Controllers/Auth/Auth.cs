using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcTemplate.Components.Mail;
using MvcTemplate.Objects;
using MvcTemplate.Resources;
using MvcTemplate.Services;
using MvcTemplate.Validators;
using System;
using System.Threading.Tasks;

namespace MvcTemplate.Controllers
{
    [AllowAnonymous]
    public class Auth : ValidatedController<IAccountValidator, IAccountService>
    {
        public IMailClient MailClient { get; }

        public Auth(IAccountValidator validator, IAccountService service, IMailClient mailClient)
            : base(validator, service)
        {
            MailClient = mailClient;
        }

        [HttpGet]
        public ActionResult Recover()
        {
            if (Service.IsLoggedIn(User))
                return RedirectToDefault();

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Recover(AccountRecoveryView account)
        {
            if (Service.IsLoggedIn(User))
                return RedirectToDefault();

            if (!Validator.CanRecover(account))
                return View(account);

            if (Service.Recover(account) is String token)
            {
                String url = Url.Action(nameof(Reset), nameof(Auth), new { token }, Request.Scheme);

                await MailClient.SendAsync(account.Email!,
                    Message.For<AccountView>("RecoveryEmailSubject"),
                    Message.For<AccountView>("RecoveryEmailBody", url));
            }

            Alerts.AddInfo(Message.For<AccountView>("RecoveryInformation"));

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public ActionResult Reset(String? token)
        {
            if (Service.IsLoggedIn(User))
                return RedirectToDefault();

            if (!Validator.CanReset(new AccountResetView { Token = token }))
                return RedirectToAction(nameof(Recover));

            return View();
        }

        [HttpPost]
        public ActionResult Reset(AccountResetView account)
        {
            if (Service.IsLoggedIn(User))
                return RedirectToDefault();

            if (!Validator.CanReset(account))
                return RedirectToAction(nameof(Recover));

            Service.Reset(account);

            Alerts.AddSuccess(Message.For<AccountView>("SuccessfulReset"), 4000);

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public ActionResult Login(String? returnUrl)
        {
            if (Service.IsLoggedIn(User))
                return RedirectToLocal(returnUrl);

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(AccountLoginView account, String? returnUrl)
        {
            if (Service.IsLoggedIn(User))
                return RedirectToLocal(returnUrl);

            if (!Validator.CanLogin(account))
                return View(account);

            await Service.Login(HttpContext, account.Username!);

            return RedirectToLocal(returnUrl);
        }

        [HttpGet]
        public async Task<RedirectToActionResult> Logout()
        {
            await Service.Logout(HttpContext);

            return RedirectToAction(nameof(Login));
        }
    }
}
