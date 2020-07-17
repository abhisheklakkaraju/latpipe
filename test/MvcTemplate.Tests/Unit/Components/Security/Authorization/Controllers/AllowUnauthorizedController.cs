using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;

namespace MvcTemplate.Components.Security.Tests
{
    [AllowUnauthorized]
    [ExcludeFromCodeCoverage]
    public class AllowUnauthorizedController : AuthorizeController
    {
        [HttpGet]
        public ViewResult AuthorizedAction()
        {
            return View();
        }
    }
}
