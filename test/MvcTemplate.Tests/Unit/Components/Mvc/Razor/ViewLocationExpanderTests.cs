using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using Xunit;

namespace MvcTemplate.Components.Mvc.Tests
{
    public class ViewLocationExpanderTests
    {
        [Fact]
        public void ExpandViewLocations_Area_ReturnsAreaLocations()
        {
            ActionContext context = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
            ViewLocationExpanderContext expander = new ViewLocationExpanderContext(context, "Index", null, null, null, true);
            context.RouteData.Values["area"] = "Test";

            IEnumerable<String> expected = new[] { "/Views/{2}/Shared/{0}.cshtml", "/Views/{2}/{1}/{0}.cshtml", "/Views/Shared/{0}.cshtml" };
            IEnumerable<String> actual = new ViewLocationExpander().ExpandViewLocations(expander, Array.Empty<String>());

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ExpandViewLocations_ReturnsViewLocations()
        {
            ActionContext context = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
            ViewLocationExpanderContext expander = new ViewLocationExpanderContext(context, "Index", null, null, null, true);

            IEnumerable<String> actual = new ViewLocationExpander().ExpandViewLocations(expander, Array.Empty<String>());
            IEnumerable<String> expected = new[] { "/Views/{1}/{0}.cshtml", "/Views/Shared/{0}.cshtml" };

            Assert.Equal(expected, actual);
        }
    }
}
