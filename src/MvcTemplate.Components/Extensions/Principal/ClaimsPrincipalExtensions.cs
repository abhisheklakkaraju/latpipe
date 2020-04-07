using System;
using System.Security.Claims;

namespace MvcTemplate.Components.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Int64? Id(this ClaimsPrincipal principal)
        {
            String? id = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (String.IsNullOrEmpty(id))
                return null;

            return Int64.Parse(id);
        }
        public static String? Email(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(ClaimTypes.Email)?.Value;
        }
        public static String? Username(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(ClaimTypes.Name)?.Value;
        }

        public static void UpdateClaim(this ClaimsPrincipal principal, String type, String value)
        {
            ClaimsIdentity identity = (ClaimsIdentity)principal.Identity;
            identity.TryRemoveClaim(identity.FindFirst(type));
            identity.AddClaim(new Claim(type, value));
        }
    }
}
