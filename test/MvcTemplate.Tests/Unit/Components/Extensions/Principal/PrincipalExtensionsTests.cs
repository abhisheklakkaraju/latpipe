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
        public void Email_NoClaim_ReturnsNull()
        {
            Assert.Null(new ClaimsPrincipal().Email());
        }

        [Fact]
        public void Email_ReturnsEmailClaim()
        {
            ClaimsIdentity identity = new ClaimsIdentity();
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            identity.AddClaim(new Claim(ClaimTypes.Email, "ClaimTypeEmail"));

            String? expected = "ClaimTypeEmail";
            String? actual = principal.Email();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Username_NoClaim_ReturnsNull()
        {
            Assert.Null(new ClaimsPrincipal().Username());
        }

        [Fact]
        public void Username_ReturnsNameClaim()
        {
            ClaimsIdentity identity = new ClaimsIdentity();
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            identity.AddClaim(new Claim(ClaimTypes.Name, "ClaimTypeName"));

            String? actual = principal.Username();
            String? expected = "ClaimTypeName";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UpdateClaim_New()
        {
            ClaimsIdentity identity = new ClaimsIdentity();
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            principal.UpdateClaim(ClaimTypes.Name, "Test");

            String actual = principal.FindFirst(ClaimTypes.Name).Value;
            String expected = "Test";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UpdateClaim_Existing()
        {
            ClaimsIdentity identity = new ClaimsIdentity();
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            identity.AddClaim(new Claim(ClaimTypes.Name, "ClaimTypeName"));

            principal.UpdateClaim(ClaimTypes.Name, "Test");

            String actual = principal.FindFirst(ClaimTypes.Name).Value;
            String expected = "Test";

            Assert.Equal(expected, actual);
        }
    }
}
