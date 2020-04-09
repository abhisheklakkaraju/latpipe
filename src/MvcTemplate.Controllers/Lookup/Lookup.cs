using Microsoft.AspNetCore.Mvc;
using MvcTemplate.Components.Lookups;
using MvcTemplate.Components.Mvc;
using MvcTemplate.Components.Security;
using MvcTemplate.Data;
using MvcTemplate.Objects;
using NonFactors.Mvc.Lookup;
using System;

namespace MvcTemplate.Controllers
{
    [AllowUnauthorized]
    public class Lookup : BaseController
    {
        private IUnitOfWork UnitOfWork { get; }

        public Lookup(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        [AjaxOnly]
        public JsonResult Role(LookupFilter filter)
        {
            return Json(new MvcLookup<Role, RoleView>(UnitOfWork) { Filter = filter }.GetData());
        }

        protected override void Dispose(Boolean disposing)
        {
            UnitOfWork.Dispose();

            base.Dispose(disposing);
        }
    }
}
