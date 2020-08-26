using System;
using System.Security.Claims;
using Xunit;

namespace MvcTemplate.Components.Extensions.Tests
{
    public class ClaimsPrincipalExtensionsTests
    {
        [Fact]
        public void Id_NoClaim_ReturnsNull()
        {
            Assert.Null(new ClaimsPrincipal().Id());
        }

        [Theory]
        [InlineData("1", 1)]
        [InlineData("", null)]
        public void Id_ReturnsNameIdentifierClaim(String identifier, Int64? id)
        {
            ClaimsIdentity identity = new ClaimsIdentity();
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, identifier));

            Int64? actual = principal.Id();
            Int64? expected = id;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UpdateClaim_New()
        {
            ClaimsIdentity identity = new ClaimsIdentity();
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            principal.UpdateClaim(ClaimTypes.Name, "Test");

            String? actual = principal.FindFirstValue(ClaimTypes.Name);
            String? expected = "Test";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UpdateClaim_Existing()
        {
            ClaimsIdentity identity = new ClaimsIdentity();
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            identity.AddClaim(new Claim(ClaimTypes.Name, "ClaimTypeName"));

            principal.UpdateClaim(ClaimTypes.Name, "Test");

            String? actual = principal.FindFirstValue(ClaimTypes.Name);
            String? expected = "Test";

            Assert.Equal(expected, actual);
        }
    }
}
