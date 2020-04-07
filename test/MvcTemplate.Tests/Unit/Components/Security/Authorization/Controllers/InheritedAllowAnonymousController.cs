using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace MvcTemplate.Components.Security.Tests
{
    [ExcludeFromCodeCoverage]
    public class InheritedAllowAnonymousController : AllowAnonymousController
    {
        [HttpGet]
        public ViewResult InheritanceAction()
        {
            return View();
        }
    }
}
