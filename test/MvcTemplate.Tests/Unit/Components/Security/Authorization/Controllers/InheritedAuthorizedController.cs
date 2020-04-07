using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace MvcTemplate.Components.Security.Tests
{
    [ExcludeFromCodeCoverage]
    public class InheritedAuthorizedController : AuthorizeController
    {
        [HttpGet]
        public ViewResult InheritanceAction()
        {
            return View();
        }
    }
}
