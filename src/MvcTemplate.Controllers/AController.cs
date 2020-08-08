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
    public abstract class AController : Controller
    {
        public virtual IAuthorization Authorization { get; protected set; }
        public virtual Int64 CurrentAccountId { get; protected set; }
        public Alerts Alerts { get; protected set; }

        protected AController()
        {
            Alerts = new Alerts();
            Authorization = Authorization!;
        }

        public virtual ViewResult NotFoundView()
        {
            Response.StatusCode = StatusCodes.Status404NotFound;

            return View($"~/Views/{nameof(Home)}/{nameof(Home.NotFound)}.cshtml");
        }
        public virtual ViewResult NotEmptyView(Object? model)
        {
            if (model == null)
                return NotFoundView();

            return View(model);
        }
        public virtual ActionResult RedirectToLocal(String? url)
        {
            if (!Url.IsLocalUrl(url))
                return RedirectToDefault();

            return Redirect(url);
        }
        public virtual RedirectToActionResult RedirectToDefault()
        {
            return base.RedirectToAction(nameof(Home.Index), nameof(Home), new { area = "" });
        }

        public virtual Boolean IsAuthorizedFor(String permission)
        {
            return Authorization.IsGrantedFor(CurrentAccountId, permission);
        }

        public override RedirectToActionResult RedirectToAction(String? actionName, String? controllerName, Object? routeValues)
        {
            IDictionary<String, Object> values = HtmlHelper.AnonymousObjectToHtmlAttributes(routeValues);
            controllerName ??= values.ContainsKey("controller") ? values["controller"] as String : null;
            String? area = values.ContainsKey("area") ? values["area"] as String : null;
            controllerName ??= RouteData.Values["controller"] as String;
            actionName ??= RouteData.Values["action"] as String;
            area ??= RouteData.Values["area"] as String;

            if (!IsAuthorizedFor($"{area}/{controllerName}/{actionName}"))
                return RedirectToDefault();

            return base.RedirectToAction(actionName, controllerName, routeValues);
        }

        public override void OnActionExecuting(ActionExecutingContext? context)
        {
            Authorization = HttpContext.RequestServices.GetRequiredService<IAuthorization>();

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
