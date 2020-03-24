using MvcTemplate.Components.Mvc;
using MvcTemplate.Data.Migrations;
using MvcTemplate.Objects;
using MvcTemplate.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Xunit;

namespace MvcTemplate.Resources.Tests
{
    public class ResourcesTests
    {
        [Fact]
        public void Resources_HasAllPageTitles()
        {
            XElement sitemap = XDocument.Load("../../../../../src/MvcTemplate.Web/mvc.sitemap").Element("siteMap");

            foreach (SiteMapNode node in Flatten(sitemap.Elements()).Where(node => node.Action != null))
            {
                String path = $"{node.Area}{node.Controller}{node.Action}";

                Assert.True(!String.IsNullOrEmpty(Resource.ForPage(path)), $"'{path}' page, does not have a title.");
            }
        }

        [Fact]
        public void Resources_HasAllSiteMapTitles()
        {
            XElement sitemap = XDocument.Load("../../../../../src/MvcTemplate.Web/mvc.sitemap").Element("siteMap");

            foreach (SiteMapNode node in Flatten(sitemap.Elements()))
                Assert.True(!String.IsNullOrEmpty(Resource.ForSiteMap(node.Area, node.Controller, node.Action)),
                    $"Sitemap node '{node}' page, does not have a title.");
        }

        [Fact]
        public void Resources_HasAllPermissionAreaTitles()
        {
            using TestingContext context = new TestingContext();
            using Configuration configuration = new Configuration(context, null);

            configuration.Seed();

            foreach (Permission permission in context.Set<Permission>().Where(permission => permission.Area != null))
                Assert.True(!String.IsNullOrEmpty(Resource.ForArea(permission.Area!)),
                    $"'{permission.Area}' permission, does not have a title.");
        }

        [Fact]
        public void Resources_HasAllPermissionControllerTitles()
        {
            using TestingContext context = new TestingContext();
            using Configuration configuration = new Configuration(context, null);

            configuration.Seed();

            foreach (Permission permission in context.Set<Permission>())
                Assert.True(!String.IsNullOrEmpty(Resource.ForController(permission.Area + permission.Controller)),
                    $"'{permission.Area}{permission.Controller}' permission, does not have a title.");
        }

        [Fact]
        public void Resources_HasAllPermissionActionTitles()
        {
            using TestingContext context = new TestingContext();
            using Configuration configuration = new Configuration(context, null);

            configuration.Seed();

            foreach (Permission permission in context.Set<Permission>())
                Assert.True(!String.IsNullOrEmpty(Resource.ForAction(permission.Action)),
                    $"'{permission.Area}{permission.Controller}{permission.Action} permission', does not have a title.");
        }

        private List<SiteMapNode> Flatten(IEnumerable<XElement> elements, SiteMapNode? parent = null)
        {
            List<SiteMapNode> list = new List<SiteMapNode>();
            foreach (XElement element in elements)
            {
                SiteMapNode node = new SiteMapNode
                {
                    Action = (String)element.Attribute("action"),
                    Area = (String)element.Attribute("area") ?? parent?.Area,
                    Controller = (String)element.Attribute("controller") ?? (element.Attribute("area") == null ? parent?.Controller : null)
                };

                list.Add(node);
                list.AddRange(Flatten(element.Elements(), node));
            }

            return list;
        }
    }
}
