using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
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
            ViewContext context = new();
            IUrlHelper url = Substitute.For<IUrlHelper>();
            IHtmlHelper<T?> html = Substitute.For<IHtmlHelper<T?>>();
            IAuthorization authorization = Substitute.For<IAuthorization>();
            IUrlHelperFactory factory = Substitute.For<IUrlHelperFactory>();

            context.ViewData.Model = model;
            html.ViewContext.Returns(context);
            context.RouteData = new RouteData();
            context.HttpContext = Substitute.For<HttpContext>();
            html.MetadataProvider.Returns(new EmptyModelMetadataProvider());
            url.ActionContext.Returns(new ActionContext { RouteData = context.RouteData });
            context.HttpContext.RequestServices.GetService(typeof(IUrlHelperFactory)).Returns(factory);
            context.HttpContext.RequestServices.GetService(typeof(IAuthorization)).Returns(authorization);

            url.ActionContext.HttpContext = html.ViewContext.HttpContext;
            factory.GetUrlHelper(html.ViewContext).Returns(url);

            return html;
        }
    }
}
