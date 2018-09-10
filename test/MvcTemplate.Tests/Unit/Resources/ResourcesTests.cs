﻿using MvcTemplate.Data.Core;
using MvcTemplate.Data.Migrations;
using MvcTemplate.Objects;
using MvcTemplate.Tests.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Xml.Linq;
using Xunit;

namespace MvcTemplate.Tests.Unit.Resources
{
    public class ResourcesTests
    {
        [Fact]
        public void Resources_HasAllPermissionAreaTitles()
        {
            ResourceManager manager = MvcTemplate.Resources.Permission.Area.Titles.ResourceManager;

            using (TestingContext context = new TestingContext())
            using (Configuration configuration = new Configuration(context, null))
            {
                configuration.SeedData();

                String[] areas = context
                    .Set<Permission>()
                    .Select(permission => permission.Area)
                    .Distinct()
                    .ToArray();

                foreach (String area in areas)
                    Assert.True(!String.IsNullOrEmpty(manager.GetString(area)), $"'{area}' permission, does not have a title.");
            }
        }

        [Fact]
        public void Resources_HasAllPermissionControllerTitles()
        {
            ResourceManager manager = MvcTemplate.Resources.Permission.Controller.Titles.ResourceManager;

            using (TestingContext context = new TestingContext())
            using (Configuration configuration = new Configuration(context, null))
            {
                configuration.SeedData();

                String[] controllers = context
                    .Set<Permission>()
                    .Select(permission => permission.Area + permission.Controller)
                    .Distinct()
                    .ToArray();

                foreach (String controller in controllers)
                    Assert.True(!String.IsNullOrEmpty(manager.GetString(controller)), $"'{controller}' permission, does not have a title.");
            }
        }

        [Fact]
        public void Resources_HasAllPermissionActionTitles()
        {
            ResourceManager manager = MvcTemplate.Resources.Permission.Action.Titles.ResourceManager;

            using (TestingContext context = new TestingContext())
            using (Configuration configuration = new Configuration(context, null))
            {
                configuration.SeedData();

                String[] actions = context
                    .Set<Permission>()
                    .Select(permission => permission.Area + permission.Controller + permission.Action)
                    .Distinct()
                    .ToArray();

                foreach (String action in actions)
                    Assert.True(!String.IsNullOrEmpty(manager.GetString(action)), $"'{action} permission', does not have a title.");
            }
        }

        [Fact]
        public void Resources_HasAllPageTitles()
        {
            ResourceManager manager = MvcTemplate.Resources.Shared.Pages.ResourceManager;
            IEnumerable<String> sitemap = XDocument
                .Load("Mvc.sitemap")
                .Descendants("siteMapNode")
                .Where(node => node.Attribute("action") != null)
                .Select(node => node.Attribute("area")?.Value + node.Attribute("controller")?.Value + node.Attribute("action")?.Value);

            foreach (String node in sitemap)
                Assert.True(!String.IsNullOrEmpty(manager.GetString(node)), $"'{node}' page, does not have a title.");
        }

        [Fact]
        public void Resources_HasAllSiteMapTitles()
        {
            ResourceManager manager = MvcTemplate.Resources.SiteMap.Titles.ResourceManager;
            IEnumerable<String> sitemap = XDocument
                .Load("Mvc.sitemap")
                .Descendants("siteMapNode")
                .Select(node => node.Attribute("area")?.Value + node.Attribute("controller")?.Value + node.Attribute("action")?.Value);

            foreach (String node in sitemap)
                Assert.True(!String.IsNullOrEmpty(manager.GetString(node)), $"Sitemap node '{node}', does not have a title.");
        }

        [Fact]
        public void Resources_HasEquivalents()
        {
            IEnumerable<CultureInfo> languages = XDocument
                .Load("languages.config")
                .Descendants("language")
                .Select(language => new CultureInfo(language.Attribute("culture").Value));

            IEnumerable<Type> types = Assembly
                .Load("MvcTemplate.Resources")
                .GetTypes()
                .Where(type => type.Namespace.StartsWith("MvcTemplate.Resources."));

            foreach (Type type in types)
            {
                IEnumerable<String> keys = new String[0];
                ResourceManager manager = new ResourceManager(type);

                foreach (ResourceSet set in languages.Select(language => manager.GetResourceSet(language, true, true)))
                    keys = keys.Union(set.Cast<DictionaryEntry>().Select(resource => resource.Key.ToString()));

                foreach (CultureInfo language in languages)
                {
                    ResourceSet set = manager.GetResourceSet(language, true, true);
                    foreach (String key in keys)
                        Assert.True((set.GetObject(key) ?? "").ToString() != "",
                            $"{type.FullName}, does not have translation for '{key}' in {language.EnglishName} language.");
                }
            }
        }
    }
}
