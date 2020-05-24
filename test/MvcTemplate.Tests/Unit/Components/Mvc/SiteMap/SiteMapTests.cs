using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using Xunit;

namespace MvcTemplate.Components.Mvc.Tests
{
    public class SiteMapTests
    {
        private SiteMap siteMap;

        public SiteMapTests()
        {
            siteMap = new SiteMap(CreateSiteMap());
        }

        [Fact]
        public void Tree_Menu()
        {
            SiteMapNode[] actual = siteMap.Tree;

            Assert.Single(actual);

            Assert.Null(actual[0].Action);
            Assert.Null(actual[0].Controller);
            Assert.Equal("Administration", actual[0].Area);
            Assert.Equal("fa fa-cogs", actual[0].IconClass);
            Assert.Equal("Administration//", actual[0].Path);

            actual = actual[0].Children.ToArray();

            Assert.Equal(2, actual.Length);

            Assert.Empty(actual[0].Children);

            Assert.Equal("Index", actual[0].Action);
            Assert.Equal("Accounts", actual[0].Controller);
            Assert.Equal("Administration", actual[0].Area);
            Assert.Equal("fa fa-user", actual[0].IconClass);
            Assert.Equal("Administration/Accounts/Index", actual[0].Path);

            Assert.Null(actual[1].Action);
            Assert.Equal("Roles", actual[1].Controller);
            Assert.Equal("Administration", actual[1].Area);
            Assert.Equal("fa fa-users", actual[1].IconClass);
            Assert.Equal("Administration/Roles/", actual[1].Path);

            actual = actual[1].Children.ToArray();

            Assert.Single(actual);
            Assert.Empty(actual[0].Children);

            Assert.Equal("Create", actual[0].Action);
            Assert.Equal("Roles", actual[0].Controller);
            Assert.Equal("Administration", actual[0].Area);
            Assert.Equal("far fa-file", actual[0].IconClass);
            Assert.Equal("Administration/Roles/Create", actual[0].Path);
        }

        [Fact]
        public void BreadcrumbFor_IsCaseInsensitive()
        {
            ViewContext context = new ViewContext { RouteData = new RouteData() };
            context.RouteData.Values["controller"] = "profile";
            context.RouteData.Values["action"] = "edit";

            SiteMapNode[] actual = siteMap.BreadcrumbFor(context).ToArray();

            Assert.Equal(3, actual.Length);

            Assert.Equal("fa fa-home", actual[0].IconClass);
            Assert.Equal("/Home/Index", actual[0].Path);
            Assert.Equal("Home", actual[0].Controller);
            Assert.Equal("Index", actual[0].Action);
            Assert.Null(actual[0].Area);

            Assert.Equal("fa fa-user", actual[1].IconClass);
            Assert.Equal("Profile", actual[1].Controller);
            Assert.Equal("/Profile/", actual[1].Path);
            Assert.Null(actual[1].Action);
            Assert.Null(actual[1].Area);

            Assert.Equal("fa fa-pencil-alt", actual[2].IconClass);
            Assert.Equal("/Profile/Edit", actual[2].Path);
            Assert.Equal("Profile", actual[2].Controller);
            Assert.Equal("Edit", actual[2].Action);
            Assert.Null(actual[2].Area);
        }

        [Fact]
        public void BreadcrumbFor_NoAction_ReturnsEmpty()
        {
            ViewContext context = new ViewContext { RouteData = new RouteData() };
            context.RouteData.Values["controller"] = "profile";
            context.RouteData.Values["action"] = "edit";
            context.RouteData.Values["area"] = "area";

            Assert.Empty(siteMap.BreadcrumbFor(context));
        }

        private static String CreateSiteMap()
        {
            return @"<siteMap>
                <siteMapNode icon=""fa fa-home"" controller=""Home"" action=""Index"">
                    <siteMapNode icon=""fa fa-user"" controller=""Profile"">
                        <siteMapNode icon=""fa fa-pencil-alt"" action=""Edit"" />
                    </siteMapNode>
                    <siteMapNode menu=""true"" icon=""fa fa-cogs"" area=""Administration"">
                        <siteMapNode menu=""true"" icon=""fa fa-user"" controller=""Accounts"" action=""Index"">
                            <siteMapNode icon=""fa fa-info"" action=""Details"" route-id=""id"">
                                <siteMapNode icon=""fa fa-pencil-alt"" action=""Edit"" />
                            </siteMapNode>
                        </siteMapNode>
                        <siteMapNode menu=""true"" icon=""fa fa-users"" controller=""Roles"">
                            <siteMapNode menu=""true"" icon=""far fa-file"" action=""Create"" />
                            <siteMapNode icon=""fa fa-pencil-alt"" action=""Edit"" />
                        </siteMapNode>
                    </siteMapNode>
                </siteMapNode>
            </siteMap>";
        }
    }
}
