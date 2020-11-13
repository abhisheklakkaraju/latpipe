using Microsoft.AspNetCore.Mvc;
using MvcTemplate.Objects;
using MvcTemplate.Services;
using MvcTemplate.Validators;
using System;

namespace MvcTemplate.Controllers.Administration
{
    [Area(nameof(Area.Administration))]
    public class Roles : ValidatedController<IRoleValidator, IRoleService>
    {
        public Roles(IRoleValidator validator, IRoleService service)
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
            RoleView role = new();

            Service.Seed(role.Permissions);

            return View(role);
        }

        [HttpPost]
        public ActionResult Create(RoleView role)
        {
            if (!Validator.CanCreate(role))
            {
                Service.Seed(role.Permissions);

                return View(role);
            }

            Service.Create(role);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public ActionResult Details(Int64 id)
        {
            return NotEmptyView(Service.GetView(id));
        }

        [HttpGet]
        public ActionResult Edit(Int64 id)
        {
            return NotEmptyView(Service.GetView(id));
        }

        [HttpPost]
        public ActionResult Edit(RoleView role)
        {
            if (!Validator.CanEdit(role))
            {
                Service.Seed(role.Permissions);

                return View(role);
            }

            Service.Edit(role);

            Authorization.Refresh(HttpContext.RequestServices);

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public ActionResult Delete(Int64 id)
        {
            return NotEmptyView(Service.GetView(id));
        }

        [HttpPost]
        [ActionName("Delete")]
        public RedirectToActionResult DeleteConfirmed(Int64 id)
        {
            Service.Delete(id);

            Authorization.Refresh(HttpContext.RequestServices);

            return RedirectToAction(nameof(Index));
        }
    }
}
