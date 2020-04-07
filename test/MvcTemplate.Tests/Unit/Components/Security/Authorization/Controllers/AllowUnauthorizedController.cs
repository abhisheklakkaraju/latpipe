using System.Diagnostics.CodeAnalysis;

namespace MvcTemplate.Components.Security.Tests
{
    [AllowUnauthorized]
    [ExcludeFromCodeCoverage]
    public class AllowUnauthorizedController : AuthorizeController
    {
    }
}
