using Microsoft.AspNetCore.Mvc;
using MvcTemplate.Objects;
using MvcTemplate.Services;
using MvcTemplate.Validators;
using System;

namespace MvcTemplate.Controllers.Administration
{
    [Area(nameof(Area.Administration))]
    public class Accounts : ValidatedController<IAccountValidator, IAccountService>
    {
        public Accounts(IAccountValidator validator, IAccountService service)
            : base(validator, service)
        {
        }

        [HttpGet]
        public ViewResult Index()
        {
            return View(Service.GetViews());
        }

        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(AccountCreateView account)
        {
            if (!Validator.CanCreate(account))
                return View(account);

            Service.Create(account);

            Authorization.Refresh(HttpContext.RequestServices);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public ActionResult Details(Int64 id)
        {
            return NotEmptyView(Service.Get<AccountView>(id));
        }

        [HttpGet]
        public ActionResult Edit(Int64 id)
        {
            return NotEmptyView(Service.Get<AccountEditView>(id));
        }

        [HttpPost]
        public ActionResult Edit(AccountEditView account)
        {
            if (!Validator.CanEdit(account))
                return View(account);

            Service.Edit(account);

            Authorization.Refresh(HttpContext.RequestServices);

            return RedirectToAction(nameof(Index));
        }
    }
}
