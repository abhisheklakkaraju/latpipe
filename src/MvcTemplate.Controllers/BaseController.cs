using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using MvcTemplate.Components.Extensions;
using MvcTemplate.Components.Notifications;
using MvcTemplate.Components.Security;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace MvcTemplate.Controllers
{
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public abstract class BaseController : Controller
    {
        public virtual Int32 CurrentAccountId { get; protected set; }
        public IAuthorization Authorization { get; protected set; }
        public Alerts Alerts { get; protected set; }

        protected BaseController()
        {
            Alerts = new Alerts();
        }

        public virtual ViewResult NotFoundView()
        {
            Response.StatusCode = StatusCodes.Status404NotFound;

            return View("~/Views/Home/NotFound.cshtml");
        }
        public virtual ViewResult NotEmptyView(Object model)
        {
            if (model == null)
                return NotFoundView();

            return View(model);
        }
        public virtual ActionResult RedirectToLocal(String url)
        {
            if (!Url.IsLocalUrl(url))
                return RedirectToDefault();

            return Redirect(url);
        }
        public virtual RedirectToActionResult RedirectToDefault()
        {
            return base.RedirectToAction("Index", "Home", new { area = "" });
        }
        public override RedirectToActionResult RedirectToAction(String action, String controller, Object route)
        {
            IDictionary<String, Object> values = HtmlHelper.AnonymousObjectToHtmlAttributes(route);
            controller ??= values.ContainsKey("controller") ? values["controller"] as String : null;
            String area = values.ContainsKey("area") ? values["area"] as String : null;
            controller ??= RouteData.Values["controller"] as String;
            area ??= RouteData.Values["area"] as String;

            if (!IsAuthorizedFor(action, controller, area))
                return RedirectToDefault();

            return base.RedirectToAction(action, controller, route);
        }

        public virtual Boolean IsAuthorizedFor(String action, String controller, String area)
        {
            return Authorization?.IsGrantedFor(CurrentAccountId, area, controller, action) != false;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Authorization = HttpContext.RequestServices.GetService<IAuthorization>();

            CurrentAccountId = User.Id() ?? 0;
        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is JsonResult)
                return;

            Alerts alerts = Alerts;

            if (TempData["Alerts"] is String alertsJson)
            {
                alerts = JsonSerializer.Deserialize<Alerts>(alertsJson);
                alerts.Merge(Alerts);
            }

            if (alerts.Count > 0)
                TempData["Alerts"] = JsonSerializer.Serialize(alerts);
        }
    }
}
