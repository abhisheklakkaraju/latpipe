using Microsoft.AspNetCore.Mvc;
using MvcTemplate.Components.Security;
using MvcTemplate.Objects;
using MvcTemplate.Resources;
using MvcTemplate.Services;
using MvcTemplate.Validators;

namespace MvcTemplate.Controllers
{
    [AllowUnauthorized]
    public class Profile : ValidatedController<IAccountValidator, IAccountService>
    {
        public Profile(IAccountValidator validator, IAccountService service)
            : base(validator, service)
        {
        }

        [HttpGet]
        public ActionResult Edit()
        {
            if (!Service.IsActive(CurrentAccountId))
                return RedirectToAction(nameof(Auth.Logout), nameof(Auth));

            return View(Service.Get<ProfileEditView>(CurrentAccountId));
        }

        [HttpPost]
        public ActionResult Edit(ProfileEditView profile)
        {
            if (!Service.IsActive(CurrentAccountId))
                return RedirectToAction(nameof(Auth.Logout), nameof(Auth));

            if (!Validator.CanEdit(profile))
                return View(profile);

            Service.Edit(User, profile);

            Alerts.AddSuccess(Message.For<AccountView>("ProfileUpdated"), 4000);

            return RedirectToAction(nameof(Edit));
        }

        [HttpGet]
        public ActionResult Delete()
        {
            if (!Service.IsActive(CurrentAccountId))
                return RedirectToAction(nameof(Auth.Logout), nameof(Auth));

            Alerts.AddWarning(Message.For<AccountView>("ProfileDeleteDisclaimer"));

            return View();
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(ProfileDeleteView profile)
        {
            if (!Service.IsActive(CurrentAccountId))
                return RedirectToAction(nameof(Auth.Logout), nameof(Auth));

            if (!Validator.CanDelete(profile))
            {
                Alerts.AddWarning(Message.For<AccountView>("ProfileDeleteDisclaimer"));

                return View();
            }

            Service.Delete(CurrentAccountId);

            Authorization.Refresh(HttpContext.RequestServices);

            return RedirectToAction(nameof(Auth.Logout), nameof(Auth));
        }
    }
}
