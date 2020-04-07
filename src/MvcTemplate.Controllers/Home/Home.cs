using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MvcTemplate.Components.Notifications;
using MvcTemplate.Components.Security;
using MvcTemplate.Resources;
using MvcTemplate.Services;

namespace MvcTemplate.Controllers
{
    [AllowUnauthorized]
    public class Home : ServicedController<IAccountService>
    {
        public Home(IAccountService service)
            : base(service)
        {
        }

        [HttpGet]
        public ActionResult Index()
        {
            if (!Service.IsActive(CurrentAccountId))
                return RedirectToAction(nameof(Auth.Logout), nameof(Auth));

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Error()
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                Alerts.Add(new Alert
                {
                    Id = "SystemError",
                    Type = AlertType.Danger,
                    Message = Resource.ForString("SystemError", HttpContext.TraceIdentifier)
                });

                return Json(new { alerts = Alerts });
            }

            return View();
        }

        [AllowAnonymous]
        [HttpGet("[controller]/not-found")]
        [HttpGet("{language}/[controller]/not-found")]
        public new ActionResult NotFound()
        {
            if (Service.IsLoggedIn(User) && !Service.IsActive(CurrentAccountId))
                return RedirectToAction(nameof(Auth.Logout), nameof(Auth));

            Response.StatusCode = StatusCodes.Status404NotFound;

            return View();
        }
    }
}
