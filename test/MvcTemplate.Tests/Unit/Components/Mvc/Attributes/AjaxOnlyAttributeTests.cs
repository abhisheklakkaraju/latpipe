using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using System;
using Xunit;

namespace MvcTemplate.Components.Mvc.Tests
{
    public class AjaxOnlyAttributeTests
    {
        [Theory]
        [InlineData("", false)]
        [InlineData("XMLHttpRequest", true)]
        public void IsValidForRequest_Ajax(String header, Boolean isValid)
        {
            RouteContext context = new(Substitute.For<HttpContext>());
            context.HttpContext.Request.Headers["X-Requested-With"].Returns(new StringValues(header));

            Boolean actual = new AjaxOnlyAttribute().IsValidForRequest(context, new ActionDescriptor());
            Boolean expected = isValid;

            Assert.Equal(expected, actual);
        }
    }
}
