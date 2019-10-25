using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using MvcTemplate.Components.Security;
using NSubstitute;
using System;

namespace MvcTemplate.Tests
{
    public static class HtmlHelperFactory
    {
        public static IHtmlHelper CreateHtmlHelper()
        {
            return CreateHtmlHelper<Object>(null);
        }
        public static IHtmlHelper<T?> CreateHtmlHelper<T>(T? model) where T : class
        {
            IHtmlHelper<T?> html = Substitute.For<IHtmlHelper<T?>>();

            html.ViewContext.Returns(new ViewContext());
            html.ViewContext.RouteData = new RouteData();
            html.ViewContext.HttpContext = Substitute.For<HttpContext>();
            html.MetadataProvider.Returns(new EmptyModelMetadataProvider());
            html.ViewContext.HttpContext.RequestServices
                .GetService(typeof(IAuthorization))
                .Returns(Substitute.For<IAuthorization>());
            html.ViewContext.ViewData.Model = model;

            return html;
        }
    }
}
