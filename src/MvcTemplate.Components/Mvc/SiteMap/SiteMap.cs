using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MvcTemplate.Components.Extensions;
using MvcTemplate.Components.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MvcTemplate.Components.Mvc
{
    public class SiteMap : ISiteMap
    {
        private SiteMapNode[] Tree { get; }
        private IAuthorization Authorization { get; }
        private Dictionary<String, SiteMapNode> Lookup { get; }

        public SiteMap(String map, IAuthorization authorization)
        {
            Authorization = authorization;
            Tree = Parse(XElement.Parse(map));
            Lookup = Flatten(Tree).ToDictionary(node => node.Path!, StringComparer.OrdinalIgnoreCase);
        }

        public SiteMapNode[] For(ViewContext context)
        {
            Int64? account = context.HttpContext.User.Id();
            IUrlHelperFactory factory = context.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            SiteMapNode[] nodes = SetState(null, Tree, factory.GetUrlHelper(context), CurrentNodeFor(context.RouteData.Values));

            return Authorize(account, nodes);
        }
        public SiteMapNode[] BreadcrumbFor(ViewContext context)
        {
            IUrlHelperFactory factory = context.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            SiteMapNode? current = CurrentNodeFor(context.RouteData.Values);
            List<SiteMapNode> breadcrumb = new List<SiteMapNode>();
            IUrlHelper url = factory.GetUrlHelper(context);

            while (current != null)
            {
                if (current.Action != null)
                    breadcrumb.Insert(0, new SiteMapNode
                    {
                        Path = current.Path,
                        Url = FormUrl(url, current),
                        IconClass = current.IconClass
                    });

                current = current.Parent;
            }

            return breadcrumb.ToArray();
        }

        private SiteMapNode[] SetState(SiteMapNode? parent, IEnumerable<SiteMapNode> nodes, IUrlHelper url, SiteMapNode? current)
        {
            List<SiteMapNode> copies = new List<SiteMapNode>();

            foreach (SiteMapNode node in nodes)
            {
                SiteMapNode copy = new SiteMapNode();
                copy.IconClass = node.IconClass;
                copy.Url = FormUrl(url, node);
                copy.IsMenu = node.IsMenu;
                copy.Path = node.Path;
                copy.Parent = parent;

                copy.Controller = node.Controller;
                copy.Action = node.Action;
                copy.Area = node.Area;

                copy.IsActive = node == current;
                copy.Children = SetState(copy, node.Children, url, current);

                if (parent?.IsActive == false)
                    parent.IsActive = copy.IsActive;

                copies.Add(copy);
            }

            return copies.ToArray();
        }
        private SiteMapNode[] Authorize(Int64? accountId, IEnumerable<SiteMapNode> nodes)
        {
            List<SiteMapNode> authorized = new List<SiteMapNode>();

            foreach (SiteMapNode node in nodes)
            {
                node.Children = Authorize(accountId, node.Children);

                if (node.IsMenu && !IsEmpty(node) && Authorization.IsGrantedFor(accountId, node.Path!))
                    authorized.Add(node);
                else
                    authorized.AddRange(node.Children);
            }

            return authorized.ToArray();
        }

        private SiteMapNode[] Parse(XContainer root, SiteMapNode? parent = null)
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

                node.Path = $"{node.Area}/{node.Controller}/{node.Action}";
                node.Children = Parse(element, node);
                node.Parent = parent;

                nodes.Add(node);
            }

            return nodes.ToArray();
        }
        private List<SiteMapNode> Flatten(SiteMapNode[] branches)
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
            return node.Action == null && node.Children.Length == 0;
        }
    }
}
