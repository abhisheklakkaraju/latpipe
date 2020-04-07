using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace MvcTemplate.Components.Security.Tests
{
    [AllowAnonymous]
    [ExcludeFromCodeCoverage]
    public class AllowAnonymousController : AuthorizeController
    {
        [HttpGet]
        [Authorize]
        [AllowAnonymous]
        [AllowUnauthorized]
        public ViewResult AuthorizedAction()
        {
            return View();
        }
    }
}
