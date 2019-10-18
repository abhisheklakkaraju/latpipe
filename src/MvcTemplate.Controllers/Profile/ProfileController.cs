using Microsoft.AspNetCore.Mvc;
using MvcTemplate.Components.Security;
using MvcTemplate.Objects;
using MvcTemplate.Resources;
using MvcTemplate.Services;
using MvcTemplate.Validators;

namespace MvcTemplate.Controllers
{
    [AllowUnauthorized]
    public class ProfileController : ValidatedController<IAccountValidator, IAccountService>
    {
        public ProfileController(IAccountValidator validator, IAccountService service)
            : base(validator, service)
        {
        }

        [HttpGet]
        public ActionResult Edit()
        {
            if (!Service.IsActive(CurrentAccountId))
                return RedirectToAction("Logout", "Auth");

            return View(Service.Get<ProfileEditView>(CurrentAccountId));
        }

        [HttpPost]
        public ActionResult Edit(ProfileEditView profile)
        {
            if (!Service.IsActive(CurrentAccountId))
                return RedirectToAction("Logout", "Auth");

            if (!Validator.CanEdit(profile))
                return View(profile);

            Service.Edit(User, profile);

            Alerts.AddSuccess(Message.For<AccountView>("ProfileUpdated"), 4000);

            return RedirectToAction("Edit");
        }

        [HttpGet]
        public ActionResult Delete()
        {
            if (!Service.IsActive(CurrentAccountId))
                return RedirectToAction("Logout", "Auth");

            Alerts.AddWarning(Message.For<AccountView>("ProfileDeleteDisclaimer"));

            return View();
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(ProfileDeleteView profile)
        {
            if (!Service.IsActive(CurrentAccountId))
                return RedirectToAction("Logout", "Auth");

            if (!Validator.CanDelete(profile))
            {
                Alerts.AddWarning(Message.For<AccountView>("ProfileDeleteDisclaimer"));

                return View();
            }

            Service.Delete(CurrentAccountId);

            Authorization?.Refresh();

            return RedirectToAction("Logout", "Auth");
        }
    }
}
