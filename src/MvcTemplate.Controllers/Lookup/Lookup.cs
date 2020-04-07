using Microsoft.AspNetCore.Mvc;
using MvcTemplate.Components.Lookups;
using MvcTemplate.Components.Mvc;
using MvcTemplate.Components.Security;
using MvcTemplate.Data.Core;
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

        [NonAction]
        public virtual JsonResult GetData(ALookup lookup, LookupFilter filter)
        {
            lookup.Filter = filter;

            return Json(lookup.GetData());
        }

        [AjaxOnly]
        public JsonResult Role(LookupFilter filter)
        {
            return GetData(new MvcLookup<Role, RoleView>(UnitOfWork), filter);
        }

        protected override void Dispose(Boolean disposing)
        {
            UnitOfWork.Dispose();

            base.Dispose(disposing);
        }
    }
}
