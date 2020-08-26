using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MvcTemplate.Components.Extensions;
using MvcTemplate.Components.Security;
using System;

namespace MvcTemplate.Components.Mvc
{
    public class AuthorizationFilter : IAuthorizationFilter
    {
        private IAuthorization Authorization { get; }

        public AuthorizationFilter(IAuthorization authorization)
        {
            Authorization = authorization;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity?.IsAuthenticated != true)
                return;

            Int64? accountId = context.HttpContext.User.Id();
            String? area = context.RouteData.Values["area"] as String;
            String? action = context.RouteData.Values["action"] as String;
            String? controller = context.RouteData.Values["controller"] as String;

            if (!Authorization.IsGrantedFor(accountId, $"{area}/{controller}/{action}"))
                context.Result = new ViewResult
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    ViewName = "~/Views/Home/NotFound.cshtml"
                };
        }
    }
}
