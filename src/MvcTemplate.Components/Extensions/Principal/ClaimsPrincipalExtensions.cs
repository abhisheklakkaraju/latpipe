using System;
using System.Security.Claims;

namespace MvcTemplate.Components.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Int64? Id(this ClaimsPrincipal principal)
        {
            String? id = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (String.IsNullOrEmpty(id))
                return null;

            return Int64.Parse(id);
        }

        public static void UpdateClaim(this ClaimsPrincipal principal, String type, String value)
        {
            ClaimsIdentity? identity = (ClaimsIdentity?)principal.Identity;
            identity?.TryRemoveClaim(identity.FindFirst(type));
            identity?.AddClaim(new Claim(type, value));
        }
    }
}
