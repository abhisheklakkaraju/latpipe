using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MvcTemplate.Components.Extensions;
using MvcTemplate.Components.Security;
using MvcTemplate.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MvcTemplate.Components.Mvc
{
    public class SiteMap : ISiteMap
    {
        private List<SiteMapNode> Tree { get; }
        private IAuthorization? Authorization { get; }
        private Dictionary<String, SiteMapNode> Lookup { get; }

        public SiteMap(String map, IAuthorization? authorization)
        {
            Authorization = authorization;
            Tree = Parse(XElement.Parse(map));
            Lookup = Flatten(Tree).ToDictionary(node => $"{node.Area}/{node.Controller}/{node.Action}", StringComparer.OrdinalIgnoreCase);
        }

        public IEnumerable<SiteMapNode> For(ViewContext context)
        {
            Int32? account = context.HttpContext.User.Id();
            IUrlHelperFactory factory = context.HttpContext.RequestServices.GetService<IUrlHelperFactory>();
            List<SiteMapNode> nodes = SetState(Tree, factory.GetUrlHelper(context), CurrentNodeFor(context.RouteData.Values));

            return Authorize(account, nodes);
        }
        public IEnumerable<SiteMapNode> BreadcrumbFor(ViewContext context)
        {
            IUrlHelperFactory factory = context.HttpContext.RequestServices.GetService<IUrlHelperFactory>();
            SiteMapNode? current = CurrentNodeFor(context.RouteData.Values);
            List<SiteMapNode> breadcrumb = new List<SiteMapNode>();
            IUrlHelper url = factory.GetUrlHelper(context);

            while (current != null)
            {
                breadcrumb.Insert(0, new SiteMapNode
                {
                    Title = current.Title,
                    Url = FormUrl(url, current),
                    IconClass = current.IconClass,

                    Controller = current.Controller,
                    Action = current.Action,
                    Area = current.Area
                });

                current = current.Parent;
            }

            return breadcrumb;
        }

        private List<SiteMapNode> SetState(IEnumerable<SiteMapNode> nodes, IUrlHelper url, SiteMapNode? current)
        {
            List<SiteMapNode> copies = new List<SiteMapNode>();

            foreach (SiteMapNode node in nodes)
            {
                SiteMapNode copy = new SiteMapNode();
                copy.IconClass = node.IconClass;
                copy.Url = FormUrl(url, node);
                copy.IsMenu = node.IsMenu;
                copy.Title = node.Title;

                copy.Controller = node.Controller;
                copy.Action = node.Action;
                copy.Area = node.Area;

                copy.Children = SetState(node.Children, url, current);
                copy.HasActiveChildren = copy.Children.Any(child => child.IsActive || child.HasActiveChildren);
                copy.IsActive = copy.Children.Any(child => child.IsActive && !child.IsMenu) || node == current;

                copies.Add(copy);
            }

            return copies;
        }
        private List<SiteMapNode> Authorize(Int32? accountId, IEnumerable<SiteMapNode> nodes)
        {
            List<SiteMapNode> authorized = new List<SiteMapNode>();
            foreach (SiteMapNode node in nodes)
            {
                node.Children = Authorize(accountId, node.Children);

                if (node.IsMenu && IsAuthorizedFor(accountId, node.Area, node.Controller, node.Action) && !IsEmpty(node))
                    authorized.Add(node);
                else
                    authorized.AddRange(node.Children);
            }

            return authorized;
        }

        private Boolean IsAuthorizedFor(Int32? accountId, String? area, String? controller, String? action)
        {
            return action == null || Authorization?.IsGrantedFor(accountId, area, controller, action) != false;
        }
        private List<SiteMapNode> Parse(XElement root, SiteMapNode? parent = null)
        {
            List<SiteMapNode> nodes = new List<SiteMapNode>();
            foreach (XElement element in root.Elements("siteMapNode"))
            {
                SiteMapNode node = new SiteMapNode();
                node.IconClass = (String)element.Attribute("icon");
                node.IsMenu = (Boolean?)element.Attribute("menu") == true;

                node.Route = ParseRoute(element);
                node.Action = (String)element.Attribute("action");
                node.Area = (String)element.Attribute("area") ?? parent?.Area;
                node.Controller = (String)element.Attribute("controller") ?? (element.Attribute("area") == null ? parent?.Controller : null);

                node.Title = Resource.ForSiteMap(node.Area, node.Controller, node.Action);
                node.Children = Parse(element, node);
                node.Parent = parent;

                nodes.Add(node);
            }

            return nodes;
        }
        private List<SiteMapNode> Flatten(IEnumerable<SiteMapNode> branches)
        {
            List<SiteMapNode> list = new List<SiteMapNode>();
            foreach (SiteMapNode branch in branches)
            {
                list.Add(branch);
                list.AddRange(Flatten(branch.Children));
            }

            return list;
        }
        private Dictionary<String, String> ParseRoute(XElement element)
        {
            return element
                .Attributes()
                .Where(attribute => attribute.Name.LocalName.StartsWith("route-"))
                .ToDictionary(attribute => attribute.Name.LocalName.Substring(6), attribute => attribute.Value);
        }
        private SiteMapNode? CurrentNodeFor(RouteValueDictionary route)
        {
            String? area = route["area"] as String;
            String? action = route["action"] as String;
            String? controller = route["controller"] as String;

            return Lookup.TryGetValue($"{area}/{controller}/{action}", out SiteMapNode? node) ? node : null;
        }
        private String FormUrl(IUrlHelper url, SiteMapNode node)
        {
            if (node.Action == null)
                return "#";

            Dictionary<String, Object?> route = new Dictionary<String, Object?>();
            ActionContext context = url.ActionContext;
            route["area"] = node.Area;

            foreach ((String key, String newKey) in node.Route)
                route[key] = context.RouteData.Values[newKey] ?? context.HttpContext.Request.Query[newKey];

            return url.Action(node.Action, node.Controller, route);
        }
        private Boolean IsEmpty(SiteMapNode node)
        {
            return node.Action == null && !node.Children.Any();
        }
    }
}
